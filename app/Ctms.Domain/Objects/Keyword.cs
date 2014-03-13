using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;

namespace Ctms.Domain
{
    // A keyword can be 
    public class Keyword : Model
    {
        private string _name;
        /*
        
        private string _description;
        private double _weight;
        */
        public Keyword(string name, KeywordTypes type)
        {
            Name = name;
            Type = type;
        }

        public int Id { get; set; }

        public KeywordTypes Type { get; protected set; }

        
        
        //public string Name { get; set; }
        public string Description { get; set; }

        public double Weight { get; set; }
        /*
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    RaisePropertyChanged("Weight");
                }
            }
        }*/
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }
        /*
        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged("Description");
                }
            }
        }
        */
    }

    public class Style : Keyword
    {
        public List<Style> SubStyles { get; set; }

        public Style(string name) : base(name, KeywordTypes.Genre)
        {
        }
    }

    public class Title : Keyword
    {
        public string SpotifyId     { get; set; }
        public List<Artist> Artists { get; set; }

        public Title(string name) : base(name, KeywordTypes.Title)
        {
        }
    }

    public class Artist : Keyword
    {
        public List<Title> Titles { get; set; }

        public Artist(string name) : base(name, KeywordTypes.Artist)
        {
        }
    }
}
