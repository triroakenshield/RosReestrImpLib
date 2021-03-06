﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
//
using Autodesk.Gis.Map;
using Autodesk.Gis.Map.ObjectData;
using JetBrains.Annotations;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace RosReestrImp
{
    /// <summary>Команды для AutoCAD Civil 3D</summary>
    [UsedImplicitly]
    public class MyCommands
    {
        /// <summary>Создаём таблицу ObjectData.</summary>
        /// <param name="wRule"> LayerRule как источник описания таблицы </param>
        /// <returns> ObjectData.Table </returns>
        public Autodesk.Gis.Map.ObjectData.Table CreateOdTable(Rule.LayerRule wRule)
        {
            var mapApp = HostMapApplicationServices.Application;
            var activeProject = mapApp.ActiveProject;
            var tableList = activeProject.ODTables;
            //
            if (!tableList.GetTableNames().Contains(wRule.CorrectName))
            {
                var fieldDefs = mapApp.ActiveProject.MapUtility.NewODFieldDefinitions();
                foreach (var fr in from Rule.FieldRule fr in wRule.FieldList where !fr.IsGeom select fr)
                {
                    fieldDefs.Add(fr.CorrectName, "", Autodesk.Gis.Map.Constants.DataType.Character, 0);//!!1
                }
                tableList.Add(wRule.CorrectName, fieldDefs, "", true);
            }
            //
            return tableList[wRule.CorrectName];
        }

        /// <summary>Создаём Point2d из Geometry.TGeometry.MyPoint</summary>
        /// <param name="wp"> Geometry.TGeometry.MyPoint </param>
        /// <returns> Point2d </returns>
        public Point2d ConvertPoint2d(Geometry.MyPoint wp)
        {
            return new Point2d(wp.X, wp.Y);
        }

        /// <summary>Создаём MPolygonLoop из Geometry.TLineString</summary>
        /// <param name="wLine"></param>
        /// <returns> MPolygonLoop </returns>
        public MPolygonLoop ConvertLineString(Geometry.TLineString wLine)
        {
            var res = new MPolygonLoop();
            wLine.Coords.ForEach(tp => res.Add(new BulgeVertex(ConvertPoint2d(tp), 0)));
            return res;
        }

        /// <summary>Создаём MPolygonLoopCollection из Geometry.TPolygon</summary>
        /// <param name="wPoly"> Geometry.TPolygon </param>
        /// <returns> MPolygonLoopCollection </returns>
        public MPolygonLoopCollection ConvertPolygon(Geometry.TPolygon wPoly)
        {
            var res = new MPolygonLoopCollection();
            wPoly.Rings.ForEach(wl => res.Add(ConvertLineString(wl)));
            return res;
        }

        /// <summary>Создаём DBPoint из Geometry.TPoint </summary>
        /// <param name="wp"> Geometry.TPoint </param>
        /// <returns> DBPoint </returns>
        public DBPoint MakePoint(Geometry.TPoint wp)
        {
            return new DBPoint(new Point3d(wp.Coord.X, wp.Coord.Y, wp.Coord.Z));
        }

        /// <summary>Создаём Polyline из Geometry.TLineString</summary>
        /// <param name="wl"> Geometry.TLineString </param>
        /// <returns> Polyline </returns>
        public Polyline MakePolyLine(Geometry.TLineString wl)
        {
            var nPoly = new Polyline();
            wl.Coords.ForEach(wp => nPoly.AddVertexAt(0, new Point2d(wp.X, wp.Y), 0, 0, 0));
            return nPoly;
        }

        /// <summary>Создаём MPolygon из Geometry.TPolygon</summary>
        /// <param name="wp"> Geometry.TPolygon </param>
        /// <returns> MPolygon </returns>
        public MPolygon MakePolygon(Geometry.TPolygon wp)
        {
            var mpoly = new MPolygon();
            var acPolyColl = ConvertPolygon(wp);
            foreach (MPolygonLoop loop in acPolyColl)
            {
                mpoly.AppendMPolygonLoop(loop, false, 0);
            }
            mpoly.PatternScale = 50;
            mpoly.PatternSpace = 50;
            mpoly.SetPattern(HatchPatternType.PreDefined, "ANSI37");
            return mpoly;
        }

        /// <summary>Добавляем ObjectData-атрибуты к примитиву чертежа</summary>
        /// <param name="wid"> ObjectId примитива </param>
        /// <param name="wr"> Данные атрибутов </param>
        /// <param name="wTbl"> Таблица ObjectData </param>
        public void AddAttr(ObjectId wid, Data.MyRecord wr, Autodesk.Gis.Map.ObjectData.Table wTbl)
        {
            if (wTbl == null) return;
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            using (acDoc.LockDocument())
            {
                using (var acTrans = acDoc.Database.TransactionManager.StartTransaction())
                {
                    using (var odRecord = Record.Create())
                    {
                        wTbl.InitRecord(odRecord);
                        for (var i = 0; i < wTbl.FieldDefinitions.Count; i++)
                        {
                            var fdef = wTbl.FieldDefinitions[i];
                            var fval = wr.SearchField(fdef.Name);
                            if (fval != null)
                            {
                                if (!fval.IsGeom)
                                {
                                    odRecord[i].Assign(fval.GetString());
                                }
                            }
                        }
                        wTbl.AddRecord(odRecord, wid);
                        acTrans.Commit();
                    }
                }
            }
        }

        /// <summary>Создаём AutoCAD-геометрию из TGeometry</summary>
        /// <param name="wg"> геометрия </param>
        /// <returns></returns>
        public Entity MakeGeometry(Geometry.TGeometry wg)
        {
            switch (wg.GetGeometryType())
            {
                case Geometry.GeometryType.Point:
                    return MakePoint((Geometry.TPoint)wg);

                case Geometry.GeometryType.LineString:
                    return MakePolyLine((Geometry.TLineString)wg);

                case Geometry.GeometryType.Polygon:
                    return MakePolygon((Geometry.TPolygon)wg);

                default:
                    return null;
            }
        }

        /// <summary>Добавляем примитивы в БД чертежа</summary>
        /// <param name="wEntList"> список примитивов </param>
        public void DrawEntity(List<Entity> wEntList)
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurDb = acDoc.Database;
            using (var acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                var acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                var acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (var ent in wEntList)
                {
                    acBlkTblRec.AppendEntity(ent);
                    acTrans.AddNewlyCreatedDBObject(ent, true);
                }
                acTrans.Commit();
            }
        }

        /// <summary>Добавляем примитив в БД чертежа</summary>
        /// <param name="ent"> примитив </param>
        /// <returns></returns>
        public ObjectId DrawEntity(Entity ent)
        {
            var acDoc = Application.DocumentManager.MdiActiveDocument;
            var acCurDb = acDoc.Database;
            using (var acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                var acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                var acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], 
                    OpenMode.ForWrite);
                acBlkTblRec.AppendEntity(ent);
                acTrans.AddNewlyCreatedDBObject(ent, true);
                acTrans.Commit();
                return ent.ObjectId;
            }
        }

        /// <summary>Рисуем слой</summary>
        /// <param name="wl"> слой </param>
        public void DrawLayer(Data.DataLayer wl)
        {
            if (!wl.HasGeometry()) return;
            Autodesk.Gis.Map.ObjectData.Table wTbl = null;
            if (wl.HasAttributes()) wTbl = CreateOdTable(wl._Rule);
            foreach (var wr in wl.Table)
            {
                var g = wr.GetGeometry();
                if (g == null || g.IsEmpty() || !g.IsValid()) continue;
                if (wl.HasAttributes()) AddAttr(DrawEntity(MakeGeometry(g)), wr, wTbl);
            }
        }

        private string FilterString = "xml files (*.xml)|*.xml";

        /// <summary>Импорт xml росреестра, используется файл правил - rule.xml</summary>
        [UsedImplicitly,CommandMethod("ImportXML")]
        public void ImportXml()
        {
            var rulePath = Assembly.GetExecutingAssembly().Location.Replace("RosReestrImp.dll", "rule.xml");
            var wRm = new Rule.RuleManager(rulePath);
            var openFileDialog1 = new OpenFileDialog { Filter = FilterString };
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            wRm.LoadData(openFileDialog1.FileName).ForEach(DrawLayer);
        }

        /// <summary>Импорт xml росреестра, с запросом файла правил</summary>
        [UsedImplicitly,CommandMethod("ImportXMLwithRule")]
        public void ImportXmLwithRule()
        {
            var openFileDialog1 = new OpenFileDialog { Filter = FilterString };
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            var wRm = new Rule.RuleManager(openFileDialog1.FileName);
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            wRm.LoadData(openFileDialog1.FileName).ForEach(DrawLayer);
        }
    }
}