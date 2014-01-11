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
        private MusicStreamManager _musicStreamManager;
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
            _musicStreamManager = new MusicStreamManager();
            _musicStreamManager.receiveLogMessage = ReceiveLogMessage;
            _musicStreamManager.MusicStreamListener.SpotifyLoggedIn = SpotifyLoggedIn;
            _musicStreamManager.Login();
        }

        public void TestSearch()
        {
            _musicStreamManager.SpotifySearch();
        }

        private void ReceiveLogMessage(string logMessage)
        {
            _menuViewModel.LoginLogMessage += "\n" + logMessage;
        }

        private void SpotifyLoggedIn()
        {
            //TestSearch();
            _menuViewModel.IsLoggedIn = true;

        }
    }
}
