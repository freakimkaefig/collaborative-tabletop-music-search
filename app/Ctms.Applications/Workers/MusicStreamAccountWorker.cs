using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
using Helpers;
using Ctms.Applications.ViewModels;


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


        public void Login()
        {
            //do stuff
            _sessionManager = new MusicStreamSessionManager();
            SessionManagerCreated(_sessionManager);
            _sessionManager.receiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.ReadyForPlayback = ReadyForPlayback;
            _sessionManager.Login(_spotifyUsername, _spotifyPassword);
        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyLoggedIn()
        {
            _menuViewModel.IsLoggedIn = true;
        }

        private void ReadyForPlayback()
        {
            _playlistViewModel.ReadyForPlayback = true;
        }
    }
}