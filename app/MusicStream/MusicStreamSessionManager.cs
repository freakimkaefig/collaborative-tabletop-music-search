using System;
using SpotifySharp;
using System.Threading;
using System.IO;
using System.Collections.Concurrent;
using System.Collections;
using NAudio;
using NAudio.Wave;
using System.Runtime.InteropServices;

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
        }

        public void PlaylistContainerLoadedCallback()
        {

            Playlist playlist = _playlistContainerManager.AddNewPlaylist("Test Playlist");
            _playlistManager = new MusicStreamPlaylistManager(this, playlist);

            _playlistManager.AddTracksToPlaylist(session, playlist, GetTestTracks());

            //_playlistContainerManager.Container.MovePlaylist(0, 0, true); //Move playlist to folder
        }

        public void PlaylistTracksAddedCallback(Playlist playlist)
        {
            logMessages.Enqueue("PlaylistTracksAddedCallback: " + _playlistManager.GetPlaylistMetadata(playlist));
            Play(playlist.Track(0));
        }

        private Track[] GetTestTracks()
        {
            Track katy = Track.GetPlayable(session, Link.CreateFromString("spotify:track:4lCv7b86sLynZbXhfScfm2").AsTrack());
            Track miley = Track.GetPlayable(session, Link.CreateFromString("spotify:track:6oDPg7fXW3Ug3KmbafrXzA").AsTrack());
            Track[] tracks = new Track[2];
            tracks[0] = katy;
            tracks[1] = miley;

            return tracks;
        }

        public void Play(Track track)
        {
            session.PlayerLoad(track);
            session.PlayerPlay(true);
        }

        public void MusicDeliveryCallback(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            /*  //Writing Spotify Data in .pcm File
            if (num_frames != 0)
            {
                var numBytesToWrite = (num_frames) * format.channels * sizeof(short);
                if (numBytesToWrite > 0)
                {
                    fileStream = new FileStream("test.pcm", FileMode.OpenOrCreate);
                    //file = File.Create("test/file.pcm");

                    Int32 numsamples = num_frames * format.channels;
                    //fileStream = new FileStream(frames, FileAccess.ReadWrite);
                    //fileStream.Write(
                    //fwrite(frames, sizeof(short), numSamplesToWrite, pFile);

                    //fileStream = new FileStream(frames, FileAccess.ReadWrite, false, numsamples);

                    var wavm = new Int16[numsamples];
                    Marshal.Copy(frames, wavm, 0, numsamples);
                    // and do something with wavm

                    //or
                    var wavbytes = new Byte[numsamples * 2];
                    Marshal.Copy(frames, wavbytes, 0, numsamples * 2);
                    fileStream.Write(wavbytes, 0, numsamples * 2);
                    fileStream.Close();
                }
            }
            */

            /*  http://stackoverflow.com/questions/11452236/how-to-play-a-spotify-music-stream
            IWavePlayer waveOutDevice = new WaveOut();

            using (var pcmStream = new FileStream("test.pcm", FileMode.OpenOrCreate))
            {
                //int songDuration = 3000;
                int sampleRate = format.sample_rate;
                int channels = format.channels;
                var waveFormat = WaveFormat.CreateCustomFormat(WaveFormatEncoding.Pcm, sampleRate * channels, 1, sampleRate * 2 * channels, channels, 16);
                var waveStream = new RawSourceWaveStream(pcmStream, waveFormat);

                waveOutDevice.Init(waveStream);
                waveOutDevice.Play();
                Thread.Sleep(songDuration);
                waveOutDevice.Stop();
                waveStream.Close();
                waveOutDevice.Dispose();
            }*/

            //  http://stackoverflow.com/questions/2488426/how-to-play-a-mp3-file-using-naudio
            //Playing test.mp3 in app\Ctms.Presentation\bin\Debug
            using (var ms = File.OpenRead("test.pcm"))
            using (var rdr = new Mp3FileReader(ms))
            using (var wavStream = WaveFormatConversionStream.CreatePcmStream(rdr))
            using (var baStream = new BlockAlignReductionStream(wavStream))
            using (var waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
            {
                waveOut.Init(baStream);
                waveOut.Play();
                while (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}