using System;
using SpotifySharp;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Collections;
using NAudio;
using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using NAudio.Utils;

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
        private PlaylistContainer _playlistcontainer;
        private MusicStreamPlaylistManager _playlistManager;
        SpotifySession session;
        Search search;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;

        //File file;
        FileStream fileStream;
        private Queue<IntPtr> _frames_list;
        public BufferedWaveProvider bufferedWaveProvider;
        private bool _waveOutDeviceInitialized = false;
        

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

        /*
        public void SpotifySearch()
        {
            search = Search.Create(session, "artist:Katy Perry", 0, 0, 0, 250, 0, 0, 0, 0, SearchType.Standard, SessionListener.SearchComplete, null);
        }
        */

        public void Login()
        {
            session.Login(spotifyUsername, spotifyPassword, false, null);
        }

        public void LoginCallback()
        {
            _playlistContainerManager = new MusicStreamPlaylistContainerManager(this, session);
            _playlistcontainer = _playlistContainerManager.CreatePlaylistContainer();
            //Play(Track.GetPlayable(session, Link.CreateFromString("spotify:track:6oDPg7fXW3Ug3KmbafrXzA").AsTrack()));
        }

        public void PlaylistContainerLoadedCallback()
        {
            //Playlist playlist = _playlistContainerManager.AddNewPlaylist("Test Playlist");

            Playlist playlist = _playlistcontainer.Playlist(0);
            var test1 = playlist.Name();
            var test2 = playlist.Track(0);
            var test3 = test2.Artist(0);
            var test4 = test3.Name();

            //_playlistManager = new MusicStreamPlaylistManager(this, playlist);

            //_playlistManager.AddTracksToPlaylist(session, playlist, GetTestTracks());
            Play(test2);
        }

        public void PlaylistTracksAddedCallback(Playlist playlist)
        {
            //logMessages.Enqueue("PlaylistTracksAddedCallback: " + _playlistManager.GetPlaylistMetadata(playlist));
            Play(playlist.Track(0));
        }

        private Track[] GetTestTracks()
        {
            Track katy = Track.GetPlayable(session, Link.CreateFromString("spotify:track:4lCv7b86sLynZbXhfScfm2").AsTrack());
            Track miley = Track.GetPlayable(session, Link.CreateFromString("spotify:track:6oDPg7fXW3Ug3KmbafrXzA").AsTrack());
            Track[] tracks = new Track[1];
            tracks[0] = katy;
            //tracks[1] = miley;

            return tracks;
        }

        public void Play(Track track)
        {
            session.PlayerLoad(track);
            session.PlayerPlay(true);
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            //http://stackoverflow.com/questions/21307520/playing-ohlibspotify-pcm-data-stream-in-c-sharp-with-naudio
            //format.channels = 2, format.samplerate = 44100, format.sample_type = Int16NativeEndian
            //frames = ?
            //num_frames = 2048

            byte[] frames_copy = new byte[num_frames];
            Marshal.Copy(frames, frames_copy, 0, num_frames);

            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat(format.sample_rate, format.channels));
            bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(40);            
            bufferedWaveProvider.AddSamples(frames_copy, 0, num_frames);
            bufferedWaveProvider.Read(frames_copy, 0, num_frames);

            if (_waveOutDeviceInitialized == false)
            {
                IWavePlayer waveOutDevice = new WaveOut();
                waveOutDevice.Init(bufferedWaveProvider);
                waveOutDevice.Play();
                _waveOutDeviceInitialized = true;
            }
        }
    }
}