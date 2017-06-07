using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosReestrImp.Geometry
{
    /// <summary>
    /// Внутрений формат для представления геометрии - линии
    /// </summary>
    public class TLineString : TGeometry
    {
        /// <summary>
        /// Список координат
        /// </summary>
        public List<TGeometry.MyPoint> Coords;

        /// <summary>
        /// Создание линии из списка координат
        /// </summary>
        /// <param name="nCoords"> Список координат </param>
        public TLineString(List<TGeometry.MyPoint> nCoords)
        {
            this.Coords = nCoords.GetRange(0, nCoords.Count);
        }

        /// <summary>
        /// Создание линии из списка точек
        /// </summary>
        /// <param name="nCoords"> Список точек </param>
        public TLineString(List<Geometry.TPoint> nCoords)
        {
            this.Coords = new List<TGeometry.MyPoint>();
            foreach (Geometry.TPoint p in nCoords)
            {
                this.Coords.Add(new TGeometry.MyPoint(p.Coord));
            }
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0])  
        /// </summary>
        /// <returns> wkt-строка (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0]) </returns>
        public override string ToWKT2D()
        {
            string workStr = "LineString(";
            foreach (TGeometry.MyPoint p in this.Coords)
            {
                workStr = String.Concat(workStr, p.X, " ", p.Y, ", ");
            }
            workStr = String.Concat(workStr, this.Coords[0].X, " ", this.Coords[0].Y, ")");
            return String.Concat(workStr, ")");
        }

        /// <summary>
        /// Тип геометрии, всегда возвращает - TGeometry.GeometryType.LineString
        /// </summary>
        /// <returns> TGeometry.GeometryType.LineString </returns>
        public override TGeometry.GeometryType GetGeometryType()
        {
            return TGeometry.GeometryType.LineString;
        }

    }
}
