using System;

namespace FeralNerd.FLog
{
    public class LoggerException : Exception
    {
        public LoggerException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public LoggerException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }
    }
}
