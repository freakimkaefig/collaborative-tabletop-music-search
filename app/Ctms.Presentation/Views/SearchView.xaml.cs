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
using PieInTheSky;
using Ctms.Domain.Objects;
using MusicSearch.SearchObjects;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        private readonly Lazy<SearchViewModel> _lazyVm;

        public SearchView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchViewModel>(() => ViewHelper.GetViewModel<SearchViewModel>(this));

            //Tags = new List<Tag>();
        }

        // Provides this view's viewmodel
        private SearchViewModel _viewModel { get { return _lazyVm.Value; } }

        public TagVisualizer TagVisualizer { get { return SearchTagVisualizer; } set {} }

        //public List<Tag> Tags { get; set; }
        
        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
            //KeywordType
            //viewModel.OnVisualizationAdded(sender);
            _viewModel.OnVisualizationAdded(e.TagVisualization);

            var tagVisualization    = (SearchTagView) e.TagVisualization;
            var tagId               = (int) tagVisualization.VisualizedTag.Value;

            var pieMenu             = ((PieMenu) tagVisualization.PieMenu1);//!!
            var pieMenuItems        = (ItemCollection) pieMenu.Items;

            //pieMenuItems[0].

            foreach (var item in pieMenuItems)
            {
                //((PieMe
            }

            var tag                 = _viewModel.Tags[tagId];
            var counter             = 0;

            foreach (PieMenuItem item in pieMenuItems)
            {
                if (counter >= _viewModel.Tags.Count) break;
                item.Header     = tag.SearchOptions[counter].Header;
                item.SubHeader  = tag.SearchOptions[counter].SubHeader;
                item.Id         = tag.SearchOptions[counter].Id;
                counter++;
            }
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
        }
    }
}
