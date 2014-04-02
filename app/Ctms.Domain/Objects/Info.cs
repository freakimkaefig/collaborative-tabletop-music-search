using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using System.Windows;

namespace Ctms.Domain.Objects
{
    public class Info : Model
    {
        public Info()
        {

        }

        public enum InfoState
        {
        }

        public int Id { get; set; }

        public string MainText { get; set; }

        public string SubText { get; set; }

        public Point Position { get; set; }

        public InfoState State { get; set; }
    }
}
