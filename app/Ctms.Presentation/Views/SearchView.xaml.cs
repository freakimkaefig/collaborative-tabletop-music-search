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
using Ctms.Presentation.Converters;
using System.Windows.Media.Animation;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        private readonly Lazy<SearchViewModel> _lazyVm;

        public Dictionary<int, SearchTagView> SearchTagViews;

        public SearchView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchViewModel>(() => ViewHelper.GetViewModel<SearchViewModel>(this));
            SearchTagViews = new Dictionary<int, SearchTagView>();
                
        }

        // Provides this view's viewmodel
        private SearchViewModel _viewModel { get { return _lazyVm.Value; } }

        public TagVisualizer TagVisualizer { get { return SearchTagVisualizer; } set {} }

        public void InitializeRectangles()
        {
            List<System.Windows.Shapes.Rectangle> rectangles = new List<System.Windows.Shapes.Rectangle>();
            rectangles.Add(Fft1);
            rectangles.Add(Fft2);
            rectangles.Add(Fft3);
            rectangles.Add(Fft4);
            rectangles.Add(Fft5);
            rectangles.Add(Fft6);
            rectangles.Add(Fft7);
            rectangles.Add(Fft8);
            rectangles.Add(Fft9);
            rectangles.Add(Fft10);
            rectangles.Add(Fft11);
            rectangles.Add(Fft12);
            rectangles.Add(Fft13);
            rectangles.Add(Fft14);
            rectangles.Add(Fft15);

            _viewModel.FftRectangle = rectangles;
        }

        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            // Every time tag is placed its visualization must be initiated again (is lost after tag remove)

            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;
            var tagDM = _viewModel.Tags[tagId];
            var pieMenu = searchTagView.PieMenu;

            searchTagView.ViewModel = _viewModel;

            if(SearchTagViews.ContainsKey(tagId) == false) SearchTagViews.Add(tagId, searchTagView);
            else
            {
                SearchTagViews[tagId] = searchTagView;
            }

            UpdateVisual(tagId);

            tagDM.Height = (float) searchTagView.ActualHeight;
        }

       /* private void UpdateAnimation()
        {
            try
            {
                //Storyboard sb = (Storyboard)Application.Current.Resources["Tests"];
                //animation.BeginStoryboard(sb);
            }
            catch (Exception)
            {
            }
        }*/

        private void UpdateResources(SearchTagView searchTagView, int tagId, TagDataModel tagDM)
        {
            //  create dynamic resources
            searchTagView.Resources["SearchVM"] = _viewModel;
            searchTagView.Resources["TagDM"] = tagDM;
            searchTagView.Resources["TagId"] = tagId;
            searchTagView.Resources["BreadcrumbOptions"] = tagDM.Tag.BreadcrumbOptions;

            var converter = new BoolToVisibilityConverter();
            var isEditVisible = converter.Convert(tagDM.IsEditVisible, null, null, null);
            searchTagView.Resources["IsEditVisible"] = converter.Convert(tagDM.IsEditVisible, null, null, null);
            searchTagView.Resources["IsMenuVisible"] = converter.Convert(tagDM.IsMenuVisible, null, null, null);

            
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
            //if (SearchTagViews.ContainsKey(tagId) == false) SearchTagViews.Add(tagId, null);

            UpdateMenuItems(tagId);
            UpdateResources(SearchTagViews[tagId], tagId, _viewModel.Tags[tagId]);
            CalcMenuVisibility(SearchTagViews[tagId], _viewModel.Tags[tagId]);

            var pieMenu = SearchTagViews[tagId].PieMenu;

            foreach (PieMenuItem item in pieMenu.Items)
            {
                item.InvalidateVisual();
                item.InvalidateProperty(PieMenuItem.HeaderProperty);
                item.InvalidateProperty(PieMenuItem.SubHeaderProperty);
            }
            pieMenu.InvalidateVisual();

           // UpdateAnimation();
        }

        /// <summary>
        /// Update items of pie menu
        /// </summary>
        public void UpdateMenuItems(int tagId)
        {
            var pieMenu = SearchTagViews[tagId].PieMenu;
            var pieMenuMain = SearchTagViews[tagId].PieMenuMain;
            var tagDM = _viewModel.Tags[tagId];

            var pieMenuItems    = (ItemCollection)pieMenu.Items;
            var pieMenuMainItem = (ItemCollection)pieMenuMain.Items;

            // remove inserted items (when initializing this is the placeholder item)
            pieMenuItems.Clear();
            pieMenuMainItem.Clear();

            var options = tagDM.VisibleOptions;

            var count = options.Count;

            var i = 0;
            TagOption option;

            // loop backwards so that first element of options is last in pieMenu
            for (i = options.Count - 1; i >= 0; i--)
            {
                option              = options[i];
                var backgroundHex   = "";
                var textHex         = "";

                if ((tagDM.CurrentOptionsIndex + i) % 3 == 0) 
                {
                    backgroundHex = "#5000";
                    textHex = "#ffff";
                }
                else if ((tagDM.CurrentOptionsIndex + i) % 3 == 1)
                {
                    backgroundHex = "#5444";
                    textHex = "#ffff";
                }
                else if ((tagDM.CurrentOptionsIndex + i) % 3 == 2)
                {
                    backgroundHex = "#5888";
                    textHex = "#ffff";
                }
                backgroundHex = "#0000";

                var backgroundColor = (Brush)(new BrushConverter().ConvertFrom(backgroundHex));
                var textColor = (Brush)(new BrushConverter().ConvertFrom(textHex));

                var pieMenuItem = new PieMenuItem()
                {
                    Id = option.Id,
                    BorderThickness = new Thickness(0.0),
                    Foreground = textColor,
                    Background = backgroundColor,
                    CenterTextVertically = true
                };

                if (i == 0)
                {
                    pieMenuItem.FontSize = 16;
                    pieMenuItem.CenterTextHorizontal = true;
                }
                else
	            {
                    pieMenuItem.FontSize = 13;
                    pieMenuItem.CenterTextHorizontal = false;
	            }

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

                if (i == 0)
                {   // Add item to main pie menu with one big option
                    pieMenuMain.Items.Add(pieMenuItem);
                }
                else
                {   // add item to pie menu with multiple small options
                    pieMenu.Items.Add(pieMenuItem);
                }
            }

            pieMenu.InvalidateVisual();
            pieMenuMain.InvalidateVisual();
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // problem is that options of the pie menu can be pressed with the tag. this can't 
            // be avoided, because the framework doesn't recognize early enough that the tag isn't recognized anymore
            // and thinks that the tag is a finger. So the combination of fingerrecognized and tagrecognized doesn't help
            // with identifying finger touches unambiguously
            /* Don't delete, may will be needed later
            var t = (TouchEventArgs)e;

            Debug.WriteLine("SV: MyTagVisualization_PreviewTouchDown");
            Debug.WriteLine("SV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            Debug.WriteLine("SV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());
            */
        }

        #region UnusedEvents

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

        #endregion UnusedEvents
    }
}
