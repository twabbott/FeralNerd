using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FeralNerd.FLog
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A class that keeps track of an exception and the time when it 
    ///     occurred.
    /// </summary>
    public class LoggerExceptionHistoryItem
    {
        ////////////////////////////////////////////////////
        /// <summary>
        ///     A message that the application logged along with the exception 
        ///     that was caught.
        /// </summary>
        public readonly string Message;


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The exception that was logged.
        /// </summary>
        public readonly Exception Exception;


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The time that the exception was logged.
        /// </summary>
        public readonly DateTime Timestamp = DateTime.Now;


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="ex"></param>
        internal LoggerExceptionHistoryItem(Exception ex, string message)
        {
            Message = message;
            Exception = ex;
        }
    }


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A class that keeps track of all exceptions that get logged via
    ///     the Logger.WriteException method.
    /// </summary>
    public class LoggerExceptionHistory
    {
        private Queue<LoggerExceptionHistoryItem> _queue = new Queue<LoggerExceptionHistoryItem>();
        private int _count = 0;


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Maximum items allowed in the history.
        /// </summary>
        public readonly int MaxItems;


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="maxItems"></param>
        internal LoggerExceptionHistory(int maxItems = 1000)
        {
            MaxItems = maxItems;
        }


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Inserts an exception.
        /// </summary>
        internal void AddItem(Exception ex, string message)
        {
            LoggerExceptionHistoryItem item = new LoggerExceptionHistoryItem(ex, message);

            lock (_queue)
            {
                _queue.Enqueue(item);
                while (_queue.Count > MaxItems)
                    _queue.Dequeue();

                Interlocked.Exchange(ref _count, _queue.Count());
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Clears the exception history.
        /// </summary>
        public void Clear()
        {
            lock (_queue)
            {
                _queue.Clear();
                Interlocked.Exchange(ref _count, 0);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Returns a count of queued exceptions.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets a copy of the current exception history.
        /// </summary>
        public List<LoggerExceptionHistoryItem> GetHistory()
        {
            List<LoggerExceptionHistoryItem> list = new List<LoggerExceptionHistoryItem>();

            lock (_queue)
            {
                foreach (LoggerExceptionHistoryItem item in _queue)
                    list.Add(item);
            }

            return list;
        }
    }
}
