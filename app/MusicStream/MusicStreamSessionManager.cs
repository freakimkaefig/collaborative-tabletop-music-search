﻿using System;
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
        public Action<String> PrelistenStarted;
        public Action PrelistenStopped;
        public Action<String> PlaybackStarted;
        public Action PlaybackPaused;
        public Action PlaybackStopped;
        public Action PlaybackEndOfTrack;
        public Action<int> PlaylistTrackRemoved;
        public Action<String> PrelistenLoadingReady;
        private bool _firstPrelistenLoadingReady;
        public Action<String> PlaybackLoadingReady;
        private bool _firstPlaybackLoadingReady;
        public Action<String> ShowError;

        private string _credentialsBlob = null;
        private object _userdata = null;
        private ObservableCollection<Playlist> _playlists;

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
        private String _currentPrelistenTrack;

        private bool _isShuffle = false;
        private bool _isRepeat = false;
        

        public MusicStreamSessionManager()
        {
            //constructor for MusicStreamSessionManager
            //first object needed for streaming

            _backgroundWorkHelper = new BackgroundWorkHelper();

            syncContext = SynchronizationContext.Current;
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

        public String CurrentPrelistenTrack
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
        public String CheckTrackAvailability(string spotifyTrackId)
        {
            Link link = Link.CreateFromString(spotifyTrackId);
            if (link != null)
            {
                Track track = link.AsTrack();

                if (track != null)
                {
                    if (Track.GetAvailability(_session, track) == TrackAvailability.Available || Track.GetPlayable(_session, track) != null)
                    {
                        return spotifyTrackId;
                    }
                    else
                    {
                        var avail = Track.GetAvailability(_session, track);
                        //logMessages.Enqueue("Track unavailable: " + spotifyTrackId);
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
            
        }

        /// <summary>
        /// Starts streaming passed track from middle (prelistening)
        /// </summary>
        /// <param name="track"></param>
        public void StartPrelisteningTrack(String track)
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
            _backgroundWorkHelper.DoInBackground(OpenPlaylistWorker, OpenPlaylistCompleted, playlist);
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
        public void AddTrackToPlaylist(Playlist playlist, String spotifyTrack)
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
                PlaybackStarted(Link.CreateFromTrack(_currentPlaylist.Track(_currentPlaylistTrackIndex), 0).AsString());
                _playlistPlaying = true;
                _firstPlaybackLoadingReady = true;
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

        /// <summary>
        /// Start playlist
        /// </summary>
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
                //throw new NullReferenceException("No Playlist selected! Cannot play!");
            }
        }

        /// <summary>
        /// pause playlist
        /// </summary>
        public void PlaylistPause()
        {
            _waveOutDevice.Pause();
            _session.PlayerPlay(false);
        }

        /// <summary>
        /// stop playlist
        /// </summary>
        public void PlaylistStop()
        {
            _waveOutDevice.Stop();
            _session.PlayerPlay(false);
            _session.PlayerUnload();
            _bufferedWaveProvider.ClearBuffer();
            _currentPlaylistTrackIndex = 0;
            _currentPlaylistTrackPlayedDuration = 0.0;
            PlaybackStopped();
        }

        public void StopTrack()
        {
            _waveOutDevice.Stop();
            _session.PlayerPlay(false);
            _session.PlayerUnload();
            _bufferedWaveProvider.ClearBuffer();
        }

        /// <summary>
        /// Method to continue with playlist after end of track callback
        /// </summary>
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
                    PlaybackStarted(Link.CreateFromTrack(_currentPlaylist.Track(_currentPlaylistTrackIndex), 0).AsString());
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
                PlaybackStarted(Link.CreateFromTrack(_currentPlaylist.Track(_currentPlaylistTrackIndex), 0).AsString());
            }

            _currentPlaylistTrackPlayedDuration = 0;
        }

        /// <summary>
        /// return to playlist, when prelistening is finished
        /// </summary>
        public void ProceedPlayingPlaylistAfterPrelisten()
        {
            
            _bufferedWaveProvider.ClearBuffer();
            _session.PlayerLoad(_currentPlaylist.Track(_currentPlaylistTrackIndex));
            _session.PlayerSeek((int)_currentPlaylistTrackPlayedDuration);
            _session.PlayerPlay(true);
            _waveOutDevice.Play();
            PlaybackStarted(Link.CreateFromTrack(_currentPlaylist.Track(_currentPlaylistTrackIndex), 0).AsString());
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
            try
            {
                playlist.ReorderTracks(tracks, newIndex);
            }
            catch (AccessViolationException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /* ---------- CALLBACKS ---------- */
        public void LoggedInCallback()
        {
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

        /// <summary>
        /// Callback for PlaylistContainer. If ready, spotify is ready for action
        /// </summary>
        public void PlaylistContainerLoadedCallback()
        {
            //Retrieving available user playlists
            _playlists = new ObservableCollection<Playlist>();
            for (int i = 0; i < _playlistContainer.NumPlaylists(); i++)
            {
                _playlists.Add(_playlistContainer.Playlist(i));
            }

            //Create Buffer, Stats & AudioDevice
            _bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat()); //Create new Buffer
            _bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(240);
            _audioBufferStats = new AudioBufferStats(); //Create stats for Spotify
            _waveOutDevice = new WaveOut(); //Create new AudioDevice
            _waveOutDevice.Init(_bufferedWaveProvider);

            //Notify PlaylistWorker that Playback is ready to start!!!;
            ReadyForPlayback(_playlists);
        }

        public void PlaylistAddedCallback(Playlist playlist)
        {

        }

        /// <summary>
        /// Callback when music data is delivered by spotify
        /// </summary>
        /// <param name="session"></param>
        /// <param name="format"></param>
        /// <param name="frames"></param>
        /// <param name="num_frames"></param>
        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            //handle received music data from spotify for streaming
            //format: audio format for streaming
            //frames: pointer to the byte-data in storage

            //calculate how much seconds already received;
            double howMuchSecs = 0.0;
            if (_currentPlaylist != null)
            {
                Track track = _currentPlaylist.Track(_currentPlaylistTrackIndex);
                if (track != null)
                {
                    //Calculate how much seconds of the current track is already played
                    int duration = track.Duration();
                    var bytesPerSec = (format.sample_rate * 16 * format.channels) / 8;
                    howMuchSecs = (((double)num_frames * 2.0 * 2.0) / (double)bytesPerSec) * 1000.0;
                    _currentPlaylistTrackPlayedDuration += (int)howMuchSecs;
                }
            }
            else
            {
                int duration = Link.CreateFromString(_currentPrelistenTrack).AsTrack().Duration();
                var bytesPerSec = (format.sample_rate * 16 * format.channels) / 8;
                howMuchSecs = (((double)num_frames * 2.0 * 2.0) / (double)bytesPerSec) * 1000.0;
            }

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
                }
            }

            if (_playlistPlaying && _firstPlaybackLoadingReady)
            {
                PlaybackLoadingReady(Link.CreateFromTrack(_currentPlaylist.Track(_currentPlaylistTrackIndex), 0).AsString());
                _firstPlaybackLoadingReady = false;
            }
            if (_prelistPlaying && _firstPrelistenLoadingReady)
            {
                PrelistenLoadingReady(_currentPrelistenTrack);
                _firstPrelistenLoadingReady = false;
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

        //Playlist
        private void OpenPlaylistWorker(object sender, DoWorkEventArgs e)
        {
            Playlist playlist = (Playlist)e.Argument;
            PlaylistStop();

            _playlistListener = new MusicStreamPlaylistListener(this);
            playlist.AddCallbacks(_playlistListener, Userdata);
            _currentPlaylist = playlist;
            e.Result = playlist;
        }
        private void OpenPlaylistCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PlaylistOpened((Playlist)e.Result);
        }

        //Prelisten
        private void PrelistenPlayWorker(object sender, DoWorkEventArgs e)
        {
            try
            {
                String trackId = (String)e.Argument;
                Track track = Link.CreateFromString(trackId).AsTrack();
                _session.PlayerLoad(track);
                _session.PlayerPrefetch(track);
                _session.PlayerSeek(track.Duration() / 2);    //Seek to half of the song for prelistening
                _session.PlayerPlay(true);
                _waveOutDevice.Play();
                e.Result = e.Argument;
            }
            catch (SpotifyException exception)
            {
                // Exception get caught in StreamingWorker
            }
            catch (AccessViolationException exception)
            {
                // write exception to console.
                // no further interaction
                Console.WriteLine(exception.Message);
            }
        }
        private void PrelistenPlayCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            CurrentPrelistenTrack = (String)e.Result;
            PrelistenStarted(_currentPrelistenTrack);
            _firstPrelistenLoadingReady = true;
        }

        private void PrelistenStopWorker(object sender, DoWorkEventArgs e)
        {
            try
            {
                _session.PlayerPlay(false);
                _session.PlayerUnload();
                _waveOutDevice.Stop();
                _bufferedWaveProvider.ClearBuffer();
            }
            catch (AccessViolationException exception)
            {
                Console.WriteLine(exception.Message);
            }
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
            String trackId = (String)((object[])e.Argument)[1];
            Track track = Link.CreateFromString(trackId).AsTrack();
            ((Playlist)((object[])e.Argument)[0]).AddTracks(new Track[1] { track }, ((Playlist)((object[])e.Argument)[0]).NumTracks(), _session);
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
        // notifying main thread
        void ProcessEvents(SpotifySession session)
        {
            this._session = session;
            int timeout = 0;
            while (timeout == 0)
            {
                session.ProcessEvents(ref timeout);
            }
            timer.Change(timeout, Timeout.Infinite);
        }


    }
}