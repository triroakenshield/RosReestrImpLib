using System.Collections.Generic;
using System.Xml;

namespace RosReestrImp.Rule
{
    /// <summary>Правила схемы, элемент Shema</summary>
    internal class ShemaRule
    {
        /// <summary>Имя схемы</summary>
        internal string RootElem;

        /// <summary>Список слоёв (правил)</summary>
        internal List<LayerRule> LayerList;

        /// <summary>Загружаем правила схемы</summary>
        /// <param name="wEl">Элемент Shema xml-файла правил</param>
        /// <exception cref="RuleLoadException">Ошибка xml-файла правил</exception>
        internal ShemaRule(XmlElement wEl)
        {
            if (wEl.HasAttribute("rootElem"))
            {
                RootElem = wEl.GetAttribute("rootElem");
                LayerList = new List<LayerRule>();
                foreach (XmlElement ch in wEl.ChildNodes)
                {
                    LayerList.Add(new LayerRule(ch));
                }
            }
            else { throw new RuleLoadException("У Shema нет rootElem"); }
        }

        /// <summary>Загружаем данные слоя</summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <param name="wRule"> правила слоя </param>
        /// <returns></returns>
        internal Data.DataLayer LoadData(XmlDocument wDoc, LayerRule wRule)
        {
            var res = new Data.DataLayer(wRule);
            res.LoadData(wDoc);
            return res;
        }

        /// <summary>Загружаем данные схемы</summary>
        /// <param name="wDoc"> xml-файл с данными </param>
        /// <returns></returns>
        internal List<Data.DataLayer> LoadData(XmlDocument wDoc)
        {
            var res = new List<Data.DataLayer>();
            LayerList.ForEach(r => res.Add(LoadData(wDoc, r)));
            return res;
        }
    }
}