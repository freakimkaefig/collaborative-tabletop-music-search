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
using Microsoft.Surface;
using System.Diagnostics;
using System.Waf.Applications;
using Ctms.Applications.ViewModels;

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export(typeof(IShellView))]
    public partial class ShellWindow : SurfaceWindow, IShellView
    {
        private readonly Lazy<ShellViewModel> viewModel;

        public ShellWindow()
        {
            InitializeComponent();
            viewModel = new Lazy<ShellViewModel>(() => ViewHelper.GetViewModel<ShellViewModel>(this));
            AddWindowAvailabilityHandlers();
            //InitTangibleDefinitions();
        }

        public bool IsMaximized
        {
            get { return WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    WindowState = WindowState.Maximized;
                }
                else if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
            }
        }
        /*
        public void InitTangibleDefinitions()
        {
            for (int i = 0; i < 12; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("../../Views/SearchTagView.xaml", UriKind.Relative);
                tagDefinition.MaxCount = 1;
                tagDefinition.LostTagTimeout = 5000.0;
                //tagDefinition.OrientationOffsetFromTag = 0;
                //tagDefinition.OrientationOffsetFromTag = 45;
                //tagDefinition.PhysicalCenterOffsetFromTag = new Vector(0.3, -0.4);
                tagDefinition.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagDefinition.UsesTagOrientation = false;
                //MyTagVisualizer.Definitions.Add(tagDefinition);
                AddTagVisualization(tagDefinition);
            }
        }

        public void AddTagVisualization(TagVisualizationDefinition tagDefinition)
        {
            MyTagVisualizer.Definitions.Add(tagDefinition);
        }

        public TagVisualizer TagVisualizer { get { return MyTagVisualizer; } set {} }
        */
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        private void OnWindowInteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowNoninteractive(object sender, EventArgs e)
        {

        }

        private void OnWindowUnavailable(object sender, EventArgs e)
        {

        }


        private void OnVisualizationAdded(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
        }

        private void OnVisualizationRemoved(object sender, TagVisualizerEventArgs e)
        {
            //HandleTagAction(e);
        }
    }
}
