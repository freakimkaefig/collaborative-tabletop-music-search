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

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //_viewModel.Id = e.TagVisualization.VisualizedTag.Value;
            //Tag = e.TagVisualization.VisualizedTag;
            //PieMenuItem1.SubHeader = "Header1";
            //HomeButton.Content = "Dig";
        }

        private void PieMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //When mouse click, not when finger touch. 

            MessageBox.Show("STV: PieMenuItem_Click");
            /*var t = (TouchEventArgs)e;

            if (!t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {
                MessageBox.Show("SV: PieMenuItem_Click if");

                // t.Handled = true;
            }*/
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // Called when finger touch and tangible click on main pie menu item. 
            // Not when mouse click
            //MessageBox.Show("STV: MyTagVisualization_PreviewTouchDown");
        }

        private void PieMenuItem_ItemPreviewTouchDown(object sender, TouchEventArgs e)
        {

        }
    }
}
