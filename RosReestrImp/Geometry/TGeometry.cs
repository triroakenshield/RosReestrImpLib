using System.Globalization;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary>Тип геометрии: точка (Point), линия (LineString), полигон (Polygon)</summary>
    public enum GeometryType
    {
        /// <summary>Отсутствует</summary>
        No,
        /// <summary>Точка</summary>
        Point,
        /// <summary>Линия</summary>
        LineString,
        /// <summary>Полигон</summary>
        Polygon,
        /// <summary>GeometryCollection</summary>
        GeometryCollection,
        /// <summary>Мултиполигон</summary>
        MultiPolygon
    }

    /// <summary>Структура для хранения координат точки</summary>
    public struct MyPoint
    {
        /// <summary>Координата X</summary>
        public double X;
        /// <summary>Координата Y</summary>
        public double Y;
        /// <summary>Координата Z</summary>
        public double Z;

        /// <summary>Создание 3D точки</summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        /// <param name="nZ"> Координата Z </param>
        public MyPoint(double nX, double nY, double nZ)
        {
            X = nX;
            Y = nY;
            Z = nZ;
        }

        /// <summary>Создание 2D точки (Z = 0)</summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        public MyPoint(double nX, double nY)
        {
            X = nX;
            Y = nY;
            Z = 0;
        }

        /// <summary>Копирование точки</summary>
        /// <param name="op"> исходная точка </param>
        public MyPoint(MyPoint op)
        {
            X = op.X;
            Y = op.Y;
            Z = op.Z;
        }

        /// <summary>Получение wkt-строки с 2d геометрией</summary>
        /// <returns></returns>
        public string ToWKT2D() => $"{X:InvariantCulture} {Y:InvariantCulture}";
    }

    /// <summary>Внутренний формат для представления геометрии</summary>
    public class TGeometry
    {
        /// <summary>Имя типа</summary>
        public static readonly string Type = "GEOMETRY";

        /// <summary>Пустое значение</summary>
        public static readonly string Emp = "EMPTY";

        /// <summary>Признак пустой геометрии</summary>
        /// <returns></returns>
        public virtual bool IsEmpty() => true;

        /// <summary>Получение короткой wkt-строки с 2d геометрией</summary>
        /// <returns></returns>
        public virtual string ToShortWKT2D() => $"{Type} {Emp}";

        /// <summary>Получение геометрии в виде wkt-строки (2D)</summary>
        /// <returns> wkt-строка (2D) </returns>
        public virtual string ToWKT2D() => ToShortWKT2D();

        /// <summary>Получение типа геометрии</summary>
        /// <returns> <see cref="TGeometry"/>.<see cref="GeometryType"/> </returns>
        public virtual GeometryType GetGeometryType() => GeometryType.No;

        /// <summary>Получить архив координат xy</summary>
        /// <returns></returns>
        public virtual double[] GetXYArray() => null;

        /// <summary>Получить архив координат z</summary>
        /// <returns></returns>
        public virtual double[] GetZArray() => null;

        /// <summary>Получить MBR для геометрии</summary>
        /// <returns></returns>
        public virtual TMBR GetMBR() => null;

        /// <summary>Признак правильности геометрии</summary>
        /// <returns></returns>
        public virtual bool IsValid() => true;
    }
}