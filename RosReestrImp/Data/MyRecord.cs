using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
//
using RosReestrImp.Geometry;

namespace RosReestrImp.Data
{
    /// <summary>Запись данных</summary>
    public class MyRecord
    {
        internal Rule.LayerRule _Rule;

        /// <summary>Правило загрузки данных</summary>
        public Rule.LayerRule Rule => this._Rule;

        /// <summary>Список полей</summary>
        public List<FieldValue> FieldList;

        internal MyRecord(Rule.LayerRule nRule)
        {
            this._Rule = nRule;
            this.FieldList = new List<FieldValue>();
            this._Rule.FieldList.ForEach(r => this.FieldList.Add(new FieldValue(r)));         
        }

        internal void LoadData(XmlNode wNode, XmlNamespaceManager wNM)
        {
            this.FieldList.ForEach(f => f.LoadData(wNode, wNM));
        }

        /// <summary>Получение геометрии записи</summary>
        /// <returns> Геометрия </returns>
        public Geometry.TGeometry GetGeometry()
        {
            if (this.FieldList.Exists(x => x.IsGeom))
            {
                FieldValue res = this.FieldList.First(x => x.IsGeom);
                return (Geometry.TGeometry)res.Value;
            }
            else return null; 
        }
        
        /// <summary>Поиск значения поля, по его имени</summary>
        /// <param name="fname"> Имя поля </param>
        /// <returns> Значение поля </returns>
        public FieldValue SearchField(string fname)
        {
            return this.FieldList.Exists(x => x.Rule.FName == fname) 
                ? this.FieldList.First(x => x.Rule.FName == fname) : null;
        }

        /// <summary>Получение записи в виде списка значений (строк) </summary>
        /// <returns> Список значений </returns>
        public List<String> ToList()
        {
            var res = new List<String>();
            this.FieldList.ForEach(f => res.Add(f.GetString()));
            return res;
        }

        public string GetFieldsString()
        {
            return string.Join(";", this.FieldList.Where(f=>!f.IsGeom).Select(f => $"{f.FName}:{f.Value.ToString().Replace("\"", "\"\"")}"));
        }

        /// <summary>Получение записи в виде строки, разделённой ";"</summary>
        /// <returns> строка с разделителем - ";" </returns>
        public override string ToString() // на удаление?
        {
            return string.Join(";", this.FieldList.Select(f => f.GetCSV()));
        }

        /// <summary>Получить MBR записи</summary>
        /// <returns></returns>
        public TMBR GetMBR()
        {
            TGeometry geom = this.GetGeometry();
            return geom?.GetMBR();
        }
    }
}