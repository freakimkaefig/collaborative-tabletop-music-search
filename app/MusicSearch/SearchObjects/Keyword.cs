using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    public class Keyword
    {
        public int KeywordId            { get; set; }   
        public KeywordType.Types Type   { get; protected set; }        
        public string Name              { get; set; }
        public double Weight            { get; set; }

        public Keyword(string name)
        {
            Name = name;
        }
    }

    public class Style : Keyword
    {
        public List<Style> SubStyles { get; set; }

        public Style(string name) : base(name)
        {
            Type = KeywordType.Types.Style;
        }
    }

    public class Title : Keyword
    {
        public string SpotifyId     { get; set; }
        public List<Artist> Artists { get; set; }

        public Title(string name) : base(name)
        {
            Type = KeywordType.Types.Title;
        }
    }

    public class Artist : Keyword
    {
        public List<Title> Titles { get; set; }

        public Artist(string name) : base(name)
        {
            Type = KeywordType.Types.Artist;
        }
    }
}
