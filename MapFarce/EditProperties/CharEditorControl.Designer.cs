namespace MapFarce.EditProperties
{
    partial class CharEditorControl
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
            this.comboCharType = new System.Windows.Forms.ComboBox();
            this.textBoxCustomChar = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // comboCharType
            // 
            this.comboCharType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCharType.FormattingEnabled = true;
            this.comboCharType.Location = new System.Drawing.Point(0, 0);
            this.comboCharType.Name = "comboCharType";
            this.comboCharType.Size = new System.Drawing.Size(121, 21);
            this.comboCharType.TabIndex = 0;
            this.comboCharType.SelectedValueChanged += new System.EventHandler(this.comboCharType_SelectedValueChanged);
            // 
            // textBoxCustomChar
            // 
            this.textBoxCustomChar.Enabled = false;
            this.textBoxCustomChar.Location = new System.Drawing.Point(127, 0);
            this.textBoxCustomChar.MaxLength = 1;
            this.textBoxCustomChar.Name = "textBoxCustomChar";
            this.textBoxCustomChar.Size = new System.Drawing.Size(42, 20);
            this.textBoxCustomChar.TabIndex = 1;
            // 
            // CharEditorControl
            // 
            this.Controls.Add(this.textBoxCustomChar);
            this.Controls.Add(this.comboCharType);
            this.Name = "CharEditorControl";
            this.Size = new System.Drawing.Size(173, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboCharType;
        private System.Windows.Forms.TextBox textBoxCustomChar;
    }
}
