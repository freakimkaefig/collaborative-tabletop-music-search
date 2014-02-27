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
using System.Text.RegularExpressions;

namespace MusicSearch.Test
{

    [TestClass]
    public class testObjects
    {
        public String artist_id { get; set; }
        public String title_id { get; set; }
        public String genre { get; set; }
        public int tangibleId { get; set; }
    }

    // Test-Enviroment for Queries to EchoNest, Parsing the responses and logging them
    [TestClass]
    public class QueryTests
    {

        public ResponseContainer ResponseContainer;
        private string _defaultURL = "http://developer.echonest.com/api/v4/";
        //API Key aus DB holen
        private string _apiKey = "L5WMCPOK4F2LA9H5X&"; //ends with "&" !
        
        //neue Instanz vom ResponseContainer
        List<ResponseContainer.ResponseObj.Song> SearchRC = new List<ResponseContainer.ResponseObj.Song>();

        //neue Instanz vom ResponseContainer für die Playliste (für Anfrage-Listen beim "playliste öffnen" um infos zu einer liste von spotify id's zu erhalten
        List<ResponseContainer.ResponseObj.Song> PlaylistRC = new List<ResponseContainer.ResponseObj.Song>();
        
        //TEST-LISTE
        List<testObjects> testListe = new List<testObjects>();

        //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
        List<ResponseContainer.ResponseObj.ArtistSuggestion> ArtistSuggestionsRC = new List<ResponseContainer.ResponseObj.ArtistSuggestion>();

        //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
        List<ResponseContainer.ResponseObj.TitleSuggestion> TitleSuggestionsRC = new List<ResponseContainer.ResponseObj.TitleSuggestion>();


        [TestMethod]
        public void startTests()
        {
            SuggestionQuery("title",1,"roar");

            //SearchQuery(/*Testliste*/);
        }


        public void SuggestionQuery(String type, int ID, String term)
        {
            //Debug.WriteLine("suggestion query, nothing else...");

            if(type == "title")
            {
                getTitleSuggestions(ID, term);
            }
            if(type == "artist")
            {
                getArtistSuggestions(ID, term);
            }

            //Ergebnisse darstellen
            sendSuggestionResults();
        }

        public void getTitleSuggestions(int ID, String term)
        {
            /*
             http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=roar
            */
            if (term.Contains(" "))
            {
                term = term.Replace(" ", "+");
                Debug.WriteLine("fixed term-spacing to: " + term);
            }

            term = term.ToLower();

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=" + term;

            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");

            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //'songs' durch 'TitleSuggestions' ersetzen
            var regex = new Regex(Regex.Escape("songs"));
            var newText = regex.Replace(cleared, "TitleSuggestions", 1);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //ID einfügen, zwecks Rückschlüssen
            String JSONTangibleId = "{\"tangibleId\": \"" + ID + "\"}";

            for (int i = 0; i < temp.Response.TitleSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONTangibleId, temp.Response.TitleSuggestions[i]);
                TitleSuggestionsRC.Add(temp.Response.TitleSuggestions[i]);
                //
                //event auslösen, dass ein neues ergebniss da ist. event listener zeigt dieses sofort an.
                //

            }

        }

        public void getArtistSuggestions(int ID, String term)
        {
            /*
             * http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=du+hast
            */
        }
        
        public void SearchQuery(/*List testListe*/)
        {
         
            
            //##################################
            //TEST-Liste befüllen
            //##################################
            /*testListe.Add(new testObjects
            {
               genre = "Rock",
               tangibleId = 1
                
            });*/
            testListe.Add(new testObjects
            {
                artist_id = "ARH6W4X1187B99274F",
                tangibleId = 2

            });
            testListe.Add(new testObjects
            {
                title_id = "SOFJZMT12A6D4F883D",
                tangibleId = 3

            });
            //##################################
            //##################################

            Debug.WriteLine("Reading List...");
            Debug.WriteLine("testliste.länge = " + testListe.Count);
            

            //#############################
            //#           START           #
            //#############################
            //TEST-Liste auslesen
            for (int i = 0; i < testListe.Count; i++)
            {
                if (!String.IsNullOrEmpty(testListe[i].artist_id))
                {
                    Debug.WriteLine("\nFOUND ARTIST IN TESTLISTE: " + testListe[i].artist_id + " at position: " + i);
                    SongsByArtistIDQuery(testListe[i].artist_id, testListe[i].tangibleId);
                    
                }
                if (!String.IsNullOrEmpty(testListe[i].title_id))
                {
                    Debug.WriteLine("\nFOUND TITLE IN TESTLISTE: " + testListe[i].title_id + " at position: " + i);
                    SongsByTitleIDQuery(testListe[i].title_id, testListe[i].tangibleId);
                    
                }
                if (!String.IsNullOrEmpty(testListe[i].genre))
                {
                    Debug.WriteLine("\nFOUND GENRE IN TESTLISTE: " + testListe[i].genre + " at position: " + i);
                    SongsByGenreQuery(testListe[i].genre, testListe[i].tangibleId);

                }
            }
            //Ergebnisse darstellen
            sendSearchResults();
        }


        public void SongsByGenreQuery(String genre, int ID)
        {
            
            if (genre.Contains(" "))
            {
                genre = genre.Replace(" ", "+");
                Debug.WriteLine("fixed genre-spacing to: " + genre);
            }

            genre = genre.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&song_selection=song_hotttnesss-top&variety=1&type=genre-radio&genre=" + genre;

            //
            //WIEVIELE RESULTS ?????????????????????????
            //

            //Debug.WriteLine("genre-request URL = " + request);

            //_getGenre = true;
            LoadOnlineResponse(request, ID); //Send Query


            //find similiar songs (or artists and then songs...) by those artists
            //bsp:
            //
        }



        public void SongsByTitleIDQuery(String title_id, int ID)
        {
            /* NO LONGER NEEDED SINCE ID IS UNIQUE AND CORRECTLY FORMATED
             * 
             * if (title.Contains(" "))
            {
                title = title.Replace(" ", "+");
                Debug.WriteLine("fixed title-spacing to: "+title);
            }

            title = title.ToLower();

            //'api_key' via GetAPIKey()
            // check for further parameters like "hotttnesss"*/

            String request = _defaultURL + "song/profile?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&id=" + title_id;

            Debug.WriteLine("sending title query...\n" + request);
            //############
            LoadOnlineResponse(request, ID); //Send Query
            //############
        }


        public void SongsByArtistIDQuery(String artist_id, int ID)
        {
            /* NO LONGER NEEDED SINCE ID IS UNIQUE AND CORRECTLY FORMATED
             * 
             if (artist.Contains(" "))
             {
                 artist = artist.Replace(" ", "+");
                 //Debug.WriteLine(artist);
             }

             artist = artist.ToLower();*/

            //'api_key' via GetAPIKey()
            // check for further parameters like "hotttnesss" ?

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist_id=" + artist_id;
            
            Debug.WriteLine("sending artist query...\n"+request);
            //############
            LoadOnlineResponse(request, ID); //Send Query
            //############

            //Assert.IsNull(_request); //TEST
        }


        public void GetAPIKey()
        {
            //return API-Key
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


            var temp = JsonConvert.DeserializeObject<ResponseContainer>(cleared);
            //tangibleId einfügen, zwecks Rückschlüssen
            String JSONTangibleId = "{\"tangibleId\": \"" + ID + "\"}";
            
            for(int i = 0; i < temp.Response.Songs.Count; i++)
            {
                JsonConvert.PopulateObject(JSONTangibleId,temp.Response.Songs[i]);
                SearchRC.Add(temp.Response.Songs[i]);
                //
                //event auslösen, dass ein neues ergebniss da ist. event listener zeigt dieses sofort an.
                //

            }
        }

        public void sendSearchResults()
        {
            //event auslösen, dass suchergebnisse komplett sind

        }

        public void sendSuggestionResults()
        {
            //event auslösen (MIT FlAG auf ID), dass suchvorschläge da sind

        }
    }
}
