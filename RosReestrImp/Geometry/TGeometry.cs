using System.Globalization;

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
            this.X = nX;
            this.Y = nY;
            this.Z = nZ;
        }

        /// <summary>Создание 2D точки (Z = 0)</summary>
        /// <param name="nX"> Координата X </param>
        /// <param name="nY"> Координата Y </param>
        public MyPoint(double nX, double nY)
        {
            this.X = nX;
            this.Y = nY;
            this.Z = 0;
        }

        /// <summary>Копирование точки</summary>
        /// <param name="op"> исходная точка </param>
        public MyPoint(MyPoint op)
        {
            this.X = op.X;
            this.Y = op.Y;
            this.Z = op.Z;
        }

        /// <summary>Получение wkt-строки с 2d геометрией</summary>
        /// <returns></returns>
        public string ToWKT2D()
        {
            return $"{X.ToString(CultureInfo.InvariantCulture)} {Y.ToString(CultureInfo.InvariantCulture)}";
        }
    }

    /// <summary>Внутренний формат для представления геометрии</summary>
    public abstract class TGeometry
    {
        /// <summary>Имя типа</summary>
        public static readonly string Type = "GEOMETRY";

        /// <summary>Пустое значение</summary>
        public static readonly string Emp = "EMPTY";

        /// <summary>Признак пусттой геометрии</summary>
        /// <returns></returns>
        public abstract bool IsEmpty();

        /// <summary>Получение короткой wkt-строки с 2d геометрией</summary>
        /// <returns></returns>
        public abstract string ToShortWKT2D();

        /// <summary>Получение геометрии в виде wkt-строки (2D)</summary>
        /// <returns> wkt-строка (2D) </returns>
        public abstract string ToWKT2D();

        /// <summary>Получение типа геометрии</summary>
        /// <returns> <see cref="TGeometry"/>.<see cref="GeometryType"/> </returns>
        public abstract GeometryType GetGeometryType();

        /// <summary>Получить архив координат xy</summary>
        /// <returns></returns>
        public abstract double[] GetXYArray();

        /// <summary>Получить архив координат z</summary>
        /// <returns></returns>
        public abstract double[] GetZArray();

        /// <summary>Получить MBR для геометрии</summary>
        /// <returns></returns>
        public abstract TMBR GetMBR();

        /// <summary>Признак правильности геометрии</summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}