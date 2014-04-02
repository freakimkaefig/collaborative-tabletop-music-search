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
            _searchViewModel.Fft1Value = (int)arr[2];
            _searchViewModel.Fft2Value = (int)arr[3];
            _searchViewModel.Fft3Value = (int)arr[4];
            _searchViewModel.Fft4Value = (int)arr[5];
            _searchViewModel.Fft5Value = (int)arr[7];
            _searchViewModel.Fft6Value = (int)arr[10];
            _searchViewModel.Fft7Value = (int)arr[15];
            _searchViewModel.Fft8Value = (int)arr[30];
            _searchViewModel.Fft9Value = (int)arr[150];
            _searchViewModel.Fft10Value = (int)arr[1000];
        }
    }
}
