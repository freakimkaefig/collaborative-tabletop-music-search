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

        private short windowHeight;

        public SearchTagView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchTagViewModel>(() => ViewHelper.GetViewModel<SearchTagViewModel>(this));

            count++;

            windowHeight = (short)Application.Current.MainWindow.ActualHeight;
        }

        public SearchViewModel ViewModel { get; set; }

        // Provides this view's viewmodel
        private SearchTagViewModel _viewModel { get { return _lazyVm.Value; } }

        private void TagVisualization_Moved(object sender, TagVisualizerEventArgs e)
        {
            var searchTagView   = (SearchTagView)e.TagVisualization;
            var tagId           = (int)searchTagView.VisualizedTag.Value;

            var screenPosition = searchTagView.PointToScreen(new Point(0d, 0d));

            // set angle and position of this tag
            var tag         = ViewModel.Tags[tagId];
            tag.Tag.Angle       = (short) e.TagVisualization.TrackedTouch.GetOrientation(this);
            tag.Tag.PositionX   = (short) screenPosition.X;
            tag.Tag.PositionY   = (short) screenPosition.Y;

            // orientate tag to the nearest side of the two long sides
            tag.Tag.Orientation = tag.Tag.PositionY > windowHeight / 2 ? (short) 180 : (short) 0;

            tag.UpdateVisibleOptions();
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
