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

        public Keyword(int id, string name, KeywordTypes type)
        {
            Id      = id;
            Name    = name;
            Type    = type;
        }

        public int Id { get; set; }

        public string SearchId { get; set; }

        public KeywordTypes Type { get; protected set; }
        
        
        //public string Name { get; set; }
        public object Description { get; set; }

        public double Weight { get; set; }

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
