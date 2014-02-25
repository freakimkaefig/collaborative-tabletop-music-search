using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MusicStream.Test
{
    [TestClass]
    public class StreamingTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            MusicStreamSessionManager _sessionManager = new MusicStreamSessionManager();
            //_sessionManager.GetLoadedTrackWorker();
            Assert.IsFalse(false);
        }
    }
}
