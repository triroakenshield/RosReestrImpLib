using System;

namespace RosReestrImp.Geometry
{
    /// <summary>
    /// Внутрений формат для представления геометрии - точки
    /// </summary>
    public class TPoint : TGeometry
    {

        /// <summary>
        /// 
        /// </summary>
        public new static readonly string Type = "POINT";

        /// <summary>
        /// Координаты точки (MyPoint)
        /// </summary>
        public MyPoint Coord;              

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
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsEmpty()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToShortWKT2D()
        {
            return Coord.ToWKT2D();
        }

        /// <summary>
        /// Получение геометрии в виде wkt-строки (2D) - POINT(X Y)
        /// </summary>
        /// <returns> wkt-строка (2D) - POINT(X Y) </returns>
        public override string ToWKT2D()
        {
            if (this.IsEmpty()) return $"{TPoint.Type} {TGeometry.Emp}";
            return $"{TPoint.Type}({this.ToShortWKT2D()})";
        }

        /// <summary>
        /// Тип геометрии, всегда возвращает - TGeometry.GeometryType.Point
        /// </summary>
        /// <returns> TGeometry.GeometryType.Point </returns>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.Point;
        }

    }

}
