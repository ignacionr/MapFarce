namespace MapFarce.UI
{
    partial class MappingControl
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
            this.lnkAddOutput = new System.Windows.Forms.LinkLabel();
            this.lnkAddInput = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblName.Location = new System.Drawing.Point(-1, -1);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(150, 48);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Some mapping";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lnkAddOutput
            // 
            this.lnkAddOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkAddOutput.AutoSize = true;
            this.lnkAddOutput.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkAddOutput.Location = new System.Drawing.Point(132, 17);
            this.lnkAddOutput.Name = "lnkAddOutput";
            this.lnkAddOutput.Size = new System.Drawing.Size(13, 13);
            this.lnkAddOutput.TabIndex = 1;
            this.lnkAddOutput.TabStop = true;
            this.lnkAddOutput.Text = "+";
            this.lnkAddOutput.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddOutput_LinkClicked);
            // 
            // lnkAddInput
            // 
            this.lnkAddInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkAddInput.AutoSize = true;
            this.lnkAddInput.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.lnkAddInput.Location = new System.Drawing.Point(3, 17);
            this.lnkAddInput.Name = "lnkAddInput";
            this.lnkAddInput.Size = new System.Drawing.Size(13, 13);
            this.lnkAddInput.TabIndex = 1;
            this.lnkAddInput.TabStop = true;
            this.lnkAddInput.Text = "+";
            this.lnkAddInput.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkAddInput_LinkClicked);
            // 
            // MappingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lnkAddInput);
            this.Controls.Add(this.lnkAddOutput);
            this.Controls.Add(this.lblName);
            this.Name = "MappingControl";
            this.Size = new System.Drawing.Size(148, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblName;
        private System.Windows.Forms.LinkLabel lnkAddOutput;
        private System.Windows.Forms.LinkLabel lnkAddInput;

    }
}
