using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary> Внутренний формат для представления геометрии - полигона </summary>
    public class TPolygon : TGeometry
    {
        /// <summary>Имя типа</summary>
        public new static readonly string Type = "POLYGON";

        /// <summary>Список замкнутых контуров - колец</summary>
        public List<TLineString> Rings;

        /// <summary>Создание полигона</summary>
        /// <param name="nRings"> Список контуров </param>
        public TPolygon(List<TLineString> nRings)
        {
            Rings = nRings.GetRange(0, nRings.Count);
        }

        /// <inheritdoc/>
        public override bool IsEmpty() => Rings.Count == 0;

        /// <inheritdoc/>
        public override string ToShortWKT2D() => string.Join(", ", Rings.Select(p => $"({p.RingToShortWKT2D()})"));

        /// <summary>Получение геометрии в виде wkt-строки (2D) - Polygon((x0 y0, x1 y1, ..., xn yn, x0 y0), ..., (...))</summary>
        /// <returns></returns>
        public override string ToWKT2D() => IsEmpty() ? $"{Type} {Emp}" : $"{Type}({ToShortWKT2D()})";

        /// <summary>Тип геометрии, всегда возвращает - TGeometry.GeometryType.Polygon</summary>
        /// <returns> TGeometry.GeometryType.Polygon </returns>
        public override GeometryType GetGeometryType() => GeometryType.Polygon;

        /// <inheritdoc/>
        public override double[] GetXYArray() => null;

        /// <inheritdoc/>
        public override double[] GetZArray() => null;

        /// <inheritdoc/>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (var p in Rings)
            {
                if (res == null) res = p.GetMBR();
                else { res.AddMBR(p.GetMBR()); }
            }
            return res;
        }

        /// <inheritdoc/>
        public override bool IsValid() => Rings.Count > 0 && Rings.All(ls => ls.IsValid());

        /// <summary>Получаем внешний контур полигона</summary>
        /// <returns></returns>
        public TLineString GetOuterBoundary()
        {
            var AllMBR = Rings.Select(r => r.GetMBR()).ToArray();
            var res = -1;
            for (var i = 0; i < Rings.Count; i++)
            {
                var tMBR = AllMBR[i];
                for (var j=0; j < Rings.Count; j++)
                {
                    if (j == i) continue;
                    if (!tMBR.Contains(AllMBR[j])) break;
                    res = i;
                }
            }
            return res == -1 ? null : Rings[res];
        }

        /// <summary>Получаем коллекцию контуров полигона</summary>
        /// <returns></returns>
        public TGeometryCollection AsCollection() 
            => new TGeometryCollection(Rings.Select(l => (TGeometry)l).ToList());
    }
}