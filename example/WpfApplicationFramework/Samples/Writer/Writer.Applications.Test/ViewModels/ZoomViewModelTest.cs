using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Waf.Applications;
using Waf.Writer.Applications.ViewModels;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.Test.Views;
using System.Waf.UnitTesting;

namespace Waf.Writer.Applications.Test.ViewModels
{
    [TestClass]
    public class ZoomViewModelTest
    {
        [TestMethod]
        public void ShellServiceZoomSyncTest()
        {
            ShellService shellService = new ShellService();
            MockZoomViewModel zoomViewModel = new MockZoomViewModel(new MockView(), shellService);

            // The ShellService.Zoom and ZoomViewModel are not synchronized as long IsVisible == false
            shellService.Zoom = 2;
            zoomViewModel.Zoom = 0.5;
            Assert.AreEqual(2, shellService.Zoom);
            Assert.AreEqual(0.5, zoomViewModel.Zoom);

            // Set IsVisible = true
            AssertHelper.PropertyChangedEvent(zoomViewModel, x => x.IsVisible, () => zoomViewModel.IsVisible = true);
            Assert.AreEqual(0.5, shellService.Zoom);
            Assert.AreEqual(0.5, zoomViewModel.Zoom);

            // Now the changes are synchronized between the ShellService.Zoom and the ZoomViewModel
            AssertHelper.PropertyChangedEvent(zoomViewModel, x => x.Zoom, () => shellService.Zoom = 3);
            Assert.AreEqual(3, zoomViewModel.Zoom);

            AssertHelper.PropertyChangedEvent(shellService, x => x.Zoom, () => zoomViewModel.Zoom = 0.75);
            Assert.AreEqual(0.75, shellService.Zoom);
        }
        
        private class MockZoomViewModel : ZoomViewModel<IView>
        {
            public MockZoomViewModel(IView view, IShellService shellService) : base(view, shellService)
            {
            }
        }
    }
}
