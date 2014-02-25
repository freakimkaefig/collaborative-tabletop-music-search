using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.ComponentModel.Composition;
using Waf.InformationManager.AddressBook.Modules.Domain;
using Waf.InformationManager.AddressBook.Modules.Applications.ViewModels;

namespace Waf.InformationManager.AddressBook.Modules.Applications.Controllers
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class SelectContactController : Controller
    {
        private readonly SelectContactViewModel selectContactViewModel;
        private readonly ContactListViewModel contactListViewModel;
        

        [ImportingConstructor]
        public SelectContactController(SelectContactViewModel selectContactViewModel, ContactListViewModel contactListViewModel)
        {
            this.selectContactViewModel = selectContactViewModel;
            this.contactListViewModel = contactListViewModel;
        }


        public object OwnerView { get; set; }

        public AddressBookRoot Root { get; set; }

        public Contact SelectedContact { get; private set; }

        internal SelectContactViewModel SelectContactViewModel { get { return selectContactViewModel; } }

        internal ContactListViewModel ContactListViewModel { get { return contactListViewModel; } }


        public void Initialize()
        {
            contactListViewModel.Contacts = Root.Contacts;
            contactListViewModel.SelectedContact = Root.Contacts.FirstOrDefault();
            selectContactViewModel.ContactListView = contactListViewModel.View;
        }

        public void Run()
        {
            if (selectContactViewModel.ShowDialog(OwnerView))
            {
                SelectedContact = contactListViewModel.SelectedContact;
            }
            else
            {
                SelectedContact = null;
            }
        }
    }
}
