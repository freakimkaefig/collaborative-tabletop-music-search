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

        // Provides this view's viewmodel
        private SearchTagViewModel _viewModel { get { return _lazyVm.Value; } }

        private void TagVisualization_Moved(object sender, TagVisualizerEventArgs e)
        {
            var trorientation = e.TagVisualization.TrackedTouch.GetOrientation(this);
            //var or = e.TagVisualization.

            var tag = (SearchTagView)e.TagVisualization;
            var orientation = tag.Orientation;
            var oriOff = tag.OrientationOffsetFromTag;

            var tagOr = e.TagVisualization.Orientation;
            var transform = tag.TagGrid.RenderTransform;
            var myTag = tag.Tag;

            Debug.WriteLine("Orientation : " + orientation);
            Debug.WriteLine("OrientationOffset : " + oriOff);
            Debug.WriteLine("RenderTransform : " + transform);
            Debug.WriteLine("trorientation : " + trorientation);

            var myTag2 = e.TagVisualization.Orientation;
            var myTag3 = e.TagVisualization.Tag;

            Debug.WriteLine("Orientation : " + myTag3);
            
            /*
            var tag = (SearchTagView)e.TagVisualization;
            Double orientation = tag.MyTagVisualization.GetOrientation(this);
            RotateTransform render = newRotateTransform(orientation);
            render.CenterX = x;
            render.CenterY = x;
            pointer.RenderTransform = render;

            int angle = (int)orientation;

            if (angle > a && angle < b)
            {

                //trigger method

            }

            else
            {

                //trigger method

            }*/
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
