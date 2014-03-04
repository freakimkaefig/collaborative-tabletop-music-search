using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctms.Domain.Objects
{
    public class Result
    {
        public Song Song { get; set; }

        // Defines which tags take how much effect on this result
        public Dictionary<Tag, double> TagInfluences { get; set; }
    }
}
