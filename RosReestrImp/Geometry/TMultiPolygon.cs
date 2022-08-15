using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary>Мултиполигон</summary>
    public class TMultiPolygon : TGeometryCollection
    {
        /// <summary></summary>
        public new static readonly string Type = "MULTIPOLYGON";

        /// <summary>Конструктор из списка</summary>
        /// <param name="nPolygons"></param>
        public TMultiPolygon(List<TPolygon> nPolygons) //: base((List<TGeometry>)nPolygons.Cast<TGeometry>())
        {
            Geometries.AddRange(nPolygons.GetRange(0, nPolygons.Count)); 
        }

        /// <inheritdoc/>
        public override bool IsEmpty() => Geometries is not { Count: > 0 };

        /// <inheritdoc/>
        public override GeometryType GetGeometryType() => GeometryType.MultiPolygon;

        /// <inheritdoc/>
        public override double[] GetXYArray() => null;

        /// <inheritdoc/>
        public override double[] GetZArray() => null;

        /// <inheritdoc/>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (var p in Geometries)
            {
                if (res == null) res = p.GetMBR();
                else res.AddMBR(p.GetMBR());
            }
            return res;
        }

        /// <inheritdoc/>
        public override bool IsValid()
        {
            if (Geometries == null) return false;
            return Geometries.Count > 0 && Geometries.All(ls => ls.IsValid());
        }

        /// <inheritdoc/>
        public override string ToShortWKT2D() 
            => string.Join(", ", Geometries.Select(p => $"({p.ToShortWKT2D()})"));

        /// <inheritdoc/>
        public override string ToWKT2D()  => IsEmpty() ? $"{Type} {Emp}" : $"{Type}({ToShortWKT2D()})";
    }
}