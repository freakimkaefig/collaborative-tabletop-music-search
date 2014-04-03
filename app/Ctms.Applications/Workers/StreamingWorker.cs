using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
using Ctms.Applications.ViewModels;
using SpotifySharp;
using Helpers;
using System.ComponentModel;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    class StreamingWorker
    {
        private BackgroundWorkHelper _backgroundWorkHelper;
        private PlaylistViewModel _playlistViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;

        [ImportingConstructor]
        public StreamingWorker(PlaylistViewModel playlistViewModel, MusicStreamAccountWorker musicStreamAccountWorker)
        {
            this._playlistViewModel = playlistViewModel;
            _accountWorker = musicStreamAccountWorker;

            _accountWorker.StreamingSessionManagerCreated = StreamingSessionManagerCreated;
        }

        //SETTER & GETTER
        public bool CanStream() { return _playlistViewModel.ReadyForPlayback; }
        public bool Playing() { return _playlistViewModel.Playing; }
        

        //CALLBACKS
        private void StreamingSessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            this._sessionManager = sessionManager;

            //_sessionManager.GetLoadedTrackCompleted = GetLoadedTrackCompleted;
            _sessionManager.PrelistenStarted = PrelistenStarted;
            _sessionManager.PrelistenStopped = PrelistenStopped;
            
        }

        private void PrelistenStarted()
        {
            _playlistViewModel.Prelistening = true;
        }

        private void PrelistenStopped()
        {
            _playlistViewModel.Prelistening = false;
        }

        public void PlaylistPlayPause()
        {
            //Called when Button "Play/Pause" from PlaylistView clicked
            if (_playlistViewModel.Playing)
            {
                _sessionManager.PlaylistPause();
                _playlistViewModel.Playing = false;
            }
            else
            {
                _sessionManager.PlaylistPlay();
                _playlistViewModel.Playing = true;
            }
        }

        public void StopPlayback()
        {
            //Called when Button "Stop" from PlaylistView clicked
            _sessionManager.PlaylistStop();
        }

        //PUBLIC METHODS
        public void Prelisten(ResultDataModel result)
        {
            if (_playlistViewModel.Prelistening || _playlistViewModel.Playing)
            {
                if (result.SpotifyTrack == _sessionManager.CurrentPrelistenTrack)
                {
                    //_sessionManager.logMessages.Enqueue("PRELISTEN STOP!");
                    _sessionManager.StopPrelisteningTrack();
                    _sessionManager.StopTrack();
                }
                else
                {
                    //_sessionManager.logMessages.Enqueue("PRELISTEN STOP!");
                    _sessionManager.StopPrelisteningTrack();
                    _sessionManager.StopTrack();
                    //_sessionManager.logMessages.Enqueue("PRELISTEN START!");
                    _sessionManager.StartPrelisteningTrack(result.SpotifyTrack);
                }
            }
            else
            {
                //_sessionManager.logMessages.Enqueue("PRELISTEN START!");
                _sessionManager.StartPrelisteningTrack(result.SpotifyTrack);
            }
        }
    }
}
