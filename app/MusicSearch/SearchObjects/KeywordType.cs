using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    public class KeywordType
    {
        public enum Types
        {
            Style,
            Title,
            Artist
        };

        public Types Type { get; set; }

        public string Name 
        { 
            get 
            {
                return Type.ToString();
            }
        }
    }
}
