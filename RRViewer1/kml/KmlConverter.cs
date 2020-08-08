using System.Linq;
//
using DotSpatial.Projections;
//
using RosReestrImp.Data;
using RosReestrImp.Geometry;
//
using SharpKml.Base;
using SharpKml.Dom;

namespace RRViewer1.kml
{
    /// <summary>Конвертер в kml</summary>
    public class KmlConverter
    {
        readonly static ProjectionInfo WGS1984 = KnownCoordinateSystems.Geographic.World.WGS1984;

        private ProjectionInfo Projection;
        private DataLayer Layer;

        public KmlConverter(DataLayer layer, ProjectionInfo projection)
        {
            Projection = projection;
            Layer = layer;
        }

        public Document GetKmlDocument()
        {
            var document = new Document();

            foreach (var record in Layer.Table)
            {
                document.AddFeature(GetKml(record));
            }

            return document;
        }

        private Placemark GetKml(MyRecord record)
        {
            return new Placemark() { 
                Name = record.Rule.Entpath, 
                Geometry = GetKmlGeometry(record.GetGeometry()), 
                Description = new Description() { Text = record.GetFieldsString() 
            }};
        }

        private Geometry GetKmlGeometry(TGeometry geom)
        {
            if (geom.IsEmpty() || !geom.IsValid()) return null;
            switch (geom.GetType().ToString())
            {
                case "RosReestrImp.Geometry.TPoint": return GetKml((TPoint)geom);
                case "RosReestrImp.Geometry.TLineString": return GetKml((TLineString)geom);
                case "RosReestrImp.Geometry.TPolygon": return GetKml((TPolygon)geom);
                case "RosReestrImp.Geometry.TGeometryCollection":
                case "RosReestrImp.Geometry.TMultiPolygon":
                    return GetKml((TGeometryCollection)geom);
                default: return null;
            }
        }

        private (double[] xys, double[] zs) ReprojectGeometry(TGeometry geometry)
        {
            var xys = geometry.GetXYArray();
            var zs = geometry.GetZArray();
            Reproject.ReprojectPoints(xys, zs, Projection, WGS1984, 0, zs.Length);
            return (xys, zs);
        }

        private Point GetKml(TPoint point)
        {
            var (xys, zs) = ReprojectGeometry(point);
            return new Point() { Coordinate = new Vector(xys[0], xys[1]) };
        }

        private LineString GetKml(TLineString line)
        {
            var (xys, zs) = ReprojectGeometry(line);
            var kml = new LineString();
            var cc = new CoordinateCollection();
            for (var i = 0; i < line.Coords.Count; i++)
            {
                cc.Add(new Vector(xys[i * 2 + 1], xys[i * 2]));
            }
            kml.Coordinates = cc;
            return kml;
        }

        private LinearRing GetRingKml(TLineString line)
        {
            var (xys, zs) = ReprojectGeometry(line);
            var kml = new LinearRing();
            var cc = new CoordinateCollection();
            for (var i = 0; i < line.Coords.Count; i++)
            {
                cc.Add(new Vector(xys[i * 2 + 1], xys[i * 2]));
            }
            kml.Coordinates = cc;
            return kml;
        }

        private Geometry GetKml(TPolygon polygon)
        {
            if (polygon.Rings.Count == 0) return null;

            var outher = polygon.GetOuterBoundary();

            if (outher == null) return GetKml(polygon.AsCollection());

            var outherKml = new OuterBoundary { LinearRing = GetRingKml(outher) };

            var kml = new Polygon { OuterBoundary = outherKml };

            foreach (var ring in polygon.Rings)
            {
                if (ring != outher)
                {
                    var inner = new InnerBoundary() { LinearRing = GetRingKml(ring) };
                    kml.InnerBoundary.Append<InnerBoundary>(inner);
                }
            }
            return kml;
        }

        private MultipleGeometry GetKml(TGeometryCollection geom)
        {
            var kml = new MultipleGeometry();
            foreach (var sub in geom.Geometries)
            {
                kml.AddGeometry(GetKmlGeometry(sub));
            }
            return kml;
        }
    }
}