using System.IO;

namespace FeralNerd.FLog
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is a class for logging to a File.  It does not cycle when the
    ///     file becomes large, and it is not meant for use with long-running
    ///     services.
    /// </summary>
    public class FileLoggerHandler : LoggerHandler
    {
        #region ### Private Fields #############################################

        private string filePath;
        private StreamWriter outfile;

        #endregion


        #region ### Constructors ###############################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Initialize variables
        /// </summary>
        /// <param name="filepath">Path for a file to be placed</param>
        public FileLoggerHandler(string filepath)
        {
            this.filePath = filepath;
        }

        #endregion


        #region ### LoggerHandler Overrides ####################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Close the output file
        /// </summary>
        public override void Cleanup()
        {
            outfile.Close();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Flushes the file's output buffers.
        /// </summary>
        public override void Flush()
        {
            outfile.Flush();
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Opens the output file.
        /// </summary>
        public override void Initialize()
        {
            string dir = Path.GetDirectoryName(filePath);

            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            outfile = new StreamWriter(filePath, false);
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Writes the message to the file and to the screen.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(LoggerFormatInfo format, string message)
        {
            outfile.WriteLine(message);
        }

        #endregion
    }
}
