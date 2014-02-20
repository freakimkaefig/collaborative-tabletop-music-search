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
    class PlaylistWorker
    {
        private BackgroundWorkHelper _backgroundWorkHelper;
        private PlaylistViewModel _playlistViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;

        [ImportingConstructor]
        public PlaylistWorker(PlaylistViewModel playlistViewModel, MusicStreamAccountWorker musicStreamAccountWorker)
        {
            this._playlistViewModel = playlistViewModel;
            this._accountWorker = musicStreamAccountWorker;

            _accountWorker.SessionManagerCreated = SessionManagerCreated;
        }

        //SETTER & GETTER
        

        //CALLBACKS
        private void SessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            this._sessionManager = sessionManager;

            _sessionManager.TrackLoaded = TrackLoaded;
            //_sessionManager.PlaybackStarted = PlaybackStarted;
            _sessionManager.PlaybackPaused = PlaybackPaused;
            _sessionManager.PlaybackStopped = PlaybackStopped;
            _sessionManager.PlaybackEndOfTrack = EndOfTrack;
        }

        private void TrackLoaded(Track track)
        {
            _sessionManager.logMessages.Enqueue("Track: '" + track.Artist(0).Name() + " - " + track.Name() + "' loaded.");
            _playlistViewModel.CurrentTrack = track;
            NotifyModel(true, false, false);
        }

        private void PlaybackStarted(object sender, RunWorkerCompletedEventArgs e)
        {
            NotifyModel(false, true, false);
        }

        private void PlaybackPaused()
        {
            NotifyModel(true, false, false);
        }

        private void PlaybackStopped()
        {
            NotifyModel(true, false, false);
        }

        private void EndOfTrack()
        {

        }

        //METHODS
        private void NotifyModel(bool canPlay, bool canPause, bool canStop)
        {
            _playlistViewModel.CanPlayTrack = canPlay;
            _playlistViewModel.CanPauseTrack = canPause;
            _playlistViewModel.CanStopPlayback = canStop;
        }
        public void PlayCurrentTrack()
        {
            //_sessionManager.PlayTrack(_playlistViewModel.CurrentTrack);
            _backgroundWorkHelper.DoInBackground(_sessionManager.PlayTrack, PlaybackStarted);
        }

        public void PauseCurrentTrack()
        {
            _sessionManager.PauseTrack();
        }

        public void StopPlayback()
        {
            _sessionManager.StopTrack();
        }
    }
}
