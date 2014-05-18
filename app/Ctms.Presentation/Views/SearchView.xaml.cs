using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Ctms.Applications.DataModels;
using Ctms.Applications.DevHelper;
using Ctms.Applications.ViewModels;
using Ctms.Applications.Views;
using Ctms.Domain.Objects;
using Ctms.Presentation.Converters;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using PieInTheSky;
using System.Diagnostics;
using System.Collections;
using Helpers;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für search.xaml
    /// </summary>
    [Export(typeof(ISearchView))]
    public partial class SearchView : UserControl, ISearchView
    {
        private readonly Lazy<SearchViewModel> _lazyVm;
        private Rectangle _fft1;
        private Rectangle _fft2;
        private Rectangle _fft3;
        private Rectangle _fft4;
        private Rectangle _fft5;
        private Rectangle _fft6;
        private Rectangle _fft7;
        private Rectangle _fft8;
        private Rectangle _fft9;
        private Rectangle _fft10;
        private Rectangle _fft11;
        private Rectangle _fft12;
        private Rectangle _fft13;
        private Rectangle _fft14;
        private Rectangle _fft15;

        public Dictionary<int, SearchTagView> SearchTagViews;

        private string _backgroundHex = "#0000";
        private string _textHex = "#fff";
        private Storyboard storyboardResource;
        private Brush backgroundColor;
        private Brush textColor;
        private Random rnd;
        private Stopwatch timer;
        private TimingHelper timingHelper;

        //private TextBox focusedElement;
        //private KeyboardController keyboard;

        public SearchView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchViewModel>(() => ViewHelper.GetViewModel<SearchViewModel>(this));
            SearchTagViews = new Dictionary<int, SearchTagView>();

            _fft1 = Fft1;
            _fft2 = Fft2;
            _fft3 = Fft3;
            _fft4 = Fft4;
            _fft5 = Fft5;
            _fft6 = Fft6;
            _fft7 = Fft7;
            _fft8 = Fft8;
            _fft9 = Fft9;
            _fft10 = Fft10;
            _fft11 = Fft11;
            _fft12 = Fft12;
            _fft13 = Fft13;
            _fft14 = Fft14;
            _fft15 = Fft15;

            //focusedElement = this.KeyboardInput;
            //initKeyboard(); 
            InitPieMenu();

            timingHelper = new TimingHelper();
            timingHelper.InitTimeMeasure();

            storyDict = new Dictionary<Storyboard, Tuple<Ellipse, TagCombinationDataModel>>();
        }

        private void InitPieMenu()
        {
            // get storyboard resource
            storyboardResource = this.Resources["TagCombiStoryboard"] as Storyboard;

            backgroundColor = (Brush)(new BrushConverter().ConvertFrom(_backgroundHex));
            textColor = (Brush)(new BrushConverter().ConvertFrom(_textHex));
        }

        public Rectangle GetFft1 { get { return _fft1; } set { } }
        public Rectangle GetFft2 { get { return _fft2; } set { } }
        public Rectangle GetFft3 { get { return _fft3; } set { } }
        public Rectangle GetFft4 { get { return _fft4; } set { } }
        public Rectangle GetFft5 { get { return _fft5; } set { } }
        public Rectangle GetFft6 { get { return _fft6; } set { } }
        public Rectangle GetFft7 { get { return _fft7; } set { } }
        public Rectangle GetFft8 { get { return _fft8; } set { } }
        public Rectangle GetFft9 { get { return _fft9; } set { } }
        public Rectangle GetFft10 { get { return _fft10; } set { } }
        public Rectangle GetFft11 { get { return _fft11; } set { } }
        public Rectangle GetFft12 { get { return _fft12; } set { } }
        public Rectangle GetFft13 { get { return _fft13; } set { } }
        public Rectangle GetFft14 { get { return _fft14; } set { } }
        public Rectangle GetFft15 { get { return _fft15; } set { } }

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

            searchTagView.SearchVm = _viewModel;

            if(SearchTagViews.ContainsKey(tagId) == false) SearchTagViews.Add(tagId, searchTagView);
            else
            {
                SearchTagViews[tagId] = searchTagView;
            }

            UpdateVisual(tagId);

            tagDM.Height        = (float)searchTagView.ActualHeight;
            tagDM.Width         = (float)searchTagView.ActualWidth;

            //var screenPosition  = searchTagView.PointToScreen(new Point(0d, 0d));
            var screenPosition  = searchTagView.Center;
            tagDM.Tag.PositionX = (short)(screenPosition.X);
            tagDM.Tag.PositionY = (short)(screenPosition.Y);

            tagDM.ExistenceState = TagDataModel.ExistenceStates.Added;
        }

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
            searchTagView.Resources["IsCircleMenuVisible"] = converter.Convert(tagDM.IsCircleMenuVisible, null, null, null);
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
            //timingHelper.StartMeasureTime();
            UpdateMenuItems(tagId);
            //Console.WriteLine("UpdateMenuItems");
            //timingHelper.StopMeasureTime();

            //timingHelper.StartMeasureTime();
            UpdateResources(SearchTagViews[tagId], tagId, _viewModel.Tags[tagId]);
            //Console.WriteLine("UpdateResources");
            //timingHelper.StopMeasureTime();

            //timingHelper.StartMeasureTime();
            CalcMenuVisibility(SearchTagViews[tagId], _viewModel.Tags[tagId]);
            //Console.WriteLine("CalcMenuVisibility");
            //timingHelper.StopMeasureTime();

            //timingHelper.StartMeasureTime();
            // update pie menu visually
            var pieMenu = SearchTagViews[tagId].PieMenu;
            foreach (PieMenuItem item in pieMenu.Items)
            {
                item.InvalidateVisual();
            }
            pieMenu.InvalidateVisual();
            //Console.WriteLine("InvalidateVisual");
            //timingHelper.StopMeasureTime();
        }

        /// <summary>
        /// Update items of pie menu: clear old items and add new items. Set values by binding
        /// </summary>
        public void UpdateMenuItems(int tagId)
        {
            //Console.WriteLine("UpdateMenuItems");
            //timingHelper.StartMeasureTime();

            var pieMenu     = SearchTagViews[tagId].PieMenu;
            var tagDM       = _viewModel.Tags[tagId];
            var pieMenuItems= (ItemCollection)pieMenu.Items;
            var options     = tagDM.VisibleOptions;
            var count       = options.Count;
            var i = 0;
            TagOption option;

            // remove inserted items (when initializing this is the placeholder item)
            pieMenuItems.Clear();

            // loop backwards so that first element of options is last in pieMenu
            for (i = options.Count - 1; i >= 0; i--)
            {
                option              = options[i];

                var pieMenuItem = new PieMenuItem()
                {
                    Id = option.Id,
                    BorderThickness = new Thickness(0),
                    BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#171815")),
                    Foreground = textColor,
                    Background = backgroundColor,
                    CenterTextVertically = true
                };

                if (options.Count == 1 || options.Count == 2 || options.Count == 3)
                {   // center text if there are 1-3 options and make it bigger
                    pieMenuItem.FontSize = 14;
                    pieMenuItem.CenterTextHorizontal = true;
                }
                else if (options.Count >= 4)
                {
                    if (i == 0)
                    {
                        // this is a main item. center it and make it bigger
                        pieMenuItem.FontSize = 14;
                        pieMenuItem.CenterTextHorizontal = true;
                    }
                    else
                    {   // this is not a main item
                        pieMenuItem.FontSize = 12;
                        pieMenuItem.CenterTextHorizontal = false;
                    }
                }

                // set id and header
                pieMenuItem.Id      = option.Id;
                pieMenuItem.Header  = option.Keyword.DisplayName;

                // bind command
                Binding commandBinding = new Binding("SelectOptionCmd");
                commandBinding.Source = _viewModel;
                commandBinding.NotifyOnSourceUpdated = true;
                commandBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.CommandProperty, commandBinding);
                
                pieMenu.Items.Add(pieMenuItem);
            }

            pieMenu.InvalidateVisual();

            //timingHelper.StopMeasureTime();
        }

        /// <summary>
        /// Tag has been removed from table, so update variables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;
            var tagDM = _viewModel.Tags[tagId];

            _viewModel.OnVisualizationRemoved(tagDM);
        }

        /// <summary>
        /// Stop animation loop when tag is removed from combintion
        /// </summary>
        private void combiTag_Unloaded(object sender, RoutedEventArgs e)
        {
            //Storyboard storyboardResource = this.Resources["TagCombiStoryboard"] as Storyboard;
            storyboardResource.Completed -= (p, s) => PlayCombiAnimation(null);
        }

        /// <summary>
        /// Start animation when tag is loaded
        /// </summary>
        /// <param name="sender">The grid which contains the ellipse</param>
        /// <param name="e"></param>
        private void combiTag_Loaded(object sender, RoutedEventArgs e)
        {
            // the grid which contains the ellipse
            var grid = sender as Grid;

            // get ellipse by name
            var ellipse = grid.FindName("combiEll") as Ellipse;

            // get the combi's id which is hidden in the textblock as text
            var combiId = Int32.Parse(((TextBlock)grid.FindName("HiddenTagCombiId")).Text);
            var combi = _viewModel.TagCombinations.FirstOrDefault(tc => tc.Id == combiId);

            //var parameters = new Tuple<Storyboard, Ellipse, TagCombinationDataModel>(storyboardResource, ellipse, combi);
            var parameters = new Tuple<Storyboard, Ellipse, TagCombinationDataModel>(storyboardResource, ellipse, combi);

            // create loop of animation
            //storyboardResource.Completed -= (p, s) => PlayCombiAnimation(parameters);
            storyboardResource.Completed += (p, s) => PlayCombiAnimation(parameters);

            // first start
            //PlayCombiAnimation(parameters);
            storyboardResource.Begin(ellipse, true);
            //storyboardResource.Begin(grid, true);
        }

        private int counter = 0;

        private Dictionary<Storyboard, Tuple<Ellipse, TagCombinationDataModel>> storyDict;

        /// <summary>
        /// Play combination animation for a tag
        /// </summary>
        /// <param name="parameters">Tuple of storyboard, ellipse and tagCombinaton</param>
        private void PlayCombiAnimation(Tuple<Storyboard, Ellipse, TagCombinationDataModel> parameters)
        {
            //counter++;
            //timingHelper.StartMeasureTime();
            //Console.WriteLine(counter + "PlayCombiAnimation for tag: " + ((TagDataModel)parameters.Item2.DataContext).Tag.Id);
            // loop through animation elements of storyboard
            foreach (var animationElement in parameters.Item1.Children)
            {
                //timingHelper.StartMeasureTime();
                if (animationElement.Name == "XTransform")
                {   //set animation's from and to for x coordinate

                    // start from tag position
                    if (parameters.Item2.DataContext is TagDataModel)
                        (animationElement as DoubleAnimation).From = ((TagDataModel)parameters.Item2.DataContext).Tag.PositionX;

                    // end at combi center
                    (animationElement as DoubleAnimation).To = parameters.Item3.CenterX;
                }
                else if (animationElement.Name == "YTransform")
                {   //set animation's from and to for y coordinate

                    // start from tag position
                    if (parameters.Item2.DataContext is TagDataModel)
                        (animationElement as DoubleAnimation).From = ((TagDataModel)parameters.Item2.DataContext).Tag.PositionY;
                    
                    // end at combi center
                    (animationElement as DoubleAnimation).To = parameters.Item3.CenterY;
                }

                //timingHelper.StopMeasureTime();
            }

            //storyboardResource.Completed -= (p, s) => PlayCombiAnimation(parameters);
            //timingHelper.StopMeasureTime();
            // start storyboard animation
            //parameters.Item1.Begin(parameters.Item2, true);
            parameters.Item1.Begin(parameters.Item2, true);

        }


        private void SearchTagVisualizer_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTagVisualizer_GotTouchCapture(object sender, TouchEventArgs e)
        {

        }


        private int eventCounter;
        private void TagCombiStoryboard_Completed(object sender, EventArgs e)
        {
            //eventCounter++;
            //Console.WriteLine(eventCounter + "TagCombiStoryboard_Completed");
            /*
            var StoryBoardName = ((ClockGroup)sender).Timeline.Name;
            var storyBoard2 = (Storyboard)((ClockGroup)sender).Timeline;

            var animClock = sender as ClockGroup;
            var timeline = animClock.Timeline as Storyboard;

            var target = Storyboard.GetTarget(timeline);
            storyDict[storyBoard2] = new Tuple<Ellipse, TagCombinationDataModel>(null, null);

            */
            //storyBoard2.
            /*
            var count = storyDict.Count;
            // loop through animation elements of storyboard
            foreach (var animationElement in parameters.Item1.Children)
            {
                //timingHelper.StartMeasureTime();
                if (animationElement.Name == "XTransform")
                {   //set animation's from and to for x coordinate

                    // start from tag position
                    if (parameters.Item2.DataContext is TagDataModel)
                        (animationElement as DoubleAnimation).From = ((TagDataModel)parameters.Item2.DataContext).Tag.PositionX;

                    // end at combi center
                    (animationElement as DoubleAnimation).To = parameters.Item3.CenterX;
                }
                else if (animationElement.Name == "YTransform")
                {   //set animation's from and to for y coordinate

                    // start from tag position
                    if (parameters.Item2.DataContext is TagDataModel)
                        (animationElement as DoubleAnimation).From = ((TagDataModel)parameters.Item2.DataContext).Tag.PositionY;
                    
                    // end at combi center
                    (animationElement as DoubleAnimation).To = parameters.Item3.CenterY;
                }

                //timingHelper.StopMeasureTime();
            }
            //*/
        }
    }
}
