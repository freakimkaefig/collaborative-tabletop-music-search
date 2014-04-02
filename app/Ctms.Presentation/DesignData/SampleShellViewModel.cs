using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ctms.Applications.ViewModels;
using Ctms.Applications.Views;
using System.ComponentModel;
using Ctms.Presentation.Views;
using System.Waf.Applications;
using Microsoft.Surface.Presentation.Controls;

namespace Ctms.Presentation.DesignData
{
    public class SampleShellViewModel : ShellViewModel
    {
        public SampleShellViewModel()
            : base(new MockShellView(), null, null, new MockShellService())
        {
            ShellService.DetailView = new DetailView();
            ShellService.MenuView = new MenuView();
            ShellService.PlaylistView = new PlaylistView();
            ShellService.ResultView = new ResultView();
            ShellService.SearchView = new SearchView();
            ShellService.InfoView = new InfoView();
        }

        private class MockShellView : IShellView
        {
            public object DataContext { get; set; }

            public double Left { get; set; }

            public double Top { get; set; }

            public double Width { get; set; }

            public double Height { get; set; }

            public bool IsMaximized { get; set; }

            public event CancelEventHandler Closing;

            public event EventHandler Closed;

            public void Show() { }

            public void Close() { }

            protected virtual void OnClosing(CancelEventArgs e)
            {
                if (Closing != null) { Closing(this, e); }
            }

            protected virtual void OnClosed(EventArgs e)
            {
                if (Closed != null) { Closed(this, e); }
            }
        }
    }
}
