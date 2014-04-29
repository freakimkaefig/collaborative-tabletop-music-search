using System;

namespace Ctms.Domain.Objects
{
    public class ArtistImage
    {
        public String ImageUrl { get; set; }
        public Uri Image { get { return new Uri(ImageUrl, UriKind.Absolute); } }
    }
}
