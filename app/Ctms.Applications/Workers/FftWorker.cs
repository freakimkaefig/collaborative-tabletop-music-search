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
        private MusicStreamVisualizationManager _msvm;
        private SearchViewModel _searchVm;
        public Action<MusicStreamVisualizationManager> VisualizationManagerCreated;

        [ImportingConstructor]
        public FftWorker(SearchViewModel searchVm)
        {
            _msvm = new MusicStreamVisualizationManager();
            _searchVm = searchVm;
            VisualizationManagerCreated(_msvm);
            _msvm.FftDataReceived = UpdateVm;
        }

        private void UpdateVm(double[] arr)
        {
            _searchVm.Fft1Value = (int)arr[2];
            _searchVm.Fft2Value = (int)arr[3];
            _searchVm.Fft3Value = (int)arr[4];
            _searchVm.Fft4Value = (int)arr[5];
            _searchVm.Fft5Value = (int)arr[7];
            _searchVm.Fft6Value = (int)arr[10];
            _searchVm.Fft7Value = (int)arr[15];
            _searchVm.Fft8Value = (int)arr[30];
            _searchVm.Fft9Value = (int)arr[150];
            _searchVm.Fft10Value = (int)arr[1000];
        }
    }
}
