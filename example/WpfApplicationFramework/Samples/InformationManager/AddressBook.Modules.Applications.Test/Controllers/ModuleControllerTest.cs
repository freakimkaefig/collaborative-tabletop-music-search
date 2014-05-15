using System.Linq;
using System.Waf.Applications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.InformationManager.AddressBook.Modules.Applications.Services;
using Test.InformationManager.AddressBook.Modules.Applications.Views;
using Waf.InformationManager.AddressBook.Modules.Applications.Controllers;
using Waf.InformationManager.AddressBook.Modules.Applications.ViewModels;
using Test.InformationManager.Infrastructure.Modules.Applications.Services;

namespace Test.InformationManager.AddressBook.Modules.Applications.Controllers
{
    [TestClass]
    public class ModuleControllerTest : AddressBookTest
    {
        [TestMethod]
        public void ShowAndCloseAddressBook()
        {
            var controller = Container.GetExportedValue<ModuleController>();
            
            // Initialize the controller

            Assert.IsFalse(controller.Root.Contacts.Any());
            
            controller.Initialize();

            Assert.IsTrue(controller.Root.Contacts.Any());
            var navigationService = Container.GetExportedValue<MockNavigationService>();
            var node = navigationService.NavigationNodes.Single();
            Assert.AreEqual("Contacts", node.Name);

            // Run the controller

            controller.Run();

            // Show the address book

            var shellService = Container.GetExportedValue<MockShellService>();
            Assert.IsNull(shellService.ContentView);
            Assert.IsFalse(shellService.ToolBarCommands.Any());
            
            node.ShowAction();

            Assert.IsNotNull(shellService.ContentView);
            Assert.AreEqual(2, shellService.ToolBarCommands.Count());

            // Close the address book

            node.CloseAction();

            Assert.IsFalse(shellService.ToolBarCommands.Any());

            // Shutdown the controller

            controller.Shutdown();
        }

        [TestMethod]
        public void ShowSelectContactViewTest()
        {
            var controller = Container.GetExportedValue<ModuleController>();
            controller.Initialize();
            controller.Run();

            // Show the select contact view

            MockSelectContactView.ShowDialogAction = view =>
            {
                var viewModel = ViewHelper.GetViewModel<SelectContactViewModel>(view);
                viewModel.OkCommand.Execute(null);
            };
            var ownerView = new object();
            var contactDto = controller.ShowSelectContactView(ownerView);

            Assert.IsNotNull(contactDto);

            MockSelectContactView.ShowDialogAction = null;

            controller.Shutdown();
        }
    }
}
