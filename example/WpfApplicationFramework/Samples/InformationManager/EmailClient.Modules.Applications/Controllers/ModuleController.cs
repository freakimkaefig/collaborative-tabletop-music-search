using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Waf.Applications;
using Waf.InformationManager.EmailClient.Modules.Applications.SampleData;
using Waf.InformationManager.EmailClient.Modules.Applications.ViewModels;
using Waf.InformationManager.EmailClient.Modules.Domain.Emails;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Waf.InformationManager.EmailClient.Modules.Applications.Controllers
{
    /// <summary>
    /// Responsible for the whole module. This controller delegates the tasks to other controllers.
    /// </summary>
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : Controller, IModuleController
    {
        private readonly CompositionContainer container;
        private readonly IShellService shellService;
        private readonly INavigationService navigationService;
        private readonly EmailAccountsController emailAccountsController;
        private readonly DelegateCommand newEmailCommand;
        private readonly EmailClientRoot root;
        private readonly List<ItemCountSynchronizer> itemCountSychronizers;
        private EmailFolderController activeEmailFolderController;
        
        
        [ImportingConstructor]
        public ModuleController(CompositionContainer container, IShellService shellService, INavigationService navigationService,
            EmailAccountsController emailAccountsController)
        {
            this.container = container;
            this.shellService = shellService;
            this.navigationService = navigationService;
            this.emailAccountsController = emailAccountsController;
            this.newEmailCommand = new DelegateCommand(NewEmail);
            this.root = new EmailClientRoot();
            this.itemCountSychronizers = new List<ItemCountSynchronizer>();
        }


        internal EmailClientRoot Root { get { return root; } }


        public void Initialize()
        {
            root.AddEmailAccount(SampleDataProvider.CreateEmailAccount());
            foreach (var email in SampleDataProvider.CreateInboxEmails()) { root.Inbox.AddEmail(email); }
            foreach (var email in SampleDataProvider.CreateSentEmails()) { root.Sent.AddEmail(email); }
            foreach (var email in SampleDataProvider.CreateDrafts()) { root.Drafts.AddEmail(email); }

            emailAccountsController.Root = root;
            
            INavigationNode node = navigationService.AddNavigationNode("Inbox", ShowInbox, CloseCurrentView, 1, 1);
            itemCountSychronizers.Add(new ItemCountSynchronizer(node, root.Inbox));
            node = navigationService.AddNavigationNode("Outbox", ShowOutbox, CloseCurrentView, 1, 2);
            itemCountSychronizers.Add(new ItemCountSynchronizer(node, root.Outbox));
            node = navigationService.AddNavigationNode("Sent", ShowSentEmails, CloseCurrentView, 1, 3);
            itemCountSychronizers.Add(new ItemCountSynchronizer(node, root.Sent));
            node = navigationService.AddNavigationNode("Drafts", ShowDrafts, CloseCurrentView, 1, 4);
            itemCountSychronizers.Add(new ItemCountSynchronizer(node, root.Drafts));
            node = navigationService.AddNavigationNode("Deleted", ShowDeletedEmails, CloseCurrentView, 1, 5);
            itemCountSychronizers.Add(new ItemCountSynchronizer(node, root.Deleted));
        }

        public void Run()
        {
        }

        public void Shutdown()
        {   
        }

        private void ShowEmails(EmailFolder emailFolder)
        {
            activeEmailFolderController = container.GetExportedValue<EmailFolderController>();
            activeEmailFolderController.EmailFolder = emailFolder;
            activeEmailFolderController.Initialize();
            activeEmailFolderController.Run();

            ToolBarCommand uiNewEmailCommand = new ToolBarCommand(newEmailCommand, "_New email", 
                "Creates a new email.");
            ToolBarCommand uiDeleteEmailCommand = new ToolBarCommand(activeEmailFolderController.DeleteEmailCommand, "_Delete",
                "Deletes the selected email.");
            ToolBarCommand uiEmailAccountsCommand = new ToolBarCommand(emailAccountsController.EmailAccountsCommand, "_Email accounts",
                "Opens a window that shows the email accounts.");
            shellService.AddToolBarCommands(new[] { uiNewEmailCommand, uiDeleteEmailCommand, uiEmailAccountsCommand });
        }

        private void ShowInbox()
        {
            ShowEmails(root.Inbox);
        }

        private void ShowOutbox()
        {
            ShowEmails(root.Outbox);
        }

        private void ShowSentEmails()
        {
            ShowEmails(root.Sent);
        }

        private void ShowDrafts()
        {
            ShowEmails(root.Drafts);
        }

        private void ShowDeletedEmails()
        {
            ShowEmails(root.Deleted);
        }

        private void CloseCurrentView()
        {
            shellService.ClearToolBarCommands();
            
            if (activeEmailFolderController != null)
            {
                activeEmailFolderController.Shutdown();
                activeEmailFolderController = null;
            }
        }

        private void NewEmail()
        {
            var newEmailController = container.GetExportedValue<NewEmailController>();
            newEmailController.Root = root;
            newEmailController.Initialize();
            newEmailController.Run();
        }


        private class ItemCountSynchronizer : DataModel
        {
            private readonly INavigationNode node;
            private readonly EmailFolder folder;
            

            public ItemCountSynchronizer(INavigationNode node, EmailFolder folder)
            {
                this.node = node;
                this.folder = folder;
                AddWeakEventListener((INotifyCollectionChanged)folder.Emails, EmailsCollectionChanged);
                UpdateItemCount();
            }


            private void EmailsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { UpdateItemCount(); }

            private void UpdateItemCount() { node.ItemCount = folder.Emails.Count; }
        }
    }
}
