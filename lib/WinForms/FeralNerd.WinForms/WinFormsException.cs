using System;

namespace FeralNerd.WinForms
{
    /// <summary>
    ///     A general-purpose exception class for the WinForms library.
    /// </summary>
    public class WinFormsException : Exception
    {
        /// <summary>
        ///     Creates a new WinFormsException object.
        /// </summary>
        /// 
        /// <param name="format">
        ///     The exception message.
        /// </param>
        /// <param name="args">
        ///     Variable-argument list.
        /// </param>
        public WinFormsException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }


        /// <summary>
        ///     Creates a new WinFormsException object.
        /// </summary>
        /// 
        /// <param name="innerException">
        ///     An inner exception that you want to re-throw.
        /// </param>
        /// <param name="format">
        ///     The exception message.
        /// </param>
        /// <param name="args">
        ///     Variable-argument list.
        /// </param>
        public WinFormsException(Exception innerException, string format, params object[] args)
            : base(string.Format(format, args), innerException)
        {
        }
    }
}
