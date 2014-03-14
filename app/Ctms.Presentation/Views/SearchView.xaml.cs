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
using Microsoft.Surface.Presentation.Input;
using System.Diagnostics;
using Ctms.Applications.DataModels;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        private readonly Lazy<SearchViewModel> _lazyVm;

        public Dictionary<int, SearchTagView> Tags;

        public SearchView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchViewModel>(() => ViewHelper.GetViewModel<SearchViewModel>(this));
            Tags = new Dictionary<int, SearchTagView>();
        }

        // Provides this view's viewmodel
        private SearchViewModel _viewModel { get { return _lazyVm.Value; } }

        public TagVisualizer TagVisualizer { get { return SearchTagVisualizer; } set {} }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            // Every time tag is placed its visualization must be initiated again (is lost after tag remove)

            _viewModel.OnVisualizationAdded(e.TagVisualization);

            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;
            var tagDM = _viewModel.Tags[tagId];
            var pieMenu = searchTagView.PieMenu;

            if(Tags.ContainsKey(tagId) == false) Tags.Add(tagId, searchTagView);
            //else Tags[tagId] = searchTagView;

            pieMenu.Items.Clear();

            //  create dynamic resources
            searchTagView.Resources["SearchVM"] = _viewModel;
            searchTagView.Resources["TagDM"] = tagDM;
            searchTagView.Resources["TagId"] = tagId;
            searchTagView.Resources["TagOptions"] = tagDM.Tag.TagOptions;


            UpdateMenuItems(tagId);
            UpdateInputField(searchTagView, tagDM);

            CalcMenuVisibility(searchTagView, tagDM);

            UpdateVisual(tagId);
        }

        private static void CalcMenuVisibility(SearchTagView searchTagView, TagDataModel tagDM)
        {
            // calculate visibility and create dynamic resource
            var converter = new BooleanToVisibilityConverter();
            var isVisible = converter.Convert(tagDM.IsMenuVisible, null, null, null);
            searchTagView.Resources["IsMenuVisible"] = isVisible;
        }

        public void UpdateVisual(int tagId)
        {
            UpdateMenuItems(tagId);
            CalcMenuVisibility(Tags[tagId], _viewModel.Tags[tagId]);

            var pieMenu = Tags[tagId].PieMenu;


            foreach (PieMenuItem item in pieMenu.Items)
            {
                item.InvalidateVisual();
                item.InvalidateProperty(PieMenuItem.HeaderProperty);
                item.InvalidateProperty(PieMenuItem.SubHeaderProperty);
            }
            pieMenu.InvalidateVisual();
        }

        private void UpdateInputField(SearchTagView searchTagView, TagDataModel tagDM)
        {
            //tagDM.InputTerms = searchTagView.InputField.
            //searchTagView.DataContext = tagDM;
        }


        public void ToogleInputVisibility(bool visible)
        {
            //!! Load elements dynamically
            //if (visible == false) SearchInputPanel.Visibility = Visibility.Hidden;
            //else if (visible == true) SearchInputPanel.Visibility = Visibility.Visible;
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            //_viewModel.OnVisualizationRemoved(e.TagVisualization);
        }

        private void SearchTagVisualizer_GotTouchCapture(object sender, TouchEventArgs e)
        {
            //When tag is placed
            //MessageBox.Show("SV: SearchTagVisualizer_GotTouchCapture");
        }

        private void SearchTagVisualizer_GotFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("SV: SearchTagVisualizer_GotFocus");

        }

        private void SearchTagVisualizer_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            //MessageBox.Show("SV: SearchTagVisualizer_SourceUpdated");

        }

        private void SearchTagVisualizer_TouchEnter(object sender, TouchEventArgs e)
        {
            //When tag is placed
            //MessageBox.Show("SV: SearchTagVisualizer_TouchEnter");

        }

        private void SearchTagVisualizer_TouchDown(object sender, TouchEventArgs e)
        {
            //MessageBox.Show("SV: SearchTagVisualizer_TouchDown");

        }

        private void PieMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("SV: PieMenuItem_Click");
            var t = (TouchEventArgs)e;
            
            if (!t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {
                MessageBox.Show("SV: PieMenuItem_Click if");
            }
        }
        
        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            
            //MessageBox.Show("SV: MyTagVisualization_PreviewTouchDown");
        }

        private void PieMenuItem_ItemPreviewTouchDown(object sender, TouchEventArgs e)
        {
            //Is not called 
        }
        
        public void UpdateMenuItems(int tagId)
        {
            var pieMenu = Tags[tagId].PieMenu;
            var tagDM   = _viewModel.Tags[tagId];

            var pieMenuItems = (ItemCollection) pieMenu.Items;

            // remove inserted placeholder item which has been placed just for correct item size calculation
            pieMenuItems.Clear();

            //if (tagDM.IsMenuVisible == false) return;

            var options = tagDM.Tag.TagOptions.Where(to => to.LayerNr == tagDM.Tag.CurrentLayerNr);
            foreach (var option in options)
            {
                var hexColor = "#5555";
                if (option.Id % 3 == 1) { hexColor = "#2222"; }
                if (option.Id % 3 == 2) { hexColor = "#0000"; }
                var brush = (Brush)(new BrushConverter().ConvertFrom(hexColor));

                var pieMenuItem = new PieMenuItem()
                {
                    Id = option.Id,
                    BorderThickness = new Thickness(0.0),
                    FontSize = 16,
                    CenterText = true,
                    Background = brush
                };

                // Id binding
                Binding idBinding = new Binding("Id");
                idBinding.Source = option;
                idBinding.NotifyOnSourceUpdated = true;
                idBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.IdProperty, idBinding);

                // Header binding
                Binding headerBinding = new Binding("Keyword.Name");
                headerBinding.Source = option;
                headerBinding.NotifyOnSourceUpdated = true;
                headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.HeaderProperty, headerBinding);

                // SubHeader binding
                Binding subHeaderBinding = new Binding("Keyword.Description");
                subHeaderBinding.Source = option;
                subHeaderBinding.NotifyOnSourceUpdated = true;
                subHeaderBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.SubHeaderProperty, subHeaderBinding);

                // Command binding
                Binding commandBinding = new Binding("SelectOptionCmd");
                commandBinding.Source = _viewModel;                
                commandBinding.NotifyOnSourceUpdated = true;
                commandBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.CommandProperty, commandBinding);

                pieMenu.Items.Add(pieMenuItem);
            }

            pieMenu.InvalidateVisual();
        }

        private void Old_OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            /*
            // Every time tag is placed its visualization must be initiated again (is lost after tag remove)

            _viewModel.OnVisualizationAdded(e.TagVisualization);

            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;
            var tagDM = _viewModel.Tags[tagId];

            searchTagView.PieMenu.Items.Clear();
            searchTagView.PieMenu.ItemsSource = null;

            searchTagView.Resources["SearchVM"] = _viewModel;
            searchTagView.Resources["TagDM"] = tagDM;
            searchTagView.Resources["TagId"] = tagId;
            searchTagView.Resources["TagOptions"] = tagDM.Tag.TagOptions;
            searchTagView.Resources["PieMenuItems"] = tagDM.PieMenuItems;

            UpdateMenuItems(searchTagView, tagDM);
            UpdateInputField(searchTagView, tagDM);

            UpdateVisual(tagId);
            */ 
        }

        /*
        public void Old_UpdateMenuItems(ISearchTagView searchTagView, TagDataModel tagDM)
        {
            var pieMenu = ((PieMenu)((SearchTagView)searchTagView).PieMenu);
            var pieMenuItems = (ItemCollection)pieMenu.Items;

            // remove inserted placeholder item which has been placed just for correct item size calculation
            pieMenuItems.Clear();

            var options = tagDM.Tag.TagOptions;
            foreach (var option in options)
            {
                var brush = (Brush)(new BrushConverter().ConvertFrom("#5555"));

                var pieMenuItem = new PieMenuItem()
                {
                    Id = option.Id,
                    Header = option.Keyword.Name,
                    SubHeader = option.Keyword.Description,
                    BorderThickness = new Thickness(0.0),
                    FontSize = 16,
                    CenterText = true,
                    Command = _viewModel.SelectOptionCmd,
                    Background = brush
                };
                pieMenu.Items.Add(pieMenuItem);
            }
            pieMenu.InvalidateVisual();
        }*/
    }
}
