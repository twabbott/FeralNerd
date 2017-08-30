using System;
using System.Collections.Generic;
using System.Text;

namespace FeralNerd.uTest
{
    public class AutomationFrameworkException : Exception
    {
        public AutomationFrameworkException(string format, params object[] args)
            : base((args.Length > 0) ? string.Format(format, args) : format)
        {
        }

        public AutomationFrameworkException(Exception innerException, string format, params object[] args)
            : base((args.Length > 0) ? string.Format(format, args) : format, innerException)
        {
        }
    }
}
