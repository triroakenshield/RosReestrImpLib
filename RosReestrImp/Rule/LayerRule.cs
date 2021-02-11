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
        public string Entpath; 

        /// <summary>Список полей слоя</summary>
        public List<FieldRule> FieldList; 

        private string GetElAttr(XmlElement wEl, string attrName)
        {
            return wEl.HasAttribute(attrName) ? wEl.GetAttribute(attrName) : "";
        }

        internal LayerRule(XmlElement wEl)
        {
                LName = GetElAttr(wEl, "Name");
                LayerPath = GetElAttr(wEl, "LayerPath");
                Entpath = GetElAttr(wEl, "EntPath");
                FieldList = new List<FieldRule>();
                foreach (XmlElement ch in wEl.ChildNodes)
                {
                    FieldList.Add(new FieldRule(ch));
                }
        }

        /// <summary>Получение списка имён полей слоя</summary>
        /// <returns>список имён полей слоя</returns>
        public string[] GetColumnNames()
        {
            var res = new string[FieldList.Count];
            for (var i = 0; i < FieldList.Count; i++)
            {
                res[i] = FieldList[i].FName;
            }
            return res;
        }
    }
}