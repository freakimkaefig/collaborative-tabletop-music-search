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

        }

        // Provides this view's viewmodel
        private SearchViewModel _viewModel { get { return _lazyVm.Value; } }

        public TagVisualizer TagVisualizer { get { return SearchTagVisualizer; } set {} }

        //public List<Tag> Tags { get; set; }

        private void UpdateItem()
        {
            // .Header = "Header";
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            // KeywordType
            _viewModel.OnVisualizationAdded(e.TagVisualization);

            var tagVisualization    = (SearchTagView) e.TagVisualization;
            var tagId               = (int) tagVisualization.VisualizedTag.Value;
            /*
            var pieMenu             = ((PieMenu) tagVisualization.PieMenu1);//!!
            var pieMenuItems        = (ItemCollection) pieMenu.Items;

            var counter = 0;
            foreach (var item in pieMenuItems)
            {
                var pieMenuItem = (PieMenuItem)item;
                pieMenuItem.Header = "Tag" + tagId + "Dyn" + counter;
                counter++;
            }
            */
            //var pieMenuItems = pieMenu.Items.Cast<PieMenuItem>().ToList();

            //UpdateTagValues(tagId, pieMenuItems);

            //SetBinding(tagVisualization, tagId);

            //tagVisualization.PieMenuItem1.
            /*
            foreach (var item in pieMenuItems)
            {
                //item = (PieMenuItem)
                SetItemBinding(tagVisualization, tagId, (PieMenuItem) item);
            }*/
        }

        private void SetItemBinding(SearchTagView tagVisualization, int tagId, PieMenuItem item)
        {
            var binding = new Binding();
            binding.Source = _viewModel.Entries[tagId];
            //binding.Path = new PropertyPath("SomePropertyOfBindingSourceObject");
            //item.Header.
        }

        private void SetBinding(SearchTagView tagVisualization, int tagId)
        {
            var binding = new Binding();
            binding.Source = _viewModel.Entries[tagId];
            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                
                       //            Mode=TwoWay,
                    //               UpdateSourceTrigger=PropertyChanged}"
            //binding.Path = new PropertyPath("SomePropertyOfBindingSourceObject");
            //tagVisualization.PieMenuItem1.SetBinding(PieMenuItem.SubHeaderProperty, binding);
        }

        private void UpdateTagValues(int tagId, ItemCollection pieMenuItems)
        {
            var tag = _viewModel.Tags[tagId];
            var counter = 0;
            
            foreach (PieMenuItem item in pieMenuItems)
            {
                if (counter >= _viewModel.Tags.Count) break;//!!

                var tagOption = tag.TagOptions[counter];

                if (tagOption is DoubleTextTagOption)
                {
                    var myTagOption = (DoubleTextTagOption)tagOption;
                    item.Header = myTagOption.MainText;
                    item.SubHeader = myTagOption.SubText;
                }
                else if (tagOption is SingleTextTagOption)
                {
                    var myTagOption = (SingleTextTagOption)tagOption;
                    item.Header = myTagOption.Text;
                }
                item.Id = tagOption.Id;
                counter++;
            }
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
        }
    }
}
