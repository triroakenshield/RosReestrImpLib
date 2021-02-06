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

        ///<inheritdoc/>
        public override bool IsEmpty()
        {
            return false;
        }

        ///<inheritdoc/>
        public override string ToShortWKT2D()
        {
            return Coord.ToWKT2D();
        }

        /// <summary>Получение геометрии в виде wkt-строки (2D) - POINT(X Y)</summary>
        /// <returns> wkt-строка (2D) - POINT(X Y) </returns>
        public override string ToWKT2D()
        {
            return IsEmpty() ? $"{TPoint.Type} {TGeometry.Emp}" : $"{TPoint.Type}({ToShortWKT2D()})";
        }

        /// <summary>Тип геометрии, всегда возвращает - TGeometry.GeometryType.Point</summary>
        /// <returns> TGeometry.GeometryType.Point </returns>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.Point;
        }

        ///<inheritdoc/>
        public override double[] GetXYArray()
        {
              return new[] { Coord.X, Coord.Y };
        }

        ///<inheritdoc/>
        public override double[] GetZArray()
        {
            return new[] { Coord.Z };
        }

        ///<inheritdoc/>
        public override TMBR GetMBR()
        {
            return new TMBR(Coord);
        }

        ///<inheritdoc/>
        public override bool IsValid()
        {
            return true;
        }
    }
}