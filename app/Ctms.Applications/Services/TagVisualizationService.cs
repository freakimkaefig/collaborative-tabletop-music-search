using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;

namespace Ctms.Applications.Services
{
    [Export(typeof(TagVisualizationService)), Export]
    class TagVisualizationService
    {
        private SearchTagViewModel _searchTagVm;

        public TagVisualizationService(SearchTagViewModel searchTagVm)
        {
            _searchTagVm = searchTagVm;
        }

        public void InitTangibleDefinitions()
        {
            for (int i = 0; i < 12; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("../../Views/SearchTagView.xaml", UriKind.Relative);
                tagDefinition.MaxCount = 1;
                tagDefinition.LostTagTimeout = 2000.0;
                //tagDefinition.OrientationOffsetFromTag = 0;
                //tagDefinition.OrientationOffsetFromTag = 45;
                //tagDefinition.PhysicalCenterOffsetFromTag = new Vector(0.3, -0.4);
                tagDefinition.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagDefinition.UsesTagOrientation = true;
                //shellVm.
                //MyTagVisualizer.Definitions.Add(tagDefinition);
            }

        }

    }
}
