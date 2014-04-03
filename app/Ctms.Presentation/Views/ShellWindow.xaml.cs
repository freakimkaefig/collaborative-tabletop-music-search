using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface;
using System.Diagnostics;
using System.Waf.Applications;
using Ctms.Applications.ViewModels;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export(typeof(IShellView))]
    public partial class ShellWindow : SurfaceWindow, IShellView
    {
        private readonly Lazy<ShellViewModel> _lazyVm;

        public ShellWindow()
        {
            InitializeComponent();
            _lazyVm = new Lazy<ShellViewModel>(() => ViewHelper.GetViewModel<ShellViewModel>(this));
            AddWindowAvailabilityHandlers();
        }

        // Provides this view's viewmodel
        private ShellViewModel _viewModel { get { return _lazyVm.Value; } }
        
        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Maximized;
                }
            }
        }

        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        private void OnWindowInteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowNoninteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowUnavailable(object sender, EventArgs e)
        {

        }
    }
}
