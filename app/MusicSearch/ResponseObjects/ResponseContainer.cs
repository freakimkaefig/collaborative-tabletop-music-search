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
            public List<Song> Songs { get; set; }
            public List<ArtistSuggestion> ArtistSuggestions { get; set; }
            public List<TitleSuggestion> TitleSuggestions { get; set; }
            public List<ArtistInfo> ArtistInfos { get; set; }
            

            public class StatusObj
            {
                public string Version { get; set; }
                public int Code { get; set; }
                public string Message { get; set; }
            }

            public class ArtistSuggestion
            {
                public String name { get; set; }
                public String id { get; set; }
                public int originId { get; set; }
            }

            public class TitleSuggestion
            {
                public String title { get; set; }
                public String id { get; set; }
                public int originId { get; set; }
                public String artist_name { get; set; }
            }

            public class ArtistInfo
            {
                public List<Object> terms { get; set; }
                public String name { get; set; }
                public List<Object> foreign_ids { get; set; }
                public List<Object> blogs { get; set; }
                public String id { get; set; }
                public List<Object> reviews { get; set; }
                public List<Object> biographies { get; set; }
                public List<Object> years_active { get; set; }
                public List<Object> video { get; set; }
                public Object urls { get; set; }
                public List<Object> images { get; set; }
                public List<Object> news { get; set; }
                public Object artist_location { get; set; }
                public int originId { get; set; }

                public List<ArtistInfo.ArtistSong> ArtistSongs { get; set; }

                public class ArtistSong
                {
                    public String title { get; set; }
                    public String title_id { get; set; }
                }
            }

            public class Song
            {
                public string Artist_Id { get; set; }
                public string Artist_Name { get; set; }
                public string Title { get; set; }
                public List<Object> tracks { get; set; }
                public int originId { get; set; }
                public double song_hotttnesss { get; set; }

                public Object audio_summary { get; set; }
               /* {
                    public double energy { get; set; }
                    public double liveness { get; set; }
                    public double tempo { get; set; }
                    public double speechiness { get; set; }
                    public double duration { get; set; }
                    public double acousticness { get; set; }
                    public double danceability { get; set; }
                    public double loudness { get; set; }
                }*/
             
            }

            
        }
    }
}
