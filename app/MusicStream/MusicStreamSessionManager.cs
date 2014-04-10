using System;
using SpotifySharp;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using NAudio.Wave;
using System.Runtime.InteropServices;
using System.Collections.Generic;
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
        private MusicStreamVisualizationManager _visualizationManager;

        //Actions
        public Action<string> ReceiveLogMessage;
        public Action<SpotifyError> SpotifyError;
        public Action SpotifyLoggedIn;
        public Action SpotifyLoggedOut;
        public Action<ObservableCollection<Playlist>> ReadyForPlayback;
        public Action<Playlist> PlaylistOpened;
        public Action<Track> PrelistenStarted;
        public Action PrelistenStopped;
        public Action<Track> PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;
        public Action<int> PlaylistTrackRemoved;
        public Action<Track> PrelistenLoadingReady;
        public Action<Track> PlaybackLoadingReady;

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
        private int _currentPlaylistTrackIndex = 0;
        private double _currentPlaylistTrackPlayedDuration = 0.0;
        private bool _playlistPlaying = false;
        private bool _prelistPlaying = false;
        private Track _currentPrelistenTrack;

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
        /// <summary>
        /// Getter&Setter for the current SpotifySession
        /// </summary>
        public SpotifySession Session
        {
            set { _session = value; }
            get { return _session; }
        }

        public MusicStreamPlaylistContainerListener PlaylistContainerListener
        {
            set { _playlistContainerListener = value; }
            get { return _playlistContainerListener; }
        }

        /// <summary>
        /// Getter&Setter for the current CredentialsBlob
        /// </summary>
        public string CredentialsBlob
        {
            set { this._credentialsBlob = value; }
            get { return this._credentialsBlob; }
        }

        /// <summary>
        /// Getter&Setter for the current Userdata
        /// </summary>
        public object Userdata
        {
            set { this._userdata = value; }
            get { return this._userdata; }
        }

        /// <summary>
        /// Returns current AudioBufferStats
        /// </summary>
        /// <returns>int samples; int stutter</returns>
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

        public MusicStreamVisualizationManager VisualizationManager
        {
            set { _visualizationManager = value; }
            get { return _visualizationManager; }

        }


        /* ---------- PUBLIC METHODS ---------- */
        /// <summary>
        /// Handles Logging in to Spotify
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
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

        /// <summary>
        /// Handles logout from Spotify
        /// </summary>
        public void Logout()
        {
            //handles logging out from spotify

            _backgroundWorkHelper.DoInBackground(LogoutWorker, LogoutCompleted);
        }

        /// <summary>
        /// Checks if track is available for streaming from Spotify
        /// </summary>
        /// <param name="spotifyTrackId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Starts streaming passed track from middle (prelistening)
        /// </summary>
        /// <param name="track"></param>
        public void StartPrelisteningTrack(Track track)
        {
            _prelistPlaying = true;
            _backgroundWorkHelper.DoInBackground(PrelistenPlayWorker, PrelistenPlayCompleted, track);
        }

        /// <summary>
        /// Stops prelistening of current prelistened track
        /// </summary>
        public void StopPrelisteningTrack()
        {
            _backgroundWorkHelper.DoInBackground(PrelistenStopWorker, PrelistenStopCompleted);
        }

        /// <summary>
        /// Open passed playlist from Spotify
        /// </summary>
        /// <param name="playlist"></param>
        public void OpenPlaylists(Playlist playlist)
        {
            //logMessages.Enqueue("MusicStreamSessionManager.OpenPlaylists");
            //_backgroundWorkHelper.DoInBackground(OpenPlaylistWorker, OpenPlaylistCompleted, playlist);

            _playlistListener = new MusicStreamPlaylistListener(this);
            playlist.AddCallbacks(_playlistListener, Userdata);
            _currentPlaylist = playlist;
            PlaylistOpened(playlist);
        }

        /// <summary>
        /// Create and open a new playlist with the passed name for the currently logged in user
        /// </summary>
        /// <param name="name"></param>
        public void CreatePlaylist(string name)
        {
            //logMessages.Enqueue("MusicStreamSessionManager.CreatePlaylist");
            _backgroundWorkHelper.DoInBackground(CreatePlaylistWorker, CreatePlaylistCompleted, name);
        }

        /// <summary>
        /// Add passed track to passed playlist in Spotify
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="spotifyTrack"></param>
        public void AddTrackToPlaylist(Playlist playlist, Track spotifyTrack)
        {
            object[] data = new object[2] { playlist, spotifyTrack };
            _backgroundWorkHelper.DoInBackground(AddTrackToPlaylistWorker, AddTrackToPlaylistCompleted, data);
        }

        /// <summary>
        /// Remove track on given position from given playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="index"></param>
        public void RemoveTrackFromPlaylist(Playlist playlist, int index)
        {
            int[] tracks = new int[] { index };
            playlist.RemoveTracks(tracks);
        }

        /// <summary>
        /// Moves to passed track(index) in passed playlist
        /// </summary>
        /// <param name="playlist"></param>
        /// <param name="index"></param>
        public void JumpToTrackInPlaylist(Playlist playlist, int index)
        {
            _currentPlaylist = playlist;
            _currentPlaylistTrackIndex = index;
            _currentPlaylistTrackPlayedDuration = 0;
            _bufferedWaveProvider.ClearBuffer();
            try
            {
                _session.PlayerLoad(playlist.Track(index));
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                PlaybackStarted(_currentPlaylist.Track(_currentPlaylistTrackIndex));
                _playlistPlaying = true;
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

        public void PlaylistPlay()
        {
            if (_currentPlaylist != null)
            {
                //if playlist is selected
                if (_currentPlaylistTrackIndex != 0)
                {
                    //if trackIndex is already set
                    if (_currentPlaylistTrackPlayedDuration != 0)
                    {
                        //if track was played before
                        ProceedPlayingPlaylistAfterPrelisten();
                    }
                    else
                    {
                        //if no track was played before
                        JumpToTrackInPlaylist(_currentPlaylist, _currentPlaylistTrackIndex);
                    }
                }
                else
                {
                    //if trackIndex is not set
                    _currentPlaylistTrackIndex = 0;
                    JumpToTrackInPlaylist(_currentPlaylist, _currentPlaylistTrackIndex);
                }
            }
            else
            {
                //if no playlist is selected
                throw new NullReferenceException("No Playlist selected! Cannot play!");
            }
        }

        public void PlaylistPause()
        {
            _waveOutDevice.Pause();
            _session.PlayerPlay(false);
        }

        public void PlaylistStop()
        {
            _waveOutDevice.Stop();
            _session.PlayerPlay(false);
            _session.PlayerUnload();
            _bufferedWaveProvider.ClearBuffer();
            _currentPlaylistTrackIndex = 0;
            _currentPlaylistTrackPlayedDuration = 0.0;
        }

        public void StopTrack()
        {
            _waveOutDevice.Stop();
            _session.PlayerPlay(false);
            _session.PlayerUnload();
            _bufferedWaveProvider.ClearBuffer();
        }

        public void ProceedPlayingPlaylist()
        {
            if (IsShuffle)
            {
                //choose next track by random
                var test = _random.Next(0, 100);
                _currentPlaylistTrackIndex = _random.Next(0, _currentPlaylist.NumTracks());
            }
            else
            {
                //next track in the list
                _currentPlaylistTrackIndex++;
            }

            if (_currentPlaylistTrackIndex >= _currentPlaylist.NumTracks())
            {
                //finished track is last of the list
                if (IsRepeat)
                {
                    //restart playlist from beginning
                    _currentPlaylistTrackIndex = 0;
                    _bufferedWaveProvider.ClearBuffer();
                    _session.PlayerLoad(_currentPlaylist.Track(_currentPlaylistTrackIndex));
                    _session.PlayerPlay(true);
                    _waveOutDevice.Play();
                    PlaybackStarted(_currentPlaylist.Track(_currentPlaylistTrackIndex));
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
                _session.PlayerLoad(_currentPlaylist.Track(_currentPlaylistTrackIndex));
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                PlaybackStarted(_currentPlaylist.Track(_currentPlaylistTrackIndex));
            }

            _currentPlaylistTrackPlayedDuration = 0;
        }

        public void ProceedPlayingPlaylistAfterPrelisten()
        {
            
            _bufferedWaveProvider.ClearBuffer();
            _session.PlayerLoad(_currentPlaylist.Track(_currentPlaylistTrackIndex));
            _session.PlayerSeek((int)_currentPlaylistTrackPlayedDuration);
            _session.PlayerPlay(true);
            _waveOutDevice.Play();
            PlaybackStarted(_currentPlaylist.Track(_currentPlaylistTrackIndex));
        }

        /// <summary>
        /// Reorder tracks in passed playlist
        /// </summary>
        /// <param name="playlist">Playlist from SpotifySharp</param>
        /// <param name="oldIndex">index of track to reorder</param>
        /// <param name="newIndex">index of track where to put</param>
        public void ReorderTrack(Playlist playlist, int oldIndex, int newIndex)
        {
            int[] tracks = new int[] { oldIndex };
            playlist.ReorderTracks(tracks, newIndex);
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

            //calculate how much seconds already received;
            double howMuchSecs = 0.0;
            if (_currentPlaylist != null)
            {
                int duration = _currentPlaylist.Track(_currentPlaylistTrackIndex).Duration();
                var bytesPerSec = (format.sample_rate * 16 * format.channels) / 8;
                howMuchSecs = (((double)num_frames * 2.0 * 2.0) / (double)bytesPerSec) * 1000.0;
                _currentPlaylistTrackPlayedDuration += (int)howMuchSecs;
            }
            else
            {
                int duration = _currentPrelistenTrack.Duration();
                var bytesPerSec = (format.sample_rate * 16 * format.channels) / 8;
                howMuchSecs = (((double)num_frames * 2.0 * 2.0) / (double)bytesPerSec) * 1000.0;
                //_currentPrelistenTrackDuration += (int)howMuchSecs;
            }
            //_currentPlaylistTrackPlayedDuration += _bufferedWaveProvider.BufferedDuration.TotalSeconds;
            //logMessages.Enqueue("Received: " + _currentPlaylistTrackPlayedDuration + " / " + duration);

            var size = num_frames * format.channels * 2;
            if (size != 0)
            {
                _copiedFrames = new byte[size];
                Marshal.Copy(frames, _copiedFrames, 0, size);   //Copy Pointer Bytes to _copiedFrames
                _bufferedWaveProvider.AddSamples(_copiedFrames, 0, size);    //adding bytes from _copiedFrames as samples

                if (NumberHelper.IsPowerOfTwo(num_frames))
                {
                    _visualizationManager.MusicDeliveryCallback(format, _copiedFrames, num_frames, howMuchSecs);
                }
                else
                {
                    var newNumFrames = NumberHelper.GetPowerOfTwoLessThanOrEqualTo(num_frames);
                    var newSize = newNumFrames  * format.channels * 2;
                    byte[] temp = new byte[newSize];
                    Array.Copy(_copiedFrames, 0, temp, 0, newSize);
                    _visualizationManager.MusicDeliveryCallback(format, temp, newNumFrames, howMuchSecs);
                    //http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
                }
            }

            if (_playlistPlaying)
            {
                PlaybackLoadingReady(_currentPlaylist.Track(_currentPlaylistTrackIndex));
            }
            if (_prelistPlaying)
            {
                PrelistenLoadingReady(_currentPrelistenTrack);
            }
        }

        public void EndOfTrack(SpotifySession session)
        {
            if (_currentPlaylist != null)
            {
                if (!_prelistPlaying)
                {
                    PlaybackEndOfTrack();
                    ProceedPlayingPlaylist();
                }
                else
                {
                    ProceedPlayingPlaylistAfterPrelisten();
                }
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
            if (_session != null)
            {
                _session.Logout();
            }
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
            PrelistenStarted(_currentPrelistenTrack);
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