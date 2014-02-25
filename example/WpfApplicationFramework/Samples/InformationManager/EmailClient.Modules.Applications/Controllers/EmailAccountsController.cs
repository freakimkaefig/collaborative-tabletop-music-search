using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.ViewModels;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;
using System.Windows.Input;

namespace Waf.InformationManager.EmailClient.Modules.Applications.Controllers
{
    /// <summary>
    /// Responsible for an email account.
    /// </summary>
    [Export]
    internal class EmailAccountsController : Controller
    {
        private readonly CompositionContainer container;
        private readonly IShellService shellService;
        private readonly DelegateCommand emailAccountsCommand;
        private DelegateCommand removeEmailAccountCommand;
        private DelegateCommand editEmailAccountCommand;
        private EmailAccountsViewModel emailAccountsViewModel;

        
        [ImportingConstructor]
        public EmailAccountsController(CompositionContainer container, IShellService shellService)
        {
            this.container = container;
            this.shellService = shellService;
            this.emailAccountsCommand = new DelegateCommand(ShowEmailAccounts);
        }


        public EmailClientRoot Root { get; set; }

        public ICommand EmailAccountsCommand { get { return emailAccountsCommand; } }

        
        private void ShowEmailAccounts()
        {
            removeEmailAccountCommand = new DelegateCommand(RemoveEmailAccount, CanRemoveEmailAccount);
            editEmailAccountCommand = new DelegateCommand(EditEmailAccount, CanEditEmailAccount);

            emailAccountsViewModel = container.GetExportedValue<EmailAccountsViewModel>();
            emailAccountsViewModel.EmailClientRoot = Root;
            emailAccountsViewModel.NewAccountCommand = new DelegateCommand(NewEmailAccount);
            emailAccountsViewModel.RemoveAccountCommand = removeEmailAccountCommand;
            emailAccountsViewModel.EditAccountCommand = editEmailAccountCommand;

            AddWeakEventListener(emailAccountsViewModel, EmailAccountsViewModelPropertyChanged);

            emailAccountsViewModel.ShowDialog(shellService.ShellView);

            RemoveWeakEventListener(emailAccountsViewModel, EmailAccountsViewModelPropertyChanged);
            emailAccountsViewModel = null;
            removeEmailAccountCommand = null;
        }

        private void NewEmailAccount()
        {
            var editEmailAccountController = container.GetExportedValue<EditEmailAccountController>();
            editEmailAccountController.OwnerWindow = emailAccountsViewModel.View;
            editEmailAccountController.EmailAccount = new EmailAccount();

            editEmailAccountController.Initialize();
            if (editEmailAccountController.Run())
            {
                Root.AddEmailAccount(editEmailAccountController.EmailAccount);
            }
        }

        private bool CanRemoveEmailAccount() { return emailAccountsViewModel.SelectedEmailAccount != null; }

        private void RemoveEmailAccount()
        {
            Root.RemoveEmailAccount(emailAccountsViewModel.SelectedEmailAccount);
        }

        private bool CanEditEmailAccount() { return emailAccountsViewModel.SelectedEmailAccount != null; }

        private void EditEmailAccount()
        {
            var originalAccount = emailAccountsViewModel.SelectedEmailAccount;

            var editEmailAccountController = container.GetExportedValue<EditEmailAccountController>();
            editEmailAccountController.OwnerWindow = emailAccountsViewModel.View;
            editEmailAccountController.EmailAccount = originalAccount.Clone();

            editEmailAccountController.Initialize();
            if (editEmailAccountController.Run())
            {
                Root.ReplaceEmailAccount(originalAccount, editEmailAccountController.EmailAccount);
            }
        }

        private void EmailAccountsViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedEmailAccount")
            {
                removeEmailAccountCommand.RaiseCanExecuteChanged();
                editEmailAccountCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
