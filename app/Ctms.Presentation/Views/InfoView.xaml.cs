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
using Ctms.Applications.Views;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using Ctms.Applications.ViewModels;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für Info.xaml
    /// </summary>
    /// 
    [Export(typeof(IInfoView))]
    public partial class InfoView : UserControl, IInfoView
    {
        private readonly Lazy<InfoViewModel> _lazyVm;

        public InfoView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<InfoViewModel>(() => ViewHelper.GetViewModel<InfoViewModel>(this));
        }

        // Provides this view's viewmodel
        private InfoViewModel _viewModel { get { return _lazyVm.Value; } }
    }
}
