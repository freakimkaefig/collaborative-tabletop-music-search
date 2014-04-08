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
        private int _multiplier = 400;
        private int _maxheight = 400;
        private int _duration = 250;


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
            if (_array.Length > 6)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[6] * _array[6] + _array[_array.Length - 6] * _array[_array.Length - 6] * _multiplier));

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft1Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft1Value = calcMagnitude;
                }
                
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
            if (_array.Length > 9)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[9] * _array[9] + _array[_array.Length - 9] * _array[_array.Length - 9]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft2Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft2Value = calcMagnitude;
                }
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
            if (_array.Length > 16)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[16] * _array[16] + _array[_array.Length - 16] * _array[_array.Length - 16]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft3Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft3Value = calcMagnitude;
                }
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
            if (_array.Length > 23)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[23] * _array[23] + _array[_array.Length - 23] * _array[_array.Length - 23]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft4Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft4Value = calcMagnitude;
                }
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
            if (_array.Length > 34)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[34] * _array[34] + _array[_array.Length - 34] * _array[_array.Length - 34]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft5Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft5Value = calcMagnitude;
                }
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
            if (_array.Length > 46)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[46] * _array[46] + _array[_array.Length - 46] * _array[_array.Length - 46]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft6Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft6Value = calcMagnitude;
                }
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
            if (_array.Length > 58)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[58] * _array[58] + _array[_array.Length - 58] * _array[_array.Length - 58]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft7Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft7Value = calcMagnitude;
                }
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
            if (_array.Length > 70)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[70] * _array[70] + _array[_array.Length - 70] * _array[_array.Length - 70]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft8Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft8Value = calcMagnitude;
                }
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
            if (_array.Length > 81)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[81] * _array[81] + _array[_array.Length - 81] * _array[_array.Length - 81]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft9Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft9Value = calcMagnitude;
                }
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
            if (_array.Length > 93)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[93] * _array[93] + _array[_array.Length - 93] * _array[_array.Length - 93]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft10Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft10Value = calcMagnitude;
                }
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
            if (_array.Length > 104)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[104] * _array[104] + _array[_array.Length - 104] * _array[_array.Length - 104]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft11Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft11Value = calcMagnitude;
                }
                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate11 = new DoubleAnimation() { From = _searchViewModel.Fft11Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate11, _searchViewModel.FftRectangle[10]);

                Storyboard.SetTargetProperty(interpolate11, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));



                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate11);
                sb.Begin();

            }
            else
            {
                _searchViewModel.Fft11Value = 0;
            }
            if (_array.Length > 116)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[116] * _array[116] + _array[_array.Length - 116] * _array[_array.Length - 116]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft12Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft12Value = calcMagnitude;
                }
                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate12 = new DoubleAnimation() { From = _searchViewModel.Fft12Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate12, _searchViewModel.FftRectangle[11]);

                Storyboard.SetTargetProperty(interpolate12, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));



                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate12);
                sb.Begin();

            }
            else
            {
                _searchViewModel.Fft12Value = 0;
            }
            if (_array.Length > 128)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[128] * _array[128] + _array[_array.Length - 128] * _array[_array.Length - 128]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft13Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft13Value = calcMagnitude;
                }
                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate13 = new DoubleAnimation() { From = _searchViewModel.Fft13Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate13, _searchViewModel.FftRectangle[12]);

                Storyboard.SetTargetProperty(interpolate13, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));



                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate13);
                sb.Begin();

            }
            else
            {
                _searchViewModel.Fft13Value = 0;
            }
            if (_array.Length > 139)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[139] * _array[139] + _array[_array.Length - 139] * _array[_array.Length - 139]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft14Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft14Value = calcMagnitude;
                }
                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate14 = new DoubleAnimation() { From = _searchViewModel.Fft14Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate14, _searchViewModel.FftRectangle[13]);

                Storyboard.SetTargetProperty(interpolate14, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));



                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate14);
                sb.Begin();

            }
            else
            {
                _searchViewModel.Fft14Value = 0;
            }
            if (_array.Length > 151)
            {
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(_array[151] * _array[151] + _array[_array.Length - 151] * _array[_array.Length - 151]) * _multiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft15Value = _maxheight;
                }
                else
                {
                    _searchViewModel.Fft15Value = calcMagnitude;
                }
                //int ms = (int)arr[arr.Length - 1];

                DoubleAnimation interpolate15 = new DoubleAnimation() { From = _searchViewModel.Fft15Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                Storyboard.SetTarget(interpolate15, _searchViewModel.FftRectangle[14]);

                Storyboard.SetTargetProperty(interpolate15, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));



                Storyboard sb = new Storyboard();
                sb.Children.Add(interpolate15);
                sb.Begin();

            }
            else
            {
                _searchViewModel.Fft15Value = 0;
            }


        }
    }
}
