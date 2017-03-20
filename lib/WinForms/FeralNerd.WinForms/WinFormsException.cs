using System;

namespace FeralNerd.WinForms
{
    public class WinFormsException : Exception
    {
        public WinFormsException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public WinFormsException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }
    }
}
