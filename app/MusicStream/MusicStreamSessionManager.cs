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
    /// <summary>
    /// main class for handling streaming from spotify
    /// accessed via ViewModels or Workers from outside
    /// 
    /// </summary>
    public class MusicStreamSessionManager
    {
        private BackgroundWorkHelper _backgroundWorkHelper;
        private SpotifySession _session;
        public MusicStreamSessionListener SessionListener;
        private PlaylistContainer _playlistContainer;
        private MusicStreamPlaylistContainerListener _playlistContainerListener;
        private MusicStreamPlaylistListener _playlistListener;

        //Actions
        public Action<string> ReceiveLogMessage;
        public Action<SpotifyError> SpotifyError;
        public Action SpotifyLoggedIn;
        public Action SpotifyLoggedOut;
        public Action<ObservableCollection<Playlist>> ReadyForPlayback;
        public Action<Playlist> PlaylistOpened;
        public Action PrelistenStarted;
        public Action PrelistenStopped;
        public Action PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;
        public Action<int> PlaylistTrackRemoved;

        private string _credentialsBlob = null;
        private object _userdata = null;
        private ObservableCollection<Playlist> _playlists;

        public ConcurrentQueue<string> logMessages;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;
        private Random _random;

        //Player
        private IWavePlayer _waveOutDevice;
        private BufferedWaveProvider _bufferedWaveProvider;

        private AudioBufferStats _audioBufferStats;
        private byte[] _copiedFrames;
        private Playlist _currentPlaylist;
        private Track _currentPrelistenTrack;
        private int _currentTrackIndex;

        private bool _isShuffle = false;
        private bool _isRepeat = false;
        

        public MusicStreamSessionManager()
        {
            //constructor for MusicStreamSessionManager
            //first object needed for streaming

            _backgroundWorkHelper = new BackgroundWorkHelper();

            syncContext = SynchronizationContext.Current;
            logMessages = new ConcurrentQueue<string>();
            _random = new Random();

            //configuration for SpotifySession
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
            _session.FlushCaches();
        }

        //SETTER & GETTER
        public SpotifySession Session
        {
            set { _session = value; }
            get { return _session; }
        }

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
            _audioBufferStats.samples = _bufferedWaveProvider.BufferedBytes / 2;
            _audioBufferStats.stutter = 0;
            return _audioBufferStats;
        }

        public Track CurrentPrelistenTrack
        {
            set { _currentPrelistenTrack = value; }
            get { return _currentPrelistenTrack; }
        }

        public bool IsShuffle
        {
            set { _isShuffle = value; }
            get { return _isShuffle; }
        }

        public bool IsRepeat
        {
            set { _isRepeat = value; }
            get { return _isRepeat; }
        }


        /* ---------- PUBLIC METHODS ---------- */
        public void Login(string username, string password)
        {
            //logMessages.Enqueue("MusicStreamSessionManager.Login");
            //handles logging in to spotify
            //accessed through menu

            Dictionary<string, object> credentials = new Dictionary<string, object>();
            credentials.Add("username", username);
            credentials.Add("password", password);
            _backgroundWorkHelper.DoInBackground(LoginWorker, LoginCompleted, credentials);
            //_session.Login(_spotifyUsername, _spotifyPassword, true, CredentialsBlob);
        }

        public void Logout()
        {
            //handles logging out from spotify

            _backgroundWorkHelper.DoInBackground(LogoutWorker, LogoutCompleted);
        }

        public Track CheckTrackAvailability(string spotifyTrackId)
        {
            Track track = Link.CreateFromString(spotifyTrackId).AsTrack();
            if (Track.GetAvailability(_session, track) == TrackAvailability.Available && Track.GetPlayable(_session, track) != null)
            {
                return Track.GetPlayable(_session, track);
            }
            else
            {
                var avail = Track.GetAvailability(_session, track);
                //logMessages.Enqueue("Track unavailable: " + spotifyTrackId);
                return null;
            }
        }

        public void StartPrelisteningTrack(Track track)
        {  
            _backgroundWorkHelper.DoInBackground(PrelistenPlayWorker, PrelistenPlayCompleted, track);
        }
        public void StopPrelisteningTrack()
        {
            _backgroundWorkHelper.DoInBackground(PrelistenStopWorker, PrelistenStopCompleted);
        }

        public void OpenPlaylists(Playlist playlist)
        {
            //logMessages.Enqueue("MusicStreamSessionManager.OpenPlaylists");
            //_backgroundWorkHelper.DoInBackground(OpenPlaylistWorker, OpenPlaylistCompleted, playlist);

            _playlistListener = new MusicStreamPlaylistListener(this);
            playlist.AddCallbacks(_playlistListener, Userdata);
            _currentPlaylist = playlist;
            PlaylistOpened(playlist);
        }

        public void CreatePlaylist(string name)
        {
            //logMessages.Enqueue("MusicStreamSessionManager.CreatePlaylist");
            _backgroundWorkHelper.DoInBackground(CreatePlaylistWorker, CreatePlaylistCompleted, name);
        }

        public void AddTrackToPlaylist(Playlist playlist, Track spotifyTrack)
        {
            object[] data = new object[2] { playlist, spotifyTrack };
            _backgroundWorkHelper.DoInBackground(AddTrackToPlaylistWorker, AddTrackToPlaylistCompleted, data);
        }

        public void JumpToTrackInPlaylist(Playlist playlist, int index)
        {
            _currentPlaylist = playlist;
            _currentTrackIndex = index;
            _bufferedWaveProvider.ClearBuffer();
            try
            {
                _session.PlayerLoad(playlist.Track(index));
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                PlaybackStarted();
            }
            catch (SpotifyException spotifyException)
            {
                int[] tracks = new int[1] {index};
                playlist.RemoveTracks(tracks);
                PlaylistTrackRemoved(index);
                JumpToTrackInPlaylist(playlist, index + 1);
            }
        }
        

        /*public void PlayTrack(Track track)
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
            if (IsShuffle)
            {
                //choose next track by random
                var test = _random.Next(0, 100);
                _currentTrackIndex = _random.Next(0, _currentPlaylist.NumTracks());
            }
            else
            {
                //next track in the list
                _currentTrackIndex++;
            }

            if (_currentTrackIndex >= _currentPlaylist.NumTracks())
            {
                //finished track is last of the list
                if (IsRepeat)
                {
                    //restart playlist from beginning
                    _currentTrackIndex = 0;
                    _bufferedWaveProvider.ClearBuffer();
                    _session.PlayerLoad(_currentPlaylist.Track(_currentTrackIndex));
                    _session.PlayerPlay(true);
                    _waveOutDevice.Play();
                    PlaybackStarted();
                }
                else
                {
                    //stop playback after last track
                    _session.PlayerPlay(false);
                    _session.PlayerUnload();
                    _waveOutDevice.Stop();
                    return;
                }
            }
            else
            {
                //play next track
                _bufferedWaveProvider.ClearBuffer();
                _session.PlayerLoad(_currentPlaylist.Track(_currentTrackIndex));
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                PlaybackStarted();
            }
        }

        public void ReorderTrack(int oldIndex, int newIndex)
        {
            int[] tracks = new int[] { oldIndex };
            _currentPlaylist.ReorderTracks(tracks, newIndex);
        }


        /* ---------- CALLBACKS ---------- */
        public void LoggedInCallback()
        {
            //logMessages.Enqueue("MusicStreamSessionManager.LoggedInCallback");
            /* Callback, when successfully logged in to Spotify
             * creating new PlaylistContainer for streaming or playlist operations
             */
            SpotifyLoggedIn();  //Notify MenuController

            if (_playlistContainer == null)
            {
                _playlistContainer = _session.Playlistcontainer();
            }

            if (_playlistContainerListener == null)
            {
                _playlistContainerListener = new MusicStreamPlaylistContainerListener(this);
                _playlistContainer.AddCallbacks(_playlistContainerListener, Userdata);
            }
        }

        public void LoggedOutCallback()
        {
            SpotifyLoggedOut();
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
            _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat()); //Create new Buffer
            _bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(240);
            //_copiedFrames = new byte[5000];
            _audioBufferStats = new AudioBufferStats(); //Create stats for Spotify
            _waveOutDevice = new WaveOut(); //Create new AudioDevice
            _waveOutDevice.Init(_bufferedWaveProvider);

            //Notify PlaylistWorker that Playback is ready to start!!!
            logMessages.Enqueue("SPOTIFY IS READY");
            ReadyForPlayback(_playlists);
        }

        public void PlaylistAddedCallback(Playlist playlist)
        {

        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            //handle received music data from spotify for streaming
            //format: audio format for streaming
            //frames: pointer to the byte-data in storage

            var size = num_frames * format.channels * 2;
            if (size != 0)
            {
                _copiedFrames = new byte[size];
                Marshal.Copy(frames, _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
                _bufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples
            }
        }

        public void EndOfTrack(SpotifySession session)
        {
            if (_currentPlaylist != null)
            {
                PlaybackEndOfTrack();
                ProceedPlayingPlaylist();
            }
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

        private void LogoutWorker(object sender, DoWorkEventArgs e)
        {
            _session.Logout();
        }
        private void LogoutCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        //Prelisten
        private void PrelistenPlayWorker(object sender, DoWorkEventArgs e)
        {
            try
            {
                _session.PlayerLoad((Track)e.Argument);
                _session.PlayerPrefetch((Track)e.Argument);
                _session.PlayerSeek((((Track)e.Argument).Duration()) / 2);    //Seek to half of the song for prelistening
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                e.Result = e.Argument;
            }
            catch (SpotifyException spotifyException)
            {
                logMessages.Enqueue("SpotifyException: " + spotifyException.Message);
            }
        }
        private void PrelistenPlayCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //PlaybackStarted();
            CurrentPrelistenTrack = (Track)e.Result;
            PrelistenStarted();
        }

        private void PrelistenStopWorker(object sender, DoWorkEventArgs e)
        {
            _session.PlayerPlay(false);
            _session.PlayerUnload();
            _waveOutDevice.Stop();
            _bufferedWaveProvider.ClearBuffer();
        }
        private void PrelistenStopCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PrelistenStopped();
        }

        private void CreatePlaylistWorker(object sender, DoWorkEventArgs e)
        {
            e.Result = (Playlist)_playlistContainer.AddNewPlaylist((string)e.Argument);
        }
        private void CreatePlaylistCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _currentPlaylist = (Playlist)e.Result;
            PlaylistOpened((Playlist)e.Result);
        }

        private void AddTrackToPlaylistWorker(object sender, DoWorkEventArgs e)
        {
            ((Playlist)((object[])e.Argument)[0]).AddTracks(new Track[1] { (Track)((object[])e.Argument)[1] }, ((Playlist)((object[])e.Argument)[0]).NumTracks(), _session);
            e.Result = e.Argument;
        }
        private void AddTrackToPlaylistCompleted(object sender, RunWorkerCompletedEventArgs e)
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
                ReceiveLogMessage(message);
            }
            while (timeout == 0)
            {
                session.ProcessEvents(ref timeout);
            }
            timer.Change(timeout, Timeout.Infinite);
        }


    }
}