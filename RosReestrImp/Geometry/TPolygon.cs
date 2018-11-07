using System;
using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    /// <summary>
    /// Внутрений формат для представления геометрии - полигона
    /// </summary>
    public class TPolygon : TGeometry
    {

        /// <summary>
        /// 
        /// </summary>
        public new static readonly string Type = "POLYGON";

        /// <summary>
        /// Список замкнутых контуров - колец
        /// </summary>
        public List<TLineString> Rings;

        /// <summary>
        /// Создание полигона
        /// </summary>
        /// <param name="nRings"> Список контуров </param>
        public TPolygon(List<TLineString> nRings)
        {
            this.Rings = nRings.GetRange(0, nRings.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return this.Rings.Count == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToShortWKT2D()
        {
            return String.Join(", ", this.Rings.Select(p => $"({p.RingToShortWKT2D()})"));
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - Polygon((x0 y0, x1 y1, ..., xn yn, x0 y0), ..., (...))
        /// </summary>
        /// <returns></returns>
        public override string ToWKT2D()
        {
            if (this.IsEmpty()) return $"{TPolygon.Type} {TGeometry.Emp}";
            return $"{TPolygon.Type}({this.ToShortWKT2D()})";
        }

        /// <summary>
        /// Тип геометрии, всегда возвращает - TGeometry.GeometryType.Polygon
        /// </summary>
        /// <returns> TGeometry.GeometryType.Polygon </returns>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.Polygon;
        }

        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (TLineString p in this.Rings)
            {
                if (res == null) res = p.GetMBR();
                else { res.AddMBR(p.GetMBR()); }
            }
            return res;
        }

        public override bool IsValid()
        {
            if (this.Rings.Count > 0)
            {
                foreach (TLineString ls in this.Rings)
                {
                    if (!ls.IsValid()) return false;
                }
                return true;
            }
            return false;

        }
    }

}
