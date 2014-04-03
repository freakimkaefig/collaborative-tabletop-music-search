using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
using Ctms.Applications.ViewModels;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Threading;

namespace Ctms.Applications.Workers
{
    [Export]
    public class FftWorker
    {
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        private SearchViewModel _searchViewModel;
        private double[] _array;
        public delegate void UpdateDelegate();
        //private Dispatcher _dp;
        private int _duration = 250;
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
            _visualizationManager.FftDataReceived = Dispatch;
        }

        public void Dispatch(double[] arr)
        {
            _array = arr;
            Application.Current.Dispatcher.Invoke(new UpdateDelegate(UpdateVm));
        }

        public void UpdateVm()
        {
            if (_array.Length >= 6)
            {
                _searchViewModel.Fft1Value = (int)Math.Abs(_array[6] * 800);

                //int _duration = (int)_array[_array.Length - 1];

                DoubleAnimation interpolate1 = new DoubleAnimation() { From = _searchViewModel.Fft1Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate1, _searchViewModel.FftRectangle[0]);

                Storyboard.SetTargetProperty(interpolate1, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));


                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate1);

                sb.Begin();
            

            }
            else
            {
                _searchViewModel.Fft1Value = 0;
            }
            if (_array.Length >= 9)
            {
                _searchViewModel.Fft2Value = (int)Math.Abs(_array[9] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate2 = new DoubleAnimation() { From = _searchViewModel.Fft2Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate2, _searchViewModel.FftRectangle[1]);

                Storyboard.SetTargetProperty(interpolate2, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));


                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate2);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft2Value = 0;
            }
            if (_array.Length >= 16)
            {
                _searchViewModel.Fft3Value = (int)Math.Abs(_array[16] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate3 = new DoubleAnimation() { From = _searchViewModel.Fft3Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate3, _searchViewModel.FftRectangle[2]);

                Storyboard.SetTargetProperty(interpolate3, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

               
                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate3);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft3Value = 0;
            }
            if (_array.Length >= 23)
            {
                _searchViewModel.Fft4Value = (int)Math.Abs(_array[23] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate4 = new DoubleAnimation() { From = _searchViewModel.Fft4Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate4, _searchViewModel.FftRectangle[3]);

                Storyboard.SetTargetProperty(interpolate4, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

                

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate4);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft4Value = 0;
            }
            if (_array.Length >= 34)
            {
                _searchViewModel.Fft5Value = (int)Math.Abs(_array[34] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate5 = new DoubleAnimation() { From = _searchViewModel.Fft5Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate5, _searchViewModel.FftRectangle[4]);

                Storyboard.SetTargetProperty(interpolate5, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

                

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate5);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft5Value = 0;
            }
            if (_array.Length >= 70)
            {
                _searchViewModel.Fft6Value = (int)Math.Abs(_array[70] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate6 = new DoubleAnimation() { From = _searchViewModel.Fft6Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate6, _searchViewModel.FftRectangle[5]);

                Storyboard.SetTargetProperty(interpolate6, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

               

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate6);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft6Value = 0;
            }
            if (_array.Length >= 116)
            {
                _searchViewModel.Fft7Value = (int)Math.Abs(_array[116] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate7 = new DoubleAnimation() { From = _searchViewModel.Fft7Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate7, _searchViewModel.FftRectangle[6]);

                Storyboard.SetTargetProperty(interpolate7, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

                

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate7);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft7Value = 0;
            }
            if (_array.Length >= 163)
            {
                _searchViewModel.Fft8Value = (int)Math.Abs(_array[163] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate8 = new DoubleAnimation() { From = _searchViewModel.Fft8Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate8, _searchViewModel.FftRectangle[7]);

                Storyboard.SetTargetProperty(interpolate8, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

               

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate8);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft8Value = 0;
            }
            if (_array.Length >= 232)
            {
                _searchViewModel.Fft9Value = (int)Math.Abs(_array[232] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate9 = new DoubleAnimation() { From = _searchViewModel.Fft9Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate9, _searchViewModel.FftRectangle[8]);

                Storyboard.SetTargetProperty(interpolate9, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

             

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate9);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft9Value = 0;
            }
            if (_array.Length >= 372)
            {
                _searchViewModel.Fft10Value = (int)Math.Abs(_array[372] * 800);

                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate10 = new DoubleAnimation() { From = _searchViewModel.Fft10Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate10, _searchViewModel.FftRectangle[9]);

                Storyboard.SetTargetProperty(interpolate10, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

               

                        Storyboard sb = new Storyboard();
                        sb.Children.Add(interpolate10);
                        sb.Begin();

            }
            else
            {
                _searchViewModel.Fft10Value = 0;
            }


        }
    }
}
