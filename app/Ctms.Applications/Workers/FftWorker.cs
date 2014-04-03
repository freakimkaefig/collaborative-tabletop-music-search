using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
using Ctms.Applications.ViewModels;
using System.Windows.Media.Animation;
using System.Windows;

namespace Ctms.Applications.Workers
{
    [Export]
    public class FftWorker
    {
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        private SearchViewModel _searchViewModel;
        Storyboard sb = new Storyboard();
        //public Action<MusicStreamVisualizationManager> VisualizationManagerCreated;

        [ImportingConstructor]
        public FftWorker(SearchViewModel searchVm, MusicStreamAccountWorker accountWorker)
        {
            _searchViewModel = searchVm;
            _accountWorker = accountWorker;
            
            //VisualizationManagerCreated(_visualizationManager);
            _accountWorker.FftSessionManagerCreated = FftSessionManagerCreated;
        }

        private void FftSessionManagerCreated(MusicStreamSessionManager sessionManager)
        {
            _sessionManager = sessionManager;
            _visualizationManager = new MusicStreamVisualizationManager(sessionManager);
            _sessionManager.VisualizationManager = _visualizationManager;
            _visualizationManager.FftDataReceived = UpdateVm;
        }

        private void UpdateVm(double[] arr)
        {

            if (arr.Length >= 6)
            {
                int x = _searchViewModel.Fft1Value;

                _searchViewModel.Fft1Value = (int)Math.Abs(arr[6] * 200);

                DoubleAnimation interpolate1 = new DoubleAnimation() { From = x, To = _searchViewModel.Fft1Value, Duration = TimeSpan.FromMilliseconds(4) };

                Storyboard.SetTarget(interpolate1, _searchViewModel.FftRectangle[0]);

                Storyboard.SetTargetProperty(interpolate1, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

                sb.Children.Add(interpolate1);
            }
            else
            {
                _searchViewModel.Fft1Value = 0;
            }
            /*if (arr.Length >= 9)
            {
                _searchViewModel.Fft2Value = (int)Math.Abs(arr[9] * 200);
            }
            else
            {
                _searchViewModel.Fft2Value = 0;
            }
            if (arr.Length >= 16)
            {
                _searchViewModel.Fft3Value = (int)Math.Abs(arr[16] * 200);
            }
            else
            {
                _searchViewModel.Fft3Value = 0;
            }
            if (arr.Length >= 23)
            {
                _searchViewModel.Fft4Value = (int)Math.Abs(arr[23] * 200);
            }
            else
            {
                _searchViewModel.Fft4Value = 0;
            }
            if (arr.Length >= 34)
            {
                _searchViewModel.Fft5Value = (int)Math.Abs(arr[34] * 200);
            }
            else
            {
                _searchViewModel.Fft5Value = 0;
            }
            if (arr.Length >= 70)
            {
                _searchViewModel.Fft6Value = (int)Math.Abs(arr[70] * 200);
            }
            else
            {
                _searchViewModel.Fft6Value = 0;
            }
            if (arr.Length >= 116)
            {
                _searchViewModel.Fft7Value = (int)Math.Abs(arr[116] * 200);
            }
            else
            {
                _searchViewModel.Fft7Value = 0;
            }
            if (arr.Length >= 163)
            {
                _searchViewModel.Fft8Value = (int)Math.Abs(arr[163] * 200);
            }
            else
            {
                _searchViewModel.Fft8Value = 0;
            }
            if (arr.Length >= 232)
            {
                _searchViewModel.Fft9Value = (int)Math.Abs(arr[232] * 200);
            }
            else
            {
                _searchViewModel.Fft9Value = 0;
            }
            if (arr.Length >= 372)
            {
                _searchViewModel.Fft10Value = Math.Abs((int)arr[372] * 200);
            }
            else
            {
                _searchViewModel.Fft10Value = 0;
            }*/



            sb.Begin();

        }
    }
}
