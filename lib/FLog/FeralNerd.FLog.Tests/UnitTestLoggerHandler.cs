using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeralNerd.FLog;

namespace FeralNerd.FLog.Tests
{
    class UnitTestLoggerHandler: LoggerHandler
    {
        private List<string> _lines = new List<string>();

        public string GetLines()
        {
            return string.Join("\r\n", _lines);
        }

        #region ### LoggerHandler Overrides ####################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes data to the test logger
        /// </summary>
        /// 
        /// <param name="message">
        ///     The text to be written
        /// </param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            _lines.Add(message);
        }

        #endregion
    }
}
