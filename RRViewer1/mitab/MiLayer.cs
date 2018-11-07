using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using RosReestrImp.Geometry;
using RosReestrImp.Rule;
using RosReestrImp.Data;

namespace MITAB
{
    public class MiLayer
    {
        protected internal int next_id = 1;
        protected internal IntPtr _handle;
        protected internal Fields _fields;
        protected internal Features _features;
        protected internal string _fileName;
        protected internal TMBR _bounds;

        /// <summary>
        /// Handle used to manipulate the object in the C API
        /// </summary>
        public IntPtr Handle { get { return this._handle; } }
        public Fields Fields { get { return this._fields; } }
        public Features Features { get { return this._features; } }
        public string FileName { get { return this._fileName; } }

        protected internal MiLayer(IntPtr handle, string fileName)
        {
            this._handle = handle;
            this._fields = CreateFields();
            this._features = CreateFeatures();
            this._fileName = fileName;
        }

        protected internal MiLayer(string fileName)
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
            List<List<Vertex>> res = new List<List<Vertex>>();
            List<Vertex> plist;
            switch (geom.GetGeometryType())
            {
                case GeometryType.Point:
                    TPoint p = geom as TPoint;
                    res.Add(new List<Vertex>() { new Vertex(p.Coord.X, p.Coord.Y) });
                    break;
                case GeometryType.LineString:
                    TLineString ls = geom as TLineString;
                    plist = new List<Vertex>();
                    foreach (MyPoint np in ls.Coords)
                    {
                        plist.Add(new Vertex(np.X, np.Y));
                    }
                    res.Add(plist);
                    break;
                case GeometryType.Polygon:
                    TPolygon poly = geom as TPolygon;
                    foreach (TLineString ring in poly.Rings)
                    {
                        plist = new List<Vertex>();
                        foreach (MyPoint np in ring.Coords)
                        {
                            plist.Add(new Vertex(np.X, np.Y));
                        }
                        res.Add(plist);
                    }
                    break;
            }
            return res;
        }

        private List<String> GetFieldValues(MyRecord rec)
        {
            List<String> res = new List<String>();
            foreach (FieldValue f in rec.FileldList)
            {
                if (!f.IsGeom) res.Add(f.GetString());
            }
            return res;
        }

        public Feature AddFeature(MyRecord rec)
        {
            TGeometry geom = rec.GetGeometry();
            FeatureType type = FeatureType.TABFC_NoGeom;
            if (geom != null)
            {
                switch (geom.GetGeometryType())
                {
                    case GeometryType.Point:
                        type = FeatureType.TABFC_Point;
                        break;
                    case GeometryType.LineString:
                        type = FeatureType.TABFC_Polyline;
                        break;
                    case GeometryType.Polygon:
                        type = FeatureType.TABFC_Region;
                        break;
                }
            }
            if (geom.IsValid()) this.AddFeature(type, this.GetParts(geom), GetFieldValues(rec), null);
            else this.AddFeature(FeatureType.TABFC_NoGeom, new List<List<Vertex>>(), GetFieldValues(rec), null);
            return null;
        }

        private void AddLayer(DataLayer lay)
        {
            foreach (FieldRule fr in lay.Rule.FieldList)
            {
                if (!fr.IsGeom) this.AddField(fr.FName, FieldType.TABFT_Char, 250, 0, 0, 0);
            }
            foreach (MyRecord rec in lay.Table)
            {
                this.AddFeature(rec);
            }
        }

        public static MiLayer CreateTAB(string tabFileName, DataLayer lay)
        {
            TMBR bounds = lay.GetMBR();
            MiLayer res = new MiLayer(MiApi.mitab_c_create(tabFileName, "tab", "NonEarth Units \"m\"", 
                bounds.maxy, bounds.miny, bounds.minx, bounds.maxx), tabFileName);
            //
            res.AddLayer(lay);
            res.Close();
            return res;
        }

        public static MiLayer CreateMIF(string tabFileName, DataLayer lay)
        {
            TMBR bounds = lay.GetMBR();
            MiLayer res = new MiLayer(MiApi.mitab_c_create(tabFileName, "mif", "NonEarth Units \"m\"", 
                bounds.maxy, bounds.miny, bounds.minx, bounds.maxx), tabFileName);
            res.AddLayer(lay);
            res.Close();
            return res;
        }

        /// <summary>
        /// Override this to support descendants of the Fields class.
        /// </summary>
        /// <returns>This layers fields</returns>
        protected internal virtual Fields CreateFields()
        {
            return new Fields(this);
        }

        /// <summary>
        /// Override this to support descendants of the Feature class.
        /// </summary>
        /// <returns>This layers features</returns>
        protected internal virtual Features CreateFeatures()
        {
            return new Features(this);
        }

        /// <summary>
        /// Factory method to return the layer with a given name.
        /// </summary>
        /// <param name="tabFileName"></param>
        /// <returns></returns>
        public static MiLayer GetByName(string tabFileName)
        {
            return new MiLayer(tabFileName);
        }

        public static MiLayer CreateTAB(string tabFileName)
        {
            return new MiLayer(MiApi.mitab_c_create(tabFileName, "tab", null, 0, 0, 0, 0), tabFileName);
        }

        public static MiLayer CreateMIF(string tabFileName)
        {
            return new MiLayer(MiApi.mitab_c_create(tabFileName, "mif", null, 0, 0, 0, 0), tabFileName);
        }

        public void AddField(string field_name, FieldType field_type, int width, int precision, int indexed, int unique)
        {
            this.Fields.AddField(this, field_name, field_type, width, precision, indexed, unique);
        }

        public Feature AddFeature(FeatureType type, List<List<Vertex>> nParts, List<String> nFieldValues, Dictionary<string, string> nStyle)
        {
            return this.Features.AddFeature(type, this.next_id++, nParts, nFieldValues, nStyle);
        }

        public override string ToString()
        {
            return "Layer: " + this.FileName;
        }

        /// <summary>
        /// Writes this layers features to the given textwriter
        /// </summary>
        /// <param name="writer">Destintation for the layers features</param>
        public void ToText(TextWriter writer)
        {
            writer.WriteLine(this);
            writer.WriteLine(this.Fields + "\n");
            writer.WriteLine(this.Features);
        }

        /// <summary>
        /// Writes this layers features as a text file.
        /// </summary>
        /// <param name="fileName">The name of the file that will be created.</param>
        public void ToText(string fileName)
        {
            ToText(new StreamWriter(fileName));
        }

        public void Close()
        {
            MiApi.mitab_c_close(this.Handle);
        }

    }
}
