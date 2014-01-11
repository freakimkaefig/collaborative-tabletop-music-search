using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.ResponseObjects;
using Ctms.Applications.DataFactories;
using Ctms.Domain.Objects;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;

namespace Ctms.Applications.Workers
{
    public class ResultWorker
    {
        private ResultViewModel _resultViewModel;

        public ResultWorker(ResultViewModel resultViewModel)
        {
            _resultViewModel = resultViewModel;
        }

        public bool CanRefreshResults() { return _resultViewModel.IsValid; }

        public void RefreshResults(ResponseContainer responseContainer)
        {
            //Example of how to read a resulting song and assign it to viewmodel
            SongFactory factory = new SongFactory();
            var result = new Result();
            Random rnd = new Random();
            int index = rnd.Next(0, responseContainer.response.songs.Count);
            result.Song = factory.Create(responseContainer.response.songs[index]);
            _resultViewModel.Result = result;
        }
    }
}
