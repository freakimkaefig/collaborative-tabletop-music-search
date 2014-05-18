using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Globalization;
using System.Threading;
using System.Waf.Applications;
using Waf.Writer.Applications.Properties;
using Waf.Writer.Applications.Services;
using Waf.Writer.Applications.ViewModels;

namespace Waf.Writer.Applications.Controllers
{
    /// <summary>
    /// Responsible for the application lifecycle.
    /// </summary>
    [Export(typeof(IApplicationController))]
    internal class ApplicationController : Controller, IApplicationController
    {
        private readonly IEnvironmentService environmentService;
        private readonly FileController fileController;
        private readonly RichTextDocumentController richTextDocumentController;
        private readonly PrintController printController;
        private readonly ShellViewModel shellViewModel;
        private readonly MainViewModel mainViewModel;
        private readonly StartViewModel startViewModel;
        private readonly DelegateCommand exitCommand;

        
        [ImportingConstructor]
        public ApplicationController(CompositionContainer container, IEnvironmentService environmentService, 
            IPresentationService presentationService, ShellService shellService, FileController fileController)
        {
            InitializeCultures();
            presentationService.InitializeCultures();

            this.environmentService = environmentService;
            this.fileController = fileController;
            
            this.richTextDocumentController = container.GetExportedValue<RichTextDocumentController>();
            this.printController = container.GetExportedValue<PrintController>();
            this.shellViewModel = container.GetExportedValue<ShellViewModel>();
            this.mainViewModel = container.GetExportedValue<MainViewModel>();
            this.startViewModel = container.GetExportedValue<StartViewModel>();

            shellService.ShellView = shellViewModel.View;
            this.shellViewModel.Closing += ShellViewModelClosing;
            this.exitCommand = new DelegateCommand(Close);
        }

        
        public void Initialize()
        {
            mainViewModel.StartView = startViewModel.View;
            mainViewModel.ExitCommand = exitCommand;
            
            printController.Initialize();
            fileController.Initialize();
        }

        public void Run()
        {
            shellViewModel.ContentView = mainViewModel.View;
            
            if (!string.IsNullOrEmpty(environmentService.DocumentFileName))
            {
                fileController.Open(environmentService.DocumentFileName);
            }
            
            shellViewModel.Show();
        }

        public void Shutdown()
        {
            fileController.Shutdown();
            printController.Shutdown();

            if (mainViewModel.NewLanguage != null) 
            { 
                Settings.Default.UICulture = mainViewModel.NewLanguage.Name; 
            }
            try
            {
                Settings.Default.Save();
            }
            catch (Exception)
            {
                // When more application instances are closed at the same time then an exception occurs.
            }
        }

        private static void InitializeCultures()
        {
            if (!String.IsNullOrEmpty(Settings.Default.Culture))
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(Settings.Default.Culture);
            }
            if (!String.IsNullOrEmpty(Settings.Default.UICulture))
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(Settings.Default.UICulture);
            }
        }

        private void Close()
        {
            shellViewModel.Close();
        }

        private void ShellViewModelClosing(object sender, CancelEventArgs e)
        {
            // Try to close all documents and see if the user has already saved them.
            e.Cancel = !fileController.CloseAll();
        }
    }
}
