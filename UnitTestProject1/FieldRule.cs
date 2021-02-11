using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RosReestrImp.Rule;
using System.IO;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {
        [TestCategory("FieldRule"), TestCategory("RuleLoadException"), TestMethod]
        public void LoadEmpFieldRule()
        {
            try
            {
                var workRuleManager = new RuleManager("Schema\\emptyFieldRule.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Нет имени поля", "good");
            }
        }

        [TestCategory("FieldRule"), TestCategory("RuleLoadException"), TestMethod]
        public void LoadEmpPolygon()
        {
            try
            {
                var workRuleManager = new RuleManager(@"Schema\emptyPolygon.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка чтения правила геометрии", "good");
            }
        }

    }
}
