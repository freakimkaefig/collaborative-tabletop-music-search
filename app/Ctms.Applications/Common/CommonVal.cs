using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;

namespace Ctms.Applications.Common
{
    //Contains static values and enums
    public class CommonVal
    {
        /*
         * State variables
        */
        //Example 
        public static string State_UiIsBlocked { get; set; }
        //Example for an enum
        public enum AppMode
        {
            Search,
            Playlist
        }

        //Values
        public const int Tag_MaxNumber              = 12;
        public const int Tag_VisibleOptionsCount    = 10;
        public const int Tag_OptionsStepAngle       = 10;
        public const int Tag_CombineCircleDiameter  = 350;

        /*
         * Path variables
        */
        //Example: public static readonly string Path_Settings { get { return "../Settings.xml"; } }
        public static string Path_AppPath               { get; set; }
        public static string Path_ApplicationsPath      { get; set; }
        public static string Path_DomainPath            { get; set; }
        public static string Path_PresentationPath      { get; set; }
        public static string Path_ApplicationsFilesPath { get; set; }
        public static string Path_ViewsPath             { get; set; }

        public static string ImageSource_TagBackgrArtist = "../Resources/Images/Search/tag-backgr.png";
        public static string ImageSource_TagBackgrTitle = "../Resources/Images/Search/tag-backgr.png";
        public static string ImageSource_TagBackgrAttribute = "../Resources/Images/Search/tag-backgr.png";
        public static string ImageSource_TagBackgrGenre = "../Resources/Images/Search/tag-backgr.png";

        public static Dictionary<int, string> TagColors = new Dictionary<int, string>()
        {
            { 1, "Red" },
            { 2, "Blue" },
            { 3, "Yellow" }
        };

    }
}
