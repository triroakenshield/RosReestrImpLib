using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace RosReestrImp.Data
{
    /// <summary>
    /// Данные поля
    /// </summary>
    public class FieldValue
    {
        /// <summary>
        /// Правило поля
        /// </summary>
        internal Rule.FieldRule _Rule;

        /// <summary>
        /// Правило загрузки данных
        /// </summary>
        public Rule.FieldRule Rule
        {
            get
            {
                return this._Rule;
            }
        }

        /// <summary>
        /// Значение поля
        /// </summary>
        public object Value;

        /// <summary>
        /// Создание значения поля
        /// </summary>
        /// <param name="nRule"> правило </param>
        internal FieldValue(Rule.FieldRule nRule)
        {
            this._Rule = nRule;
        }

        /// <summary>
        /// Создание значения поля
        /// </summary>
        /// <param name="nRule"> правило </param>
        /// <param name="nValue"> значение </param>
        internal FieldValue(Rule.FieldRule nRule, object nValue)
        {
            this._Rule = nRule;
            this.Value = nValue;
        }

        /// <summary>
        /// Является ли поле геометрией
        /// </summary>
        public bool IsGeom
        {
            get
            {
                return this.Rule.IsGeom;
            }
        }

        /// <summary>
        /// Загрузка данных поля
        /// </summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath при загрузке поля </exception>
        internal void LoadData(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                XmlNode crNode;
                XmlNode resNode;
                if (this.Rule.FPath != "") crNode = wNode.SelectSingleNode(this.Rule.FPath, wNM);
                else crNode = wNode;
                if (crNode != null)
                {
                    if (this.Rule.FAttr != "")
                    {
                        XmlElement tNode = (XmlElement)crNode;
                        if (tNode.HasAttribute(this.Rule.FAttr))
                        {
                            resNode = crNode.Attributes.GetNamedItem(this.Rule.FAttr);
                        }
                        else resNode = crNode;
                    }
                    else resNode = crNode;
                    //
                    if (this.IsGeom) this.Value = this.Rule.LoadGeometry(resNode, wNM);
                    else this.Value = resNode.Value; //!dict!
                }
            }
            catch (System.Xml.XPath.XPathException e) 
            {
                throw new Data.DataLoadException("Ошибка XPath при загрузке поля", e);
            }
        }
        
        /// <summary>
        /// Получение значения поля, в виде строки
        /// </summary>
        /// <returns> значение поля в виде строки </returns>
        public string GetString() 
        {
            if (this.Value != null)
            {
                if (this.IsGeom) return ((Geometry.TGeometry)this.Value).ToWKT2D();
                else return this.Value.ToString();
            }
            else return "null";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetCSV()
        {
            if (this.Value != null)
            {
                if (this.IsGeom) return ((Geometry.TGeometry)this.Value).ToWKT2D();
                else return String.Format("\"{0}\"", this.Value.ToString().Replace("\"", "\"\""));
            }
            else return "\"null\"";
        }

    }
}
