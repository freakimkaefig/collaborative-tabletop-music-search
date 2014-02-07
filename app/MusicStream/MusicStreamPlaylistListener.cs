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

        public override void PlaylistStateChanged(Playlist pl, object userdata)
        {
            base.PlaylistStateChanged(pl, userdata);
        }

        public override void TrackCreatedChanged(Playlist pl, int position, User user, int when, object userdata)
        {
            base.TrackCreatedChanged(pl, position, user, when, userdata);
        }

        public override void TrackMessageChanged(Playlist pl, int position, string message, object userdata)
        {
            base.TrackMessageChanged(pl, position, message, userdata);
        }

        public override void TrackSeenChanged(Playlist pl, int position, bool seen, object userdata)
        {
            base.TrackSeenChanged(pl, position, seen, userdata);
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
