using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

/********************************************************************
 * S a m p l e   C o d e
 ********************************************************************
 
    using EsnLibrary.Common.Diagnostics;

    static void Main(string[] args)
    {
        // Create a new instance.  The constructor has various overloads,
        // but this is the most basic.
        Logger logger = new Logger();

        // Now you tell it where you want your output sent.  This overload
        // opens up a file.  If you like, you can also declare your own 
        // LogHandler class, just like with LogHelper.
        logger.AddHandler("TestLogFile.txt");

        // This will echo all output to the console.  Handy if you're running
        // in a console window.
        logger.EnableConsoleOutput = true;

        // This will echo all output to Visual Studio's debug output window.
        // It's handy if you're debugging in VS
        logger.EnableDebugOutput = true;

        // Now you can do stuff
        try
        {
            for (int i = 0; i < 10; i++)
            {
                // Just call WriteLine.  There are a bunch of other methods,
                // but this is the one you'll use 99% of the time.
                logger.WriteLine("This is pass {0}", i);
                Thread.Sleep(1000);
            }
        }
        catch (Exception ex)
        {
            // This will walk through all inner exceptions, and give a 
            // stack dump of each.
            logger.WriteException(ex, "Something bad happened!");
        }
        finally
        {
            // This method is guaranteed to block until the background worker has
            // written the last line of output from your thread.
            logger.Flush();

            // Shut it down when you're done.  This tells the logger to close
            // all files, and then shuts down the background worker thread.
            logger.Close();
        }
    }

*********************************************************************/

namespace FeralNerd.FLog
{
    /// <summary>
    ///     Relays log messages to output handlers
    /// </summary>
#pragma warning disable 618
    public class Logger: IDisposable
    {
        public const int MaxHandles = 64;
        public const string DefaultTimestampFormat = @"{0:yyyy/MM/dd HH:mm:ss.fff}";

        #region ### Private Fields #############################################

        // List of output handlers
        private List<LoggerHandler> _outputHandlers = new List<LoggerHandler>();

        // A Win32 event used to signal the worker thread when output is ready.
        private AutoResetEvent _dataReadyEvent = new AutoResetEvent(false);

        // A Win32 event used to signal the worker thread to shut down
        private AutoResetEvent _quitNowEvent = new AutoResetEvent(false);

        // A Win32 event used to tell all callers that the worker thread has quit.
        private ManualResetEvent _workerDoneEvent = new ManualResetEvent(false);
        private ManualResetEvent _resumeFromSuspendEvent = new ManualResetEvent(true);
        private int _isSuspended = 0;

        // Internal queues used by the worker thread
        private Queue<LoggerWorkerMessage> _inputList = new Queue<LoggerWorkerMessage>();
        private Queue<LoggerWorkerMessage> _workingList = new Queue<LoggerWorkerMessage>();

        // Various logger state variables.
        private bool _consoleOutput = false;
        private bool _vsOutput = false;
        private bool _enableTimestamp = true;

        private int _foreColor = LoggerColors.NoChange;
        private int _backColor = LoggerColors.NoChange;

        private Thread LogWorkerThread;

        private void __CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Flush();
        }

        #endregion
        

        #region ### Constructors ###############################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Initializes the LogWorkerThread
        /// </summary>
        public Logger()
        {
            TimestampFormat = DefaultTimestampFormat;

            // Start out with the delegate logger enabled
            _outputHandlers.Add(DelegateLoggerHandler.Singleton);

            LogWorkerThread = new Thread(new ThreadStart(__WorkerThreadMain));
            LogWorkerThread.IsBackground = true;
            LogWorkerThread.Name = "Logger WorkerThread";
            LogWorkerThread.Start();

            // Set up a handler so we can catch all un-handled exceptions and flush
            // the logs before shutting down.
            AppDomain.CurrentDomain.UnhandledException += __CurrentDomain_UnhandledException;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Initializes the LogWorkerThread and
        ///     adds a FileLoggerHandler to the collection
        ///     of ouput handlers.
        /// </summary>
        /// <param name="fileName">
        ///     path of file to write to
        /// </param>
        public Logger(string fileName)
            : this()
        {
            AddHandler(new FileLoggerHandler(fileName));
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Initializes the LogWorkerThread and
        ///     adds a ConsoleLoggerHandler to the 
        ///     collection of ouput handlers.
        /// </summary>
        /// <param name="echoToConsole">
        ///     adds a ConsoleLogger if true
        /// </param>
        public Logger(bool echoToConsole)
            : this()
        {
            EnableConsoleOutput = echoToConsole;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Initializes the LogWorkerThread,
        ///     adds a FileLoggerHandler, and 
        ///     ConsoleLoggerHandler to the 
        ///     collection of ouput handlers.
        /// </summary>
        /// <param name="fileName">
        ///     path of file to write to
        /// </param>
        /// <param name="echoToConsole">
        ///     adds a console logger if true
        /// </param>
        public Logger(string fileName, bool echoToConsole)
            : this()
        {
            AddHandler(new FileLoggerHandler(fileName));

            EnableConsoleOutput = echoToConsole;
        }

        #endregion


        #region ### Worker Thread ##############################################

        /// <summary>
        ///     Worker thread for the logger.  The worker thread distributes 
        ///     incoming messages to all subscribed output handlers once a 
        ///     message is recieved. 
        /// </summary>
        private void __WorkerThreadMain()
        {
            bool quit = false;
            StringBuilder sbTime = new StringBuilder();
            int truncate;

            WaitHandle[] handles = new WaitHandle[]
            {
                _dataReadyEvent,
                _quitNowEvent
            };

            while (!quit)
            {
                int handleIdx = WaitHandle.WaitAny(handles);

                try
                {
                    switch (handleIdx)
                    {
                        case 0:
                            //  Data is ready
                            lock (this)//list of strings
                            {
                                Queue<LoggerWorkerMessage> tmp = _inputList;
                                _inputList = _workingList;
                                _workingList = tmp;
                            }

                            //   Spew each output message
                            while (_workingList.Count > 0)
                            {
                                LoggerWorkerMessage message = _workingList.Dequeue();

                                try
                                {
                                    switch (message.MessageType)
                                    {
                                        case WorkerMessageType.Text:
                                            TextWorkerMessage textmsg = message as TextWorkerMessage;
                                            lock (_outputHandlers)
                                            {

                                                sbTime.Clear();
                                                sbTime.Append(textmsg.Time);
                                                sbTime.Append(' ');
                                                truncate = sbTime.Length;

                                                string[] lines = textmsg.Text.Split('\n');
                                                foreach (string line in lines)
                                                {
                                                    sbTime.Length = truncate;
                                                    sbTime.Append(line.TrimEnd());

                                                    foreach (LoggerHandler handler in _outputHandlers)
                                                    {
                                                        try
                                                        {
                                                            if (handler.ExceptionCount < 5)
                                                            {
                                                                if (handler.EnableTimeStamp)
                                                                    handler.Write(textmsg.Format, sbTime.ToString());
                                                                else
                                                                    handler.Write(textmsg.Format, line.TrimEnd());

                                                                handler.ExceptionCount = 0;
                                                            }
                                                        }
                                                        catch (Exception ex)
                                                        {
                                                            handler.ExceptionCount++;
                                                            if (handler.ExceptionCount >= 5)
                                                                WriteLine("Logger {0} has thrown 5 consecutive exceptions, and will be disabled!", handler.GetType());
                                                            else if (handler.ExceptionCount == 1)
                                                                WriteException(ex, "Logger {0} threw an exception!", handler.GetType());
                                                        }
                                                    }
                                                }

                                            }
                                            break;

                                        case WorkerMessageType.SetColor:
                                            SetFormatMessage colorMsg = message as SetFormatMessage;
                                            lock (_outputHandlers)
                                            {
                                                foreach (LoggerHandler handler in _outputHandlers)
                                                    handler.SetFormat(colorMsg.Format);
                                            }
                                            break;

                                        case WorkerMessageType.Flush:
                                            // Manually flush all streams.
                                            lock (_outputHandlers)
                                            {
                                                foreach (LoggerHandler handler in _outputHandlers)
                                                    handler.Flush();
                                            }

                                            // Now tell whoever called us that we did it.
                                            FlushWorkerMessage flushMsg = message as FlushWorkerMessage;
                                            flushMsg.FlushComplete();
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("Exception in Logger worker thread processing {0} message.", message.MessageType);
                                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                                }
                            }
                            break;

                        case 1:
                            // Time to shut down
                            quit = true;
                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    // If there is a shut-down, or if somehow someone aborts this thread,
                    // clean up gracefully.
                    Thread.ResetAbort();
                    quit = true;
                }
            }

            // Flush all streams before giving up the ghost.  This won't dump the data in the
            // input queue, but it will make sure all file buffers are written.
            try
            {
                lock (_outputHandlers)
                {
                    foreach (LoggerHandler handler in _outputHandlers)
                        handler.Flush();
                }
            }
            catch //(Exception ex)
            {
                // It's really time to go.  Let's just go...
            }

            _workerDoneEvent.Set();
        }

        #endregion


        #region ### Public Methods #############################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a Handler to the Collection of output Handlers
        /// </summary>
        /// 
        /// <param name="handler">
        ///     ILogOutputHandler to be added
        /// </param>
        public void AddHandler(LoggerHandler handler)
        {
            // Make sure all pending output up to this point has been written.
            if (!IsSuspended)
                Flush();

            // call the handler's init method
            handler.EnableTimeStamp = _enableTimestamp;
            handler.Initialize();

            // lock the dictionary, and add the handler
            lock (_outputHandlers)
            {
                if ((handler is ConsoleLoggerHandler) && !_outputHandlers.Contains(ConsoleLoggerHandler.Singleton))
                    _outputHandlers.Add(ConsoleLoggerHandler.Singleton);
                else if ((handler is DebugOutputLoggerHandler) && !_outputHandlers.Contains(DebugOutputLoggerHandler.Singleton))
                    _outputHandlers.Add(DebugOutputLoggerHandler.Singleton);
                else
                    _outputHandlers.Add(handler);
            }

            return;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Opens up a log file using a file name that you specify.  
        /// </summary>
        /// 
        /// <param name="logFileName">
        ///     The name of log file.
        /// </param>
        public void AddHandler(string logFileName)
        {
            AddHandler(new FileLoggerHandler(logFileName));
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Updates the background color for all output.  NOTE: Not every handler can
        ///     display color.
        /// </summary>
        public int BackColor
        {
            get { return _backColor; }

            set
            {
                // Lock the logger
                lock (this)
                {
                    // Send a flush message
                    _backColor = value;
                    _inputList.Enqueue(new SetFormatMessage(LoggerColors.NoChange, value));

                    // Set the data-ready event
                    _dataReadyEvent.Set();
                }
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Flushes all output handlers, then terminates the worker thread.  Always
        ///     call this method before your program exits, or you will likely lose
        ///     output.
        /// </summary>
        /// 
        /// <param name="waitTime">
        ///     Amount of time you want to wait for the worker thread to finish dumping
        ///     all data in its queue.  You shouldn't need to change this unless you 
        ///     expect logging to take a very long time.
        /// </param>
        public void Close(int waitTime = 1000)
        {
            Flush();

            //signal the worker thread to close
            _quitNowEvent.Set();

            // wait for the thread to be done
            if (!LogWorkerThread.Join(waitTime))
                LogWorkerThread.Abort();

            // go through all handlers and call their Cleanup method, one by one.
            lock (_outputHandlers)
            {
                foreach (LoggerHandler pair in _outputHandlers)
                {
                    pair.Cleanup();
                }
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Turns console output on and off.
        /// </summary>
        public bool EnableConsoleOutput
        {
            get
            {
                return _consoleOutput;
            }

            set
            {
                if (_consoleOutput == value)
                    return;

                _consoleOutput = value;
                if (_consoleOutput)
                    AddHandler(ConsoleLoggerHandler.Singleton);
                else
                    RemoveHandler(ConsoleLoggerHandler.Singleton);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Turns output to Visual Studio's Output window on and off.
        /// </summary>
        public bool EnableDebugOutput
        {
            get
            {
                return _vsOutput;
            }

            set
            {
                if (_vsOutput == value)
                    return;

                _vsOutput = value;
                if (_vsOutput)
                    AddHandler(DebugOutputLoggerHandler.Singleton);
                else
                    RemoveHandler(DebugOutputLoggerHandler.Singleton);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets or sets whether text is time-stamped by default.  This value affects
        ///     all current loggers, and all loggers added in the future.  You should set
        ///     this property once when you initialize your Logger object.<para/>
        ///     
        ///     If you want a specific handler to have a different behavior, add the 
        ///     handler, then call GetHandler and set the handler's EnableTimeStamp 
        ///     property, explicitly.
        /// </summary>
        public bool EnableTimeStamp
        {
            set
            {
                lock (_outputHandlers)
                {
                    _enableTimestamp = value;
                    foreach (LoggerHandler handler in _outputHandlers)
                    {
                        handler.EnableTimeStamp = _enableTimestamp;
                    }
                }
            }

            get
            {
                return _enableTimestamp;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Causes any temporary buffers in all logger handlers to be written
        ///     and purged.
        /// </summary>
        /// 
        /// <param name="waitTime">
        ///     The amount of time you want to wait for the flush to complete.  This
        ///     ought to be some value greater than zero, or System.Threading.Timeout.Infinite.
        /// </param>
        /// 
        /// <returns>
        ///     A value of true means that all output that the current thread has logged
        ///     so far *has* been written to all output handlers.  A value of false indicates
        ///     that the output has not yet been written.
        /// </returns>
        public bool Flush(int waitTime = 1000)
        {
            // If the worker is dead, then there is literally nothing you can do
            // here.
            if (_workerDoneEvent.WaitOne(0))
                return true;

            if (IsSuspended)
                throw new LoggerException("Can't call Logger.Flush() while the worker thread is suspended.  This will cause a nasty deadlock!");

            // This will get set when the worker calls us back.
            ManualResetEvent done = new ManualResetEvent(false);

            // Here is an anonymous callback method for the worker thread to use.
            LoggerFlushCompleteEventHandler callback = delegate ()
            {
                done.Set();
            };

            // Lock the logger
            lock (this)
            {
                // Send a flush message
                _inputList.Enqueue(new FlushWorkerMessage(callback));

                // Set the data-ready event
                _dataReadyEvent.Set();
            }

            // Now wait for the 
            return done.WaitOne(waitTime);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Updates the foreground color for all output.  NOTE: Not every handler can
        ///     can display color.  
        /// </summary>
        public int ForeColor
        {
            get { return _foreColor; }

            set
            {
                // Lock the logger
                lock (this)
                {
                    // Send a flush message
                    _foreColor = value;
                    _inputList.Enqueue(new SetFormatMessage(value, LoggerColors.NoChange));

                    // Set the data-ready event
                    _dataReadyEvent.Set();
                }
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Used to tell if the worker thread is currently suspended.  To
        ///     resume the worker thread, call Resume().
        /// </summary>
        public bool IsSuspended
        {
            get { return (_isSuspended != 0); }
            private set { Interlocked.Exchange(ref _isSuspended, value ? 1 : 0); }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     This event gets fired whenever there is log output.  Use this to eavesdrop on 
        ///     log output, or to implement a logger handler without using inheritance.
        /// </summary>
        public event NewLogMessageEventHandler OnLogMessage
        {
            add
            {
                lock (_outputHandlers)
                {
                    DelegateLoggerHandler.Singleton.OnLogMessage += value;
                }
            }

            remove
            {
                lock (_outputHandlers)
                {
                    DelegateLoggerHandler.Singleton.OnLogMessage -= value;
                }
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Removes a handler.  NOTE: If you remove a handler, you are responsible
        ///     for cleaning it up.
        /// </summary>
        /// 
        /// <param name="name">
        ///     Name of the handler you want to remove.
        /// </param>
        /// 
        /// <returns>
        ///     Returns the LoggerHandler you specified, or a null if no handler exists
        ///     for the name you specified.
        /// </returns>
        public void RemoveHandler(LoggerHandler handler)
        {
            // lock the list and remove the handler
            lock (_outputHandlers)
            {
                if (_outputHandlers.Contains(handler))
                    _outputHandlers.Remove(handler);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Tells the worker thread to resume activity.  Call this method
        ///     after you have called Suspend().
        /// </summary>
        public void Resume()
        {
            // Signal the event.  The worker thread will then wake up
            // and continue.
            _resumeFromSuspendEvent.Set();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Synchronously suspends the logger worker thread.  This method
        ///     flushes all output up to this point in time, and then suspends
        ///     the worker thread (by waiting on an internal EventWaitHandle).<para/>
        ///     <para/>
        ///     To resume output, call Resume().
        /// </summary>
        public void Suepend()
        {
            // If the worker is dead, then there is literally nothing you can do
            // here.
            if (_workerDoneEvent.WaitOne(0))
                return;

            // If we're already suspended, do NOT continue.  It will cause a deadlock.
            if (IsSuspended)
                return;

            // Set the pause event to un-signalled
            _resumeFromSuspendEvent.Reset();

            // This will get set when the worker calls us back.
            ManualResetEvent done = new ManualResetEvent(false);

            // Here is an anonymous callback method for the worker thread to use.
            LoggerFlushCompleteEventHandler callback = delegate ()
            {
                // Tell the calling thread that we've flushed the queue
                done.Set();

                // Now wait for the calling thread to tell us to go once more.
                IsSuspended = true;
                _resumeFromSuspendEvent.WaitOne();
                IsSuspended = false;
            };

            // Lock the logger
            lock (this)
            {
                // Send a flush message
                _inputList.Enqueue(new FlushWorkerMessage(callback));

                // Tell the worker thred to come 'n get it.
                _dataReadyEvent.Set();
            }

            // Now wait for the worker thread to signal that it
            // is entering suspend mode.
            done.WaitOne();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     You can set this property to change the way the timestamp is 
        ///     formatted
        /// </summary>
        public string TimestampFormat { get; set; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes a banner to the Logger
        /// </summary>
        /// 
        /// <param name="msg">
        ///     The banner message.
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        public void WriteBanner(string msg, params object[] args)
        {
            WriteBanner(null, msg, args);
        }


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Writes a banner to the Logger
        /// </summary>
        /// 
        /// <param name="msg">
        ///     The banner message.
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        internal void WriteBanner(LoggerFormatInfo info, string msg, params object[] args)
        {
            WriteLine(info, "################################################################################");
            WriteLine(info, "### " + string.Format(msg, args));
            WriteLine(info, "################################################################################");
            WriteLine(info, "");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes an exception with full stack trace to the logger.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     The exception you want to log.
        /// </param>
        public void WriteException(Exception ex)
        {
            WriteException(null, ex, null);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes an exception plus an error message, with full stack trace,
        ///     to the Logger.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void WriteException(Exception ex, string msg, params object[] args)
        {
            WriteException(null, ex, msg, args);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes an exception plus an error message, with full stack trace,
        ///     to the Logger.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        /// <param name="args"></param>
        public void WriteException(LoggerFormatInfo info, Exception ex, string msg, params object[] args)
        {
            string text = (args.Length == 0) ? msg : string.Format(msg, args);

            WriteLine(info, "********************************************************************************");

            if (!string.IsNullOrEmpty(text))
            {
                WriteLine(text);
            }

            while (ex != null)
            {
                WriteLine(info, "");
                WriteLine(info, ex.GetType().ToString());
                WriteLine(info, ex.Message);
                WriteLine(info, ex.StackTrace);

                ex = ex.InnerException;
            }

            WriteLine(info, "********************************************************************************");
            WriteLine(info, "");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes an exception in short form to the logger.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     The exception to log.
        /// </param>
        public void WriteExceptionShort(Exception ex)
        {
            WriteExceptionShort(null, ex, null);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes an exception in short form, plus a message to the logger.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     The exception to log.
        /// </param>
        /// <param name="format">
        ///     A message you want displayed with the exception.
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        public void WriteExceptionShort(Exception ex, string format, params object[] args)
        {
            WriteExceptionShort(null, ex, format, args);
        }


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Writes an exception in short form, plus a message to the logger.
        /// </summary>
        /// 
        /// <param name="handle">
        ///     LoggerChannel that this exception was logged from.
        /// </param>
        /// <param name="ex">
        ///     The exception to log.
        /// </param>
        /// <param name="format">
        ///     A message you want displayed with the exception.
        /// </param>
        /// <param name="args">
        ///     Format args.
        /// </param>
        public void WriteExceptionShort(LoggerFormatInfo info, Exception ex, string format, params object[] args)
        {
            string text = (args.Length == 0) ? format : string.Format(format, args);

            if (!string.IsNullOrEmpty(text))
            {
                WriteLine(info, text);
            }

            while (ex != null)
            {
                WriteLine(info, "  - " + ex.Message);
                ex = ex.InnerException;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes a blank line to all handlers.
        /// </summary>
        public void WriteLine()
        {
            WriteLine(null, "");
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes text to all handlers, using the current format.
        /// </summary>
        /// 
        /// <param name="message">
        ///     The text to be written.
        /// </param>
        /// <param name="args">
        ///     Arguements for the message.
        /// </param>
        public void WriteLine(string text, params object[] args)
        {
            WriteLine(null, text, args);
        }


        /////// INTERNAL ///////////////////////////////////
        /// <summary>
        ///     Writes text to all handlers, using a given color for this message only.
        ///     To change the foreground/background color for all messages, use the
        ///     ForeColor and BackColor properties.
        /// </summary>
        /// 
        /// <param name="handle">
        ///     LoggerChannel that logged this text.
        /// </param>
        /// <param name="format">
        ///     Format to use for this line only.
        /// </param>
        /// <param name="message">
        ///     The text to be written.
        /// </param>
        /// <param name="args">
        ///     Arguements for the message.
        /// </param>
        public void WriteLine(LoggerFormatInfo format, string text, params object[] args)
        {
            if (_workerDoneEvent.WaitOne(0))
                return;

            // Compose the string
            string tempString;
            if (args.Length == 0)
                tempString = text;
            else
                tempString = string.Format(text, args);

            string timeStamp = "";
            if (!string.IsNullOrWhiteSpace(TimestampFormat))
                timeStamp = string.Format(TimestampFormat, DateTime.Now);

            TextWorkerMessage message = new TextWorkerMessage(
                format,
                timeStamp,
                tempString);

            // Lock the logger
            lock (this)
            {
                _inputList.Enqueue(message);

                // Set the data-ready event
                _dataReadyEvent.Set();
            }
        }


        //// IDisposable ///////////////////////////////////
        /// <summary>
        ///     This calls the Close() method for you.
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        #endregion
    }
}
