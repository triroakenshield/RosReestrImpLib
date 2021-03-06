﻿using RosReestrImp.Geometry;

using System.Collections.Generic;
using System.Linq;
using System.Xml;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Data
{
    /// <summary>Запись данных</summary>
    public class MyRecord
    {
        internal Rule.LayerRule _Rule;

        /// <summary>Правило загрузки данных</summary>
        public Rule.LayerRule Rule => _Rule;

        /// <summary>Список полей</summary>
        public List<FieldValue> FieldList;

        internal MyRecord(Rule.LayerRule nRule)
        {
            _Rule = nRule;
            FieldList = new List<FieldValue>();
            _Rule.FieldList.ForEach(r => FieldList.Add(new FieldValue(r)));
        }

        internal void LoadData(XmlNode wNode, XmlNamespaceManager wNM)
        {
            FieldList.ForEach(f => f.LoadData(wNode, wNM));
        }

        /// <summary>Получение геометрии записи</summary>
        /// <returns>Геометрия</returns>
        public TGeometry GetGeometry()
        {
            if (!FieldList.Exists(x => x.IsGeom)) return null;
            var res = FieldList.First(x => x.IsGeom);
            return (TGeometry)res.Value;
        }

        /// <summary>Признак наличия геометрии в записи</summary>
        /// <returns></returns>
        public bool HasGeometry()
        {
            var geom = GetGeometry();
            return geom != null && !geom.IsEmpty();
        }

        /// <summary>Поиск значения поля, по его имени</summary>
        /// <param name="fName">Имя поля</param>
        /// <returns>Значение поля</returns>
        public FieldValue SearchField(string fName)       
        {
            return FieldList.Exists(x => x.Rule.CorrectName == fName)
                 ? FieldList.First(x => x.Rule.CorrectName == fName) : null;
        }

        /// <summary>Получение записи в виде списка значений (строк)</summary>
        /// <returns>Список значений</returns>
        public List<string> ToList()
        {
            var res = new List<string>();
            FieldList.ForEach(f => res.Add(f.GetString()));
            return res;
        }

        /// <summary>Получить строку со значениями свойств</summary>
        /// <returns></returns>
        public string GetFieldsString()
        {
            return string.Join(";", FieldList.Where(f => !f.IsGeom)
                .Select(f => $"{f.FName}:{f.Value.ToString().Replace("\"", "\"\"")}"));
        }

        /// <summary>Получение записи в виде строки, разделённой ";"</summary>
        /// <returns>строка с разделителем - ";"</returns>
        public override string ToString() => string.Join(";", FieldList.Select(f => f.GetCSV()));

        /// <summary>Получить MBR записи</summary>
        /// <returns></returns>
        public TMBR GetMBR() => GetGeometry()?.GetMBR();
    }
}