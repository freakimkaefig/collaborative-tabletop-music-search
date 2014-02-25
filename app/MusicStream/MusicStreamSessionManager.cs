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
        public Action ReadyForPlayback;

        //public Action<Track> GetLoadedTrackCompleted;
        public Action PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;

        private string _credentialsBlob = null;
        private object _userdata;

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
            _backgroundWorkHelper.DoInBackground(PrelistenWorker, PrelistenCompleted, spotifyTrackId);
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
            _playlistContainer.AddCallbacks(_playlistContainerListener, null);
        }

        public void PlaylistContainerLoadedCallback()
        {
            //Logging available Playlists
            /*
            for (int i = 0; i < _playlistContainer.NumPlaylists(); i++)
            {
                logMessages.Enqueue("Found Playlist: (" + i + ")" + _playlistContainer.Playlist(i).Name());
            }
             * */

            //Create Buffer, Stats & AudioDevice
            _prelistenBufferedWaveProvider = new BufferedWaveProvider(new WaveFormat()); //Create new Buffer
            _prelistenBufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(240);
            _audioBufferStats = new AudioBufferStats(); //Create stats for Spotify
            _prelistenWaveOutDevice = new WaveOut(); //Create new AudioDevice
            _prelistenWaveOutDevice.Init(_prelistenBufferedWaveProvider);

            //Notify PlaylistWorker that Playback is ready to start!!!
            logMessages.Enqueue("READY FOR PLAYBACK");
            ReadyForPlayback();
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            Dictionary<string, object> musicDelivery = new Dictionary<string, object>();
            musicDelivery.Add("format.channels", format.channels);
            musicDelivery.Add("frames", frames);
            musicDelivery.Add("num_frames", num_frames);

            _backgroundWorkHelper.DoInBackground(MusicDeliveryWorker, MusicDeliveryCompleted, musicDelivery);
            /*
            
            var size = num_frames * format.channels * 2;
            _copiedFrames = new byte[size];
            Marshal.Copy(frames, _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
            _prelistenBufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
            */
        }

        public void EndOfTrack(SpotifySession session)
        {
            logMessages.Enqueue("Finished playing: '" + _currentTrack.Artist(0).Name() + " - " + _currentTrack.Name() + "'.");

            _currentTrackIndex++;
            //GetLoadedTrackWorker(_currentPlaylist.Track(_currentTrackIndex));

            PlaybackEndOfTrack();
        }


        /*---------- BACKGROUNDHELPERS ----------*/
        //Login
        public void LoginWorker(object sender, DoWorkEventArgs e)
        {
            _session.Login((string)((Dictionary<string, object>)e.Argument)["username"], (string)((Dictionary<string, object>)e.Argument)["password"], false, CredentialsBlob);
        }
        public void LoginCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        //Prelisten
        public void PrelistenWorker(object sender, DoWorkEventArgs e)
        {
            Track track = Track.GetPlayable(_session, Link.CreateFromString((String)e.Argument).AsTrack());
            _session.PlayerLoad(track);
            _session.PlayerPrefetch(track);
            _session.PlayerSeek(200);
            _session.PlayerPlay(true);
            _prelistenWaveOutDevice.Play();
            PlaybackStarted();
        }
        public void PrelistenCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        //MusicDelivery
        public void MusicDeliveryWorker(object sender, DoWorkEventArgs e)
        {
            var size = (int)((Dictionary<string, object>)e.Argument)["num_frames"] * (int)((Dictionary<string, object>)e.Argument)["format.channels"] * 2;
            _copiedFrames = new byte[size];
            if (size != 0)
            {
                Marshal.Copy((IntPtr)((Dictionary<string, object>)e.Argument)["frames"], _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
                _prelistenBufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
            }
        }
        public void MusicDeliveryCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

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