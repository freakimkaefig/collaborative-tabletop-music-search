﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Helpers;
using MusicSearch.ResponseObjects;
using Newtonsoft.Json;

namespace MusicSearch.Managers
{
    public class SearchManager
    {
        public ResponseContainer ResponseContainer;
        private String _defaultURL = "http://developer.echonest.com/api/v4/";
        private String  currentPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
        private String apiKey = null;

        //neue Instanz vom ResponseContainer für die Genres
        List<ResponseContainer.ResponseObj.genres> GenresRC = new List<ResponseContainer.ResponseObj.genres>();

        
        public SearchManager()
        {
            initGenresRC();
        }

        //###################################################
        //###################################################

        public void combinedSearchQuery(List<combinedSearchObjects> list)
        {
            foreach (combinedSearchObjects cso in list)
            {
                if (!String.IsNullOrEmpty(cso.artist_id))
                {
                    //combinedArtistQuery(c.originIds, c.artist_id, c.ArtistParameter);
                }
                else if (!String.IsNullOrEmpty(cso.genre))
                {
                    //combinedGenreQuery(cso.originIds, cso.genre, cso.GenreParameter);
                }
            }
        }

       /* public void combinedGenreQuery(List<int> IDs, String genre, List<GenreParameter> gp)
        {
            if (genre.Contains(" "))
            {
                genre = genre.Replace(" ", "+");
            }

            genre = genre.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&type=genre-radio&genre=" + genre;

            //Debug.WriteLine("genre-request URL = " + request);
            
            foreach (var prop in gp.GetType().GetProperties())
            {
                request += "&"+prop.Name.ToString()+"="+prop.GetValue(gp, null);

            }
            Debug.WriteLine("request: " + request);
        }*/

        //###################################################
        //###################################################

        public void initGenresRC()
        {
            var newpath = currentPath.Substring(0, currentPath.LastIndexOf("app")) + "app/MusicSearch/files/genres.txt";
            //var regex3 = new Regex(Regex.Escape("%20"));
            //var newText4 = regex3.Replace(newpath, " ", 100);
            var newText4 = StringHelper.replacePartialString(newpath, "%20", " ", 100);
            
            System.IO.StreamReader rdr = System.IO.File.OpenText(newText4);
            string reader = rdr.ReadToEnd();

            //reader = reader.Replace("'", "&#39;");
            var cleared = @"" + reader.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            for (int i = 0; i < temp.Response.Genres.Count; i++)
            {
                GenresRC.Add(temp.Response.Genres[i]);
            }
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

        public List<ResponseContainer.ResponseObj.ArtistInfo> getArtistInfo(String artist,String ID)
        {
            //neue Instanz vom ResponseContainer für die Infos des DetailViews pro Artist
            List<ResponseContainer.ResponseObj.ArtistInfo> ArtistInfosRC = new List<ResponseContainer.ResponseObj.ArtistInfo>();

           
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
            //var regex = new Regex(Regex.Escape("artists"));
            //var newText = regex.Replace(cleared, "ArtistInfos", 1);
            var newText = StringHelper.replacePartialString(cleared, "artists", "ArtistInfos", 1);

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
            //'songs' durch 'TitleSuggestions' ersetzen
            //var regex2 = new Regex(Regex.Escape("songs"));
            //var newText2 = regex2.Replace(cleared2, "ArtistInfos\': [{\'ArtistSongs", 1);
            var newText2 = StringHelper.replacePartialString(cleared2, "songs", "ArtistInfos\': [{\'ArtistSongs", 1);
            newText2 = newText2.Insert(newText2.LastIndexOf("}") - 1, "}]");
            //var regex3 = new Regex(Regex.Escape("id"));
            //var newText3 = regex3.Replace(newText2, "title_id", 100);
            var newText3 = StringHelper.replacePartialString(newText2, "id", "title_id", 100);
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
            //var regex4 = new Regex(Regex.Escape("artists"));
            //var newText4 = regex4.Replace(cleared3, "ArtistInfos\': [{\'SimilarArtists", 1);
            var newText4 = StringHelper.replacePartialString(cleared3, "artists", "ArtistInfos\': [{\'SimilarArtists", 1);
            newText4 = newText4.Insert(newText4.LastIndexOf("}") - 1, "}]");
            //var regex5 = new Regex(Regex.Escape("id"));
            //var newText5 = regex5.Replace(newText4, "artist_id", 100);
            var newText5 = StringHelper.replacePartialString(newText4, "id", "artist_id", 100);

            var temp3 = JsonConvert.DeserializeObject<ResponseContainer>(newText5);
            //Innere Liste initialisieren
            ArtistInfosRC[0].SimilarArtists = new List<ResponseContainer.ResponseObj.ArtistInfo.SimilarArtist>();

            for (int i = 0; i < temp3.Response.ArtistInfos[0].SimilarArtists.Count; i++)
            {
                ArtistInfosRC[0].SimilarArtists.Add(temp3.Response.ArtistInfos[0].SimilarArtists[i]);
                //ArtistInfosRC.ArtistSongs.Add(temp2.Response.ArtistSongs[i]);
            }
            ArtistInfosRC[0].SimilarArtists = ArtistInfosRC[0].SimilarArtists.OrderByDescending(a => a.familiarity).ToList();

            return ArtistInfosRC;
        }

        public void SuggestionQuery(String type, int ID, String term)
        {
            //Debug.WriteLine("suggestion query, nothing else...");

            if (type == "title")
            {
                getTitleSuggestions(ID, term);
            }
            if (type == "artist")
            {
                getArtistSuggestions(ID, term);
            }
        }

        public List<ResponseContainer.ResponseObj.TitleSuggestion> getTitleSuggestions(int ID, String term)
        {
            //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
            List<ResponseContainer.ResponseObj.TitleSuggestion> TitleSuggestionsRC = new List<ResponseContainer.ResponseObj.TitleSuggestion>();


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
            //var regex = new Regex(Regex.Escape("songs"));
            //var newText = regex.Replace(cleared, "TitleSuggestions", 1);
            var newText = StringHelper.replacePartialString(cleared, "songs", "TitleSuggestions", 1);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //ID einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            // clear previous suggestions for this origin
            //unnötig da bei methodenaufruf neue instanz generiert wird
            //TitleSuggestionsRC.RemoveAll(s => s.originId == ID);

            for (int i = 0; i < temp.Response.TitleSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.TitleSuggestions[i]);
                TitleSuggestionsRC.Add(temp.Response.TitleSuggestions[i]);
            }
            return TitleSuggestionsRC;
        }

        public List<ResponseContainer.ResponseObj.ArtistSuggestion> getArtistSuggestions(int ID, String term)
        {

            //neue Instanz vom ResponseContainer für die Vorschläge zur Artisten-Suche
            List<ResponseContainer.ResponseObj.ArtistSuggestion> ArtistSuggestionsRC = new List<ResponseContainer.ResponseObj.ArtistSuggestion>();

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
            //var regex = new Regex(Regex.Escape("artists"));
            //var newText = regex.Replace(cleared, "ArtistSuggestions", 1);
            var newText = StringHelper.replacePartialString(cleared, "artists", "ArtistSuggestions", 1);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //ID einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            // clear previous suggestions for this origin
            //unnötig, da bei Methodenaufruf neue Instanz generiert wird
            //ArtistSuggestionsRC.RemoveAll(s => s.originId == ID);

            for (int i = 0; i < temp.Response.ArtistSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistSuggestions[i]);
                ArtistSuggestionsRC.Add(temp.Response.ArtistSuggestions[i]);
            }
            return ArtistSuggestionsRC;
        }

        public List<ResponseContainer.ResponseObj.Song> SearchQuery(List<searchObjects> searchList)
        {
            //neue Instanz vom ResponseContainer
            List<ResponseContainer.ResponseObj.Song> SearchRC = new List<ResponseContainer.ResponseObj.Song>();

            if (searchList != null && searchList.Any())
            {
                //Liste auslesen
                for (int i = 0; i < searchList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(searchList[i].artist_id))
                    {
                        Debug.WriteLine("\nFOUND ARTIST_ID IN TESTLISTE: " + searchList[i].artist_id + " at position: " + i);
                        return SongsByArtistIDQuery(searchList[i].artist_id, searchList[i].originId, SearchRC);

                    }
                    else if (!String.IsNullOrEmpty(searchList[i].title_id))
                    {
                        Debug.WriteLine("\nFOUND TITLE_ID IN TESTLISTE: " + searchList[i].title_id + " at position: " + i);
                        return SongsByTitleIDQuery(searchList[i].title_id, searchList[i].originId, SearchRC);

                    }
                    else if (!String.IsNullOrEmpty(searchList[i].genre))
                    {
                        Debug.WriteLine("\nFOUND GENRE IN TESTLISTE: " + searchList[i].genre + " at position: " + i);
                        return SongsByGenreQuery(searchList[i].genre, searchList[i].originId, SearchRC);

                    }
                    
                }
            }
            return null;
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByGenreQuery(String genre, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {

            if (genre.Contains(" "))
            {
                genre = genre.Replace(" ", "+");
                Debug.WriteLine("fixed spacing of genre to: " + genre);
            }

            genre = genre.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&song_selection=song_hotttnesss-top&variety=1&type=genre-radio&genre=" + genre;

            //Debug.WriteLine("genre-request URL = " + request);

            return LoadOnlineResponse(request, ID, SearchRC); //Send Query


            //Genre-Suche vertiefen (nice to have):
            //find similiar songs (or artists and then songs...) by those artists
            //bsp:
            //http://developer.echonest.com/docs/v4/genre.html#similar
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByTitleIDQuery(String title_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            // check for further parameters like "hotttnesss"

            String request = _defaultURL + "song/profile?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&id=" + title_id;

            Debug.WriteLine("sending title query...\n" + request);
            //############
            return LoadOnlineResponse(request, ID, SearchRC); //Send Query
            //############
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByArtistIDQuery(String artist_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist_id=" + artist_id;

            Debug.WriteLine("sending artist query...\n" + request);
            //############
            return LoadOnlineResponse(request, ID, SearchRC); //Send Query
            //############

            //Assert.IsNull(_request); //TEST
        }


        private String GetAPIKey()
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                //XML auslesen
                XDocument doc = XDocument.Load(currentPath.Substring(0, currentPath.LastIndexOf("app")) + "app/MusicSearch/files/config.xml");
                XElement el = doc.Element("apikey");
                apiKey = (String)el;

                //Debug.WriteLine("apiKey: " + apiKey);
            }
            return apiKey;
        }

        public List<ResponseContainer.ResponseObj.Song> LoadOnlineResponse(String request, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC) //Send Query
        {
            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");

            //Debug.WriteLine("received&fixed response: "+response);

            return ParseResponse(response, ID, SearchRC);
        }

        public List<ResponseContainer.ResponseObj.Song> ParseResponse(String response, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + response.Replace("\"", "'");//Apostrophes are replaced by HTML unicode
            //var regex3 = new Regex(Regex.Escape("spotify-WW:track"));
            //var newText4 = regex3.Replace(cleared, "spotify:track", 1000);
            
            var newText4 = StringHelper.replacePartialString(cleared, "spotify-WW:track", "spotify:track" , 1000);
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText4);
            //originId einfügen, zwecks Rückschlüssen
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            for (int i = 0; i < temp.Response.Songs.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.Songs[i]);
                SearchRC.Add(temp.Response.Songs[i]);
            }
            return SearchRC;
        }

        public List<ResponseContainer.ResponseObj.genres> getGenres()
        {
            return GenresRC;
        }

        /*public List<ResponseContainer.ResponseObj.genres> getSubgenres()
        {

        }*/

        public List<String> getGenreSuggestions(String term)
        {
            //neue Instanz vom ResponseContainer für die GenreVorschläge
            //List<ResponseContainer.ResponseObj.genres> GenreSuggestionsRC = new List<ResponseContainer.ResponseObj.genres>();
            List<String> GenreSuggestions = new List<String>();

            foreach (ResponseContainer.ResponseObj.genres g in GenresRC)
            {
                if (g.genre_name.Contains(term))
                {
                    GenreSuggestions.Add(StringHelper.lowerToUpper(g.genre_name.ToString()));
                    //GenreSuggestionsRC.Add(c);
                }
                for(int i = 0; i<g.Subgenres.Count; i++)
                {
                    if(g.Subgenres[i].name.Contains(term))
                    {
                        GenreSuggestions.Add(StringHelper.lowerToUpper(g.Subgenres[i].name.ToString()));
                        //GenreSuggestionsRC.Add(c);
                    }
                }  
            }
            //foreach (ResponseContainer.ResponseObj.genres.subgenres x in GenresRC)
            return GenreSuggestions;
            //return GenreSuggestionsRC;
        }
    }

    public class searchObjects
    {
        public String artist_id { get; set; }
        public String title_id { get; set; }
        public String genre { get; set; }
        public int originId { get; set; }
    }
    
    public class combinedSearchObjects
    {
        public String artist_id { get; set; }
        public String genre { get; set; }
        public List<int> originIds { get; set; }

        //parameters...
        public List<ArtistParameter> ArtistParameter { get; set; }
        public List<GenreParameter> GenreParameter { get; set; }
    }
   
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
   
    public class GenreParameter
    {
        public String song_selection { get; set; }
        public double variety { get; set; }
        public String distribution { get; set; }
        public double adventurousness { get; set; }
    }
}
