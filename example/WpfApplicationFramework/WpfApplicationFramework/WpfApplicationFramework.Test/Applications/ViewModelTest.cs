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
    public class ViewModelTest
    {
        [TestMethod]
        public void GetView() 
        {
            IView view = new MockView();
            MockViewModel viewModel = new MockViewModel(view);
            Assert.AreEqual(view, viewModel.View);

            MockViewModel viewModel2 = new MockViewModel(view, false);
            Assert.AreEqual(view, viewModel2.View);
        }

        [TestMethod]
        public void ConstructorParameter()
        {
            AssertHelper.ExpectedException<ArgumentNullException>(() => new MockViewModel(null));
            AssertHelper.ExpectedException<ArgumentNullException>(() => new MockViewModel(null, false));
        }



        private class MockViewModel : ViewModel
        {
            public MockViewModel(IView view) : base(view)
            {
            }

#pragma warning disable 618
            public MockViewModel(IView view, bool isChild) : base(view, isChild)
            {
            }
#pragma warning restore 618
        }

        private class MockView : IView
        {
            public object DataContext { get; set;}
        }
    }
}
