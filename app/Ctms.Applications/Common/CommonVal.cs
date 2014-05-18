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
         * Values
        */
        // number of tags that can be used
        public const int Tag_MaxNumber              = 6;

        // count options that shall be displayd in a tag circle menu
        public const int Tag_VisibleOptionsCount    = 10;

        // angle after which tag turning is adapted
        public const int Tag_OptionsStepAngle       = 10;

        // radius in which tags can be combined
        public const int Tag_CombineCircleDiameter  = 280;

        // max number of results to display
        public const int Results_MaxNumber          = 20;

        // max number of results to collect internally
        public const int Results_MaxCollectNumber   = 100;


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

        // Paths to images for each attribute (right now identical for all types)
        public static string ImageSource_TagBackgrArtist = "../Resources/Images/Common/icons/vinylTangible4.png";
        public static string ImageSource_TagBackgrTitle = "../Resources/Images/Common/icons/vinylTangible4.png";
        public static string ImageSource_TagBackgrAttribute = "../Resources/Images/Common/icons/vinylTangible4.png";
        public static string ImageSource_TagBackgrGenre = "../Resources/Images/Common/icons/vinylTangible4.png";

        // colors for tags
        public static Dictionary<int, string> TagColors = new Dictionary<int, string>()
        {
            { 0, "#0f0" },
            { 1, "#00f" },
            { 2, "#f00" },
            { 3, "#ff0" },
            { 4, "#0ff" },
            { 5, "#f0f" }
        };

        // actual height of application window
        public static short WindowHeight { get; set; }

    }
}
