namespace FeralNerd.WinForms
{
    partial class ExceptionDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.OKBtn = new System.Windows.Forms.Button();
            this.ExceptionInfoEdit = new System.Windows.Forms.TextBox();
            this.MessageLabel = new System.Windows.Forms.Label();
            this.CopyToClipboardBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // OKBtn
            // 
            this.OKBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKBtn.Location = new System.Drawing.Point(471, 196);
            this.OKBtn.Name = "OKBtn";
            this.OKBtn.Size = new System.Drawing.Size(75, 23);
            this.OKBtn.TabIndex = 0;
            this.OKBtn.Text = "OK";
            this.OKBtn.UseVisualStyleBackColor = true;
            // 
            // ExceptionInfoEdit
            // 
            this.ExceptionInfoEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ExceptionInfoEdit.BackColor = System.Drawing.SystemColors.Window;
            this.ExceptionInfoEdit.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExceptionInfoEdit.HideSelection = false;
            this.ExceptionInfoEdit.Location = new System.Drawing.Point(12, 25);
            this.ExceptionInfoEdit.Multiline = true;
            this.ExceptionInfoEdit.Name = "ExceptionInfoEdit";
            this.ExceptionInfoEdit.ReadOnly = true;
            this.ExceptionInfoEdit.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ExceptionInfoEdit.Size = new System.Drawing.Size(532, 165);
            this.ExceptionInfoEdit.TabIndex = 1;
            this.ExceptionInfoEdit.WordWrap = false;
            // 
            // MessageLabel
            // 
            this.MessageLabel.AutoSize = true;
            this.MessageLabel.Location = new System.Drawing.Point(12, 9);
            this.MessageLabel.Name = "MessageLabel";
            this.MessageLabel.Size = new System.Drawing.Size(35, 13);
            this.MessageLabel.TabIndex = 2;
            this.MessageLabel.Text = "label1";
            // 
            // CopyToClipboardBtn
            // 
            this.CopyToClipboardBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.CopyToClipboardBtn.Location = new System.Drawing.Point(12, 196);
            this.CopyToClipboardBtn.Name = "CopyToClipboardBtn";
            this.CopyToClipboardBtn.Size = new System.Drawing.Size(118, 23);
            this.CopyToClipboardBtn.TabIndex = 3;
            this.CopyToClipboardBtn.Text = "Copy to Clipboard";
            this.CopyToClipboardBtn.UseVisualStyleBackColor = true;
            this.CopyToClipboardBtn.Click += new System.EventHandler(this.CopyToClipboardBtn_Click);
            // 
            // ExceptionDlg
            // 
            this.AcceptButton = this.OKBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 231);
            this.Controls.Add(this.CopyToClipboardBtn);
            this.Controls.Add(this.MessageLabel);
            this.Controls.Add(this.ExceptionInfoEdit);
            this.Controls.Add(this.OKBtn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExceptionDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Exception";
            this.Load += new System.EventHandler(this.ExceptionDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OKBtn;
        private System.Windows.Forms.TextBox ExceptionInfoEdit;
        private System.Windows.Forms.Label MessageLabel;
        private System.Windows.Forms.Button CopyToClipboardBtn;
    }
}