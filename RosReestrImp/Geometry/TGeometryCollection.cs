using System;
using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    public class TGeometryCollection : TGeometry
    {        
        /// <summary></summary>
        public new static readonly string Type = "GEOMETRYCOLLECTION";

        /// <summary>Список</summary>
        public List<TGeometry> Geometries;

        public TGeometryCollection()
        {
            this.Geometries = new List<TGeometry>();
        }

        public TGeometryCollection(List<TGeometry> nGeometries)
        {
            this.Geometries = nGeometries.GetRange(0, nGeometries.Count);
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
            return GeometryType.GeometryCollection;
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
            if (this.IsEmpty()) return $"{TGeometryCollection.Type} {TGeometry.Emp}";
            return $"{TGeometryCollection.Type}({this.ToShortWKT2D()})";
        }
    }
}