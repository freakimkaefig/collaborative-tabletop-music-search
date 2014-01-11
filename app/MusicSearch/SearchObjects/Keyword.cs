using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    public class Keyword : SelectOption
    {
        public double Weighting { get; set; }
        public KeywordType.Types Type { get; set; }        
        public String Name { get; set; }

        public class Style : Keyword
        {
            public Style(String name)
            {
                Name = name;
                Type = KeywordType.Types.Style;
            }
        }

        public class Title : Keyword
        {
            public Title(String name)
            {
                Name = name;
                Type = KeywordType.Types.Title;
            }
        }

        public class Artist : Keyword
        {
            public Artist(String name)
            {
                Name = name;
                Type = KeywordType.Types.Artist;
            }
        }

        public class AcousticAttribute : Keyword
        {
            public AcousticAttribute(String name)
            {
                Name = name;
                Type = KeywordType.Types.AcousticAttribute;
            }
        }
    }

    
}
