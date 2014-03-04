using System;
using System.ComponentModel.Composition;
using MusicStream;
using Helpers;
using Ctms.Applications.ViewModels;
using Microsoft.Surface.Presentation.Controls;
using SpotifySharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;


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

            _menuViewModel.CanLogin = true;
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
            //string spotifyUsername = _menuViewModel.SpotifyUsernameInput;
            //MessageServiceExtensions.ShowMessage(_messageService, "Username: " + spotifyUsername + "\nPassword: " + spotifyPasswordInput.Password);
            
            _sessionManager = new MusicStreamSessionManager();
            SessionManagerCreated(_sessionManager);
            _sessionManager.receiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.ReadyForPlayback = ReadyForPlayback;
            _sessionManager.Login(_menuViewModel.SpotifyUsernameInput, spotifyPasswordInput.Password);

            //_menuViewModel.SpotifyLoginMenuItem = "Logout from Spotify";
            _menuViewModel.CanLogin = false;
        }

        public void OpenPlaylist(Playlist playlist)
        {

        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyLoggedIn()
        {
            _menuViewModel.IsLoggedIn = true;
        }

        private void ReadyForPlayback(ObservableCollection<Playlist> playlists)
        {
            _playlistViewModel.ReadyForPlayback = true;
            this._playlists = playlists;
            _menuViewModel.Playlists = playlists;
            _menuViewModel.PlaylistName = _menuViewModel.Playlists[0].Name();
        }
    }
}