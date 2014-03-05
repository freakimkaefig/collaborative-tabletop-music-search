using System;
using System.ComponentModel.Composition;
using MusicStream;
using Helpers;
using Ctms.Applications.ViewModels;
using Microsoft.Surface.Presentation.Controls;
using SpotifySharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ctms.Domain.Objects;


namespace Ctms.Applications.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    public class MusicStreamAccountWorker
    {
        private string _spotifyUsername = "mybleton";
        private string _spotifyPassword = "ctms";
        private ObservableCollection<Playlist> _playlists;

        private MusicStreamSessionManager _sessionManager;
        private MenuViewModel _menuViewModel;
        private PlaylistViewModel _playlistViewModel;

        public Action<MusicStreamSessionManager> SessionManagerCreated;

        [ImportingConstructor]
        public MusicStreamAccountWorker(MenuViewModel menuViewModel, PlaylistViewModel playlistViewModel)
        {
            _menuViewModel = menuViewModel;
            _playlistViewModel = playlistViewModel;
        }

        //Getter
        public bool CanLogin() { return _menuViewModel.CanLogin; }
        public string SpotifyUsername
        {
            set { _spotifyUsername = value; }
            get { return _spotifyUsername; }
        }
        public string SpotifyPassword
        {
            set { _spotifyPassword = value; }
            get { return _spotifyPassword; }
        }


        public void Login(SurfacePasswordBox spotifyPasswordInput)
        {
            _sessionManager = new MusicStreamSessionManager();
            SessionManagerCreated(_sessionManager);
            _sessionManager.receiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.SpotifyLoggedOut = SpotifyLoggedOut;
            _sessionManager.ReadyForPlayback = ReadyForPlayback;
            _sessionManager.Login(_menuViewModel.SpotifyUsernameInput, spotifyPasswordInput.Password);
        }

        public void Logout()
        {
            _sessionManager.Logout();
        }

        public void OpenPlaylist(SpotifyPlaylist spotifyPlaylist)
        {
            _sessionManager.OpenPlaylists(spotifyPlaylist.Playlist);
        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyLoggedIn()
        {
            _menuViewModel.IsLoggedIn = true;
        }

        private void SpotifyLoggedOut()
        {
            _menuViewModel.IsLoggedIn = false;
        }

        private void ReadyForPlayback(ObservableCollection<Playlist> playlists)
        {
            _playlistViewModel.ReadyForPlayback = true;
            _playlists = playlists;

            foreach (var playlist in playlists)
            {
                _menuViewModel.Playlists.Add(new SpotifyPlaylist() { Playlist = playlist });
                _menuViewModel.PlaylistName = _menuViewModel.Playlists[0].Playlist.Name();
            }
        }
    }
}