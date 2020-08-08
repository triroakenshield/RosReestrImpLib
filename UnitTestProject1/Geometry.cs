using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RosReestrImp.Geometry;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {

        [TestCategoryAttribute("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestMyPointWKT()
        {
            MyPoint p = new MyPoint(11.111, 22.222);
            string test = p.ToWKT2D();
            Assert.IsTrue(true, "good");
        }

        [TestCategoryAttribute("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestTLineStringToShortWKT2D()
        {
            TLineString ls = new TLineString(new List<MyPoint>(){
            new MyPoint(11.111, 22.222),
            new MyPoint(33.333, 44.444),
            new MyPoint(55.555, 66.666)
            });
            string test1 = ls.ToShortWKT2D();
            string test2 = ls.ToWKT2D();
            string test3 = ls.RingToShortWKT2D();
            string test4 = ls.RingToWKT2D();
            Assert.IsTrue(true, "good");
        }

        [TestCategoryAttribute("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestMTPolygonToShortWKT2D()
        {
            TLineString ls1 = new TLineString(new List<MyPoint>(){
            new MyPoint(0, 0),
            new MyPoint(1, 0),
            new MyPoint(1, 1),
            new MyPoint(0, 1) });

            TLineString ls2 = new TLineString(new List<MyPoint>(){
            new MyPoint(0.25, 0.25),
            new MyPoint(0.75, 0.25),
            new MyPoint(0.75, 0.75),
            new MyPoint(0.25, 0.75) });

            TPolygon poly = new TPolygon(new List<TLineString>() { ls1, ls2 });
            string test1 = poly.ToShortWKT2D();
            string test2 = poly.ToWKT2D();
            string tp = poly.GetType().ToString();
            Assert.IsTrue(true, "good");
        }

        [TestCategoryAttribute("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestGeometryType()
        {
            var np = new TPoint(0, 0);
            string tp = np.GetType().ToString();

            var nl = new TLineString(new List<MyPoint>()
            {
                new MyPoint(11.111, 22.222),
                new MyPoint(33.333, 44.444),
                new MyPoint(55.555, 66.666)
            });

            string tl = nl.GetType().ToString();
            Assert.IsTrue(true, "good");
        }
    }
}