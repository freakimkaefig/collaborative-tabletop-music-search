﻿using System;
using System.ComponentModel.Composition;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using Ctms.Applications.DataModels;
using Ctms.Applications.ViewModels;
using Ctms.Applications.Views;
using Helpers;
using Microsoft.Surface.Presentation.Controls;
using Ctms.Applications.Common;
using Microsoft.Surface.Presentation.Input;

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


        public SearchTagView()
        {
            InitializeComponent();
            _lazyVm = new Lazy<SearchTagViewModel>(() => ViewHelper.GetViewModel<SearchTagViewModel>(this));

            count++;

            CommonVal.WindowHeight = (short)Application.Current.MainWindow.ActualHeight;
        }

        public SearchViewModel SearchVm { get; set; }

        // Provides this view's viewmodel
        private SearchTagViewModel _searchTagVm { get { return _lazyVm.Value; } }

        private void TagVisualization_Moved(object sender, TagVisualizerEventArgs e)
        {
            var searchTagView   = (SearchTagView) e.TagVisualization;
            var tagId           = (int) searchTagView.VisualizedTag.Value;

            var screenPosition = searchTagView.Center;

            // set angle and position of this tag
            var tag         = SearchVm.Tags[tagId];

            var trackedTouch = e.TagVisualization.TrackedTouch;
            if (trackedTouch != null)
            {
                tag.Tag.Angle = (short)trackedTouch.GetOrientation(this);
            }

            tag.Tag.PositionX   = (short) (screenPosition.X);
            tag.Tag.PositionY   = (short) (screenPosition.Y);
        }

        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
            //_viewModel.Id = e.TagVisualization.VisualizedTag.Value;
            //Tag = e.TagVisualization.VisualizedTag;
            //Log("STV: SimpleVisualization_Loaded");
        }

        private void MyTagVisualization_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // Called when finger touch and tangible click on main pie menu item. 
            // Not when mouse click
            var t = (TouchEventArgs)e;
            /*
            Log("STV: MyTagVisualization_PreviewTouchDown");
            Log("STV: MyTagVisualization_PreviewTouchDown Finger" + t.TouchDevice.GetIsFingerRecognized());
            Log("STV: MyTagVisualization_PreviewTouchDown Tag" + t.TouchDevice.GetIsTagRecognized());
            */
            if (!t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {   //!! Funktioniert! Taginput wird abgefangen
                //Log("STV: No finger, no Tag");
                t.Handled = true;
            }
            else if (t.TouchDevice.GetIsFingerRecognized() && !t.TouchDevice.GetIsTagRecognized())
            {
                //Log("STV: Finger, no Tag");
                //t.Handled = true;
            }
            else if (!t.TouchDevice.GetIsFingerRecognized() && t.TouchDevice.GetIsTagRecognized())
            {
                //Log("STV: No Finger, but Tag");
                //t.Handled = true;
                //t.TouchDevice.
                //var searchTagView = (SearchTagView)e.TagVisualization;
            }
            else
            {
                //Log("STV: No Finger, no Tag");
            }
        }

        private void Log(string message)
        {
            //DevLogger.Log(message);
        }

        private void MyTagVisualization_TouchDown(object sender, TouchEventArgs e)
        {
            /*
            if (!e.TouchDevice.GetIsFingerRecognized() && !e.TouchDevice.GetIsTagRecognized())
            {
                e.Handled = true;
            }*/
        }

        private void MyTagVisualization_TouchUp(object sender, TouchEventArgs e)
        {

        }

        private void InputField_GotFocus(object sender, RoutedEventArgs e)
        {
            //KeyboardHelper.ShowKeyboard();
        }
        
    }
}
