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
        //private string _spotifyUsername = "mybleton";     //deprecated: Username not saved (input via menu)
        //private string _spotifyPassword = "ctms";         //deprecated: Password not saved (security issues, input via menu)
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
        public bool IsLoggedIn() { return _menuViewModel.IsLoggedIn; }

        /* deprecated: Username & Password not saved in code
         * 
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
         * */


        //PUBLIC METHODS
        public void Login(SurfacePasswordBox spotifyPasswordInput)
        {
            //handles Click on LoginButton in Menu

            _sessionManager = new MusicStreamSessionManager();
            SessionManagerCreated(_sessionManager);

            //registration of listeners to MusicStreamSessionManager
            _sessionManager.ReceiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyError = SpotifyError;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.SpotifyLoggedOut = SpotifyLoggedOut;
            _sessionManager.ReadyForPlayback = ReadyForPlayback;

            //passing credentials to MusicStreamSessionManager & logging in
            _sessionManager.Login(_menuViewModel.SpotifyUsernameInput, spotifyPasswordInput.Password);
        }

        public void Logout()
        {
            _sessionManager.Logout();
        }

        public void OpenPlaylist(SpotifyPlaylist spotifyPlaylist)
        {
            if (_menuViewModel.IsLoggedIn)
            {
                _sessionManager.OpenPlaylists(spotifyPlaylist.Playlist);
            }
            else
            {
                //Notify user to login to spotify
            }
        }

        public void CreateNewPlaylist(SurfaceTextBox name)
        {
            if (_menuViewModel.IsLoggedIn)
            {
                _sessionManager.CreatePlaylist(name.Text);
            }
            else
            {
                //Notify user to login to spotify
            }
        }


        //CALLBACKS
        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyError(SpotifyError spotifyError)
        {
            switch (spotifyError)
            {
                case SpotifySharp.SpotifyError.Ok:
                    //no Error
                    break;
                case SpotifySharp.SpotifyError.TrackNotPlayable:
                    //no Error
                    break;
                //... switch all errors from SpotifySharp.SpotifyError
                default:
                    ReceiveLogMessage("ERROR");
                    break;
            }
        }

        private void SpotifyLoggedIn()
        {
            //toast for visual feedback

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