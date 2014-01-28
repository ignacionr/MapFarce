namespace MapFarce.UI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddInput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddOutput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddMapping = new System.Windows.Forms.ToolStripButton();
            this.btnRunMappings = new System.Windows.Forms.ToolStripButton();
            this.projectPanel = new MapFarce.UI.ProjectPanel();
            this.menuStrip1.SuspendLayout();
            this.projectToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(670, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem,
            this.saveProjectasToolStripMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.newProjectToolStripMenuItem.Text = "&New Project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.openProjectToolStripMenuItem.Text = "&Open Project...";
            this.openProjectToolStripMenuItem.Click += new System.EventHandler(this.openProjectToolStripMenuItem_Click);
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.saveProjectToolStripMenuItem.Text = "&Save Project";
            this.saveProjectToolStripMenuItem.Click += new System.EventHandler(this.saveProjectToolStripMenuItem_Click);
            // 
            // saveProjectasToolStripMenuItem
            // 
            this.saveProjectasToolStripMenuItem.Name = "saveProjectasToolStripMenuItem";
            this.saveProjectasToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.saveProjectasToolStripMenuItem.Size = new System.Drawing.Size(235, 22);
            this.saveProjectasToolStripMenuItem.Text = "Save Project &As...";
            this.saveProjectasToolStripMenuItem.Click += new System.EventHandler(this.saveProjectasToolStripMenuItem_Click);
            // 
            // projectToolStrip
            // 
            this.projectToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddInput,
            this.btnAddOutput,
            this.btnAddMapping,
            this.btnRunMappings});
            this.projectToolStrip.Location = new System.Drawing.Point(0, 24);
            this.projectToolStrip.Name = "projectToolStrip";
            this.projectToolStrip.Size = new System.Drawing.Size(670, 25);
            this.projectToolStrip.TabIndex = 6;
            this.projectToolStrip.Text = "toolStrip1";
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
            // btnAddMapping
            // 
            this.btnAddMapping.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAddMapping.Image = ((System.Drawing.Image)(resources.GetObject("btnAddMapping.Image")));
            this.btnAddMapping.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddMapping.Name = "btnAddMapping";
            this.btnAddMapping.Size = new System.Drawing.Size(84, 22);
            this.btnAddMapping.Text = "Add &mapping";
            this.btnAddMapping.Click += new System.EventHandler(this.btnAddMapping_Click);
            // 
            // btnRunMappings
            // 
            this.btnRunMappings.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRunMappings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRunMappings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRunMappings.Name = "btnRunMappings";
            this.btnRunMappings.Size = new System.Drawing.Size(96, 22);
            this.btnRunMappings.Text = "&Run mapping(s)";
            this.btnRunMappings.Click += new System.EventHandler(this.btnRunMappings_Click);
            // 
            // projectPanel
            // 
            this.projectPanel.AutoScroll = true;
            this.projectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectPanel.Location = new System.Drawing.Point(0, 24);
            this.projectPanel.Name = "projectPanel";
            this.projectPanel.Size = new System.Drawing.Size(670, 303);
            this.projectPanel.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 327);
            this.Controls.Add(this.projectToolStrip);
            this.Controls.Add(this.projectPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MapFarce";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.projectToolStrip.ResumeLayout(false);
            this.projectToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveProjectToolStripMenuItem;
        private ProjectPanel projectPanel;
        private System.Windows.Forms.ToolStrip projectToolStrip;
        private System.Windows.Forms.ToolStripDropDownButton btnAddInput;
        private System.Windows.Forms.ToolStripDropDownButton btnAddOutput;
        private System.Windows.Forms.ToolStripMenuItem saveProjectasToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnAddMapping;
        private System.Windows.Forms.ToolStripButton btnRunMappings;
    }
}

