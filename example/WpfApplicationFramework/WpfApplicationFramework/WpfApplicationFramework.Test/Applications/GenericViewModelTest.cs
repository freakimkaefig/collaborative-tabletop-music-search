using System;
using System.Waf.Applications;
using System.Waf.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Waf.Applications
{
    [TestClass]
    public class GenericViewModelTest
    {
        [TestMethod]
        public void GetView()
        {
            IView view = new MockView();
            GenericMockViewModel viewModel = new GenericMockViewModel(view);
            Assert.AreEqual(view, viewModel.ViewInternal);

            GenericMockViewModel viewModel2 = new GenericMockViewModel(view, false);
            Assert.AreEqual(view, viewModel2.ViewInternal);
        }

        [TestMethod]
        public void ConstructorParameter()
        {
            AssertHelper.ExpectedException<ArgumentNullException>(() => new GenericMockViewModel(null));
            AssertHelper.ExpectedException<ArgumentNullException>(() => new GenericMockViewModel(null, false));
        }


        private class GenericMockViewModel : ViewModel<IView>
        {
            public GenericMockViewModel(IView view)
                : base(view)
            {
            }

#pragma warning disable 618
            public GenericMockViewModel(IView view, bool isChild)
                : base(view, isChild)
            {
            }
#pragma warning restore 618


            public IView ViewInternal { get { return ViewCore; } }
        }

        private class MockView : IView
        {
            public object DataContext { get; set; }
        }
    }
}
