﻿using System;
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

        private string _credentialsBlob = null;
        private object _userdata = null;
        private ObservableCollection<Playlist> _playlists;

        public ConcurrentQueue<string> logMessages;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;

        //Player
        private IWavePlayer _waveOutDevice;
        private BufferedWaveProvider _bufferedWaveProvider;

        private AudioBufferStats _audioBufferStats;
        private byte[] _copiedFrames;
        private Playlist _currentPlaylist;
        private Track _currentPrelistenTrack;
        private int _currentTrackIndex;
        

        public MusicStreamSessionManager()
        {
            //constructor for MusicStreamSessionManager
            //first object needed for streaming

            _backgroundWorkHelper = new BackgroundWorkHelper();

            syncContext = SynchronizationContext.Current;
            logMessages = new ConcurrentQueue<string>();

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
            _audioBufferStats.samples = _bufferedWaveProvider.BufferedBytes / 2;
            _audioBufferStats.stutter = 0;
            return _audioBufferStats;
        }

        public Track CurrentPrelistenTrack
        {
            set { _currentPrelistenTrack = value; }
            get { return _currentPrelistenTrack; }
        }


        /* ---------- PUBLIC METHODS ---------- */
        public void Login(string username, string password)
        {
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
            if (Track.GetAvailability(_session, track) == TrackAvailability.Available || Track.GetPlayable(_session, track) != null)
            {
                return Track.GetPlayable(_session, track);
            }
            else
            {
                logMessages.Enqueue("Track unavailable: " + spotifyTrackId);
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
            _backgroundWorkHelper.DoInBackground(OpenPlaylistWorker, OpenPlaylistCompleted, playlist);
        }

        public void CreatePlaylist(string name)
        {
            _backgroundWorkHelper.DoInBackground(CreatePlaylistWorker, CreatePlaylistCompleted, name);
        }

        public void AddTrackToPlaylist(Playlist playlist, string spotifyTrackId)
        {
            object[] data = new object[2] { playlist, spotifyTrackId };
            _backgroundWorkHelper.DoInBackground(AddTrackToPlaylistWorker, AddTrackToPlaylistCompleted, data);
        }

        public void JumpToTrackInPlaylist(Playlist playlist, int index)
        {
            _currentPlaylist = playlist;
            _currentTrackIndex = index;
            _bufferedWaveProvider.ClearBuffer();
            _session.PlayerLoad(playlist.Track(index));
            _session.PlayerPlay(true);
            _waveOutDevice.Play();
            PlaybackStarted();
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
            _currentTrackIndex++;
            _bufferedWaveProvider.ClearBuffer();
            _session.PlayerLoad(_currentPlaylist.Track(_currentTrackIndex));
            _session.PlayerPlay(true);
            _waveOutDevice.Play();
            PlaybackStarted();
        }


        /* ---------- CALLBACKS ---------- */
        public void LoggedInCallback()
        {
            /* Callback, when successfully logged in to Spotify
             * creating new PlaylistContainer for streaming or playlist operations
             */
            SpotifyLoggedIn();  //Notify MenuController

            _playlistContainer = _session.Playlistcontainer();
            _playlistContainerListener = new MusicStreamPlaylistContainerListener(this);
            _playlistContainer.AddCallbacks(_playlistContainerListener, Userdata);
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
            logMessages.Enqueue("READY FOR PLAYBACK");
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
            PlaybackEndOfTrack();
            ProceedPlayingPlaylist();
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

        private void OpenPlaylistWorker(object sender, DoWorkEventArgs e)
        {
            _playlistListener = new MusicStreamPlaylistListener(this);
            ((Playlist)e.Argument).AddCallbacks(_playlistListener, Userdata);
            e.Result = (Playlist)e.Argument;
        }
        private void OpenPlaylistCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logMessages.Enqueue("Playlist '" + ((Playlist)e.Result).Name() + "' opened");
            for (var i = 0; i < ((Playlist)e.Result).NumTracks(); i++)
            {
                logMessages.Enqueue("Track " + i + ": " + ((Playlist)e.Result).Track(i).Artist(0).Name() + " - " + ((Playlist)e.Result).Track(i).Name());
            }

            _currentPlaylist = (Playlist)e.Result;
            PlaylistOpened((Playlist)e.Result);
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
            Playlist playlist = (Playlist)((object[])e.Argument)[0];
            string spotifyTrackId = (string)((object[])e.Argument)[1];
            Link link = Link.CreateFromString(spotifyTrackId);
            playlist.AddTracks(new Track[1] { Track.GetPlayable(_session, link.AsTrack()) }, playlist.NumTracks(), _session);
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