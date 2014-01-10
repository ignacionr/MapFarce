namespace MapFarce.UI
{
    partial class ProjectPanel
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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddInput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddOutput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnTestRead = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddInput,
            this.btnAddOutput,
            this.btnTestRead});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(551, 25);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Text = "toolStrip1";
            // 
            // btnAddInput
            // 
            this.btnAddInput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddInput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddInput.Name = "btnAddInput";
            this.btnAddInput.Size = new System.Drawing.Size(73, 22);
            this.btnAddInput.Text = "Add &input";
            // 
            // btnAddOutput
            // 
            this.btnAddOutput.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddOutput.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddOutput.Name = "btnAddOutput";
            this.btnAddOutput.Size = new System.Drawing.Size(81, 22);
            this.btnAddOutput.Text = "Add &output";
            // 
            // btnTestRead
            // 
            this.btnTestRead.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTestRead.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTestRead.Margin = new System.Windows.Forms.Padding(20, 1, 0, 2);
            this.btnTestRead.Name = "btnTestRead";
            this.btnTestRead.Size = new System.Drawing.Size(59, 22);
            this.btnTestRead.Text = "Test read";
            this.btnTestRead.Click += new System.EventHandler(this.btnTestRead_Click);
            // 
            // ProjectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.toolStrip);
            this.Name = "ProjectPanel";
            this.Size = new System.Drawing.Size(551, 291);
            this.Load += new System.EventHandler(this.ProjectPanel_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripDropDownButton btnAddInput;
        private System.Windows.Forms.ToolStripDropDownButton btnAddOutput;
        private System.Windows.Forms.ToolStripButton btnTestRead;
    }
}
