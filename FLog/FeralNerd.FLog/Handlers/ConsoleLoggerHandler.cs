using System;

namespace FeralNerd.FLog
{
    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     This is a class for logging to the Console
    /// </summary>
    internal class ConsoleLoggerHandler : LoggerHandler
    {
        internal static readonly ConsoleLoggerHandler Singleton = new ConsoleLoggerHandler();

        private ConsoleLoggerHandler() : base() { }

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
            ConsoleColor fore = default(ConsoleColor), back = default(ConsoleColor);
            if (format != null)
            {
                fore = Console.ForegroundColor;
                back = Console.BackgroundColor;
                SetFormat(format);
            }

            Console.WriteLine(message);

            if (format != null)
            {
                Console.ForegroundColor = fore;
                Console.BackgroundColor = back;
            }
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Updates the foreground color for all output.  NOTE: Not every handler can
        ///     can display color.  
        /// </summary>
        public override void SetFormat(LoggerFormatInfo format)
        {
            switch (format.ForegroundColor)
            {
                case LoggerColors.NoChange: break;
                case LoggerColors.Black: Console.ForegroundColor = ConsoleColor.Black; break;
                case LoggerColors.Blue: Console.ForegroundColor = ConsoleColor.Blue; break;
                case LoggerColors.Brown: Console.ForegroundColor = ConsoleColor.DarkYellow; break;
                case LoggerColors.Cyan: Console.ForegroundColor = ConsoleColor.Cyan; break;
                case LoggerColors.DarkBlue: Console.ForegroundColor = ConsoleColor.DarkBlue; break;
                case LoggerColors.DarkGray: Console.ForegroundColor = ConsoleColor.DarkGray; break;
                case LoggerColors.DarkGreen: Console.ForegroundColor = ConsoleColor.DarkGreen; break;
                case LoggerColors.DarkCyan: Console.ForegroundColor = ConsoleColor.DarkCyan; break;
                case LoggerColors.DarkRed: Console.ForegroundColor = ConsoleColor.DarkRed; break;
                case LoggerColors.DarkMagenta: Console.ForegroundColor = ConsoleColor.DarkMagenta; break;
                case LoggerColors.Green: Console.ForegroundColor = ConsoleColor.Green; break;
                case LoggerColors.Gray: Console.ForegroundColor = ConsoleColor.Gray; break;
                case LoggerColors.Magenta: Console.ForegroundColor = ConsoleColor.Magenta; break;
                case LoggerColors.Red: Console.ForegroundColor = ConsoleColor.Red; break;
                case LoggerColors.White: Console.ForegroundColor = ConsoleColor.White; break;
                case LoggerColors.Yellow: Console.ForegroundColor = ConsoleColor.Yellow; break;
            }

            if (format.Bold > 0)
            {
                switch (Console.ForegroundColor)
                {
                    case ConsoleColor.DarkYellow: Console.ForegroundColor = ConsoleColor.Yellow; break;
                    case ConsoleColor.DarkBlue: Console.ForegroundColor = ConsoleColor.Blue; break;
                    case ConsoleColor.DarkGray: Console.ForegroundColor = ConsoleColor.Gray; break;
                    case ConsoleColor.DarkGreen: Console.ForegroundColor = ConsoleColor.Green; break;
                    case ConsoleColor.DarkCyan: Console.ForegroundColor = ConsoleColor.Cyan; break;
                    case ConsoleColor.DarkRed: Console.ForegroundColor = ConsoleColor.Red; break;
                    case ConsoleColor.DarkMagenta: Console.ForegroundColor = ConsoleColor.Magenta; break;
                    case ConsoleColor.Gray: Console.ForegroundColor = ConsoleColor.White; break;
                }
            }

            switch (format.BackgroundColor)
            {
                case LoggerColors.NoChange: break;
                case LoggerColors.Black: Console.BackgroundColor = ConsoleColor.Black; break;
                case LoggerColors.Blue: Console.BackgroundColor = ConsoleColor.Blue; break;
                case LoggerColors.Brown: Console.BackgroundColor = ConsoleColor.DarkYellow; break;
                case LoggerColors.Cyan: Console.BackgroundColor = ConsoleColor.Cyan; break;
                case LoggerColors.DarkBlue: Console.BackgroundColor = ConsoleColor.DarkBlue; break;
                case LoggerColors.DarkGray: Console.BackgroundColor = ConsoleColor.DarkGray; break;
                case LoggerColors.DarkGreen: Console.BackgroundColor = ConsoleColor.DarkGreen; break;
                case LoggerColors.DarkCyan: Console.BackgroundColor = ConsoleColor.DarkCyan; break;
                case LoggerColors.DarkRed: Console.BackgroundColor = ConsoleColor.DarkRed; break;
                case LoggerColors.DarkMagenta: Console.BackgroundColor = ConsoleColor.DarkMagenta; break;
                case LoggerColors.Green: Console.BackgroundColor = ConsoleColor.Green; break;
                case LoggerColors.Gray: Console.BackgroundColor = ConsoleColor.Gray; break;
                case LoggerColors.Magenta: Console.BackgroundColor = ConsoleColor.Magenta; break;
                case LoggerColors.Red: Console.BackgroundColor = ConsoleColor.Red; break;
                case LoggerColors.White: Console.BackgroundColor = ConsoleColor.White; break;
                case LoggerColors.Yellow: Console.BackgroundColor = ConsoleColor.Yellow; break;
            }
        }

        #endregion
    }
}