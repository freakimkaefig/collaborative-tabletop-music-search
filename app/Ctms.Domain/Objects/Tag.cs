using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MusicSearch.SearchObjects;

namespace Ctms.Domain.Objects
{
    public class Tag
    {
        public int              Id                  { get; set; }
        public double           Angle               { get; set; }
        //public string State { get; set; }
        public List<TagOption>  TagOptions          { get; set; } // A tag provides multiple TagOptions
        public Keyword          SelectedKeyword     { get; set; }
        public List<Keyword>    ParentKeywords      { get; set; }

        public Tag()
        {
            TagOptions = new List<TagOption>();
        }
    }

    // Is provided by a Tag
    public class TagOption
    {
        public int Id           { get; set; }
        public Keyword Keyword  { get; set; }
    }

    // TagOption with two text items
    public class DoubleTextTagOption : TagOption
    {
        //!! still needed?
        public string MainText  { get; set; }
        public string SubText   { get; set; }
    }

    // TagOption with one text item
    public class SingleTextTagOption : TagOption
    {
        public string Text { get; set; }
    }
}
