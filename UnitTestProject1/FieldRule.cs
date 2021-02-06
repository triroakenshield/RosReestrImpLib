using Microsoft.VisualStudio.TestTools.UnitTesting;
//
using RosReestrImp.Rule;

namespace UnitTestProject1
{
    public partial class UnitTest1
    {

        [TestCategoryAttribute("FieldRule"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadEmpFieldRule()
        {
            try
            {
                var workRuleManager = new RuleManager("Shema\\emptyFieldRule.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Нет имени поля", "good");
            }
        }

        [TestCategoryAttribute("FieldRule"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadEmpPolygon()
        {
            try
            {
                var workRuleManager = new RuleManager("Shema\\emptyPolygon.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка чтения правила геометрии", "good");
            }
        }

    }
}
