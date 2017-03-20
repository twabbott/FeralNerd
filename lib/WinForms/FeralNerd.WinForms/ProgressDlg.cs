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
    internal partial class ProgressDlg : Form
    {
        private Thread _workerThread;
        private string _messageText;
        private WaitDlgProgressWorker _workerProc;
        private bool _success = false;
        private Exception _workerException = null;

        public Exception Exception { get { return _workerException; } }

        private class WaitDlgProgressUpdateControl: IWaitDlgProgressUpdateControl
        {
            ProgressDlg _parent;

            public WaitDlgProgressUpdateControl(ProgressDlg parent)
            {
                _parent = parent;
            }

            public bool CheckForCancelRequest()
            {
                return _parent.BackgroundWorker.CancellationPending;
            }

            public void UpdateProgress(int progress)
            {
                _parent.BackgroundWorker.ReportProgress(progress);
            }

            public void UpdateStatusText(string format, params object[] args)
            {
                _parent.Invoke((MethodInvoker)delegate()
                {
                    _parent.MessageLabel.Text = string.Format(format, args);
                });
            }
        }

        public ProgressDlg(WaitDlgProgressWorker workerProc, string format, params object[] args)
        {
            if (workerProc == null)
                throw new ArgumentNullException("workerProc");
            _workerProc = workerProc;
            _messageText = string.Format(format, args);

            InitializeComponent();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _workerThread = Thread.CurrentThread;
                _success = _workerProc(e.Argument as IWaitDlgProgressUpdateControl);
            }
            catch (ThreadAbortException ex)
            {
                Thread.ResetAbort();
                _success = false;
            }
            catch (Exception ex)
            {
                _workerException = ex;
            }
        }

        private void ProgressDlg_Shown(object sender, EventArgs e)
        {
            MessageLabel.Text = _messageText;
            StatusLabel.Text = "";
            JobProgressBar.Value = 0;

            BackgroundWorker.RunWorkerAsync(new WaitDlgProgressUpdateControl(this));
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int x = e.ProgressPercentage;
            if (x > 100)
                x = 100;

            JobProgressBar.Value = x;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (_workerException != null)
            {
                DialogResult = DialogResult.Abort;
                Close();
            }

            DialogResult = _success ? DialogResult.Yes : DialogResult.No;
            Close();
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            BackgroundWorker.CancelAsync();
            CheckWorkerDoneTimer.Start();

            DialogResult = DialogResult.None;
        }

        private void CheckWorkerDoneTimer_Tick(object sender, EventArgs e)
        {
            CheckWorkerDoneTimer.Stop();
            _workerThread.Abort();
        }
    }
}
