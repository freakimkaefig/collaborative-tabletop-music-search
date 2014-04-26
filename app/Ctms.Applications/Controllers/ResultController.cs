﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.Waf.Applications;
using System.Waf.Applications.Services;
using Ctms.Applications.Properties;
using Ctms.Applications.Services;
using Ctms.Applications.ViewModels;
using Ctms.Domain;
using System.IO;
using System.Data.EntityClient;
using System.Data.Common;
using System.ComponentModel.Composition.Hosting;
using Ctms.Applications.Views;
using Ctms.Applications.Workers;
using Ctms.Applications.DataModels;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the result management.
    /// </summary>
    [Export]
    internal class ResultController : Controller
    {
        private readonly CompositionContainer container;
        //Services
        private readonly IShellService shellService;
        private readonly EntityService entityService;
        //ViewModels
        private ResultViewModel _resultViewModel;
        //Workers
        private StreamingWorker _streamingWorker;
        private PlaylistWorker _playlistWorker;
        //Commands
        //private readonly DelegateCommand selectOptionCommand;
        private readonly DelegateCommand _prelistenCommand;
        private readonly DelegateCommand _addTrackCommand;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public ResultController(CompositionContainer container, IShellService shellService, EntityService entityService,
            ResultViewModel resultViewModel, StreamingWorker streamingWorker, PlaylistWorker playlistWorker)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            this._resultViewModel = resultViewModel;
            //Workers
            this._streamingWorker = streamingWorker;
            this._playlistWorker = playlistWorker;
            //Commands
            this._prelistenCommand = new DelegateCommand((result) => _streamingWorker.Prelisten((ResultDataModel)result));
            this._addTrackCommand = new DelegateCommand((data) => _playlistWorker.AddTrackToPlaylist((object[])data));
            //this.selectOptionCommand = new DelegateCommand(chooseResult, CanSelectResult);
        }

        public void Initialize()
        {
            //Commands
            //_resultViewModel.SelectCommand = selectCommand;
            _resultViewModel.PrelistenCommand = _prelistenCommand;
            _resultViewModel.AddTrackCommand = _addTrackCommand;
            //Views
            shellService.ResultView = _resultViewModel.View;
            //Listeners
            AddWeakEventListener(_resultViewModel, ResultViewModelPropertyChanged);
        }

        private void UpdateCommands()
        {

        }

        private void ResultViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Result")
            {
                UpdateCommands();
            }

            if (e.PropertyName == "Results")
            {
                UpdateCommands();
            }
        }
    }
}
