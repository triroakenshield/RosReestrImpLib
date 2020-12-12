﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
                XmlElement ch = wEl;
                bool ex = true;
                do
                {
                    switch (ch.LocalName)
                    {
                        case "Point":
                            this._gr.PointPath = this.GetElAttr(ch, "Path");
                            this._gr.XPath = this.GetElAttr(ch, "Xattr");
                            this._gr.YPath = this.GetElAttr(ch, "Yattr");
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.Point;
                            ex = false;
                            break;
                        case "LineString":
                            this._gr.LineStringPath = this.GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.LineString;
                            break;
                        case "Polygon":
                            this._gr.PolygonPath = this.GetElAttr(ch, "Path");
                            ch = (XmlElement)ch.FirstChild;
                            if (_gr.Type == Geometry.GeometryType.No) _gr.Type = Geometry.GeometryType.Polygon;
                            break;
                        case "MultiPolygon":
                            ch = (XmlElement)ch.FirstChild;
                            this._gr.Type = Geometry.GeometryType.MultiPolygon;
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

        /// <summary>Получение типа геометрии поля</summary>
        /// <returns> тип геометрии </returns>
        public Geometry.GeometryType GetGeomType()
        {
            return this._gr.Type;
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
                XmlNode xmlNode2 = wNode.SelectSingleNode(this._gr.XPath, wNm);
                if (xmlNode2 != null)
                {
                    var workStr = xmlNode2.Value;
                    x = double.Parse(workStr, System.Globalization.CultureInfo.InvariantCulture);
                    //WorkStr = wNode.Attributes.GetNamedItem(this.GR.YPath).Value;
                    xmlNode2 = wNode.SelectSingleNode(this._gr.YPath, wNm);
                    if (xmlNode2 != null)
                    {
                        workStr = xmlNode2.Value;
                        y = double.Parse(workStr, System.Globalization.CultureInfo.InvariantCulture);
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

        /// <summary>Загрузка данных линии</summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNm"> Список пространств имён файла данных </param>
        /// <returns> Линия </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в LineString </exception>
        private Geometry.TLineString LoadLineString(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                XmlNodeList crNodes = wNode.SelectNodes(this._gr.PointPath, wNm);
                List<Geometry.TPoint> coords = (from XmlNode n in crNodes select this.LoadPoint(n, wNm)).ToList();
                return new Geometry.TLineString(coords);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в LineString", e);
            }
        }

        /// <summary>Загрузка данных полигона</summary>
        /// <param name="wNode"> Узел с данными </param>
        /// <param name="wNm"> Список пространств имён файла данных </param>
        /// <returns> Полигон </returns>
        /// <exception cref="Data.DataLoadException"> Ошибка XPath в Polygon </exception>
        private Geometry.TPolygon LoadPolygon(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                XmlNodeList crNodes = wNode.SelectNodes(this._gr.LineStringPath, wNm);
                List<Geometry.TLineString> rings = (from XmlNode n in crNodes select this.LoadLineString(n, wNm)).ToList();
                return new Geometry.TPolygon(rings);
            }
            catch (System.Xml.XPath.XPathException e)
            {
                throw new Data.DataLoadException("Ошибка XPath в Polygon", e);
            }
        }

        private Geometry.TMultiPolygon LoadMultiPolygon(XmlNode wNode, XmlNamespaceManager wNm)
        {
            try
            {
                XmlNodeList crNodes = wNode.SelectNodes(this._gr.PolygonPath, wNm);
                List<Geometry.TPolygon> nPolygons = (from XmlNode n in crNodes select this.LoadPolygon(n, wNm)).ToList();
                return new Geometry.TMultiPolygon(nPolygons);
            }
            catch (System.Xml.XPath.XPathException e)
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
            switch (this.GetGeomType())
            {
                case Geometry.GeometryType.Point:
                    return this.LoadPoint(wNode, wNm);

                case Geometry.GeometryType.LineString:
                    return this.LoadLineString(wNode, wNm);

                case Geometry.GeometryType.Polygon:
                    return this.LoadPolygon(wNode, wNm);

                case Geometry.GeometryType.MultiPolygon:
                    return this.LoadMultiPolygon(wNode, wNm);

                default:
                    return null;
            }
        }
    }
}