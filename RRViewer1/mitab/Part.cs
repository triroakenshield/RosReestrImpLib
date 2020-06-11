using System.Collections.Generic;

namespace MITAB
{
    /// <summary>Represents a Part.</summary>
    /// <remarks>A feature will contain one or more parts.</remarks>
    public sealed class Part
    {
        /// <summary>Вершины</summary>
        internal Vertices _vertices;

        /// <summary>Ссылка на родительскую сущность</summary>
        public readonly Feature Feature;

        /// <summary>Вершины</summary>
        public Vertices Vertices => this._vertices;

        /// <summary>Индекс</summary>
        public readonly int Index;

        /// <summary>Конструктор</summary>
        /// <param name="feature">Родительская сущность</param>
        /// <param name="partIdx">Номер части</param>
        /// <param name="nVertices">Список вершин</param>
        internal Part(Feature feature, int partIdx, List<Vertex> nVertices)
        {
            this.Index = partIdx;
            this.Feature = feature;            
            this.SetPoints(nVertices);
            this._vertices = CreateVertices(this);
        }

        /// <summary>Конструктор</summary>
        /// <param name="feature">Родительская сущность</param>
        /// <param name="partIdx">Номер части</param>
        internal Part(Feature feature, int partIdx)
        {
            this.Index = partIdx;
            this.Feature = feature;
            this._vertices = CreateVertices(this);
        }

        /// <summary>Override this to support descendants of the Vertices class.</summary>
        /// <returns>This parts vertices.</returns>
        internal Vertices CreateVertices(Part part)
        {
            return new Vertices(this);
        }

        /// <summary>Установка значений координат вершин</summary>
        /// <param name="nVertices">Список новых значений вершин</param>
        internal void SetPoints(List<Vertex> nVertices)
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

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Part: {Index}\nVertices:\n{this.Vertices}";
        }
    }
}