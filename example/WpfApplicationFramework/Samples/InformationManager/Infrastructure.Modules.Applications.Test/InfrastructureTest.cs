using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Test.InformationManager.Common.Applications;
using Waf.InformationManager.Infrastructure.Modules.Applications.Controllers;

namespace Test.InformationManager.Infrastructure.Modules.Applications
{
    [TestClass]
    public abstract class InfrastructureTest : TestClassBase
    {
        protected override void OnCatalogInitialize(AggregateCatalog catalog)
        {
            base.OnCatalogInitialize(catalog);
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ModuleController).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(InfrastructureTest).Assembly));
        }
    }
}
