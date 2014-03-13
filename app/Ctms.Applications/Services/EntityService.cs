using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using Ctms.Domain.Objects;
using Ctms.Applications.DataModels;

namespace Ctms.Applications.Services
{
    //Provides entities (data from database and files). Just getters, no setters, but add to/remove from list is possible.
    [Export]
    public class EntityService
    {
        /*
        private ObservableCollection<Style> _styles;
        private ObservableCollection<Tag> _tags;
        private ObservableCollection<TagDataModel> _tagDataModels;
        private ObservableCollection<TagOption> _tagOptions;

        public ObservableCollection<Style> Styles
        {
            get
            {
                if (_styles == null)
                {   // read and set styles
                    var styles = XmlProvider.ReadStyles();

                    _styles = new ObservableCollection<Style>();                    
                    foreach (var style in styles)
                    {
                        _styles.Add(style);
                    }
                }
                return _styles;
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get
            {
                if (_tags == null)
                {   // read and set styles
                    //_tags = 
                }
                _tags = new ObservableCollection<Tag>();//!!ToDo: Fill
                return _tags;
            }
        }

        public ObservableCollection<TagDataModel> TagDataModels
        {
            get
            {
                if (_tagDataModels == null)
                {   // init list
                    _tagDataModels = new ObservableCollection<TagDataModel>();
                }
                return _tagDataModels;
            }
        }

        public ObservableCollection<TagOption> TagOptions
        {
            get
            {
                if (_tagOptions == null)
                {   // init list
                    _tagOptions = new ObservableCollection<TagOption>();
                }
                return _tagOptions;
            }
        }
        */
    }
}
