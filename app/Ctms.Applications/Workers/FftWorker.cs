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
using Ctms.Applications.Views;

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
        private int _lowbandMultiplier = 300;
        private int _midbandMultiplier = 1400;
        private int _highbandMultiplier = 1700;
        private int _maxheight = 150;
        private int _duration = 450;
        private bool _every2ndTime = true;


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
            //decreasing animation-flickering by only using every 2nd received byte-array 
            //for visualization
            if (_every2ndTime)
            {
                _every2ndTime = false;

                _array = arr;
                Application.Current.Dispatcher.Invoke(new UpdateDelegate(UpdateVm));
            }
            else
            {
                _every2ndTime = true;
            }
            
        }


        #region setEQ-Bar-Values

        public void UpdateVm()
        {
            var searchView = ((ISearchView)_searchViewModel.View);

            if (_array.Length > 6)
            {
                //split real & imaginary data into 2 arrays
                double[] b = new double[7];
                Array.Copy(_array, 0, b, 0, 7);
                double[] c = new double[7];
                Array.Copy(_array, _array.Length - 7, c, 0, 7);

                //calculate and use the median of this EQ-bars-range to enhance visualized data
                //instead of picking single frequencies
                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _lowbandMultiplier);

                //check if calculated value is higher than defined max value for the height
                //of the rectangles
                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft1Value = _maxheight;
                }
                //check if the calculated value is higher than the actualheight-value during the 
                //running animation to decrease flickering of the visualization
                else if (calcMagnitude > (int)searchView.GetFft1.ActualHeight)
                {
                    _searchViewModel.Fft1Value = calcMagnitude;
                }

                //setting the exact duration of the music-snippet received by 
                //the current byte-array for the animation causes flickering.
                //used value for the duration is based on (try&error) gained experience
                    //int _duration = (int)_array[_array.Length - 1];

                //set up the animation, which will collapse the EQ-bar smoothly and
                //add it to the storyboard.
                    DoubleAnimation interpolate1 = new DoubleAnimation() { From = _searchViewModel.Fft1Value, To = 0, Duration = TimeSpan.FromMilliseconds(_duration) };

                    Storyboard.SetTarget(interpolate1, _searchViewModel.FftRectangle[0]);

                //Set the Height-Property of the rectangle-EQ-bars as target property
                    Storyboard.SetTargetProperty(interpolate1, new PropertyPath(System.Windows.Shapes.Rectangle.HeightProperty));

                //start animation
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
                double[] b = new double[3];
                Array.Copy(_array, 7, b, 0, 3);
                double[] c = new double[3];
                Array.Copy(_array, _array.Length - 9, c, 0, 3);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _lowbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft2Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft2.ActualHeight)
                {
                    _searchViewModel.Fft2Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[8];
                Array.Copy(_array, 10, b, 0, 8);
                double[] c = new double[8];
                Array.Copy(_array, _array.Length - 16, c, 0, 8);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _lowbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft3Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft3.ActualHeight)
                {
                    _searchViewModel.Fft3Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[15];
                Array.Copy(_array, 17, b, 0, 15);
                double[] c = new double[15];
                Array.Copy(_array, _array.Length - 23, c, 0, 15);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _lowbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft4Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft4.ActualHeight)
                {
                    _searchViewModel.Fft4Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[12];
                Array.Copy(_array, 24, b, 0, 12);
                double[] c = new double[12];
                Array.Copy(_array, _array.Length - 34, c, 0, 12);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft5Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft5.ActualHeight)
                {
                    _searchViewModel.Fft5Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[13];
                Array.Copy(_array, 35, b, 0, 13);
                double[] c = new double[13];
                Array.Copy(_array, _array.Length - 46, c, 0, 13);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft6Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft6.ActualHeight)
                {
                    _searchViewModel.Fft6Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[13];
                Array.Copy(_array, 47, b, 0, 13);
                double[] c = new double[13];
                Array.Copy(_array, _array.Length - 58, c, 0, 13);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft7Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft7.ActualHeight)
                {
                    _searchViewModel.Fft7Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[13];
                Array.Copy(_array, 59, b, 0, 13);
                double[] c = new double[13];
                Array.Copy(_array, _array.Length - 70, c, 0, 13);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft8Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft8.ActualHeight)
                {
                    _searchViewModel.Fft8Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[12];
                Array.Copy(_array, 71, b, 0, 12);
                double[] c = new double[12];
                Array.Copy(_array, _array.Length - 81, c, 0, 12);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft9Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft9.ActualHeight)
                {
                    _searchViewModel.Fft9Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[14];
                Array.Copy(_array, 81, b, 0, 14);
                double[] c = new double[14];
                Array.Copy(_array, _array.Length - 93, c, 0, 14);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft10Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft10.ActualHeight)
                {
                    _searchViewModel.Fft10Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[12];
                Array.Copy(_array, 94, b, 0, 12);
                double[] c = new double[12];
                Array.Copy(_array, _array.Length - 104, c, 0, 12);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _midbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft11Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft11.ActualHeight)
                {
                    _searchViewModel.Fft11Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[13];
                Array.Copy(_array, 105, b, 0, 13);
                double[] c = new double[13];
                Array.Copy(_array, _array.Length - 116, c, 0, 13);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _highbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft12Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft12.ActualHeight)
                {
                    _searchViewModel.Fft12Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[13];
                Array.Copy(_array, 117, b, 0, 13);
                double[] c = new double[13];
                Array.Copy(_array, _array.Length - 128, c, 0, 13);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _highbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft13Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft13.ActualHeight)
                {
                    _searchViewModel.Fft13Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[12];
                Array.Copy(_array, 129, b, 0, 12);
                double[] c = new double[12];
                Array.Copy(_array, _array.Length - 139, c, 0, 12);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _highbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft14Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft14.ActualHeight)
                {
                    _searchViewModel.Fft14Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];

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
                double[] b = new double[12];
                Array.Copy(_array, 140, b, 0, 12);
                double[] c = new double[12];
                Array.Copy(_array, _array.Length - 151, c, 0, 12);

                int calcMagnitude = (int)Math.Abs(Math.Sqrt(calcMedian(b) * calcMedian(b) + calcMedian(c) * calcMedian(c)) * _highbandMultiplier);

                if (calcMagnitude > _maxheight)
                {
                    _searchViewModel.Fft15Value = _maxheight;
                }
                else if (calcMagnitude > (int)searchView.GetFft15.ActualHeight)
                {
                    _searchViewModel.Fft15Value = calcMagnitude;
                }

                //int _duration = (int)_array[_array.Length - 1];
                
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
        #endregion

        public double calcMedian(double[] arr)
        {

            //Framework 2.0 version of this method. there is an easier way in F4        
            if (arr == null || arr.Length == 0)
                return 0D;

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])arr.Clone();
            arr.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;

        }
    }
}
