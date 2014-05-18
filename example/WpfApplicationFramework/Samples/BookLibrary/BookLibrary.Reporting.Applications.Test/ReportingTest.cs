using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.BookLibrary.Library.Applications.Test;
using Waf.BookLibrary.Reporting.Applications.Controllers;

namespace Waf.BookLibrary.Reporting.Applications.Test
{
    [TestClass]
    public class ReportingTest : TestClassBase
    {
        protected override void OnCatalogInitialize(AggregateCatalog catalog)
        {
            base.OnCatalogInitialize(catalog);
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleController).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ReportingTest).Assembly));
        }
    }
}
