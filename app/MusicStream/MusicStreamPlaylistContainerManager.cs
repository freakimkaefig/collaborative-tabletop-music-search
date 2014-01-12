using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace MusicStream
{
    class MusicStreamPlaylistContainerManager
    {
        private MusicStreamPlaylistContainerListener _playlistContainerListener;
        private MusicStreamSessionManager _sessionManager;
        private SpotifySession _session;
        private PlaylistContainer _container;

        public MusicStreamPlaylistContainerManager(MusicStreamSessionManager sessionManager, SpotifySession session)
        {
            _sessionManager = sessionManager;
            _session = session;

            _playlistContainerListener = new MusicStreamPlaylistContainerListener(_sessionManager);
        }

        public PlaylistContainer CreatePlaylistContainer()
        {
            _container = _session.Playlistcontainer();  //creates PlaylistContainer for SpotifySession
            _container.AddCallbacks(_playlistContainerListener, null);  //Adds listener for PlaylistContainer events

            return _container;
        }

        public Playlist AddNewPlaylist(string name)
        {
            Playlist playlist = _container.AddNewPlaylist(name);    //Adds new Playlist to PlaylistContainer

            return playlist;
        }
    }
}
