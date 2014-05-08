using System;
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
using Ctms.Applications.Common;

namespace Ctms.Applications.Workers
{
    [Export]
    public class ResultWorker
    {
        private ResultViewModel _resultViewModel;
        private MenuViewModel _menuViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private InfoWorker _infoWorker;
        private SearchWorker _searchWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        private int _refreshTrialsCounter;
        //private FftWorker _fftWorker;

        [ImportingConstructor]
        public ResultWorker(ResultViewModel resultViewModel, MenuViewModel menuViewModel, MusicStreamAccountWorker accountWorker,
            InfoWorker infoWorker)//, SearchWorker searchWorker)
        {

            _resultViewModel = resultViewModel;
            _menuViewModel = menuViewModel;
            _accountWorker = accountWorker;
            _infoWorker = infoWorker;
            //_fftWorker = fftWorker;
            _accountWorker.ResultSessionManagerCreated = ResultSessionManagerCreated;
            //_fftWorker.VisualizationManagerCreated = VisualizationManagerCreated;
        }

        public void Initialize(SearchWorker searchWorker)
        {
            _searchWorker = searchWorker;
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

        /// <summary>
        /// Check songs and display them as results
        /// </summary>
        /// <param name="songs"></param>
        public void RefreshResults(List<ResponseContainer.ResponseObj.Song> songs)
        {
            if (songs != null)
            {
                if (_menuViewModel.IsLoggedIn)
                {
                    if (_refreshTrialsCounter == 0) RemoveResults();
                    foreach (var song in songs)
                    {                        
                        if (_resultViewModel.Results.Count < CommonVal.Results_MaxNumber)
                        {
                            foreach (var track in song.tracks)
                            {
                                if (_sessionManager.CheckTrackAvailability(track.foreign_id) != null)
                                {
                                    //remove unwanted chars/expressions
                                    String name = StringHelper.cleanText(song.Artist_Name);
                                    String title = StringHelper.cleanText(song.Title);

                                    var identicalSong = _resultViewModel.Results.FirstOrDefault(r => r.Result.Song.ArtistName == name && r.Result.Song.Title == title);

                                    if (identicalSong == null)
                                    {
                                        _resultViewModel.Results.Add(new ResultDataModel(title, name, _sessionManager.CheckTrackAvailability(track.foreign_id)));
                                        ResultDataModel result = _resultViewModel.Results.Last();
                                        result.OriginIds = song.originIDs;

                                        result.OriginColors = new ObservableCollection<string>();
                                        result.Result.Song.ArtistId = song.Artist_Id;
                                        result.Result.Response = song;
                                        foreach (var originId in song.originIDs)
                                        {
                                            result.OriginColors.Add(CommonVal.TagColors[originId]);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else // not logged in
                {
                    _menuViewModel.DisplayLoginDialog(true);
                }
            }

            if (songs == null || !songs.Any() || _resultViewModel.Results.Count < 1)
            {   // redo if there are too few results and searching again hasn't been repeated too often
                if (_refreshTrialsCounter < 3)
                {
                    _refreshTrialsCounter++;

                    //search again
                    _searchWorker.StartSearch();
                }
                else
                {
                    _refreshTrialsCounter = 0;

                    _infoWorker.ShowCommonInfo("No results found",
                        "Your search query didn't return any results. \nTry changing your keywords.",
                        "Ok");
                }                
            }
            else if(_resultViewModel.Results.Count >= 1)
	        {   // reset trials counter
                 _refreshTrialsCounter = 0;
	        }
        }

        public void RemoveResults()
        {
            _resultViewModel.Results.Clear();
        }

        public void RefreshDetails(List<ResponseContainer.ResponseObj.ArtistInfo> response, ResultDataModel result)
        {
            if (response != null)
            {
                if (response.Count > 0)
                {
                    var r = response[0];
                    Detail d = new Detail();

                    //About
                    d.Name = r.name;

                    foreach (var biography in r.biographies)
                    {
                        if (biography.text.Length > 500)
                        {
                            d.Biography = StringHelper.cleanText(biography.text);
                            break;
                        }
                    }

                    d.City = r.artist_location[0].location;

                    d.Image = new ArtistImage();
                    d.Image.ImageUrl = r.images[0].url;

                    if (r.terms != null)
                    {
                        d.Genres = new List<String>();
                        for (var i = 0; i < r.terms.Count; i++)
                        {
                            d.Genres.Add(r.terms[i].name);
                            if (i == 3) break;
                        }
                    }

                    //News
                    d.News = new ObservableCollection<ArtistNews>();
                    foreach (var news in r.news)
                    {
                        ArtistNews newsEntry = new ArtistNews();
                        newsEntry.Title = StringHelper.cleanText(news.name);
                        newsEntry.Summary = StringHelper.cleanText(news.summary);
                        newsEntry.Url = news.url;
                        d.News.Add(newsEntry);
                    }

                    //Media
                    d.Images = new ObservableCollection<ArtistImage>();
                    foreach (var images in r.images)
                    {
                        ArtistImage image = new ArtistImage();
                        image.ImageUrl = images.url;
                        d.Images.Add(image);
                    }
                    d.Videos = new ObservableCollection<ArtistVideo>();
                    foreach (var videos in r.video)
                    {
                        ArtistVideo video = new ArtistVideo();
                        video.Title = StringHelper.cleanText(videos.title);
                        video.VideoUrl = videos.url;
                        video.PreviewUrl = videos.image_url;
                        d.Videos.Add(video);
                    }

                    //Reviews
                    d.Reviews = new ObservableCollection<ArtistReview>();
                    foreach (var reviews in r.reviews)
                    {
                        ArtistReview review = new ArtistReview();
                        review.Name = StringHelper.cleanText(reviews.name);
                        review.Release = StringHelper.cleanText(reviews.release);
                        review.Summary = StringHelper.cleanText(reviews.summary);
                        review.Url = reviews.url;
                        d.Reviews.Add(review);
                    }

                    //Songs
                    d.Songs = new ObservableCollection<ArtistSong>();
                    foreach (var songs in r.ArtistSongs)
                    {
                        ArtistSong song = new ArtistSong();
                        song.Title = StringHelper.cleanText(songs.title);
                        song.TrackId = songs.title_id;
                        d.Songs.Add(song);
                    }

                    result.Detail = d;
                    result.IsDetailLoading = false;
                    result.IsDetailLoaded = true;
                }
            }
        }

        public void PrelistenFromDetailView(String spotifyTrackId)
        {
             
        }
    }
}