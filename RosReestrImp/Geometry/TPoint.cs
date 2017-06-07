using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RosReestrImp.Geometry
{
    /// <summary>
    /// Внутрений формат для представления геометрии - точки
    /// </summary>
    public class TPoint : TGeometry
    {
        /// <summary>
        /// Координаты точки (MyPoint)
        /// </summary>
        public TGeometry.MyPoint Coord;

        /// <summary>
        /// Создание 3D точки
        /// </summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        /// <param name="nZ"> Координата Z </param>
        public TPoint(double nX, double nY, double nZ)
        {
            this.Coord.X = nX;
            this.Coord.Y = nY;
            this.Coord.Z = nZ;
        }

        /// <summary>
        /// Создание 2D точки (Z = 0)
        /// </summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        public TPoint(double nX, double nY)
        {
            this.Coord.X = nX;
            this.Coord.Y = nY;
            this.Coord.Z = 0;
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - POINT(X Y)
        /// </summary>
        /// <returns> wkt-строка (2D) - POINT(X Y) </returns>
        public override string ToWKT2D()
        {
            return String.Concat("POINT(", Coord.X, " ", Coord.Y, ")");
        }

        /// <summary>
        /// Тип геометрии, всегда возвращает - TGeometry.GeometryType.Point
        /// </summary>
        /// <returns> TGeometry.GeometryType.Point </returns>
        public override TGeometry.GeometryType GetGeometryType()
        {
            return TGeometry.GeometryType.Point;
        }

    }

}
