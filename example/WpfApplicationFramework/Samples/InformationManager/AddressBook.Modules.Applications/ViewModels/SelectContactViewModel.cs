using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.AddressBook.Modules.Applications.Views;
using System.Waf.Applications;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Waf.InformationManager.AddressBook.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class SelectContactViewModel : ViewModel<ISelectContactView>
    {
        private readonly DelegateCommand okCommand;
        private object contactListView;
        private bool dialogResult;

        
        [ImportingConstructor]
        public SelectContactViewModel(ISelectContactView view) : base(view)
        {
            this.okCommand = new DelegateCommand(Ok);
        }


        public ICommand OkCommand { get { return okCommand; } }

        public object ContactListView
        {
            get { return contactListView; }
            set
            {
                if (contactListView != value)
                {
                    contactListView = value;
                    RaisePropertyChanged("ContactListView");
                }
            }
        }


        public bool ShowDialog(object owner)
        {
            ViewCore.ShowDialog(owner);
            return dialogResult;
        }

        private void Close()
        {
            ViewCore.Close();
        }

        private void Ok()
        {
            dialogResult = true;
            Close();
        }
    }
}
