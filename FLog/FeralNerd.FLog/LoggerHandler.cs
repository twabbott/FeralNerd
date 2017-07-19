using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FeralNerd.FLog
{
    public class LoggerColors
    {
        internal const int ControlMask = 0x7F000000;

        public const int NoChange = int.MinValue;
        public const int Black = 0x00000000;
        public const int Blue = 0x000000FF;
        public const int Brown = 0x007F7F00;
        public const int Cyan = 0x0000FFFF;
        public const int DarkBlue = 0x0000007F;
        public const int DarkGray = 0x003F3F3F;
        public const int DarkGreen = 0x00007F00;
        public const int DarkCyan = 0x00007F7F;
        public const int DarkRed = 0x007F0000;
        public const int DarkMagenta = 0x007F007F;
        public const int Green = 0x0000FF00;
        public const int Gray = 0X007F7F7F;
        public const int Magenta = 0x00FF00FF;
        public const int Red = 0x00FF0000;
        public const int White = 0x00FFFFFF;
        public const int Yellow = 0x00FFFF00;
    }

    /// <summary>
    ///     A class containing format info 
    /// </summary>
    public class LoggerFormatInfo
    {
        public static readonly LoggerFormatInfo UnformattedText = new LoggerFormatInfo(LoggerColors.NoChange, LoggerColors.NoChange, -1, -1, -1);
        public static readonly LoggerFormatInfo BoldText = new LoggerFormatInfo(LoggerColors.NoChange, LoggerColors.NoChange, 1, -1, -1);
        public static readonly LoggerFormatInfo ItalicsText = new LoggerFormatInfo(LoggerColors.NoChange, LoggerColors.NoChange, -1, 1, -1);
        public static readonly LoggerFormatInfo UnderlineText = new LoggerFormatInfo(LoggerColors.NoChange, LoggerColors.NoChange, -1, -1, 1);

        public readonly int Bold;
        public readonly int Italics;
        public readonly int Underline;
        public readonly string StyleName;

        public readonly int ForegroundColor;
        public readonly int BackgroundColor;

        public LoggerFormatInfo(
            int foregroundColor = LoggerColors.NoChange,
            int backgroundColor = LoggerColors.NoChange,
            int bold = -1,
            int italics = -1,
            int underline = -1)
        {
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
            Bold = bold;
            Italics = italics;
            Underline = underline;
        }

        public LoggerFormatInfo Clone()
        {
            return new LoggerFormatInfo(
                ForegroundColor,
                BackgroundColor,
                Bold,
                Italics,
                Underline);
        }

        public LoggerFormatInfo CloneAs(
            int foregroundColor = LoggerColors.NoChange,
            int backgroundColor = LoggerColors.NoChange,
            int bold = -1,
            int italics = -1,
            int underline = -1)
        {
            return new LoggerFormatInfo(
                (foregroundColor == LoggerColors.NoChange) ? ForegroundColor : foregroundColor,
                (backgroundColor == LoggerColors.NoChange) ? BackgroundColor : backgroundColor,
                (bold == -1) ? Bold : bold,
                (italics == -1) ? Italics : italics,
                (underline == -1) ? Underline : underline);
        }

        public static LoggerFormatInfo operator +(LoggerFormatInfo left, LoggerFormatInfo right)
        {
            return left.CloneAs(
                right.ForegroundColor,
                right.BackgroundColor,
                right.Bold,
                right.Italics,
                right.Underline);
        }
    }

    ////////////////////////////////////////////////////////
    /// <summary>
    ///     The common base class for all logger handlers.  If you would like to
    ///     create a logger handler of your own, you can inherit from this class
    ///     and override its methods to suit your needs.
    /// </summary>
    public class LoggerHandler
    {
        #region ### Constructor ################################################

        public LoggerHandler()
        {
            EnableTimeStamp = false;
        }

        #endregion


        #region ### Internal ###################################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Used by the Logger class to keep track of how many times a 
        ///     handler throws an exception.  If your handler bombs out 5 times,
        ///     it'll be disabled.
        /// </summary>
        internal int ExceptionCount = 0;

        #endregion


        #region ### Public #####################################################

        ////////////////////////////////////////////////////
        /// <summary>
        ///     Override this method to do any necessary cleanup work, like closing
        ///     files, sockets, etc.  This method gets called when the handler is 
        ///     removed from the Logger.
        /// </summary>
        public virtual void Cleanup() { }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Used to turn the timestamp on and off for an individual logger.
        /// </summary>
        public bool EnableTimeStamp { get; set; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Override this method if your LoggerHandler needs to flush its 
        ///     buffers (for instance, when streaming to a file, etc).
        /// </summary>
        public virtual void Flush() { }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     This method gets called when the handler is added to the logger.
        ///     It allows the handler to initialize any internal state, like 
        ///     opening a file, etc.
        /// </summary>
        public virtual void Initialize() { }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Updates the foreground color for all output.  NOTE: Not every handler can
        ///     can display color.  It is the job of the deriving class to maintain color
        ///     state.
        /// </summary>
        public virtual void SetFormat(LoggerFormatInfo format) { }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     This method gets called by the worker thread every time there
        ///     is output to be written.
        /// </summary>
        /// 
        /// <param name="channelHandle">
        ///     A 64-bit bitfield that identifies a line of text as belonging to
        ///     a particular LoggerHandler (there being a possible total of 64
        ///     LoggerHandler instances per process).<para/>
        ///     <para/>
        ///     This parameter is useful when you need to be able to filter
        ///     through output that has already been logged.
        /// </param>
        /// <param name="format">
        ///     LoggerFormatInfo for this single message.  This parameter can be
        ///     null.
        /// </param>
        /// <param name="message">
        ///     The text to be sent to your handler.
        /// </param>
        public virtual void Write(LoggerFormatInfo format, string message) { }

        #endregion
    }
}
