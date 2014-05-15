using System;
using System.Waf.UnitTesting;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Presentation.Converters;

namespace Waf.Writer.Presentation.Test.Converters
{
    [TestClass]
    public class TabFileNameConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            TabFileNameConverter converter = TabFileNameConverter.Default;

            Assert.AreEqual("Document 1.rtf", 
                converter.Convert(new object[] { "Document 1.rtf", false }, null, null, null));
            Assert.AreEqual("Document 1.rtf*",
                converter.Convert(new object[] { "Document 1.rtf", true }, null, null, null));
            Assert.AreEqual("This is a document with a very long f...",
                converter.Convert(new object[] { "This is a document with a very long file name.rtf", false }, null, null, null));

            Assert.AreEqual(DependencyProperty.UnsetValue, converter.Convert(new object[] { new object(), new object() }, 
                typeof(string), null, null));

            AssertHelper.ExpectedException<NotImplementedException>(() =>
                converter.ConvertBack(null, null, null, null));
        }
    }
}
