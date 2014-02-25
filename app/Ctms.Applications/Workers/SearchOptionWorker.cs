using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicSearch.Managers;
using Ctms.Applications.ViewModels;
using MusicSearch.SearchObjects;
using System.Waf.Applications.Services;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchOptionWorker
    {
        private SearchViewModel _searchViewModel;
        private SearchTagViewModel _searchTagViewModel;
        private SelectionManager _selectionManager;
        private IMessageService _messageService;

        [ImportingConstructor]
        public SearchOptionWorker(SearchViewModel searchViewModel, SearchTagViewModel searchTagVm, IMessageService messageService)
        {
            //ViewModels
            _searchViewModel = searchViewModel;
            _searchTagViewModel = searchTagVm;
            //Services
            _messageService = messageService;
            //Workers
            //_streamingWorker = resultWorker;
            //Managers
            _selectionManager = new SelectionManager();
        }

        public bool CanSelectOption() { return _searchViewModel.IsValid; }

        public void SelectOption(int id)
        {
            //_searchTagViewModel.Item1Header = "Funzt! (Selection Option)";
            MessageServiceExtensions.ShowMessage(_messageService, "Selected: " + id);

            // check selection
            // load next options / set keyword 
            
        }


        //public void SelectOption(KeywordType.Types selectOption)
        public void SelectOption(string selectOption)
        {

            //_selectionManager.SetSelection(selectOption);
        }

        public void LoadNextOptions()
        {

        }
    }
}
