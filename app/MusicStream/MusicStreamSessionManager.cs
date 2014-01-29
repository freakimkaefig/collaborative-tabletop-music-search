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
        private SpotifySession _session;
        public MusicStreamSessionListener SessionListener;
        private PlaylistContainer _playlistContainer;
        private MusicStreamPlaylistContainerListener _playlistContainerListener;
        private MusicStreamPlaylistManager _playlistManager;

        public Action<string> receiveLogMessage;
        public Action SpotifyLoggedIn;
        public Action PlaylistContainerLoaded;
        private string _spotifyUsername = "mybleton";
        private string _spotifyPassword = "ctms";
        private string _credentialsBlob = null;

        public ConcurrentQueue<string> logMessages;
        SynchronizationContext syncContext;
        System.Threading.Timer timer;

        private BufferedWaveProvider bufferedWaveProvider;
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

            timer = new System.Threading.Timer(obj => InvokeProcessEvents(_session), null, Timeout.Infinite, Timeout.Infinite);

            //Creating new SpotifySession
            _session = SpotifySession.Create(config);

                     
        }

        /* SETTER & GETTER */
        public string CredentialsBlob
        {
            set { this._credentialsBlob = value; }
            get { return this._credentialsBlob; }
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
            /* Login to Spotify by clicking Login button in interface
             * (called from Ctms.Applications/Workers/MusicStreamAccountWorker.cs - Login() :34)
             */
            _session.Login(_spotifyUsername, _spotifyPassword, true, CredentialsBlob);
        }

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

            Playlist playlist = _playlistContainer.Playlist(0);     //Getting Playlist (CTMS Test)
            var test1 = playlist.Name();                            //Getting Name of Playlist
            var test2 = playlist.Track(0);                          //Getting first Track of Playlist
            var test3 = test2.Artist(0);                            //Getting first Artist of Track
            var test4 = test3.Name();                               //Getting Name of Artist

            Play(test2);    //Play Track
        }

        public void Play(Track track)
        {
            /* Play Track
             * (called from MusicStream/MusicStreamSessionManager.cs - PlaylistContainerLoadedCallback() :120)
             */
            _session.PlayerLoad(track);  //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gac73bf2c569a43d824439b557d5e4b293
            _session.PlayerPlay(true);   //https://developer.spotify.com/docs/libspotify/12.1.45/group__session.html#gab66c5915967e4f90db945b118e620624
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            //http://stackoverflow.com/questions/21307520/playing-ohlibspotify-pcm-data-stream-in-c-sharp-with-naudio
            //format.channels = 2, format.samplerate = 44100, format.sample_type = Int16NativeEndian
            //frames = ?
            //num_frames = 2048

            byte[] frames_copy = new byte[num_frames];
            Marshal.Copy(frames, frames_copy, 0, num_frames);   //Copy Pointer Bytes to frames_copy

            ByteArrayToFile("test.pcm", frames_copy);

            bufferedWaveProvider = new BufferedWaveProvider(new WaveFormat());
            bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(40);
            bufferedWaveProvider.AddSamples(frames_copy, 0, num_frames);    //adding bytes from frames_copy as samples
            //bufferedWaveProvider.Read(frames_copy, 0, num_frames);

            if (_waveOutDeviceInitialized == false)
            {
                IWavePlayer waveOutDevice = new WaveOut();
                waveOutDevice.Init(bufferedWaveProvider);
                waveOutDevice.Play();
                _waveOutDeviceInitialized = true;
            }
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }
    }
}