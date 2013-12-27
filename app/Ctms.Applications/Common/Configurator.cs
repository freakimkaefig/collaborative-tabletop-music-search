using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ctms.Applications.Common
{
    class Configurator
    {

        public static void Init()
        {
            InitPaths();
            //Init...();
        }

        //Initialize paths relatively
        private static void InitPaths()
        {
            var assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            CommonVal.Path_AppPath                  = Path.GetFullPath(assemblyPath + "/../../../..");
            CommonVal.Path_ApplicationsPath         = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Applications");
            CommonVal.Path_DomainPath               = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Domain");
            CommonVal.Path_PresentationPath         = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Presentation");
            CommonVal.Path_ApplicationsFilesPath    = Path.GetFullPath(CommonVal.Path_ApplicationsPath + "/Files");

        }
    }
}
