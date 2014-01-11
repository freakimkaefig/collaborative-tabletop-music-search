using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using MusicSearch.Managers;
using System.ComponentModel;
using MusicSearch.SearchObjects;

namespace Ctms.Applications.Managers
{
    public class SearchWorker
    {
        private BackgroundWorkHelper mBackgroundWorker;
        private SearchManager mSearchManager;
        private SelectionManager mSelectionManager;

        public SearchWorker()
        {

        }

        public void SelectOption(KeywordType.Types selectOption)
        {
            mSelectionManager.SetSelection(selectOption);
        }

        public void SetSelectionOptions()
        {

        }

        public void StartSearch()
        {
            var searchManager = new SearchManager();
            //RefreshResults(searchManager.ResponseContainer);

            mBackgroundWorker = new BackgroundWorkHelper();
            //Tell which method to execute in background in what to do after completion
            mBackgroundWorker.DoInBackground(searchManager.Start, SearchCompleted);
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        public void SearchCompleted()
        {
        
        }
    }
}
