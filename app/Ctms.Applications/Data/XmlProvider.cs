using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Ctms.Domain;

namespace Ctms.Applications.Services
{
    public static class XmlProvider
    {
        /// <summary>
        /// Read all possible styles from xml file and create objects
        /// </summary>
        /// <returns>return style objects</returns>
        public static List<Style> ReadStyles()
        {
            //ToDo: Read styles from xml
            var styles = new List<Style>()
            {
                new Style(0, "Rock"),
                new Style(1, "Drum 'n' Bass"),
                new Style(2, "Elektro")               
            };
            return styles;
        }
    }
}
