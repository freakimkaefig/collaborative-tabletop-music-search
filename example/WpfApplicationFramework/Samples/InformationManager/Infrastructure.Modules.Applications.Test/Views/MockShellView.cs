using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.InformationManager.Infrastructure.Modules.Applications.Views;
using Waf.InformationManager.Infrastructure.Interfaces.Applications;
using System.ComponentModel.Composition;

namespace Test.InformationManager.Infrastructure.Modules.Applications.Views
{
    [Export(typeof(IShellView)), Export]
    public class MockShellView : IShellView
    {
        private readonly List<ToolBarCommand> toolBarCommands;


        public MockShellView()
        {
            this.toolBarCommands = new List<ToolBarCommand>();
        }


        public IEnumerable<ToolBarCommand> ToolBarCommands { get { return toolBarCommands; } }

        public object DataContext { get; set; }

        public bool IsVisible { get; private set; }

        public double VirtualScreenWidth { get; set; }
        
        public double VirtualScreenHeight { get; set; }
        
        public double Left { get; set; }
        
        public double Top { get; set; }
        
        public double Width { get; set; }
        
        public double Height { get; set; }
        
        public bool IsMaximized { get; set; }
        

        public event EventHandler Closed;


        public void Show()
        {
            IsVisible = true;
        }

        public void Close()
        {
            IsVisible = false;
            OnClosed(EventArgs.Empty);
        }

        public void AddToolBarCommands(IEnumerable<ToolBarCommand> commands)
        {
            toolBarCommands.AddRange(commands);    
        }

        public void ClearToolBarCommands()
        {
            toolBarCommands.Clear();
        }

        public void SetNAForLocationAndSize()
        {
            Top = double.NaN;
            Left = double.NaN;
            Width = double.NaN;
            Height = double.NaN;
        }

        protected virtual void OnClosed(EventArgs e)
        {
            if (Closed != null) { Closed(this, e); }
        }
    }
}
