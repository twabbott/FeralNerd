using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace FeralNerd.WinForms
{
    internal partial class WaitDlg : Form
    {
        private Thread _backgroundThread = null;
        private ManualResetEvent _connectionTestComplete = new ManualResetEvent(false);

        public WaitDlg()
        {
            InitializeComponent();
        }

        public string Message { get; set; }
        public bool ShowCancelButton { get; set; }
        public WaitDlgBackgroundWorker OnThreadStart;
        public Exception Exception { get; private set; }


        private void WaitDlg_Load(object sender, EventArgs e)
        {
            MessageLabel.Text = Message;
            CancelBtn.Visible = ShowCancelButton;
            CancelBtn.Enabled = true;
        }

        new public DialogResult ShowDialog(IWin32Window owner)
        {
            throw new NotSupportedException("Do not call WaitDlg.Show(IWin32Window owner)!");
        }

        new public DialogResult ShowDialog()
        {
            DialogResult = DialogResult.None;

            CheckConnectionTestCompleteTimer.Enabled = true;

            // Un-set the event.  This will get set by __WorkerThreadMain when 
            // the work is complete.
            _connectionTestComplete.Reset();

            // Start the worker thread.
            _backgroundThread = new Thread(__WorkerThreadMain);
            _backgroundThread.Start();

            return base.ShowDialog();
        }

        public DialogResult ShowDialog(string format, params object[] args)
        {
            try
            {
                Message = string.Format(format, args);
            }
            catch (FormatException)
            {
                Message = "<<<format error>>>";
            }

            ShowCancelButton = true;

            return ShowDialog();
        }

        new public void Show()
        {
            throw new NotSupportedException("Do not call WaitDlg.Show()!");
        }

        new public void Show(IWin32Window owner)
        {
            throw new NotSupportedException("Do not call WaitDlg.Show()!");
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            // Disable the cancel button to give the user some feedback
            CancelBtn.Enabled = false;

            // Abort the thread.
            _backgroundThread.Abort();

            // Signal that processing is complete.  The rest of the cleanup will happen 
            // in the event handler for the timer.
            _connectionTestComplete.Set();
        }

        private void CheckConnectionTestCompleteTimer_Tick(object sender, EventArgs e)
        {
            if (_connectionTestComplete.WaitOne(0))
            {
                CheckConnectionTestCompleteTimer.Enabled = false;

                Close();
            }
        }

        private void __WorkerThreadMain()
        {
            try
            {
                if (OnThreadStart())
                    DialogResult = DialogResult.Yes; // Operation was successful
                else
                    DialogResult = DialogResult.No; // Operation was not successful
            }
            catch (ThreadAbortException)
            {
                // User canceled the request.
                DialogResult = DialogResult.Cancel;
            }
            catch (Exception ex)
            {
                // Some other exception got thrown, and the real worker function
                // never caught it.
                this.Exception = ex;
                DialogResult = DialogResult.Abort;
            }

            _connectionTestComplete.Set();
        }
    }
}
