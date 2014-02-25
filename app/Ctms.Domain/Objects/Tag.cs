using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.SearchObjects;

namespace Ctms.Domain.Objects
{
    public class Tag
    {
        public Tag()
        {
            SearchOptions = new List<SearchOption>();
        }

        public int Id { get; set; }
        public List<SearchOption> SearchOptions { get; set; }
    }
}
