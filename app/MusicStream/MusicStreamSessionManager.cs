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
using System.Collections.ObjectModel;

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
        public Action<ObservableCollection<Playlist>> ReadyForPlayback;

        //public Action<Track> GetLoadedTrackCompleted;
        public Action PrelistenStarted;
        public Action PrelistenStopped;
        public Action PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;

        private string _credentialsBlob = null;
        private object _userdata = null;
        private ObservableCollection<Playlist> _playlists;

        public ConcurrentQueue<string> logMessages;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;

        //PlayerPlaylist
        private IWavePlayer _prelistenWaveOutDevice;
        private BufferedWaveProvider _prelistenBufferedWaveProvider;

        private AudioBufferStats _audioBufferStats;
        private byte[] _copiedFrames;
        private Track _currentTrack;
        private int _currentTrackIndex;
        private Playlist _currentPlaylist;
        

        public MusicStreamSessionManager()
        {
            //InitializeComponent();

            _backgroundWorkHelper = new BackgroundWorkHelper();

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

        public AudioBufferStats GetCurrentAudioBufferStats()
        {
            _audioBufferStats.samples = _prelistenBufferedWaveProvider.BufferedBytes / 2;
            _audioBufferStats.stutter = 0;
            return _audioBufferStats;
        }


        /* ---------- PUBLIC METHODS ---------- */
        public void Login(string username, string password)
        {
            Dictionary<string, object> credentials = new Dictionary<string, object>();
            credentials.Add("username", username);
            credentials.Add("password", password);
            _backgroundWorkHelper.DoInBackground(LoginWorker, LoginCompleted, credentials);
            //_session.Login(_spotifyUsername, _spotifyPassword, true, CredentialsBlob);
        }

        public void StartPrelisteningTrack(String spotifyTrackId)
        {
            _backgroundWorkHelper.DoInBackground(PrelistenPlayWorker, PrelistenPlayCompleted, spotifyTrackId);
        }
        public void StopPrelisteningTrack()
        {
            _backgroundWorkHelper.DoInBackground(PrelistenStopWorker, PrelistenStopCompleted);
        }

        

        /*public void PlayTrack(Track track)
        {
            _session.PlayerPlay(true);   //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gab66c5915967e4f90db945b118e620624
            _prelistenWaveOutDevice.Play();
            PlaybackStarted();
        }*/

        public void PauseTrack()
        {
            _prelistenWaveOutDevice.Pause();
        }

        public void StopTrack()
        {
            _prelistenWaveOutDevice.Stop();
            _session.PlayerUnload();
        }

        public void ProceedPlayingPlaylist()
        {

        }


        /* ---------- CALLBACKS ---------- */
        public void LoggedInCallback()
        {
            /* Callback, when successfully logged in to Spotify
             * creating new PlaylistContainer
             */
            SpotifyLoggedIn();  //Notify MenuController

            _playlistContainer = _session.Playlistcontainer();
            _playlistContainerListener = new MusicStreamPlaylistContainerListener(this);
            _playlistContainer.AddCallbacks(_playlistContainerListener, Userdata);
        }

        public void PlaylistContainerLoadedCallback()
        {
            //Retrieving available user playlists
            _playlists = new ObservableCollection<Playlist>();
            for (int i = 0; i < _playlistContainer.NumPlaylists(); i++)
            {
                _playlists.Add(_playlistContainer.Playlist(i));
                //logMessages.Enqueue("Found Playlist: (" + i + ")" + _playlistContainer.Playlist(i).Name());
            }

            //Create Buffer, Stats & AudioDevice
            _prelistenBufferedWaveProvider = new BufferedWaveProvider(new WaveFormat()); //Create new Buffer
            _prelistenBufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(240);
            //_copiedFrames = new byte[5000];
            _audioBufferStats = new AudioBufferStats(); //Create stats for Spotify
            _prelistenWaveOutDevice = new WaveOut(); //Create new AudioDevice
            _prelistenWaveOutDevice.Init(_prelistenBufferedWaveProvider);

            //Notify PlaylistWorker that Playback is ready to start!!!
            logMessages.Enqueue("READY FOR PLAYBACK");
            ReadyForPlayback(_playlists);
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            var size = num_frames * format.channels * 2;
            if (size != 0)
            {
                _copiedFrames = new byte[size];
                Marshal.Copy(frames, _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
                _prelistenBufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
            }
        }

        public void EndOfTrack(SpotifySession session)
        {
            //_currentTrack is null at this point
            logMessages.Enqueue("Finished playing: '" + _currentTrack.Artist(0).Name() + " - " + _currentTrack.Name() + "'.");

            _currentTrackIndex++;
            //GetLoadedTrackWorker(_currentPlaylist.Track(_currentTrackIndex));

            PlaybackEndOfTrack();
        }


        /*---------- BACKGROUNDHELPERS ----------*/
        //Login
        private void LoginWorker(object sender, DoWorkEventArgs e)
        {
            _session.Login((string)((Dictionary<string, object>)e.Argument)["username"], (string)((Dictionary<string, object>)e.Argument)["password"], false, CredentialsBlob);
        }
        private void LoginCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        //Prelisten
        private void PrelistenPlayWorker(object sender, DoWorkEventArgs e)
        {
            Track track = Track.GetPlayable(_session, Link.CreateFromString((String)e.Argument).AsTrack());
            _session.PlayerLoad(track);
            _session.PlayerPrefetch(track);
            var duration = track.Duration();
            _session.PlayerSeek(track.Duration()/2);    //Seek to half of the song for prelistening
            _session.PlayerPlay(true);
            _prelistenWaveOutDevice.Play();
        }
        private void PrelistenPlayCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //PlaybackStarted();
            PrelistenStarted();
        }

        private void PrelistenStopWorker(object sender, DoWorkEventArgs e)
        {
            _prelistenWaveOutDevice.Stop();
            _session.PlayerPlay(false);
            _session.PlayerUnload();
        }
        private void PrelistenStopCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PrelistenStopped();
        }


        /* ---------- HELPER METHODS ---------- */
        //Message Logging
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


    }
}