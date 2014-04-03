using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Waf.Foundation;
using System.Windows;

namespace Ctms.Domain.Objects
{
    public enum InfoTypes
    {
        CommonInfo,
        TagInfo,
        TutorialInfo
    }

    public class Info : Model
    {
        protected Info(string mainText, InfoTypes infoType)
        {

        }

        public int Id { get; set; }

        public string MainText { get; set; }

        public string SubText { get; set; }

        public Point Position { get; set; }

        public InfoStates State { get; set; }

        public enum InfoStates
        {

        }

        public InfoTypes InfoType { get; set; }
    }

    public class CommonInfo : Info
    {
        public CommonInfo(string mainText) : base (mainText, InfoTypes.CommonInfo)
        {

        }
    }

    public class TagInfo : Info
    {
        public TagInfo(string mainText) : base(mainText, InfoTypes.TagInfo)
        {

        }
    }

    public class TutorialInfo : Info
    {
        public TutorialInfo(string mainText) : base(mainText, InfoTypes.TutorialInfo)
        {

        }
    }
}
