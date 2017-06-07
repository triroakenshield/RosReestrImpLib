using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RosReestrImp.Rule;
using RosReestrImp.Geometry;
using RosReestrImp.Data;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {

        [TestCategoryAttribute("RuleManager"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void LoadRuleManager1()
        {
            RuleManager wRM = new RuleManager("Shema\\testList1.xml");
            Assert.IsNotNull(wRM, "good");
        }

        [TestCategoryAttribute("RuleManager"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadErrorXML()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\errorxml.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка открытия xml-файла", "good");
            }
        }

        [TestCategoryAttribute("RuleManager"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadEmpRule()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\empty.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "ShemaList не найден", "good");
            }
        }

        [TestCategoryAttribute("RuleManager"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadEmpShema()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\emptyShema.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "У Shema нет rootElem", "good");
            }
        }

        [TestCategoryAttribute("Layer"), TestCategoryAttribute("DataLoadException"), TestMethod]
        public void LoadEmpLayer()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\emptyLayer.xml");
                List<Layer> res = wRM.LoadData("doc9415874.xml");
            }
            catch (DataLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка XPath при загрузке слоя ", "good");
            }
        }

        [TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadShemaRule_rootElem()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\rootElem.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "У Shema нет rootElem", "good");
            }
        }

    }
}
