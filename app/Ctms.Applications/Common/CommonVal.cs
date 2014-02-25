using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ctms.Applications.Common
{
    //Contains static values and enums
    class CommonVal
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
        public const int MaxTagNumber = 12;

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

        

    }
}
