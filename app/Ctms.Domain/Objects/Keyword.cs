using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using MusicSearch.Objects;

namespace Ctms.Domain
{
    // A keyword can be 
    public class Keyword : Model
    {
        private string _displayName;
        private string _displayNameFirst;
        private string _displayNameSecond;
        private object _displayDescription;
        private object _value;
        private int _referenceAngleTopFirst;
        private int _referenceAngleTopSecond;
        private int _referenceAngleBottomFirst;
        private int _referenceAngleBottomSecond;

        public Keyword(int id, string displayName, KeywordTypes type)
        {
            Id      = id;
            DisplayName    = displayName;
            KeywordType    = type;
        }

        public int Id { get; set; }

        public string Key { get; set; }

        public KeywordTypes KeywordType { get; protected set; }

        public AttributeTypes AttributeType { get; set; }
        
        //public string Name { get; set; }
        public object DisplayDescription
        {
            get { return _displayDescription; }
            set
            {
                if (_displayDescription != value)
                {
                    _displayDescription = value;
                    RaisePropertyChanged("DisplayDescription");
                }
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged("Value");
                }
            }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    RaisePropertyChanged("DisplayName");
                }
            }
        }

        public string DisplayNameFirst
        {
            get { return _displayNameFirst; }
            set
            {
                if (_displayNameFirst != value)
                {
                    _displayNameFirst = value;
                    RaisePropertyChanged("DisplayNameFirst");
                }
            }
        }

        public string DisplayNameSecond
        {
            get { return _displayNameSecond; }
            set
            {
                if (_displayNameSecond != value)
                {
                    _displayNameSecond = value;
                    RaisePropertyChanged("DisplayNameSecond");
                }
            }
        }

        public int ReferenceAngleTopFirst
        {
            get { return _referenceAngleTopFirst; }
            set
            {
                if (_referenceAngleTopFirst != value)
                {
                    _referenceAngleTopFirst = value;
                    RaisePropertyChanged("ReferenceAngleTopFirst");
                }
            }
        }

        public int ReferenceAngleTopSecond
        {
            get { return _referenceAngleTopSecond; }
            set
            {
                if (_referenceAngleTopSecond != value)
                {
                    _referenceAngleTopSecond = value;
                    RaisePropertyChanged("ReferenceAngleTopSecond");
                }
            }
        }

        public int ReferenceAngleBottomFirst
        {
            get { return _referenceAngleBottomFirst; }
            set
            {
                if (_referenceAngleBottomFirst != value)
                {
                    _referenceAngleBottomFirst = value;
                    RaisePropertyChanged("ReferenceAngleBottomFirst");
                }
            }
        }

        public int ReferenceAngleBottomSecond
        {
            get { return _referenceAngleBottomSecond; }
            set
            {
                if (_referenceAngleBottomSecond != value)
                {
                    _referenceAngleBottomSecond = value;
                    RaisePropertyChanged("ReferenceAngleBottomSecond");
                }
            }
        }
    }

    public class Style : Keyword
    {
        public List<Style> SubStyles { get; set; }

        public Style(int id, string name) : base(id, name, KeywordTypes.Genre)
        {
        }
    }

    public class Title : Keyword
    {
        public string SpotifyId     { get; set; }
        public List<Artist> Artists { get; set; }

        public Title(int id, string name) : base(id, name, KeywordTypes.Title)
        {
        }
    }

    public class Artist : Keyword
    {
        public List<Title> Titles { get; set; }

        public Artist(int id, string name) : base(id, name, KeywordTypes.Artist)
        {
        }
    }
}
