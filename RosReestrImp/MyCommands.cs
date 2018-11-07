using System;
using System.Collections.Generic;
//
using System.Reflection;
using System.Windows.Forms;
//
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using AppServ = Autodesk.AutoCAD.ApplicationServices;
//
using Autodesk.Gis.Map.ObjectData;

namespace RosReestrImp
{
    /// <summary>
    /// Команды для AutoCAD Civil 3D
    /// </summary>
    public class MyCommands
    {
        //Autodesk.Gis.Map.ObjectData.Table crODTable;

        /// <summary>
        /// Создаём таблицу ObjectData.
        /// </summary>
        /// <param name="wRule"> LayerRule как источник описания таблицы </param>
        /// <returns> ObjectData.Table </returns>
        public Autodesk.Gis.Map.ObjectData.Table CreateODTable(Rule.LayerRule wRule)
        {
            Autodesk.Gis.Map.MapApplication mapApp = Autodesk.Gis.Map.HostMapApplicationServices.Application;
            Autodesk.Gis.Map.Project.ProjectModel activeProject = mapApp.ActiveProject;
            Autodesk.Gis.Map.ObjectData.Tables tableList = activeProject.ODTables;
            //
            if (!tableList.GetTableNames().Contains(wRule.LName))
            {
                FieldDefinitions fieldDefs = mapApp.ActiveProject.MapUtility.NewODFieldDefinitions();
                foreach (Rule.FieldRule fr in wRule.FieldList)
                {
                    if (!fr.IsGeom)
                    {
                        fieldDefs.Add(fr.FName, "", Autodesk.Gis.Map.Constants.DataType.Character, 0); //!!1
                    }
                }
                tableList.Add(wRule.LName, fieldDefs, "", true);
            }
            //
            return tableList[wRule.LName];
        }

        /// <summary>
        /// Создаём Point2d из Geometry.TGeometry.MyPoint
        /// </summary>
        /// <param name="wp"> Geometry.TGeometry.MyPoint </param>
        /// <returns> Point2d </returns>
        public Point2d ConvertPoint2d(Geometry.MyPoint wp)
        {
            return new Point2d(wp.X, wp.Y);
        }

        /// <summary>
        /// Создаём MPolygonLoop из Geometry.TLineString
        /// </summary>
        /// <param name="wLine"></param>
        /// <returns> MPolygonLoop </returns>
        public MPolygonLoop ConvertLineString(Geometry.TLineString wLine)
        {
            MPolygonLoop res = new MPolygonLoop();
            wLine.Coords.ForEach(tp => res.Add(new BulgeVertex(ConvertPoint2d(tp), 0)));
            return res;
        }

        /// <summary>
        /// Создаём MPolygonLoopCollection из Geometry.TPolygon
        /// </summary>
        /// <param name="wPoly"> Geometry.TPolygon </param>
        /// <returns> MPolygonLoopCollection </returns>
        public MPolygonLoopCollection ConvertPolygon(Geometry.TPolygon wPoly)
        {
            MPolygonLoopCollection res = new MPolygonLoopCollection();
            wPoly.Rings.ForEach(wl => res.Add(ConvertLineString(wl)));
            return res;
        }

        /// <summary>
        /// Создаём DBPoint из Geometry.TPoint 
        /// </summary>
        /// <param name="wp"> Geometry.TPoint </param>
        /// <returns> DBPoint </returns>
        public DBPoint MakePoint(Geometry.TPoint wp)
        {
            return new DBPoint(new Point3d(wp.Coord.X, wp.Coord.Y, wp.Coord.Z));
        }

        /// <summary>
        /// Создаём Polyline из Geometry.TLineString
        /// </summary>
        /// <param name="wl"> Geometry.TLineString </param>
        /// <returns> Polyline </returns>
        public Polyline MakePolyLine(Geometry.TLineString wl)
        {
            Polyline nPoly = new Polyline();
            wl.Coords.ForEach(wp => nPoly.AddVertexAt(0, new Point2d(wp.X, wp.Y), 0, 0, 0));
            return nPoly;
        }

        /// <summary>
        /// Создаём MPolygon из Geometry.TPolygon
        /// </summary>
        /// <param name="wp"> Geometry.TPolygon </param>
        /// <returns> MPolygon </returns>
        public MPolygon MakePolygon(Geometry.TPolygon wp)
        {
            MPolygon mpoly = new MPolygon();
            MPolygonLoopCollection acPolyColl = ConvertPolygon(wp);
            foreach (MPolygonLoop loop in acPolyColl)
            {
                mpoly.AppendMPolygonLoop(loop, false, 0);
            }
            mpoly.PatternScale = 50;
            mpoly.PatternSpace = 50;
            mpoly.SetPattern(HatchPatternType.PreDefined, "ANSI37");
            return mpoly;
        }

        /// <summary>
        /// Добавляем ObjectData-атрибуты к примитиву чертежа
        /// </summary>
        /// <param name="wid"> ObjectId примитива </param>
        /// <param name="wr"> Данные атрибутов </param>
        /// <param name="wTbl"> Таблица ObjectData </param>
        public void AddAttr(ObjectId wid, Data.MyRecord wr, Autodesk.Gis.Map.ObjectData.Table wTbl)
        {
            AppServ.Document acDoc = AppServ.Application.DocumentManager.MdiActiveDocument;
            using (AppServ.DocumentLock acLckDoc = acDoc.LockDocument())
            {
                using (Transaction acTrans = acDoc.Database.TransactionManager.StartTransaction())
                {
                    using (Record odRecord = Record.Create())
                    {
                        wTbl.InitRecord(odRecord);
                        FieldDefinition fdef;
                        Data.FieldValue fval;
                        for (int i = 0; i < wTbl.FieldDefinitions.Count; i++)
                        {
                            fdef = wTbl.FieldDefinitions[i];
                            fval = wr.SearchField(fdef.Name);
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

        /// <summary>
        /// Создаём AutoCAD-геометрию из TGeometry
        /// </summary>
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

        /// <summary>
        /// Добавляем примитивы в БД чертежа
        /// </summary>
        /// <param name="wEntlist"> список примитивов </param>
        public void DrawEntity(List<Entity> wEntlist)
        {
            AppServ.Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                foreach (Entity ent in wEntlist)
                {
                    acBlkTblRec.AppendEntity(ent);
                    acTrans.AddNewlyCreatedDBObject(ent, true);
                }
                acTrans.Commit();
            }
        }

        /// <summary>
        /// Добавляем примитив в БД чертежа
        /// </summary>
        /// <param name="ent"> примитив </param>
        /// <returns></returns>
        public ObjectId DrawEntity(Entity ent)
        {
            AppServ.Document acDoc = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                BlockTable acBlkTbl = (BlockTable)acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead);
                BlockTableRecord acBlkTblRec = (BlockTableRecord)acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                acBlkTblRec.AppendEntity(ent);
                acTrans.AddNewlyCreatedDBObject(ent, true);
                acTrans.Commit();
                return ent.ObjectId;
            }
        }

        /// <summary>
        /// Рисуем слой
        /// </summary>
        /// <param name="wl"> слой </param>
        public void DrawLayer(Data.DataLayer wl)
        {
            //List<Entity> wEntlist = new List<Entity>();
            Autodesk.Gis.Map.ObjectData.Table wTbl = null;
            if (wl.HasAttributes()) wTbl = CreateODTable(wl._Rule);
            ObjectId wid;
            Entity wEnt;
            Geometry.TGeometry g;
            foreach (Data.MyRecord wr in wl.Table)
            {
                g = wr.GetGeometry();
                if (g != null)
                {
                    wEnt = MakeGeometry(g);
                    wid = DrawEntity(wEnt);
                    if (wl.HasAttributes()) AddAttr(wid, wr, wTbl);
                }
                //wEntlist.Add(wEnt);
            }
            //DrawEntity(wEntlist);
        }

        /// <summary>
        /// Импорт xml росреестра, используется файл правил - rule.xml
        /// </summary>
        [CommandMethod("ImportXML")]
        public void ImportXML()
        {
            string RulePath = Assembly.GetExecutingAssembly().Location.Replace("RosReestrImp.dll", "rule.xml");
            Rule.RuleManager wRM = new Rule.RuleManager(RulePath);
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                List<Data.DataLayer> res = wRM.LoadData(openFileDialog1.FileName);
                res.ForEach(l => DrawLayer(l));
            }
        }

        /// <summary>
        /// Импорт xml росреестра, с запросом файла правил
        /// </summary>
        [CommandMethod("ImportXMLwithRule")]
        public void ImportXMLwithRule()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "xml files (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Rule.RuleManager wRM = new Rule.RuleManager(openFileDialog1.FileName);
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    List<Data.DataLayer> res = wRM.LoadData(openFileDialog1.FileName);
                    res.ForEach(l => DrawLayer(l));
                }
            }
        }

    }
}
