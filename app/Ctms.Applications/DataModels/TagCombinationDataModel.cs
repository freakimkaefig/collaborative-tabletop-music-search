using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Applications;
using System.Collections.ObjectModel;
using Ctms.Domain;
using Ctms.Domain.Objects;

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
        private CombinationTypes _combinationType;

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
            {   // set only if difference bigger than 5 pixels
                if (Math.Abs(_centerX - value) > 15)
                {
                    _centerX = value;
                    //Console.WriteLine("set CenterX: " + _centerX + ", id: " + _id + ", activeOptionsCount tags: " + _tags.Count);
                    RaisePropertyChanged("CenterX");
                }
            }
        }

        public double CenterY
        {
            get { return _centerY; }
            set
            {   
                // set only if difference bigger than 5 pixels
                if (Math.Abs(_centerY - value) > 15)
                {
                    _centerY = value;
                    //Console.WriteLine("set CenterY: " + _centerY + ", id: " + _id + ", activeOptionsCount tags: " + _tags.Count);
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

        public CombinationTypes CombinationType
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
