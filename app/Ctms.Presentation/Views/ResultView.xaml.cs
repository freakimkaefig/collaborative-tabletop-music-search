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
    [Export(typeof(IResultView))]
    public partial class ResultView : UserControl, IResultView
    {
        private readonly Lazy<ResultViewModel> viewModel;

        public ResultView()
        {
            InitializeComponent();
            viewModel = new Lazy<ResultViewModel>(() => ViewHelper.GetViewModel<ResultViewModel>(this));
        }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
