using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.InformationManager.AddressBook.Modules.Applications.Controllers;
using Waf.InformationManager.AddressBook.Modules.Domain;
using Test.InformationManager.AddressBook.Modules.Applications.Views;

namespace Test.InformationManager.AddressBook.Modules.Applications.Controllers
{
    [TestClass]
    public class SelectContactControllerTest : AddressBookTest
    {
        [TestMethod]
        public void SelectContactTest()
        {
            var root = new AddressBookRoot();
            var contact1 = root.AddNewContact();
            var contact2 = root.AddNewContact();
            
            var controller = Container.GetExportedValue<SelectContactController>();

            controller.OwnerView = new object();
            controller.Root = root;

            controller.Initialize();

            MockSelectContactView.ShowDialogAction = view =>
            {
                controller.ContactListViewModel.SelectedContact = contact2;
                controller.SelectContactViewModel.OkCommand.Execute(null);
            };

            controller.Run();

            Assert.AreEqual(contact2, controller.SelectedContact);
            MockSelectContactView.ShowDialogAction = null;
        }

        [TestMethod]
        public void SelectContactAndCancelTest()
        {
            var root = new AddressBookRoot();
            var contact1 = root.AddNewContact();
            var contact2 = root.AddNewContact();

            var controller = Container.GetExportedValue<SelectContactController>();

            controller.OwnerView = new object();
            controller.Root = root;

            controller.Initialize();

            bool showDialogActionCalled = false;
            MockSelectContactView.ShowDialogAction = view =>
            {
                showDialogActionCalled = true;
                // Do nothing, this simulates that the user has closed the window.
            };

            controller.Run();

            Assert.IsTrue(showDialogActionCalled);
            Assert.IsNull(controller.SelectedContact);

            MockSelectContactView.ShowDialogAction = null;
        }
    }
}
