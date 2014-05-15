using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using System.Waf.UnitTesting;

namespace Test.Waf.Applications
{
    [TestClass]
    public class ConverterCollectionTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
#pragma warning disable 618
            var converterCollection = new ConverterCollection<string, string>(new string[] { }, x => "Test");
#pragma warning restore 618
        }
    }
}
