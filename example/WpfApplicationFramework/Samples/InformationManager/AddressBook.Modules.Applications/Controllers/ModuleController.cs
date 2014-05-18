using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.InformationManager.AddressBook.Interfaces.Applications;
using Waf.InformationManager.AddressBook.Interfaces.Domain;
using Waf.InformationManager.AddressBook.Modules.Applications.SampleData;
using Waf.InformationManager.AddressBook.Modules.Domain;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;

namespace Waf.InformationManager.AddressBook.Modules.Applications.Controllers
{
    /// <summary>
    /// Responsible for the whole module. This controller delegates the tasks to other controllers.
    /// </summary>
    [Export(typeof(IModuleController)), Export(typeof(IAddressBookService)), Export]
    internal class ModuleController : Controller, IModuleController, IAddressBookService
    {
        private readonly CompositionContainer container;
        private readonly IShellService shellService;
        private readonly INavigationService navigationService;
        private readonly AddressBookRoot root;
        private ContactController activeContactController;

        
        [ImportingConstructor]
        public ModuleController(CompositionContainer container, IShellService shellService, INavigationService navigationService)
        {
            this.container = container;
            this.shellService = shellService;
            this.navigationService = navigationService;
            this.root = new AddressBookRoot();
        }


        internal AddressBookRoot Root { get { return root; } }


        public void Initialize()
        {
            foreach (var contact in SampleDataProvider.CreateContacts()) { root.AddContact(contact); }
            
            navigationService.AddNavigationNode("Contacts", ShowAddressBook, CloseAddressBook, 2, 1);
        }

        public void Run()
        {   
        }

        public void Shutdown()
        {   
        }

        public ContactDto ShowSelectContactView(object ownerView)
        {
            var selectContactController = container.GetExportedValue<SelectContactController>();
            selectContactController.OwnerView = ownerView;
            selectContactController.Root = root;
            selectContactController.Initialize();
            selectContactController.Run();
            return selectContactController.SelectedContact.ToDto();
        }

        private void ShowAddressBook()
        {
            activeContactController = container.GetExportedValue<ContactController>();
            activeContactController.Root = root;
            activeContactController.Initialize();
            activeContactController.Run();

            ToolBarCommand uiNewContactCommand = new ToolBarCommand(activeContactController.NewContactCommand, "_New contact",
                "Creates a new contact.");
            ToolBarCommand uiDeleteCommand = new ToolBarCommand(activeContactController.DeleteContactCommand, "_Delete",
                "Deletes the selected contact.");
            shellService.AddToolBarCommands(new[] { uiNewContactCommand, uiDeleteCommand });
        }

        private void CloseAddressBook()
        {
            shellService.ClearToolBarCommands();
            
            if (activeContactController != null)
            {
                activeContactController.Shutdown();
                activeContactController = null;
            }
        }
    }
}
