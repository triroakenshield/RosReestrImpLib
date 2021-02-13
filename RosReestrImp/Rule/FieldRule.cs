using System;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

namespace RosReestrImp.Rule
{
    /// <summary>Описание поля слоя, элемент FieldRule</summary>
    public class FieldRule
    {
        /// <summary>Описание геометрии</summary>
        public struct GeomRule
        {
            /// <summary>Путь к данным полигона</summary>
            public string PolygonPath;

            /// <summary>Путь к данным линии</summary>
            public string LineStringPath;

            /// <summary>Путь к данным точки</summary>
            public string PointPath;

            /// <summary>Путь к координате X</summary>
            public string XPath;

            /// <summary>Путь к координате Y</summary>
            public string YPath;

            /// <summary>Тип геометрии</summary>
            public Geometry.GeometryType Type;
        }

        /// <summary>Имя поля</summary>
        public string FName;

        /// <summary>Имя без пробелов (для AutoCAD Map 3D)</summary>
        public string CorrectName => LayerRule.TrimString(FName.Replace(" ", ""));

        /// <summary>Путь к данным</summary>
        public string FPath;

        /// <summary>Имя атрибута-источника</summary>
        public string FAttr;

        /// <summary>Является ли поле геометрией</summary>
        public bool IsGeom;

        private GeomRule _gr;
        //
        //public string dict;

        /// <summary>Получение значения атрибута, возвращает string.Empty если атрибута нет</summary>
        /// <param name="wEl"> xml-элемент </param>
        /// <param name="attrName"> имя атрибута </param>
        /// <returns></returns>
        private string GetElAttr(XmlElement wEl, string attrName)
        {
            return wEl.HasAttribute(attrName) ? wEl.GetAttribute(attrName) : string.Empty;
        }

        /// <summary>Загрузка описания геометрии</summary>
        /// <param name="wEl"> Описание геометрии </param>
        private void LoadGeomRule(XmlElement wEl)
        {
            try
            {
                var ch = wEl;
                var ex = true;
                do
                {
                    switch (ch.LocalName)
                    {
                        case "Point":
                            _gr.PointPath = GetElAttr(ch, "Path");
                            _gr.XPath = GetElAttr(ch, "Xattr");
                            _gr.YPath = GetElAttr(ch, "Yattr");
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.Point;
                            ex = false;
                            break;
                        case "LineString":
                            _gr.LineStringPath = GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.LineString;
                            break;
                        case "Polygon":
                            _gr.PolygonPath = GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.Polygon;
                            break;
                        case "MultiPolygon":
                            ch = (XmlElement)ch.FirstChild;
                            _gr.Type = Geometry.GeometryType.MultiPolygon;
                            break;
                    }
                } while (ex);
            }
            catch (Exception e) { throw new RuleLoadException("Ошибка чтения правила геометрии", e); }
        }

        /// <summary>Создание правила</summary>
        /// <param name="wEl"> Описание правил </param>
        internal FieldRule(XmlElement wEl)
        {
            FName = GetElAttr(wEl, "Name");
            if (FName != string.Empty)
            {
                FPath = GetElAttr(wEl, "Path");
                FAttr = GetElAttr(wEl, "Attr");
                if (GetElAttr(wEl, "Geom").ToLower() == "true") { IsGeom = true; }
                if (IsGeom) LoadGeomRule((XmlElement)wEl.FirstChild);
            }
            else throw new RuleLoadException("Нет имени поля");
        }

        /// <summary>Получение типа геометрии поля</summary>
        /// <returns> тип геометрии </returns>
        public Geometry.GeometryType GetGeomType()
        {
            return _gr.Type;
        }

        /// <summary>Загрузка данных точки</summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNm"> Список пространств имён файла данных </param>
        /// <returns> точка </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка чтения координат точки - отрицательные координаты </exception>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в Point </exception>
        private Geometry.TPoint LoadPoint(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                double x = -1, y = -1;
                //WorkStr = wNode.Attributes.GetNamedItem(this.GR.XPath).Value;
                var xmlNode2 = wNode.SelectSingleNode(_gr.XPath, wNm);
                if (xmlNode2 != null)
                {
                    var workStr = xmlNode2.Value;
                    x = double.Parse(workStr, CultureInfo.InvariantCulture);
                    //WorkStr = wNode.Attributes.GetNamedItem(this.GR.YPath).Value;
                    xmlNode2 = wNode.SelectSingleNode(_gr.YPath, wNm);
                    if (xmlNode2 != null)
                    {
                        workStr = xmlNode2.Value;
                        y = double.Parse(workStr, CultureInfo.InvariantCulture);
                    }
                }
                if (x == -1 || y == -1) throw new Data.DataLoadException("Ошибка чтения координат точки");
                return new Geometry.TPoint(x, y);
            }
            catch (XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в Point", e);
            }
        }

        /// <summary>Загрузка данных линии</summary>
        /// <param name="wNode">Узел с данными</param>
        /// <param name="wNm">Список пространств имён файла данных</param>
        /// <returns>Линия</returns>
        /// <exception cref="Data.DataLoadException">Ошибка XPath в LineString</exception>
        private Geometry.TLineString LoadLineString(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                var crNodes = wNode.SelectNodes(_gr.PointPath, wNm);
                var coords = (from XmlNode n in crNodes select LoadPoint(n, wNm)).ToList();
                return new Geometry.TLineString(coords);
            }
            catch (XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в LineString", e);
            }
        }

        /// <summary>Загрузка данных полигона</summary>
        /// <param name="wNode">Узел с данными</param>
        /// <param name="wNm">Список пространств имён файла данных</param>
        /// <returns> Полигон </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в Polygon </exception>
        private Geometry.TPolygon LoadPolygon(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                var crNodes = wNode.SelectNodes(_gr.LineStringPath, wNm);
                var rings = (from XmlNode n in crNodes select LoadLineString(n, wNm)).ToList();
                return new Geometry.TPolygon(rings);
            }
            catch (XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в Polygon", e);
            }
        }

        private Geometry.TMultiPolygon LoadMultiPolygon(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                var crNodes = wNode.SelectNodes(_gr.PolygonPath, wNm);
                var nPolygons = (from XmlNode n in crNodes select LoadPolygon(n, wNm)).ToList();
                return new Geometry.TMultiPolygon(nPolygons);
            }
            catch (XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в MultiPolygon", e);
            }
        }

        /// <summary>Загрузка данных геометрии</summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNm"> Список пространств имён файла данных </param>
        /// <returns> Геометрия </returns>
        internal Geometry.TGeometry LoadGeometry(XmlNode wNode, XmlNamespaceManager wNm)
        {
            switch (GetGeomType())
            {
                case Geometry.GeometryType.Point:
                    return LoadPoint(wNode, wNm);

                case Geometry.GeometryType.LineString:
                    return LoadLineString(wNode, wNm);

                case Geometry.GeometryType.Polygon:
                    return LoadPolygon(wNode, wNm);

                case Geometry.GeometryType.MultiPolygon:
                    return LoadMultiPolygon(wNode, wNm);

                default:
                    return null;
            }
        }
    }
}