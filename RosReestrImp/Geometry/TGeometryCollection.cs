using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary>Абстрактная коллекция геометрии</summary>
    public class TGeometryCollection : TGeometry
    {
        /// <summary>Имя типа</summary>
        public new static readonly string Type = "GEOMETRYCOLLECTION";

        /// <summary>Список</summary>
        public List<TGeometry> Geometries;

        /// <summary>Конструктор</summary>
        public TGeometryCollection()
        {
            Geometries = new List<TGeometry>();
        }

        /// <summary>Конструктор из списка</summary>
        /// <param name="nGeometries"></param>
        public TGeometryCollection(List<TGeometry> nGeometries)
        {
            Geometries = nGeometries.GetRange(0, nGeometries.Count);
        }

        /// <summary>Признак пустой геометрии</summary>
        /// <returns></returns>
        public override bool IsEmpty() 
        {
            if (Geometries == null) return true;
            return Geometries.Count <= 0;
        }

        /// <summary>Получаем тип геометрии</summary>
        /// <returns></returns>
        public override GeometryType GetGeometryType() => GeometryType.GeometryCollection;

        /// <summary>Получаем массив координат XY</summary>
        /// <returns></returns>
        public override double[] GetXYArray() => null;

        /// <summary>Получаем массив координат Z</summary>
        /// <returns></returns>
        public override double[] GetZArray() => null;

        /// <summary>Получаем MBR (Минимальный ограничивающий прямоугольник)</summary>
        /// <returns></returns>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (var p in Geometries)
            {
                if (res == null) res = p.GetMBR();
                else res.AddMBR(p.GetMBR());
            }
            return res;
        }

        /// <summary>Признак корректности геометрии</summary>
        /// <returns></returns>
        public override bool IsValid()
        {
            if (Geometries == null) return false;
            return Geometries.Count > 0 && Geometries.All(ls => ls.IsValid());
        }

        /// <summary>Короткая строка Wkt (Только координаты)</summary>
        /// <returns></returns>
        public override string ToShortWKT2D() => string.Join(", ", Geometries.Select(p => $"({p.ToShortWKT2D()})"));

        /// <summary>Полная строка Wkt</summary>
        /// <returns></returns>
        public override string ToWKT2D()
        {
            return IsEmpty() ? $"{Type} {Emp}" : $"{Type}({ToShortWKT2D()})";
        }
    }
}