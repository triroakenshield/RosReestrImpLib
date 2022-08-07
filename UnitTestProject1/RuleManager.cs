using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RosReestrImp.Data;
//
using RosReestrImp.Rule;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {

        [TestCategory("RuleManager"), TestMethod] //TestCategoryAttribute("RuleManager"), 
        public void LoadRuleManager1()
        {
            var wRM = new RuleManager("Schema\\testList1.xml");
            Assert.IsNotNull(wRM, "good");
        }

        [TestCategory("RuleManager"), TestCategory("RuleLoadException"), TestMethod]
        public void LoadErrorXML()
        {
            try
            {
                var wRM = new RuleManager("Schema\\errorxml.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка открытия xml-файла", "good");
            }
        }

        [TestCategory("RuleManager"), TestCategory("RuleLoadException"), TestMethod]
        public void LoadEmpRule()
        {
            try
            {
                var wRM = new RuleManager("Schema\\empty.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "ShemaList не найден", "good");
            }
        }

        [TestCategory("RuleManager"), TestCategory("RuleLoadException"), TestMethod]
        public void LoadEmpShema()
        {
            try
            {
                var wRM = new RuleManager("Schema\\emptyShema.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "У Shema нет rootElem", "good");
            }
        }

        [TestCategory("DataLayer"), TestCategory("DataLoadException"), TestMethod]
        public void LoadEmpLayer()
        {
            try
            {
                var wRM = new RuleManager("Schema\\emptyLayer.xml");
                var res = wRM.LoadData("doc9415874.xml");
            }
            catch (DataLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка XPath при загрузке слоя ", "good");
            }
        }

        [TestCategory("RuleLoadException"), TestMethod]
        public void LoadShemaRule_rootElem()
        {
            try
            {
                var wRM = new RuleManager("Schema\\rootElem.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "У Shema нет rootElem", "good");
            }
        }
    }
} 