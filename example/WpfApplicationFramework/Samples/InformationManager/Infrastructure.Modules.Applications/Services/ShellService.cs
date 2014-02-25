using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;

namespace Waf.InformationManager.Infrastructure.Modules.Applications.Services
{
    [Export(typeof(IShellService)), Export]
    public class ShellService : DataModel, IShellService
    {
        private readonly Lazy<IShellViewModel> shellViewModel;
        private object contentView;

        
        [ImportingConstructor]
        public ShellService(Lazy<IShellViewModel> shellViewModel)
        {
            this.shellViewModel = shellViewModel;
        }


        public object ShellView { get { return shellViewModel.Value.View; } }

        public object ContentView
        {
            get { return contentView; }
            set
            {
                if (contentView != value)
                {
                    contentView = value;
                    RaisePropertyChanged("ContentView");
                }
            }
        }


        public void AddToolBarCommands(IEnumerable<ToolBarCommand> commands)
        {
            shellViewModel.Value.AddToolBarCommands(commands);
        }

        public void ClearToolBarCommands()
        {
            shellViewModel.Value.ClearToolBarCommands();
        }
    }
}
