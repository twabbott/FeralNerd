using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FeralNerd.FLog
{
    public enum ServiceLogFileSize : long
    {
        OneMegabyte = 1048576,
        TwoMegabytes = 2097152,
        FourMegabytes = 4194304,
        EightMegabytes = 8388608,
        SixteenMegabytes = 16777216
    }

    public enum ServiceLogFileSpace : long
    {
        OneGigabyte = 1073741824,
        TwoGigabytes = 2147483648
    }

    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A class for managing log-files for services that are meant to run
    ///     for a very long time.  Files are broken up into 8MB chunks, and are
    ///     cycled after 12:00am every night.  You can also control how many 
    ///     days' worth of log files you want kept locally, and you can limit 
    ///     how much total space the log files can consume.
    /// </summary>
    public class ServiceLogFileLoggerHandler : LoggerHandler
    {
        private static readonly TimeSpan _fiveMinutes = new TimeSpan(0, 5, 0);
        private FileStream _outStream = null;
        private StreamWriter _outWriter = null;
        private DateTime _nextRetry = default(DateTime);
        private DateTime _afterMidnight;

        #region ### PUBLIC #####################################################

        public string CurrentFilename { get; private set; }
        public long FileSize { get; private set; }
        public long ReservedSpace { get; private set; }
        public string ServiceName { get; private set; }
        public string OutputDirectory { get; private set; }
        public Encoding Encoding { get; private set; }

        public ServiceLogFileLoggerHandler(string ServiceName) :
            this(@"..\LogFiles\" + ServiceName, ServiceName, (long)ServiceLogFileSize.EightMegabytes, (long)ServiceLogFileSpace.OneGigabyte, Encoding.ASCII)
        {
        }

        public ServiceLogFileLoggerHandler(string directory, string serviceName) :
            this(directory, serviceName, (long)ServiceLogFileSize.EightMegabytes, (long)ServiceLogFileSpace.OneGigabyte, Encoding.ASCII)
        {
        }

        public ServiceLogFileLoggerHandler(string directory, string serviceName, long size, long reservedSpace) :
            this(directory, serviceName, size, reservedSpace, Encoding.ASCII)
        {
        }

        public ServiceLogFileLoggerHandler(string directory, string serviceName, long size, long reservedSpace, Encoding encoding)
        {
            FileSize = size;
            ReservedSpace = reservedSpace;
            ServiceName = serviceName;

            // Make sure that if we're running as a service, 
            if (Path.IsPathRooted(directory))
                OutputDirectory = directory;
            else
                OutputDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), directory);

            Encoding = encoding;

            if (ReservedSpace < FileSize)
                throw new LoggerException("Size must be less than the reserved space.");
        }


        /// <summary>
        ///     Erases all log files from the output directory, except for the 
        ///     current log file.
        /// </summary>
        public void ClearAllLogs()
        {
            // Scan the output directory and get all log files
            string[] filenameList = Directory.GetFiles(OutputDirectory, "*.log", SearchOption.TopDirectoryOnly);
            foreach (string filename in filenameList)
            {
                if (filename == CurrentFilename)
                    continue;

                File.Delete(filename);
            }
        }

        #endregion


        #region ### PRIVATE ####################################################

        private void __CycleLogFile()
        {
            // If anything here bombs, retry again in five minutes.  Yes, this means we could
            // lose log data, but it also means that logging will resume as soon as the problem
            // is corrected.
            _nextRetry = DateTime.Now + _fiveMinutes;

            // Close the current log file
            if (_outWriter != null)
                Cleanup();

            // Scan the output directory and get all log files
            List<string> filenameList = Directory.GetFiles(OutputDirectory, "*.log", SearchOption.TopDirectoryOnly).ToList();

            long totalSize = 0;
            List<FileInfo> infoList = new List<FileInfo>();
            foreach (string file in filenameList)
            {
                FileInfo fi = new FileInfo(file);
                totalSize += fi.Length;
                infoList.Add(fi);
            }

            // Start deleting older files until we have enough space to
            // create a new log file.
            while (filenameList.Count > 0 && totalSize + FileSize > ReservedSpace)
            {
                // If this throws an exception, then this handler will become deactivated.
                File.Delete(filenameList[0]);
                totalSize -= infoList[0].Length;

                filenameList.RemoveAt(0);
                infoList.RemoveAt(0);
            }

            // Find a new file name that is guaranteed not to already exist
            CurrentFilename = Path.Combine(OutputDirectory, string.Format("{0} - {1:yyyy-MM-dd HH-mm-ss.fff}.log", ServiceName, DateTime.Now));
            for (int i = 2; File.Exists(CurrentFilename); i++)
            {
                CurrentFilename = Path.Combine(OutputDirectory, string.Format("{0} - {1:yyyy-MM-dd HH-mm-ss.fff} ({2:D3}).log", ServiceName, DateTime.Now, i));
            }

            // Open the file, using the designated encoding
            _outStream = new FileStream(CurrentFilename, FileMode.Create);
            _outWriter = new StreamWriter(_outStream, Encoding);
            _afterMidnight = DateTime.Today + new TimeSpan(24, 0, 0);
        }


        private long __GetUsedSpace()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region ### LoggerHandler ##############################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     This method gets called when the handler is removed from the 
        ///     logger.  It allows the handler to do any cleanup work, like closing 
        ///     a file, etc.
        /// </summary>
        public override void Cleanup()
        {
            if (_outWriter != null)
            {
                _outStream.Flush();

                _outWriter.Close();
                _outWriter.Dispose();
                _outWriter = null;

                _outStream.Close();
                _outStream.Dispose();
                _outStream = null;

                CurrentFilename = null;
                _nextRetry = default(DateTime);
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Flushes all output buffers.
        /// </summary>
        public override void Flush()
        {
            if (_outWriter == null)
                return;

            _outWriter.Flush();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     This method gets called when the handler is added to the logger.
        ///     It allows the handler to initialize any internal state, like 
        ///     opening a file, etc.
        /// </summary>
        public override void Initialize()
        {
            if (!Directory.Exists(OutputDirectory))
                Directory.CreateDirectory(OutputDirectory);

            __CycleLogFile();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     This method gets called by the worker thread every time there
        ///     is output to be written.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            if (_outWriter == null && DateTime.Now > _nextRetry)
                __CycleLogFile();

            // This isn't super-accurate, but as far as calculating the remaining
            // space in the file but it's close enough.
            if (message.Length + _outStream.Position > FileSize || DateTime.Now > _afterMidnight)
                __CycleLogFile();

            if (_outWriter != null)
                _outWriter.WriteLine(message);
        }

        #endregion
    }
}
