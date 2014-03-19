using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Waf.Foundation;

namespace Ctms.Domain.Objects
{
    // A Tag is the digital representation of a "Tangible"
    public class Tag : Model
    {
        private Keyword _assignedKeyword;
        private readonly ObservableCollection<TagOption> _tagOptions;
        private readonly ObservableCollection<TagOption> _previousOptions;
        private double _angle;

        public Tag()
        {
            _tagOptions         = new ObservableCollection<TagOption>();
            _previousOptions    = new ObservableCollection<TagOption>();

            //_angle = 90.0;
        }

        public int Id { get; set; }

        // A tag provides multiple TagOptions which lead to a final selection of a keyword
        public ObservableCollection<TagOption> TagOptions
        {
            get { return _tagOptions; }
        }
        
        public ObservableCollection<TagOption> PreviousOptions
        {
            get { return _previousOptions; }
        }
        
        public int CurrentLayerNr { get; set; }

        // What is the current angle in relation to the default orientation
        public double Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    RaisePropertyChanged("Angle");
                }
            }
        }

        // Which keyword is assigned to this tag
        public Keyword AssignedKeyword
        {
            get { return _assignedKeyword; }
            set
            {
                if (_assignedKeyword != value)
                {
                    _assignedKeyword = value;
                    RaisePropertyChanged("AssignedKeyword");
                }
            }
        }
    }
}
