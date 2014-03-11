using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using Ctms.Applications.Views;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using System.Waf.Applications;
using System;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export(typeof(ISearchTagView))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class SearchTagView : ISearchTagView
    {
        private readonly Lazy<SearchTagViewModel> _lazyVm;

        private int count = 0;

        public SearchTagView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchTagViewModel>(() => ViewHelper.GetViewModel<SearchTagViewModel>(this));

            count++;

            //var r = new Random(DateTime.Now.Millisecond);

            //testContent = _viewModel.TestContent;
            //PieMenuItem1.SubHeader = "Header1";
        }

        // Provides this view's viewmodel
        private SearchTagViewModel _viewModel { get { return _lazyVm.Value; } }

        public void DoSth()
        {

        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //_viewModel.Id = e.TagVisualization.VisualizedTag.Value;
            //Tag = e.TagVisualization.VisualizedTag;

            //HomeButton.Content = _viewModel.TestContent;
            //PieMenuItem1.SubHeader = "Header1";
            //HomeButton.Content = "Dig";
        }
    }
}
