using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using RosReestrImp.Geometry;
// ReSharper disable InconsistentNaming

namespace RosReestrImp.Acad
{
    /// <summary>Расширения для экспорта геометрии в AutoCAD</summary>
    public static class GeometryExtensions
    {
        /// <summary>Конвертируем внутреннею структура MyPoint в AutoCAD-структуру Point2d</summary>
        /// <param name="wp">структура MyPoint</param>
        /// <returns></returns>
        public static Point2d GetPoint2d(this MyPoint wp) => new Point2d(wp.X, wp.Y);

        /// <summary>Конвертируем линию в MPolygon</summary>
        /// <param name="wLine"></param>
        /// <returns></returns>
        public static MPolygonLoop GetMPolygonLoop(this TLineString wLine)
        {
            var res = new MPolygonLoop();
            wLine.Coords.ForEach(tp => res.Add(new BulgeVertex(tp.GetPoint2d(), 0)));
            return res;
        }

        private static MPolygonLoopCollection GetMPolygonLoopCollection(this TPolygon wPoly)
        {
            var res = new MPolygonLoopCollection();
            wPoly.Rings.ForEach(wl => res.Add(wl.GetMPolygonLoop()));
            return res;
        }

        /// <summary>Конвертируем объект точки в точку AutoCAD'а</summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        public static DBPoint GetDBPoint(this TPoint wp) 
            => new DBPoint(new Point3d(wp.Coord.X, wp.Coord.Y, wp.Coord.Z));

        /// <summary>Конвертируем линию в полилинию AutoCAD'а</summary>
        /// <param name="wl"></param>
        /// <returns></returns>
        public static Polyline GetPolyline(this TLineString wl)
        {
            var nPoly = new Polyline();
            wl.Coords.ForEach(wp => nPoly.AddVertexAt(0, wp.GetPoint2d(), 0, 0, 0));
            return nPoly;
        }

        /// <summary>Конвертируем полигон в MPolygon</summary>
        /// <param name="wp"></param>
        /// <returns></returns>
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

        /// <summary>Конвертируем внутреннею геометрию в примитив AutoCAD'а</summary>
        /// <param name="wg"></param>
        /// <returns></returns>
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