using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Waf;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Threading;
using Ctms.Applications.ViewModels;
using Ctms.Presentation.Properties;

namespace Ctms.Presentation
{
    public partial class App : Application
    {
        private AggregateCatalog catalog;
        private CompositionContainer container;
        private IEnumerable<IModuleController> moduleControllers;


        static App()
        {
#if (DEBUG)
            WafConfiguration.Debug = true;
#endif
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

//#if (DEBUG != true)
            // Don't handle the exceptions in Debug mode because otherwise the Debugger wouldn't
            // jump into the code when an exception occurs.
            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
//#endif

            catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Controller).Assembly));
            // Add the Ctms.Presentation assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            // Add the Ctms.Applications assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));

            container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            //ModuleController gets loaded. If no controller found check the contstructor arguments
            moduleControllers = container.GetExportedValues<IModuleController>();
            //moduleControllers = container.GetExportedValues<Controller>();
            foreach (IModuleController moduleController in moduleControllers) { moduleController.Initialize(); }
            foreach (IModuleController moduleController in moduleControllers) { moduleController.Run(); }

            //throw new Exception("Halts Maul");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            foreach (IModuleController moduleController in moduleControllers.Reverse()) { moduleController.Shutdown(); }
            container.Dispose();
            catalog.Dispose();

            base.OnExit(e);
        }

        private void AppDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception, false);
            e.Handled = true;
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception, e.IsTerminating);
        }

        private static void HandleException(Exception e, bool isTerminating)
        {
            if (e == null) { return; }

            Trace.TraceError(e.ToString());

            if (!isTerminating)
            {
                //MessageBox.Show(string.Format(CultureInfo.CurrentCulture,
                //        "Unknown Error", e.ToString())
                //    , ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);

                string errorMessage = string.Format("An unhandled exception occurred: {0}", e.Message);
                MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        
            }
        }
    }
}
