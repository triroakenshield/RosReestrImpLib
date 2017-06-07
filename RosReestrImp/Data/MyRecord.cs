using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace RosReestrImp.Data
{
    /// <summary>
    /// Запись данных
    /// </summary>
    public class MyRecord
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
        /// Список полей
        /// </summary>
        public List<FieldValue> FileldList;

        internal MyRecord(Rule.LayerRule nRule)
        {
            this._Rule = nRule;
            this.FileldList = new List<FieldValue>();
            foreach (Rule.FieldRule r in this._Rule.FieldList)
            {
                this.FileldList.Add(new FieldValue(r));
            }            
        }

        internal void LoadData(XmlNode wNode, XmlNamespaceManager wNM)
        {
            foreach (FieldValue f in this.FileldList) 
            {
                f.LoadData(wNode, wNM);
            }
        }

        /// <summary>
        /// Получение геометрии записи
        /// </summary>
        /// <returns> Геометрия </returns>
        public Geometry.TGeometry GetGeometry()
        {
            if (this.FileldList.Exists(x => x.IsGeom))
            {
                FieldValue res = this.FileldList.First(x => x.IsGeom);
                return (Geometry.TGeometry)res.Value;
            }
            else return null; 
        }

        /// <summary>
        /// Поиск значения поля, по его имени
        /// </summary>
        /// <param name="fname"> Имя поля </param>
        /// <returns> Значение поля </returns>
        public FieldValue SearchField(string fname)
        {
            if (this.FileldList.Exists(x => x.Rule.FName == fname)) 
            {
                return this.FileldList.First(x => x.Rule.FName == fname);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Получение записи в виде списка значений (строк)
        /// </summary>
        /// <returns> Список значений </returns>
        public List<String> ToList()
        {
            List<String> res = new List<String>();
            foreach (FieldValue f in this.FileldList)
            {
                res.Add(f.GetString());
            }
            return res;
        }

        /// <summary>
        /// Получение записи в виде строки, разделённой ";"
        /// </summary>
        /// <returns> строка с разделителем - ";" </returns>
        public override string ToString()
        {
            string res = string.Empty;
            foreach (FieldValue f in this.FileldList)
            {
                res += f.GetString() + ";";
            }
            return res;
        }

    }
}
