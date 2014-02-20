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
using Helpers;
using System.ComponentModel;

namespace MusicStream
{
    public class MusicStreamSessionManager
    {
        private BackgroundWorkHelper _backgroundWorkHelper;
        private SpotifySession _session;
        public MusicStreamSessionListener SessionListener;
        private PlaylistContainer _playlistContainer;
        private MusicStreamPlaylistContainerListener _playlistContainerListener;
        private MusicStreamPlaylistListener _playlistListener;
        //private MusicStreamPlaylistManager _playlistManager;

        //Actions
        public Action<string> receiveLogMessage;
        public Action SpotifyLoggedIn;
        public Action PlaylistContainerLoaded;
        public Action<Track> TrackLoaded;
        public Action PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;

        private string _spotifyUsername = "mybleton";
        private string _spotifyPassword = "ctms";
        private string _credentialsBlob = null;
        private object _userdata;

        public ConcurrentQueue<string> logMessages;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;

        private IWavePlayer _waveOutDevice;
        private BufferedWaveProvider _bufferedWaveProvider;
        private AudioBufferStats _audioBufferStats;
        private byte[] _copiedFrames;
        private Track _currentTrack;
        private int _currentTrackIndex;
        private Playlist _currentPlaylist;
        

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

            timer = new System.Threading.Timer(obj => InvokeProcessEvents(_session), null, Timeout.Infinite, Timeout.Infinite);

            //Creating new SpotifySession
            _session = SpotifySession.Create(config);

                     
        }

        //SETTER & GETTER
        public string CredentialsBlob
        {
            set { this._credentialsBlob = value; }
            get { return this._credentialsBlob; }
        }

        public object Userdata
        {
            set { this._userdata = value; }
            get { return this._userdata; }
        }

        public string SpotifyUsername
        {
            set { this._spotifyUsername = value; }
            get { return this._spotifyUsername; }
        }

        public string SpotifyPassword
        {
            set { this._spotifyPassword = value; }
            get { return this._spotifyPassword; }
        }

        public AudioBufferStats GetCurrentAudioBufferStats()
        {
            _audioBufferStats.samples = _bufferedWaveProvider.BufferedBytes / 2;
            _audioBufferStats.stutter = 0;
            return _audioBufferStats;
        }


        //CALLBACKS
        public void LoggedInCallback()
        {
            /* Callback, when successfully logged in to Spotify
             * (called from Ctms.Applications/Workers/MusicStreamAccountWorker.cs - SpotifyLoggedIn() :45)
             * 
             * creating new PlaylistContainerManager
             * creating new PlaylistContainer
             */
            SpotifyLoggedIn();  //Notify MenuController

            _playlistContainer = _session.Playlistcontainer();
            _playlistContainerListener = new MusicStreamPlaylistContainerListener(this);
            _playlistContainer.AddCallbacks(_playlistContainerListener, null);
        }

        public void PlaylistContainerLoadedCallback()
        {
            /* Callback, when PlaylistContainer is loaded
             * (called from MusicStream/MusicStreamPlaylistContainerListener.cs - ContainerLoaded() :43)
             */
            PlaylistContainerLoaded();
            for (int i = 0; i < _playlistContainer.NumPlaylists(); i++)
            {
                logMessages.Enqueue("Found Playlist: (" + i + ")" + _playlistContainer.Playlist(i).Name());
            }

            OpenPlaylist(_playlistContainer.Playlist(0));
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            //http://stackoverflow.com/questions/21307520/playing-ohlibspotify-pcm-data-stream-in-c-sharp-with-naudio
            //http://forum.openhome.org/showthread.php?tid=1202&pid=2223#pid2223

            var size = num_frames * format.channels * 2;
            _copiedFrames = new byte[size];
            Marshal.Copy(frames, _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
            _bufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
        }

        public void EndOfTrack(SpotifySession session)
        {
            logMessages.Enqueue("Finished playing: '" + _currentTrack.Artist(0).Name() + " - " + _currentTrack.Name() + "'.");

            _currentTrackIndex++;
            LoadTrack(_currentPlaylist.Track(_currentTrackIndex));

            PlaybackEndOfTrack();
        }


        //METHODS
        public void InvokeProcessEvents(SpotifySession session)
        {
            syncContext.Post(obj => ProcessEvents(session), null);
        }

        void ProcessEvents(SpotifySession session)
        {
            this._session = session;
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

        public void Login()
        {
            _session.Login(_spotifyUsername, _spotifyPassword, true, CredentialsBlob);
        }

        public void OpenPlaylist(Playlist playlist)
        {
            logMessages.Enqueue("Opening Playlist: '" + playlist.Name() + "'.");
            _currentPlaylist = playlist;
            _playlistListener = new MusicStreamPlaylistListener(this);
            _currentPlaylist.AddCallbacks(_playlistListener, _userdata);

            LoadTrack(playlist.Track(0));
            _currentTrackIndex = 0;
        }

        public void LoadTrack(Track track)
        {
            logMessages.Enqueue("Loading Track: '" + track.Artist(0).Name() + " - " + track.Name() + "'.");
            _currentTrack = track;

            _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat());
            _bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(240);
            _audioBufferStats = new AudioBufferStats();

            _session.PlayerLoad(track);  //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gac73bf2c569a43d824439b557d5e4b293
            _session.PlayerPrefetch(track);

            _waveOutDevice = new WaveOut();
            _waveOutDevice.Init(_bufferedWaveProvider);
            TrackLoaded(track);
        }

        public void PlayTrack(object sender, DoWorkEventArgs e)
        {
            _session.PlayerPlay(true);   //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gab66c5915967e4f90db945b118e620624
            _waveOutDevice.Play();
            //PlaybackStarted();
        }
        /*
        public void PlayTrack(Track track)
        {
            _session.PlayerPlay(true);   //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gab66c5915967e4f90db945b118e620624
            _waveOutDevice.Play();
            PlaybackStarted();
        }*/

        public void PauseTrack()
        {
            _waveOutDevice.Pause();
        }

        public void StopTrack()
        {
            _waveOutDevice.Stop();
            _session.PlayerUnload();
        }

        public void ProceedPlayingPlaylist()
        {

        }
    }
}