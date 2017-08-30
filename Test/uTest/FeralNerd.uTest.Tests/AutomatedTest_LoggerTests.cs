using FeralLib.uTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace FeralNerd.uTest.Tests
{
    [TestClass]
    // [Ignore] // Un-comment this attribute if you want to run these tests.
    public class AutomatedTest_LoggerTests : AutomatedTest
    {
        [TestMethod]
        [Description("Demo of how to use the Logger object.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_LoggerDemo()
        {
            Logger.WriteLine("This test demonstrates how to use the Logger object.");
            Logger.WriteLine();

            int x = 0;
            try
            {
                for (x = 1; x < 10; x++)
                {
                    // Test some random thing.
                    foo(x);
                    Logger.LogResult(Result.Pass, "Iteraiton {0}.  Everything's good so far.", x);
                }

                Logger.LogResult(Result.Pass, "Testing completed without an error.");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Caught an exception on try number {0}", x);
            }
        }



        [TestMethod]
        [Description("Test that doesn't use Logger")]
        public void ValidateNotUsingLogger()
        {
            Console.WriteLine("This test demonstrates what happens if you don't use the Logger object.");
            Console.WriteLine();
            Console.WriteLine("It should just pass.");
            Console.WriteLine("You SHOULD NOT see any banners from the Logger object.");
        }

        [TestMethod]
        [Description("Test that doesn't log a result")]
        [TestCategory("LocalOnly")]
        public void ShouldSkip_ValidateNoResultLogged()
        {
            Logger.WriteLine("This test demonstrates what happens if you initialize the Logger object, but don't log a result.");
            Logger.WriteLine("Expected final outcome: Skipped.");
            Logger.WriteLine();
        }

        [TestMethod]
        [Description("Test that logs a information.")]
        [TestCategory("LocalOnly")]
        public void ShouldSkip_ValidateInformation()
        {
            Logger.WriteLine("This test demonstrates what happens when you log an information result.");
            Logger.WriteLine("Expected final outcome: Skipped.");
            Logger.WriteLine();

            Logger.LogResult(Result.Information, "This demonstrates an informational result.");
        }

        [TestMethod]
        [Description("Test that logs a pass.")]
        public void ValidatePass()
        {
            Logger.WriteLine("This test demonstrates what happens when you log a passing result.");
            Logger.WriteLine("This test should have an outcome of a pass.");
            Logger.WriteLine();

            Logger.LogResult(Result.Pass, "This demonstrates a passing result.");
        }

        [TestMethod]
        [Description("Test that logs a warning.")]
        public void ValidateWarning()
        {
            Logger.WriteLine("This test demonstrates what happens when you log a warning result.");
            Logger.WriteLine("This test should have an outcome of a pass.");
            Logger.WriteLine();

            Logger.LogResult(Result.Warning, "This demonstrates a warning result.");
        }

        [TestMethod]
        [Description("Test that logs a fail.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateFail()
        {
            Logger.WriteLine("This test demonstrates what happens when you log a failing result.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Logger.LogResult(Result.Fail, "This demonstrates a failing result.");
        }

        [TestMethod]
        [Description("Test that logs a skipped.")]
        [TestCategory("LocalOnly")]
        public void ShouldSkip_ValidateSkipped()
        {
            Logger.WriteLine("This test demonstrates what happens when you log a skipped result.  You can");
            Logger.WriteLine("get a skipped result by logging infomational or ignore results, but those");
            Logger.WriteLine("can be overridden if you log a pass/warning/fail.  Logging a skipped will force");
            Logger.WriteLine("the final result to be skipped.");
            Logger.WriteLine("Expected final outcome: Skipped.");
            Logger.WriteLine();

            Logger.LogResult(Result.Skipped, "This demonstrates a skipped result.");
        }

        [TestMethod]
        [Description("Test the priority of a pass.")]
        public void ValidatePriorityPass()
        {
            Logger.WriteLine("This test demonstrates that a passing result overrides an ignore / infomration result.");
            Logger.WriteLine("This test should have an outcome of pass.");
            Logger.WriteLine();

            Logger.LogResult(Result.Pass, "This demonstrates a passing result.");
            Logger.LogResult(Result.Information, "This demonstrates an info result.");
            Logger.LogResult(Result.Ignore, "This demonstrates an ignore result.");
        }

        [TestMethod]
        [Description("Test the priority of a warning.")]
        public void ValidatePriorityWarning()
        {
            Logger.WriteLine("This test demonstrates that a warning result overrides a passing result.");
            Logger.WriteLine("This test should have an outcome of pass.");
            Logger.WriteLine();

            Logger.LogResult(Result.Warning, "This demonstrates a passing result.");
            Logger.LogResult(Result.Pass, "This demonstrates a passing result.");
            Logger.LogResult(Result.Information, "This demonstrates an info result.");
            Logger.LogResult(Result.Ignore, "This demonstrates an ignore result.");
        }

        [TestMethod]
        [Description("Test the priority of a fail.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidatePriorityFail()
        {
            Logger.WriteLine("This test demonstrates that a fail result overrides a passing/warning result.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Logger.LogResult(Result.Fail, "This demonstrates a failing result.");
            Logger.LogResult(Result.Warning, "This demonstrates a passing result.");
            Logger.LogResult(Result.Pass, "This demonstrates a passing result.");
            Logger.LogResult(Result.Information, "This demonstrates an info result.");
            Logger.LogResult(Result.Ignore, "This demonstrates an ignore result.");
        }

        [TestMethod]
        [Description("Test the priority of a skip.")]
        [TestCategory("LocalOnly")]
        public void ShouldSkip_ValidatePrioritySkip()
        {
            Logger.WriteLine("This test demonstrates that a skip result overrides all other results.");
            Logger.WriteLine("Expected final outcome: Skipped.");
            Logger.WriteLine();

            Logger.LogResult(Result.Skipped, "This demonstrates a failing result.");
            Logger.LogResult(Result.Fail, "This demonstrates a failing result.");
            Logger.LogResult(Result.Warning, "This demonstrates a passing result.");
            Logger.LogResult(Result.Pass, "This demonstrates a passing result.");
            Logger.LogResult(Result.Information, "This demonstrates an info result.");
            Logger.LogResult(Result.Ignore, "This demonstrates an ignore result.");
        }


        [TestMethod]
        [Description("Test Abort")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_AbortBailsOutOfATest()
        {
            Logger.WriteLine("This test demonstrates that calling Abort will immediately fail a test and bail out.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");
            Logger.Abort("Oh, the HUMANITY!!!");
            Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE!!!");
        }


        [TestMethod]
        [Description("Test Abort with exception")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_AbortWithException()
        {
            Logger.WriteLine("This test demonstrates that calling Abort will immediately fail a test and bail out.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            try
            {
                for (int x = 0; x < 10; x++)
                {
                    Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");
                    if (x >= 2)
                        throw new Exception("Oh, my gosh!  Why???  This is how it ends!!");
                }

                Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE!!!");
            }
            catch (Exception ex)
            {
                Logger.Abort(ex, "The best-laid plans of mice and men so often go astray.");
            }
        }


        [TestMethod]
        [Description("Test Assert with a passing result.")]
        public void ValidateAssertPassing()
        {
            Logger.WriteLine("This test validates calling assert with a passing result.");
            Logger.WriteLine();

            Logger.AssertIsTrue(1 + 1 == 2, "Make sure 1 + 1 is 2.");
            Logger.AssertIsTrue(true, "Make sure true is the source of truth.");
        }


        [TestMethod]
        [Description("Test Assert with a failing result.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateAssertFailing()
        {
            Logger.WriteLine("This test validates calling assert with a failing result.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Logger.AssertIsTrue(1 == 0, "Check that a false result fails.");
        }


        [TestMethod]
        [Description("Test AssertIsEqual with a passing result.")]
        public void ValidateAssertIsEqualPassing()
        {
            Logger.WriteLine("This test validates calling AssertIsEqual with a passing result.");
            Logger.WriteLine();

            Logger.AssertIsEqual(1, 101 - 100, "Make sure we get a 1.");
        }


        [TestMethod]
        [Description("Test AssertIsEqual with a failing result.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateAssertIsEqualFailing()
        {
            Logger.WriteLine("This test validates calling AssertIsEqual with a failing result.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Logger.AssertIsEqual(1, 100 - 100, "Check that the value is 1.");
        }


        [TestMethod]
        [Description("Test AssertIsOneOf with a passing result.")]
        public void ValidateAssertIsOneOfPassing()
        {
            Logger.WriteLine("This test validates calling AssertIsOneOf with a passing result.");
            Logger.WriteLine();

            int[] testValues = new int[] { 1, 2, 3, 5, 8, 13, 21 };
            Logger.AssertIsOneOf(testValues, 8, "Make sure we got a Fibonacci value.");
        }


        [TestMethod]
        [Description("Test AssertIsOneOf with a failing result.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateAssertIsOneOfFailing()
        {
            Logger.WriteLine("This test validates calling AssertIsOneOf with a failing result.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            int[] testValues = new int[] { 1, 2, 3, 5, 8, 13, 21 };
            Logger.AssertIsOneOf(testValues, 9, "Make sure we got a Fibonacci value.");
        }


        [TestMethod]
        [Description("Test CompareLists")]
        public void ValidateCompareLists()
        {
            Logger.WriteLine("This test validates comparing two lists.  Red, green, and blue should show up as matching.");
            Logger.WriteLine("Black and white should show up as missing, and get logged as a warning.  Apple, banana, and peach");
            Logger.WriteLine("should show up as extras, and get logged as Info.");

            string[] expectedValues = new string[] { "white", "red", "green", "blue", "black" };
            string[] actualValues = new string[] { "Red", "GREEN", "BlUe", "apple", "banana", "peach" };
            Logger.CompareLists(expectedValues, actualValues, Result.Warning, Result.Information, true);
        }

        [TestMethod]
        [Description("Test exception without a message.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateExceptionWithoutMessage()
        {
            Logger.WriteLine("This test will show what happens when you just throw an exception and catch it.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            try
            {
                for (int x = 0; x < 10; x++)
                {
                    Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");
                    if (x >= 2)
                        throw new Exception("Oh, my gosh!  Why???  This is how it ends!!");
                }

                Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE!!!");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        [TestMethod]
        [Description("Test exception with a message.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateExceptionWithMessage()
        {
            Logger.WriteLine("This test will show what happens when you just throw an exception and catch it.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            int x = 0;
            try
            {
                for (x = 1; x < 10; x++)
                {
                    Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");
                    if (x == 3)
                        throw new Exception("Oh, my gosh!  Why???  This is how it ends!!");
                }

                Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE!!!");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Caught an exception on try number {0}", x);
            }
        }

        private void bar(int x)
        {
            try
            {
                if (x == 3)
                    throw new Exception("Oh, my gosh!  Why???  This is how it ends!!");
            }
            catch (Exception ex)
            {
                throw new System.NotSupportedException("WTH are you doing??", ex);
            }
        }

        private void foo(int x)
        {
            try
            {
                bar(x);
            }
            catch (Exception ex)
            {
                throw new AutomationFrameworkException(ex, "A terrible, no-good, very bad thing happened!");
            }
        }

        [TestMethod]
        [Description("Test exception with multiple inner exceptions.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateExceptionWithInnerException()
        {
            Logger.WriteLine("This test will show what happens when you just throw an exception and catch it.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            int x = 0;
            try
            {
                for (x = 1; x < 10; x++)
                {
                    foo(x);
                    Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");
                }

                Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE!!!");
            }
            catch (Exception ex)
            {
                Logger.LogException(ex, "Caught an exception on try number {0}", x);
            }
        }

        [TestMethod]
        [Description("Test when a test throws an unhandled exception.")]
        [TestCategory("LocalOnly")]
        public void ShouldFail_ValidateFailOnUnhandledException()
        {
            Logger.WriteLine("This test will show what happens when a test throws an unhandled exception.  The log should NOT say");
            Logger.WriteLine("that the test was skipped.  It should say FAIL.");
            Logger.WriteLine("Expected final outcome: Failed.");
            Logger.WriteLine();

            Dictionary<string, string> someDictionary = new Dictionary<string, string>();
            Logger.LogResult(Result.Pass, "Laa-de-daa.  Testing stuff...everything's good so far.");

            Logger.AssertIsEqual("Hoo boy!", someDictionary["bad key"], "Check the random thing.");
            Logger.LogResult(Result.Information, "YOU SHOULD NOT SEE THIS MESSAGE.");
        }
    }
}
