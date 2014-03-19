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
        }

        public SearchViewModel ViewModel { get; set; }

        // Provides this view's viewmodel
        private SearchTagViewModel _viewModel { get { return _lazyVm.Value; } }

        private void TagVisualization_Moved(object sender, TagVisualizerEventArgs e)
        {
            var searchTagView = (SearchTagView)e.TagVisualization;
            var tagId = (int)searchTagView.VisualizedTag.Value;

            var tagAngle = (int) e.TagVisualization.TrackedTouch.GetOrientation(this);
            //var position = e.TagVisualization.TrackedTouch.GetPosition(this);
            var screenPosition = searchTagView.PointToScreen(new Point(0d, 0d));


            var tag = ViewModel.Tags[tagId].Tag;
            tag.Angle = tagAngle;
            tag.PositionX = (double)screenPosition.X;
            tag.PositionY = (double)screenPosition.Y;


            //Debug.WriteLine("tag.PositionY : " + tag.PositionY);
            Debug.WriteLine("screenPosition.Y : " + screenPosition.Y);
            //Debug.WriteLine("screenPosition.X : " + screenPosition.X);
        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //_viewModel.Id = e.TagVisualization.VisualizedTag.Value;
            //Tag = e.TagVisualization.VisualizedTag;
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // Called when finger touch and tangible click on main pie menu item. 
            // Not when mouse click

            var t = (TouchEventArgs)e;

            Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown");
            Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());

            if (!t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsFingerRecognized())
            {   //!! Funktioniert! Taginput wird abgefangen
                //MessageBox.Show("SV: PieMenuItem_Click if");
                Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Handled!");
                t.Handled = true;
            }
            else
            {
                Debug.WriteLine("STV: MyTagVisualization_PreviewTouchDown Finger!");
                //var searchTagView = (SearchTagView)e.TagVisualization;
            }
        }

        
    }
}
