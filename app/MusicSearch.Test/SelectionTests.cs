using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicSearch.Managers;
using MusicSearch.ResponseObjects;
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
            SearchManager sm = new SearchManager();
            //var temp = sm.getCombinedSearchAttributes("artist");

            //var temp = sm.combinedSearchQuery(combinedSearchListe);
                //combinedSearchQuery(combinedSearchListe);


            //String temp = sm.lowerToUpper("lower upper");
            //var temp = sm.getGenreSuggestions("elec");
            //var temp2 = sm.getGenres();
            var temp = sm.getArtistSuggestions(1, "katy");

            //ArtistInfosRC.Add(new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistSong>());

            
            /*searchListe.Add(new searchObjects
            {
                artist_id = "ARH6W4X1187B99274F",
                originId = 2

            });
            searchListe.Add(new searchObjects
            {
                title_id = "SOFJZMT12A6D4F883D",
                originId = 3

            });*/
            
            //Debug.WriteLine("Reading List...");
            //Debug.WriteLine("testliste.länge = " + searchListe.Count);


            //Vorschlag-Anfrage
            //sm.SuggestionQuery("artist",1,"Katy P");

            //Such-Anfrage
            //sm.SearchQuery(searchListe);

            //Detail-View Info's Anfrage
            //sm.getDetailInfo(null, "ARH6W4X1187B99274F", "123");

            //api key holen
            //var temp = GetAPIKey();
            //Debug.WriteLine(temp);
            
        }

        //###################################################
        //###################################################

        

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
