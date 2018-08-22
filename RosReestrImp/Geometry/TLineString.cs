using System;
using System.Collections.Generic;
using System.Linq;

namespace RosReestrImp.Geometry
{
    /// <summary>
    /// Внутрений формат для представления геометрии - линии
    /// </summary>
    public class TLineString : TGeometry
    {

        public new static readonly string Type = "LINESTRING";

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
            nCoords.ForEach(p => this.Coords.Add(new TGeometry.MyPoint(p.Coord)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToShortWKT2D()
        {
            return String.Join(", ", this.Coords.Select(p => p.ToWKT2D()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string RingToShortWKT2D()
        {
            return $"{ToShortWKT2D()}, {this.Coords[0].ToWKT2D()}";
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0])  
        /// </summary>
        /// <returns> wkt-строка (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0]) </returns>
        public override string ToWKT2D()
        {
            return $"{TLineString.Type}({this.ToShortWKT2D()})";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string RingToWKT2D()
        {
            return $"{TLineString.Type}({this.RingToShortWKT2D()})";
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
