using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;

namespace Ctms.Applications.Services
{
    class TagVisualizationService
    {

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
                _tagVisualizer = new TagVisualizer();
                _tagVisualizer.Definitions.Add(tagDefinition);
            }

        }

    }
}
