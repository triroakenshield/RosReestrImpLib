using System.Xml;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Data
{
    /// <summary>Данные поля</summary>
    public class FieldValue
    {
        /// <summary>Правило поля</summary>
        internal Rule.FieldRule _Rule;

        /// <summary>Правило загрузки данных</summary>
        public Rule.FieldRule Rule => this._Rule;

        public string FName => this._Rule.FName;

        /// <summary>Значение поля</summary>
        public object Value;

        /// <summary>Создание значения поля</summary>
        /// <param name="nRule"> правило </param>
        internal FieldValue(Rule.FieldRule nRule)
        {
            this._Rule = nRule;
        }

        /// <summary>Создание значения поля</summary>
        /// <param name="nRule"> правило </param>
        /// <param name="nValue"> значение </param>
        internal FieldValue(Rule.FieldRule nRule, object nValue)
        {
            this._Rule = nRule;
            this.Value = nValue;
        }

        /// <summary>Является ли поле геометрией</summary>
        public bool IsGeom => this.Rule.IsGeom;
        
        /// <summary>Загрузка данных поля</summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath при загрузке поля </exception>
        internal void LoadData(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                XmlNode crNode = Rule.FPath != "" ? wNode.SelectSingleNode(Rule.FPath, wNM) : wNode;
                if (crNode != null)
                {
                    XmlNode resNode;
                    if (this.Rule.FAttr != "")
                    {
                        var tNode = (XmlElement)crNode;
                        resNode = tNode.HasAttribute(Rule.FAttr) ? crNode.Attributes?.GetNamedItem(Rule.FAttr) : crNode;
                    }
                    else resNode = crNode;
                    //
                    Value = IsGeom ? Rule.LoadGeometry(resNode, wNM) : resNode?.Value as object;
                }
            }
            catch (System.Xml.XPath.XPathException e) 
            {
                throw new DataLoadException("Ошибка XPath при загрузке поля", e);
            }
        }
        
        /// <summary>Получение значения поля, в виде строки</summary>
        /// <returns> значение поля в виде строки </returns>
        public string GetString() 
        {
            if (Value != null) return IsGeom ? ((Geometry.TGeometry)Value).ToWKT2D() : Value.ToString();
            else return "null";
        }

        /// <summary>Получение данных поля, в виде csv-файла</summary>
        /// <returns></returns>
        public string GetCSV()
        {
            if (this.Value != null)
            {
                return this.IsGeom ? ((Geometry.TGeometry)this.Value).ToWKT2D() 
                    : $"\"{this.Value.ToString().Replace("\"", "\"\"")}\"";
            }
            else return "\"null\"";
        }
    }
}