using System.Windows.Controls;
using System.Windows;
using System.ComponentModel.Composition;
using Ctms.Applications.Views;
using Microsoft.Surface.Presentation.Controls;
using System;

namespace Ctms.Presentation.Views
{
    //[Export(typeof(ITagVisualizationView))]
    //public partial class SimpleVisualization : UserControl//, ITagVisualizationView
    public partial class SimpleVisualization //: UserControl//, ITagVisualizationView
    {
        public SimpleVisualization()
        {
            InitializeComponent();

            InitTangibleDefinitions();
            
        }
        /*
        public void SimpleVisualization_Loaded(object sender, RoutedEventArgs e)
        {
        }
         * */

        private TagVisualizer _tagVisualizer;

        public void InitTangibleDefinitions()
        {

            for (int i = 0; i < 12; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("Views/TagVisualization.xaml", UriKind.Relative);
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
