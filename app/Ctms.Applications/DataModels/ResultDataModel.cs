using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Ctms.Domain.Objects;
using SpotifySharp;

namespace Ctms.Applications.DataModels
{
    public class ResultDataModel : DataModel
    {
        private Result _result;
        private bool _canDrop = true;
        private bool _isLoading = false;
        private bool _isPlaying = false;

        private double _stdWidth;
        private double _stdHeight;
        private bool _isDetail;

        public ResultDataModel(string title, string artistName, Track spotifyTrack)
        {
            _result = new Result();
            Result.Song = new Song();
            Result.Song.Title = title;
            Result.Song.ArtistName = artistName;
            SpotifyTrack = spotifyTrack;

            Opacity = 1.0;
        }

        public Result Result { get { return _result; } set { _result = value; } }

        public Track SpotifyTrack { get; set; }
        public string Duration { get { return TimeSpan.FromMilliseconds(SpotifyTrack.Duration()).Minutes + ":" + TimeSpan.FromMilliseconds(SpotifyTrack.Duration()).Seconds; } }

        public bool IsLoading { get { return _isLoading; } set { _isLoading = value; RaisePropertyChanged("IsLoading"); } }
        public bool IsPlaying { get { return _isPlaying; } set { _isPlaying = value; RaisePropertyChanged("IsPlaying"); } }

        public object DraggedElement { get; set; }
        public double Opacity { get; set; }
        public bool CanDrop { get { return _canDrop; } set { if (_canDrop != value) { _canDrop = value; } } }
        public object ClickedElement { get; set; }
        public object ActiveElement { get; set; }

        public double StdWidth { get { return _stdWidth; } set { _stdWidth = value; RaisePropertyChanged("StdWidth"); } }
        public double StdHeight { get { return _stdHeight; } set { _stdHeight = value; RaisePropertyChanged("StdHeight"); } }
        public bool IsDetail { get { return _isDetail; } set { _isDetail = value; RaisePropertyChanged("IsDetail"); } }
    }
}