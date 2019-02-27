using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

namespace RosReestrImp.Rule
{
    /// <summary>
    /// Описание поля слоя
    /// элемент FieldRule
    /// </summary>
    public class FieldRule
    {
        /// <summary>
        /// Описание геометрии
        /// </summary>
        public struct GeomRule
        {

            public string PolygonPath;

            /// <summary>
            /// Путь к данным линии
            /// </summary>
            public string LineStringPath;

            /// <summary>
            /// Путь к данным точки
            /// </summary>
            public string PointPath;

            /// <summary>
            /// Путь к координате X
            /// </summary>
            public string XPath;

            /// <summary>
            /// Путь к координате Y
            /// </summary>
            public string YPath;

            /// <summary>
            /// Тип геометрии
            /// </summary>
            public Geometry.GeometryType Type;
        }

        /// <summary>
        /// Имя поля
        /// </summary>
        public string FName = "";

        /// <summary>
        /// Путь к данным
        /// </summary>
        public string FPath = "";

        /// <summary>
        /// Имя атрибута-источника
        /// </summary>
        public string FAttr = "";

        /// <summary>
        /// Является ли поле геометрией
        /// </summary>
        public bool IsGeom = false;
        private GeomRule GR;
        //
        //public string dict;

        /// <summary>
        /// Получение значения атрибута, возвращает string.Empty если атрибута нет
        /// </summary>
        /// <param name="wEl"> xml-элемент </param>
        /// <param name="AttrName"> имя атрибута </param>
        /// <returns></returns>
        private string GetElAttr(XmlElement wEl, string AttrName)
        {
            if (wEl.HasAttribute(AttrName)) return wEl.GetAttribute(AttrName);
            else return string.Empty;
        }

        /// <summary>
        /// Загрузка описания геометрии
        /// </summary>
        /// <param name="wEl"> Описание геометрии </param>
        private void LoadGeomRule(XmlElement wEl)
        {
            try
            {
                XmlElement ch = wEl;
                bool ex = true;
                do
                {
                    switch (ch.LocalName)
                    {
                        case "Point":
                            this.GR.PointPath = this.GetElAttr(ch, "Path");
                            this.GR.XPath = this.GetElAttr(ch, "Xattr");
                            this.GR.YPath = this.GetElAttr(ch, "Yattr");
                            if (this.GR.Type == Geometry.GeometryType.No)
                            {
                                this.GR.Type = Geometry.GeometryType.Point;
                            }
                            ex = false;
                            break;
                        case "LineString":
                            this.GR.LineStringPath = this.GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (this.GR.Type == Geometry.GeometryType.No)
                            {
                                this.GR.Type = Geometry.GeometryType.LineString;
                            }
                            break;
                        case "Polygon":
                            this.GR.PolygonPath = this.GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (this.GR.Type == Geometry.GeometryType.No) this.GR.Type = Geometry.GeometryType.Polygon;
                            break;
                        case "MultiPolygon":
                            ch = (XmlElement)ch.FirstChild;
                            this.GR.Type = Geometry.GeometryType.MultiPolygon;
                            break;
                    }
                } while (ex);
            }
            catch (Exception e) 
            {
                throw new RuleLoadException("Ошибка чтения правила геометрии", e);
            }
        }

        /// <summary>
        /// Создание правила
        /// </summary>
        /// <param name="wEl"> Описание правил </param>
        internal FieldRule(XmlElement wEl)
        {
            this.FName = this.GetElAttr(wEl, "Name");
            if (this.FName != string.Empty)
            {
                this.FPath = this.GetElAttr(wEl, "Path");
                this.FAttr = this.GetElAttr(wEl, "Attr");
                if (this.GetElAttr(wEl, "Geom").ToLower() == "true") { this.IsGeom = true; }
                if (this.IsGeom) this.LoadGeomRule((XmlElement)wEl.FirstChild);
            }
            else throw new RuleLoadException("Нет имени поля");
        }

        /// <summary>
        /// Получение типа геометрии поля
        /// </summary>
        /// <returns> тип геометрии </returns>
        public Geometry.GeometryType GetGeomType()
        {
            return this.GR.Type;
        }

        /// <summary>
        /// Загрузка данных точки
        /// </summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <returns> точка </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка чтения координат точки - отрицательные координаты </exception>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в Point </exception>
        private Geometry.TPoint LoadPoint(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                double x = -1, y = -1;
                string WorkStr;
                //WorkStr = wNode.Attributes.GetNamedItem(this.GR.XPath).Value;
                XmlNode XmlNode2 = wNode.SelectSingleNode(this.GR.XPath, wNM);
                if (XmlNode2 != null)
                {
                    WorkStr = XmlNode2.Value;
                    x = double.Parse(WorkStr, System.Globalization.CultureInfo.InvariantCulture);
                    //WorkStr = wNode.Attributes.GetNamedItem(this.GR.YPath).Value;
                    XmlNode2 = wNode.SelectSingleNode(this.GR.YPath, wNM);
                    if (XmlNode2 != null)
                    {
                        WorkStr = XmlNode2.Value;
                        y = double.Parse(WorkStr, System.Globalization.CultureInfo.InvariantCulture);
                    }
                }
                if (x == -1 || y == -1) throw new Data.DataLoadException("Ошибка чтения координат точки");
                return new Geometry.TPoint(x, y);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в Point", e);
            }
        }

        /// <summary>
        /// Загрузка данных линии
        /// </summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <returns> Линия </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в LineString </exception>
        private Geometry.TLineString LoadLineString(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                List<Geometry.TPoint> Coords = new List<Geometry.TPoint>();
                XmlNodeList crNodes = wNode.SelectNodes(this.GR.PointPath, wNM);
                foreach (XmlNode n in crNodes)
                {
                    Coords.Add(this.LoadPoint(n, wNM));
                }
                return new Geometry.TLineString(Coords);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в LineString", e);
            }
        }

        /// <summary>
        /// Загрузка данных полигона
        /// </summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <returns> Полигон </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в Polygon </exception>
        private Geometry.TPolygon LoadPolygon(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                List<Geometry.TLineString> Rings = new List<Geometry.TLineString>();
                XmlNodeList crNodes = wNode.SelectNodes(this.GR.LineStringPath, wNM);
                foreach (XmlNode n in crNodes)
                {
                    Rings.Add(this.LoadLineString(n, wNM));
                }
                return new Geometry.TPolygon(Rings);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в Polygon", e);
            }
        }

        private Geometry.TMultiPolygon LoadMultiPolygon(XmlNode wNode, XmlNamespaceManager wNM)
        {
            try
            {
                List<Geometry.TPolygon> nPolygons = new List<Geometry.TPolygon>();
                //
                XmlNodeList crNodes = wNode.SelectNodes(this.GR.PolygonPath, wNM);
                foreach (XmlNode n in crNodes)
                {
                    nPolygons.Add(this.LoadPolygon(n, wNM));
                }
                //
                return new Geometry.TMultiPolygon(nPolygons);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в MultiPolygon", e);
            }
        }

        /// <summary>
        /// Загрузка данных геометрии
        /// </summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNM"> Список пространств имён файла данных </param>
        /// <returns> Геометрия </returns>
        internal Geometry.TGeometry LoadGeometry(XmlNode wNode, XmlNamespaceManager wNM)
        {
            switch (this.GetGeomType())
            {
                case Geometry.GeometryType.Point:
                    return this.LoadPoint(wNode, wNM);

                case Geometry.GeometryType.LineString:
                    return this.LoadLineString(wNode, wNM);

                case Geometry.GeometryType.Polygon:
                    return this.LoadPolygon(wNode, wNM);

                case Geometry.GeometryType.MultiPolygon:
                    return this.LoadMultiPolygon(wNode, wNM);

                default:
                    return null;
            }
        }

    }
}
