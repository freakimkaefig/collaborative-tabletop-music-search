using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicSearch.Managers;
using MusicSearch.Objects;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;



namespace MusicSearch.Test
{

    [TestClass]
    public class searchObjects
    {
        public String artist_id { get; set; }
        public String title_id { get; set; }
        public String genre { get; set; }
        public int originId { get; set; }
    }

    [TestClass]
    public class combinedSearchObjects
    {
        public String artist_id { get; set; }
        public String[] genre { get; set; }
        public List<int> originIds { get; set; }

        //parameters...
        public List<ArtistParameter> ArtistParameter { get; set; }
        public List<GenreParameter> GenreParameter { get; set; }
    }
    [TestClass]
    public class ArtistParameter
    {
        public double max_tempo { get; set; }
        public double min_tempo { get; set; }
        public double artist_min_familiarity { get; set; }
        public String artist_start_year_before { get; set; }
        public String artist_start_year_after { get; set; }
        public String artist_end_year_before { get; set; }
        public String artist_end_year_after { get; set; }
        public double song_min_hotttnesss { get; set; }
        public double artist_min_hotttnesss { get; set; }
        public double min_danceability { get; set; }
        public double min_energy { get; set; }
        public double min_liveness { get; set; }
        public double min_acousticness { get; set; }
    }
    [TestClass]
    public class GenreParameter
    {
        public String song_selection { get; set; }
        public double variety { get; set; }
        public String distribution { get; set; }
        public double adventurousness { get; set; }
    }

    // Test-Enviroment for Queries to EchoNest, Parsing the responses and merging them into Response-Container
    [TestClass]
    public class QueryTests
    {

        public ResponseContainer ResponseContainer;
        private string _defaultURL = "http://developer.echonest.com/api/v4/";

        private String apiKey = null;
        
        //neue Instanz vom ResponseContainer
        List<ResponseContainer.ResponseObj.Song> SearchRC = new List<ResponseContainer.ResponseObj.Song>();
                
        //TEST-LISTE
        List<searchObjects> searchListe = new List<searchObjects>();

        //TEST-LISTE
        List<combinedSearchObjects> combinedSearchListe = new List<combinedSearchObjects>();

        //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
        List<ResponseContainer.ResponseObj.ArtistSuggestion> ArtistSuggestionsRC = new List<ResponseContainer.ResponseObj.ArtistSuggestion>();

        //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
        List<ResponseContainer.ResponseObj.TitleSuggestion> TitleSuggestionsRC = new List<ResponseContainer.ResponseObj.TitleSuggestion>();

        //neue Instanz vom ResponseContainer für die Infos des DetailViews pro Artist
        List<ResponseContainer.ResponseObj.ArtistInfo> ArtistInfosRC = new List<ResponseContainer.ResponseObj.ArtistInfo>();


        //Dictionaries
        Dictionary<string, object> combinedSearchArtistAttributes =
        new Dictionary<string, object>();

        Dictionary<string, object> combinedSearchGenreAttributes =
        new Dictionary<string, object>();


        [TestMethod]
        public void startTests()
        {
            // Diese Methode kann später gelöscht werden. Dient hier nur als Startpunkt.
            searchListe.Add(new searchObjects
            {
               genre = "Rock",
               originId = 1
                
            });

            combinedSearchListe.Add(new combinedSearchObjects
            {
                artist_id = "ARH6W4X1187B99274F",
                originIds = new List<int>() { 456, 555 },
                ArtistParameter = new List<ArtistParameter>() { new ArtistParameter() { artist_start_year_before = "2010", song_min_hotttnesss = 0.6 } }  

            });
            //SearchManager sm = new SearchManager();
            var temp = getArtistInfo("U2");
        }

        //###################################################
        //###################################################

        public List<ResponseContainer.ResponseObj.ArtistInfo> getArtistInfo(String artist)
        {
            List<ResponseContainer.ResponseObj.ArtistInfo> ArtistInfosRC = new List<ResponseContainer.ResponseObj.ArtistInfo>();

            /* 
             * FB-Seite:
             * gibt "facebook:artist:6979332244" zurück, seite lautet dann http://www.facebook.com/profile.php?id=6979332244
             */
            //fix spacing and upper-case letters
            if (artist.Contains(" "))
            {
                artist = artist.Replace(" ", "+");
            }
            artist = artist.ToLower();

            //build first query (basic information about the artist)
            String request = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=terms&bucket=id:facebook&bucket=biographies&bucket=years_active&bucket=video&bucket=blogs&bucket=reviews&bucket=images&bucket=news&sort=hotttnesss-desc&results=1&name=" + artist;
            String response = HttpRequester.StartRequest(request);
            if (String.IsNullOrEmpty(response))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response to receive results in RC
            var newText = StringHelper.replacePartialString(cleared, "artists", "ArtistInfos", 1);
            newText = StringHelper.replacePartialString(newText, "foreign_id", "facebookId", 1000);
            newText = StringHelper.replacePartialString(newText, "facebook:artist:", "", 1000);

        
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //Origin-IDs not needed since the implementation of (this) method-calls 
            //allow to distinguish their origin
            //String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            //JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistInfos[0]);
            //add first artist-info-results to RC
            ArtistInfosRC.Add(temp.Response.ArtistInfos[0]);

            //build 2nd query (songs of the artist)
            String request2 = _defaultURL + "artist/songs?" + "api_key=" + GetAPIKey() + "&format=json&results=100&name=" + artist;
            String response2 = HttpRequester.StartRequest(request2);
            if (String.IsNullOrEmpty(response2))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response2 = response2.Replace("'", "&#39;");
            //Apostrophes are replaced by HTML unicode
            var cleared2 = @"" + response2.Replace("\"", "'");
            //manipulate response to receive results in RC
            var newText2 = StringHelper.replacePartialString(cleared2, "songs", "ArtistInfos\': [{\'ArtistSongs", 1);
            newText2 = newText2.Insert(newText2.LastIndexOf("}") - 1, "}]");
            var newText3 = StringHelper.replacePartialString(newText2, "id", "title_id", 100);
            var temp2 = JsonConvert.DeserializeObject<ResponseContainer>(newText3);
            //Initialise inner list of RC
            ArtistInfosRC[0].ArtistSongs = new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistSong>();
            //add further artist-info-results to inner list of RC
            for (int i = 0; i < temp2.Response.ArtistInfos[0].ArtistSongs.Count; i++)
            {
                ArtistInfosRC[0].ArtistSongs.Add(temp2.Response.ArtistInfos[0].ArtistSongs[i]);
            }

            //build 3rd query (similiar artists)
            String request3 = _defaultURL + "artist/similar?" + "api_key=" + GetAPIKey() + "&format=json&bucket=familiarity&min_familiarity=0.7&name=" + artist;
            String response3 = HttpRequester.StartRequest(request3);
            if (String.IsNullOrEmpty(response3))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response3 = response3.Replace("'", "&#39;");
            var cleared3 = @"" + response3.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //manipulate response to receive results in RC
            var newText4 = StringHelper.replacePartialString(cleared3, "artists", "ArtistInfos\': [{\'SimilarArtists", 1);
            newText4 = newText4.Insert(newText4.LastIndexOf("}") - 1, "}]");
            var newText5 = StringHelper.replacePartialString(newText4, "id", "artist_id", 100);
            var temp3 = JsonConvert.DeserializeObject<ResponseContainer>(newText5);
            //Initialise inner list of RC
            ArtistInfosRC[0].SimilarArtists = new List<ResponseContainer.ResponseObj.ArtistInfo.SimilarArtist>();
            //add remaining artist-info-results to second inner list of RC
            for (int i = 0; i < temp3.Response.ArtistInfos[0].SimilarArtists.Count; i++)
            {
                ArtistInfosRC[0].SimilarArtists.Add(temp3.Response.ArtistInfos[0].SimilarArtists[i]);
            }
            //order results of second inner list descending by familiarity
            ArtistInfosRC[0].SimilarArtists = ArtistInfosRC[0].SimilarArtists.OrderByDescending(a => a.familiarity).ToList();

            //build 4th query (urls)
            String request4 = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&results=1&name=" + artist + "&bucket=urls&sort=hotttnesss-desc";
            String response4 = HttpRequester.StartRequest(request4);
            if (String.IsNullOrEmpty(response4))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response4 = response4.Replace("'", "&#39;");
            var cleared4 = @"" + response4.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //manipulate response to receive results in RC
            var newText6 = StringHelper.replacePartialString(cleared4, "artists", "ArtistInfos", 1);
            var newText7 = StringHelper.replacePartialString(newText6, "\'urls\': {", "\'Urls\': [{", 1);
            var newText8 = StringHelper.replacePartialString(newText7, "html\'},", "html\'}],", 1);
            //var newText8 = newText7.Remove(newText7.LastIndexOf("}") - 3);
            //newText8 = newText8.Insert(newText8.LastIndexOf("}"), "]}]}");
            //var newText7 = StringHelper.replacePartialString(newText6, "id", "artist_id", 100);
            var temp4 = JsonConvert.DeserializeObject<ResponseContainer>(newText8);

            //Initialise inner list of RC
            ArtistInfosRC[0].Urls = new List<ResponseContainer.ResponseObj.ArtistInfo.Url>();
            //add remaining artist-info-results to second inner list of RC
            for (int i = 0; i < temp4.Response.ArtistInfos[0].Urls.Count; i++)
            {
                ArtistInfosRC[0].Urls.Add(temp4.Response.ArtistInfos[0].Urls[i]);
            }

            //build 5th request (artist location)
            String request5 = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&results=1&name=" + artist + "&bucket=artist_location&sort=hotttnesss-desc";
            String response5 = HttpRequester.StartRequest(request5);
            if (String.IsNullOrEmpty(response5))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response5 = response5.Replace("'", "&#39;");
            var cleared5 = @"" + response5.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //manipulate response to receive results in RC
            var newText9 = StringHelper.replacePartialString(cleared5, "artists", "ArtistInfos", 1);
            var newText10 = StringHelper.replacePartialString(newText9, "\'artist_location\': {", "\'artist_location\': [{", 1);
            var newText11 = newText10.Insert(newText10.LastIndexOf("\'},") + 2, "]");
            //var newText8 = newText7.Remove(newText7.LastIndexOf("}") - 3);
            //newText8 = newText8.Insert(newText8.LastIndexOf("}"), "]}]}");
            //var newText7 = StringHelper.replacePartialString(newText6, "id", "artist_id", 100);
            var temp5 = JsonConvert.DeserializeObject<ResponseContainer>(newText11);

            //Initialise inner list of RC
            ArtistInfosRC[0].artist_location = new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistLocation>();
            //add remaining artist-info-results to second inner list of RC
            for (int i = 0; i < temp5.Response.ArtistInfos[0].artist_location.Count; i++)
            {
                ArtistInfosRC[0].artist_location.Add(temp5.Response.ArtistInfos[0].artist_location[i]);
            }

            //reutrn gathered results
            return ArtistInfosRC;
        }

        //###################################################
        //###################################################
        

        private String GetAPIKey()
        {
            if(String.IsNullOrEmpty(apiKey))
            {
                //XML auslesen
                XDocument doc = XDocument.Load(@"../../../MusicSearch/files/config.xml");
                XElement el = doc.Element("apikey");
                apiKey = (String)el;

                //Debug.WriteLine("apiKey: " + apiKey);
            }
            return apiKey;
        }

        public void LoadOnlineResponse(String request, int ID) //Send Query
        {
            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            
            //Debug.WriteLine("received&fixed response: "+response);

            ParseResponse(response, ID);
        }

        public void ParseResponse(String response, int ID)
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            var regex3 = new Regex(Regex.Escape("spotify-WW:track"));
            var newText4 = regex3.Replace(cleared, "spotify:track", 1000);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText4);
            //originId einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            
            for(int i = 0; i < temp.Response.Songs.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId,temp.Response.Songs[i]);
                SearchRC.Add(temp.Response.Songs[i]);
                //
                //event auslösen, dass ein neues ergebniss da ist. event listener zeigt dieses sofort an.
                //

            }
           // var tempArray = SearchRC.ToArray();
            //Debug.Write(SearchRC);
            //File.WriteAllLines(@"C:\foo.txt", SearchRC.ConvertAll(Convert.ToString));
            //Debug.WriteLine(SearchRC);
            //Debug.WriteLine("\n" + SearchRC.ToString());
        }
    }
}
