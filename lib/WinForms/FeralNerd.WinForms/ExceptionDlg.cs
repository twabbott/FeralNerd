using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeralNerd.WinForms
{
    internal partial class ExceptionDlg : Form
    {
        public string Message;
        public Exception Exception;

        public ExceptionDlg()
        {
            InitializeComponent();
        }

        private void CopyToClipboardBtn_Click(object sender, EventArgs e)
        {
            // Don't have to do this.  It's mostly for visual effect.
            ExceptionInfoEdit.SelectAll();

            // Copy to the clipboard...
            Clipboard.SetText(ExceptionInfoEdit.Text);
        }

        private void ExceptionDlg_Load(object sender, EventArgs e)
        {
            MessageLabel.Text = Message;

            Exception ex = Exception;
            StringBuilder sb = new StringBuilder();
            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.GetType().ToString());
                sb.AppendLine(ex.StackTrace);
                sb.AppendLine();

                ex = ex.InnerException;
            }

            ExceptionInfoEdit.Text = sb.ToString();
        }
    }
}
