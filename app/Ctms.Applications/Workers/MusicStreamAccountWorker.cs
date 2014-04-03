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
        private ObservableCollection<Playlist> _playlists;

        private MusicStreamSessionManager _sessionManager;
        private MenuViewModel _menuViewModel;
        private PlaylistViewModel _playlistViewModel;

        public Action<MusicStreamSessionManager> StreamingSessionManagerCreated;
        public Action<MusicStreamSessionManager> PlaylistSessionManagerCreated;
        public Action<MusicStreamSessionManager> ResultSessionManagerCreated;
        public Action<MusicStreamSessionManager> FftSessionManagerCreated;

        public string test;

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
            Debug.WriteLine(CodeHelpers.GetTimeStamp() + ": " + logMessage);
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
                    ReceiveLogMessage("SpotifyError" + spotifyError.ToString());
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

            _menuViewModel.Playlists.Clear();

            for (var i = 0; i < playlists.Count; i++)
            {
                SpotifyPlaylist spotifyPlaylist = new SpotifyPlaylist();
                spotifyPlaylist.Playlist = playlists[i];
                spotifyPlaylist.Playlist.AddCallbacks(new MusicStreamPlaylistListener(_sessionManager), _sessionManager.Userdata);
                _menuViewModel.Playlists.Add(spotifyPlaylist);
            }
        }
    }
}