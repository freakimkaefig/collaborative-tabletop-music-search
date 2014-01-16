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
       
        public ResponseContainer ResponseContainer;
        private string _defaultURL = "http://developer.echonest.com/api/v4/";
        //API Key aus DB holen
        private string _apiKey = "L5WMCPOK4F2LA9H5X&"; //ends with "&" !


        [TestMethod]
        public void BuildQuery(/*Liste*/)
        {
            //TODO
        /* 1)   bei aufruf übergebene Liste traversieren nach API-Teilen
         * 2)   Einzelfälle (suche nach interpret/titel) herauspicken -> einzelne suchanfrage
         * 3)   Spezialfälle herauspicken (Genres,Parameter,...)
         * 3.1) Mögliche Kombinationen aus Objekten (graphen/Netzwerk) abfragen
         * 4)   Übergabe der auslösenden Information/Tangibles, um Rückschlüsse zu ermöglichen
         */
            //Bsp-Query
            SongsByArtistQuery("Katy Perry");
            SongsByTitleQuery("wrecking ball");
        }


        public void SongsByGenreQuery()
        {
            //get artists by genre(s)
            //bsp:
            //http://developer.echonest.com/api/v4/playlist/static?api_key=L5WMCPOK4F2LA9H5X&format=json&distribution=wandering&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=song_hotttnesss&bucket=audio_summary&type=genre-radio&genre=house&genre=rock


            //find similiar songs (or artists and then songs...) by those artists
            //bsp:
            //
        }


                //############
                //# WORKS    #
                //# not used #
                //############
        public void SongsByTitleQuery(String title)
        {
            if (title.Contains(" "))
            {
                title = title.Replace(" ", "+");
            }

            //'api_key' via GetAPIKey()

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "title=" + title;
            
            //LoadOnlineResponse(request); //Send Query

        }

                //############
                //# WORKS    #
                //# not used #
                //############
        public void SongsByArtistQuery(String artist)
        {
                //Leerzeichen aus String durch '+' ersetzten!
            if (artist.Contains(" "))
            {
                artist = artist.Replace(" ", "+");
                //Debug.WriteLine(artist);
            }

                //'api_key' via GetAPIKey()

                String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist=" + artist;
                
                //LoadOnlineResponse(request); //Send Query


                //Assert.IsNull(_request); //TEST
        }


        public void GetAPIKey()
        {
            //get API-Key 
        }

        public void LoadOnlineResponse(String request) //Send Query
        {
            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            Debug.WriteLine("\n_response: " + response);
            //_response = _response.Replace("'", "&#39;");

            ParseResponse(response);
        }

        public void ParseResponse(String response)
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            foreach (var Song in ResponseContainer.Response.Songs)
            {
                //Debug.WriteLine("\n" + Song.Title);

                //send results to Visualization
                //ShowResult();
            }
        }
    }
}
