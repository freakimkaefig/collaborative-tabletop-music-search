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

        private string _testtrack = "spotify:track:4lCv7b86sLynZbXhfScfm2";

        [ImportingConstructor]
        public StreamingWorker(PlaylistViewModel playlistViewModel, MusicStreamAccountWorker musicStreamAccountWorker)
        {
            this._playlistViewModel = playlistViewModel;
            this._accountWorker = musicStreamAccountWorker;

            _accountWorker.SessionManagerCreated = SessionManagerCreated;
        }

        //SETTER & GETTER
        public bool CanStream() { return _playlistViewModel.ReadyForPlayback; }
        public bool Playing() { return _playlistViewModel.Playing; }
        

        //CALLBACKS
        private void SessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            this._sessionManager = sessionManager;

            //_sessionManager.GetLoadedTrackCompleted = GetLoadedTrackCompleted;
            _sessionManager.PlaybackStarted = PlaybackStarted;
            _sessionManager.PlaybackPaused = PlaybackPaused;
            _sessionManager.PlaybackStopped = PlaybackStopped;
            _sessionManager.PlaybackEndOfTrack = EndOfTrack;
        }

        private void PlaybackStarted()
        {
            _playlistViewModel.Playing = true;
        }

        private void PlaybackPaused()
        {
            
        }

        private void PlaybackStopped()
        {
            
        }

        private void EndOfTrack()
        {

        }

        public void PlayCurrentTrack()
        {
           //Called when Button "Play" from PlaylistView clicked
        }

        public void PauseCurrentTrack()
        {
            //Called when Button "Pause" from PlaylistView clicked
        }

        public void StopPlayback()
        {
            //Called when Button "Stop" from PlaylistView clicked
        }

        //PUBLIC METHODS
        public void Prelisten()
        {
            //Called when Button "Prelisten" from ResultView clicked
            _sessionManager.StartPrelisteningTrack(_testtrack);
        }
    }
}
