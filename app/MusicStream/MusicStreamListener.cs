using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;

namespace MusicStream
{
    public class MusicStreamListener : SpotifySessionListener
    {
        MusicStreamManager musicStreamManager;

        public MusicStreamListener(MusicStreamManager musicStreamManager)
        {
            this.musicStreamManager = musicStreamManager;
        }

        public override void NotifyMainThread(SpotifySession session)
        {
            base.NotifyMainThread(session);
            musicStreamManager.InvokeProcessEvents(session);
        }

        public override void LoggedIn(SpotifySession session, SpotifyError error)
        {
            base.LoggedIn(session, error);
            musicStreamManager.logMessages.Enqueue("WIN: LOGGED IN");
        }

        public override void ConnectionError(SpotifySession session, SpotifyError error)
        {
            base.ConnectionError(session, error);
            musicStreamManager.logMessages.Enqueue("ERROR: ConnectionError: {0}\n" + error.ToString());
        }

        public override void LogMessage(SpotifySession session, string data)
        {
            base.LogMessage(session, data);
            musicStreamManager.logMessages.Enqueue(data);
            NotifyMainThread(session);
        }

        public static void SearchComplete(IntPtr result, IntPtr nativeUserdata)
        {

        }
    }
}
