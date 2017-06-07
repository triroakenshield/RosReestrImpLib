using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Получение геометрии в виде wkt-строки (2D) - Polygon((x0 y0, x1 y1, ..., xn yn, x0 y0), ..., (...))
        /// </summary>
        /// <returns></returns>
        public override string ToWKT2D()
        {
            //string workStr = "Polygon(";
            string workStr1 = "Polygon(";
            foreach (TLineString ls in this.Rings)
            {
                //workStr1 = "Polygon(";
                foreach (TGeometry.MyPoint p in ls.Coords)
                {
                    workStr1 = String.Concat(workStr1, p.X, " ", p.Y, ", ");
                }
                workStr1 = String.Concat(workStr1, ls.Coords[0].X, " ", ls.Coords[0].Y, ")");

            }

            return String.Concat(workStr1, ")");
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
