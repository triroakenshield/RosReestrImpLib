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

        [TestCategoryAttribute("FieldRule"), TestCategoryAttribute("RuleLoadException"), TestMethod]
        public void LoadEmpFieldRule()
        {
            try
            {
                RuleManager wRM = new RuleManager("Shema\\emptyFieldRule.xml");
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
                RuleManager wRM = new RuleManager("Shema\\emptyPolygon.xml");
            }
            catch (RuleLoadException e)
            {
                Assert.IsTrue(e.Message == "Ошибка чтения правила геометрии", "good");
            }
        }

    }
}
