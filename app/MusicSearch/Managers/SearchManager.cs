using System;
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
    /// <summary>
    /// SearchManager:
    /// contains methods to fulfill several http-requests to echonest, format the received response
    /// and finally return the resulting Response Containers
    /// </summary>
    public class SearchManager
    {
        public ResponseContainer ResponseContainer;
        private String _defaultURL = "http://developer.echonest.com/api/v4/";
        private String  currentPath = (new System.Uri(Assembly.GetExecutingAssembly().CodeBase)).AbsolutePath;
        private String apiKey = null;

        List<ResponseContainer.ResponseObj.genres> GenresRC = new List<ResponseContainer.ResponseObj.genres>();

        List<String> combinedSearchArtistAttributes = new List<String>();
        List<String> combinedSearchGenreAttributes = new List<String>();

        
        public SearchManager()
        {
            init();
        }


        /// <summary>
        /// COMBINED SEARCH QUERY:
        /// traverses the comitted list to call respective methods that either do a combined-artist-query
        /// or a combined-genre-query
        /// </summary>
        /// <param name="list">list containing the searchobjects & their attributes 
        /// according to the specified class</param>
        /// <returns>returns a Response Container with the data fetched from echonest</returns>
        public List<ResponseContainer.ResponseObj.combinedQuery> combinedSearchQuery(List<combinedSearchObjects> list)
        {
            foreach (combinedSearchObjects cso in list)
            {
                if (!String.IsNullOrEmpty(cso.artist_id))
                {
                    return combinedArtistQuery(cso.originIds, cso.artist_id, cso.ArtistParameter);
                }
                else if (!String.IsNullOrEmpty(cso.genre[0].ToString()))
                {
                    return combinedGenreQuery(cso.originIds, cso.genre, cso.GenreParameter);
                }
            }
            return null;
        }

        public List<ResponseContainer.ResponseObj.combinedQuery> combinedArtistQuery(List<int> IDs, String artist_id, List<ArtistParameter> ap)
        {
            List<ResponseContainer.ResponseObj.combinedQuery> combinedArtistRC = new List<ResponseContainer.ResponseObj.combinedQuery>();

            //basic URL
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&" + "artist_id=" + artist_id;

            //get & add attributes to combined-search-URL
            var properties = ap[0].GetType().GetProperties();
            foreach (var prop in properties)
            {
                string name = prop.Name;
                var propValue = prop.GetValue(ap[0], null);
                if (propValue != null && propValue.ToString() != "0.0" && propValue.ToString() != "0")
                {
                    if (propValue.ToString().Contains(","))
                    {
                        propValue = StringHelper.replacePartialString(propValue.ToString(), ",", ".", 1);
                    }
                    request += "&" + name + "=" + propValue.ToString();

                }
            }

            Debug.WriteLine("request: " + request);
            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            if (String.IsNullOrEmpty(response))
            {
                return null;
            }
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response to receive results in RC
            var newText = StringHelper.replacePartialString(cleared, "songs", "CombinedArtists", 1);
            //Add Origin-IDs to each result
            String OriginIDS = "\'originIDs\': [";

            for (int i = 0; i < IDs.Count; i++)
            {
                if (i < IDs.Count - 1)
                {
                    OriginIDS += "\'" + IDs[i] + "\', ";
                }
                else
                {
                    OriginIDS += "\'" + IDs[i] + "\'";
                }

            }
            OriginIDS += "], ";
            newText = StringHelper.replacePartialString(newText, "\'title\'", OriginIDS + "\'title\'", 1000);

            //convert response (JSON) in RC-instance
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            for (int i = 0; i < temp.Response.combinedQueries.Count; i++)
            {
                combinedArtistRC.Add(temp.Response.combinedQueries[i]);
            }
            return combinedArtistRC;
        }


        public List<ResponseContainer.ResponseObj.combinedQuery> combinedGenreQuery(List<int> IDs, String[] genre, List<GenreParameter> gp)
        {
            List<ResponseContainer.ResponseObj.combinedQuery> combinedGenreRC = new List<ResponseContainer.ResponseObj.combinedQuery>();

            //basic URL
            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&type=genre-radio";

            //Add genre(s) to URL
            for (int i = 0; i < genre.Length; i++)
            {
                if (genre[i].Contains(" "))
                {
                    genre[i] = genre[i].Replace(" ", "+");
                }

                genre[i] = genre[i].ToLower();
                request += "&genre=" + genre[i].ToString();
            }

            //get & add attributes to combined-search-URL by using reflection
            var properties = gp[0].GetType().GetProperties();
            foreach (var prop in properties)
            {
                string name = prop.Name;
                var propValue = prop.GetValue(gp[0], null);
                if (propValue != null && propValue.ToString() != "0.0" && propValue.ToString() != "0")
                {
                    //check if values are correctly formated, if not fix them
                    if (propValue.ToString().Contains(","))
                    {
                        propValue = StringHelper.replacePartialString(propValue.ToString(), ",", ".", 1);
                    }
                    request += "&" + name + "=" + propValue.ToString();

                }
            }

            //JSON response delivered as string
            String response = HttpRequester.StartRequest(request);
            if (String.IsNullOrEmpty(response))
            {
                return null;
            }
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response to receive results in RC
            var newText = StringHelper.replacePartialString(cleared, "songs", "CombinedGenres", 1);
            //Add Origin-IDs to each result
            String OriginIDS = "\'originIDs\': [";

            for (int i = 0; i < IDs.Count; i++)
            {
                if (i < IDs.Count - 1)
                {
                    OriginIDS += "\'" + IDs[i] + "\', ";
                }
                else
                {
                    OriginIDS += "\'" + IDs[i] + "\'";
                }

            }
            OriginIDS += "], ";
            newText = StringHelper.replacePartialString(newText, "\'title\'", OriginIDS + "\'title\'", 1000);

            //convert response (JSON) in RC-instance
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            for (int i = 0; i < temp.Response.combinedQueries.Count; i++)
            {
                combinedGenreRC.Add(temp.Response.combinedQueries[i]);
            }

            return combinedGenreRC;
        }


        /// <summary>
        /// initialises a list of genres and the combined-search-lists containing the respective attributes
        /// </summary>
        public void init()
        {
            //absolute path to genre.txt
            //!!var lastAppIndex = currentPath.LastIndexOf("app");
            //!!var newpath = currentPath.Substring(0, currentPath.LastIndexOf("app")) + "app/MusicSearch/files/genres.txt";
            var filePath = "files/genres.txt";
            var genresText = StringHelper.replacePartialString(filePath, "%20", " ", 100);
            //open txt-file & read it
            System.IO.StreamReader rdr = System.IO.File.OpenText(genresText);
            string reader = rdr.ReadToEnd();

            var cleared = @"" + reader.Replace("\"", "'");
            
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            for (int i = 0; i < temp.Response.Genres.Count; i++)
            {
                GenresRC.Add(temp.Response.Genres[i]);
            }
            foreach (var prop in typeof(ArtistParameter).GetProperties())
            {
                string name = prop.Name;

                combinedSearchArtistAttributes.Add(name);
            }
            foreach (var prop in typeof(GenreParameter).GetProperties())
            {
                string name = prop.Name;

                combinedSearchGenreAttributes.Add(name);
            }
        }


        /// <summary>
        /// GET ATTRIBUTES FOR A COMBINED SEARCH QUERY:
        /// call to receive the attributes that can be set by a user for a combined search query
        /// </summary>
        /// <param name="artistORgenre">must be "artist" or "genre" to receive the respective attributes
        /// that can be set by user for that kind of combined search query</param>
        /// <returns>returns a list containing all the possible attributes</returns>
        /// 
        public List<String> getCombinedSearchAttributes(String artistORgenre)
        {
            //return prefetched lists according to parameter
            if (artistORgenre == "artist")
            {
                return combinedSearchArtistAttributes;
            }
            else if (artistORgenre == "genre")
            {
                return combinedSearchGenreAttributes;
            }
            return null;
        }


        // <summary>
        /// GET DETAIL-INFO:
        /// collects Info about an artist by calling several queries
        /// </summary>
        /// <param name="artist_name">the name of the artist</param>
        /// <param name="artist_id">the echonest-ID of the artist</param>
        /// <param name="originID">ID of the tangible responsible for the call</param>
        /// <returns>a formatted Response Container filled with the collected info</returns>
        /// 
        public void getDetailInfo(String artist_name, String artist_id, String originID)
        {
            //check if artist-name or artist-id is available
            if (!String.IsNullOrEmpty(artist_id))
            {
                //get name of the artist by id
                String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&artist_id=" + artist_id + "&results=1";
                
                String response = HttpRequester.StartRequest(request);
                //split response into array
                String[] splitted = response.Split(new Char[] { '\"' });
                //name of artist is at fixed position in array
                String name = splitted[27].ToString();

                //do a query by that artist-name
                getArtistInfo(name, originID);
            }
            else if (!String.IsNullOrEmpty(artist_name))
            {
                //directly do a query by artist_name
                getArtistInfo(artist_name, originID);
            }
           }


        public List<ResponseContainer.ResponseObj.ArtistInfo> getArtistInfo(String artist,String ID)
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
            String request = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=terms&bucket=id:facebook&bucket=artist_location&bucket=biographies&bucket=years_active&bucket=video&bucket=urls&bucket=blogs&bucket=reviews&bucket=images&bucket=news&sort=hotttnesss-desc&results=1&name=" + artist;
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
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //add Origin-IDs
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistInfos[0]);
            //add first artist-info-results to RC
            ArtistInfosRC.Add(temp.Response.ArtistInfos[0]);
            
            //build 2nd query (songs of the artist)
            String request2 = _defaultURL + "artist/songs?" + "api_key=" + GetAPIKey() + "&format=json&results=100&name=" + artist;
            String response2 = HttpRequester.StartRequest(request2);
            if (String.IsNullOrEmpty(response))
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
            //Initialise first inner list of RC
            ArtistInfosRC[0].ArtistSongs = new List<ResponseContainer.ResponseObj.ArtistInfo.ArtistSong>();
            //add further artist-info-results to inner list of RC
            for (int i = 0; i < temp2.Response.ArtistInfos[0].ArtistSongs.Count; i++)
            {
                ArtistInfosRC[0].ArtistSongs.Add(temp2.Response.ArtistInfos[0].ArtistSongs[i]);
            }

            //build 3rd query (similiar artists)
            String request3 = _defaultURL + "artist/similar?" + "api_key=" + GetAPIKey() + "&format=json&bucket=familiarity&min_familiarity=0.7&name=" + artist;
            String response3 = HttpRequester.StartRequest(request3);
            if (String.IsNullOrEmpty(response))
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
            //Initialise second inner list of RC
            ArtistInfosRC[0].SimilarArtists = new List<ResponseContainer.ResponseObj.ArtistInfo.SimilarArtist>();
            //add remaining artist-info-results to second inner list of RC
            for (int i = 0; i < temp3.Response.ArtistInfos[0].SimilarArtists.Count; i++)
            {
                ArtistInfosRC[0].SimilarArtists.Add(temp3.Response.ArtistInfos[0].SimilarArtists[i]);
            }
            //order results of second inner list descending by familiarity
            ArtistInfosRC[0].SimilarArtists = ArtistInfosRC[0].SimilarArtists.OrderByDescending(a => a.familiarity).ToList();

            return ArtistInfosRC;
        }

        
        /// <summary>
        /// SUGGESTION-QUERIES:
        /// look for suggestions based on comitted partial strings to support user
        /// </summary>
        /// <param name="ID">ID of the tangible</param>
        /// <param name="term">partial string on which suggestions will be based</param>
        /// <returns>suggestions deliverd by echonest or null</returns>
        /// 
        public List<ResponseContainer.ResponseObj.TitleSuggestion> getTitleSuggestions(int ID, String term)
        {
            List<ResponseContainer.ResponseObj.TitleSuggestion> TitleSuggestionsRC = new List<ResponseContainer.ResponseObj.TitleSuggestion>();

            //fix spacing & upper-case letters
            if (term.Contains(" "))
            {
                term = term.Replace(" ", "+");
            }
            term = term.ToLower();

            //build query
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&sort=song_hotttnesss-desc&title=" + term;
            
            String response = HttpRequester.StartRequest(request);
            if (String.IsNullOrEmpty(response))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response to receive correct results in RC
            var newText = StringHelper.replacePartialString(cleared, "songs", "TitleSuggestions", 1);
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //Add Origin-ID and add results to RC
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            for (int i = 0; i < temp.Response.TitleSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.TitleSuggestions[i]);
                TitleSuggestionsRC.Add(temp.Response.TitleSuggestions[i]);
            }

            if (TitleSuggestionsRC != null && !TitleSuggestionsRC.Any())
            {
                var emptyResponse = new ResponseContainer.ResponseObj.TitleSuggestion();
                emptyResponse.originId = ID;
                TitleSuggestionsRC.Add(emptyResponse);
            }

            return TitleSuggestionsRC;
        }


        /// <summary>
        /// SUGGESTION-QUERIES:
        /// look for suggestions based on comitted partial strings to support user
        /// </summary>
        /// <param name="ID">ID of the tangible responsible for the call</param>
        /// <param name="term">partial string, that suggestions will be based on</param>
        /// <returns>artist-suggestions delivered by echonest or null</returns>
        /// 
        public List<ResponseContainer.ResponseObj.ArtistSuggestion> getArtistSuggestions(int ID, String term)
        {
            List<ResponseContainer.ResponseObj.ArtistSuggestion> ArtistSuggestionsRC = new List<ResponseContainer.ResponseObj.ArtistSuggestion>();

            //fix spacing and upper-case letters
            if (term.Contains(" "))
            {
                term = term.Replace(" ", "+");
                Debug.WriteLine("fixed term-spacing to: " + term);
            }
            term = term.ToLower();

            String request = _defaultURL + "artist/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&sort=hotttnesss-desc&name=" + term;
            
            String response = HttpRequester.StartRequest(request);
            if (String.IsNullOrEmpty(response))
            {
                return null;
            }
            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response to receive correct results in RC
            var newText = StringHelper.replacePartialString(cleared, "artists", "ArtistSuggestions", 1);
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText);
            //Add Origin-ID and results to RC
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";
            for (int i = 0; i < temp.Response.ArtistSuggestions.Count; i++)
            {
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.ArtistSuggestions[i]);
                ArtistSuggestionsRC.Add(temp.Response.ArtistSuggestions[i]);
            }

            if (ArtistSuggestionsRC != null && !ArtistSuggestionsRC.Any())
            {
                var emptyResponse = new ResponseContainer.ResponseObj.ArtistSuggestion();
                emptyResponse.originId = ID;
                ArtistSuggestionsRC.Add(emptyResponse);
            }

            return ArtistSuggestionsRC;
        }


        /// <summary>
        /// SEARCH-QUERIES:
        /// traverses the searchlist to call the appropriate method which formulates the query and returns 
        /// the formated response in a prepared ResponseContainer (see class for details)
        /// </summary>
        /// <param name="searchList">contains data specified by user input via tangibles</param>
        /// <returns> 1 Response Container or null per searchlist-entry </returns>
        /// 
        public List<ResponseContainer.ResponseObj.Song> SearchQuery(List<searchObjects> searchList)
        {
            List<ResponseContainer.ResponseObj.Song> SearchRC = new List<ResponseContainer.ResponseObj.Song>();

            if (searchList != null && searchList.Any())
            {
                //traverse list and call respective methods
                for (int i = 0; i < searchList.Count; i++)
                {
                    if (!String.IsNullOrEmpty(searchList[i].artist_id))
                    {
                        return SongsByArtistIDQuery(searchList[i].artist_id, searchList[i].originId, SearchRC);

                    }
                    else if (!String.IsNullOrEmpty(searchList[i].title_id))
                    {
                       return SongsByTitleIDQuery(searchList[i].title_id, searchList[i].originId, SearchRC);

                    }
                    else if (!String.IsNullOrEmpty(searchList[i].genre))
                    {
                        return SongsByGenreQuery(searchList[i].genre, searchList[i].originId, SearchRC);

                    }
                }
            }
            return null;
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByGenreQuery(String genre, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
            //fix spacing and upper-case letters
            if (genre.Contains(" "))
            {
                genre = genre.Replace(" ", "+");
            }
            genre = genre.ToLower();

            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&song_selection=song_hotttnesss-top&variety=1&type=genre-radio&genre=" + genre;

            return LoadOnlineResponse(request, ID, SearchRC); 
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByTitleIDQuery(String title_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            String request = _defaultURL + "song/profile?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&id=" + title_id;

            return LoadOnlineResponse(request, ID, SearchRC); 
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByArtistIDQuery(String artist_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&" + "artist_id=" + artist_id;

            return LoadOnlineResponse(request, ID, SearchRC); 
        }


        public List<ResponseContainer.ResponseObj.Song> LoadOnlineResponse(String request, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
            String response = HttpRequester.StartRequest(request);

            if (String.IsNullOrEmpty(response))
            {
                return null;
            }

            //transform "\'" to unicode equivalent
            response = response.Replace("'", "&#39;");
            return ParseResponse(response, ID, SearchRC);
        }


        public List<ResponseContainer.ResponseObj.Song> ParseResponse(String response, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
            //Apostrophes are replaced by HTML unicode
            var cleared = @"" + response.Replace("\"", "'");
            //manipulate response for correct formatting
            var newText4 = StringHelper.replacePartialString(cleared, "spotify-WW:track", "spotify:track" , 1000);
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText4);
            String JSONOriginId = "{\"originId\": \"" + ID + "\"}";

            for (int i = 0; i < temp.Response.Songs.Count; i++)
            {
                //add Origin-ID and results to RC
                JsonConvert.PopulateObject(JSONOriginId, temp.Response.Songs[i]);
                SearchRC.Add(temp.Response.Songs[i]);
            }
            return SearchRC;
        }
        

        /// <summary>
        /// GETTER-METHODS:
        /// used for frequently requested data
        /// </summary>
        /// <returns>each method returns often used data</returns>
        /// 
        private String GetAPIKey()
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                //open&read xml-file
                XDocument doc = XDocument.Load(currentPath.Substring(0, currentPath.LastIndexOf("app")) + "app/MusicSearch/files/config.xml");
                XElement el = doc.Element("apikey");
                apiKey = (String)el;
            }
            return apiKey;
        }

        public List<ResponseContainer.ResponseObj.genres> getGenres()
        {
            return GenresRC;
        }

        public List<ResponseContainer.ResponseObj.genres.subgenres> getSubgenres(String topLevelGenre)
        {
            //return subgenres of first matching genre (there are no duplicate genres)
            return getGenres().FirstOrDefault(g => g.genre_name == topLevelGenre).Subgenres;
        }

        public List<String> getGenreSuggestions(String term)
        {
            List<String> GenreSuggestions = new List<String>();
            foreach (ResponseContainer.ResponseObj.genres g in GenresRC)
            {
                if (g.genre_name.Contains(term))
                {
                    GenreSuggestions.Add(StringHelper.lowerToUpper(g.genre_name.ToString()));
                }
                for(int i = 0; i<g.Subgenres.Count; i++)
                {
                    if(g.Subgenres[i].name.Contains(term))
                    {
                        GenreSuggestions.Add(StringHelper.lowerToUpper(g.Subgenres[i].name.ToString()));
                    }
                }  
            }
            return GenreSuggestions;
        }
    }


    /// <summary>
    /// Inherent classes, needed here and there
    /// </summary>
    /// 
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
        public String[] genre { get; set; }
        public List<int> originIds { get; set; }

        //containing parameters...
        public List<ArtistParameter> ArtistParameter { get; set; }
        public List<GenreParameter> GenreParameter { get; set; }
    }
    
    public class ArtistParameter
    {
        //of this class...
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
        //and this.
        public String song_selection { get; set; }
        public double variety { get; set; }
        public String distribution { get; set; }
        public double adventurousness { get; set; }
    }
}
