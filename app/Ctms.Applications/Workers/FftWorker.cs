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
            _searchViewModel.Fft1Value = (int)Math.Abs(arr[2] * 100);
            _searchViewModel.Fft2Value = (int)Math.Abs(arr[3] * 100);
            _searchViewModel.Fft3Value = (int)Math.Abs(arr[4] * 100);
            _searchViewModel.Fft4Value = (int)Math.Abs(arr[5] * 100);
            _searchViewModel.Fft5Value = (int)Math.Abs(arr[7] * 100);
            _searchViewModel.Fft6Value = (int)Math.Abs(arr[10] * 100);
            _searchViewModel.Fft7Value = (int)Math.Abs(arr[15] * 100);
            _searchViewModel.Fft8Value = (int)Math.Abs(arr[30] * 100);
            _searchViewModel.Fft9Value = (int)Math.Abs(arr[150] * 100);
            _searchViewModel.Fft10Value = (int)Math.Abs(arr[1000] * 100);
        }
    }
}
