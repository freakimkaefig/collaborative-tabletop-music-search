using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Surface.Presentation.Controls;
using System.Windows;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using Ctms.Applications.Views;
using Ctms.Applications.Common;

namespace Ctms.Applications.Services
{
    class TagVisualizationService
    {
        private SearchViewModel _searchVm;
        private TagVisualizer _tagVisualizer;

        public TagVisualizationService(SearchViewModel searchVm)
        {
            _searchVm = searchVm;
            _tagVisualizer = ((ISearchView)_searchVm.View).TagVisualizer;
        }

        public void InitTangibleDefinitions()
        {
            for (int i = 0; i < CommonVal.MaxTagNumber; i++)
            {
                TagVisualizationDefinition tagDefinition = new TagVisualizationDefinition();
                tagDefinition.Value = i;
                tagDefinition.Source = new Uri("../../Views/SearchTagView.xaml", UriKind.Relative);
                tagDefinition.MaxCount = 1;
                tagDefinition.LostTagTimeout = 5000.0;
                tagDefinition.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagDefinition.UsesTagOrientation = false;
                AddTagVisualization(tagDefinition);
            }
        }

        private void AddTagVisualization(TagVisualizationDefinition tagDefinition)
        {
            _tagVisualizer.Definitions.Add(tagDefinition);
        }

    }
}
