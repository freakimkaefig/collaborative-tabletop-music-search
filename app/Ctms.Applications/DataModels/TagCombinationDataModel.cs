using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Collections.ObjectModel;
using Ctms.Domain;

namespace Ctms.Applications.DataModels
{
    /// <summary>
    /// This DataModel serves for extending object with visual-specific variables
    /// </summary>
    public class TagCombinationDataModel : DataModel
    {
        private ObservableCollection<TagDataModel> _tags;

        private double _centerX;
        private double _centerY;
        private int _id;
        private KeywordTypes _combinationType;

        public TagCombinationDataModel(int id)
        {
            _tags = new ObservableCollection<TagDataModel>();
            _id = id;
        }

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

        public double CenterX
        {
            get { return _centerX; }
            set
            {
                if (_centerX != value)
                {
                    _centerX = value;
                    RaisePropertyChanged("CenterX");
                }
            }
        }

        public double CenterY
        {
            get { return _centerY; }
            set
            {
                if (_centerY != value)
                {
                    _centerY = value;
                    RaisePropertyChanged("CenterY");
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

        public KeywordTypes CombinationType
        {
            get { return _combinationType; }
            set
            {
                if (_combinationType != value)
                {
                    _combinationType = value;
                    RaisePropertyChanged("CombinationType");
                }
            }
        }
    }
}
