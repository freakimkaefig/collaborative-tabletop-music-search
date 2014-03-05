using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace Ctms.Domain.Objects
{
    // Wrapper class for the SpotifySharp.Playlist class which has no constructor.
    public class SpotifyPlaylist
    {
        public Playlist Playlist { get; set; }
        public string Name
        {
            get
            {
                return Playlist.Name();
            }
            set
            {
                Playlist.Rename(value);
            }
        }
    }
}
