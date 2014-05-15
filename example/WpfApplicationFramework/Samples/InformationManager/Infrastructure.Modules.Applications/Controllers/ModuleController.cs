using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.InformationManager.Infrastructure.Modules.Applications.Properties;
using Waf.InformationManager.Infrastructure.Modules.Applications.Services;
using Waf.InformationManager.Infrastructure.Modules.Applications.ViewModels;

namespace Waf.InformationManager.Infrastructure.Modules.Applications.Controllers
{
    [Export(typeof(IModuleController)), Export]
    internal class ModuleController : Controller, IModuleController
    {
        private readonly Lazy<ShellViewModel> shellViewModel;

        
        [ImportingConstructor]
        public ModuleController(Lazy<ShellViewModel> shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }


        public void Initialize()
        {
        }

        public void Run()
        {
            shellViewModel.Value.Show();
        }

        public void Shutdown()
        {
            try
            {
                Settings.Default.Save();
            }
            catch (Exception)
            {
                // When more application instances are closed at the same time then an exception occurs.
            }
        }
    }
}
