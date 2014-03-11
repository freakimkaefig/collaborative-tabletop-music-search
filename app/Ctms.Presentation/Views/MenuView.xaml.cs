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
using Ctms.Applications.ViewModels;
using System.Waf.Applications;

namespace Ctms.Presentation.Views
{
    [Export(typeof(IMenuView))]
    public partial class MenuView : UserControl, IMenuView
    {
        private readonly Lazy<MenuViewModel> _lazyVm;

        public MenuView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<MenuViewModel>(() => ViewHelper.GetViewModel<MenuViewModel>(this));
        }

        // Provides this view's viewmodel
        private MenuViewModel _viewModel { get { return _lazyVm.Value; } }
    }
}
