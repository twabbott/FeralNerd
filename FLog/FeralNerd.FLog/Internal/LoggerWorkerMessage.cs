namespace FeralNerd.FLog
{
    internal enum WorkerMessageType
    {
        Text,
        SetColor,
        Flush
    }

    ////////////////////////////////////////////////////////
    /// <summary>
    ///     Base class for all worker thread messages.
    /// </summary>
    internal class LoggerWorkerMessage
    {
        public LoggerWorkerMessage(WorkerMessageType type)
        {
            MessageType = type;
        }

        public WorkerMessageType MessageType;
    }


    ////////////////////////////////////////////////////////
    /// <summary>
    ///     Stores the timestamp and message text for a single line written to
    ///     a Logger instance.
    /// </summary>
    internal class TextWorkerMessage : LoggerWorkerMessage
    {
        public TextWorkerMessage(LoggerFormatInfo format, string time, string message) :
            base(WorkerMessageType.Text)
        {
            Time = time;
            Text = message;
            Format = format;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The line of text that was logged.
        /// </summary>
        public string Text;


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The time that this line of text was logged.
        /// </summary>
        public string Time;


        public LoggerFormatInfo Format;
    }


    ////////////////////////////////////////////////////////
    /// <summary>
    ///     Stores color-change information.
    /// </summary>
    internal class SetFormatMessage : LoggerWorkerMessage
    {
        public SetFormatMessage(int foreColor, int backColor) :
            base(WorkerMessageType.SetColor)
        {
            Format = new LoggerFormatInfo(foreColor, backColor);
        }

        public LoggerFormatInfo Format;
    }


    ////////////////////////////////////////////////////////
    /// <summary>
    ///     A delegate that gets called when the flush is complete
    /// </summary>
    internal delegate void LoggerFlushCompleteEventHandler();


    ////////////////////////////////////////////////////////
    /// <summary>
    ///     A message class indicating that a client wants the queue to be
    ///     flushed.
    /// </summary>
    internal class FlushWorkerMessage : LoggerWorkerMessage
    {
        internal LoggerFlushCompleteEventHandler FlushComplete = null;

        public FlushWorkerMessage(LoggerFlushCompleteEventHandler flushCompleteDelegate) :
            base(WorkerMessageType.Flush)
        {
            FlushComplete = flushCompleteDelegate;
        }
    }
}
