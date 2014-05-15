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
    [Export(typeof(IDetailView))]
    public partial class DetailView : UserControl, IDetailView
    {
        private readonly Lazy<DetailViewModel> _lazyVm;

        public DetailView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<DetailViewModel>(() => ViewHelper.GetViewModel<DetailViewModel>(this));
        }

        // Provides this view's viewmodel
        private DetailViewModel _viewModel { get { return _lazyVm.Value; } }
    }
}
