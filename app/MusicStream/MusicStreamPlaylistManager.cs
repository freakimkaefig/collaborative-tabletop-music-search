using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace MusicStream
{
    class MusicStreamPlaylistManager
    {
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamPlaylistListener _playlistListener;

        public MusicStreamPlaylistManager(MusicStreamSessionManager sessionManager, Playlist playlist)
        {
            // TODO: Complete member initialization
            this._sessionManager = sessionManager;

            _playlistListener = new MusicStreamPlaylistListener(_sessionManager);
            playlist.AddCallbacks(_playlistListener, null);
        }

        public void AddTracksToPlaylist(SpotifySession session, Playlist playlist, Track[] tracks)
        {
            playlist.AddTracks(tracks, playlist.NumTracks(), session);
        }

        public void RemoveTracksFromPlaylist(Playlist playlist, int[] tracks)
        {
            playlist.RemoveTracks(tracks);
        }

        public String GetPlaylistMetadata(Playlist playlist)
        {
            //Metadata in String packen und zurückgeben
            String name = playlist.Name();
            int numTracks = playlist.NumTracks();
            String description = playlist.GetDescription();

            return "Name: " + name + " | Description: " + description + " | Tracks: " + numTracks;
        }
    }
}
