using System;

namespace FeralNerd.FLog
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Defines a delegate for receiving log data.
    /// </summary>
    /// 
    /// <param name="message">
    ///     A single line of log output.
    /// </param>
    public delegate void NewLogMessageEventHandler(string message);


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is a class for logging to a delegate.  It handles ALL delegate
    ///     references.  Thread synchronization is guaranteed through the Logger
    ///     class, itself (when Logger locks _outputHandlers).
    /// </summary>
    internal class DelegateLoggerHandler : LoggerHandler
    {
        internal static readonly DelegateLoggerHandler Singleton = new DelegateLoggerHandler();

        private DelegateLoggerHandler() : base() { }

        #region ### Internal Methods ###########################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Used to add and remove a delegate object from this handler.
        /// </summary>
        public event NewLogMessageEventHandler OnLogMessage = null;

        #endregion


        #region ### LoggerHandler Overrides ####################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes data to the delegate instance.  This method is thread-safe.
        /// </summary>
        /// 
        /// <param name="message">
        ///     The text to be written
        /// </param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            if (OnLogMessage == null)
                return;

            OnLogMessage(message);
        }

        #endregion
    }
}
