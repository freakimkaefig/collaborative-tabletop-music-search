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
using Helpers;
using WPFKeyboard.Keyboard;
using Ctms.Applications.DevHelper;

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

        private bool added = false;

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

        private void Rotate()
        {
            throw new NotImplementedException();
            _viewModel.Tags[0].Tag.Orientation += 2;

            TimingHelper.SetTimeout(10, Rotate);
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
        }

        /// <summary>
        /// Update items of pie menu
        /// </summary>
        public void UpdateMenuItems(int tagId)
        {
            var pieMenu = SearchTagViews[tagId].PieMenu;
            var tagDM = _viewModel.Tags[tagId];

            var pieMenuItems    = (ItemCollection)pieMenu.Items;

            // remove inserted items (when initializing this is the placeholder item)
            pieMenuItems.Clear();

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
                /*
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
                }*/
                backgroundHex = "#0000";
                textHex = "#fff";

                var backgroundColor = (Brush)(new BrushConverter().ConvertFrom(backgroundHex));
                var textColor = (Brush)(new BrushConverter().ConvertFrom(textHex));

                var pieMenuItem = new PieMenuItem()
                {
                    Id = option.Id,
                    BorderThickness = new Thickness(0),
                    BorderBrush = (Brush)(new BrushConverter().ConvertFrom("#171815")),
                    Foreground = textColor,
                    Background = backgroundColor,
                    CenterTextVertically = true
                };

                if (options.Count == 1)
                {
                    pieMenuItem.FontSize = 14;
                    pieMenuItem.CenterTextHorizontal = true;
                }
                else if (options.Count == 2)
                {
                    pieMenuItem.FontSize = 14;
                    pieMenuItem.CenterTextHorizontal = true;
                }
                else if (options.Count == 3)
                {
                    pieMenuItem.FontSize = 14;
                    pieMenuItem.CenterTextHorizontal = true;
                }
                else if (options.Count >= 3)
                {
                    if (i == 0)
                    {
                        pieMenuItem.FontSize = 14;
                        pieMenuItem.CenterTextHorizontal = true;
                    }
                    else
                    {
                        pieMenuItem.FontSize = 12;
                        pieMenuItem.CenterTextHorizontal = false;
                    }
                }

                // Id binding
                Binding idBinding = new Binding("Id");
                idBinding.Source = option;
                idBinding.NotifyOnSourceUpdated = true;
                idBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.IdProperty, idBinding);

                // Header binding
                Binding headerBinding = new Binding("Keyword.DisplayName");
                headerBinding.Source = option;
                headerBinding.NotifyOnSourceUpdated = true;
                headerBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                pieMenuItem.SetBinding(PieMenuItem.HeaderProperty, headerBinding);

                // SubHeader binding
                Binding subHeaderBinding = new Binding("Keyword.DisplayDescription");
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

                //if (i == 0)
                {   // Add item to main pie menu with one big option
                    //pieMenuMain.Items.Add(pieMenuItem);
                }
                //else
                {   // add item to pie menu with multiple small options
                    pieMenu.Items.Add(pieMenuItem);
                }
            }

            pieMenu.InvalidateVisual();
            //pieMenuMain.InvalidateVisual();
        }

        public void CreateEllipse()
        {

        }

        public void UpdateStoryboard(int combiId)
        {
            /*
            var itemControls = FindVisualChildren<ItemsControl>(this);
            var storyboards = FindVisualChildren<Storyboard>(this);
            var beginStoryboards = FindVisualChildren<BeginStoryboard>(this);
            var doubleAnimations = FindVisualChildren<DoubleAnimation>(this);

            Storyboard storyboardResource = this.Resources["TagCombiStoryboard"] as Storyboard;

            //var ellipse = this.Resources["EllipseAnimation"] as Ellipse;
            //storyboardResource.Begin(combiEll);

            var tagCombinationItems = TagCombinationsControl.Items;
            //var childControls = FindVisualChildren<Grid>(this);
            //var childControls3 = FindVisualChildren<Ellipse>(this);

            storyboardResource.Begin(el1);
            */
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            _viewModel.AddLog("SV: OnVisualizationRemoved");

            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;
            var tagDM = _viewModel.Tags[tagId];

            tagDM.ExistenceState = TagDataModel.ExistenceStates.Removed;

            _viewModel.OnVisualizationRemoved(tagDM);
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // problem is that options of the pie menu can be pressed with the tag. this can't 
            // be avoided, because the framework doesn't recognize early enough that the tag isn't recognized anymore
            // and thinks that the tag is a finger. So the combination of fingerrecognized and tagrecognized doesn't help
            // with identifying finger touches unambiguously
            /* Don't delete, may will be needed later
            var t = (TouchEventArgs)e;

            DevLogger.Log("SV: MyTagVisualization_PreviewTouchDown");
            DevLogger.Log("SV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            DevLogger.Log("SV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());
            */

            var t = (TouchEventArgs)e;
            //_viewModel.AddLog("SV: MyTagVisualization_PreviewTouchDown");
            //_viewModel.AddLog("SV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            //_viewModel.AddLog("SV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());
        }

        #region UnusedEvents

        private void SearchTagVisualizer_GotTouchCapture(object sender, TouchEventArgs e)
        {
            //When tag is placed
            _viewModel.AddLog("SV: SearchTagVisualizer_GotTouchCapture");
        }

        private void SearchTagVisualizer_GotFocus(object sender, RoutedEventArgs e)
        {
            _viewModel.AddLog("SV: SearchTagVisualizer_GotFocus");

        }

        private void SearchTagVisualizer_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            _viewModel.AddLog("SV: SearchTagVisualizer_SourceUpdated");

        }

        private void SearchTagVisualizer_TouchEnter(object sender, TouchEventArgs e)
        {
            //When tag is placed
            _viewModel.AddLog("SV: SearchTagVisualizer_TouchEnter");

        }

        private void SearchTagVisualizer_TouchDown(object sender, TouchEventArgs e)
        {
            _viewModel.AddLog("SV: SearchTagVisualizer_TouchDown");

        }

        #endregion UnusedEvents

        private void SearchTagVisualizer_LostTouchCapture(object sender, TouchEventArgs e)
        {
            //_viewModel.AddLog("SV: SearchTagVisualizer_LostTouchCapture");
        }

        private void SearchTagVisualizer_LostFocus(object sender, RoutedEventArgs e)
        {
            //_viewModel.AddLog("SV: SearchTagVisualizer_LostFocus");
        }

        public void LogScrollToEnd()
        {
            //SearchViewLog.ScrollToEnd();
        }

        private void combiTag_Unloaded(object sender, RoutedEventArgs e)
        {
            Storyboard storyboardResource = this.Resources["TagCombiStoryboard"] as Storyboard;
            storyboardResource.Completed -= (p, s) => PlayCombiAnimation(null);
        }

        private void combiTag_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;

            // get ellipse by name
            var ellipse = grid.FindName("combiEll") as Ellipse;

            // get the combi's id which is hidden in the textblock as text
            var combiId = Int32.Parse(((TextBlock)grid.FindName("HiddenTagCombiId")).Text);
            var combi = _viewModel.TagCombinations.FirstOrDefault(tc => tc.Id == combiId);

            // get storyboard resource
            Storyboard storyboardResource = this.Resources["TagCombiStoryboard"] as Storyboard;

            var parameters = new Tuple<Storyboard, Ellipse, TagCombinationDataModel>(storyboardResource, ellipse, combi);

            storyboardResource.Completed += (p, s) => PlayCombiAnimation(parameters);

            PlayCombiAnimation(parameters);
        }

        private void PlayCombiAnimation(Tuple<Storyboard, Ellipse, TagCombinationDataModel> parameters)
        {
            var storyboard  = parameters.Item1;
            var ellipse     = parameters.Item2;
            var combi       = parameters.Item3;
            
            //if (storyboard.Is == ClockState.Stopped)
            {
                storyboard.Begin(ellipse, true);
                storyboard.Seek(ellipse, TimeSpan.FromSeconds(0), TimeSeekOrigin.BeginTime);
            }

            // loop through animation elements of storyboard
            var animationElements = storyboard.Children;
            foreach (var animationElement in animationElements)
            {
                if (animationElement.Name == "XTransform")
                {
                    var xTransform = animationElement as DoubleAnimation;

                    if (ellipse.DataContext is TagDataModel)
                        xTransform.From = ((TagDataModel)ellipse.DataContext).Tag.PositionX;

                    xTransform.To = combi.CenterX;
                }
                else if (animationElement.Name == "YTransform")
                {
                    var yTransform = animationElement as DoubleAnimation;

                    if (ellipse.DataContext is TagDataModel)
                        yTransform.From = ((TagDataModel)ellipse.DataContext).Tag.PositionY;

                    yTransform.To = combi.CenterY;
                }
                
                 
            }
          
            storyboard.Begin(ellipse, true);
        }

        private void SurfaceButton_TouchDown(object sender, TouchEventArgs e)
        {
            if (!e.TouchDevice.GetIsFingerRecognized() && !e.TouchDevice.GetIsTagRecognized())
            {
                DevLogger.Log("HandledEventArgs!");
                //e.Handled = true;
            }

        }
        /*
        private void initKeyboard()
        {
            keyboard = this.CustomKeyboard;
            keyboard.setFocusedElement(focusedElement);

            //OPTIONAL
            //keyboard.setCustomKeyboardListener(this);
        }

        /// <summary>
        /// only for demonstration
        /// </summary>
        /// <returns></returns>
        public TextBox getFocusedElement()
        {
            return focusedElement;
        }

        /// <summary>
        /// an element with no caret needed is focused
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotFocusableElement_TouchDown(object sender, TouchEventArgs e)
        {
            focusedElement = null;
            keyboard.setFocusedElement(focusedElement);
        }

        private void TextBox_TouchUp(object sender, TouchEventArgs e)
        {
            focusedElement = sender as TextBox;
            keyboard.setFocusedElement(focusedElement);
        }
        */
        /*
         * COMPLETELY OPTIONAL -> only if you want to knwo which key was pressed
         */
        /*
        /// <summary>
        /// normal key/ new line/ space was pressed
        /// </summary>
        /// <param name="key">key (can be parsed in char) or "\n" or " "</param>
        public void typedKey(string key)
        {

        }

        /// <summary>
        /// backspace was typed
        /// </summary>
        public void typedBackSpace()
        {

        }

        /// <summary>
        /// one of the arrows was typed
        /// </summary>
        /// <param name="arrowIndex"> -1 for left-arrow; 1 for right arrow</param>
        public void typedArrow(int arrowIndex)
        {

        }*/
    }
}
