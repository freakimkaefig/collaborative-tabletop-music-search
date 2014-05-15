using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.Services;
using System.Threading;
using System.Globalization;

namespace Waf.Writer.Applications.Test.Services
{
    [TestClass]
    public class ShellServiceTest
    {
        private CultureInfo currentCulture;

        
        [TestInitialize]
        public void TestInitialize()
        {
            currentCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Thread.CurrentThread.CurrentCulture = currentCulture;
        }
        
        
        [TestMethod]
        public void DefaultZoomsTest()
        {
            ShellService shellService = new ShellService();
            shellService.DefaultZooms.SequenceEqual(new[] { "200%", "150%", "125%", "100%", "75%", "50%", "25%" });
        }
        
        [TestMethod]
        public void ZoomTest()
        {
            ShellService shellService = new ShellService();
            Assert.AreEqual(1d, shellService.Zoom);
            
            shellService.Zoom = 0.75;
            Assert.AreEqual(0.75, shellService.Zoom);

            shellService.Zoom = 0.1;
            Assert.AreEqual(0.25, shellService.Zoom);

            shellService.Zoom = 25;
            Assert.AreEqual(16, shellService.Zoom);
        }
    }
}
