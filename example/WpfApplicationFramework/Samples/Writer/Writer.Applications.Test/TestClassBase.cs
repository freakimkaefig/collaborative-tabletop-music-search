using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waf.Writer.Applications.Controllers;

namespace Waf.Writer.Applications.Test
{
    [TestClass]
    public abstract class TestClassBase
    {
        private readonly CompositionContainer container;
        private PrintController printController;
        private RichTextDocumentController richTextDocumentController;

        
        protected TestClassBase()
        {
            AggregateCatalog catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new AssemblyCatalog(typeof(ApplicationController).Assembly));
            catalog.Catalogs.Add(new AssemblyCatalog(typeof(TestClassBase).Assembly));
            
            container = new CompositionContainer(catalog);
            CompositionBatch batch = new CompositionBatch();
            batch.AddExportedValue(container);
            container.Compose(batch);
        }


        protected CompositionContainer Container { get { return container; } }


        [TestInitialize]
        public void TestInitialize()
        {
            richTextDocumentController = Container.GetExportedValue<RichTextDocumentController>();
            Container.GetExportedValue<FileController>().Initialize();
            
            OnTestInitialize();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            OnTestCleanup();
        }

        protected virtual void OnTestInitialize() { }

        protected virtual void OnTestCleanup() { }

        internal PrintController InitializePrintController()
        {
            printController = container.GetExportedValue<PrintController>();
            printController.Initialize();
            return printController;
        }

        protected void ShutdownPrintController()
        {
            printController.Shutdown();
            printController = null;
        }
    }
}
