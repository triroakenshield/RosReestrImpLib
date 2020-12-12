using System.Collections.Generic;
using System.Linq;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Geometry
{
    /// <summary> Внутренний формат для представления геометрии - полигона </summary>
    public class TPolygon : TGeometry
    {
        /// <summary></summary>
        public new static readonly string Type = "POLYGON";

        /// <summary>Список замкнутых контуров - колец</summary>
        public List<TLineString> Rings;

        /// <summary>Создание полигона</summary>
        /// <param name="nRings"> Список контуров </param>
        public TPolygon(List<TLineString> nRings)
        {
            this.Rings = nRings.GetRange(0, nRings.Count);
        }

        ///<inheritdoc/>
        public override bool IsEmpty()
        {
            return this.Rings.Count == 0;
        }

        ///<inheritdoc/>
        public override string ToShortWKT2D()
        {
            return string.Join(", ", this.Rings.Select(p => $"({p.RingToShortWKT2D()})"));
        }

        /// <summary>Получение геометрии в виде wkt-строки (2D) - Polygon((x0 y0, x1 y1, ..., xn yn, x0 y0), ..., (...))</summary>
        /// <returns></returns>
        public override string ToWKT2D()
        {
            return this.IsEmpty() ? $"{TPolygon.Type} {TGeometry.Emp}" : $"{TPolygon.Type}({this.ToShortWKT2D()})";
        }

        /// <summary>Тип геометрии, всегда возвращает - TGeometry.GeometryType.Polygon</summary>
        /// <returns> TGeometry.GeometryType.Polygon </returns>
        public override GeometryType GetGeometryType()
        {
            return GeometryType.Polygon;
        }

        ///<inheritdoc/>
        public override double[] GetXYArray()
        {
            return null;
        }

        ///<inheritdoc/>
        public override double[] GetZArray()
        {
            return null;
        }

        ///<inheritdoc/>
        public override TMBR GetMBR()
        {
            TMBR res = null;
            foreach (TLineString p in this.Rings)
            {
                if (res == null) res = p.GetMBR();
                else { res.AddMBR(p.GetMBR()); }
            }
            return res;
        }

        ///<inheritdoc/>
        public override bool IsValid()
        {
            return this.Rings.Count > 0 && this.Rings.All(ls => ls.IsValid());
        }

        public TLineString GetOuterBoundary()
        {
            var AllMBR = this.Rings.Select(r => r.GetMBR()).ToArray();
            int res = -1;
            for (int i = 0; i < this.Rings.Count; i++)
            {
                var tMBR = AllMBR[i];
                for (int j=0; j < this.Rings.Count; j++)
                {
                    if (j == i) continue;
                    if (!tMBR.Contains(AllMBR[j])) break;
                    res = i;
                }
            }
            return res == -1 ? null : this.Rings[res];
        }

        public TGeometryCollection AsCollection()
        {
            return new TGeometryCollection(this.Rings.Select(l=>(TGeometry)l).ToList());
        }
    }
}