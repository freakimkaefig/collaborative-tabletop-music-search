using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.ViewModels;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Waf.Writer.Applications.Documents;
using Waf.Writer.Applications.Test.Documents;
using Waf.Writer.Applications.Test.Services;
using System.Waf.Applications.Services;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class StartViewModelTest : TestClassBase
    {
        [TestMethod]
        public void GetFileService()
        {
            StartViewModel startViewModel = Container.GetExportedValue<StartViewModel>();
            Assert.IsNotNull(startViewModel.FileService);
        }
    }
}
