using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FeralNerd.WinForms
{
    ///////////////////////////////////////////////////////
    /// <summary>
    ///     The delegate to be used when calling MessageDlg.ShowWait()
    /// </summary>
    /// 
    /// <remarks>
    ///     If the method returns a true, the wait dialog will return a DialogResult.Yes.  If the 
    ///     method returns false, a DialogResult.No will be returned.  If the method throws an 
    ///     exception then the wait dialog will return a DialogResult.Abort.
    ///     
    ///     The Cancel button is taken care of for you, so don't worry about it.
    /// </remarks>
    /// 
    /// <returns>
    ///     Must return a true if the operation was successful, a false if the operation
    ///     ran to completion but was unsuccessful.  Throw an exception if an unexpected error
    ///     occurrs.
    /// </returns>
    public delegate bool WaitDlgBackgroundWorker();


    public interface IWaitDlgProgressUpdateControl
    {
        bool CheckForCancelRequest();
        void UpdateProgress(int progress);
        void UpdateStatusText(string format, params object[] args);
    }

    ///////////////////////////////////////////////////////
    /// <summary>
    ///     The delegate to be used when calling MessageDlg.ShowProgress()
    /// </summary>
    /// 
    /// <remarks>
    ///     If the method returns a true, the wait dialog will return a DialogResult.Yes.  If the 
    ///     method returns false, a DialogResult.No will be returned.  If the method throws an 
    ///     exception then the progress dialog will return a DialogResult.Abort.
    ///     
    ///     The Cancel button is taken care of for you, so don't worry about it.
    /// </remarks>
    /// 
    /// <returns>
    ///     Must return a true if the operation was successful, a false if the operation
    ///     ran to completion but was unsuccessful.  Throw an exception if an unexpected error
    ///     occurrs.
    /// </returns>
    public delegate bool WaitDlgProgressWorker(IWaitDlgProgressUpdateControl updater);

    public static class MessageDlg
    {
        ///////////////////////////////////////////////////
        /// <summary>
        ///   Sets the default title bar text for all MessageDlg dialog boxes.
        /// </summary>
        public static string TitleText { get; set; }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows an error message box with an OK button, and an icon with a red
        ///     circle and a white X.
        /// </summary>
        /// 
        /// <param name="format">
        ///     The error message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <returns>
        ///     DialogResult.OK
        /// </returns>
        public static DialogResult ShowError(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, TitleText, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        
        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows an exception dialog.
        /// </summary>
        /// 
        /// <param name="ex">
        ///     An exception that was thrown.
        /// </param>
        /// <param name="format">
        ///     The error message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// <returns></returns>
        public static DialogResult ShowException(Exception ex, string format, params object[] args)
        {
            ExceptionDlg dlg = new ExceptionDlg();
            dlg.Message = string.Format(format, args);
            dlg.Exception = ex;

            return dlg.ShowDialog();
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows a message box with an OK button, and an icon with a lower-case i
        ///     in a blue circle.
        /// </summary>
        /// 
        /// <param name="format">
        ///     The message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <returns>
        ///     DialogResult.OK
        /// </returns>
        public static DialogResult ShowOk(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, TitleText, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows a message box with OK and Cancel buttons, and an icon with a lower-case i
        ///     in a blue circle.
        /// </summary>
        /// 
        /// <param name="format">
        ///     The message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <returns>
        ///     DialogResult.OK
        /// </returns>
        public static DialogResult ShowOkCancel(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, TitleText, MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows a please wait dialog with a progress bar and a Cancel button.  
        ///     This dialog will spawn a background worker thread and do whatever 
        ///     you want while the user waits for the operation to complete.<para/>  
        ///     <para/>
        ///     You will get a DialogResult.Yes or DialogResult.No based on whether or 
        ///     not the operation succeeded or failed.  If the user cancels, you will 
        ///     get a DialogResult.Cancel.  If the operation throws an unhandled exception, 
        ///     MessageDlg.ShowWait will re-throw the exception.
        /// </summary>
        /// 
        /// <param name="workerProc">
        ///     A method that you want the background worker thread to execute.
        /// </param>
        /// <param name="format">
        ///     The message you want displayed.  This should explain what the computer is
        ///     checking in the background.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <remarks>
        ///     Use this method to pop up a please-wait dialog.  The worker proc should 
        ///     return true if the operation succeeds and false if the operation fails.<para/>
        ///     <para/>
        ///     The user can press the cancel button at any time to abort the operation.<para/>
        ///     <para/>
        ///     Once ShowWait returns, the caller can check between one of four DialogResult
        ///     values.  Yes means the operation was successful.  No means that the 
        ///     operation failed.  Cancel means that the user cancelled.  Abort means that
        ///     an unhandled exception occurred.<para/>
        ///     <para/>
        ///     It is your responsibility to catch all exceptions in your workerProc and 
        ///     display an appropriate message box.  As a best practice, your workerProc
        ///     should never throw an exception.
        /// </remarks>
        /// 
        /// <example>
        ///     WaitDlgProgressWorker worker = delegate(IWaitDlgProgressUpdateControl ctrl)
        ///     {
        ///         for (int i = 0; i &lt;= 10; i++)
        ///         {
        ///             if (ctrl.CheckForCancelRequest())
        ///                 break;
        ///    
        ///             ctrl.UpdateProgress(i * 10);
        ///             ctrl.UpdateStatusText("Updating item {0} of 100", i);
        ///    
        ///             Thread.Sleep(1000);
        ///         }
        ///    
        ///         return true;
        ///     };
        ///    
        ///     MessageDlg.ShowProgress(worker, "This is going to take a while...");
        /// </example>
        /// 
        /// <returns>
        ///     DialogResult.Yes: The operation completed, and was successful.<para/>
        ///     DialogResult.No: The operation completed, and was unsuccessful.<para/>
        ///     DialogResult.Cancel: The user clicked the cancel button, quitting the operation
        ///         before it could complete.<para/>
        /// </returns>
        /// 
        /// <exception cref="WinFormsException">
        ///     This will pass along any exception that 
        /// </exception>
        public static DialogResult ShowProgress(WaitDlgProgressWorker workerProc, string format, params object[] args)
        {
            ProgressDlg _waitDlg = new ProgressDlg(workerProc, format, args);

            DialogResult res = _waitDlg.ShowDialog();
            if (res == DialogResult.Abort)
                throw new Exception(_waitDlg.Exception.Message, _waitDlg.Exception);

            return res;
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows a please wait dialog with an animated busy-spinner and a Cancel
        ///     button.  This dialog will spawn a background worker thread and do
        ///     whatever you want while the user waits for the operation to complete.<para/>  
        ///     <para/>
        ///     You will get a DialogResult.Yes or DialogResult.No based on whether or 
        ///     not the operation succeeded or failed.  If the user cancels, you will 
        ///     get a DialogResult.Cancel.  If the operation throws an unhandled exception, 
        ///     MessageDlg.ShowWait will re-throw the exception.
        /// </summary>
        /// 
        /// <param name="workerProc">
        ///     A method that you want the background worker thread to execute.
        /// </param>
        /// <param name="format">
        ///     The message you want displayed.  This should explain what the computer is
        ///     checking in the background.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <remarks>
        ///     Use this method to pop up a please-wait dialog.  The worker proc should 
        ///     return true if the operation succeeds and false if the operation fails.<para/>
        ///     <para/>
        ///     The user can press the cancel button at any time to abort the operation.<para/>
        ///     <para/>
        ///     Once ShowWait returns, the caller can check between one of four DialogResult
        ///     values.  Yes means the operation was successful.  No means that the 
        ///     operation failed.  Cancel means that the user cancelled.  Abort means that
        ///     an unhandled exception occurred.<para/>
        ///     <para/>
        ///     It is your responsibility to catch all exceptions in your workerProc and 
        ///     display an appropriate message box.  As a best practice, your workerProc
        ///     should never throw an exception.
        /// </remarks>
        /// 
        /// <returns>
        ///     DialogResult.Yes: The operation completed, and was successful.<para/>
        ///     DialogResult.No: The operation completed, and was unsuccessful.<para/>
        ///     DialogResult.Cancel: The user clicked the cancel button, quitting the operation
        ///         before it could complete.<para/>
        /// </returns>
        /// <exception cref="WinFormsException">
        ///     This will pass along any exception that 
        /// </exception>
        public static DialogResult ShowWait(WaitDlgBackgroundWorker workerProc, string format, params object[] args)
        {
            WaitDlg _waitDlg = new WaitDlg();
            _waitDlg.OnThreadStart = workerProc;

            DialogResult res = DialogResult.Abort;
            res = _waitDlg.ShowDialog(format, args);

            if (res == DialogResult.Abort)
                throw new Exception(_waitDlg.Exception.Message, _waitDlg.Exception);

            return res;
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows an Yes/No message box, with an yellow triangle icon and an 
        ///     exclamation point.  Use this dialog for asking questions.
        /// </summary>
        /// 
        /// <param name="titleText">
        ///     Text for the title bar.
        /// </param>
        /// <param name="format">
        ///     The error message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <returns>
        ///     DialogResult.OK
        /// </returns>
        public static DialogResult ShowYesNo(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, TitleText, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
        }


        ///////////////////////////////////////////////////
        /// <summary>
        ///     Shows an Yes/No/Cancel message box, with an yellow triangle icon and an 
        ///     exclamation point.  Use this dialog for asking questions.
        /// </summary>
        /// 
        /// <param name="titleText">
        ///     Text for the title bar.
        /// </param>
        /// <param name="format">
        ///     The error message you want displayed.
        /// </param>
        /// <param name="args">
        ///     Format arguments.
        /// </param>
        /// 
        /// <returns>
        ///     DialogResult.OK
        /// </returns>
        public static DialogResult ShowYesNoCancel(string format, params object[] args)
        {
            string message = string.Format(format, args);
            return MessageBox.Show(message, TitleText, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
        }
    }
}
