using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows.Input;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;

namespace Waf.InformationManager.EmailClient.Modules.Applications.ViewModels
{
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    public class NewEmailViewModel : ViewModel<INewEmailView>
    {
        private ICommand selectContactCommand;
        private ICommand sendCommand;
        private readonly DelegateCommand closeCommand;
        private IEnumerable<EmailAccount> emailAccounts;
        private EmailAccount selectedEmailAccount;
        private Email email;
        private string to = "";
        private string cc = "";
        private string bcc = "";


        [ImportingConstructor]
        public NewEmailViewModel(INewEmailView view) : base(view)
        {
            this.closeCommand = new DelegateCommand(Close);
        }


        public ICommand SelectContactCommand
        {
            get { return selectContactCommand; }
            set
            {
                if (selectContactCommand != value)
                {
                    selectContactCommand = value;
                    RaisePropertyChanged("SelectContactCommand");
                }
            }
        }

        public ICommand SendCommand
        {
            get { return sendCommand; }
            set
            {
                if (sendCommand != value)
                {
                    sendCommand = value;
                    RaisePropertyChanged("SendCommand");
                }
            }
        }

        public ICommand CloseCommand { get { return closeCommand; } }

        public IEnumerable<EmailAccount> EmailAccounts
        {
            get { return emailAccounts; }
            set
            {
                if (emailAccounts != value)
                {
                    emailAccounts = value;
                    RaisePropertyChanged("EmailAccounts");
                }
            }
        }

        public EmailAccount SelectedEmailAccount
        {
            get { return selectedEmailAccount; }
            set
            {
                if (selectedEmailAccount != value)
                {
                    selectedEmailAccount = value;
                    RaisePropertyChanged("SelectedEmailAccount");
                }
            }
        }

        public Email Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    if (email != null) { RemoveWeakEventListener(email, EmailPropertyChanged); }
                    email = value;
                    if (email != null) 
                    { 
                        AddWeakEventListener(email, EmailPropertyChanged);
                        UpdateProperties();
                    }
                    RaisePropertyChanged("Email");
                }
            }
        }

        public string To
        {
            get { return to; }
            set
            {
                if (to != value)
                {
                    IEnumerable<string> emails = ParseEmails(value);
                    to = FormatEmails(emails);
                    Email.To = emails;
                    RaisePropertyChanged("To");
                }
            }
        }

        public string CC
        {
            get { return cc; }
            set
            {
                if (cc != value)
                {
                    IEnumerable<string> emails = ParseEmails(value);
                    cc = FormatEmails(emails);
                    Email.CC = emails;
                    RaisePropertyChanged("CC");
                }
            }
        }

        public string Bcc
        {
            get { return bcc; }
            set
            {
                if (bcc != value)
                {
                    IEnumerable<string> emails = ParseEmails(value);
                    bcc = FormatEmails(emails);
                    Email.Bcc = emails;
                    RaisePropertyChanged("Bcc");
                }
            }
        }


        public void Show(object owner)
        {
            ViewCore.Show(owner);
        }

        public void Close()
        {
            ViewCore.Close();
        }

        private static IEnumerable<string> ParseEmails(string text)
        {
            return text.Trim().Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static string FormatEmails(IEnumerable<string> emailList)
        {
            return string.Join("; ", emailList);
        }

        private void UpdateProperties()
        {
            To = FormatEmails(Email.To);
            CC = FormatEmails(Email.CC);
            Bcc = FormatEmails(Email.Bcc);
        }

        private void EmailPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "To")
            {
                To = FormatEmails(Email.To);
            }
            else if (e.PropertyName == "CC")
            {
                CC = FormatEmails(Email.CC);
            }
            else if (e.PropertyName == "Bcc")
            {
                Bcc = FormatEmails(Email.Bcc);
            }
        }
    }
}
