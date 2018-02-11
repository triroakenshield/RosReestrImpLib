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
        public override string ToShortWKT2D()
        {
            return String.Join(", ", this.Rings.Select(p => 
                String.Format("({0})", p.RingToShortWKT2D())));
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - Polygon((x0 y0, x1 y1, ..., xn yn, x0 y0), ..., (...))
        /// </summary>
        /// <returns></returns>
        public override string ToWKT2D()
        {
            return String.Format("Polygon({0})", this.ToShortWKT2D());
        }

        /// <summary>
        /// Тип геометрии, всегда возвращает - TGeometry.GeometryType.Polygon
        /// </summary>
        /// <returns> TGeometry.GeometryType.Polygon </returns>
        public override TGeometry.GeometryType GetGeometryType()
        {
            return TGeometry.GeometryType.Polygon;
        }

    }

}
