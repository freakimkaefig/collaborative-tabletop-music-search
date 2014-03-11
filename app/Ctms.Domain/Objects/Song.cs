using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace Ctms.Domain.Objects
{
    public class Song
    {
        public string SpotifyId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }

        public string ArtistId { get; set; }
        public int Duration { get; set; }
    }
}
