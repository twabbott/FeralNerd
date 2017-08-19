using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeralNerd.IoC
{
    public class IoCContainerException: Exception
    {
        public IoCContainerException(string message)
            : base(message)
        {
        }

        public IoCContainerException(string message, params object[] args)
            : base (string.Format(message, args))
        {
        }
    }
}
