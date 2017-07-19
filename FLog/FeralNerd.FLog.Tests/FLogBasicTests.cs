using System;
using FeralNerd.FLog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeralNerd.FLog.Tests
{
    [TestClass]
    public class FLogBasicTests
    {
        [TestMethod]
        public void CreateLoggerAndLog()
        {
            UnitTestLoggerHandler handler = new UnitTestLoggerHandler();
            using (Logger logger = new Logger())
            {
                logger.AddHandler(handler);

                logger.WriteLine("Yep.");
            }

            Assert.IsTrue(handler.GetLines().EndsWith("Yep."));
        }
    }
}
