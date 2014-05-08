using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.Objects
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
            public List<genres> Genres { get; set; }


            public class StatusObj
            {
                public string Version { get; set; }
                public int Code { get; set; }
                public string Message { get; set; }
            }

            public class Suggestion
            {
                public String id { get; set; }
                public int originId { get; set; }
            }

            public class ArtistSuggestion : Suggestion
            {
                public String name { get; set; }
            }

            public class TitleSuggestion : Suggestion
            {
                public String title { get; set; }
                public String artist_name { get; set; }
            }

            public class ArtistInfo
            {
                public List<ArtistInfo.Terms> terms { get; set; }
                public String name { get; set; }
                public List<ArtistInfo.ForeignIds> facebookIds { get; set; }
                public List<ArtistInfo.Blog> blogs { get; set; }
                public String id { get; set; }
                public List<ArtistInfo.Review> reviews { get; set; }
                public List<ArtistInfo.Biography> biographies { get; set; }
                public List<ArtistInfo.YearsActive> years_active { get; set; }
                public List<ArtistInfo.Video> video { get; set; }
                public List<ArtistInfo.Image> images { get; set; }
                public List<ArtistInfo.News> news { get; set; }

                public List<ArtistInfo.ArtistSong> ArtistSongs { get; set; }
                public List<ArtistInfo.SimilarArtist> SimilarArtists { get; set; }
                public List<ArtistInfo.ArtistLocation> artist_location { get; set; }
                public List<ArtistInfo.Url> Urls { get; set; }

                public class News
                {
                    public String summary { get; set; }
                    public String name { get; set; }
                    public String url { get; set; }
                }

                public class Image
                {
                    public String url { get; set; }
                }

                public class Video
                {
                    public String title { get; set; }
                    public String image_url { get; set; }
                    public String url { get; set; }
                }

                public class YearsActive
                {
                    public String start { get; set; }
                }

                public class Biography
                {
                    public String text { get; set; }
                    public String url { get; set; }
                }
                
                public class Review
                {
                    public String name { get; set; }
                    public String release { get; set; }
                    public String url { get; set; }
                    public String summary { get; set; }
                }

                public class Blog
                {
                    public String name { get; set; }
                    public String url { get; set; }
                    public String date_posted { get; set; }
                    public String summary { get; set; }
                }

                public class ForeignIds
                {
                    public String facebookId { get; set; }
                }

                public class Terms
                {
                    public Double frequency { get; set; }
                    public Double weight { get; set; }
                    public String name { get; set; }
                }
                
                public class ArtistLocation
                {
                    public String city { get; set; }
                    public String region { get; set; }
                    public String location { get; set; }
                    public String country { get; set; }
                }

                public class Url
                {
                    public String official_url { get; set; }
                    public String lastfm_url { get; set; }
                    public String twitter_url { get; set; }
                    public String myspace_url { get; set; }
                    public String wikipedia_url { get; set; }
                    public String mb_url { get; set; }
                    public String name { get; set; }
                }

                public class SimilarArtist
                {
                    public String name {get;set;}
                    public double familiarity {get;set;}
                    public String artist_id { get; set; }
                }
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
                public List<Tracks> tracks { get; set; }
                public double song_hotttnesss { get; set; } 
                public int[] originIDs { get; set; }
                public Object audio_summary { get; set; }

                public string ToString()
                {
                    return Artist_Name + " - " + Title + " - Origin:" + String.Join("", new List<int>(originIDs).ConvertAll(i => i.ToString()).ToArray()); ;
                }
            }

            public class Tracks
            {
                public string foreign_id { get; set; }
            }

            public class genres
            {
                public String genre_name { get; set; }
                public List<genres.subgenres> Subgenres { get; set; }
                
                public class subgenres
                {
                    public String name { get; set; }
                }
            }
            
            
        }
    }
}
