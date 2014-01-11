using System;
using SpotifySharp;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;

namespace MusicStream
{
    public class MusicStreamManager
    {
        /* Credentials for SpotifyAPI */
        private string spotifyUsername = "mybleton";
        private string spotifyPassword = "ctms";
        SpotifySession session;
        Search search;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;
        public ConcurrentQueue<string> logMessages;
        public MusicStreamListener MusicStreamListener;
        public Action<string> receiveLogMessage;

        public MusicStreamManager()
        {
            //InitializeComponent();

            syncContext = SynchronizationContext.Current;
            logMessages = new ConcurrentQueue<string>();

            var config = new SpotifySessionConfig();
            config.ApiVersion = 12;
            config.CacheLocation = "spotifydata";
            config.SettingsLocation = "spotifydata";
            config.ApplicationKey = File.ReadAllBytes("spotify_appkey.key");
            config.UserAgent = "Samsung SUR40 Tabletop Collaborative Tabletop Music Search";
            MusicStreamListener = new MusicStreamListener(this);
            config.Listener = MusicStreamListener;

            timer = new System.Threading.Timer(obj => InvokeProcessEvents(session), null, Timeout.Infinite, Timeout.Infinite);

            session = SpotifySession.Create(config);
            //session.Login(spotifyUsername, spotifyPassword, false, null);
        }

        public void InvokeProcessEvents(SpotifySession session)
        {
            syncContext.Post(obj => ProcessEvents(session), null);
        }

        void ProcessEvents(SpotifySession session)
        {
            this.session = session;
            int timeout = 0;
            string message;
            while (logMessages.TryDequeue(out message))
            {
                receiveLogMessage(message);
            }
            while (timeout == 0)
            {
                session.ProcessEvents(ref timeout);
            }
            timer.Change(timeout, Timeout.Infinite);
        }


        private void SpotifySearch()
        {
            search = Search.Create(session, "tag:new", 0, 0, 0, 250, 0, 0, 0, 0, SearchType.Standard, null, null);
        }

        public void Login()
        {
            session.Login(spotifyUsername, spotifyPassword, false, null);
        }

        /* Method for testing connection to SpotifyAPI */
        public void Play()
        {
            
        }
    }
}