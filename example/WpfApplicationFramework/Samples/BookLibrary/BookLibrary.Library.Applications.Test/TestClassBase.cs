using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.BookLibrary.Library.Applications.Controllers;
using Waf.BookLibrary.Library.Applications.Services;
using Waf.BookLibrary.Library.Applications.ViewModels;

namespace Waf.BookLibrary.Library.Applications.Test
{
    [TestClass]
    public abstract class TestClassBase
    {
        private CompositionContainer container;


        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(EntityObject)),
                typeof(EntityObject));
        }


        public CompositionContainer Container { get { return container; } }


        [TestInitialize]
        public void TestInitialize()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TestClassBase).Assembly));
            catalog.Catalogs.Add(new TypeCatalog(typeof(ModuleController), typeof(BookController), typeof(PersonController), 
                typeof(ShellService), typeof(ShellViewModel), typeof(BookViewModel), typeof(PersonViewModel)
            ));

            OnCatalogInitialize(catalog);

            container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);
            
            OnTestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();

            if (container != null) { container.Dispose(); }
        }

        protected virtual void OnCatalogInitialize(AggregateCatalog catalog) { }

        protected virtual void OnTestInitialize() { }

        protected virtual void OnTestCleanup() { }
    }
}
