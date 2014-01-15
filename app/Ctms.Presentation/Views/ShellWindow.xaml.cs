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

namespace Ctms.Presentation.Views
{
    /// <summary>
    /// Interaktionslogik für UserControl1.xaml
    /// </summary>
    [Export(typeof(IShellView))]
    public partial class ShellWindow : Window, IShellView
    {
        public ShellWindow()
        {
            InitializeComponent();
            InitTangibleDefinitions();
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


        private TagVisualizer _tagVisualizer;

        public void InitTangibleDefinitions()
        {

            for (int i = 0; i < 12; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("../../Views/TagVisualizationControl.xaml", UriKind.Relative);
                tagDefinition.MaxCount = 1;
                tagDefinition.LostTagTimeout = 2000.0;
                tagDefinition.OrientationOffsetFromTag = 0;
                tagDefinition.PhysicalCenterOffsetFromTag = new Vector(0, 0);
                tagDefinition.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagDefinition.UsesTagOrientation = true;
                //_tagVisualizer = new TagVisualizer();
                TagVisualizer.Definitions.Add(tagDefinition);
            }

        }
    }
}
