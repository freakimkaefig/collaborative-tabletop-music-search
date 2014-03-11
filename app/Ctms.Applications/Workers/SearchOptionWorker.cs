using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicSearch.Managers;
using Ctms.Applications.ViewModels;
using MusicSearch.SearchObjects;
using System.Waf.Applications.Services;
using Ctms.Applications.Data;
using Ctms.Domain.Objects;
using Helpers;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchOptionWorker
    {
        private SearchViewModel _searchVM;
        private SearchTagViewModel _searchTagVM;
        private SelectionManager _selectionManager;
        private IMessageService _messageService;
        private Repository _repository;

        [ImportingConstructor]
        public SearchOptionWorker(SearchViewModel searchVM, SearchTagViewModel searchTagVm, IMessageService messageService,
            Repository repository)
        {
            //ViewModels
            _searchVM = searchVM;
            _searchTagVM = searchTagVm;
            //Services
            _messageService = messageService;
            //Data
            _repository = repository;
            //Workers
            //_streamingWorker = resultWorker;
            //Managers
            _selectionManager = new SelectionManager();
        }

        public bool CanSelectOption() { return _searchVM.IsValid; }


        public void UpdateOptions(TagOption selectedTagOption = null)
        {
            var styles = _repository.GetAllStyles();

            var tagOptions = new List<TagOption>();
            foreach (var style in styles)
            {
                tagOptions.Add(new SingleTextTagOption() { Id = 0, Keyword = style, Text = style.Name });
            }
            _searchVM.TagOptions = CollectionHelper.ToObservableCollection<TagOption>(tagOptions);
        }

        public void SelectOption(TagOption selectedOption)
        {
            //_searchTagViewModel.Item1Header = "Funzt! (Selection Option)";
            MessageServiceExtensions.ShowMessage(_messageService, "Selected: " + selectedOption.Keyword);

            // check selection
            // ...

            // load next options / set keyword 
            CalcNextAction(selectedOption);
        }

        public void CalcNextAction(TagOption selectedOption)
        {
            //if searchtype / subgenre etc.. selected go on with next options
            //UpdateOptions(selectedOption);

            //if searchtype is keyword and there's no substructure set option 
            //as selected keyword for this tangible
        }
    }
}
