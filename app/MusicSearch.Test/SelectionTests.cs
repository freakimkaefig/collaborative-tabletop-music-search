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


    // Test-Enviroment for Queries to EchoNest, Parsing the responses and logging them
    [TestClass]
    public class QueryTests
    {

        public ResponseContainer ResponseContainer;
        private string _defaultURL = "http://developer.echonest.com/api/v4/";
        //API Key aus DB holen
        private string _apiKey = "L5WMCPOK4F2LA9H5X&"; //ends with "&" !
        private Boolean _getGenre = false;


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

            //Bsp-genre-Array
            String[] test = {"Hip Hop","Rock"};
            SongsByGenreQuery(test);
        }


        public void SongsByGenreQuery(String[] genres)
        {
            String fixedGenre;
            String queryGenres = "";
            foreach (String genre in genres)
            {
                fixedGenre = genre;
                if (genre.Contains(" "))
                {
                    fixedGenre = genre.Replace(" ", "+");
                }
                queryGenres += "&genre="+fixedGenre;
            }
            queryGenres = queryGenres.ToLower();
            
            String request = _defaultURL + "playlist/static?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&type=genre-radio"+queryGenres+"&results=4";

            Debug.WriteLine("request URL = " + request);

            _getGenre = true;
            LoadOnlineResponse(request); //Send Query


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

            title = title.ToLower();

            //'api_key' via GetAPIKey()
            // check for further parameters like "hotttnesss"

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "title=" + title;

            //############
            //LoadOnlineResponse(request); //Send Query
            //############
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

            artist = artist.ToLower();

            //'api_key' via GetAPIKey()
            // check for further parameters like "hotttnesss"

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist=" + artist;

            //############
            //LoadOnlineResponse(request); //Send Query
            //############

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
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");

            Debug.WriteLine("\n_response: " + response);

            ParseResponse(response);
        }

        public void ParseResponse(String response)
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            if (_getGenre == true)
            {
                _getGenre = false;
                foreach (var Song in ResponseContainer.Response.Songs)
                {
                    //Debug.WriteLine("\nsong id: " + Song.Artist_Id);
                    //http://developer.echonest.com/api/v4/artist/terms?api_key=L5WMCPOK4F2LA9H5X&format=json&sort=weight&id=ID_HERE

                    String requestGenre = _defaultURL+"artist/terms?api_key="+_apiKey+"format=json&sort=weight&id="+Song.Artist_Id;
                    //JSON response delivered as string
                    String responseGenre = HttpRequester.StartRequest(requestGenre);
                    //transform "\'" to unicode equivalent
                    responseGenre = responseGenre.Replace("'", "&#39;");
                    var clearedGenre = @"" + responseGenre.Replace("\"", "'");
                    ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(clearedGenre);

                    //NUR DEN ERSTEN TERM

                    Debug.WriteLine("song id: " + Song.Artist_Id);
                    Debug.WriteLine("Artist: " + Song.Artist_Name);
                    Debug.WriteLine("Titel: " + Song.Title);

                }

                //test-schleife; reihenfolge der songs gleich reihenfolge der gesuchten genres?
                foreach (var Term in ResponseContainer.Response.Terms)
                {
                    Debug.WriteLine("Genre: " + Term.Name);
                }
            }
            

            //############
            //send results to Visualization
            //ToDo: implement ShowResult() or similiar
            //############
        }
    }
}
