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

        public Tag()
        {
            //TagOptions = new ObservableCollection<TagOption>();
            _tagOptions = new ObservableCollection<TagOption>();
        }

        public int Id { get; set; }

        // A tag provides multiple TagOptions which lead to a final selection of a keyword
        //public ObservableCollection<TagOption> TagOptions { get; set; }

        public ObservableCollection<TagOption> TagOptions
        {
            get { return _tagOptions; }
            /*
            set
            {
                if (_tagOptions != value)
                {
                    _tagOptions = value;
                    RaisePropertyChanged("TagOptions");
                }
            }*/
        }

        public int CurrentLayerNr { get; set; }

        // What is the current angle in relation to the default orientation
        public double Angle { get; set; }

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
