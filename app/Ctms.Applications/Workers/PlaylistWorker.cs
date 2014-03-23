using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Applications.ViewModels;
using MusicStream;
using Ctms.Domain.Objects;
using SpotifySharp;
using Ctms.Applications.DataModels;

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

            _accountWorker.PlaylistSessionManagerCreated = PlaylistSessionManagerCreated;
        }

        //CALLBACK
        private void PlaylistSessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.PlaylistOpened = PlaylistOpened;
            _sessionManager.PlaybackStarted = PlaybackStarted;
            _sessionManager.PlaybackPaused = PlaybackPaused;
            _sessionManager.PlaybackStopped = PlaybackStopped;
            _sessionManager.PlaybackEndOfTrack = EndOfTrack;
            _sessionManager.PlaylistTrackRemoved = PlaylistTrackRemoved;
        }

        private void PlaylistOpened(Playlist playlist)
        {
            //toast for visual feedback

            //add existing tracks to playlistCollection binded to list
            _playlistViewModel.CurrentPlaylist = playlist;
            _playlistViewModel.ResultsForPlaylist.Clear();
            for (int i = 0; i < playlist.NumTracks(); i++)
            {
               _playlistViewModel.ResultsForPlaylist.Add(new ResultDataModel(playlist.Track(i).Name(), playlist.Track(i).Artist(0).Name(), playlist.Track(i)));
            }
        }

        private void PlaybackStarted()
        {
            _playlistViewModel.Playing = true;
        }

        private void PlaybackPaused()
        {
            _playlistViewModel.Playing = false;
        }

        private void PlaybackStopped()
        {
            _playlistViewModel.Playing = false;
        }

        private void EndOfTrack()
        {
            
        }

        private void PlaylistTrackRemoved(int index)
        {
            _playlistViewModel.ResultsForPlaylist.RemoveAt(index);
        }

        //PUBLIC METHODS
        public void AddTrackToPlaylist(ResultDataModel result)
        {
            if (_accountWorker.IsLoggedIn())
            {
                if (_playlistViewModel.CurrentPlaylist != null)
                {
                    for (var i = 0; i < _playlistViewModel.CurrentPlaylist.NumTracks(); i++)
                    {
                        if (_playlistViewModel.CurrentPlaylist.Track(i).Artist(0).Name().Equals(result.SpotifyTrack.Artist(0).Name()) && _playlistViewModel.CurrentPlaylist.Track(i).Name().Equals(result.SpotifyTrack.Name()))
                        {
                            return;
                        }
                        else
                        {

                        }
                    }
                    _playlistViewModel.ResultsForPlaylist.Add(result);
                    _sessionManager.AddTrackToPlaylist(_playlistViewModel.CurrentPlaylist, result.SpotifyTrack);
                }
                else
                {
                    //no playlist available
                    //Notify user to open or create playlist
                }
            }
            else
            {
                //user not logged in
                //Notify user to login to spotify
            }
        }

        public void JumpToTrack(int index)
        {
            _sessionManager.JumpToTrackInPlaylist(_playlistViewModel.CurrentPlaylist, index);
        }

        public void ReorderTrack(object[] data)
        {
            _sessionManager.ReorderTrack((int)data[0], (int)data[1]);
        }

        public void ToggleShuffle()
        {
            if (_playlistViewModel.IsShuffle)
            {
                _playlistViewModel.IsShuffle = false;
            }
            else
            {
                _playlistViewModel.IsShuffle = true;
            }

            _sessionManager.IsShuffle = _playlistViewModel.IsShuffle;
        }

        public void ToggleRepeat()
        {
            if (_playlistViewModel.IsRepeat)
            {
                _playlistViewModel.IsRepeat = false;
            }
            else
            {
                _playlistViewModel.IsRepeat = true;
            }

            _sessionManager.IsRepeat = _playlistViewModel.IsRepeat;
        }
    }
}
