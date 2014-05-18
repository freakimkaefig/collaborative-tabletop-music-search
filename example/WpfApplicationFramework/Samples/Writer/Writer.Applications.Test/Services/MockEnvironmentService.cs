using System.ComponentModel.Composition;
using Waf.Writer.Applications.Services;

namespace Waf.Writer.Applications.Test.Services
{
    [Export(typeof(IEnvironmentService)), Export]
    public class MockEnvironmentService : IEnvironmentService
    {
        public string DocumentFileName { get; set; }
    }
}
