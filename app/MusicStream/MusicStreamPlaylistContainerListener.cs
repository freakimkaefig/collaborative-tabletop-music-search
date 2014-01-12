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
    class MusicStreamPlaylistContainerListener : PlaylistContainerListener
    {
        private MusicStreamSessionManager _sessionManager;

        public MusicStreamPlaylistContainerListener(MusicStreamSessionManager musicStreamSessionManager)
        {
            _sessionManager = musicStreamSessionManager;
        }

        public override void PlaylistAdded(PlaylistContainer pc, Playlist playlist, int position, object userdata)
        {
            base.PlaylistAdded(pc, playlist, position, userdata);
            _sessionManager.logMessages.Enqueue("PLAYLIST ADDED: PLAYLIST ADDED");
        }

        public override void PlaylistRemoved(PlaylistContainer pc, Playlist playlist, int position, object userdata)
        {
            base.PlaylistRemoved(pc, playlist, position, userdata);
            _sessionManager.logMessages.Enqueue("PLAYLIST REMOVED: PLAYLIST REMOVED");
        }

        public override void PlaylistMoved(PlaylistContainer pc, Playlist playlist, int position, int new_position, object userdata)
        {
            base.PlaylistMoved(pc, playlist, position, new_position, userdata);
            _sessionManager.logMessages.Enqueue("PLAYLIST MOVED: PLAYLIST MOVED");
        }

        public override void ContainerLoaded(PlaylistContainer pc, object userdata)
        {
            base.ContainerLoaded(pc, userdata);
            _sessionManager.logMessages.Enqueue("CONTAINER LOADED: CONTAINER LOADED");
        }
    }
}
