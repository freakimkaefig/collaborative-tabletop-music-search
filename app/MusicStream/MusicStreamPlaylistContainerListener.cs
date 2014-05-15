using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace MusicStream
{
    /// <summary>
    /// Listener for events of PlaylistContainer
    /// </summary>
    public class MusicStreamPlaylistContainerListener : PlaylistContainerListener
    {
        private MusicStreamSessionManager _sessionManager;

        public MusicStreamPlaylistContainerListener(MusicStreamSessionManager musicStreamSessionManager)
        {
            _sessionManager = musicStreamSessionManager;
        }

        public override void PlaylistAdded(PlaylistContainer pc, Playlist playlist, int position, object userdata)
        {
            base.PlaylistAdded(pc, playlist, position, userdata);
            _sessionManager.PlaylistAddedCallback(playlist);
            _sessionManager.Userdata = userdata;
        }

        public override void PlaylistRemoved(PlaylistContainer pc, Playlist playlist, int position, object userdata)
        {
            base.PlaylistRemoved(pc, playlist, position, userdata);
        }

        public override void PlaylistMoved(PlaylistContainer pc, Playlist playlist, int position, int new_position, object userdata)
        {
            base.PlaylistMoved(pc, playlist, position, new_position, userdata);
        }

        public override void ContainerLoaded(PlaylistContainer pc, object userdata)
        {
            _sessionManager.PlaylistContainerLoadedCallback();
        }
    }
}
