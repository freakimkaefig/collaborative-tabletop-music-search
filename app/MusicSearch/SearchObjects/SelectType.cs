using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MusicSearch.SearchObjects
{
    public class SelectType
    {
        public KeywordTypes Type { get; set; }

        public string Name 
        { 
            get 
            {
                return Type.ToString();
            }
        }
    }
}
