﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.Objects;
using Ctms.Applications.DataFactories;
using Ctms.Domain.Objects;
using Ctms.Applications.ViewModels;
using System.ComponentModel.Composition;
using Ctms.Applications.DataModels;
using SpotifySharp;
using MusicStream;

namespace Ctms.Applications.Workers
{
    [Export]
    public class ResultWorker
    {
        private ResultViewModel _resultViewModel;
        private MenuViewModel _menuViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        //private FftWorker _fftWorker;

        [ImportingConstructor]
        public ResultWorker(ResultViewModel resultViewModel, MenuViewModel menuViewModel, MusicStreamAccountWorker accountWorker)
        {

            _resultViewModel = resultViewModel;
            _menuViewModel = menuViewModel;
            _accountWorker = accountWorker;
            //_fftWorker = fftWorker;
            _accountWorker.ResultSessionManagerCreated = ResultSessionManagerCreated;
            //_fftWorker.VisualizationManagerCreated = VisualizationManagerCreated;
        }

        private void ResultSessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        private void VisualizationManagerCreated(MusicStreamVisualizationManager visualizationManager)
        {
            _visualizationManager = visualizationManager;
            _sessionManager.VisualizationManager = _visualizationManager;
        }

        public bool CanRefreshResults() { return _resultViewModel.IsValid; }

        public void RefreshResults(List<ResponseContainer.ResponseObj.Song> response)
        {
            if (response != null)
            {
                if (_menuViewModel.IsLoggedIn)
                {
                    _resultViewModel.Results.Clear();
                    for (int i = 0; i < response.Count; i++)
                    {
                        if (_resultViewModel.Results.Count < 10)
                        {
                            for (int j = 0; j < response[i].tracks.Count; j++)
                            {
                                if (_sessionManager.CheckTrackAvailability(response[i].tracks[j].foreign_id) != null)
                                {
                                    _resultViewModel.Results.Add(new ResultDataModel(response[i].Title, response[i].Artist_Name, _sessionManager.CheckTrackAvailability(response[i].tracks[j].foreign_id)));
                                    ResultDataModel result = _resultViewModel.Results.Last();
                                    result.OriginIds = response[i].originIDs;
                                    result.Result.Song.ArtistId = response[i].Artist_Id;
                                    result.Result.Response = response[i];

                                    j = response[i].tracks.Count;
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                    if (_resultViewModel.Results.Count < 5)
                    {
                        //search again
                    }
                }
                else
                {
                    _menuViewModel.DisplayLoginDialog(true);
                }
            }
        }

        public void RefreshDetails(List<ResponseContainer.ResponseObj.ArtistInfo> response, ResultDataModel result)
        {
            String city = response[0].artist_location.ToString();
            String biography = response[0].biographies[0].ToString();
            result.Detail = new Detail(city, biography);
        }
    }
}