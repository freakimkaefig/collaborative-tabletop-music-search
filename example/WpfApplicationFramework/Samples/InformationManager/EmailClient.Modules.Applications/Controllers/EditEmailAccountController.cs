using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.ViewModels;
using Waf.InformationManager.EmailClient.Modules.Domain.AccountSettings;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;

namespace Waf.InformationManager.EmailClient.Modules.Applications.Controllers
{
    /// <summary>
    /// Responsible for creating a new email account or modifying an existing one via a wizard.
    /// </summary>
    [Export, PartCreationPolicy(CreationPolicy.NonShared)]
    internal class EditEmailAccountController : Controller
    {
        private readonly CompositionContainer container;
        private readonly DelegateCommand backCommand;
        private readonly DelegateCommand nextCommand;
        private readonly EditEmailAccountViewModel editEmailAccountViewModel;
        private readonly BasicEmailAccountViewModel basicEmailAccountViewModel;
        private Pop3SettingsViewModel pop3SettingsViewModel;
        private ExchangeSettingsViewModel exchangeSettingsViewModel;
        private bool result;


        [ImportingConstructor]
        public EditEmailAccountController(CompositionContainer container, EditEmailAccountViewModel editEmailAccountViewModel,
            BasicEmailAccountViewModel basicEmailAccountViewModel)
        {
            this.container = container;
            this.editEmailAccountViewModel = editEmailAccountViewModel;
            AddWeakEventListener(editEmailAccountViewModel, EmailAccountsViewModelPropertyChanged);
            this.basicEmailAccountViewModel = basicEmailAccountViewModel;

            backCommand = new DelegateCommand(Back, CanBack);
            nextCommand = new DelegateCommand(Next, CanNext);
        }


        public object OwnerWindow { get; set; }

        public EmailAccount EmailAccount { get; set; }

        
        public void Initialize()
        {
            editEmailAccountViewModel.BackCommand = backCommand;
            editEmailAccountViewModel.NextCommand = nextCommand;
            basicEmailAccountViewModel.EmailAccount = EmailAccount;
        }

        public bool Run()
        {
            editEmailAccountViewModel.ContentView = basicEmailAccountViewModel.View;
            editEmailAccountViewModel.ShowDialog(OwnerWindow);
            return result;
        }


        // Wizard workflow

        private void Close()
        {
            editEmailAccountViewModel.Close();
        }

        private bool CanBack()
        {
            return editEmailAccountViewModel.ContentView != basicEmailAccountViewModel.View;
        }

        private void Back()
        {
            editEmailAccountViewModel.IsLastPage = false;
            editEmailAccountViewModel.ContentView = basicEmailAccountViewModel.View;
            UpdateCommandsState();
        }

        private bool CanNext() { return editEmailAccountViewModel.IsValid; }

        private void Next()
        {
            if (editEmailAccountViewModel.ContentView == basicEmailAccountViewModel.View)
            {
                if (basicEmailAccountViewModel.IsPop3Checked)
                {
                    editEmailAccountViewModel.IsLastPage = true;
                    ShowPop3SettingsView();
                }
                else if (basicEmailAccountViewModel.IsExchangeChecked)
                {
                    editEmailAccountViewModel.IsLastPage = true;
                    ShowExchangeSettingsView();
                }
            }
            else if (pop3SettingsViewModel != null
                && editEmailAccountViewModel.ContentView == pop3SettingsViewModel.View)
            {
                SavePop3Settings();
                Close();
            }
            else if (exchangeSettingsViewModel != null
                && editEmailAccountViewModel.ContentView == exchangeSettingsViewModel.View)
            {
                SaveExchangeSettings();
                Close();
            }

            UpdateCommandsState();
        }

        private void UpdateCommandsState()
        {
            backCommand.RaiseCanExecuteChanged();
            nextCommand.RaiseCanExecuteChanged();
        }
        
        private void EmailAccountsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsValid") { UpdateCommandsState(); }
        }

        
        // Show wizard pages

        private void ShowPop3SettingsView()
        {
            Pop3Settings pop3Settings = EmailAccount.EmailAccountSettings is Pop3Settings 
                ? (Pop3Settings)EmailAccount.EmailAccountSettings : new Pop3Settings();
            pop3SettingsViewModel = container.GetExportedValue<Pop3SettingsViewModel>();
            pop3SettingsViewModel.Model = pop3Settings;
            editEmailAccountViewModel.ContentView = pop3SettingsViewModel.View;
        }

        private void ShowExchangeSettingsView()
        {
            ExchangeSettings exchangeSettings = EmailAccount.EmailAccountSettings is ExchangeSettings 
                ? (ExchangeSettings)EmailAccount.EmailAccountSettings : new ExchangeSettings();
            exchangeSettingsViewModel = container.GetExportedValue<ExchangeSettingsViewModel>();
            exchangeSettingsViewModel.Model = exchangeSettings;
            editEmailAccountViewModel.ContentView = exchangeSettingsViewModel.View;
        }

        private void SavePop3Settings()
        {
            EmailAccount.EmailAccountSettings = pop3SettingsViewModel.Model;
            result = true;
        }

        private void SaveExchangeSettings()
        {
            EmailAccount.EmailAccountSettings = exchangeSettingsViewModel.Model;
            result = true;
        }
    }
}
