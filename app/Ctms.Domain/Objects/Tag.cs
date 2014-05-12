using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Waf.Foundation;
using Helpers;
using System.Diagnostics;

namespace Ctms.Domain.Objects
{
    // A Tag is the digital representation of a "Tangible"
    public class Tag : Model
    {
        private Keyword _assignedKeyword;
        private readonly ObservableCollection<TagOption> _tagOptions;
        private readonly ObservableCollection<TagOption> _breadcrumbOptions;
        private short _angle;
        private short _positionY;
        private short _positionX;
        private short orientation;

        public Tag()
        {
            _tagOptions         = new ObservableCollection<TagOption>();
            _breadcrumbOptions  = new ObservableCollection<TagOption>();
        }

        public int Id { get; set; }

        // A tag provides multiple TagOptions which lead to a final selection of a keyword
        public ObservableCollection<TagOption> TagOptions
        {
            get { return _tagOptions; }
        }
        
        public ObservableCollection<TagOption> BreadcrumbOptions
        {
            get { return _breadcrumbOptions; }
        }
        
        public int CurrentLayerNr { get; set; }

        // What is the current angle in relation to the default orientation
        public short Angle
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

        public short Orientation
        {
            get { return orientation; }
            set
            {
                if (orientation != value)
                {
                    orientation = value;
                    RaisePropertyChanged("Orientation");
                }
            }
        }

        public short PositionY
        {
            get { return _positionY; }
            set
            {   // optimize performance by limiting update rate
                if (Math.Abs(_positionY - value) > 0)
                {
                    _positionY = value;
                    RaisePropertyChanged("PositionY");
                }
            }
        }

        public short PositionX
        {
            get { return _positionX; }
            set
            {   // optimize performance by limiting update rate
                if (Math.Abs(_positionX - value) > 0)
                {
                    _positionX = value;
                    RaisePropertyChanged("PositionX");
                    Console.WriteLine("Tag" + Id + " PositionX: " + PositionX);
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
                    Console.WriteLine("Tag" + Id + " PositionY: " + PositionY);
                }
            }
        }
    }
}
