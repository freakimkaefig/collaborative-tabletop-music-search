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
    public partial class SearchTagView : ISearchTagView
    {
        private readonly Lazy<SearchTagViewModel> viewModel;

        public SearchTagView()
        {
            InitializeComponent();
            viewModel = new Lazy<SearchTagViewModel>(() => ViewHelper.GetViewModel<SearchTagViewModel>(this));
        }

        public void DoSth()
        {

        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
