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
using Ctms.Domain.Objects;

namespace Ctms.Applications.Services
{
    public class SearchTagVisualizationService
    {
        private SearchViewModel _searchVm;
        //private SearchTagViewModel _searchTagVm;
        private TagVisualizer _tagVisualizer;
        private TagVisualizationDefinitionCollection _tagVisualizers;

        public SearchTagVisualizationService(SearchViewModel searchVm)//, SearchTagViewModel searchTagVm)
        {
            _searchVm = searchVm;
            //_searchTagVm = searchTagVm;
            _tagVisualizer = ((ISearchView)_searchVm.View).TagVisualizer;
        }

        public TagVisualizationDefinitionCollection TagVisualizers { get { return _tagVisualizers; } }

        public void InitTagDefinitions()
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
            _tagVisualizers = _tagVisualizer.Definitions;
        }

        private void AddTagVisualization(TagVisualizationDefinition tagDefinition)
        {
            _tagVisualizer.Definitions.Add(tagDefinition);


            //var src = tagDefinition.Source.Fragment;
            /*
            // create tag object for this tag definition
            var tag = new Tag()
            {
                Id = (int) tagDefinition.Value,
                SelectionOptions = new List<string>()//!!
            };
            //!!
            tag.SelectionOptions.Add("SelectionOpt1");
            tag.SelectionOptions.Add("SelectionOpt2");
            */
            //_searchTagVm.Tags.Add(tag);
        }

    }
}
