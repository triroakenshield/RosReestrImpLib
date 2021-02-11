// ReSharper disable InconsistentNaming
namespace RosReestrImp.Geometry
{
    /// <summary>Внутренний формат для представления геометрии - точки</summary>
    public class TPoint : TGeometry
    {
        /// <summary>Имя типа</summary>
        public new static readonly string Type = "POINT";

        /// <summary>Координаты точки (<see cref="MyPoint"/>)</summary>
        public MyPoint Coord;

        /// <summary>Создание 3D точки</summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        /// <param name="nZ"> Координата Z </param>
        public TPoint(double nX, double nY, double nZ)
        {
            Coord.X = nX;
            Coord.Y = nY;
            Coord.Z = nZ;
        }

        /// <summary>Создание 2D точки (Z = 0)</summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        public TPoint(double nX, double nY)
        {
            Coord.X = nX;
            Coord.Y = nY;
            Coord.Z = 0;
        }

        public new bool IsEmpty() => false;

        public new string ToShortWKT2D() => Coord.ToWKT2D();

        /// <summary>Получение геометрии в виде wkt-строки (2D) - POINT(X Y)</summary>
        /// <returns> wkt-строка (2D) - POINT(X Y) </returns>
        public new string ToWKT2D() => IsEmpty() ? $"{TPoint.Type} {TGeometry.Emp}" : $"{TPoint.Type}({ToShortWKT2D()})";

        /// <summary>Тип геометрии, всегда возвращает - TGeometry.GeometryType.Point</summary>
        /// <returns> TGeometry.GeometryType.Point </returns>
        public new GeometryType GetGeometryType() => GeometryType.Point;

        public new double[] GetXYArray() => new[] { Coord.X, Coord.Y };

        public new double[] GetZArray() => new[] { Coord.Z };

        public new TMBR GetMBR() => new TMBR(Coord);

        public new bool IsValid() => true;
    }
}