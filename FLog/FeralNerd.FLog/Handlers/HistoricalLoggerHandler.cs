using System;
using System.Collections.Generic;
using System.Linq;

namespace FeralNerd.FLog
{
    public class HistoryLoggerHandlerItem
    {
        public readonly LoggerFormatInfo Format;
        public readonly string Message;

        internal HistoryLoggerHandlerItem(LoggerFormatInfo fmt, string msg)
        {
            Format = fmt;
            Message = msg;
        }
    }

    public class HistoricalLoggerHandler : LoggerHandler
    {
        List<HistoryLoggerHandlerItem> _outputQueue = new List<HistoryLoggerHandlerItem>();
        List<Exception> _exceptionQueue = new List<Exception>();

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes the message to the file and to the screen.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            lock (_outputQueue)
            {
                _outputQueue.Add(new HistoryLoggerHandlerItem(format, message));
                if (_outputQueue.Count > MaxHistory)
                    _outputQueue.RemoveRange(0, _outputQueue.Count - MaxHistory);
            }
        }


        #region ### PUBLIC #####################################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Creates a new HistoryLoggerHandler.
        /// </summary>
        /// 
        /// <param name="maxHistory">
        ///     The maximum number of lines to preserve.  This handler is only 
        ///     meant to preserve a small amount of history.  Large values for
        ///     this parameter (more than 10,000) can degrade performance in the
        ///     Logger.
        /// </param>
        public HistoricalLoggerHandler(int maxHistory = 2500)
        {
            MaxHistory = maxHistory;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Clears the history buffer.
        /// </summary>
        public void Clear()
        {
            lock (_outputQueue)
                _outputQueue.Clear();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns the amount of items in the historical buffer.
        /// </summary>
        public int Count
        {
            get
            {
                int count;
                lock (_outputQueue)
                    count = _outputQueue.Count;

                return count;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the stored history.  Use this method to fetch the N most 
        ///     recent lines of output.
        /// </summary>
        /// 
        /// <param name="count">
        ///     Amount of lines you want returned.
        /// </param>
        /// 
        /// <returns>
        ///     The N most recent lines of output.
        /// </returns>
        public List<string> GetHistory(int count = int.MaxValue, string filter = null)
        {
            List<string> copy = new List<string>();

            lock (_outputQueue)
            {
                // Can't remove more than we have in the queue.
                if (count > _outputQueue.Count)
                    count = _outputQueue.Count();

                for (int i = _outputQueue.Count - count; i < _outputQueue.Count; i++)
                {
                    if (filter != null && !_outputQueue[i].Message.Contains(filter))
                        continue;

                    copy.Add(_outputQueue[i].Message);
                }
            }

            return copy;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the stored history.  Use this method to fetch the N most 
        ///     recent lines of output, with formatting information.
        /// </summary>
        /// 
        /// <param name="count">
        ///     Amount of lines you want returned.
        /// </param>
        /// 
        /// <returns>
        ///     The N most recent lines of output.
        /// </returns>
        public List<HistoryLoggerHandlerItem> GetHistoryWithFormatting(int count = int.MaxValue)
        {
            List<HistoryLoggerHandlerItem> copy = new List<HistoryLoggerHandlerItem>();

            lock (_outputQueue)
            {
                // Can't remove more than we have in the queue.
                if (count > _outputQueue.Count)
                    count = _outputQueue.Count();

                for (int i = _outputQueue.Count - count; i < _outputQueue.Count; i++)
                    copy.Add(_outputQueue[i]);
            }

            return copy;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The maximum number of lines stored.
        /// </summary>
        public readonly int MaxHistory;

        #endregion
    }
}
