using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeralNerd.WinForms
{
    public static class EditBoxExtensions
    {
        public static void WriteLine(this TextBox self, string message, params object[] args)
        {
            if (args == null || args.Length == 0)
                self.AppendText(message + "\r\n");
            else
                self.AppendText(string.Format(message + "\r\n", args));
        }

        public static void WriteLine(this TextBox self)
        {
            self.AppendText("\r\n");
        }
    }
}
