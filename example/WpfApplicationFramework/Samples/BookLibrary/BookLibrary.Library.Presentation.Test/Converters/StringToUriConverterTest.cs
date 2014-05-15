using System;
using Waf.BookLibrary.Library.Presentation.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Waf.BookLibrary.Library.Presentation.Test.Converters
{
    [TestClass]
    public class StringToUriConverterTest
    {
        [TestMethod]
        public void ConvertTest()
        {
            StringToUriConverter converter = StringToUriConverter.Default;

            Uri uri = (Uri)converter.Convert("harry.potter@hogwarts.edu", null, null, null);
            Assert.AreEqual("harry.potter@hogwarts.edu", uri.OriginalString);
            
            uri = (Uri)converter.Convert("wrongAddress", null, null, null);
            Assert.AreEqual("wrongAddress", uri.OriginalString);

            uri = (Uri)converter.Convert(null, null, null, null);
            Assert.AreEqual("", uri.OriginalString);
        }
        
        [TestMethod]
        public void ConvertBackTest()
        {
            StringToUriConverter converter = StringToUriConverter.Default;

            Assert.AreEqual("harry.pott", converter.ConvertBack("harry.pott", null, null, null));

            Assert.AreEqual("harry.pott", converter.ConvertBack(new Uri("harry.pott", UriKind.RelativeOrAbsolute), null, null, null));
        }
    }
}
