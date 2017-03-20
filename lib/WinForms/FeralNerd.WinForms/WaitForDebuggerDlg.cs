using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace FeralNerd.WinForms
{
    public enum WaitForDebuggerResult
    {
        Run,
        Quit
    }

    public partial class WaitForDebuggerDlg : Form
    {
        internal WaitForDebuggerDlg()
        {
            InitializeComponent();
        }

        private void WaitForDebuggerDlg_Load(object sender, EventArgs e)
        {
            Text = Path.GetFileName(Assembly.GetEntryAssembly().Location);
            OnAttachCombo.SelectedIndex = 1;
            CheckDebuggerTimer.Enabled = true;
        }

        private void CheckDebuggerTimer_Tick(object sender, EventArgs e)
        {
            if (Debugger.IsAttached)
            {
                CheckDebuggerTimer.Enabled = false;

                if (OnAttachCombo.Text == "Break")
                    Debugger.Break();

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void ContinueBtn_Click(object sender, EventArgs e)
        {
            CheckDebuggerTimer.Enabled = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AbortBtn_Click(object sender, EventArgs e)
        {
            CheckDebuggerTimer.Enabled = false;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public static WaitForDebuggerResult Show()
        {
            WaitForDebuggerDlg dlg = new WaitForDebuggerDlg();
            if (dlg.ShowDialog() == DialogResult.OK)
                return WaitForDebuggerResult.Run;

            return WaitForDebuggerResult.Quit;
        }
    }
}
