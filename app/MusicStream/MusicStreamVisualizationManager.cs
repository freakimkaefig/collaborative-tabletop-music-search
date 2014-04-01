using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;
using System.IO;

namespace MusicStream
{
    class MusicStreamVisualizationManager
    {
        private MusicStreamSessionManager _sessionManager;
        private int _counter = 0;

        /// <summary>
        /// Constructor for MusicStreamVisualizationManager
        /// Handles fast fourier transformation for visualizing of streamed music
        /// </summary>
        public MusicStreamVisualizationManager(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
        }

        /// <summary>
        /// Has to be called when receiving streaming data from Spotify
        /// </summary>
        /// <param name="format"></param>
        /// <param name="frames"></param>
        /// <param param name="num_frames"></param>
        public void MusicDeliveryCallback(AudioFormat format, byte[] frames, int num_frames)
        {
            int channels = format.channels;                 //Channels = 2
            int sampleRate = format.sample_rate;            //SampleRate = 44100
            SampleType sampleType = format.sample_type;     //SampleType = Int16NativeEndian

            AudioBufferStats stats = _sessionManager.GetCurrentAudioBufferStats();
            int samples = stats.samples;
            int stutter = stats.stutter;
        }
    }
}