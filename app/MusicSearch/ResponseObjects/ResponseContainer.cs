using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.ResponseObjects
{
    /// <summary>
    /// This class is used for 
    /// </summary>
    public class ResponseContainer
    {
        public ResponseObj Response { get; set; }

        public class ResponseObj
        {
            public StatusObj Status { get; set; }
            public IList<Song> Songs { get; set; }

            public class StatusObj
            {
                public string Version { get; set; }
                public int Code { get; set; }
                public string Message { get; set; }
            }

            public class Song
            {
                public string Artist_Id { get; set; }
                public string Id { get; set; }
                public string Artist_Name { get; set; }
                public string Title { get; set; }
            }
        }
    }
}
