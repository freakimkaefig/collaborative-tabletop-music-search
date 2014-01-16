using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
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

        [ImportingConstructor]
        public MusicStreamAccountWorker(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
        }

        public bool CanLogin() { return _menuViewModel.IsValid; }

        public void Login()
        {
            //do stuff
            _sessionManager = new MusicStreamSessionManager();
            _sessionManager.receiveLogMessage = ReceiveLogMessage;
            _sessionManager.SessionListener.SpotifyLoggedIn = SpotifyLoggedIn;
            _sessionManager.Login();
        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + logMessage;
        }

        private void SpotifyLoggedIn()
        {
            _menuViewModel.IsLoggedIn = true;
            _sessionManager.LoginCallback();
        }
    }
}
