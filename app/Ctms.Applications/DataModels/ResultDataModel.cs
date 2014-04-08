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

        public ResultDataModel(string title, string artistName, Track spotifyTrack)
        {
            _result = new Result();
            Result.Song = new Song();
            Result.Song.Title = title;
            Result.Song.ArtistName = artistName;
            SpotifyTrack = spotifyTrack;

            Opacity = 1.0;
        }

        public Result Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public Track SpotifyTrack { get; set; }
        public string Duration
        {
            get
            {
                return TimeSpan.FromMilliseconds(SpotifyTrack.Duration()).Minutes + ":" + TimeSpan.FromMilliseconds(SpotifyTrack.Duration()).Seconds;
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set { _isLoading = value; }
        }

        public object DraggedElement { get; set; }
        public double Opacity { get; set; }
        public bool CanDrop
        {
            get { return _canDrop; }
            set
            {
                if (_canDrop != value)
                {
                    _canDrop = value;
                }
            }
        }
        public object ClickedElement { get; set; }
        public object ActiveElement { get; set; }
    }
}
