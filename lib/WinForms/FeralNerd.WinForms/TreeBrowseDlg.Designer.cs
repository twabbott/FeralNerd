namespace FeralNerd.WinForms
{
    partial class TreeBrowseDlg
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeBrowseDlg));
            this.OkBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.DefaultIcons = new System.Windows.Forms.ImageList(this.components);
            this.PromptLabel = new System.Windows.Forms.Label();
            this.CheckImageList = new System.Windows.Forms.ImageList(this.components);
            this.ItemsTview = new FeralNerd.WinForms.TriStateTreeView();
            this.SuspendLayout();
            // 
            // OkBtn
            // 
            this.OkBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkBtn.Location = new System.Drawing.Point(223, 357);
            this.OkBtn.Name = "OkBtn";
            this.OkBtn.Size = new System.Drawing.Size(75, 23);
            this.OkBtn.TabIndex = 0;
            this.OkBtn.Text = "OK";
            this.OkBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(304, 357);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // DefaultIcons
            // 
            this.DefaultIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("DefaultIcons.ImageStream")));
            this.DefaultIcons.TransparentColor = System.Drawing.Color.Transparent;
            this.DefaultIcons.Images.SetKeyName(0, "Folder.png");
            this.DefaultIcons.Images.SetKeyName(1, "File.png");
            // 
            // PromptLabel
            // 
            this.PromptLabel.AutoSize = true;
            this.PromptLabel.Location = new System.Drawing.Point(12, 9);
            this.PromptLabel.Name = "PromptLabel";
            this.PromptLabel.Size = new System.Drawing.Size(35, 13);
            this.PromptLabel.TabIndex = 3;
            this.PromptLabel.Text = "label1";
            // 
            // CheckImageList
            // 
            this.CheckImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("CheckImageList.ImageStream")));
            this.CheckImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.CheckImageList.Images.SetKeyName(0, "CheckStateFalse.png");
            this.CheckImageList.Images.SetKeyName(1, "CheckStateTrue.png");
            this.CheckImageList.Images.SetKeyName(2, "CheckStateIntermediate.png");
            // 
            // ItemsTview
            // 
            this.ItemsTview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsTview.CheckBoxes = true;
            this.ItemsTview.HideSelection = false;
            this.ItemsTview.ImageIndex = 0;
            this.ItemsTview.ImageList = this.DefaultIcons;
            this.ItemsTview.ItemHeight = 18;
            this.ItemsTview.Location = new System.Drawing.Point(12, 25);
            this.ItemsTview.Name = "ItemsTview";
            this.ItemsTview.SelectedImageIndex = 0;
            this.ItemsTview.Size = new System.Drawing.Size(367, 326);
            this.ItemsTview.StateImageList = this.CheckImageList;
            this.ItemsTview.TabIndex = 2;
            this.ItemsTview.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.ItemsTview_BeforeExpand);
            // 
            // TreeBrowseDlg
            // 
            this.AcceptButton = this.OkBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(391, 392);
            this.Controls.Add(this.PromptLabel);
            this.Controls.Add(this.ItemsTview);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.OkBtn);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TreeBrowseDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TreeBrowseDlg";
            this.Load += new System.EventHandler(this.TreeBrowseDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label PromptLabel;
        private System.Windows.Forms.ImageList DefaultIcons;
        private System.Windows.Forms.ImageList CheckImageList;
        private TriStateTreeView ItemsTview;
    }
}