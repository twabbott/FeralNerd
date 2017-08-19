
namespace FeralNerd.FLog
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is a class for logging to the Debug output
    /// </summary>
    internal class DebugOutputLoggerHandler : LoggerHandler
    {
        internal static readonly DebugOutputLoggerHandler Singleton = new DebugOutputLoggerHandler();

        private DebugOutputLoggerHandler() : base() { }

        #region ### LoggerHandler Overrides ####################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes data to the console
        /// </summary>
        /// 
        /// <param name="message">
        ///     The text to be written
        /// </param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        #endregion
    }
}
