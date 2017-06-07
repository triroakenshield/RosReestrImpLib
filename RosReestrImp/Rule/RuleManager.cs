using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace RosReestrImp.Rule
{

    /// <summary>
    /// Менеджер правил
    /// загружает xml-файл правил (корневой элемент - ShemaList)
    /// </summary>
    public class RuleManager
    {
        /// <summary>
        /// Список схем
        /// </summary>
        private List<ShemaRule> _ShemaList;
        
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="wDoc"> xml-файл правил </param>
        /// <exception cref="RuleLoadException"> Ошибка xml-файла правил </exception>
        public RuleManager(XmlDocument wDoc)
        {
            this._ShemaList = new List<ShemaRule>();
            XmlElement wNode = wDoc.DocumentElement;
            if (wNode.Name == "ShemaList")
            {
                foreach (XmlElement ch in wNode.ChildNodes)
                {
                    this._ShemaList.Add(new ShemaRule(ch));
                }
            }
            else
            {
                throw new RuleLoadException("ShemaList не найден");
            }
        }
        
        /// <summary>
        /// Загрузка xml-файла
        /// </summary>
        /// <param name="filename"> имя файла </param>
        /// <returns> XmlDocument </returns>
        /// <exception cref="RuleLoadException"> Ошибка открытия xml-файла </exception>
        private static XmlDocument LoadXML(string filename) 
        {
            try
            {
                XmlDocument wDoc = new XmlDocument();
                wDoc.Load(filename);
                return wDoc;
            }
            catch (Exception e)
            {
                throw new RuleLoadException("Ошибка открытия xml-файла", e);
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="filename"> xml-файл правил </param>
        /// <exception cref="RuleLoadException"> Ошибка xml-файла правил </exception>
        public RuleManager(string filename)    :
            this(RuleManager.LoadXML(filename))        
        {
        }

        /// <summary>
        /// Поиск схемы по имени
        /// </summary>
        /// <param name="DocName"> имя схемы </param>
        /// <returns> Схема </returns>
        internal ShemaRule FindShema(string DocName)
        {
            ShemaRule res = null;
            if (this._ShemaList.Exists(x => x._rootElem == DocName))
            {
                res = this._ShemaList.Find(x => x._rootElem == DocName);
            }
            return res;
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        /// <param name="filename"> xml-файл с данными </param>
        /// <returns> Список данных слоёв </returns>
        public List<Data.Layer> LoadData(string filename)
        {
            return this.LoadData(RuleManager.LoadXML(filename));
        }

        /// <summary>
        /// Загрузка данных из файла
        /// </summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns> Список данных слоёв </returns>
        public List<Data.Layer> LoadData(XmlDocument wDoc)
        {
            string DocName = wDoc.DocumentElement.Name;
            ShemaRule wSRule = this.FindShema(DocName);
            if (wSRule != null)
            {
                return wSRule.LoadData(wDoc);
            }
            else return null;
        }

    }
}
