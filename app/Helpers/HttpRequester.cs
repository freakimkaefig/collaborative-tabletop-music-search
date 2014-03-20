using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Helpers
{
    public class HttpRequester
    {

        //http://stackoverflow.com/questions/924679/c-sharp-how-can-i-check-if-a-address-exists-is-valid
        public static string StartRequest(string address)
        {
            var response = "";

            if (address == null)
            {
                throw new Exception();
            }

            // using MyClient from linked post
            using (var client = new MyClient())
            {
                try
                {
                    // throws 404 or other exception?
                    response = client.DownloadString(address);
                }
                catch (Exception ex)
                {
                    throw(ex);
                }
            }
            return response;
        }

        class MyClient : WebClient
        {
            public bool HeadOnly { get; set; }
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                if (HeadOnly && req.Method == "GET")
                {
                    req.Method = "HEAD";
                }
                return req;
            }
        }
    }
}
