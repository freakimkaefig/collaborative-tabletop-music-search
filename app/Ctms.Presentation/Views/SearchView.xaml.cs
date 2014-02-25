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
using Ctms.Applications.ViewModels;
using System.Waf.Applications;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        private readonly Lazy<SearchViewModel> viewModel;

        public SearchView()
        {
            InitializeComponent();
            viewModel = new Lazy<SearchViewModel>(() => ViewHelper.GetViewModel<SearchViewModel>(this));
        }

        public TagVisualizer TagVisualizer { get { return SearchTagVisualizer; } set {} }
        
        
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
            //KeywordType

        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
        }
    }
}
