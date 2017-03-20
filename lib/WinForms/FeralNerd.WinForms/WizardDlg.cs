using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using FeralNerd.WinForms;

//##############################################################################
// Sample code.  This is pretty straightforward:
//     1.  Define your enumerated type
//     2.  Make a new instance of WizardDlg<T>
//     3.  Set the Text property for the title bar
//     4.  Call AddPanel.  
//     5.  Call ShowDialog().  If the user finishes the wizard, you'll get a 
//         DialogResult.OK.  Otherwise you'll get a DialogResult.Cancel.
//
// The behavior of each panel is handled through the IWizardPanel<T> interface.
//
//    ///////////////////////////////////////////////
//    // Define an enum that will represent each panel in the wizard.
//    public enum WizardState
//    {
//        // IWizard.ShowPanel uses -1 as a catch-all NOOP value.  Your Welcome
//        // and finish panels can use this when there is no next/previous panel
//        // to transition to.
//        None = -1, 
//
//        Welcome,
//        EnvironmentParameters,
//        MediaServers,
//        Finished,
//        Abort
//    }
//    
//    class Program
//    {
//        public const string AppTitle = "Cluster Seeder Tool";
//        public static Logger Logger = new Logger(true);
//    
//        static void Main(string[] args)
//        {
//            ///////////////////////////////////////////////
//            // Create a new WizardDlg
//            WizardDlg<WizardState> wizard = new WizardDlg<WizardState>();
//    
//    
//            ///////////////////////////////////////////////
//            // Set the Text property
//            wizard.Text = "The Do-something Wizard";
//    
//    
//            ///////////////////////////////////////////////
//            // Add all panels
//            wizard.AddPanel(new PanelWelcome(), WizardState.Welcome, true);
//            wizard.AddPanel(new PanelEnvironmentParameters(), WizardState.EnvironmentParameters);
//            wizard.AddPanel(new PanelMediaServers(), WizardState.MediaServers);
//            wizard.AddPanel(new PanelFinished(), WizardStateFinished);
//    
//    
//            ///////////////////////////////////////////////
//            // Call ShowDialog()
//            if (wizard.ShowDialog() == DialogResult.OK)
//            {
//                // Put all your code here for when the wizard finishes
//                // successfully.
//            }
//        }
//    }
//    
//##############################################################################


namespace FeralNerd.WinForms
{
    ////////////////////////////////////////////////////////////////////////////
    public enum WizardButton
    {
        None,
        Back,
        Next,
        Finish,
        Cancel
    }


    /// <summary>
    ///     Used to designate the state you want to set the wizard buttons to.
    /// </summary>
    public enum WizardButtonState
    {
        /// <summary>
        ///     Leave the buttons as they are currently set.
        /// </summary>
        NoChange,


        /// <summary>
        ///     This panel is a welcome page.  Disable the back button.
        /// </summary>
        WelcomePage,


        /// <summary>
        ///     This panel is a middle page.  Enable all wizard buttons.
        /// </summary>
        MiddlePage,


        /// <summary>
        ///     This panel is a middle page, that uses custom logic to invoke
        ///     the next page.
        /// </summary>
        MiddlePageNoNext,


        /// <summary>
        ///     This panel is a finish page.  Hide the next button, and 
        ///     show the finished button.
        /// </summary>
        FinishedPage,


        /// <summary>
        ///     This is a finish page, but the user input on this page still
        ///     needs to be validated before the user can click finish.
        /// </summary>
        FinishedPageDisableFinish
    }


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     Interface for all wizard panels.  All panels must inherit from 
    ///     UserControl, and must implement this interface.
    /// </summary>
    public interface IWizardPanel<EnumState>
    {
        /// <summary>
        ///     The WizardDlg calls this method once, when the WizardDlg's OnLoad
        ///     event gets fired.  You should perform all first-time only init work here.
        /// </summary>
        /// 
        /// <param name="parent">
        ///     Use the IWizard interface to change the state of the controls of the
        ///     containing wizard.
        /// </param>
        void OnLoad(IWizard<EnumState> parent);


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The WizardDlg calls this method just before a panel is about to 
        ///     be shown.  
        /// </summary>
        /// 
        /// <returns>
        ///     Return one of the WizardButtonState members, which will indicate
        ///     which of the wizard buttons (back, next, etc) should be enabled.
        /// </returns>
        WizardButtonState OnPreShow();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The WizardDlg calls this method when the user clicks the Next
        ///     button.  This is where you put all the transition logic for 
        ///     picking the next panel.<para/>
        ///     <para/>
        ///     The WizardDlg will call the OnValidateAllInput method before 
        ///     this method is invoked.  To remain on the same panel, return
        ///     whatever your PanelState property is set to.<para/>
        ///     <para/>
        ///     NOTE: Do not use this method to do cleanup work.  If you have
        ///     to do on-back or on-next cleanup work, use OnValidateAllInput, 
        ///     instead.
        /// </summary>
        /// 
        /// <returns>
        ///     The name of the panel that should be shown next.
        /// </returns>
        EnumState OnTransitionToNextPanel();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The WizardDlg calls this method when the user clicks the Back
        ///     button.
        /// </summary>
        /// 
        /// <returns>
        ///     The name of the panel that the wizard should go back to.  To
        ///     stay on the same panel, return a null or empty string.
        /// </returns>
        EnumState OnTransitionToPreviousPanel();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     The WizardDlg calls this method when the user clicks the Next 
        ///     button, but before OnTransitionToNextPanel() is called.
        /// </summary>
        /// 
        /// <param name="wizardDlg">
        ///     The parent WizardDlg
        /// </param>
        /// 
        /// <returns>
        ///     If validation is successful, your panel should return true.  
        ///     Otherwise you should display an error dialog, then  should 
        ///     return false.
        /// </returns>
        bool OnValidateAllInput(IWizard<EnumState> wizardDlg);
    }


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     An interface passed to each client panel.  Panels can access the 
    ///     wizard through this interface.
    /// </summary>
    public interface IWizard<EnumState>
    {
        ////////////////////////////////////////////////////
        /// <summary>
        ///     Call this when you want the wizard to close.
        /// </summary>
        void Close();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Call this when you want to ask the user if it is all right to 
        ///     close/cancel the wizard.
        /// </summary>
        /// <returns></returns>
        DialogResult ConfirmCancel();


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the panel that is currently shown.
        /// </summary>
        IWizardPanel<EnumState> CurrentPanel { get; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Gets the state that corresponds to the current panel.
        /// </summary>
        EnumState CurrentPanelState { get; }


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Call this when you want the wizard to show a panel
        /// </summary>
        /// 
        /// <param name="panelName">
        ///     The name of the panel you want shown.  The panel that is 
        ///     currently visible (if there is one) will be hidden.
        /// </param>
        /// 
        /// <remarks>
        ///     The WizardDlg will not invoke IWizardPenel.OnValidateAllInput 
        ///     on the calling panel.  It is expected that the calling panel will
        ///     take care of all input validation.<para/>
        ///     
        ///     The WizardDlg will not invoke IWizardPanel.OnTransitionXxx
        ///     on the calling panel.  
        /// </remarks>
        void ShowPanel(EnumState panelName);


        ////////////////////////////////////////////////////
        /// <summary>
        ///     Sets the button state for the wizard.  The wizard sets the 
        ///     initial button state through the return value of IWizardPanel.OnPreShow().
        ///     Use this method to enable or disable the buttons as you see fit.
        /// </summary>
        /// 
        /// <param name="wizardButtonState">
        ///     One of the WizardButtonState values.
        /// </param>
        void SetButtonState(WizardButtonState wizardButtonState);
    }


    ////////////////////////////////////////////////////////////////////////////
    /// <summary>
    ///     A wizard framework class
    /// </summary>
    public partial class WizardDlg<EnumState> : Form, IWizard<EnumState>
    {
        #region ### Private Fields #############################################

        internal class WizardPanelInfo<State>
        {
            public State PanelID;
            public bool IsStartPanel;
            public UserControl PanelControl;

            public WizardPanelInfo(State panelID, bool isStartPanel, UserControl panel)
            {
                PanelID = panelID;
                IsStartPanel = isStartPanel;
                PanelControl = panel;
            }
        }

        bool _finished = false;
        private WizardPanelInfo<EnumState> _visiblePanel = null;

        private readonly List<WizardPanelInfo<EnumState>> _panels = new List<WizardPanelInfo<EnumState>>();

        #endregion


		public WizardDlg()
		{
            ConfirmBeforeCancel = true;
			InitializeComponent();
		}


        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Set this property to confirm with the user before they abort the 
        ///     wizard.
        /// </summary>
        public bool ConfirmBeforeCancel { get; set; }


        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Adds a panel to the wizard.  A panel is a UserControl that implements
        ///     the IWizardPanel&lt;&gt; interface.  The UserControl should be about 432x363
        ///     pixels.  
        /// </summary>
        /// 
        /// <param name="control">
        ///     A plain-vanilla UserControl that inherits IWizardPanel.  Control size
        ///     should be 432x363.
        /// </param>
        /// <param name="panelID">
        ///     An identifier used for referencing this panel.
        /// </param>
        /// <param name="isStartPanel">
        ///     Set this parameter to true if this is the starting panel.
        /// </param>
        public void AddPanel(UserControl control, EnumState panelID, bool isStartPanel = false)
        {
            if (!(control is IWizardPanel<EnumState>))
                throw new WinFormsException("WizardDlg.AddPanel must take a UserControl that implements IWizardPanel");

            object obj = Enum.Parse(typeof(EnumState), panelID.ToString());
            if (!(obj is Enum))
                throw new WinFormsException("Wizard dialog must use an enumerated type.");

            WizardPanelInfo<EnumState> panelInfo = new WizardPanelInfo<EnumState>(panelID, isStartPanel, control);

            if (isStartPanel)
            {
                if (_visiblePanel != null)
                    _visiblePanel.PanelControl.Visible = false;

                _visiblePanel = panelInfo;
                _visiblePanel.PanelControl.Visible = true;
            }
            else
                control.Visible = false;

            _panels.Add(panelInfo);
        }
         

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Sets the left panel graphic.  This graphic should be 213x362 pixels.<para/>
        ///     
        ///     You can use a strongly-typed resource.  Add a Resources.resx to your
        ///     project, then open it in the resource browser.  Resources can be accessed
        ///     as &lt;namespace-name&gt;.Properties.Resources.&lt;ResourceName&gt;.
        /// </summary>
        /// 
        /// <param name="bmp">
        ///     A bitmap.  The bitmap should be 213x362 pixels.
        /// </param>
        public void SetLeftGraphicImage(Bitmap bmp)
        {
            LeftGraphicImage.Image = bmp;
        }


        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///     Use this to set the icon.
        /// </summary>
        /// 
        /// <param name="icon">
        ///     The icon you want to set.
        /// </param>
        public void SetIcon(Icon icon)
        {
            Icon = icon;
        }

        #region ### IWizard Implementation #####################################

        public DialogResult ConfirmCancel()
		{
            if (_finished)
                return DialogResult.Yes;

            if (!ConfirmBeforeCancel)
                return DialogResult.Yes;

            return MessageDlg.ShowYesNo("Do you really wish to cancel?");
		}

        public IWizardPanel<EnumState> CurrentPanel 
        { 
            get 
            {
                return _visiblePanel.PanelControl as IWizardPanel<EnumState>; 
            } 
        }

        public EnumState CurrentPanelState
        {
            get
            {
                return _visiblePanel.PanelID;
            }
        }

        private WizardPanelInfo<EnumState> __GetPanel(EnumState panelID)
        {
            foreach (WizardPanelInfo<EnumState> panel in _panels)
            {
                if (__GetEnumValue(panel.PanelID) == __GetEnumValue(panelID))
                    return panel;
            }

            MessageDlg.ShowError("Wizard dialog \"{0}\" has no panel named \"{1}\"", Text, panelID);
            return null;
        }

        private int __GetEnumValue(EnumState panelID)
        {
            object obj = Enum.Parse(typeof(EnumState), panelID.ToString());

            return (int)obj;
        }

        public void ShowPanel(EnumState panelID)
        {
            if (__GetEnumValue(panelID) == -1)
                return;

            WizardPanelInfo<EnumState> info = __GetPanel(panelID);
            if (info == null)
                return;
            UserControl control = info.PanelControl;
            if (control == null)
                return;

            IWizardPanel<EnumState> iPanel = control as IWizardPanel<EnumState>;
            if (iPanel == null)
            {
                MessageDlg.ShowError("Panel \"{0}\" of WizardDlg \"{1}\" does not implement IWizardPanel interface.", panelID, Text);
                return;
            }

            // Hide the panel that is currently visible.
            if (_visiblePanel != null)
                _visiblePanel.PanelControl.Visible = false;

            _visiblePanel = info;
            _visiblePanel.PanelControl.Visible = true;
            SetButtonState(iPanel.OnPreShow());
        }

        public void SetButtonState(WizardButtonState buttonState)
        {
            switch (buttonState)
            {
                case WizardButtonState.NoChange:
                    break;

                case WizardButtonState.WelcomePage:
                    BackBtn.Enabled = false;
                    NextBtn.Enabled = true;
                    NextBtn.Visible = true;

                    FinishBtn.Visible = false;
                    break;

                case WizardButtonState.MiddlePage:
                    BackBtn.Enabled = true;
                    NextBtn.Enabled = true;
                    NextBtn.Visible = true;

                    FinishBtn.Visible = false;
                    break;

                case WizardButtonState.MiddlePageNoNext:
                    BackBtn.Enabled = true;
                    NextBtn.Enabled = false;
                    NextBtn.Visible = true;

                    FinishBtn.Visible = false;
                    break;

                case WizardButtonState.FinishedPage:
                    BackBtn.Enabled = true;
                    NextBtn.Enabled = false;
                    NextBtn.Visible = false;

                    FinishBtn.Visible = true;
                    FinishBtn.Enabled = true;
                    break;

                case WizardButtonState.FinishedPageDisableFinish:
                    BackBtn.Enabled = true;
                    NextBtn.Enabled = false;
                    NextBtn.Visible = false;

                    FinishBtn.Visible = true;
                    FinishBtn.Enabled = false;
                    break;
            }
        }

        #endregion


        #region ### Private: Control Handlers ##################################

        private void __WizardFrame_Load(object sender, EventArgs e)
		{
			int startX = 213;
			int sizeX = 432;
			int sizeY = 363;

            foreach (WizardPanelInfo<EnumState> panel in _panels)
            {
                panel.PanelControl.SuspendLayout();
                panel.PanelControl.Location = new Point(startX, 0);
                panel.PanelControl.Size = new Size(sizeX, sizeY);

                Controls.Add(panel.PanelControl);
                panel.PanelControl.ResumeLayout();

                IWizardPanel<EnumState> iPanel = (IWizardPanel<EnumState>)panel.PanelControl as IWizardPanel<EnumState>;
                try
                {
                    iPanel.OnLoad(this);
                }
                catch (Exception ex)
                {
                    MessageDlg.ShowException(ex, "WizardDlg caught an unhandled exception calling IWizardPanel<>.OnLoad()");
                }
            }

            if (_visiblePanel != null)
            {
                IWizardPanel<EnumState> iPanel = (IWizardPanel<EnumState>)_visiblePanel.PanelControl as IWizardPanel<EnumState>;
                SetButtonState(iPanel.OnPreShow());
            }
		}

        private void __BackBtn_Click(object sender, EventArgs e)
		{
            // If no panels are visible, do nothing.
            if (_visiblePanel == null)
                return;

            EnumState state = default(EnumState);
            try
            {
                state = CurrentPanel.OnTransitionToPreviousPanel();
            }
            catch (Exception ex)
            {
                MessageDlg.ShowException(ex, "WizardDlg caught an unhandled exception calling IWizardPanel<>.OnTransitionToPreviousPanel()");
                return;
            }

            ShowPanel(state);
		}

		private void __NextBtn_Click(object sender, EventArgs e)
		{
            // If no panels are visible, do nothing.
            if (_visiblePanel == null)
                return;

            if (!__CallValidation())
                return;

            EnumState state = default(EnumState);
            try
            {
                state = CurrentPanel.OnTransitionToNextPanel();
            }
            catch (Exception ex)
            {
                MessageDlg.ShowException(ex, "WizardDlg caught an unhandled exception calling IWizardPanel<>.OnTransitionToNextPanel()");
                return;
            }
                        
            ShowPanel(state);
        }

		private void __FinishBtn_Click(object sender, EventArgs e)
		{
            if (!__CallValidation())
            {
                DialogResult = DialogResult.None;
                return;
            }

            DialogResult = DialogResult.OK;
            _finished = true;
		}


        private bool __CallValidation()
        {
            // Validate the current panel.  If validation fails, then stay on 
            // the same panel.
            bool validated = false;
            try
            {
                validated = CurrentPanel.OnValidateAllInput(this);
            }
            catch (Exception ex)
            {
                MessageDlg.ShowException(ex, "WizardDlg caught an unhandled exception calling IWizardPanel<>.OnValidateAllInput()");
            }

            return validated;
        }

		private void __CancelBtn_Click(object sender, EventArgs e)
		{
		}

        private void __WizardFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.TaskManagerClosing:
                case CloseReason.WindowsShutDown:
                    // If this is a system shutdown, then go ahead.
                    return;

                default:
                    // Ask the user.  If they say yes, then allow it
                    if (ConfirmCancel() == DialogResult.Yes)
                        return;

                    // Looks like we better stay open.
                    e.Cancel = true;
                    break;
            }
        }

        #endregion
    }
}
