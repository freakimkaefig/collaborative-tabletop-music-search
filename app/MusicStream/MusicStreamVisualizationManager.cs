using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpotifySharp;
using System.IO;
using System.Diagnostics;

namespace MusicStream
{
    public class MusicStreamVisualizationManager
    {
        private MusicStreamSessionManager _sessionManager;
        private int _counter = 0;
        private Lomont.LomontFFT _lomontFFT;
        public Action<double[]> FftDataReceived;

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
        public void MusicDeliveryCallback(AudioFormat format, byte[] frames, int num_frames, double howmuchsec)
        {
            int channels = format.channels;                 //Channels = 2
            int sampleRate = format.sample_rate;            //SampleRate = 44100
            SampleType sampleType = format.sample_type;     //SampleType = Int16NativeEndian

            AudioBufferStats stats = _sessionManager.GetCurrentAudioBufferStats();
            int samples = stats.samples; //Anzahl der abgespielten bzw. gebufferten Bytes
            int stutter = stats.stutter; //"Hänger"

            //Debug.WriteLine("DATA received:\nSamples: "+samples+"\nHänger: "+stutter+"\nFrames: "+frames);

            Double[] preparedFrames = new Double[frames.Length + 1];
            preparedFrames = prepareBytes(channels, sampleRate, frames);


            
            _lomontFFT = new Lomont.LomontFFT();
            Double[] framesFFT = _lomontFFT.FFT(preparedFrames, true);
            framesFFT[framesFFT.Length - 1] = howmuchsec;

            FftDataReceived(framesFFT);
        }

      

        

        public Double[] prepareBytes(int channels, int sampleRate, byte[] frames)
        {
            Double[] data = new Double[frames.Length / 4];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = BitConverter.ToInt16(frames, i * 4) / 65536.0;
            }

            return data;
        }
    }
}