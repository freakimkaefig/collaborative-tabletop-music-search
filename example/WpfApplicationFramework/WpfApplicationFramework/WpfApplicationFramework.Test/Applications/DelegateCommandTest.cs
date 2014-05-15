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
    public class DelegateCommandTest
    {
        public TestContext TestContext { get; set; }


        [TestMethod]
        public void ExecuteTest()
        {
            bool executed = false;
            DelegateCommand command = new DelegateCommand(() => executed = true);

            command.Execute(null);
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void ExecuteTest2()
        {
            bool executed = false;
            object commandParameter = null;
            DelegateCommand command = new DelegateCommand((object parameter) =>
            {
                executed = true;
                commandParameter = parameter;
            });

            object obj = new object();
            command.Execute(obj);
            Assert.IsTrue(executed);
            Assert.AreEqual(obj, commandParameter);
        }

        [TestMethod]
        public void ExecuteTest3()
        {
            bool executed = false;
            bool canExecute = true;
            DelegateCommand command = new DelegateCommand(() => executed = true, () => canExecute);

            command.Execute(null);
            Assert.IsTrue(executed);
        }

        [TestMethod]
        public void ExecuteTest4()
        {
            bool executed = false;
            bool canExecute = false;
            DelegateCommand command = new DelegateCommand(() => executed = true, () => canExecute);

            AssertHelper.ExpectedException<InvalidOperationException>(() => command.Execute(null));
            Assert.IsFalse(executed);
        }

        [TestMethod]
        public void RaiseCanExecuteChangedTest()
        {
            bool executed = false;
            bool canExecute = false;
            DelegateCommand command = new DelegateCommand(() => executed = false, () => canExecute);
            
            Assert.IsFalse(command.CanExecute(null));
            canExecute = true;
            Assert.IsTrue(command.CanExecute(null));

            AssertHelper.CanExecuteChangedEvent(command, () => command.RaiseCanExecuteChanged());
            
            Assert.IsFalse(executed);
        }

        [TestMethod]
        public void ConstructorParameterTest()
        {
            AssertHelper.ExpectedException<ArgumentNullException>(() => new DelegateCommand((Action)null));

            AssertHelper.ExpectedException<ArgumentNullException>(() => new DelegateCommand((Action<object>)null));
        }

        [TestMethod]
        public void MemoryTest()
        {
            DelegateCommand command = new DelegateCommand(() => { });

            MockView view1 = new MockView();
            command.CanExecuteChanged += view1.CanExecuteChangedHandler;

            command.RaiseCanExecuteChanged();
            Assert.IsTrue(view1.CanExecuteChangedHandlerCalled);

            // Check that view1 is collected by the garbage collector.

            WeakReference weakReference = new WeakReference(view1);
            view1 = null;
            GC.Collect();
            Assert.IsFalse(weakReference.IsAlive);


            MockView view2 = new MockView();
            command.CanExecuteChanged += view2.CanExecuteChangedHandler;
            
            command.RaiseCanExecuteChanged();
            Assert.IsTrue(view2.CanExecuteChangedHandlerCalled);

            // Check that after unwiring view2 the eventhandler is not called anymore.

            command.CanExecuteChanged -= view2.CanExecuteChangedHandler;

            view2.Clear();
            command.RaiseCanExecuteChanged();
            Assert.IsFalse(view2.CanExecuteChangedHandlerCalled);
        }


        private class MockView
        {
            public bool CanExecuteChangedHandlerCalled { get; private set; }
            
            
            public void CanExecuteChangedHandler(object sender, EventArgs e)
            {
                CanExecuteChangedHandlerCalled = true;
            }

            public void Clear()
            {
                CanExecuteChangedHandlerCalled = false;
            }
        }
    }
}
