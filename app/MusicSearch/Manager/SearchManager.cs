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

        public void Start()
        {
            //Test request
            mRequest = "http://developer.echonest.com/api/v4/song/search?api_key=L5WMCPOK4F2LA9H5X&style=rock&min_danceability=0.65&min_tempo=140&results=5";
            //JSON response delivered as string
            mResponse = HttpRequester.StartRequest(mRequest);

            ParseResponse();
        }

        public void ParseResponse()
        {
            //Continue with: http://james.newtonking.com/json/help/index.html
            //Escapes in string making problems?

            //Response response = JsonConvert.DeserializeObject<Response>(mResponse);
            Console.WriteLine("Git Source Control Provider Test!");

            Console.WriteLine("GitExtensions Test");
            Console.WriteLine("GitExtensions TestMichi");

            Console.WriteLine("Provoke merge conflict");
        }
    }
}
