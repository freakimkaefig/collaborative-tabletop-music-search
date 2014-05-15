﻿using System;
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
using Waf.BookLibrary.Library.Applications.ViewModels;
using Waf.BookLibrary.Library.Presentation.Properties;

namespace Waf.BookLibrary.Library.Presentation
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

#if (DEBUG != true)
            // Don't handle the exceptions in Debug mode because otherwise the Debugger wouldn't
            // jump into the code when an exception occurs.
            DispatcherUnhandledException += AppDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
#endif

            catalog = new AggregateCatalog();
            // Add the WpfApplicationFramework assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(Controller).Assembly));
            // Add the Waf.BookLibrary.Library.Presentation assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
            // Add the Waf.BookLibrary.Library.Applications assembly to the catalog
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ShellViewModel).Assembly));

            // Load module assemblies as well (e.g. Reporting extension). See App.config file.
            foreach(string moduleAssembly in Settings.Default.ModuleAssemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(moduleAssembly));
            }
            
            container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);

            moduleControllers = container.GetExportedValues<IModuleController>();
            foreach (IModuleController moduleController in moduleControllers) { moduleController.Initialize(); }
            foreach (IModuleController moduleController in moduleControllers) { moduleController.Run(); }
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
                MessageBox.Show(string.Format(CultureInfo.CurrentCulture,
                        Waf.BookLibrary.Library.Presentation.Properties.Resources.UnknownError, e.ToString())
                    , ApplicationInfo.ProductName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
