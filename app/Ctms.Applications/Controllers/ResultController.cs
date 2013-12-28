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
        private readonly IEntityService entityService;
        //ViewModels
        private ResultViewModel resultViewModel;
        //Commands
        private readonly DelegateCommand selectCommand;
        //Further vars
        //private SynchronizingCollection<BookDataModel, Book> bookDataModels;

        [ImportingConstructor]
        public ResultController(CompositionContainer container, IShellService shellService, IEntityService entityService,
            ResultViewModel resultViewModel)
        {
            this.container = container;
            //Services
            this.shellService = shellService;
            this.entityService = entityService;
            //ViewModels
            this.resultViewModel = resultViewModel;
            //Commands
            //this.chooseCommand      = new DelegateCommand(chooseResult, CanSelectResult);
        }

        public void Initialize()
        {
            AddWeakEventListener(resultViewModel, ResultViewModelPropertyChanged);

            resultViewModel.SelectCommand = selectCommand;
            AddWeakEventListener(resultViewModel, ResultViewModelPropertyChanged);

            shellService.ResultView = resultViewModel.View;
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
        }
    }
}