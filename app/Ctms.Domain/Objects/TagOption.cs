using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;

namespace Ctms.Domain.Objects
{
    // TagOptions are provided by a Tag
    public class TagOption : Model
    {
        private Keyword _keyword;

        public TagOption(int id)
        {
            Id = id;
        }

        public int Id { get; set; }

        // Which keyword is assigned to this tag option
        public Keyword Keyword
        {
            get { return _keyword; }
            set
            {
                if (_keyword != value)
                {
                    _keyword = value;
                    RaisePropertyChanged("Keyword");
                }
            }
        }

        // Defines on which layer of options hierarchy this one is
        public int LayerNr { get; set; }
    }
}
