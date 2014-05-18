using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Waf.BookLibrary.Library.Applications.Controllers;
using System.ComponentModel.Composition;

namespace Waf.BookLibrary.Library.Applications.Test.Controllers
{
    [Export(typeof(IEntityController)), Export]
    public class MockEntityController : IEntityController
    {
        public bool InitializeCalled { get; set; }
        public bool ShutdownCalled { get; set; }
        public bool HasChangesResult { get; set; }
        public bool CanSaveResult { get; set; }
        public bool SaveResult { get; set; }
        public bool SaveCalled { get; set; }


        public MockEntityController()
        {
            CanSaveResult = true;
        }

        
        public bool HasChanges
        {
            get { return HasChangesResult; }
        }

        public void Initialize()
        {
            InitializeCalled = true;
        }

        public bool CanSave() { return CanSaveResult; }

        public bool Save()
        {
            SaveCalled = true;
            return SaveResult;
        }

        public void Shutdown()
        {
            ShutdownCalled = true;
        }
    }
}
