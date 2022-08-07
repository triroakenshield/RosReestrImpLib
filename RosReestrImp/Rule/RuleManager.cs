using System;
using System.Collections.Generic;
using System.Xml;

namespace RosReestrImp.Rule
{
    /// <summary>Менеджер правил загружает xml-файл правил (корневой элемент - ShemaList) </summary>
    public class RuleManager
    {
        /// <summary>Список схем</summary>
        private List<ShemaRule> _ShemaList;
        
        /// <summary>Конструктор</summary>
        /// <param name="wDoc">xml-файл правил</param>
        /// <exception cref="RuleLoadException">Ошибка xml-файла правил</exception>
        public RuleManager(XmlDocument wDoc)
        {
            _ShemaList = new List<ShemaRule>();
            var wNode = wDoc.DocumentElement;
            if (wNode != null && wNode.Name == "ShemaList")
            {
                foreach (XmlElement ch in wNode.ChildNodes)
                {
                    _ShemaList.Add(new ShemaRule(ch));
                }
            }
            else { throw new RuleLoadException("ShemaList не найден"); }
        }
        
        /// <summary>Загрузка xml-файла</summary>
        /// <param name="filename">имя файла</param>
        /// <returns>XmlDocument</returns>
        /// <exception cref="RuleLoadException">Ошибка открытия xml-файла</exception>
        private static XmlDocument LoadXml(string filename) 
        {
            try
            {
                var wDoc = new XmlDocument();
                wDoc.Load(filename);
                return wDoc;
            }
            catch (Exception e) { throw new RuleLoadException("Ошибка открытия xml-файла", e); }
        }

        /// <summary>Конструктор</summary>
        /// <param name="filename">xml-файл правил</param>
        /// <exception cref="RuleLoadException">Ошибка xml-файла правил</exception>
        public RuleManager(string filename) : this(LoadXml(filename)) {}

        /// <summary>Поиск схемы по имени</summary>
        /// <param name="docName"> имя схемы </param>
        /// <returns> Схема </returns>
        internal ShemaRule FindShema(string docName)
        {
            ShemaRule res = null;
            if (_ShemaList.Exists(x => x.RootElem == docName)) 
                res = _ShemaList.Find(x => x.RootElem == docName);
            return res;
        }

        /// <summary>Загрузка данных из файла</summary>
        /// <param name="filename">xml-файл с данными</param>
        /// <returns>Список данных слоёв</returns>
        public List<Data.DataLayer> LoadData(string filename)
        {
            return LoadData(LoadXml(filename));
        }

        /// <summary>Загрузка данных из файла</summary>
        /// <param name="wDoc">xml-файл с данными</param>
        /// <returns>Список данных слоёв</returns>
        public List<Data.DataLayer> LoadData(XmlDocument wDoc)
        {
            if (wDoc.DocumentElement == null) return null;
            var docName = wDoc.DocumentElement.Name;
            var wSRule = FindShema(docName);
            return wSRule?.LoadData(wDoc);
        }
    }
}