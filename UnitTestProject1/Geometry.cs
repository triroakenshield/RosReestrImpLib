using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RosReestrImp.Geometry;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {

        [TestCategory("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestMyPointWKT()
        {
            var p = new MyPoint(11.111, 22.222);
            var test = p.ToWKT2D();
            Assert.IsTrue(true, "good");
        }

        [TestCategory("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestTLineStringToShortWKT2D()
        {
            var ls = new TLineString(new List<MyPoint>(){
            new(11.111, 22.222),
            new(33.333, 44.444),
            new(55.555, 66.666)
            });
            var test1 = ls.ToShortWKT2D();
            var test2 = ls.ToWKT2D();
            var test3 = ls.RingToShortWKT2D();
            var test4 = ls.RingToWKT2D();
            Assert.IsTrue(true, "good");
        }

        [TestCategory("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestMTPolygonToShortWKT2D()
        {
            var ls1 = new TLineString(new List<MyPoint>(){
            new(0, 0),
            new(1, 0),
            new(1, 1),
            new(0, 1) });

            var ls2 = new TLineString(new List<MyPoint>(){
            new(0.25, 0.25),
            new(0.75, 0.25),
            new(0.75, 0.75),
            new(0.25, 0.75) });

            var poly = new TPolygon(new List<TLineString>() { ls1, ls2 });
            var test1 = poly.ToShortWKT2D();
            var test2 = poly.ToWKT2D();
            var tp = poly.GetType().ToString();
            Assert.IsTrue(true, "good");
        }

        [TestCategory("Geometry"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void TestGeometryType()
        {
            var np = new TPoint(0, 0);
            var tp = np.GetType().ToString();

            var nl = new TLineString(new List<MyPoint>()
            {
                new(11.111, 22.222),
                new(33.333, 44.444),
                new(55.555, 66.666)
            });

            var tl = nl.GetType().ToString();
            Assert.IsTrue(true, "good");
        }
    }
}