using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicSearch.Managers;
using Ctms.Applications.ViewModels;
using MusicSearch.SearchObjects;

namespace Ctms.Applications.Workers
{
    [Export]
    public class SearchOptionWorker
    {
        private SearchViewModel _searchViewModel;
        private SelectionManager _selectionManager;

        [ImportingConstructor]
        public SearchOptionWorker(SearchViewModel searchViewModel)
        {
            //ViewModels
            _searchViewModel = searchViewModel;
            //Workers
            //_resultWorker = resultWorker;
            //Managers
            _selectionManager = new SelectionManager();
        }

        public bool CanSelectOption() { return _searchViewModel.IsValid; }

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
