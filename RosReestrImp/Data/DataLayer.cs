using System.Collections.Generic;
using System.Text;
using System.Xml;
//
using RosReestrImp.Geometry;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Data
{
    /// <summary>Слой данных</summary>
    public class DataLayer
    {
        internal Rule.LayerRule _Rule;

        /// <summary>Правило загрузки данных</summary>
        public Rule.LayerRule Rule => _Rule;

        /// <summary>Таблица данных (записей)</summary>
        public List<MyRecord> Table;

        /// <summary>Создаём слой данных</summary>
        /// <param name="nRule"> Правило загрузки данных </param>
        internal DataLayer(Rule.LayerRule nRule)
        {
            this._Rule = nRule;
            this.Table = new List<MyRecord>();
        }

        /// <summary>Имя слоя</summary>
        public string Name => this._Rule.LName;

        /// <summary>
        /// Получаем список Пространств имен xml-файла данных, префикс ns для Пространства имён по умолчанию
        /// </summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns> Список Пространств имен </returns>
        protected XmlNamespaceManager LoadNamespace(XmlDocument wDoc)
        {
            var res = new XmlNamespaceManager(wDoc.NameTable);
            res.PopScope();
            XmlElement rNode = wDoc.DocumentElement;
            if (rNode == null) return null;
            foreach (XmlAttribute attr in rNode.Attributes)
            {
                var nStr = attr.Name;
                if (nStr.Contains("xmlns"))
                {
                    res.AddNamespace(nStr.Length == 5 ? "ns" : nStr.Replace("xmlns:", ""), attr.Value);
                }
            }
            return res;
        }

        /// <summary>Загружаем данные слоя</summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns> Список записей </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath при загрузке слоя </exception>
        internal List<MyRecord> LoadData(XmlDocument wDoc)
        {            
            try
            {
                var wNm = LoadNamespace(wDoc);
                var layersXmlNodeList = wDoc.DocumentElement?.SelectNodes(_Rule.LayerPath, wNm);
                if (layersXmlNodeList == null) return this.Table;
                foreach (XmlNode n in layersXmlNodeList)
                {
                    var layerXmlNodeList = n.SelectNodes(this._Rule.Entpath, wNm);
                    if (layerXmlNodeList == null) continue;
                    foreach (XmlNode nn in layerXmlNodeList)
                    {
                        var wRec = new MyRecord(this._Rule);
                        wRec.LoadData(nn, wNm);
                        this.Table.Add(wRec);
                    }
                }
                return this.Table;
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new DataLoadException("Ошибка XPath при загрузке слоя " + this.Name, e);
            }           
        }

        /// <summary>Проверяем наличия поля с не геометрией</summary>
        /// <returns></returns>
        public bool HasAttributes()
        {
            return this._Rule.FieldList.Exists(f => !f.IsGeom);
        }

        /// <summary>Получение таблицы значений слоя (список списков)</summary>
        /// <returns> Таблица значений слоя </returns>
        public List<List<string>> ToList()
        {
            var res = new List<List<string>>();
            this.Table.ForEach(r => res.Add(r.ToList()));
            return res;
        }

        /// <summary>Получение списка имён полей слоя</summary>
        /// <returns> список имён полей слоя </returns>
        public string[] GetColumnNames()
        {
            return this._Rule.GetColumnNames();
        }

        /// <summary>Получение данных слоя, в виде csv-файла</summary>
        /// <returns> csv-файл (в виде строки) </returns>
        public string GetCSV()
        {
            var wSB = new StringBuilder();
            wSB.AppendLine(string.Join(";", this.GetColumnNames()));
            this.Table.ForEach(r => wSB.AppendLine(r.ToString()));
            return wSB.ToString();
        }

        /// <summary>Получить MBR слоя</summary>
        /// <returns></returns>
        public TMBR GetMBR()
        {
            TMBR res = null;
            foreach (var r in this.Table)
            {
                var newMbr = r.GetMBR();
                if (newMbr == null) continue;
                if (res == null) res = newMbr;
                else res.AddMBR(newMbr);
            }
            return res;
        }
    }
}