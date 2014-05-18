using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using log4net;

namespace Ctms.Applications.Common
{
    class Configurator
    {
        /// <summary>
        /// Init log4net Logger
        /// </summary>
        public static readonly ILog Log = LogManager.GetLogger("Ctms");

        public static void Init()
        {
            InitPaths();
            InitLogging();
        }

        /// <summary>
        /// Init log4net logging which is configured in App.config of Presentation project
        /// </summary>
        private static void InitLogging()
        {
            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// Initialize paths relatively
        /// </summary>
        private static void InitPaths()
        {
            var assemblyPath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            CommonVal.Path_AppPath                  = Path.GetFullPath(assemblyPath + "/../../../..");
            CommonVal.Path_ApplicationsPath         = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Applications");
            CommonVal.Path_DomainPath               = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Domain");
            CommonVal.Path_PresentationPath         = Path.GetFullPath(CommonVal.Path_AppPath + "/Ctms.Presentation");
            CommonVal.Path_ApplicationsFilesPath    = Path.GetFullPath(CommonVal.Path_ApplicationsPath + "/Files");
            CommonVal.Path_ViewsPath                = Path.GetFullPath(CommonVal.Path_PresentationPath + "/Views");
        }
    }
}
