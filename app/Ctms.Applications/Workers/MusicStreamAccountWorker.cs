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
        private MusicStreamSessionManager _sessionManager;
        private MenuViewModel _menuViewModel;

        public Action<MusicStreamSessionManager> SessionManagerCreated;

        [ImportingConstructor]
        public MusicStreamAccountWorker(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
            _menuViewModel.CanLogin = true;
        }

        //Getter
        public bool CanLogin() { return _menuViewModel.CanLogin; }

        public void Login()
        {
            //do stuff
            _sessionManager = new MusicStreamSessionManager();
            SessionManagerCreated(_sessionManager);
            _sessionManager.receiveLogMessage = ReceiveLogMessage;
            _sessionManager.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.PlaylistContainerLoaded = PlaylistContainerLoaded;
            _sessionManager.Login();
        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + CodeHelpers.GetTimeStamp() + "\n" +logMessage + "\n";
        }

        private void SpotifyLoggedIn()
        {
            _menuViewModel.IsLoggedIn = true;
        }

        private void PlaylistContainerLoaded()
        {
            _menuViewModel.PlaylistContainerLoaded = true;
        }
    }
}
