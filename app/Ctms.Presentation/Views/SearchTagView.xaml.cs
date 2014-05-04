using System.Windows;
using System.Diagnostics;
using System.Windows.Controls;
using Ctms.Applications.Views;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using System.Waf.Applications;
using System;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Input;
using System.Windows.Data;
using Microsoft.Surface.Presentation.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Ctms.Applications.DataModels;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export(typeof(ISearchTagView))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class SearchTagView : ISearchTagView
    {
        private readonly Lazy<SearchTagViewModel> _lazyVm;

        private int count = 0;

        private short windowHeight;

        public SearchTagView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchTagViewModel>(() => ViewHelper.GetViewModel<SearchTagViewModel>(this));

            count++;

            windowHeight = (short)Application.Current.MainWindow.ActualHeight;
        }

        public SearchViewModel SearchVm { get; set; }

        // Provides this view's viewmodel
        private SearchTagViewModel _searchTagVm { get { return _lazyVm.Value; } }

        private void TagVisualization_Moved(object sender, TagVisualizerEventArgs e)
        {
            //e.TagVisualization.Center; //passing center of tag to update animation between tangibles

            var searchTagView   = (SearchTagView)e.TagVisualization;
            var tagId           = (int)searchTagView.VisualizedTag.Value;

            var screenPosition = searchTagView.PointToScreen(new Point(0d, 0d));

            // set angle and position of this tag
            var tag         = SearchVm.Tags[tagId];
            var trackedTouch = e.TagVisualization.TrackedTouch;
            if (trackedTouch != null)
            {
                tag.Tag.Angle = (short)trackedTouch.GetOrientation(this);
            }

            tag.Tag.PositionX   = (short) (screenPosition.X + tag.Width / 2);
            tag.Tag.PositionY   = (short) (screenPosition.Y + tag.Height / 2);

            // orientate tag to the nearest side of the two long sides
            tag.Tag.Orientation = tag.Tag.PositionY > (windowHeight / 2) - (tag.Height / 2) ? (short) 0 : (short) 180;

            if (tag.AssignState == TagDataModel.AssignStates.Assigned)
            {
                SearchVm.CheckTagPositionsCmd.Execute(tag.Id);
            }

            tag.UpdateVisibleOptions();
        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //_viewModel.Id = e.TagVisualization.VisualizedTag.Value;
            //Tag = e.TagVisualization.VisualizedTag;
            //Debug.WriteLine("STV: SimpleVisualization_Loaded");
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // Called when finger touch and tangible click on main pie menu item. 
            // Not when mouse click

            var t = (TouchEventArgs)e;

            //Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown");
            //Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            //Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());
            
            if (!t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {   //!! Funktioniert! Taginput wird abgefangen
                //MessageBox.Show("SV: PieMenuItem_Click if");
                //Debug.WriteLine("STV: No finger, no Tag");
                t.Handled = true;
            }
            else if (t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {
                //Debug.WriteLine("STV: Finger, no Tag");
                //t.Handled = true;
            }
            else if (!t.TouchDevice.GetIsFingerRecognized() && t.TouchDevice.GetIsTagRecognized())
            {
                //Debug.WriteLine("STV: No Finger, but Tag");
                //t.Handled = true;
                //t.TouchDevice.
                //var searchTagView = (SearchTagView)e.TagVisualization;
            }
            else
            {
                //Debug.WriteLine("STV: No Finger, no Tag");

            }
        }

        public void LogScrollToEnd()
        {
            //SearchTagViewLog.ScrollToEnd();
        }
        
    }
}
