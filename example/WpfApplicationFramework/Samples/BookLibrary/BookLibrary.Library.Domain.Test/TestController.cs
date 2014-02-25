using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Waf.BookLibrary.Library.Domain.Test
{
    [TestClass]
    public class TestController
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            TypeDescriptor.AddProviderTransparent(new AssociatedMetadataTypeTypeDescriptionProvider(typeof(EntityObject)),
                typeof(EntityObject));
        }
    }
}
