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
using System.Collections.ObjectModel;
using Helpers;
using Ctms.Applications.DevHelper;

namespace Ctms.Applications.Workers
{
    [Export]
    public class ResultWorker
    {
        private ResultViewModel _resultViewModel;
        private MenuViewModel _menuViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private InfoWorker _infoWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        //private FftWorker _fftWorker;

        [ImportingConstructor]
        public ResultWorker(ResultViewModel resultViewModel, MenuViewModel menuViewModel, MusicStreamAccountWorker accountWorker,
            InfoWorker infoWorker)
        {

            _resultViewModel = resultViewModel;
            _menuViewModel = menuViewModel;
            _accountWorker = accountWorker;
            _infoWorker = infoWorker;
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

        public void RefreshResults(List<ResponseContainer.ResponseObj.Song> songs)
        {
            if (songs != null)
            {
                if (_menuViewModel.IsLoggedIn)
                {
                    _resultViewModel.Results.Clear();
                    foreach (var song in songs)
                    {
                        //DevLogger.Log("ResultWorker:63 - " + response[i].ToString());
                        if (_resultViewModel.Results.Count < 20)
                        {
                            foreach (var track in song.tracks)
                            {
                                if (_sessionManager.CheckTrackAvailability(track.foreign_id) != null)
                                {
                                    //remove unwanted chars/expressions
                                    String name = StringHelper.cleanText(song.Artist_Name);
                                    String title = StringHelper.cleanText(song.Title);

                                    _resultViewModel.Results.Add(new ResultDataModel(title, name, _sessionManager.CheckTrackAvailability(track.foreign_id)));
                                    ResultDataModel result = _resultViewModel.Results.Last();
                                    result.OriginIds = song.originIDs;
                                    result.Result.Song.ArtistId = song.Artist_Id;
                                    result.Result.Response = song;
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
                        //!!ToDo
                    }
                }
                else
                {
                    _menuViewModel.DisplayLoginDialog(true);
                    //_infoWorker
                }
            }
        }

        public void RefreshDetails(List<ResponseContainer.ResponseObj.ArtistInfo> response, ResultDataModel result)
        {
            var r = response[0];
            Detail d = new Detail();

            //About
            d.Name = r.name;

            for (var i = 0; i < r.biographies.Count; i++)
            {
                if (r.biographies[i].text.Length > 500)
                {
                    d.Biography = StringHelper.cleanText(r.biographies[i].text);
                    break;
                }
            }
            
            d.City = r.artist_location[0].location;

            d.Image = new ArtistImage();
            d.Image.ImageUrl = r.images[0].url;

            d.Genres = new List<String>();
            for (var i = 0; i < r.terms.Count; i++)
            {
                d.Genres.Add(r.terms[i].name);
                if (i == 3) break;
            }

            //News
            d.News = new ObservableCollection<ArtistNews>();
            for (var i = 0; i < r.news.Count; i++)
            {
                ArtistNews news = new ArtistNews();
                news.Title = StringHelper.cleanText(r.news[i].name);
                news.Summary = StringHelper.cleanText(r.news[i].summary);
                news.Url = r.news[i].url;
                d.News.Add(news);
            }

            //Media
            d.Images = new ObservableCollection<ArtistImage>();
            for (var i = 0; i < r.images.Count; i++)
            {
                ArtistImage image = new ArtistImage();
                image.ImageUrl = r.images[i].url;
                d.Images.Add(image);
            }
            d.Videos = new ObservableCollection<ArtistVideo>();
            for (var i = 0; i < r.video.Count; i++)
            {
                ArtistVideo video = new ArtistVideo();
                video.Title = StringHelper.cleanText(r.video[i].title);
                video.VideoUrl = r.video[i].url;
                video.PreviewUrl = r.video[i].image_url;
                d.Videos.Add(video);
            }

            //Reviews
            d.Reviews = new ObservableCollection<ArtistReview>();
            for (var i = 0; i < r.reviews.Count; i++)
            {
                ArtistReview review = new ArtistReview();
                review.Name = StringHelper.cleanText(r.reviews[i].name);
                review.Release = StringHelper.cleanText(r.reviews[i].release);
                review.Summary = StringHelper.cleanText(r.reviews[i].summary);
                review.Url = r.reviews[i].url;
                d.Reviews.Add(review);
            }

            //Songs
            d.Songs = new ObservableCollection<ArtistSong>();
            for (var i = 0; i < r.ArtistSongs.Count; i++)
            {
                ArtistSong song = new ArtistSong();
                song.Title = StringHelper.cleanText(r.ArtistSongs[i].title);
                song.TrackId = r.ArtistSongs[i].title_id;
                d.Songs.Add(song);
            }

            result.Detail = d;
            result.IsDetailLoading = false;
            result.IsDetailLoaded = true;
        }
    }
}