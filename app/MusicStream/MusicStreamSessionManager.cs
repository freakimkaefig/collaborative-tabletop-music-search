using System;
using SpotifySharp;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Collections;

namespace MusicStream
{
    public class MusicStreamSessionManager
    {
        public ConcurrentQueue<string> logMessages;
        public MusicStreamSessionListener SessionListener;
        public Action<string> receiveLogMessage;
        private string spotifyUsername = "mybleton";
        private string spotifyPassword = "ctms";
        private MusicStreamPlaylistContainerManager _playlistContainerManager;
        SpotifySession session;
        Search search;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;
        

        public MusicStreamSessionManager()
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
            SessionListener = new MusicStreamSessionListener(this);
            config.Listener = SessionListener;

            timer = new System.Threading.Timer(obj => InvokeProcessEvents(session), null, Timeout.Infinite, Timeout.Infinite);

            session = SpotifySession.Create(config);

            _playlistContainerManager = new MusicStreamPlaylistContainerManager(this, session);          
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

        public void SpotifySearch()
        {
            search = Search.Create(session, "artist:Katy Perry", 0, 0, 0, 250, 0, 0, 0, 0, SearchType.Standard, SessionListener.SearchComplete, null);
        }

        public void Login()
        {
            session.Login(spotifyUsername, spotifyPassword, false, null);
        }

        public void Play()
        {
            PlaylistContainer playlistContainer = _playlistContainerManager.CreatePlaylistContainer();  //create PlaylistContainer
            Playlist playlist = _playlistContainerManager.AddNewPlaylist("test");   //create Playlist in PlaylistContainer
            Link playlistLink = Link.CreateFromPlaylist(playlist);  //creates Link for Playlist
            var playlistLinkType = playlistLink.Type();     //returns LinkType => Playlist

            Link link = Link.CreateFromString("spotify:track:4lCv7b86sLynZbXhfScfm2");  //creates spotify link from string
            var linkType = link.Type();     //returns LinkType => Track
            Track track = link.AsTrack();   //returns Track by Link
            Track playable = Track.GetPlayable(session, track);     //returns Track

            Track[] tracks = new Track[1];
            tracks[0] = playable;

            //playlist.AddTracks(tracks, 1, session); //throws SpotifyException[Error: InvalidIndata, Message: "Invalid input"]
        }
    }
}