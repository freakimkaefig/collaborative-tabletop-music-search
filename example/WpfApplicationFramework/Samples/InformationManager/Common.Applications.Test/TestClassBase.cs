using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.InformationManager.Common.Applications
{
    [TestClass]
    public abstract class TestClassBase
    {
        private CompositionContainer container;


        public CompositionContainer Container { get { return container; } }


        [TestInitialize]
        public void TestInitialize()
        {
            AggregateCatalog catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TestClassBase).Assembly));

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
