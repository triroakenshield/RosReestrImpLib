// $Id: MiWrapper.cs,v 1.2 2005/03/24 17:02:06 dmorissette Exp $

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
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
    /// This is a helper class for our standard enumerator based on <see cref="EnumImpl"/>. Implementataions 
    /// (Features, Parts, Fields and Vertices) are array like structures that can provide
    /// an object at a given index between 0 and Count  - 1.
    /// </summary>
    interface IObjectProvider {

        /// <summary>���������� ��������</summary>
        int Count { get; }

        /// <summary>�������� ������</summary>
        /// <param name="idx">������ �������� � ���������</param>
        /// <returns></returns>
        object GetObj(int idx);
    }

    /// <summary>
    /// Implementation of an enumeration scheme over an index (array like) structure. 
    /// This class provides an enumerator that will work over any <see cref="IObjectProvider"/> implementation
    /// (Features, Parts, Fields and Vertices).
    /// </summary>
    /// <remarks>
    /// Calls to the GetEnumerator method of Fields, Parts and Vertices will return
    /// an instance of this class. Calls to GetEnumerator of Features will return a descendant
    /// of this class (due to the fact that features don't necessarily have a sequential
    /// index).
    /// </remarks>
    public class IndexedEnum : IEnumerator 
    {
        //public readonly int Count;

        protected int eIdx = -1;
        
        private readonly IObjectProvider _objProvider;

        /// <summary>�����������</summary>
        /// <param name="objProvider"></param>
        internal IndexedEnum(IObjectProvider objProvider) 
        {
            _objProvider = objProvider;
        }

        /// <inheritdoc />
        public virtual void Reset() 
        {
            eIdx = -1;
        }

        /// <inheritdoc />
        public object Current => _objProvider.GetObj(eIdx);

        /// <inheritdoc />
        public virtual bool MoveNext() => (++eIdx < _objProvider.Count);
    }

    /// <summary>Partial implementation of <see cref="IEnumerable"/> over an indexed (array like) structure.</summary>
    /// <remarks>
    /// Fields, Vertices, Parts and Features all descend from this class. It serves to
    /// provide the common functionality required to generate their enumerators.
    /// </remarks>
    public abstract class EnumImpl : IEnumerable, IObjectProvider 
    {
        /// ���� ��� <see cref="Count"/>
        protected internal int _count;

        //public int count { get { return this._count; } }

        /// <summary>�����������</summary>
        /// <param name="count"></param>
        protected EnumImpl(int count) { _count = count; }

        /// <inheritdoc />
        public int Count => _count;

        /// <inheritdoc />
        public virtual IEnumerator GetEnumerator() => new IndexedEnum(this);

        /// <inheritdoc />
        public abstract object GetObj(int idx);
    }

    /// <summary>Represents a single vertex with an X,Y point in geometric space.</summary>
    /// <remarks>
    /// This is the base building block of a feature
    /// </remarks>
    public class Vertex 
    {
        /// <summary>���������� �����</summary>
        public readonly double X, Y;

        /// <summary>�����������</summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc />
        public override string ToString() 
        {
            return $"{X}, {Y}";
        }
    }

    /// <summary>Contains the set of Vertices belonging to a single <see cref="Part"/>.</summary>
    /// <remarks>
    /// This class descends EnumImpl, meaning the vertices in the
    /// set can be iterated using foreach.
    /// It also has an index property allowing any vertice between 0 and Vertices.Count-1
    /// to be accessed directly with Vertices[idx]
    /// </remarks>
    public class Vertices : EnumImpl 
    {
        /// <summary>������ �� ������������ �������</summary>
        public readonly Part Part;

        /// <summary>�����������</summary>
        /// <param name="part">������������ �������</param>
        protected internal Vertices(Part part): base(MiApi.mitab_c_get_vertex_count(part.Feature.Handle, part.Index)) 
        {
            Part = part;
        }

        /// <summary>Override this to support descendants of the Vertex class.</summary>
        /// <returns>A vertex with the given X,Y coordinates</returns>
        protected internal virtual Vertex CreateVertex(double x, double y) => new Vertex(x, y);

        /// <summary>��������� ����� �� �������</summary>
        /// <param name="index">������ �����</param>
        /// <returns></returns>
        public virtual Vertex this[int index] =>
            index < Count ? CreateVertex(
                MiApi.mitab_c_get_vertex_x(Part.Feature.Handle, Part.Index, index),
                MiApi.mitab_c_get_vertex_y(Part.Feature.Handle, Part.Index, index)) : null;

        /// <inheritdoc />
        public override object GetObj(int idx) => this[idx]; 

        /// <inheritdoc />
        public override string ToString() 
        {
            var str = new StringBuilder();
            foreach (Vertex v in this) str.Append($"{v}\t");
            return str.ToString();
        }
    }

    /// <summary>Contains the set of Parts belonging to a single Feature.</summary>
    /// <remarks>This class descends EnumImple, meaning the Parts in the
    /// set can be iterated using foreach.
    /// It also has an index property allowing any Part between 0 and Parts.Count-1
    /// to be accessed directly with Parts[idx]
    /// </remarks>
    public class Parts :  EnumImpl 
    {
        /// <summary>������ �� ������������ �������</summary>
        protected internal Feature _feature;

        /// <summary>The feature these parts belong to.</summary>
        public Feature Feature => _feature;

        /// <summary>�����������</summary>
        /// <param name="feature"></param>
        /// <param name="nParts"></param>
        protected internal Parts(Feature feature, List<List<Vertex>> nParts) : base(nParts.Count)
        {
            _feature = feature;
            for (var i=0; i < Count; i++)
            {
                var _ = new Part(feature, i, nParts[i]);
            }
        }

        /// <summary>�����������</summary>
        /// <param name="feature"></param>
        protected internal Parts(Feature feature):base(MiApi.mitab_c_get_parts(feature.Handle)) 
        {
            _feature = feature;
        }

        /// <summary>Override this to support descendants of the Part class.</summary>
        /// <returns>A part with the given index</returns>
        protected internal virtual Part CreatePart(int partIdx) => new Part(Feature, partIdx);

        /// <summary>��������� ����� ��������</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Part this[int index] => index < Count ? CreatePart(index) : null;

        /// <inheritdoc />
        public override object GetObj(int idx) => this[idx]; 

        /// <inheritdoc />
        public override string ToString() 
        {
            var str = new StringBuilder();
            str.Append($"Part Count: {Count}\n");
            foreach (Part part in this) str.Append($"{part}\n");
            return str.ToString();
        }
    }

    /// <summary>
    /// Unlike the other enumerators. The feature id set isn't guaranteed to be sequential. 
    /// So we override the default seqeuntial iterator.
    /// </summary>
    internal class FeaturesEnum : IndexedEnum 
    {
        private readonly MiLayer _layer;

        /// <summary>�����������</summary>
        /// <param name="objProvider"></param>
        /// <param name="layer"></param>
        internal FeaturesEnum(IObjectProvider objProvider, MiLayer layer) : base(objProvider) 
        {
            _layer = layer;
        }

        /// <inheritdoc />
        public override bool MoveNext() => (eIdx = MiApi.mitab_c_next_feature_id(_layer.Handle, eIdx)) != -1;
    }

    /// <summary>Contains the set of features belonging to a single layer.</summary>
    /// <remarks>This class descends EnumImpl, meaning the features in the
    /// set can be iterated using foreach.
    /// It also has an index property allowing any feature between 0 and Features.Count-1
    /// to be accessed directly with Features[idx]</remarks>
    public class Features : EnumImpl 
    {
        /// <summary>The layer the features belong to</summary>
        public readonly MiLayer Layer;

        /// <summary>����������</summary>
        /// <param name="layer"></param>
        protected internal Features(MiLayer layer) : base(MiApi.mitab_c_get_feature_count(layer.Handle)) 
        {
            Layer = layer;
        }

        /// <summary>���������� ��������</summary>
        /// <param name="type">��� ��������</param>
        /// <param name="nid"></param>
        /// <param name="nParts">������ ������ ��� ���������</param>
        /// <param name="nFieldValues">������ �������� �����</param>
        /// <param name="nStyle">������� ������</param>
        /// <returns></returns>
        public Feature AddFeature(FeatureType type, int nid, List<List<Vertex>> nParts, List<String> nFieldValues, 
            Dictionary<string, string> nStyle)
        {
            return new Feature(Layer, nid, type, nParts, nFieldValues, nStyle);
        }

        /// <summary>Override this to support descendants of the <see cref="Feature"/> class.</summary>
        /// <returns>This layers fields</returns>
        protected internal virtual Feature CreateFeature(int index) => new Feature(Layer, index);

        /// <summary>��������� ��������</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Feature this[int index] => (index != -1) ? CreateFeature(index) : null;

        /// <summary>��������� ������ �������� � ���������</summary>
        /// <returns></returns>
        public Feature GetFirst() => this[MiApi.mitab_c_next_feature_id(Layer.Handle, -1)];

        /// <inheritdoc />
        public override object GetObj(int idx) => this[idx];

        /// <inheritdoc />
        public override IEnumerator GetEnumerator() => new FeaturesEnum(this, Layer);

        /// <inheritdoc />
        public override string ToString() {
            var str = new StringBuilder();
            str.Append($"Feature Count: {Count}\n");
            foreach (Feature feature in this) str.Append($"{feature}\n");
            return str.ToString();
        }
    }
}