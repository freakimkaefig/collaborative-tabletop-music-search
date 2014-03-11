using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.ResponseObjects;
using Ctms.Applications.DataFactories;
using Ctms.Domain.Objects;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.Workers
{
    [Export]
    public class ResultWorker
    {
        private ResultViewModel _resultViewModel;

        [ImportingConstructor]
        public ResultWorker(ResultViewModel resultViewModel)
        {
            _resultViewModel = resultViewModel;
        }

        public bool CanRefreshResults() { return _resultViewModel.IsValid; }

        public void RefreshResults(List<ResponseContainer.ResponseObj.Song> response)
        {
            for (int i = 0; i < response.Count; i++)
            {
                //string spotifyTrackId = response[i].tracks[0].spotifyId;  //DAVE
                //if spotifyTrackId == playable return else j++; //LUKAS
                _resultViewModel.Results.Add(new ResultDataModel("spotify:track:4lCv7b86sLynZbXhfScfm2", response[i].Title, response[i].Artist_Name));
            }
        }
    }
}