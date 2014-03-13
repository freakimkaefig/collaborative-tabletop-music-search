using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    // A keyword can be 
    public class Keyword
    {
        public int Id               { get; set; }   
        public KeywordTypes Type    { get; protected set; }
        public string Name          { get; set; }
        public string Description   { get; set; }
        public double Weight        { get; set; }

        public Keyword(string name, KeywordTypes type)
        {
            Name = name;
            Type = type;
        }
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
