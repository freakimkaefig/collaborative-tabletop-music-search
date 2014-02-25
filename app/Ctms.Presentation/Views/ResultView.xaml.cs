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
        private readonly Lazy<ResultViewModel> _lazyVm;

        public ResultView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<ResultViewModel>(() => ViewHelper.GetViewModel<ResultViewModel>(this));
        }

        private ResultViewModel _viewModel { get { return _lazyVm.Value; } }

        private void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void ScatterViewItem_TouchDown(object sender, TouchEventArgs e)
        {
            object id = "spotify:track:4lCv7b86sLynZbXhfScfm2"; //Pass spotify-track-id from echonest
            _viewModel.PrelistenCommand.Execute(id);
        }
    }
}
