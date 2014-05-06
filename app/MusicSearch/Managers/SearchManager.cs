using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Helpers;
using MusicSearch.Objects;
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

        //Dictionaries
        Dictionary<string, AttributeObj> combinedSearchArtistAttributes =
            new Dictionary<string, AttributeObj>();

        Dictionary<string, AttributeObj> combinedSearchGenreAttributes =
            new Dictionary<string, AttributeObj>();

        
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
        public List<ResponseContainer.ResponseObj.combinedQuery> combinedSearchQuery(List<combinedSearchObject> list)
        {
            foreach (combinedSearchObject cso in list)
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
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&results=100&" + "artist_id=" + artist_id;

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
                    if (propValue.ToString() == DateTime.Now.Year.ToString())
                    {
                        propValue = "present";
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
            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&results=100&type=genre-radio";

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
                    if (propValue.ToString() == DateTime.Now.Year.ToString())
                    {
                        propValue = "present";
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
            var filePath = "files/genres.txt";
            var genresText = StringHelper.replacePartialString(filePath, "%20", " ", 100);
            //open txt-file & read it
            System.IO.StreamReader rdr = System.IO.File.OpenText(genresText);
            string reader = rdr.ReadToEnd();

            var cleared = @"" + reader.Replace("\"", "'");
            
            var temp = JsonConvert.DeserializeObject<ResponseContainer>(cleared);

            #region prepare list and dictionaries

            for (int i = 0; i < temp.Response.Genres.Count; i++)
            {
                GenresRC.Add(temp.Response.Genres[i]);
            }
            foreach (var prop in typeof(ArtistParameter).GetProperties())
            {
                if (prop.Name == ArtistParameterTypes.max_tempo.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.max_tempo.ToString(), new AttributeObj() { max = 500, min = 0, description = "Max. Tempo(BPM)" });
                }
                else if (prop.Name == ArtistParameterTypes.min_tempo.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.min_tempo.ToString(), new AttributeObj() { max = 500, min = 0, description = "Min. Tempo(BPM)" });
                }
                else if (prop.Name == ArtistParameterTypes.artist_min_familiarity.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_min_familiarity.ToString(), new AttributeObj() { max = 1, min = 0, description = "Min. familiarity" });
                }
                else if (prop.Name == ArtistParameterTypes.artist_start_year_before.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_start_year_before.ToString(), new AttributeObj() { max = DateTime.Now.Year, min = 1970, description = "Start year before ..." });
                }
                else if (prop.Name == ArtistParameterTypes.artist_start_year_after.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_start_year_after.ToString(), new AttributeObj() { max = DateTime.Now.Year, min = 1970, description = "Start year after ..." });
                }
                else if (prop.Name == ArtistParameterTypes.artist_end_year_before.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_end_year_before.ToString(), new AttributeObj() { max = DateTime.Now.Year, min = 1970, description = "End year before ..." });
                }
                else if (prop.Name == ArtistParameterTypes.artist_end_year_after.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_end_year_after.ToString(), new AttributeObj() { max = DateTime.Now.Year, min = 1970, description = "End year after ..." });
                }
                else if (prop.Name == ArtistParameterTypes.song_min_hotttnesss.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.song_min_hotttnesss.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum hotttnesss" });
                }
                else if (prop.Name == ArtistParameterTypes.artist_min_hotttnesss.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.artist_min_hotttnesss.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum hotttnesss" });
                }
                else if (prop.Name == ArtistParameterTypes.min_danceability.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.min_danceability.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum danceability" });
                }
                else if (prop.Name == ArtistParameterTypes.min_energy.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.min_energy.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum energy" });
                }
                else if (prop.Name == ArtistParameterTypes.min_liveness.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.min_liveness.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum liveness" });
                }
                else if (prop.Name == ArtistParameterTypes.min_acousticness.ToString())
                {
                    combinedSearchArtistAttributes.Add(ArtistParameterTypes.min_acousticness.ToString(), new AttributeObj() { max = 1, min = 0, description = "Minimum acousticness" });
                }

            }
            foreach (var prop in typeof(GenreParameter).GetProperties())
            {
                if (prop.Name == GenreParameterTypes.variety.ToString())
                {
                    combinedSearchGenreAttributes.Add(GenreParameterTypes.variety.ToString(), new AttributeObj() { max = 1, min = 0, description = "Higher number = more variety in the artists" });
                }
                else if (prop.Name == GenreParameterTypes.distribution.ToString())
                {
                    combinedSearchGenreAttributes.Add(GenreParameterTypes.distribution.ToString(), new AttributeObj() { option1 = "focused", option2 = "wandering", description = "Controls distribution of artists" });
                }
                else if (prop.Name == GenreParameterTypes.adventurousness.ToString())
                {
                    combinedSearchGenreAttributes.Add(GenreParameterTypes.adventurousness.ToString(), new AttributeObj() { max = DateTime.Now.Year, min = 1970, description = "Trade between known & unknown music" });
                }
            }
            #endregion
        }

        /// <summary>
        /// GET ATTRIBUTES FOR A COMBINED SEARCH QUERY:
        /// call to receive the attributes that can be set by a user for a combined search query
        /// </summary>
        /// <param name="attributeType">must be "artist" or "genre" to receive the respective attributes
        /// that can be set by user for that kind of combined search query</param>
        /// <returns>returns a list containing all the possible attributes</returns>
        /// 
        public Dictionary<string, AttributeObj> getCombinedSearchAttributes(AttributeTypes attributeType)
        {
            //return prefetched lists according to parameter
            if (attributeType == AttributeTypes.Artist)
            {
                return combinedSearchArtistAttributes;
            }
            else if (attributeType == AttributeTypes.Genre)
            {
                return combinedSearchGenreAttributes;
            }
            return null;
        }


        /// <summary>
        /// GET DETAIL-INFO:
        /// collects Info about an artist by calling several queries
        /// </summary>
        /// <param name="artist_name">the name of the artist</param>
        /// <param name="artist_id">the echonest-ID of the artist</param>
        /// <param name="originID">ID of the tangible responsible for the call</param>
        /// <returns>a formatted Response Container filled with the collected info</returns>
        /// 
        public List<ResponseContainer.ResponseObj.ArtistInfo> getDetailInfo(String artist_name, String artist_id)
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
                return getArtistInfo(name);
            }
            else if (!String.IsNullOrEmpty(artist_name))
            {
                //directly do a query by artist_name
                return getArtistInfo(artist_name);
            }
            return null;
           }

        /// <summary>
        /// gathers infos about the artist. Hint: FB id anhängen, z.B.: "http://www.facebook.com/profile.php?id=6979332244"
        /// </summary>
        /// <param name="artist"></param>
        /// <returns>ArtistInfosRC</returns>
        /// 
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
            newText = StringHelper.replacePartialString(newText, "<span>", "", 10000);
            newText = StringHelper.replacePartialString(newText, "</span>", "", 10000);
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
        public List<ResponseContainer.ResponseObj.Song> SearchQuery(List<searchObject> searchList)
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

            String request = _defaultURL + "playlist/static?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&song_selection=song_hotttnesss-top&variety=1&type=genre-radio&results=100&genre=" + genre;

            return LoadOnlineResponse(request, ID, SearchRC); 
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByTitleIDQuery(String title_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            String request = _defaultURL + "song/profile?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&id=" + title_id;

            return LoadOnlineResponse(request, ID, SearchRC); 
        }


        public List<ResponseContainer.ResponseObj.Song> SongsByArtistIDQuery(String artist_id, int ID, List<ResponseContainer.ResponseObj.Song> SearchRC)
        {
           
            String request = _defaultURL + "song/search?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&bucket=audio_summary&bucket=song_hotttnesss&sort=song_hotttnesss-desc&results=100&" + "artist_id=" + artist_id;

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

            String JSONOriginId = "\'originIDs\': [\'" + ID + "\'], ";
            newText4 = StringHelper.replacePartialString(newText4, "\'title\'", JSONOriginId + "\'title\'", 1000);

            var temp = JsonConvert.DeserializeObject<ResponseContainer>(newText4);

            for (int i = 0; i < temp.Response.Songs.Count; i++)
            {
                //add Origin-ID and results to RC
                SearchRC.Add(temp.Response.Songs[i]);
            }
            return SearchRC;
        }


        #region GETTER
        /// <summary>
        /// GETTER-METHODS:
        /// used for frequently requested data
        /// </summary>
        
        public String getSpotifyId(String str)
        {
            String request = _defaultURL + "song/profile?" + "api_key=" + GetAPIKey() + "&format=json&bucket=id:spotify-WW&limit=true&bucket=tracks&id=" + str;
            String response = HttpRequester.StartRequest(request);
            response = response.Replace("'", "&#39;");

            string sub = response.Substring(response.IndexOf("spotify-WW:track:") + 17);
            int index = sub.IndexOf("\", \"id\":");
            string result = sub.Substring(0, index);

            return result;
        }

        private String GetAPIKey()
        {
            if (String.IsNullOrEmpty(apiKey))
            {
                //open&read xml-file
                XDocument doc = XDocument.Load("files/config.xml");
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

        
        #endregion


    } //end of SearchManager class

    #region inherent classes
    /// <summary>
    /// Inherent classes, needed here and there
    /// </summary>
    /// 
    public class searchObject
    {
        public String artist_id { get; set; }
        public String title_id { get; set; }
        public String genre { get; set; }
        public int originId { get; set; }
    }

    public class combinedSearchObject
    {
        public String artist_id { get; set; }
        public String[] genre { get; set; }
        public List<int> originIds { get; set; }

        //lists with the parameters...
        public List<ArtistParameter> ArtistParameter { get; set; }
        public List<GenreParameter> GenreParameter { get; set; }
    }
    
    public class ArtistParameter
    {
        //of this class...
        public double max_tempo { get; set; }
        public double min_tempo { get; set; }
        public double artist_min_familiarity { get; set; }
        //the following Strings need to be Strings, because of their max-value "present"
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
        public double variety { get; set; }
        public String distribution { get; set; }
        public double adventurousness { get; set; }
    }

    #endregion

} // end of namespace
