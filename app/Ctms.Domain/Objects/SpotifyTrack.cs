using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace Ctms.Domain.Objects
{
    class SpotifyTrack
    {
        public Track Track { get; set; }
        public string Artist
        {
            get
            {
                return Track.Artist(0).Name();
            }
        }
        public string Title
        {
            get
            {
                return Track.Name();
            }
        }
        public int Duration
        {
            get
            {
                return Track.Duration();
            }
        }
    }
}
