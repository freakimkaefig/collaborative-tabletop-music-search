using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.Properties;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Ctms.Applications.Views;
using System.Waf.Applications.Services;
using Ctms.Applications.Services;
using System.Windows.Input;
using System.ComponentModel;
using System.Globalization;

namespace Ctms.Applications.ViewModels
{
    [Export]
    public class ShellViewModel : ViewModel<IShellView>
    {
        private readonly IMessageService messageService;
        private readonly IShellService shellService;
        private readonly DelegateCommand aboutCommand;
        private ICommand saveCommand;
        private ICommand exitCommand;
        private bool isValid = true;
        private string databasePath = "Nicht verfügbar";


        [ImportingConstructor]
        public ShellViewModel(IShellView view, IMessageService messageService, IPresentationService presentationService,
            IShellService shellService)
            : base(view)
        {
            this.messageService = messageService;
            this.shellService = shellService;
            this.aboutCommand = new DelegateCommand(ShowAboutMessage);
            view.Closing += ViewClosing;
            view.Closed += ViewClosed;

            // Restore the window size when the values are valid.
            if (Settings.Default.Left >= 0 && Settings.Default.Top >= 0 && Settings.Default.Width > 0 && Settings.Default.Height > 0
                && Settings.Default.Left + Settings.Default.Width <= presentationService.VirtualScreenWidth
                && Settings.Default.Top + Settings.Default.Height <= presentationService.VirtualScreenHeight)
            {
                ViewCore.Left = Settings.Default.Left;
                ViewCore.Top = Settings.Default.Top;
                ViewCore.Height = Settings.Default.Height;
                ViewCore.Width = Settings.Default.Width;
            }
            ViewCore.IsMaximized = Settings.Default.IsMaximized;
        }


        public string Title { get { return ApplicationInfo.ProductName; } }

        public IShellService ShellService { get { return shellService; } }

        public ICommand AboutCommand { get { return aboutCommand; } }

        public ICommand SaveCommand
        {
            get { return saveCommand; }
            set
            {
                if (saveCommand != value)
                {
                    saveCommand = value;
                    RaisePropertyChanged("SaveCommand");
                }
            }
        }

        public ICommand ExitCommand
        {
            get { return exitCommand; }
            set
            {
                if (exitCommand != value)
                {
                    exitCommand = value;
                    RaisePropertyChanged("ExitCommand");
                }
            }
        }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }
        
        public string DatabasePath
        {
            get { return databasePath; }
            set
            {
                if (databasePath != value)
                {
                    databasePath = value;
                    RaisePropertyChanged("DatabasePath");
                }
            }
        }
        

        public event CancelEventHandler Closing;

        public void AddTagVisualization()
        {
            //ViewCore.A
        }

        public void Show()
        {
            ViewCore.Show();
        }

        public void Close()
        {
            ViewCore.Close();
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            if (Closing != null) { Closing(this, e); }
        }

        private void ViewClosing(object sender, CancelEventArgs e)
        {
            OnClosing(e);
        }

        private void ViewClosed(object sender, EventArgs e)
        {
            Settings.Default.Left = ViewCore.Left;
            Settings.Default.Top = ViewCore.Top;
            Settings.Default.Height = ViewCore.Height;
            Settings.Default.Width = ViewCore.Width;
            Settings.Default.IsMaximized = ViewCore.IsMaximized;
        }

        private void ShowAboutMessage()
        {
            messageService.ShowMessage(View, string.Format(CultureInfo.CurrentCulture, 
                "This is a Collaborative Tabletop Music Search application",
                ApplicationInfo.ProductName, ApplicationInfo.Version));
        }
    }
}
