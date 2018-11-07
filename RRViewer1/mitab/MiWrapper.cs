// $Id: MiWrapper.cs,v 1.2 2005/03/24 17:02:06 dmorissette Exp $
//

using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MITAB
{

	/*
	 * These classes wrap the mitab c api functions to produce a hierarchy of classes giving readonly 
	 * access to the feature points.
	 * 
	 * Requires mitab.dll (www.maptools.org)
	 * See http://mitab.maptools.org/
	 * 
	 * Graham Sims,
	 * Environment Bay of Plenty, Whakatane, New Zealand
	 * http://www.envbop.govt.nz		
	 */

	/// <summary>
	/// This is a helper class for our standard enumerator based on EnumImpl. Implementataions 
	/// (Features, Parts, Fields and Vertices) are array like structures that can provide
	/// an object at a given index between 0 and Count  - 1.
	/// </summary>
	interface IObjectProvider {
		int Count {
			get;
		}
		object GetObj(int idx);
	}

	/// <summary>
	/// Implementation of an enumeration scheme over an index (array like) structure. 
	/// This class provides an enumerator that will work over any IObjectProvider implementation
	/// (Features, Parts, Fields and Vertices).
	/// </summary>
	/// <remarks>
	/// Calls to the GetEnumerator method of Fields, Parts and Vertices will return
	/// an instance of this class. Calls to GetEnumerator of Features will return a descendant
	/// of this class (due to the fact that features don't necessarily have a sequential
	/// index).
	/// </remarks>
	public class IndexedEnum : IEnumerator {
		public readonly int Count;
		protected int eIdx = -1;
		private readonly IObjectProvider objProvider;

		internal IndexedEnum(IObjectProvider objProvider) {
			this.objProvider = objProvider;
		}

		public virtual void Reset() {
			eIdx = -1;
		}

		public object Current {
			get {
				return objProvider.GetObj(eIdx);
			}
		}

		public virtual bool MoveNext() {
			return (++eIdx < objProvider.Count);
		}
	}

	/// <summary>
	/// Partial implementation of IEnumerable over an indexed (aray like) structure.
	/// </summary>
	/// <remarks>
	/// Fields, Vertices, Parts and Features all descend from this class. It serves to
	/// provide the common functionality required to generate their enumerators.
	/// </remarks>
	public abstract class EnumImpl : IEnumerable, IObjectProvider {
        protected internal int _count = 0;

        //public int count { get { return this._count; } }

		protected EnumImpl(int count) {
			this._count = count;
		}

		public int Count {
			get {
                return this._count;
			}
		}

		public virtual IEnumerator GetEnumerator() {
			return new IndexedEnum(this);
		}

		public abstract object GetObj(int idx);
	}  	
    
	/// <summary>
	/// Represents a single vertex with an X,Y point in geometric space.
	/// </summary>
	/// <remarks>
	/// This is the base building block of a feature
	/// </remarks>
	public class Vertex {
		public readonly double X,Y;
        public Vertex(double x, double y) {
			this.X = x;
			this.Y = y;
		}
		
		public override string ToString() {
			return $"{this.X}, {this.Y}";
		}
	}

	/// <summary>
	/// Contains the set of Vertices belonging to a single Part.
	/// </summary>
	/// <remarks>
	/// This class descends EnumImpl, meaning the vertices in the
	/// set can be iterated using foreach.
	/// It also has an index property allowing any vertice between 0 and Vertices.Count-1
	/// to be accessed directly with Vertices[idx]
	/// </remarks>
	public class Vertices : EnumImpl {
		public readonly Part Part;

		protected internal Vertices(Part part):
			base(MiApi.mitab_c_get_vertex_count(part.Feature.Handle, part.Index)) {
			this.Part = part;
		}

		/// <summary>
		/// Override this to support descendants of the Vertex class.
		/// </summary>
		/// <returns>A vertex with the given X,Y coordinates</returns>
		protected internal virtual Vertex CreateVertex(double x, double y) {
			return new Vertex(x ,y);
		}

		public virtual Vertex this[int index] {
			get {
				return index < Count ? CreateVertex(
					MiApi.mitab_c_get_vertex_x(Part.Feature.Handle, Part.Index, index),
					MiApi.mitab_c_get_vertex_y(Part.Feature.Handle, Part.Index, index)) : null;
			}
		}

		public override object GetObj(int idx) {
			return this[idx];
		}
		
		public override string ToString() {
			StringBuilder str = new StringBuilder();
			foreach (Vertex v in this)
				str.Append(v+"\t");
			return str.ToString();
		}
	}	

	/// <summary>
	/// Contains the set of Parts belonging to a single Feature.
	/// </summary>
	/// <remarks>This class descends EnumImple, meaning the Parts in the
	/// set can be iterated using foreach.
	/// It also has an index property allowing any Part between 0 and Parts.Count-1
	/// to be accessed directly with Parts[idx]
	/// </remarks>
	public class Parts :  EnumImpl {

        protected internal Feature _feature;

        /// <summary>
        /// The feature these parts belong to.
        /// </summary>
        public Feature Feature { get { return this._feature; } }

        protected internal Parts(Feature feature, List<List<Vertex>> nParts) : base(nParts.Count)
        {
            this._feature = feature;
            for (int i=0; i < this.Count; i++)
            {
                new Part(feature, i, nParts[i]);
            }
        }

        protected internal Parts(Feature feature):base(MiApi.mitab_c_get_parts(feature.Handle)) {
			this._feature = feature;
		}

		/// <summary>
		/// Override this to support descendants of the Part class.
		/// </summary>
		/// <returns>A part with the given index</returns>
		protected internal virtual Part CreatePart(int partIdx) {
			return new Part(this.Feature, partIdx);
		}

		public Part this[int index] {
			get {
				return index < Count ? CreatePart(index) : null;
			}
		}


		public override object GetObj(int idx) {
			return this[idx];
		}

		public override string ToString() {
			StringBuilder str = new StringBuilder();
			str.Append("Part Count: "+this.Count+"\n");
			foreach (Part part in this) 
				str.Append(part.ToString()+"\n");
			return str.ToString();
		}

	}

	/// <summary>
	/// Unlike the other enumerators. The feature id set isn't guaranteed to be sequential. 
	/// So we override the default seqeuntial iterator.
	/// </summary>
	internal class FeaturesEnum : IndexedEnum {

		private readonly MiLayer layer;

		internal FeaturesEnum(IObjectProvider objProvider, MiLayer layer) : base(objProvider) {
			this.layer = layer;
		}

		public override bool MoveNext() {
			return (eIdx = MiApi.mitab_c_next_feature_id(layer.Handle, eIdx)) != -1;
		}
	}

	/// <summary>
	/// Contains the set of features belonging to a single layer.
	/// </summary>
	/// <remarks>This class descends EnumImpl, meaning the features in the
	/// set can be iterated using foreach.
	/// It also has an index property allowing any feature between 0 and Features.Count-1
	/// to be accessed directly with Features[idx]</remarks>
	public class Features : EnumImpl {
		/// <summary>
		/// The layer the features belong to
		/// </summary>
		public readonly MiLayer Layer;

		protected internal Features(MiLayer layer) : 
			base(MiApi.mitab_c_get_feature_count(layer.Handle)) {
			this.Layer = layer;
		}

        public Feature AddFeature(FeatureType type, int nid, List<List<Vertex>> nParts, List<String> nFieldValues, Dictionary<string, string> nStyle)
        {
            return new Feature(this.Layer, nid, type, nParts, nFieldValues, nStyle);
        }

        /// <summary>
        /// Override this to support descendants of the Feature class.
        /// </summary>
        /// <returns>This layers fields</returns>
        protected internal virtual Feature CreateFeature(int index) {
			return new Feature(this.Layer, index);
		}

		public Feature this[int index] {
			get {
				return (index != -1) ? CreateFeature(index) : null;
			}
		}

		public Feature GetFirst() {
			return this[MiApi.mitab_c_next_feature_id(Layer.Handle, -1)];
		}

		public override object GetObj(int idx) {
			return this[idx];
		}

		public override IEnumerator GetEnumerator() {
			return new FeaturesEnum(this, Layer);
		}

        public override string ToString() {
			StringBuilder str = new StringBuilder();
			str.Append("Feature Count: "+this.Count+"\n");
			foreach (Feature feature in this) 
				str.Append(feature.ToString()+"\n");
			return str.ToString();
		}

	}
    
}
