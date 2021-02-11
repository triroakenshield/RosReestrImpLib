using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RosReestrImp.Data;
using RosReestrImp.Geometry;
// ReSharper disable InconsistentNaming
// ReSharper disable CheckNamespace

namespace MITAB
{
    /// <summary>Слой tab/mif</summary>
    public sealed class MiLayer
    {
        /// <summary>Следующий ключ</summary>
        internal int next_id = 1;

        /// <summary>Хэндл</summary>
        internal IntPtr _handle;

        /// <summary>Поля</summary>
        internal Fields _fields;

        /// <summary>Сущности</summary>
        internal Features _features;

        /// <summary>Имя файла</summary>
        internal string _fileName;

        /// <summary>Границы поля</summary>
        internal TMBR _bounds;

        /// <summary>Handle used to manipulate the object in the C API</summary>
        public IntPtr Handle => this._handle;

        /// <summary>Поля</summary>
        public Fields Fields => this._fields;

        /// <summary>Сущности</summary>
        public Features Features => this._features;

        /// <summary>Имя файла</summary>
        public string FileName { get { return _fileName; } }

        /// <summary>Конструктор</summary>
        /// <param name="handle">Хэндл</param>
        /// <param name="fileName">Имя файла</param>
        internal MiLayer(IntPtr handle, string fileName)
        {
            this._handle = handle;
            this._fields = CreateFields();
            this._features = CreateFeatures();
            this._fileName = fileName;
        }

        /// <summary>Конструктор</summary>
        /// <param name="fileName">Имя файла</param>
        internal MiLayer(string fileName)
        {
            this._handle = MiApi.mitab_c_open(fileName);
            if (this.Handle == IntPtr.Zero)
                throw new FileNotFoundException("File " + fileName + " not found", fileName);
            this._fields = CreateFields();
            this._features = CreateFeatures();
            this._fileName = fileName;
        }

        private List<List<Vertex>> GetParts(TGeometry geom)
        {
            var res = new List<List<Vertex>>();
            List<Vertex> plist;
            switch (geom.GetGeometryType())
            {
                case GeometryType.Point:
                    if (geom is TPoint p) res.Add(new List<Vertex>() {new Vertex(p.Coord.X, p.Coord.Y)});
                    break;
                case GeometryType.LineString:
                    plist = new List<Vertex>();
                    if (geom is TLineString ls) plist.AddRange(ls.Coords
                        .Select(np => new Vertex(np.X, np.Y)));

                    res.Add(plist);
                    break;
                case GeometryType.Polygon:
                    if (geom is TPolygon poly)
                        foreach (var ring in poly.Rings)
                        {
                            plist = ring.Coords.Select(np => new Vertex(np.X, np.Y)).ToList();
                            res.Add(plist);
                        }

                    break;
                case GeometryType.MultiPolygon:
                    if (geom is TMultiPolygon mPoly)
                        foreach (var ring in mPoly.Geometries.OfType<TPolygon>()
                            .SelectMany(sPoly => sPoly.Rings))
                        {
                            plist = ring.Coords.Select(np => new Vertex(np.X, np.Y)).ToList();
                            res.Add(plist);
                        }

                    break;
                case GeometryType.No:
                    break;
                case GeometryType.GeometryCollection:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return res;
        }

        private List<string> GetFieldValues(MyRecord rec)
        {
            var res = new List<string>();
            foreach (var f in rec.FieldList) if (!f.IsGeom) res.Add(f.GetString());
            return res;
        }

        /// <summary>Добавить сущность</summary>
        /// <param name="rec">Запись</param>
        /// <returns></returns>
        public Feature AddFeature(MyRecord rec)
        {
            var geom = rec.GetGeometry();
            var type = FeatureType.TABFC_NoGeom;
            if (geom == null)
                return this.AddFeature(type, new List<List<Vertex>>(),
                    GetFieldValues(rec), null);
            switch (geom.GetGeometryType())
            {
                case GeometryType.Point:
                    type = FeatureType.TABFC_Point;
                    break;
                case GeometryType.LineString:
                    type = FeatureType.TABFC_Polyline;
                    break;
                case GeometryType.Polygon:
                case GeometryType.MultiPolygon:
                    type = FeatureType.TABFC_Region;
                    break;
                case GeometryType.No:
                    break;
                case GeometryType.GeometryCollection:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (geom.IsValid()) return this.AddFeature(type, this.GetParts(geom), GetFieldValues(rec), null);
            return type == FeatureType.TABFC_NoGeom ? this.AddFeature(type, new List<List<Vertex>>(), 
                GetFieldValues(rec), null) : null;
        }

        private void AddLayer(DataLayer lay)
        {
            foreach (var fr in lay.Rule.FieldList)
            {
                if (!fr.IsGeom) this.AddField(fr.FName, FieldType.TABFT_Char, 250, 0, 0, 0);
            }
            foreach (var rec in lay.Table) this.AddFeature(rec);
        }

        /// <summary>Создать tab-файл</summary>
        /// <param name="tabFileName">Имя файла</param>
        /// <param name="lay">Слой данных</param>
        /// <returns></returns>
        public static MiLayer CreateTab(string tabFileName, DataLayer lay)
        {
            var bounds = lay.GetMBR();
            if (bounds == null) return null;
            var res = new MiLayer(MiApi.mitab_c_create(tabFileName, "tab", "NonEarth Units \"m\"", 
                bounds.maxy, bounds.miny, bounds.minx, bounds.maxx), tabFileName);
            //
            res.AddLayer(lay);
            res.Close();
            return res;
        }

        /// <summary>Создать mif-файл</summary>
        /// <param name="tabFileName">Имя файла</param>
        /// <param name="lay">Слой данных</param>
        /// <returns></returns>
        public static MiLayer CreateMif(string tabFileName, DataLayer lay)
        {
            var bounds = lay.GetMBR();
            if (bounds == null) return null;
            var res = new MiLayer(MiApi.mitab_c_create(tabFileName, "mif", "NonEarth Units \"m\"", 
                bounds.maxy, bounds.miny, bounds.minx, bounds.maxx), tabFileName);
            res.AddLayer(lay);
            res.Close();
            return res;
        }

        /// <summary>Override this to support descendants of the Fields class.</summary>
        /// <returns>This layers fields</returns>
        internal Fields CreateFields()
        {
            return new Fields(this);
        }

        /// <summary>Override this to support descendants of the Feature class.</summary>
        /// <returns>This layers features</returns>
        internal Features CreateFeatures()
        {
            return new Features(this);
        }

        /// <summary>Factory method to return the layer with a given name.</summary>
        /// <param name="tabFileName"></param>
        /// <returns></returns>
        public static MiLayer GetByName(string tabFileName)
        {
            return new MiLayer(tabFileName);
        }

        /// <summary>Создать tab-файл</summary>
        /// <param name="tabFileName">Имя файла</param>
        /// <returns></returns>
        public static MiLayer CreateTab(string tabFileName)
        {
            return new MiLayer(MiApi.mitab_c_create(tabFileName, "tab", null,
                0, 0, 0, 0), tabFileName);
        }

        /// <summary>Создать mif-файл</summary>
        /// <param name="tabFileName">Имя файла</param>
        /// <returns></returns>
        public static MiLayer CreateMif(string tabFileName)
        {
            return new MiLayer(MiApi.mitab_c_create(tabFileName, "mif", null,
                0, 0, 0, 0), tabFileName);
        }

        /// <summary>Добавить поле</summary>
        /// <param name="fieldName">Имя поля</param>
        /// <param name="fieldType">Тип поля</param>
        /// <param name="width">Ширина</param>
        /// <param name="precision">Точность</param>
        /// <param name="indexed"></param>
        /// <param name="unique"></param>
        public void AddField(string fieldName, FieldType fieldType, int width, int precision, int indexed, int unique)
        {
            this.Fields.AddField(this, fieldName, fieldType, width, precision, indexed, unique);
        }

        /// <summary>Добавить сущность</summary>
        /// <param name="type">Тип</param>
        /// <param name="nParts">Части</param>
        /// <param name="nFieldValues">Значения полей</param>
        /// <param name="nStyle">Стили</param>
        /// <returns></returns>
        public Feature AddFeature(FeatureType type, List<List<Vertex>> nParts, List<string> nFieldValues, 
            Dictionary<string, string> nStyle)
        {
            return this.Features.AddFeature(type, this.next_id++, nParts, nFieldValues, nStyle);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"Layer: {this.FileName}";
        }

        /// <summary>Writes this layers features to the given textwriter</summary>
        /// <param name="writer">Destintation for the layers features</param>
        public void ToText(TextWriter writer)
        {
            writer.WriteLine(this);
            writer.WriteLine($"{this.Fields}\n");
            writer.WriteLine(this.Features);
        }

        /// <summary>Writes this layers features as a text file.</summary>
        /// <param name="fileName">The name of the file that will be created.</param>
        public void ToText(string fileName)
        {
            ToText(new StreamWriter(fileName));
        }

        /// <summary>Закрыть файл</summary>
        public void Close()
        {
            MiApi.mitab_c_close(this.Handle);
        }
    }
}