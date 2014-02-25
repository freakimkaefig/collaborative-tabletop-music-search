﻿using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using Waf.InformationManager.EmailClient.Modules.Applications.Views;
using Waf.InformationManager.EmailClient.Modules.Applications.ViewModels;

namespace Waf.InformationManager.EmailClient.Modules.Presentation.Views
{
    [Export(typeof(IEmailAccountsView)), PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class EmailAccountsWindow : Window, IEmailAccountsView
    {
        private readonly Lazy<EmailAccountsViewModel> viewModel;

        
        public EmailAccountsWindow()
        {
            InitializeComponent();
            this.viewModel = new Lazy<EmailAccountsViewModel>(() => ViewHelper.GetViewModel<EmailAccountsViewModel>(this));
        }


        public void ShowDialog(object owner)
        {
            Owner = owner as Window;
            ShowDialog();
        }

        private void EmailAccountsGridMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            viewModel.Value.EditAccountCommand.Execute(null);
        }
    }
}
