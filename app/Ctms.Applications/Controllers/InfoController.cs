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
using Ctms.Domain.Objects;
using Ctms.Applications.DataFactories;
using Ctms.Applications.Data;


namespace Ctms.Applications.Controllers
{
    //!!Note: The content of this class is just an example and has to be adjusted.

    /// <summary>
    /// Responsible for the detail management.
    /// </summary>
    [Export]
    internal class InfoController : Controller
    {
        //Services
        private readonly IShellService _shellService;
        private readonly EntityService _entityService;
        //ViewModels
        private InfoViewModel _infoViewModel;
        //Commands
        private readonly DelegateCommand doTestCommand;
        //Further vars
        private InfoFactory _infoFactory;
        private Repository _repository;

        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public InfoController(CompositionContainer container, IShellService shellService, EntityService entityService,
            Repository repository, InfoViewModel infoViewModel)
        {
            //Services
            _shellService       = shellService;
            _entityService      = entityService;
            //ViewModels
            _infoViewModel      = infoViewModel;
            //Commands
            //doTestCommand      = new DelegateCommand(SelectDetail, CanSelectDetail);
            //Further vars
            _repository = repository;
            _infoFactory = new InfoFactory(_repository);
        }

        public void Initialize()
        {
            AddWeakEventListener(_infoViewModel, DetailViewModelPropertyChanged);

            var info = _infoFactory.CreateInfoDataModel();
            info.IsVisible = true;
            info.Info.MainText = "Funzt!";
            _infoViewModel.Infos.Add(info);
        }

        private void UpdateCommands()
        {

        }

        private bool CanSelectDetail() { return _infoViewModel.IsValid; }

        private void SelectDetail()
        {
        }

        private void DetailViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedSong")//SelectedSong is just an example
            {
                //...
                UpdateCommands();
            }
        }
    }
}