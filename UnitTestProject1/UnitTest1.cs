using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
using System.IO;

using RRViewer1;

namespace UnitTestProject1
{
    [TestClass] public partial class UnitTest1
    {

        [TestMethod] public void LoadData1()
        {
            RuleManager wRM = new RuleManager("Schema\\testList3.xml");
            List<DataLayer> res = wRM.LoadData("doc9415874.xml");
            var dl1 = new MyLayerDataView(res[0]);
            var r1 = dl1[0] as MyRecordView;
            var dl2 = new MyLayerDataView(res[1]);
            var r2 = dl2[0] as MyRecordView;
            var cname = r1.GetComponentName();
            var tt1 = r1.GetType();
            var tt2 = r2.GetType();
            Assert.IsTrue(true, "good");
        }

        [TestMethod] public void CreateTLineString()
        {
            List<MyPoint> nCoords = new List<MyPoint>();
            nCoords.Add(new MyPoint(1, 1, 1));
            nCoords.Add(new MyPoint(2, 2, 2));
            nCoords.Add(new MyPoint(3, 3, 3));
            nCoords.Add(new MyPoint(4, 4, 4));
            TLineString nLS = new TLineString(nCoords);
            Assert.IsTrue(true, "good");
        }

        public XmlNamespaceManager LoadNamespace(XmlDocument wDoc)
        {
            XmlNamespaceManager res = new XmlNamespaceManager(wDoc.NameTable);
            res.PopScope();
            XmlElement rNode = wDoc.DocumentElement;
            string nStr;
            foreach (XmlAttribute attr in rNode.Attributes)
            {
                nStr = attr.Name;
                if (nStr.Contains("xmlns"))
                {
                    if (nStr.Length == 5)
                    {
                        res.AddNamespace("ns", attr.Value); //!!
                    }
                    else
                    {
                        res.AddNamespace(nStr.Replace("xmlns:", ""), attr.Value);
                    }
                }
            }
            return res;
        }

        [TestMethod] public void TestXPath1()
        {
            XmlDocument wDoc = new XmlDocument();
            wDoc.Load("doc9415874.xml");
            XmlElement wNode = wDoc.DocumentElement;
            XmlNamespaceManager wNM = this.LoadNamespace(wDoc);
            XmlNode XmlNode1 = wDoc.DocumentElement.SelectSingleNode("//ns:ObjectsRealty//ns:EntitySpatial", wNM);
            Assert.IsTrue(true, "good");
        }

        [TestMethod] public void TestHashSet()
        {
            HashSet<object> tSet = new HashSet<object>
            {
                1,
                2,
                3
            };
            //var bres = tSet.Contains(i => i == 4);
            //var res = tSet.First(i => i == 4);
            var res = tSet.FirstOrDefault(i => (int)i == 4);
        }

        [TestMethod] public void TestProjectionsLoad()
        {
            var projs = Projections.Load();
            var sk = projs.GetProjectionInfo("68", 1);
            var WGS1984 = KnownCoordinateSystems.Geographic.World.WGS1984;
            var xys = new double[] { 1206205.62d, 407690.08d };
            var zs = new double[] { 0 };
            Reproject.ReprojectPoints(xys, zs, sk, WGS1984, 0, 1);
        }

        [TestMethod] public void TestKml1()
        {
            var point = new Point();
            point.Coordinate = new Vector(-13.163959, -72.545992);

            // This is the Element we are going to save to the Kml file.
            var placemark = new Placemark();
            placemark.Geometry = point;
            placemark.Name = "Machu Picchu";

            // This allows us to save and Element easily.
            KmlFile kml = KmlFile.Create(placemark, false);
            using (var stream = System.IO.File.OpenWrite("my placemark.kml"))
            {
                kml.Save(stream);
            }
        }

        [TestMethod] public void TestReadBigXml()
        {
            string path = @"C:\work1\21_00_000000_2018-02-16_kpt10.xml";
            var fstream = File.OpenRead(path);
            StreamWriter sw = new StreamWriter(@"C:\work1\test1.txt");
            for (var i=0; i<100;i++)
            {
                byte[] bb = new byte[256];
                fstream.Read(bb, 0, 256);
                sw.WriteLine(System.Text.Encoding.UTF8.GetString(bb));
            }
            sw.Close();
            fstream.Close();
        }
    }
}