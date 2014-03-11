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

            _accountWorker.SessionManagerCreated = SessionManagerCreated;
        }

        //CALLBACK
        private void SessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _sessionManager.PlaylistOpened = PlaylistOpened;
        }

        private void PlaylistOpened(Playlist playlist)
        {
            //toast for visual feedback

            //add existing tracks to playlistCollection binded to list
            _playlistViewModel.CurrentPlaylist = playlist;
            for (int i = 0; i < playlist.NumTracks(); i++)
            {
                _playlistViewModel.ResultsForPlaylist.Add(new ResultDataModel(Link.CreateFromTrack(playlist.Track(i), 0).AsString(), playlist.Track(i).Name(), playlist.Track(i).Artist(0).Name()));
            }
        }

        public void AddTrackToPlaylist(ResultDataModel result)
        {
            if (_accountWorker.IsLoggedIn())
            {
                if (_playlistViewModel.CurrentPlaylist != null)
                {
                    _playlistViewModel.ResultsForPlaylist.Add(result);
                    _sessionManager.AddTrackToPlaylist(_playlistViewModel.CurrentPlaylist, result.Result.Song.SpotifyId);
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
    }
}
