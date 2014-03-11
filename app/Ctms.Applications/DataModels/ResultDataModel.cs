using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using Ctms.Domain.Objects;

namespace Ctms.Applications.DataModels
{
    public class ResultDataModel : DataModel
    {
        private Result _result;

        public ResultDataModel(string spotifyId, string title, string artistName)
        {
            _result = new Result();
            Result.Song = new Song();
            Result.Song.SpotifyId = spotifyId;
            Result.Song.Title = title;
            Result.Song.ArtistName = artistName;
        }

        public Result Result
        {
            get { return _result; }
            set { _result = value; }
        }

        public object DraggedElement { get; set; }
    }
}
