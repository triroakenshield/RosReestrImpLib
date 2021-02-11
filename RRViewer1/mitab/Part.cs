using System.Collections.Generic;
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable CheckNamespace

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
        public Vertices Vertices => _vertices;

        /// <summary>Индекс</summary>
        public readonly int Index;

        /// <summary>Конструктор</summary>
        /// <param name="feature">Родительская сущность</param>
        /// <param name="partIdx">Номер части</param>
        /// <param name="nVertices">Список вершин</param>
        internal Part(Feature feature, int partIdx, List<Vertex> nVertices)
        {
            Index = partIdx;
            Feature = feature;            
            SetPoints(nVertices);
            _vertices = CreateVertices(this);
        }

        /// <summary>Конструктор</summary>
        /// <param name="feature">Родительская сущность</param>
        /// <param name="partIdx">Номер части</param>
        internal Part(Feature feature, int partIdx)
        {
            Index = partIdx;
            Feature = feature;
            _vertices = CreateVertices(this);
        }

        /// <summary>Override this to support descendants of the Vertices class.</summary>
        /// <returns>This parts vertices.</returns>
        internal Vertices CreateVertices(Part part) => new Vertices(this);

        /// <summary>Установка значений координат вершин</summary>
        /// <param name="nVertices">Список новых значений вершин</param>
        internal void SetPoints(List<Vertex> nVertices)
        {
            var count = nVertices.Count;
            var x = new double[count];
            var y = new double[count];
            for (var i = 0; i < count; i++)
            {
                var v = nVertices[i];
                x[i] = v.X;
                y[i] = v.Y;
            }
            MiApi.mitab_c_set_points(Feature.Handle, Index, count, x, y);
        }

        /// <inheritdoc />
        public override string ToString() => $"Part: {Index}\nVertices:\n{Vertices}";
    }
}