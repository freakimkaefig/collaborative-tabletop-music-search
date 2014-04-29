using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Ctms.Domain.Objects
{
    public class Detail
    {
        public Detail()
        {

        }

        //About tab
        public String Name { get; set; }
        public ArtistImage Image { get; set; }
        public String City { get; set; }
        public String Biography { get; set; }
        public List<String> Genres { get; set; }

        //News
        public ObservableCollection<ArtistNews> News { get; set; }

        //Media
        public List<ArtistImage> Images { get; set; }
        public List<ArtistVideo> Videos { get; set; }

        //Reviews

        //Songs
    }
}
