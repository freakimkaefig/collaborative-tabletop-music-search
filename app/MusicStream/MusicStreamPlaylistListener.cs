using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace MusicStream
{
    class MusicStreamPlaylistListener: PlaylistListener
    {
        private MusicStreamSessionManager _sessionManager;

        public MusicStreamPlaylistListener(MusicStreamSessionManager sessionManager)
        {
            this._sessionManager = sessionManager;
        }

        public override void TracksAdded(Playlist pl, Track[] tracks, int position, object userdata)
        {
            base.TracksAdded(pl, tracks, position, userdata);
            //_sessionManager.logMessages.Enqueue("Spotify: TRACKS ADDED");
        }

        public override void PlaylistUpdateInProgress(Playlist pl, bool done, object userdata)
        {
            base.PlaylistUpdateInProgress(pl, done, userdata);
            //_sessionManager.logMessages.Enqueue("Spotify: PLAYLIST UPDATE IN PROGRESS - Done? " + done);
        }
    }
}
