using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;
using Waf.InformationManager.AddressBook.Modules.Domain;
using System.Windows.Input;

namespace Waf.InformationManager.AddressBook.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class ContactListViewModel : ViewModel<IContactListView>
    {
        private IEnumerable<Contact> contacts;
        private Contact selectedContact;
        private ICommand deleteContactCommand;
        private string filterText = "";

        
        [ImportingConstructor]
        public ContactListViewModel(IContactListView view) : base(view)
        {
        }


        public IEnumerable<Contact> Contacts
        {
            get { return contacts; }
            set
            {
                if (contacts != value)
                {
                    contacts = value;
                    RaisePropertyChanged("Contacts");
                }
            }
        }

        public Contact SelectedContact
        {
            get { return selectedContact; }
            set
            {
                if (selectedContact != value)
                {
                    selectedContact = value;
                    RaisePropertyChanged("SelectedContact");
                }
            }
        }

        public IEnumerable<Contact> ContactCollectionView { get; set; }

        public ICommand DeleteContactCommand
        {
            get { return deleteContactCommand; }
            set
            {
                if (deleteContactCommand != value)
                {
                    deleteContactCommand = value;
                    RaisePropertyChanged("DeleteContactCommand");
                }
            }
        }

        public string FilterText
        {
            get { return filterText; }
            set
            {
                if (filterText != value)
                {
                    filterText = value;
                    RaisePropertyChanged("FilterText");
                }
            }
        }


        public void FocusItem() { ViewCore.FocusItem(); }

        public bool Filter(Contact contact)
        {
            if (string.IsNullOrEmpty(filterText)) { return true; }
            
            return (!string.IsNullOrEmpty(contact.Firstname) && contact.Firstname.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                || (!string.IsNullOrEmpty(contact.Lastname) && contact.Lastname.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                || (!string.IsNullOrEmpty(contact.Email) && contact.Email.IndexOf(filterText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
    }
}
