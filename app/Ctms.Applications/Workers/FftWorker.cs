using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using MusicStream;
using Ctms.Applications.ViewModels;

namespace Ctms.Applications.Workers
{
    [Export]
    public class FftWorker
    {
        private MusicStreamAccountWorker _accountWorker;
        private MusicStreamSessionManager _sessionManager;
        private MusicStreamVisualizationManager _visualizationManager;
        private SearchViewModel _searchViewModel;
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
            if (arr[6] != null)
            {
                _searchViewModel.Fft1Value = (int)Math.Abs(arr[6] * 100);
            }
            else
            {
                _searchViewModel.Fft1Value = 0;
            }
            if (arr[9] != null)
            {
                _searchViewModel.Fft2Value = (int)Math.Abs(arr[9] * 100);
            }
            else
            {
                _searchViewModel.Fft2Value = 0;
            }
            if (arr[16] != null)
            {
                _searchViewModel.Fft3Value = (int)Math.Abs(arr[16] * 100);
            }
            else
            {
                _searchViewModel.Fft3Value = 0;
            }
            if (arr[23] != null)
            {
                _searchViewModel.Fft4Value = (int)Math.Abs(arr[23] * 100);
            }
            else
            {
                _searchViewModel.Fft4Value = 0;
            }
            if (arr[34] != null)
            {
                _searchViewModel.Fft5Value = (int)Math.Abs(arr[34] * 100);
            }
            else
            {
                _searchViewModel.Fft5Value = 0;
            }
            if (arr[70] != null)
            {
                _searchViewModel.Fft6Value = (int)Math.Abs(arr[70] * 100);
            }
            else
            {
                _searchViewModel.Fft6Value = 0;
            }
            if (arr[116] != null)
            {
                _searchViewModel.Fft7Value = (int)Math.Abs(arr[116] * 100);
            }
            else
            {
                _searchViewModel.Fft7Value = 0;
            }
            if (arr[163] != null)
            {
                _searchViewModel.Fft8Value = (int)Math.Abs(arr[163] * 100);
            }
            else
            {
                _searchViewModel.Fft8Value = 0;
            }
            if (arr[232] != null)
            {
                _searchViewModel.Fft9Value = (int)Math.Abs(arr[232] * 100);
            }
            else
            {
                _searchViewModel.Fft9Value = 0;
            }
            if (arr[372] != null)
            {
                _searchViewModel.Fft10Value = Math.Abs((int)arr[372] * 100);
            }
            else
            {
                _searchViewModel.Fft10Value = 0;
            }

        }
    }
}
