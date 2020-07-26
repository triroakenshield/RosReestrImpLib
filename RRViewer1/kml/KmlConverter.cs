using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using DotSpatial.Projections;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
//
using RosReestrImp.Rule;
using RosReestrImp.Geometry;
using RosReestrImp.Data;
using RosReestrImp.Projections;

namespace RRViewer1.kml
{
    public static class KmlConverter
    {
        readonly static ProjectionInfo WGS1984 = KnownCoordinateSystems.Geographic.World.WGS1984;

        public static Document GetKmlDocument(this DataLayer layer, ProjectionInfo projection)
        {
            var document = new Document();

            foreach(var record in layer.Table)
            {
                var ch = record.GetKml(projection);
                document.AddFeature(ch);
            }

            return document;
        }

        public static Placemark GetKml(this MyRecord record, ProjectionInfo projection)
        {
            var feature = new Placemark();
            var geom = record.GetGeometry() as TPolygon;

            feature.Geometry = geom.GetKml(projection);
            feature.Name = "test";

            return feature;
        }

        public static Point GetKml(this TPoint point, ProjectionInfo projection)
        {
            if (projection == null) return null;

            var xys = new double[] { point.Coord.X, point.Coord.Y };
            var zs = new double[] { 0 };
            Reproject.ReprojectPoints(xys, zs, projection, WGS1984, 0, 1);

            var kmlPoint = new Point();
            kmlPoint.Coordinate = new Vector(xys[0], xys[1]);
            return kmlPoint;
        }

        public static LinearRing GetRingKml(this TLineString ring, ProjectionInfo projection)
        {
            if (projection == null) return null;

            var xys = ring.GetXYArray();
            var zs = ring.GetZArray();
            Reproject.ReprojectPoints(xys, zs, projection, WGS1984, 0, ring.Coords.Count);

            var kml = new LinearRing();
            var cc = new CoordinateCollection();
            for (var i = 0; i < ring.Coords.Count; i++)
            {
                cc.Add(new Vector(xys[i * 2 + 1], xys[i * 2]));
            }
            kml.Coordinates = cc;
            return kml;
        }

        public static Polygon GetKml(this TPolygon polygon, ProjectionInfo projection)
        {
            if (projection == null) return null;
            if (polygon.Rings.Count == 0) return null;

            var outher = new OuterBoundary();
            outher.LinearRing = polygon.Rings[0].GetRingKml(projection);

            var kml = new Polygon();
            kml.OuterBoundary = outher;

            return kml;
        }
    }
}