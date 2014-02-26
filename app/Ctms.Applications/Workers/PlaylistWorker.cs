using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using MusicStream;

namespace Ctms.Applications.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    class PlaylistWorker
    {
        private PlaylistViewModel _playlistViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;

        [ImportingConstructor]
        public PlaylistWorker(PlaylistViewModel playlistViewModel, MusicStreamAccountWorker accountWorker)
        {
            _playlistViewModel = playlistViewModel;
            _accountWorker = accountWorker;

            _accountWorker.SessionManagerCreated = SessionManagerCreated;
        }

        private void SessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public void CreatePlaylist()
        {

        }

        public void AddTrackToPlaylist()
        {

        }
    }
}
