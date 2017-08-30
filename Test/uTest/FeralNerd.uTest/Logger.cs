using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VSTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FeralLib.uTest
{
    /// <summary>
    ///     The set of result states for the current test case.<para/>
    ///     <para/>
    ///     IMPORTANT: VS-test will consider a result as SKIPPED unless you log
    ///     a different result.
    /// </summary>
    public enum Result
    {
        /// <summary>
        ///     This is a null result.  The logger object will ignore this result
        /// </summary>
        Ignore,


        /// <summary>
        ///     This is a general FYI.  It is neither a positive or a negative
        ///     result.  If you do not log a Pass/Fail, this test case will have
        ///     a final result of skipped.
        /// </summary>
        Information,


        /// <summary>
        ///     Logs a passing result.
        /// </summary>
        Pass,


        /// <summary>
        ///     Logs a warning.  The final result is still passing.
        /// </summary>
        Warning,


        /// <summary>
        ///     Logs a failing result.  The final result will be a Fail.
        /// </summary>
        Fail,


        /// <summary>
        ///     This test case is not applicable for the given environment.
        ///     It will be skipped.
        /// </summary>
        Skipped
    }


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This delegate is used for hooking into any kind of external logging
    ///     mechanism that you want to patch into.
    /// </summary>
    /// <param name="text"></param>
    public delegate void LogToExternalHandler(string text);


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A class for managing diagnostic test cases
    /// </summary>
    public class Logger
    {
        #region ### Private Fields #############################################

        // This is the cumulative result for the current test case.
        private Result _cumulativeResult = Result.Ignore;
        private string _firstError = "";
        private bool _abort = false;
        private TestContext _testContext;

        #endregion


        #region ### Constructor ################################################

        public Logger(TestContext testContext, string bannerText = null)
        {
            LogToExternal += delegate (string msg)
            {
                if (msg == null)
                    return;

                _testContext.WriteLine(msg);
                Debug.WriteLine(msg);
            };

            _testContext = testContext;
            if (bannerText == null)
                return;

            StringBuilder sb = new StringBuilder("***   ");
            foreach (char ch in bannerText)
            {
                sb.Append(ch);
                sb.Append(" ");
            }

            LogToExternal("********************************************************************************");
            LogToExternal(sb.ToString());
            LogToExternal("********************************************************************************");
        }

        #endregion


        #region ### Public Instance Methods ####################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Logs a failing result and aborts a test case.  This method calls
        ///     the VS-test Assert.Fail() method, which throws an exception.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     An exception that was caught.
        /// </param>
        public void Abort(Exception ex, string msg, params object[] args)
        {
            _abort = true;
            LogException(ex, msg, args);
            VSTest.Assert.Fail(msg, args);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Logs a failing result and aborts a test case.  This method calls
        ///     the VS-test Assert.Fail() method, which throws an exception.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     An exception that was caught.
        /// </param>
        public void Abort(string msg, params object[] args)
        {
            _abort = true;
            LogResult(Result.Fail, msg, args);
            VSTest.Assert.Fail(msg, args);
        }


        //////////////////////////////////////////
        /// <summary>
        ///     Logs a passing result if the test parameter is true, otherwise
        ///     logs a failing result.
        /// </summary>
        /// 
        /// <param name="test">
        ///     A Boolean test.
        /// </param>
        /// <param name="msg">
        ///     Any text you want written to the log-output.  This should be 
        ///     a brief description of the condition you're testing for, like 
        ///     "Make sure the thing is X."
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        public bool AssertIsTrue(bool test, string msg, params object[] args)
        {
            string text = (args.Length > 0) ? string.Format(msg, args) : msg;
            LogResult(
                test ? Result.Pass : Result.Fail,
                "ASSERT: " + text);

            return test;
        }


        //////////////////////////////////////////
        /// <summary>
        ///     Verify that two values are equal, and logs a result.
        /// </summary>
        /// 
        /// <typeparam name="T">
        ///     Type parameter.
        /// </typeparam>
        /// <param name="expectedValue">
        ///     The expected value.
        /// </param>
        /// <param name="actualVaule">
        ///     The value you want to have tested.
        /// </param>
        /// <param name="msg">
        ///     A short description of the check.  Use simple wording that just 
        ///     names the thing you're comparing, like "Check the address." or 
        ///     "Verify the connection status."
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        public bool AssertIsEqual<T>(T expectedValue, T actualVaule, string msg, params object[] args)
        {
            string text = string.Format(msg, args);

            string exp = (expectedValue != null) ? expectedValue.ToString() : "/NULL/";
            string obs = (actualVaule != null) ? actualVaule.ToString() : "/NULL/";

            if (exp == obs)
                LogResult(Result.Pass, "ASSERT: {0}  Found expected value: [{1}]", text, exp);
            else
                LogResult(Result.Fail, "ASSERT: {0}  Expected=[{1}]  Actual=[{2}]", text, exp, obs);

            return (exp == obs);
        }


        //////////////////////////////////////////
        /// <summary>
        ///     Verify that an observed value is one of a list of possible 
        ///     values.<para/>
        ///     <para/>
        ///     Do not call this method in your DiagnosticClassInitialize or
        ///     DiagnosticClassFinalize method.
        /// </summary>
        /// 
        /// <typeparam name="T">
        ///     Type parameter.
        /// </typeparam>
        /// <param name="expectedValues">
        ///     An array of expected values.
        /// </param>
        /// <param name="actualVaule">
        ///     The value that you read from the object under test.
        /// </param>
        /// <param name="msg">
        ///     A short description.  Use simple wording, like "Verify name." or "Check the status."
        /// </param>
        /// <param name="args"></param>
        public void AssertIsOneOf<T>(T[] expectedValues, T actualVaule, string msg, params object[] args)
        {
            string text = string.Format(msg, args);
            string obs = (actualVaule != null) ? actualVaule.ToString() : "/NULL/";

            bool found = false;
            foreach (T testVal in expectedValues)
            {
                string exp = (testVal != null) ? testVal.ToString() : "/NULL/";

                if (string.Compare(exp, obs, true) == 0)
                {
                    LogResult(Result.Pass, "ASSERT: {0}  Found expected value: [{1}]", text, exp);
                    found = true;
                    break;
                }
            }

            if (!found)
                LogResult(Result.Fail, "ASSERT: {0}  ExampleValue=[{1}]  Actual=[{2}]", text, expectedValues[0], obs);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Compares two lists of strings, and flags missing / extra items.<para/>
        /// </summary>
        /// 
        /// <param name="expectedItems">
        ///     A list of expected string items.
        /// </param>
        /// <param name="actualItems">
        ///     The list of actual items taken from the object under test.
        /// </param>
        /// <param name="missingItemsResult">
        ///     Result you want to log for items missing from actualItemList.
        ///     If you use Result.Ignore, then missing items will be ignored.
        ///     Default value is Result.Fail.
        /// </param>
        /// <param name="extraItemsResult">
        ///     Result you want to log for extra items found in actualItemList.
        ///     If you use Result.Ignore, then all extra items will be ignored.
        ///     Default value is Result.Fail.
        /// </param>
        /// <param name="ignoreCase">
        ///     Set this to true if you want to ignore case.
        /// </param>
        public void CompareLists(IEnumerable<string> expectedItems, IEnumerable<string> actualItems, Result missingItemsResult = Result.Fail, Result extraItemsResult = Result.Fail, bool ignoreCase = false)
        {
            // Make a copy of both lists.  This process is destructive.
            List<string> copyOfExpected = new List<string>();
            foreach (string str in expectedItems)
                copyOfExpected.Add(ignoreCase ? str.ToLower() : str);

            List<string> copyOfActual = new List<string>();
            foreach (string str in actualItems)
                copyOfActual.Add(ignoreCase ? str.ToLower() : str);

            for (int i = copyOfExpected.Count - 1; i >= 0; i--)
            {
                string expectedItem = copyOfExpected[i];
                string matchingItem = copyOfActual.FirstOrDefault(x => x == expectedItem);
                if (matchingItem == null)
                    continue;

                LogResult(Result.Pass, "Found matching item: \"{0}\"", matchingItem);
                copyOfExpected.Remove(matchingItem);
                copyOfActual.Remove(matchingItem);
            }

            if (missingItemsResult != Result.Ignore)
            {
                foreach (string missingItem in copyOfExpected)
                    LogResult(missingItemsResult, "Missing item: {0}", missingItem);
            }

            if (extraItemsResult != Result.Ignore)
            {
                foreach (string extraItem in copyOfActual)
                    LogResult(extraItemsResult, "Extra item: {0}", extraItem);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns the current result of the test.
        /// </summary>
        public Result CurrentResult { get { return _cumulativeResult; } }


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Logs a final result for a test case.  Call this method when
        ///     your test case is finished.
        /// </summary>
        internal void FinalizeResult()
        {
            // Make sure MS-Test and the Logger agree on the final result.
            if (_cumulativeResult < Result.Fail && _testContext.CurrentTestOutcome != UnitTestOutcome.Passed)
            {
                switch (_testContext.CurrentTestOutcome)
                {
                    case UnitTestOutcome.Aborted:
                    case UnitTestOutcome.Error:
                    case UnitTestOutcome.Failed:
                    case UnitTestOutcome.Timeout:
                    case UnitTestOutcome.Unknown:
                        _abort = true;
                        _cumulativeResult = Result.Fail;
                        LogResult(Result.Fail, "FATAL ERROR.  The test aborted unexpectedly.");
                        break;

                    case UnitTestOutcome.Inconclusive:
                        _cumulativeResult = Result.Skipped;
                        break;
                }
            }

            if (_cumulativeResult < Result.Pass)
            {
                LogResult(_cumulativeResult, "No pass/fail results were logged for this test case.");
                _cumulativeResult = Result.Skipped;
            }

            WriteSectionBreak("END TEST");
            LogToExternal(_cumulativeResult.ToString().ToUpper());
            LogToExternal("");

            switch (_cumulativeResult)
            {
                case Result.Ignore:
                case Result.Information:
                    VSTest.Assert.Inconclusive("No pass/fail results were logged for this test case.");
                    break;

                case Result.Fail:
                    // Don't call Assert.Fail if the abort flag has been set.  If we do,
                    // it will log the message twice, which looks funny on the report.
                    if (!_abort)
                        VSTest.Assert.Fail(_firstError);

                    break;

                case Result.Skipped:
                    VSTest.Assert.Inconclusive("This test case has been skipped.");
                    break;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Use this to tell if your test case is currently passing or failing.
        /// </summary>
        public bool IsPassing
        {
            get { return _cumulativeResult <= Result.Warning; }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Logs a result of Result.Fail, and writes an exception stack 
        ///     trace.  Call this method if you've caught an exception and
        ///     you want to log it.<para/>
        ///     <para/>
        ///     This method will not abort the test case.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     The exception that you want to report.
        /// </param>
        /// <param name="msg">
        ///     A message you want written to the debug output.
        /// </param>
        /// <param name="args">
        ///     Optional format args.
        /// </param>
        public void LogException(Exception ex, string msg, params object[] args)
        {
            LogResult(Result.Fail, msg, args);

            LogToExternal("------------------------------------------------------------");
            while (ex != null)
            {
                string type = ex.GetType().ToString();

                LogToExternal(ex.Message);
                LogToExternal(type);
                LogToExternal(ex.StackTrace != null ? ex.StackTrace : "   (stack trace not available)");

                if (ex.InnerException != null)
                    LogToExternal("");

                ex = ex.InnerException;
            }

            LogToExternal("------------------------------------------------------------");
            LogToExternal("");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Logs a result of Result.Fail, and writes an exception stack 
        ///     trace.  Call this method if you've caught an exception and
        ///     you want to log it.<para/>
        ///     <para/>
        ///     This method will not abort the test case.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     The exception that you want to report.
        /// </param>
        public void LogException(Exception ex)
        {
            LogException(ex, "This test caught an exception.");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Logs a result for this test case.<para/>
        ///     <para/>
        ///     Do not call this method in your DiagnosticClassInitialize or
        ///     DiagnosticClassFinalize method.
        /// </summary>
        /// 
        /// <param name="result">
        ///     The intermediate result you want to be logged.  If you pass in 
        ///     Result.Ignore, nothing will happen.
        /// </param>
        /// <param name="msg">
        ///     A message you want written to the debug logs.
        /// </param>
        /// <param name="args">
        ///     Optional format args.
        /// </param>
        /// 
        /// <remarks>
        ///     You can call this method as often as you like.  The TestCase
        ///     object will keep track of the final result for you.  For 
        ///     example, if you log a pass, a pass, a fail, and then a pass,
        ///     the final result will be a failure.
        ///     
        ///     This feature was intended for test cases that required a number
        ///     of different but related things be verified.
        ///     
        ///     To log the final result, call EndTest().
        /// </remarks>
        public void LogResult(Result result, string msg, params object[] args)
        {
            if (result > _cumulativeResult)
                _cumulativeResult = result;

            string text = (args.Length > 0) ? string.Format(msg, args) : msg;
            if (result >= Result.Fail && string.IsNullOrEmpty(_firstError))
                _firstError = text;

            string resultText = result.ToString() + ":";
            if (result == Result.Information)
                resultText = "Info:";
            else if (result == Result.Warning)
                resultText = "Warn:";
            else if (result == Result.Fail && _abort)
                resultText = "ABORT:";

            if (result >= Result.Fail)
                LogToExternal(">>>\t" + resultText + "\t" + text);
            else
                LogToExternal("\t" + resultText + "\t" + text);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Set this event with a handler to receive log output and direct 
        ///     it to an external log collector.
        /// </summary>
        public event LogToExternalHandler LogToExternal;


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Dumps informational text to the _testContext.  This text does not
        ///     affect the final result.<para/>
        ///     <para/>
        ///     You may call this method in your DiagnosticClassInitialize or
        ///     DiagnosticClassFinalize method.
        /// </summary>
        /// 
        /// <param name="msg">
        ///     Text you want written to the debug logs.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <remarks>
        ///     Any line-breaks are removed so the output you provide will be 
        ///     properly formatted.
        /// </remarks>
        public void WriteLine(string msg, params object[] args)
        {
            string text = (args.Length > 0) ? string.Format(msg, args) : msg;
            string[] lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
                LogToExternal(lines[i].TrimEnd());
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Dumps a blank line of informational text to the debug logs.  This text does not
        ///     affect the final result.<para/>
        ///     <para/>
        ///     You may call this method in your DiagnosticClassInitialize or
        ///     DiagnosticClassFinalize method.
        /// </summary>
        /// 
        /// <param name="text">
        ///     Text you want written to the debug logs.
        /// </param>
        public void WriteLine()
        {
            LogToExternal("");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Prints a banner in the test case.  This is useful for long test cases that have
        ///     a lot of intermediate stages.<para/>
        ///     <para/>
        ///     You may call this method in your DiagnosticClassInitialize or
        ///     DiagnosticClassFinalize method.
        /// </summary>
        /// 
        /// <param name="bannerText">
        ///     A short banner message.  Keep it under 40 chars.
        /// </param>
        public void WriteSectionBreak(string msg, params object[] args)
        {
            string text = (args.Length > 0) ? string.Format(msg, args) : msg;
            text = "********* " + text + " ";
            text = text.PadRight(80, '*');

            LogToExternal(text);
        }

        #endregion
    }
}
