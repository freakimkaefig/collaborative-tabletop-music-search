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
        private ResultViewModel _resultViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;
        private InfoWorker _infoWorker;

        [ImportingConstructor]
        public StreamingWorker(PlaylistViewModel playlistViewModel, ResultViewModel resultViewModel, MusicStreamAccountWorker musicStreamAccountWorker, InfoWorker infoWorker)
        {
            this._playlistViewModel = playlistViewModel;
            this._resultViewModel = resultViewModel;
            _accountWorker = musicStreamAccountWorker;

            _resultViewModel.DropTargetLeft = _playlistViewModel.GetDropTargetLeft();
            _resultViewModel.DropTargetRight = _playlistViewModel.GetDropTargetRight();

            _resultViewModel.PlusImageLeft = _playlistViewModel.GetPlusImageLeft();
            _resultViewModel.PlusImageRight = _playlistViewModel.GetPlusImageRight();

            _accountWorker.StreamingSessionManagerCreated = StreamingSessionManagerCreated;

            _infoWorker = infoWorker;
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
            _sessionManager.PrelistenLoadingReady = PrelistenLoadingReady;
        }

        private void PrelistenStarted(Track track)
        {
            if (track == null)
            {
                _infoWorker.ShowCommonInfo("Error playing track", "Sorry, an error occured, please try again", "Ok");
            }
            else
            {
                _playlistViewModel.Prelistening = true;
                foreach (ResultDataModel result in _resultViewModel.Results)
                {
                    //Vergleiche Tracks ??? keine ahnung wie!
                    if (result.SpotifyTrack.Artist(0).Name() == track.Artist(0).Name() && result.SpotifyTrack.Name() == track.Name() && result.SpotifyTrack.Duration() == track.Duration())
                    {
                        result.IsLoading = true;
                    }
                    else
                    {
                        result.IsLoading = false;
                        result.IsPlaying = false;
                    }
                }
            }
        }

        private void PrelistenStopped()
        {
            _playlistViewModel.Prelistening = false;
            foreach (ResultDataModel result in _resultViewModel.Results)
            {
                result.IsLoading = false;
                result.IsPlaying = false;
            }
        }

        private void PrelistenLoadingReady(Track track)
        {
            foreach (ResultDataModel result in _resultViewModel.Results)
            {
                try {
                    if (result.SpotifyTrack.Artist(0) != null)
                    {
                        //Vergleiche Tracks ??? keine ahnung wie!
                        if (result.SpotifyTrack.Artist(0).Name() == track.Artist(0).Name() && result.SpotifyTrack.Name() == track.Name() && result.SpotifyTrack.Duration() == track.Duration())
                        {
                            result.IsLoading = false;
                            result.IsPlaying = true;
                        }
                        else
                        {
                            result.IsLoading = false;
                            result.IsPlaying = false;
                        }
                    }
                }
                catch (NullReferenceException e)
                {
                    _infoWorker.ShowCommonInfo("Streaming error", "Please try again.", "Ok");
                }
            }
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
            _playlistViewModel.Playing = false;
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
