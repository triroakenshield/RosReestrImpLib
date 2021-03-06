﻿using System.Collections.Generic;
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

        public override bool IsEmpty() 
        {
            if (Geometries == null) return true;
            return Geometries.Count <= 0;
        }

        public override GeometryType GetGeometryType() => GeometryType.GeometryCollection;

        public override double[] GetXYArray() => null;

        public override double[] GetZArray() => null;

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

        public override bool IsValid()
        {
            if (Geometries == null) return false;
            return Geometries.Count > 0 && Geometries.All(ls => ls.IsValid());
        }

        public override string ToShortWKT2D() => string.Join(", ", Geometries.Select(p => $"({p.ToShortWKT2D()})"));

        public override string ToWKT2D()
        {
            return IsEmpty() ? $"{TGeometryCollection.Type} {TGeometry.Emp}" 
                : $"{TGeometryCollection.Type}({ToShortWKT2D()})";
        }
    }
}