using System;
using System.Globalization;

namespace RosReestrImp.Geometry
{

    /// <summary>
    /// Тип геометрии: точка (Point), 
    /// линия (LineString), полигон (Polygon)
    /// </summary>
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

        MultiPolygon

    }

    /// <summary> Структура для храниния координат точки</summary>
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

        /// <summary></summary>
        /// <returns></returns>
        public string ToWKT2D()
        {
            return $"{X.ToString(CultureInfo.InvariantCulture)} {Y.ToString(CultureInfo.InvariantCulture)}";
        }

    }

    /// <summary>Внутрений формат для представления геометрии</summary>
    public abstract class TGeometry
    {

        /// <summary></summary>
        public static readonly string Type = "GEOMETRY";

        /// <summary></summary>
        public static readonly string Emp = "EMPTY";

        /// <summary></summary>
        /// <returns></returns>
        public abstract bool IsEmpty();

        /// <summary></summary>
        /// <returns></returns>
        public abstract String ToShortWKT2D();

        /// <summary>Получение геометрии в виде wkt-строки (2D)</summary>
        /// <returns> wkt-строка (2D) </returns>
        public abstract String ToWKT2D();

        /// <summary>Получение типа геометрии</summary>
        /// <returns> TGeometry.GeometryType </returns>
        public abstract GeometryType GetGeometryType();

        public abstract TMBR GetMBR();

        public abstract bool IsValid();       
    }
}