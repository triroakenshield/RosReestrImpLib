using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using RosReestrImp.Geometry;

namespace RosReestrImp.Acad
{
    public static class GeometryExtensions
    {
        public static Point2d GetPoint2d(this MyPoint wp) => new(wp.X, wp.Y);

        public static MPolygonLoop GetMPolygonLoop(this TLineString wLine)
        {
            var res = new MPolygonLoop();
            wLine.Coords.ForEach(tp => res.Add(new BulgeVertex(tp.GetPoint2d(), 0)));
            return res;
        }

        public static MPolygonLoopCollection GetMPolygonLoopCollection(this TPolygon wPoly)
        {
            var res = new MPolygonLoopCollection();
            wPoly.Rings.ForEach(wl => res.Add(wl.GetMPolygonLoop()));
            return res;
        }

        public static DBPoint GetDBPoint(this TPoint wp) => new(new Point3d(wp.Coord.X, wp.Coord.Y, wp.Coord.Z));

        public static Polyline GetPolyline(this TLineString wl)
        {
            var nPoly = new Polyline();
            wl.Coords.ForEach(wp => nPoly.AddVertexAt(0, wp.GetPoint2d(), 0, 0, 0));
            return nPoly;
        }

        public static MPolygon GetMPolygon(this TPolygon wp)
        {
            var mpoly = new MPolygon();
            var acPolyColl = wp.GetMPolygonLoopCollection();
            foreach (MPolygonLoop loop in acPolyColl)
            {
                mpoly.AppendMPolygonLoop(loop, false, 0);
            }

            mpoly.PatternScale = 50;
            mpoly.PatternSpace = 50;
            mpoly.SetPattern(HatchPatternType.PreDefined, "ANSI37");
            return mpoly;
        }

        public static Entity GetEntity(this TGeometry wg)
        {
            return wg.GetGeometryType() switch
            {
                GeometryType.Point => ((TPoint)wg).GetDBPoint(),
                GeometryType.LineString => ((TLineString)wg).GetPolyline(),
                GeometryType.Polygon => ((TPolygon)wg).GetMPolygon(),
                _ => null
            };
        }
    }
}