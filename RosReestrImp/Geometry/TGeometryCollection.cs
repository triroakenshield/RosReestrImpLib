using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    /// <summary>Абстрактная коллекция геометрии</summary>
    public class TGeometryCollection : TGeometry
    {

        ///<inheritdoc/>
        public new static readonly string Type = "GEOMETRYCOLLECTION";

        /// <summary>Список</summary>
        public List<TGeometry> Geometries;

        /// <summary>Конструктор</summary>
        public TGeometryCollection()
        {
            this.Geometries = new List<TGeometry>();
        }

        /// <summary>Конструктор из списка</summary>
        /// <param name="nGeometries"></param>
        public TGeometryCollection(List<TGeometry> nGeometries)
        {
            this.Geometries = nGeometries.GetRange(0, nGeometries.Count);
        }

        ///<inheritdoc/>
        public override bool IsEmpty()
        {
            if (this.Geometries == null) return true;
            if (this.Geometries.Count > 0) return false;
            return true;
        }

        ///<inheritdoc/>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.GeometryCollection;
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
            if (this.Geometries.Count > 0) return this.Geometries.All(ls => ls.IsValid());
            return false;
        }

        ///<inheritdoc/>
        public override string ToShortWKT2D()
        {
            return string.Join(", ", this.Geometries.Select(p => $"({p.ToShortWKT2D()})"));
        }

        ///<inheritdoc/>
        public override string ToWKT2D()
        {
            return this.IsEmpty() ? $"{TGeometryCollection.Type} {TGeometry.Emp}" 
                : $"{TGeometryCollection.Type}({this.ToShortWKT2D()})";
        }
    }
}