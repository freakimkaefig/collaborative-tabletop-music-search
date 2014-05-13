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
using System.Windows.Media.Animation;
using System.Windows;
using System.Diagnostics;
using System.Threading;

namespace Ctms.Applications.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    class PlaylistWorker
    {
        private PlaylistViewModel _playlistViewModel;
        private ResultViewModel _resultViewModel;
        private MusicStreamAccountWorker _accountWorker;
        private InfoWorker _infoWorker;

        private MusicStreamSessionManager _sessionManager;

        DoubleAnimation fadeIn = new DoubleAnimation() { From = 0, To = 1, Duration = TimeSpan.FromSeconds(1) };
        DoubleAnimation fadeOut = new DoubleAnimation() { From = 1, To = 0, Duration = TimeSpan.FromSeconds(2) };

        [ImportingConstructor]
        public PlaylistWorker(PlaylistViewModel playlistViewModel, ResultViewModel resultViewModel, MusicStreamAccountWorker accountWorker, InfoWorker infoWorker)
        {
            _playlistViewModel = playlistViewModel;
            _resultViewModel = resultViewModel;
            _accountWorker = accountWorker;
            _infoWorker = infoWorker;

            _accountWorker.PlaylistSessionManagerCreated = PlaylistSessionManagerCreated;
        }

        //CALLBACKS
        private void PlaylistSessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.PlaylistOpened = PlaylistOpened;
            _sessionManager.PlaybackStarted = PlaybackStarted;
            _sessionManager.PlaybackPaused = PlaybackPaused;
            _sessionManager.PlaybackStopped = PlaybackStopped;
            _sessionManager.PlaybackEndOfTrack = EndOfTrack;
            _sessionManager.PlaylistTrackRemoved = PlaylistTrackRemoved;
            _sessionManager.PlaybackLoadingReady = PlaybackLoadingReady;
        }

        private void PlaylistOpened(Playlist playlist)
        {
            //toast for visual feedback

            //add existing tracks to playlistCollection binded to list
            _playlistViewModel.CurrentPlaylist = playlist;
            _playlistViewModel.ResultsForPlaylist.Clear();
            for (int i = 0; i < playlist.NumTracks(); i++)
            {
               _playlistViewModel.ResultsForPlaylist.Add(new ResultDataModel(playlist.Track(i).Name(), playlist.Track(i).Artist(0).Name(), Link.CreateFromTrack(playlist.Track(i), 0).AsString()));
            }

            _playlistViewModel.CanPlay = true;
            _resultViewModel.PlaylistOpened = true;
            _playlistViewModel.PlaylistPresent = true;
            if (_playlistViewModel.CurrentPlaylist.NumTracks() == 0) _playlistViewModel.PlaylistEmpty = true;

            int info = _infoWorker.ShowCommonInfo("Playlist opened", "You successfully opened the playlist '" + playlist.Name() + "'", "Ok");
        }

        private void PlaybackLoadingReady(String track)
        {
            foreach (ResultDataModel result in _playlistViewModel.ResultsForPlaylist)
            {
                //Vergleiche Tracks ??? keine ahnung wie!
                if (Link.CreateFromString(result.SpotifyTrack).AsTrack().Artist(0).Name() == Link.CreateFromString(track).AsTrack().Artist(0).Name() && Link.CreateFromString(result.SpotifyTrack).AsTrack().Name() == Link.CreateFromString(track).AsTrack().Name() && Link.CreateFromString(result.SpotifyTrack).AsTrack().Duration() == Link.CreateFromString(track).AsTrack().Duration())
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

        private void PlaybackStarted(String track)
        {
            _playlistViewModel.Playing = true;
            foreach (ResultDataModel result in _playlistViewModel.ResultsForPlaylist)
            {
                try
                {
                    //Vergleiche Tracks ??? keine ahnung wie!
                    if (Link.CreateFromString(result.SpotifyTrack).AsTrack().Artist(0).Name() == Link.CreateFromString(track).AsTrack().Artist(0).Name() && Link.CreateFromString(result.SpotifyTrack).AsTrack().Name() == Link.CreateFromString(track).AsTrack().Name())
                    {
                        result.IsLoading = true;
                    }
                    else
                    {
                        result.IsLoading = false;
                        result.IsPlaying = false;
                    }
                }
                catch (Exception e)
                {
                    _infoWorker.ShowCommonInfo("Spotify Error", "Spotify encountered an error. Please try again", "Ok");
                }
            }
        }

        private void PlaybackPaused()
        {
            _playlistViewModel.Playing = false;
            foreach (ResultDataModel result in _playlistViewModel.ResultsForPlaylist)
            {
                result.IsLoading = false;
                result.IsPlaying = false;
            }
        }

        private void PlaybackStopped()
        {
            _playlistViewModel.Playing = false;
            foreach (ResultDataModel result in _playlistViewModel.ResultsForPlaylist)
            {
                result.IsLoading = false;
                result.IsPlaying = false;
            }
        }

        private void EndOfTrack()
        {
            
        }

        private void PlaylistTrackRemoved(int index)
        {
            _playlistViewModel.ResultsForPlaylist.RemoveAt(index);
        }

        //PUBLIC METHODS
        public bool CanPlay() { return _playlistViewModel.CanPlay; }

        public void AddTrackToPlaylist(object[] data)
        {
            ResultDataModel result = (ResultDataModel)data[0];
            System.Windows.Controls.Image imageElement = (System.Windows.Controls.Image)data[1];
            if (_accountWorker.IsLoggedIn())
            {
                if (_playlistViewModel.CurrentPlaylist != null)
                {
                    for (var i = 0; i < _playlistViewModel.CurrentPlaylist.NumTracks(); i++)
                    {
                        if (_playlistViewModel.CurrentPlaylist.Track(i).Artist(0).Name().Equals(Link.CreateFromString(result.SpotifyTrack).AsTrack().Artist(0).Name()) && _playlistViewModel.CurrentPlaylist.Track(i).Name().Equals(Link.CreateFromString(result.SpotifyTrack).AsTrack().Name()))
                        {
                            //track already in playlist
                            //_sessionManager.logMessages.Enqueue("Track already in playlist");
                            return;
                        }
                    }
                    Storyboard.SetTarget(fadeIn, imageElement);
                    Storyboard.SetTarget(fadeOut, imageElement);
                    Storyboard.SetTargetProperty(fadeIn, new PropertyPath(System.Windows.Controls.Image.OpacityProperty));
                    Storyboard.SetTargetProperty(fadeOut, new PropertyPath(System.Windows.Controls.Image.OpacityProperty));
                    Storyboard sb = new Storyboard();
                    sb.Children.Add(fadeIn);
                    sb.Children.Add(fadeOut);
                    sb.Begin();
                    _playlistViewModel.ResultsForPlaylist.Add(result);
                    _sessionManager.AddTrackToPlaylist(_playlistViewModel.CurrentPlaylist, result.SpotifyTrack);
                    _playlistViewModel.PlaylistEmpty = false;
                }
                else
                {
                    //no playlist available
                    //Notify user to open or create playlist
                    _sessionManager.logMessages.Enqueue("No Playlist opened");
                }
            }
            else
            {
                //user not logged in
                //Notify user to login to spotify
                _sessionManager.logMessages.Enqueue("User not logged in");
            }
        }

        public void RemoveTrackFromPlaylist(int index)
        {
            _playlistViewModel.ResultsForPlaylist.RemoveAt(index);
            _sessionManager.RemoveTrackFromPlaylist(_playlistViewModel.CurrentPlaylist, index);
        }

        public void JumpToTrack(int index)
        {
            Debug.WriteLine("TrackIsLoading" + _playlistViewModel.CurrentPlaylist.Track(index).IsLoaded());
            _sessionManager.JumpToTrackInPlaylist(_playlistViewModel.CurrentPlaylist, index);
        }

        public void ReorderTrack(object[] data)
        {
            _sessionManager.ReorderTrack(_playlistViewModel.CurrentPlaylist, (int)data[0], (int)data[1]);
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
