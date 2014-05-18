using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Presentation.Converters;

namespace Waf.Writer.Presentation.Test.Converters
{
    [TestClass]
    public class DoubleToZoomConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            DoubleToZoomConverter converter = DoubleToZoomConverter.Default;
            Assert.AreEqual(75d, converter.Convert(0.75, null, null, null));
        }

        [TestMethod]
        public void ConvertBackTest()
        {
            DoubleToZoomConverter converter = DoubleToZoomConverter.Default;
            Assert.AreEqual(0.75, converter.ConvertBack(75d, null, null, null));
        }
    }
}
