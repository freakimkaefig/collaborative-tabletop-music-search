﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Domain.Objects;
using System.Windows.Input;
using Ctms.Applications.Views;
using System.Waf.Applications;
using System.Collections.ObjectModel;
using Ctms.Applications.DataModels;
using System.Windows;

namespace Ctms.Applications.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SearchTagViewModel : ViewModel<ISearchTagView>
    {
        private bool _isValid = true;
        private int _id;
        private ISearchTagView _searchTagView;
        private Point _position;
        
        [ImportingConstructor]
        public SearchTagViewModel(ISearchTagView view)
            : base(view)
        {
            _searchTagView = view;
        }

        public bool IsEnabled { get { return true; } }//Detail != null;//!! Has to be adjusted

        public bool IsValid
        {
            get { return _isValid; }
            set
            {
                if (_isValid != value)
                {
                    _isValid = value;
                    RaisePropertyChanged("IsValid");
                }
            }
        }

        public int Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        public Point Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    RaisePropertyChanged("Position");
                }
            }
        }

        public ISearchTagView MyView { get; set; }


        public string SearchTagViewLog
        {
            get { return _searchTagViewLog; }
            set
            {
                if (_searchTagViewLog != value)
                {
                    _searchTagViewLog = value;
                    RaisePropertyChanged("SearchTagViewLog");
                }
            }
        }

        private int _logCount;
        private string _searchTagViewLog;

        public void AddLog(string logMessage)
        {
            _logCount++;
            SearchTagViewLog += _logCount + logMessage + Environment.NewLine;
        }
    }
}
