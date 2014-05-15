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
using System.Diagnostics;


namespace Ctms.Applications.Workers
{
    /// <summary>
    /// 
    /// </summary>
    [Export]
    public class MusicStreamAccountWorker
    {

        private MusicStreamSessionManager _sessionManager;
        private MenuViewModel _menuViewModel;
        private PlaylistViewModel _playlistViewModel;
        private InfoWorker _infoWorker;

        public Action<MusicStreamSessionManager> StreamingSessionManagerCreated;
        public Action<MusicStreamSessionManager> PlaylistSessionManagerCreated;
        public Action<MusicStreamSessionManager> ResultSessionManagerCreated;
        public Action<MusicStreamSessionManager> FftSessionManagerCreated;

        [ImportingConstructor]
        public MusicStreamAccountWorker(MenuViewModel menuViewModel, PlaylistViewModel playlistViewModel, InfoWorker infoWorker)
        {
            _menuViewModel = menuViewModel;
            _playlistViewModel = playlistViewModel;
            _infoWorker = infoWorker;
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

            if (_sessionManager == null)
            {
                _sessionManager = new MusicStreamSessionManager();
            }
            PlaylistSessionManagerCreated(_sessionManager);
            StreamingSessionManagerCreated(_sessionManager);
            ResultSessionManagerCreated(_sessionManager);
            FftSessionManagerCreated(_sessionManager);

            //registration of listeners to MusicStreamSessionManager
            _sessionManager.ReceiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyError = SpotifyError;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.SpotifyLoggedOut = SpotifyLoggedOut;
            _sessionManager.ReadyForPlayback = ReadyForPlayback;

            //passing credentials to MusicStreamSessionManager & logging in
            _sessionManager.Login(_menuViewModel.SpotifyUsernameInput, spotifyPasswordInput.Password);

            //AUTOLOGIN REMOVE FOR RELEASE
            //_sessionManager.Login("mybleton", "ctms");
            _menuViewModel.IsLoggingIn = true;
            _menuViewModel.LoginDialogEnabled = false;
        }

        public void CancelLogin()
        {
            if (!_menuViewModel.IsLoggedIn && _menuViewModel.IsLoggingIn)
            {
                _sessionManager.Logout();
                if (_sessionManager.PlaylistContainerListener != null)
                {
                    _sessionManager.Session.Playlistcontainer().RemoveCallbacks(_sessionManager.PlaylistContainerListener, null);
                }
            }

            _menuViewModel.IsLoggingIn = false;
            _menuViewModel.IsLoggedIn = false;
            _menuViewModel.LoginDialogEnabled = true;
        }

        public void Logout()
        {
            if (_sessionManager != null)
            {
                _sessionManager.Logout();
                _menuViewModel.IsLoggingIn = false;
                _menuViewModel.IsLoggedIn = false;
                _menuViewModel.LoginDialogEnabled = true;
            }
        }

        public void OpenPlaylist(SpotifyPlaylist spotifyPlaylist)
        {
            if (_menuViewModel.IsLoggedIn)
            {
                _sessionManager.OpenPlaylists(spotifyPlaylist.Playlist);
                _playlistViewModel.PlaylistPresent = true;
                if (spotifyPlaylist.Playlist.NumTracks() > 0)
                {
                    _playlistViewModel.PlaylistEmpty = false;
                }
            }
            else
            {
                //Notify user to login to spotify
                _infoWorker.ShowCommonInfo("Not logged in to Spotify", "You have to login to Spotify", "Ok");
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
                _infoWorker.ShowCommonInfo("Not logged in to Spotify", "You have to login to Spotify", "Ok");
            }
        }


        //CALLBACKS
        private void ReceiveLogMessage(string logMessage)
        {
            Debug.WriteLine(CodeHelpers.GetTimeStamp() + ": " + logMessage);
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyError(SpotifyError spotifyError)
        {
            if (spotifyError != SpotifySharp.SpotifyError.Ok)
            {
                _infoWorker.ShowCommonInfo("Spotify error", "Spotify encountered an error. Please try again" , "Ok");
            }
        }

        private void SpotifyLoggedIn()
        {
            //toast for visual feedback

            //_menuViewModel.IsLoggedIn = true;
        }

        private void SpotifyLoggedOut()
        {
            _menuViewModel.IsLoggedIn = false;
        }

        private void ReadyForPlayback(ObservableCollection<Playlist> playlists)
        {
            _menuViewModel.ShowLoginDialog(false);
            _menuViewModel.IsLoggingIn = false;
            _menuViewModel.IsLoggedIn = true;
            _playlistViewModel.ReadyForPlayback = true;

            _menuViewModel.Playlists.Clear();

            for (var i = 0; i < playlists.Count; i++)
            {
                SpotifyPlaylist spotifyPlaylist = new SpotifyPlaylist();
                spotifyPlaylist.Playlist = playlists[i];
                spotifyPlaylist.Playlist.AddCallbacks(new MusicStreamPlaylistListener(_sessionManager), _sessionManager.Userdata);
                _menuViewModel.Playlists.Add(spotifyPlaylist);
            }
        }

        private void InfoShowError(String msg)
        {
            _infoWorker.ShowCommonInfo("Spotify error", "Spotify encountered an error. Please try again", "Ok");
        }
    }
}