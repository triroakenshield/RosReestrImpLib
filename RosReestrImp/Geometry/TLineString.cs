using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary>Внутренний формат для представления геометрии - линии</summary>
    public class TLineString : TGeometry
    {
        /// <summary>Тип</summary>
        public new static readonly string Type = "LINESTRING";

        /// <summary>Список координат</summary>
        public List<MyPoint> Coords;

        /// <summary>Создание линии из списка координат</summary>
        /// <param name="nCoords"> Список координат </param>
        public TLineString(List<MyPoint> nCoords)
        {
            this.Coords = nCoords.GetRange(0, nCoords.Count);
        }

        /// <summary>Создание линии из списка точек</summary>
        /// <param name="nCoords"> Список точек </param>
        public TLineString(List<TPoint> nCoords)
        {
            this.Coords = new List<MyPoint>();
            nCoords.ForEach(p => this.Coords.Add(new MyPoint(p.Coord)));
        }

        ///<inheritdoc/>
        public override bool IsEmpty()
        {
            return this.Coords.Count == 0;
        }

        ///<inheritdoc/>
        public override string ToShortWKT2D()
        {
            return string.Join(", ", this.Coords.Select(p => p.ToWKT2D()));
        }

        /// <summary>Получение короткой wkt-строки (2D) для кольца (без типа)</summary>
        /// <returns></returns>
        public string RingToShortWKT2D()
        {
            return $"{ToShortWKT2D()}, {this.Coords[0].ToWKT2D()}";
        }

        /// <summary>Получение геометрии в виде wkt-строки (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0])  </summary>
        /// <returns> wkt-строка (2D) - LineString(x0 y0, x1 y1, ..., xn yn[, x0 y0]) </returns>
        public override string ToWKT2D()
        {
            return this.IsEmpty() ? $"{TLineString.Type} {TGeometry.Emp}" : $"{TLineString.Type}({this.ToShortWKT2D()})";
        }

        /// <summary>Получение wkt-строки (2D) для кольца</summary>
        /// <returns></returns>
        public string RingToWKT2D()
        {
            return this.IsEmpty() ? $"{TLineString.Type} {TGeometry.Emp}" : $"{TLineString.Type}({this.RingToShortWKT2D()})";
        }

        /// <summary>Тип геометрии, всегда возвращает - TGeometry.GeometryType.LineString</summary>
        /// <returns> TGeometry.GeometryType.LineString </returns>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.LineString;
        }

        ///<inheritdoc/>
        public override double[] GetXYArray()
        {
            double[] arr = new double[this.Coords.Count*2];
            for (var i = 0; i < this.Coords.Count; i++)
            {
                arr[i * 2] = this.Coords[i].X;
                arr[i * 2 + 1] = this.Coords[i].Y;
            }
            return arr;
        }

        ///<inheritdoc/>
        public override double[] GetZArray()
        {
            double[] arr = new double[this.Coords.Count];
            for (var i = 0; i < this.Coords.Count; i++)
            {
                arr[i] = this.Coords[i].Z;
            }
            return arr;
        }

        ///<inheritdoc/>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (MyPoint p in this.Coords)
            {
                if (res == null) res = new TMBR(p);
                else res.AddPoint(p); 
            }
            return res;
        }

        ///<inheritdoc/>
        public override bool IsValid()
        {
            return this.Coords.Count > 1;
        }
    }
}