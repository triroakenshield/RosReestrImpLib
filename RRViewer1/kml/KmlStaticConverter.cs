//
using DotSpatial.Projections;

using RosReestrImp.Data;
using RosReestrImp.Geometry;

using SharpKml.Base;
using SharpKml.Dom;

namespace RRViewer1.kml
{
    public static class KmlStaticConverter
    {
        static readonly ProjectionInfo Wgs1984 = KnownCoordinateSystems.Geographic.World.WGS1984;

        /// <summary></summary>
        /// <param name="layer"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static Document GetKmlDocument(this DataLayer layer, ProjectionInfo projection)
        {
            var document = new Document();

            foreach (var record in layer.Table)
            {
                var ch = record.GetKml(projection);
                document.AddFeature(ch);
            }

            return document;
        }

        /// <summary></summary>
        /// <param name="record"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static Placemark GetKml(this MyRecord record, ProjectionInfo projection)
        {
            var feature = new Placemark();
            var geom = record.GetGeometry() as TPolygon;

            feature.Geometry = geom.GetKml(projection);
            feature.Name = "test";

            return feature;
        }

        /// <summary></summary>
        /// <param name="point"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static Point GetKml(this TPoint point, ProjectionInfo projection)
        {
            if (projection == null) return null;

            var xys = new[] { point.Coord.X, point.Coord.Y };
            var zs = new double[] { 0 };
            Reproject.ReprojectPoints(xys, zs, projection, Wgs1984, 0, 1);

            var kmlPoint = new Point();
            kmlPoint.Coordinate = new Vector(xys[0], xys[1]);
            return kmlPoint;
        }

        /// <summary></summary>
        /// <param name="line"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static LineString GetKml(this TLineString line, ProjectionInfo projection)
        {
            return null;
        }

        /// <summary></summary>
        /// <param name="ring"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static LinearRing GetRingKml(this TLineString ring, ProjectionInfo projection)
        {
            if (projection == null) return null;

            var xys = ring.GetXYArray();
            var zs = ring.GetZArray();
            Reproject.ReprojectPoints(xys, zs, projection, Wgs1984, 0, ring.Coords.Count);

            var kml = new LinearRing();
            var cc = new CoordinateCollection();
            for (var i = 0; i < ring.Coords.Count; i++)
            {
                cc.Add(new Vector(xys[i * 2 + 1], xys[i * 2]));
            }
            kml.Coordinates = cc;
            return kml;
        }

        /// <summary></summary>
        /// <param name="polygon"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public static Polygon GetKml(this TPolygon polygon, ProjectionInfo projection)
        {
            if (projection == null) return null;
            if (polygon.Rings.Count == 0) return null;

            var outher = new OuterBoundary {LinearRing = polygon.Rings[0].GetRingKml(projection)};

            var kml = new Polygon {OuterBoundary = outher};

            return kml;
        }
    }
}