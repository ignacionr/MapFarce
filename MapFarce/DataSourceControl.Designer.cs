namespace MapFarce
{
    partial class DataSourceControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblName = new System.Windows.Forms.Label();
            this.lnkEdit = new System.Windows.Forms.LinkLabel();
            this.treeView = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name";
            // 
            // lnkEdit
            // 
            this.lnkEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkEdit.AutoSize = true;
            this.lnkEdit.Location = new System.Drawing.Point(121, 3);
            this.lnkEdit.Name = "lnkEdit";
            this.lnkEdit.Size = new System.Drawing.Size(24, 13);
            this.lnkEdit.TabIndex = 1;
            this.lnkEdit.TabStop = true;
            this.lnkEdit.Text = "edit";
            this.lnkEdit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkEdit_LinkClicked);
            // 
            // treeView
            // 
            this.treeView.FullRowSelect = true;
            this.treeView.Location = new System.Drawing.Point(-1, 19);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(150, 130);
            this.treeView.TabIndex = 2;
            // 
            // DataSourceControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.lnkEdit);
            this.Controls.Add(this.lblName);
            this.Name = "DataSourceControl";
            this.Size = new System.Drawing.Size(148, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.LinkLabel lnkEdit;
        private System.Windows.Forms.TreeView treeView;
    }
}
