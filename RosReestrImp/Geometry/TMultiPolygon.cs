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

        public new bool IsEmpty() => Geometries == null || Geometries.Count <= 0;

        public new GeometryType GetGeometryType()
        {
            return GeometryType.MultiPolygon;
        }

        public new double[] GetXYArray() => null;

        public new double[] GetZArray() => null;

        public new TMBR GetMBR()
        {
            TMBR res = null;
            foreach (var p in Geometries)
            {
                if (res == null) res = p.GetMBR();
                else res.AddMBR(p.GetMBR());
            }
            return res;
        }

        public new bool IsValid()
        {
            if (Geometries == null) return false;
            return Geometries.Count > 0 && Geometries.All(ls => ls.IsValid());
        }

        public new string ToShortWKT2D() => string.Join(", ", Geometries.Select(p => $"({p.ToShortWKT2D()})"));

        public new string ToWKT2D()  => IsEmpty() ? $"{TMultiPolygon.Type} {TGeometry.Emp}" : $"{TMultiPolygon.Type}({ToShortWKT2D()})";
    }
}