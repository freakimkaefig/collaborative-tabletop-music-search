using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MusicSearch.Managers;
using MusicSearch.SearchObjects;
using MusicSearch.ResponseObjects;
using System.Diagnostics;

namespace MusicSearch.Test
{
    [TestClass]
    public class SelectionTests
    {
        [TestMethod]
        public void DoArtistSelection()
        {
            /*
            var selectionManager = new SelectionManager();
            var type = new KeywordType();
            type.Type = KeywordType.Types.Artist;
            selectionManager.SetSelection(type.Type);
             */
        }
    }


    // Test-Enviroment for Queries to EchoNest, Parsing the resonses and logging them
    [TestClass]
    public class QueryTests
    {
        private string _request;
        private string _response;
        public ResponseContainer ResponseContainer;
        private string _defaultURL = "http://developer.echonest.com/api/v4/";
        //API Key aus DB holen
        private string _apiKey = "L5WMCPOK4F2LA9H5X&";


        [TestMethod]
        public void SongsByArtistQuery()
        {
            BuildString(1, "Radiohead");
        }


        /*public void RequestOffline()
        {
            _request = "http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&style=rock&min_danceability=0.65&min_tempo=140&results=5";
            //_request = BuildString();
        }*/

        //TODO
        /* bei aufruf übergebene Liste traversieren nach API-Teilen
         * 1) Einzelfälle (suche nach interpret/titel) herauspicken -> einzelne suchanfrage
         * 2) Spezialfälle herauspicken (Genres,Parameter,...)
         * 2.1) Mögliche Kombinationen aus Objekten (graphen/Netzwerk) abfragen
         *      
         * 
         * 
         * 
         */
        public void BuildString(int i, String name)
        {
            if (i == 1) //API-Teil = Artist
            {

                //Leerzeichen aus String durch '+' ersetzten!

                //'artist/' from var!
                //'songs?' from var!
                //'name=' from var!

                _request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=tracks&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&" + "artist=" + name;
                Debug.WriteLine(_request);


                
                LoadOnlineResponse(_request);
                //Assert.IsNull(_request);
            }
            //String _request = _defaultURL;
            //angesprochener API Teil +
            //Methodenaufruf + 
            //String apiKey = GetAPIKey() +
            //Parameter 
            //LoadOnlineResponse(_request);
        }

        public void GetAPIKey()
        {
            //get API-Key 
        }

        public void LoadOnlineResponse(String request)
        {
            //JSON response delivered as string
            _response = HttpRequester.StartRequest(request);
            Debug.WriteLine("\n_response: " + _response);
            //_response = _response.Replace("'", "&#39;");

            ParseResponse();
        }

        public void ParseResponse()
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + _response.Replace("\"", "'");//Apostrophes are right now replaced by HTML unicode
            ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            foreach (var Song in ResponseContainer.Response.Songs)
            {
                //Debug.WriteLine("\n" + Song.Title);
            }
        }
    }
}
