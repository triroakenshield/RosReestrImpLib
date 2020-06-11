using System;
using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    public class TMultiPolygon : TGeometryCollection
    {     
        public new static readonly string Type = "MULTIPOLYGON";

        public TMultiPolygon(List<TPolygon> nPolygons) //: base((List<TGeometry>)nPolygons.Cast<TGeometry>())
        {
            this.Geometries.AddRange(nPolygons.GetRange(0, nPolygons.Count)); 
        }

        public override bool IsEmpty()
        {
            if (this.Geometries != null)
            {
                if (this.Geometries.Count > 0) return false;
            }
            return true;
        }

        public override GeometryType GetGeometryType()
        {
            return GeometryType.MultiPolygon;
        }

        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (TGeometry p in this.Geometries)
            {
                if (res == null) res = p.GetMBR();
                else { res.AddMBR(p.GetMBR()); }
            }
            return res;
        }

        public override bool IsValid()
        {
            if (this.Geometries != null)
            {
                if (this.Geometries.Count > 0)
                {
                    foreach (TGeometry ls in this.Geometries)
                    {
                        if (!ls.IsValid()) return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public override string ToShortWKT2D()
        {
            return String.Join(", ", this.Geometries.Select(p => $"({p.ToShortWKT2D()})"));
        }

        public override string ToWKT2D()
        {
            if (this.IsEmpty()) return $"{TMultiPolygon.Type} {TGeometry.Emp}";
            return $"{TMultiPolygon.Type}({this.ToShortWKT2D()})";
        }
    }
}