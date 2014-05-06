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
                        DevLogger.Log("Received Track: " + response[i].Artist_Name + " | " + response[i].Title + " | Origin:" + response[i].originIDs + "\n");
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
            var r = response[0];
            Detail d = new Detail();

            //About
            d.Name = r.name;

            for (var i = 0; i < r.biographies.Count; i++)
            {
                if (r.biographies[i].text.Length > 500)
                {
                    d.Biography = cleanText(r.biographies[i].text);
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
                news.Title = cleanText(r.news[i].name);
                news.Summary = cleanText(r.news[i].summary);
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
                video.Title = cleanText(r.video[i].title);
                video.VideoUrl = r.video[i].url;
                video.PreviewUrl = r.video[i].image_url;
                d.Videos.Add(video);
            }

            //Reviews
            d.Reviews = new ObservableCollection<ArtistReview>();
            for (var i = 0; i < r.reviews.Count; i++)
            {
                ArtistReview review = new ArtistReview();
                review.Name = cleanText(r.reviews[i].name);
                review.Release = cleanText(r.reviews[i].release);
                review.Summary = cleanText(r.reviews[i].summary);
                review.Url = r.reviews[i].url;
                d.Reviews.Add(review);
            }

            //Songs
            d.Songs = new ObservableCollection<String>();
            for (var i = 0; i < r.ArtistSongs.Count; i++)
            {
                d.Songs.Add(cleanText(r.ArtistSongs[i].title));
            }

            result.Detail = d;
            result.IsDetailLoading = false;
            result.IsDetailLoaded = true;
        }

        private String cleanText(String text)
        {
            String temp = text;
            temp = StringHelper.replacePartialString(temp, "&#34;", "\"", 10000);
            temp = StringHelper.replacePartialString(temp, "&quot;", "\"", 10000);
            temp = StringHelper.replacePartialString(temp, "&#38;", "&", 10000);
            temp = StringHelper.replacePartialString(temp, "&#39;", "'", 10000);
            temp = StringHelper.replacePartialString(temp, "\t", "", 100000);
            temp = StringHelper.replacePartialString(temp, "\n", "", 10000);
            return temp;
        }
    }
}