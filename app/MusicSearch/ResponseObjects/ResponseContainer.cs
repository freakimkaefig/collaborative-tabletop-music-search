using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.ResponseObjects
{
    public class ResponseContainer
    {
        public Response response { get; set; }

        public class Response
        {
            public Status status { get; set; }
            public IList<Song> songs { get; set; }

            public class Status
            {
                public string version { get; set; }
                public int code { get; set; }
                public string message { get; set; }
            }

            public class Song
            {
                public string artist_id { get; set; }
                public string id { get; set; }
                public string artist_name { get; set; }
                public string title { get; set; }
            }
        }
    }
}
