using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;
using Newtonsoft.Json;

namespace MusicStream
{
    /// <summary>
    /// Listener to events from SpotifySession
    /// </summary>
    public class MusicStreamSessionListener : SpotifySessionListener
    {
        private MusicStreamSessionManager _sessionManager;
        public Action SpotifyLoggedIn;

        public MusicStreamSessionListener(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        public override void NotifyMainThread(SpotifySession session)
        {
            base.NotifyMainThread(session);
            _sessionManager.InvokeProcessEvents(session);
        }

        public override void LoggedIn(SpotifySession session, SpotifyError error)
        {
            base.LoggedIn(session, error);
            _sessionManager.logMessages.Enqueue("LOGGED IN: LOGGED IN");    //Logging LoginMessage to TextBox
            SpotifyLoggedIn();  //Trigger Action SpotifyLoggedIn
        }

        public override void LoggedOut(SpotifySession session)
        {
            base.LoggedOut(session);
            _sessionManager.logMessages.Enqueue("LOGGED OUT: LOGGED OUT");  //Logging LogoutMessage to TextBox
        }

        public override void ConnectionstateUpdated(SpotifySession session)
        {
            base.ConnectionstateUpdated(session);
            _sessionManager.logMessages.Enqueue("CONNECTION STATUS UPDATED: " + session.Connectionstate()); //Logging changes in connection state
        }

        public override void ConnectionError(SpotifySession session, SpotifyError error)
        {
            base.ConnectionError(session, error);
            _sessionManager.logMessages.Enqueue("CONNECTION ERROR: {0}\n" + error.ToString());  //Logging ConnectionErrors
        }

        public override void LogMessage(SpotifySession session, string data)
        {
            base.LogMessage(session, data);
            _sessionManager.logMessages.Enqueue("LOG MESSAGE: " + data);    //Logging LogMessages
            NotifyMainThread(session);
        }

        public void SearchComplete(Search result, object nativeUserdata)
        {
            var album = result.Album(0);
            var albumName = album.Name();
            
            var artist = album.Artist().Name();

            var numTracks = result.NumTracks();

            var query = result.Query();

            //_musicStreamManager.logMessages.Enqueue("\tQUERY: " + query);
            //_musicStreamManager.logMessages.Enqueue("\tINTERPRET: " + artist);
            //_musicStreamManager.logMessages.Enqueue("\tALBUM: " + albumName);
        }
    }
}
