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

namespace Ctms.Applications.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class SearchTagViewModel : ViewModel<ISearchTagView>
    {
        private bool _isValid = true;
        private Detail _detail;
        private ICommand _selectOptionCmd;
        private string _item1Header;
        private int _id;
        private string _keyword;
        private ISearchTagView _searchTagView;
        
        [ImportingConstructor]
        public SearchTagViewModel(ISearchTagView view)
            : base(view)
        {
            _searchTagView = view;

            _tags = new ObservableCollection<TagDataModel>();
        }

        private ObservableCollection<TagDataModel> _tags;

        public ObservableCollection<TagDataModel> Tags
        {
            get { return _tags; }
            set
            {
                if (_tags != value)
                {
                    _tags = value;
                    RaisePropertyChanged("Tags");
                }
            }
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

        
        public ISearchTagView MyView { get; set; }

    }
}
