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

        public ResultDataModel(string title, string artistName, Track spotifyTrack)
        {
            _result = new Result();
            Result.Song = new Song();
            Result.Song.Title = title;
            Result.Song.ArtistName = artistName;
            SpotifyTrack = spotifyTrack;
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

        public object DraggedElement { get; set; }
        public object ClickedElement { get; set; }
        public object ActiveElement { get; set; }
    }
}
