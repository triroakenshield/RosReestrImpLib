using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITAB
{
    /// <summary>
    /// Represents a Part.
    /// </summary>
    /// <remarks>A feature will contain one or more parts.</remarks>
    public class Part
    {
        protected internal Vertices _vertices;

        public readonly Feature Feature;
        public Vertices Vertices { get { return this._vertices; } }
        public readonly int Index;

        protected internal Part(Feature feature, int partIdx, List<Vertex> nVertices)
        {
            this.Index = partIdx;
            this.Feature = feature;            
            this.SetPoints(nVertices);
            this._vertices = CreateVertices(this);
        }

        protected internal Part(Feature feature, int partIdx)
        {
            this.Index = partIdx;
            this.Feature = feature;
            this._vertices = CreateVertices(this);
        }

        /// <summary>
        /// Override this to support descendants of the Vertices class.
        /// </summary>
        /// <returns>This parts vertices.</returns>
        protected internal virtual Vertices CreateVertices(Part part)
        {
            return new Vertices(this);
        }

        protected internal void SetPoints(List<Vertex> nVertices)
        {
            int count = nVertices.Count;
            double[] x = new double[count];
            double[] y = new double[count];
            Vertex v;
            for (int i=0; i < count; i++)
            {
                v = nVertices[i];
                x[i] = v.X;
                y[i] = v.Y;
            }
            MiApi.mitab_c_set_points(this.Feature.Handle, this.Index, count, x, y);
        }

        public override string ToString()
        {
            return $"Part: {Index}\nVertices:\n{this.Vertices.ToString()}";
        }

    }
}
