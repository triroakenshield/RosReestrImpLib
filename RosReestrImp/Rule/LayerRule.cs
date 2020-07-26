using System.Collections.Generic;
using System.Xml;

namespace RosReestrImp.Rule
{
    /// <summary>Правила слоя, элемент DataLayer</summary>
    public class LayerRule 
    {
        /// <summary>Имя слоя, атрибут Name</summary>
        public string LName; 

        /// <summary>Путь слоя, атрибут LayerPath</summary>
        internal string LayerPath; 

        /// <summary>Путь элементов слоя, атрибут EntPath</summary>
        internal string Entpath; 

        /// <summary>Список полей слоя</summary>
        public List<FieldRule> FieldList; 

        private string GetElAttr(XmlElement wEl, string attrName)
        {
            return wEl.HasAttribute(attrName) ? wEl.GetAttribute(attrName) : "";
        }

        /// <summary></summary>
        /// <param name="wEl"></param>
        internal LayerRule(XmlElement wEl)
        {
                this.LName = this.GetElAttr(wEl, "Name");
                this.LayerPath = this.GetElAttr(wEl, "LayerPath");
                this.Entpath = this.GetElAttr(wEl, "EntPath");
                this.FieldList = new List<FieldRule>();
                foreach (XmlElement ch in wEl.ChildNodes)
                {
                    this.FieldList.Add(new FieldRule(ch));
                }
        }

        /// <summary>Получение списка имён полей слоя</summary>
        /// <returns> список имён полей слоя </returns>
        public string[] GetColumnNames()
        {
            string[] res = new string[this.FieldList.Count];
            for (var i = 0; i < this.FieldList.Count; i++)
            {
                res[i] = this.FieldList[i].FName;
            }
            return res;
        }
    }
}