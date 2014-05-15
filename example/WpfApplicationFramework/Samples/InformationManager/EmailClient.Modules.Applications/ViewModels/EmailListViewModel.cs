using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;
using System.Windows.Input;

namespace Waf.InformationManager.EmailClient.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class EmailListViewModel : ViewModel<IEmailListView>
    {
        private IEnumerable<Email> emails;
        private Email selectedEmail;
        private ICommand deleteEmailCommand;
        private string filterText = "";

        
        [ImportingConstructor]
        public EmailListViewModel(IEmailListView view) : base(view)
        {
        }


        public IEnumerable<Email> Emails 
        { 
            get { return emails; }
            set
            {
                if (emails != value)
                {
                    emails = value;
                    RaisePropertyChanged("Emails");
                }
            }
        }

        public Email SelectedEmail
        {
            get { return selectedEmail; }
            set
            {
                if (selectedEmail != value)
                {
                    selectedEmail = value;
                    RaisePropertyChanged("SelectedEmail");
                }
            }
        }

        public IEnumerable<Email> EmailCollectionView { get; set; }

        public ICommand DeleteEmailCommand
        {
            get { return deleteEmailCommand; }
            set
            {
                if (deleteEmailCommand != value)
                {
                    deleteEmailCommand = value;
                    RaisePropertyChanged("DeleteEmailCommand");
                }
            }
        }

        public void FocusItem() { ViewCore.FocusItem(); }

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


        public bool Filter(Email email)
        {
            return email.Title.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0
                || email.From.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0
                || email.To.Any(x => x.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }
    }
}
