using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace RosReestrImp.Data
{
    /// <summary>
    /// Слой данных
    /// </summary>
    public class Layer
    {

        internal Rule.LayerRule _Rule;

        /// <summary>
        /// Правило загрузки данных
        /// </summary>
        public Rule.LayerRule Rule
        {
            get
            {
                return this._Rule;
            }
        }

        /// <summary>
        /// Таблица данных (записей)
        /// </summary>
        public List<MyRecord> Table; 

        /// <summary>
        /// Создаём слой данных
        /// </summary>
        /// <param name="nRule"> Правило загрузки данных </param>
        internal Layer(Rule.LayerRule nRule)
        {
            this._Rule = nRule;
            this.Table = new List<MyRecord>();
        }

        /// <summary>
        /// Имя слоя
        /// </summary>
        public string Name
        {
            get
            {
                return this._Rule.LName;
            }
        }

        /// <summary>
        /// Получаем список Пространств имен xml-файла данных, префикс ns для Пространства имён по умолчанию
        /// </summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns> Список Пространств имен </returns>
        protected XmlNamespaceManager LoadNamespace(XmlDocument wDoc)
        {
            XmlNamespaceManager res = new XmlNamespaceManager(wDoc.NameTable);
            res.PopScope();
            XmlElement rNode = wDoc.DocumentElement;
            string nStr;
            foreach (XmlAttribute attr in rNode.Attributes)
            {
                nStr = attr.Name;
                if (nStr.Contains("xmlns"))
                {
                    if (nStr.Length == 5) res.AddNamespace("ns", attr.Value); //!!
                    else res.AddNamespace(nStr.Replace("xmlns:", ""), attr.Value);
                }
            }
            //res.AddNamespace("ns", wDoc.DocumentElement.NamespaceURI);
            return res;
        }

        /// <summary>
        /// Загружаем данные слоя
        /// </summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns> Список записей </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath при загрузке слоя </exception>
        internal List<MyRecord> LoadData(XmlDocument wDoc)
        {            
            try
            {
                XmlNamespaceManager wNM = this.LoadNamespace(wDoc);
                XmlNodeList LayersXmlNodeList = wDoc.DocumentElement.SelectNodes(this._Rule.LayerPath, wNM);
                XmlNodeList LayerXmlNodeList;
                MyRecord wRec;
                foreach (XmlNode n in LayersXmlNodeList)
                {
                    LayerXmlNodeList = n.SelectNodes(this._Rule.Entpath, wNM);
                    foreach (XmlNode nn in LayerXmlNodeList)
                    {
                        wRec = new MyRecord(this._Rule);
                        wRec.LoadData(nn, wNM);
                        this.Table.Add(wRec);
                    }
                }
                return this.Table;
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath при загрузке слоя " + this.Name, e);
            }           
        }

        /// <summary>
        /// Проверяем наличия поля с не геометрией
        /// </summary>
        /// <returns></returns>
        public bool HasAttributes()
        {
            return this._Rule.FieldList.Exists(f => !f.IsGeom);
        }

        /// <summary>
        /// Получение таблицы значений слоя (список списков)
        /// </summary>
        /// <returns> Таблица значений слоя </returns>
        public List<List<String>> ToList()
        {
            List<List<String>> res = new List<List<String>>();
            this.Table.ForEach(r => res.Add(r.ToList()));
            return res;
        }

        /// <summary>
        /// Получение списка имён полей слоя
        /// </summary>
        /// <returns> список имён полей слоя </returns>
        public string[] GetColumnNames()
        {
            return this._Rule.GetColumnNames();
        }

        /// <summary>
        /// Получение данных слоя, в виде csv-файла
        /// </summary>
        /// <returns> csv-файл (в виде строки) </returns>
        public string GetCSV()
        {
            StringBuilder wSB = new StringBuilder();
            wSB.AppendLine(String.Join(";", this.GetColumnNames()));
            this.Table.ForEach(r => wSB.AppendLine(r.ToString()));
            return wSB.ToString();
        }

    }
}
