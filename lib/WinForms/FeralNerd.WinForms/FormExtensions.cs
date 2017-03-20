using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FeralNerd.WinForms
{
    public static class FormExtensions
    {
        ////////////////////////////////////////////////////
        /// <summary>
        ///     Checks the InvokeRequired property, and invokes an anonymous
        ///     method for you.
        /// </summary>
        /// 
        /// <param name="self">
        ///     Your Form object.
        /// </param>
        /// <param name="codeToInvoke">
        ///     The anonymous method you want to invoke.
        /// </param>
        /// 
        /// <returns>
        ///     If this method return true, then the anonymous method was 
        ///     invoked, and you should return immediately.  See example for
        ///     details.
        /// </returns>
        /// 
        /// <example>
        ///     private void __OnSomeOtherThreadCallback(MyDataObject data)
        ///     {
        ///         // Pass in an anonymous method that re-calls your method.
        ///         if (this.InvokeIfRequired(() => __OnSomeOtherThreadCallback(newState)))
        ///             return;
        ///     
        ///         // Do whatever you want here.  You are now in the UI thread.
        ///     }        
        /// </example>
        public static bool InvokeIfRequired(this Control self, Action codeToInvoke)
        {
            if (self.InvokeRequired)
            {
                self.Invoke(codeToInvoke);
                return true;
            }

            return false;
        }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Restores the window to a saved position, accounting for whether 
        ///     or not the monitor configuration has changed.  If the window is
        ///     partially or entirely off-screen, then the saved position is not
        ///     restored, and Windows will place the window in the default 
        ///     location.
        /// </summary>
        /// 
        /// <param name="self">
        ///     Your Form object.
        /// </param>
        /// <param name="location">
        ///     Upper left point of the form.
        /// </param>
        /// <param name="size">
        ///     Size of the form.
        /// </param>
        public static void RestoreSavedPosition(this Control self, Point location, Size size)
        {
            Rectangle rect = new Rectangle(location, size);
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(rect))
                {
                    self.Location = location;
                    self.Size = size;

                    break;
                }
            }
        }
    }
}
