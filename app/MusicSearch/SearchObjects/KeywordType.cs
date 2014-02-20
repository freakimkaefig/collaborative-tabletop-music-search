using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    public class KeywordType : SearchOption
    {
        public enum Types
        {
            Style,
            Title,
            Artist,
            AcousticAttribute
        };
        //public Types Type { get; set; }

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
