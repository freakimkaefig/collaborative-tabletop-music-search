using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;
using Newtonsoft.Json;

namespace MusicStream
{
    public class MusicStreamListener : SpotifySessionListener
    {
        private MusicStreamManager _musicStreamManager;
        public Action SpotifyLoggedIn;

        public MusicStreamListener(MusicStreamManager musicStreamManager)
        {
            _musicStreamManager = musicStreamManager;
        }

        public override void NotifyMainThread(SpotifySession session)
        {
            base.NotifyMainThread(session);
            _musicStreamManager.InvokeProcessEvents(session);
        }

        public override void LoggedIn(SpotifySession session, SpotifyError error)
        {
            //base.LoggedIn(session, error);
            _musicStreamManager.logMessages.Enqueue("WIN: LOGGED IN");
            SpotifyLoggedIn();
        }

        public override void ConnectionError(SpotifySession session, SpotifyError error)
        {
            base.ConnectionError(session, error);
            _musicStreamManager.logMessages.Enqueue("ERROR: ConnectionError: {0}\n" + error.ToString());
        }

        public override void LogMessage(SpotifySession session, string data)
        {
            base.LogMessage(session, data);
            _musicStreamManager.logMessages.Enqueue(data);
            NotifyMainThread(session);
        }

        public void SearchComplete(Search result, object nativeUserdata)
        {
            var album = result.Album(0);
            var albumName = album.Name();
            
            var artist = album.Artist().Name();

            var numTracks = result.NumTracks();

            var query = result.Query();

            _musicStreamManager.logMessages.Enqueue("\tQUERY: " + query);
            _musicStreamManager.logMessages.Enqueue("\tINTERPRET: " + artist);
            _musicStreamManager.logMessages.Enqueue("\tALBUM: " + albumName);
        }
    }
}
