using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helpers;
using Newtonsoft.Json;
using MusicSearch.ResponseObjects;
using Newtonsoft.Json.Linq;

namespace MusicSearch.Manager
{
    public class SearchManager
    {
        private String mRequest;
        private String mResponse;
        public ResponseContainer ResponseContainer;
        private bool mIsTest = false;

        public void Start()
        {
            RequestOffline();
            StartRequest();
            ParseResponse();

            var firstSong = ResponseContainer.response.songs.First();
            Console.WriteLine(firstSong.title);
            Console.WriteLine(firstSong.artist_name);
        }

        private void StartRequest()
        {
            if (mIsTest == false)
            {   //Do online query
                LoadOnlineResponse();
            }
            else
            {   //Load offline test response
                LoadTestResponse();
            }
        }

        private void RequestOffline()
        {
            mRequest = "http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&style=rock&min_danceability=0.65&min_tempo=140&results=5";
        }

        private void LoadOnlineResponse()
        {
            //JSON response delivered as string
            mResponse = HttpRequester.StartRequest(mRequest);
            mResponse = mResponse.Replace("'", "&#39;");
        }

        public void ParseResponse()
        {
            //http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?
            var cleared = @"" + mResponse.Replace("\"", "'");//Apostrophes are right now replaced by HTML unicode
            ResponseContainer = JsonConvert.DeserializeObject<ResponseContainer>(cleared);
        }

        public void LoadTestResponse()
        {
            mResponse = @"{'response': 
            {
            'status': 
                {'version': '4.2', 'code': 0, 'message': 'Success'}, 
            'songs': [
                    {'artist_id': 'AR1R1P51187B9A9AB6', 'id': 'SOKJAGG13134398A08', 'artist_name': 'The Reason', 'title': 'The Reason'}, 
                    {'artist_id': 'AR1R1P51187B9A9AB6', 'id': 'SOMLHVF137460C1EE7', 'artist_name': 'The Reason', 'title': 'Aint Nuthin'}, 
                    {'artist_id': 'AR1R1P51187B9A9AB6', 'id': 'SOUUVGW12D8578C479', 'artist_name': 'The Reason', 'title': 'Mirror, Mirror (Talkin Bout Me)'}, 
                    {'artist_id': 'ARS73SD1187B9971AF', 'id': 'SORXFOX1400C70B29F', 'artist_name': 'Home Grown', 'title': 'Cleopatras Dream'}, 
                    {'artist_id': 'ARS73SD1187B9971AF', 'id': 'SOXXCYS131343A079A', 'artist_name': 'Home Grown', 'title': 'Complicated'}]}}";

        }
    }
}
