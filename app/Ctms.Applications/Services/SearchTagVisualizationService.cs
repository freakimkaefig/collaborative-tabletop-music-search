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
using Ctms.Applications.DataFactories;
using Ctms.Applications.Data;

namespace Ctms.Applications.Services
{
    public class SearchTagVisualizationService
    {
        private SearchViewModel _searchVm;
        private Repository _repository;
        //private SearchTagViewModel _searchTagVm;
        private TagVisualizer _tagVisualizer;
        private TagVisualizationDefinitionCollection _tagVisualizers;

        public SearchTagVisualizationService(SearchViewModel searchVm, Repository repository)//, SearchTagViewModel searchTagVm)
        {
            _searchVm = searchVm;
            _repository = repository;
            //_searchTagVm = searchTagVm;
            _tagVisualizer = ((ISearchView)_searchVm.View).TagVisualizer;
        }

        public TagVisualizationDefinitionCollection TagVisualizers { get { return _tagVisualizers; } }

        public void InitTagDefinitions()
        {
            for (int i = 0; i < CommonVal.MaxTagNumber; i++)
            {
                var tagVisDef = new TagVisualizationDefinition();
                tagVisDef.Value = i;
                tagVisDef.Source = new Uri("../../Views/SearchTagView.xaml", UriKind.Relative);
                tagVisDef.MaxCount = 1;
                tagVisDef.LostTagTimeout = 5000.0;
                tagVisDef.TagRemovedBehavior = TagRemovedBehavior.Fade;
                tagVisDef.UsesTagOrientation = false;

                AddTagVisualization(tagVisDef, i);
            }
            _tagVisualizers = _tagVisualizer.Definitions;
        }

        private void AddTagVisualization(TagVisualizationDefinition tagVisDef, int id)
        {
            var factory = new TagFactory(_repository);

            var tagDm = factory.CreateTagDataModel(tagVisDef, id);

            _repository.AddTagDMs(tagDm);

            _tagVisualizer.Definitions.Add(tagVisDef);            
        }

    }
}
