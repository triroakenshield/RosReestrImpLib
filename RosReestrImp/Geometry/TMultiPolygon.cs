using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    /// <summary>Мултиполигон</summary>
    public class TMultiPolygon : TGeometryCollection
    {
        ///<inheritdoc/>
        public new static readonly string Type = "MULTIPOLYGON";

        /// <summary>Конструктор из списка</summary>
        /// <param name="nPolygons"></param>
        public TMultiPolygon(List<TPolygon> nPolygons) //: base((List<TGeometry>)nPolygons.Cast<TGeometry>())
        {
            this.Geometries.AddRange(nPolygons.GetRange(0, nPolygons.Count)); 
        }

        ///<inheritdoc/>
        public override bool IsEmpty()
        {
            return Geometries == null || Geometries.Count <= 0;
        }

        ///<inheritdoc/>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.MultiPolygon;
        }

        ///<inheritdoc/>
        public override double[] GetXYArray()
        {
            return null;
        }

        ///<inheritdoc/>
        public override double[] GetZArray()
        {
            return null;
        }

        ///<inheritdoc/>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (TGeometry p in this.Geometries)
            {
                if (res == null) res = p.GetMBR();
                else res.AddMBR(p.GetMBR());
            }
            return res;
        }

        ///<inheritdoc/>
        public override bool IsValid()
        {
            if (this.Geometries == null) return false;
            return this.Geometries.Count > 0 && this.Geometries.All(ls => ls.IsValid());
        }

        ///<inheritdoc/>
        public override string ToShortWKT2D()
        {
            return string.Join(", ", this.Geometries.Select(p => $"({p.ToShortWKT2D()})"));
        }

        ///<inheritdoc/>
        public override string ToWKT2D()
        {
            return this.IsEmpty() ? $"{TMultiPolygon.Type} {TGeometry.Emp}" : $"{TMultiPolygon.Type}({this.ToShortWKT2D()})";
        }
    }
}