﻿using System;
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

    [TestClass]
    public class testObjects
    {
        public String artist { get; set; }
        public String title { get; set; }
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
        private Boolean _getGenre = false;
        public int responseCounter = 0;


        //neue Instanz vom ResponseContainer
        List<ResponseContainer.ResponseObj.Song> RC = new List<ResponseContainer.ResponseObj.Song>();

        //neue Instanz vom ResponseContainer für die Playliste
        List<ResponseContainer.ResponseObj.Song> PRC = new List<ResponseContainer.ResponseObj.Song>();
        

        //TEST-LISTE
        List<testObjects> testListe = new List<testObjects>();
        

        [TestMethod]
        public void BuildQuery(/*Liste*/)
        {
            responseCounter = 0;
            //Liste leeren
            RC.RemoveRange(0,RC.Count);

            //TODO
            //Übergebene Liste enthält Objecte {id, definition(genre/artist/title)}
            /* 1)   bei aufruf übergebene Liste traversieren nach API-Teilen
             * 2)   Einzelfälle (suche nach interpret/titel) herauspicken -> einzelne suchanfrage
             * 3)   Spezialfälle herauspicken (Genres,Parameter,...)
             * 3.1) Mögliche Kombinationen aus Objekten (graphen/Netzwerk) abfragen
             * 4)   Übergabe der auslösenden Information/Tangibles, um Rückschlüsse zu ermöglichen
             * 5)   Queries für Infos des DetailView erstellen, ausgehend von Playlist oder Suchergebnis.
             */
          
            //Bsp-Query
            //SongsByArtistQuery("Katy Perry");
            //SongsByTitleQuery("wrecking ball");


            //TEST-Liste befüllen
            testListe.Add(new testObjects
            {
               artist = "Katy Perry",
               tangibleId = 1
                
            });
            testListe.Add(new testObjects
            {
                artist = "Rammstein",
                tangibleId = 2

            });

            Debug.WriteLine("Reading List...");
            Debug.WriteLine("testliste.länge = " + testListe.Count);
            

            //#############################
            //#           START           #
            //#############################
            //TEST-Liste auslesen
            for (int i = 0; i < testListe.Count; i++)
            {
                if (!String.IsNullOrEmpty(testListe[i].artist))
                {
                    Debug.WriteLine("\nFOUND ARTIST IN TESTLISTE: " + testListe[i].artist+" at position: "+i);
                    Debug.WriteLine("sending artist query...");
                    SongsByArtistQuery(testListe[i].artist, testListe[i].tangibleId);
                }
                if (!String.IsNullOrEmpty(testListe[i].title))
                {
                    Debug.WriteLine("\nFOUND TITELE IN TESTLISTE: " + testListe[i].title + " at position: " + i);
                    Debug.WriteLine("sending title query...");
                    //SongsByArtistQuery(testListe[i].artist, testListe[i].tangibleId);
                }
                
                
                //if genre...
                //Bsp-genre-Array
                //String[] test = { "Hip Hop", "Rock" };
                //SongsByGenreQuery(test);

            }
        }


        public void SongsByGenreQuery(String[] genres, int tangibleId)
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
                queryGenres += "&genre=" + fixedGenre;
            }
            queryGenres = queryGenres.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&type=genre-radio" + queryGenres + "&results=4";

            //
            //WIEVIELE RESULTS ?????????????????????????
            //

            Debug.WriteLine("request URL = " + request);

            //_getGenre = true;
            LoadOnlineResponse(request, tangibleId); //Send Query


            //find similiar songs (or artists and then songs...) by those artists
            //bsp:
            //
        }


        //############
        //# WORKS    #
        //# USED     #
        //############
        public void SongsByTitleQuery(String title, int tangibleId)
        {
            if (title.Contains(" "))
            {
                title = title.Replace(" ", "+");
            }

            title = title.ToLower();

            //'api_key' via GetAPIKey()
            // check for further parameters like "hotttnesss"

            String request = _defaultURL + "song/search?" + "api_key=" + _apiKey + "format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "title=" + title + "&results=1";

            //############
            LoadOnlineResponse(request, tangibleId); //Send Query
            //############
        }

        //############
        //# WORKS    #
        //# USED     #
        //############
        public void SongsByArtistQuery(String artist, int tangibleId)
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
            LoadOnlineResponse(request, tangibleId); //Send Query
            //############

            //Assert.IsNull(_request); //TEST
        }


        public void GetAPIKey()
        {
            //return API-Key
        }

        public void LoadOnlineResponse(String request, int tId) //Send Query
        {
            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            //Debug.WriteLine("response: "+response);    
            
            //String editedResponse = response.Insert(response.IndexOf("\"title\""),JSONTangibleId);
            //Debug.WriteLine("response: " + editedResponse);
   
            ParseResponse(response, tId);
        }

        public void ParseResponse(String response, int tId)
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode


            var temp = JsonConvert.DeserializeObject<ResponseContainer>(cleared);
            //tangibleId einfügen, zwecks Rückschlüssen
            String JSONTangibleId = "{\"tangibleId\": \"" + tId + "\"}";
            
            for(int i = 0; i < temp.Response.Songs.Count; i++)
            {
                JsonConvert.PopulateObject(JSONTangibleId,temp.Response.Songs[i]);
                RC.Add(temp.Response.Songs[i]);
                //JsonConvert.PopulateObject(JSONTangibleId, RC[i], null);
            }



            //Debug.WriteLine("tangibleID: "+tangibleId);
            /*for (int i = 0; i < RC.Count; i++)
            {
                Debug.WriteLine("\nResponsecontainer Song-Liste Tracks["+i+"]= " + RC[i].tracks);
            }*/

            /*TANGIBLE ID's auslesen
            for(int i=0; i<RC.Count; i++)
            {
                Debug.WriteLine("\nResponsecontainer Song-Liste Tangible-ID's an Position "+responseCounter+" = " + RC[i].tangibleId);
            }
            */
            
            
            //+ ", " + ResponseContainer.Response.Songs[0].Artist_Name + ", " + ResponseContainer.Response.Songs[0].Title);


            /*if (_getGenre == true)
            {

                char[] delimiterChar = { '}' };
                String cuttedResponseString;
                String finalGenreResponse = "";
                _getGenre = false;
                int loopIndex = 0;
                foreach (var Song in ResponseContainer.Response.Songs)
                {
                    loopIndex++;
                    //Debug.WriteLine("\nsong id: " + Song.Artist_Id);
                    //http://developer.echonest.com/api/v4/artist/terms?api_key=L5WMCPOK4F2LA9H5X&format=json&sort=weight&id=ID_HERE

                    String requestGenre = _defaultURL + "artist/terms?api_key=" + _apiKey + "format=json&sort=weight&id=" + Song.Artist_Id;
                    //JSON response delivered as string
                    String responseGenre = HttpRequester.StartRequest(requestGenre);
                    //transform "\'" to unicode equivalent
                    responseGenre = responseGenre.Replace("'", "&#39;");

                    // string[] words = text.Split(delimiterChars);
                    string[] cuttedResponse = responseGenre.Split(delimiterChar);
                    //Debug.WriteLine("gekappte Terms: " + cuttedResponse[0] + cuttedResponse[1] +"}]}}}");

                    //falls erster durchlauf
                    if (loopIndex == 1)
                    {
                        cuttedResponseString = cuttedResponse[0] + "}" + cuttedResponse[1];
                        finalGenreResponse += @"" + cuttedResponseString.Replace("\"", "'");
                    }
                    if (loopIndex > 1)
                    {
                        Debug.WriteLine("index of: "+cuttedResponse[1].IndexOf('[') + 1);
                        Debug.WriteLine("cuttedResponse[1].length: " + cuttedResponse[1].Length);
                        cuttedResponseString = "}" + cuttedResponse[1].Substring(cuttedResponse[1].IndexOf('[') + 1, cuttedResponse[1].Length);
                        finalGenreResponse += @"" + cuttedResponseString.Replace("\"", "'");
                    }
                    if (loopIndex == ResponseContainer.Response.Songs.Count)
                    {
                        cuttedResponseString = "}" + cuttedResponse[1].Substring(cuttedResponse[1].IndexOf('[') + 1, cuttedResponse[1].Length) + "}]}}";
                        finalGenreResponse += @"" + cuttedResponseString.Replace("\"", "'");
                    }




                    //Debug.WriteLine("artist id: " + Song.Artist_Id);
                    //Debug.WriteLine("Artist: " + Song.Artist_Name);
                    //Debug.WriteLine("Titel: " + Song.Title);

                }
                Debug.WriteLine("finalgenreresponse: " + finalGenreResponse);
                ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(finalGenreResponse);

                //test-schleife; reihenfolge der songs gleich reihenfolge der gesuchten genres?
                foreach (var Term in ResponseContainer.Response.Terms)
                {
                    Debug.WriteLine("Genre: " + Term.Name);
                }

                //ResponseGenre zu Ergebniss-Objekt mit übereinstimmender Artist_id (SongObj = anfrage-ID) packen
            }*/


            //############
            //send results to Visualization
            //ToDo: implement ShowResult(Object) or similiar
            //{Tangible-id, Tangible-definition(genre/artist/title), ergebnisse}
            //############
        }
    }
}
