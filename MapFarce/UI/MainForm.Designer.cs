﻿namespace MapFarce.UI
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStrip = new System.Windows.Forms.ToolStrip();
            this.btnAddInput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnAddOutput = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnTestRead = new System.Windows.Forms.ToolStripButton();
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
            this.menuStrip1.Size = new System.Drawing.Size(589, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newProjectToolStripMenuItem,
            this.openProjectToolStripMenuItem,
            this.saveProjectToolStripMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // newProjectToolStripMenuItem
            // 
            this.newProjectToolStripMenuItem.Name = "newProjectToolStripMenuItem";
            this.newProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newProjectToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.newProjectToolStripMenuItem.Text = "&New project";
            this.newProjectToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // openProjectToolStripMenuItem
            // 
            this.openProjectToolStripMenuItem.Name = "openProjectToolStripMenuItem";
            this.openProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openProjectToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.openProjectToolStripMenuItem.Text = "&Open project...";
            // 
            // saveProjectToolStripMenuItem
            // 
            this.saveProjectToolStripMenuItem.Name = "saveProjectToolStripMenuItem";
            this.saveProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveProjectToolStripMenuItem.Size = new System.Drawing.Size(195, 22);
            this.saveProjectToolStripMenuItem.Text = "&Save project";
            // 
            // projectToolStrip
            // 
            this.projectToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddInput,
            this.btnAddOutput,
            this.btnTestRead});
            this.projectToolStrip.Location = new System.Drawing.Point(0, 24);
            this.projectToolStrip.Name = "projectToolStrip";
            this.projectToolStrip.Size = new System.Drawing.Size(589, 25);
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
            // projectPanel
            // 
            this.projectPanel.AutoScroll = true;
            this.projectPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectPanel.HasChanges = false;
            this.projectPanel.Location = new System.Drawing.Point(0, 24);
            this.projectPanel.Name = "projectPanel";
            this.projectPanel.Size = new System.Drawing.Size(589, 303);
            this.projectPanel.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 327);
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
        private System.Windows.Forms.ToolStripButton btnTestRead;
    }
}

