using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Reflection;

namespace FeralLib.uTest
{
    [TestClass]
    public class AutomatedTest
    {
        private Logger _logger = null;
        private string _testTitle = null;

        public AutomatedTest()
        {
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the Logger object.  If you never use this property, no 
        ///     logger will be created.
        /// </summary>
        public Logger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new Logger(TestContext, _testTitle);

                return _logger;
            }

            private set { _logger = value; }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns the TestContext object.  This property is populated for 
        ///     you by the MS-test framework.
        /// </summary>
        public TestContext TestContext { get; set; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Test cleanup method for the AutomatedTest base class.  If your 
        ///     test class needs to do any cleanup of its own, you should 
        ///     define your own cleanup method.
        /// </summary>
        [TestCleanup]
        public void AutomatedTest_TestCleanup()
        {
            if (_logger != null)
                Logger.FinalizeResult();
            _logger = null;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Test initialization method for the AutomatedTest base class.  If 
        ///     your test class needs to do any initialization of its own, you 
        ///     should define your own initialization method.
        /// </summary>
        [TestInitialize]
        public void AutomatedTest_TestInitialize()
        {
            _testTitle = null;
            if (TestContext != null)
            {
                _testTitle = TestContext.TestName;

                // Find the method that VS is about to run, so we can create a startup banner.
                Type type = this.GetType();
                MethodInfo mi = type.GetMethod(_testTitle, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                if (mi != null)
                {
                    DescriptionAttribute attr = mi.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                        _testTitle = attr.Description;
                }
            }
        }
    }
}
