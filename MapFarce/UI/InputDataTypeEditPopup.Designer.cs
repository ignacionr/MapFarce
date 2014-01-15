namespace MapFarce.UI
{
    partial class InputDataTypeEditPopup
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
            this.fieldGrid = new System.Windows.Forms.DataGridView();
            this.colFieldName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFieldType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.fieldGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // fieldGrid
            // 
            this.fieldGrid.AllowUserToResizeColumns = false;
            this.fieldGrid.AllowUserToResizeRows = false;
            this.fieldGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.fieldGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFieldName,
            this.colFieldType});
            this.fieldGrid.Location = new System.Drawing.Point(4, 83);
            this.fieldGrid.Name = "fieldGrid";
            this.fieldGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.fieldGrid.Size = new System.Drawing.Size(320, 168);
            this.fieldGrid.TabIndex = 3;
            // 
            // colFieldName
            // 
            this.colFieldName.HeaderText = "Name";
            this.colFieldName.MaxInputLength = 200;
            this.colFieldName.Name = "colFieldName";
            this.colFieldName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colFieldName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.colFieldName.Width = 140;
            // 
            // colFieldType
            // 
            this.colFieldType.HeaderText = "Type";
            this.colFieldType.Name = "colFieldType";
            this.colFieldType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colFieldType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // InputDataTypeEditPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 335);
            this.Controls.Add(this.fieldGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InputDataTypeEditPopup";
            this.Text = "Edit Data Type";
            ((System.ComponentModel.ISupportInitialize)(this.fieldGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView fieldGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colFieldType;
    }
}