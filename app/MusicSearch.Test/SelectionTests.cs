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
using System.IO;
using System.Xml;
using System.Xml.Linq;



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
        public String genre { get; set; }
        public int[] originIds { get; set; }


    }

    // Test-Enviroment for Queries to EchoNest, Parsing the responses and logging them
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
            SearchManager sm = new SearchManager();
            //String temp = sm.lowerToUpper("lower upper");
            //var temp = sm.getGenreSuggestions("elec");
            //var temp2 = sm.getGenres();
            //sm.getArtistSuggestions(1, "katy");

            //ArtistInfosRC.Add(new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistSong>());

            //##################################
            //TEST-Liste befüllen
            //##################################
            
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
            //##################################
            //##################################
            //Debug.WriteLine("Reading List...");
            //Debug.WriteLine("testliste.länge = " + searchListe.Count);


            //Vorschlag-Anfrage
            //SuggestionQuery("artist",1,"Katy P");

            //Such-Anfrage
            //SearchQuery(searchListe);

            //Detail-View Info's Anfrage
            //getDetailInfo(null, "ARH6W4X1187B99274F", "123");

            //api key holen
            //var temp = GetAPIKey();
            //Debug.WriteLine(temp);
            
        }

        public void getDetailInfo(String artist_name, String artist_id, String originID)
        {
            //check if artist-name or artist-id is available
            if (!String.IsNullOrEmpty(artist_id))
            {
                //query by artist_id
                //find exact name 
                //http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&artist_id=ARH6W4X1187B99274F&results=1
                
                String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&artist_id=" + artist_id + "&results=1";
                //JSON response delivered as string
                String response = HttpRequester.StartRequest(request);
                //Antwort in Array splitten
                String[] splitted = response.Split(new Char[] { '\"' });
                //Artist-Name an fester Position in Array:
                String name = splitted[27].ToString();

                //do a query by artist_name
                getArtistInfo(name, originID);
            }
            else if (!String.IsNullOrEmpty(artist_name))
            {
                //do a query by artist_name
                getArtistInfo(artist_name, originID);
            }
            //#################################
            //#################################
            //detailierte Infos zum Song via artist&titel bisher nicht über echonest verfügbar...

            /*if (!String.IsNullOrEmpty(artist_id) && !String.IsNullOrEmpty(title))
            {
                //find exact name 
                //do a query by artist_name and by title
            }
            else if (!String.IsNullOrEmpty(artist_name) && !String.IsNullOrEmpty(title))
            {
                //do a query by artist_name and by title
            }*/
            //#################################
            //#################################
        }

        public void getArtistInfo(String artist, String ID)
        {
                 /* Viele Infos:
                 * http://developer.echonest.com/api/v4/artist/search?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=terms&bucket=id:facebook&bucket=artist_location&bucket=biographies&bucket=years_active&bucket=video&bucket=urls&bucket=blogs&bucket=reviews&bucket=images&bucket=news&bucket=discovery&sort=hotttnesss-desc&results=1&name=katy+perry
                 * FB-Seite:
                 * gibt "facebook:artist:6979332244" zurück, seite lautet dann http://www.facebook.com/profile.php?id=6979332244
                 */

            if (artist.Contains(" "))
            {
                artist = artist.Replace(" ", "+");
                Debug.WriteLine("fixed artist-spacing to: " + artist);
            }

            artist = artist.ToLower();

            String request = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=terms&bucket=id:facebook&bucket=artist_location&bucket=biographies&bucket=years_active&bucket=video&bucket=urls&bucket=blogs&bucket=reviews&bucket=images&bucket=news&sort=hotttnesss-desc&results=1&name=" + artist;

            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");

            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //'artists' durch 'ArtistInfos' ersetzen
            var regex = new Regex(Regex.Escape("artists"));
            var newText = regex.Replace(cleared, "ArtistInfos", 1);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //ID einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistInfos[0]);
            ArtistInfosRC.Add(temp.Response.ArtistInfos[0]);
            
            /* Liste der Songs:
             * http://developer.echonest.com/api/v4/artist/songs?api_key=L5WMCPOK4F2LA9H5X&format=json&results=100&name=katy+perry
             */

            String request2 = _defaultURL + "artist/songs?" + "api_key=" + GetAPIKey() + "&format=json&results=100&name=" + artist;

            //JSON response delivered as string
            String response2 = HttpRequester.StartRequest(request2);
            //transform "\'" to unicode equivalent
            response2 = response2.Replace("'", "&#39;");

            var cleared2 = @"" + response2.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //'songs' durch 'ArtistInfos\': [{\'ArtistSongs' ersetzen
            var regex2 = new Regex(Regex.Escape("songs"));
            var newText2 = regex2.Replace(cleared2, "ArtistInfos\': [{\'ArtistSongs", 1);
            newText2 = newText2.Insert(newText2.LastIndexOf("}") - 1, "}]");
            var regex3 = new Regex(Regex.Escape("id"));
            var newText3 = regex3.Replace(newText2, "title_id", 100);

            var temp2 = JsonConvert.DeserializeObject<ResponseContainer>(newText3);
            //Innere Liste initialisieren
            ArtistInfosRC[0].ArtistSongs = new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistSong>();

            for (int i = 0; i < temp2.Response.ArtistInfos[0].ArtistSongs.Count; i++)
            {
                ArtistInfosRC[0].ArtistSongs.Add(temp2.Response.ArtistInfos[0].ArtistSongs[i]);
                //ArtistInfosRC.ArtistSongs.Add(temp2.Response.ArtistSongs[i]);
            }

            /* Ähnliche Artisten (unsortiert!)
            * http://developer.echonest.com/api/v4/artist/similar?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=familiarity&min_familiarity=0.7&name=katy+perry
            * 
            * */
            String request3 = _defaultURL + "artist/similar?" + "api_key=" + GetAPIKey() + "&format=json&bucket=familiarity&min_familiarity=0.7&name=" + artist;

            //JSON response delivered as string
            String response3 = HttpRequester.StartRequest(request3);
            //transform "\'" to unicode equivalent
            response3 = response3.Replace("'", "&#39;");

            var cleared3 = @"" + response3.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //'artists' durch 'SimilarArtist' ersetzen
            var regex4 = new Regex(Regex.Escape("artists"));
            var newText4 = regex4.Replace(cleared3, "ArtistInfos\': [{\'SimilarArtists", 1);
            newText4 = newText4.Insert(newText4.LastIndexOf("}") - 1, "}]");
            var regex5 = new Regex(Regex.Escape("id"));
            var newText5 = regex5.Replace(newText4, "artist_id", 100);

            var temp3 = JsonConvert.DeserializeObject<ResponseContainer>(newText5);
            //Innere Liste initialisieren
            ArtistInfosRC[0].SimilarArtists = new List<ResponseContainer.ResponseObj.ArtistInfo.SimilarArtist>();

            for (int i = 0; i < temp3.Response.ArtistInfos[0].SimilarArtists.Count; i++)
            {
                ArtistInfosRC[0].SimilarArtists.Add(temp3.Response.ArtistInfos[0].SimilarArtists[i]);
                //ArtistInfosRC.ArtistSongs.Add(temp2.Response.ArtistSongs[i]);
            }
            ArtistInfosRC[0].SimilarArtists = ArtistInfosRC[0].SimilarArtists.OrderByDescending(a => a.familiarity).ToList();
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
        }

        public void getTitleSuggestions(int ID, String term)
        {
            /*Bsp URL:
             http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=roar
             * 
             * Mögliche Erweiterung falls Vorschläge nicht erwartungskonform sind:
             * # &fuzzy_match=true in URL einbauen
             * # &start=0(default),15,30,... einbauen um weitere (andere!) Ergebnisse abzurufen
             */
            if (term.Contains(" "))
            {
                term = term.Replace(" ", "+");
                Debug.WriteLine("fixed term-spacing to: " + term);
            }

            term = term.ToLower();

            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=" + term;

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
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            for (int i = 0; i < temp.Response.TitleSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.TitleSuggestions[i]);
                TitleSuggestionsRC.Add(temp.Response.TitleSuggestions[i]);
            }

        }

        public void getArtistSuggestions(int ID, String term)
        {
            /*Bsp URL:
             * http://developer.echonest.com/api/v4/artist/search?api_key=L5WMCPOK4F2LA9H5X&format=json&bucket=id:spotify-WW&limit=true&sort=hotttnesss-desc&name=emin
             * 
             * Mögliche Erweiterung falls Vorschläge nicht erwartungskonform sind:
             * # &fuzzy_match=true in URL einbauen
             * # &start=0(default),15,30,... einbauen um weitere (andere!) Ergebnisse abzurufen
             * 
            */
            if (term.Contains(" "))
            {
                term = term.Replace(" ", "+");
                Debug.WriteLine("fixed term-spacing to: " + term);
            }

            term = term.ToLower();

            String request = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&sort=hotttnesss-desc&name=" + term;

            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");

            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //'songs' durch 'TitleSuggestions' ersetzen
            var regex = new Regex(Regex.Escape("artists"));
            var newText = regex.Replace(cleared, "ArtistSuggestions", 1);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //ID einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            for (int i = 0; i < temp.Response.ArtistSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistSuggestions[i]);
                ArtistSuggestionsRC.Add(temp.Response.ArtistSuggestions[i]);
            }
        }
        
        public void SearchQuery(List<searchObjects> searchList)
        {
         
            //TEST-Liste auslesen
            for (int i = 0; i < searchListe.Count; i++)
            {
                if (!String.IsNullOrEmpty(searchListe[i].artist_id))
                {
                    Debug.WriteLine("\nFOUND ARTIST IN TESTLISTE: " + searchListe[i].artist_id + " at position: " + i);
                    SongsByArtistIDQuery(searchListe[i].artist_id, searchListe[i].originId);
                    
                }
                if (!String.IsNullOrEmpty(searchListe[i].title_id))
                {
                    Debug.WriteLine("\nFOUND TITLE IN TESTLISTE: " + searchListe[i].title_id + " at position: " + i);
                    SongsByTitleIDQuery(searchListe[i].title_id, searchListe[i].originId);
                    
                }
                if (!String.IsNullOrEmpty(searchListe[i].genre))
                {
                    Debug.WriteLine("\nFOUND GENRE IN TESTLISTE: " + searchListe[i].genre + " at position: " + i);
                    SongsByGenreQuery(searchListe[i].genre, searchListe[i].originId);

                }
            }
        }


        public void SongsByGenreQuery(String genre, int ID)
        {
            
            if (genre.Contains(" "))
            {
                genre = genre.Replace(" ", "+");
                Debug.WriteLine("fixed genre-spacing to: " + genre);
            }

            genre = genre.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&song_selection=song_hotttnesss-top&variety=1&type=genre-radio&genre=" + genre;

            //
            //WIEVIELE RESULTS ?????????????????????????
            //

            //Debug.WriteLine("genre-request URL = " + request);

            //_getGenre = true;
            LoadOnlineResponse(request, ID); //Send Query


            //Genre-Suche vertiefen (nice to have):
            //find similiar songs (or artists and then songs...) by those artists
            //bsp:
            //http://developer.echonest.com/docs/v4/genre.html#similar
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

            String request = _defaultURL + "song/profile?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&id=" + title_id;

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

            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist_id=" + artist_id;
            
            Debug.WriteLine("sending artist query...\n"+request);
            //############
            LoadOnlineResponse(request, ID); //Send Query
            //############

            //Assert.IsNull(_request); //TEST
        }


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
