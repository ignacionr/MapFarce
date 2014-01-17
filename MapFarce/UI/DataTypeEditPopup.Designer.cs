namespace MapFarce.UI
{
    partial class DataTypeEditPopup
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
            this.ColDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFieldType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lnkMoveUp = new System.Windows.Forms.LinkLabel();
            this.lnkMoveDown = new System.Windows.Forms.LinkLabel();
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
            this.ColDisplayName,
            this.colFieldType});
            this.fieldGrid.Location = new System.Drawing.Point(3, 4);
            this.fieldGrid.Name = "fieldGrid";
            this.fieldGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.fieldGrid.Size = new System.Drawing.Size(425, 290);
            this.fieldGrid.TabIndex = 3;
            this.fieldGrid.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.fieldGrid_UserAddedRow);
            this.fieldGrid.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.fieldGrid_UserDeletedRow);
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
            // ColDisplayName
            // 
            this.ColDisplayName.HeaderText = "Display Name";
            this.ColDisplayName.MaxInputLength = 200;
            this.ColDisplayName.Name = "ColDisplayName";
            this.ColDisplayName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.ColDisplayName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.ColDisplayName.Width = 140;
            // 
            // colFieldType
            // 
            this.colFieldType.HeaderText = "Type";
            this.colFieldType.Name = "colFieldType";
            this.colFieldType.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colFieldType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(264, 300);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(345, 300);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lnkMoveUp
            // 
            this.lnkMoveUp.AutoSize = true;
            this.lnkMoveUp.Location = new System.Drawing.Point(12, 305);
            this.lnkMoveUp.Name = "lnkMoveUp";
            this.lnkMoveUp.Size = new System.Drawing.Size(49, 13);
            this.lnkMoveUp.TabIndex = 6;
            this.lnkMoveUp.TabStop = true;
            this.lnkMoveUp.Text = "Move up";
            this.lnkMoveUp.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMoveUp_LinkClicked);
            // 
            // lnkMoveDown
            // 
            this.lnkMoveDown.AutoSize = true;
            this.lnkMoveDown.Location = new System.Drawing.Point(67, 305);
            this.lnkMoveDown.Name = "lnkMoveDown";
            this.lnkMoveDown.Size = new System.Drawing.Size(63, 13);
            this.lnkMoveDown.TabIndex = 6;
            this.lnkMoveDown.TabStop = true;
            this.lnkMoveDown.Text = "Move down";
            this.lnkMoveDown.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMoveDown_LinkClicked);
            // 
            // DataTypeEditPopup
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(432, 335);
            this.Controls.Add(this.lnkMoveDown);
            this.Controls.Add(this.lnkMoveUp);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.fieldGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DataTypeEditPopup";
            this.Text = "Edit Data Type";
            ((System.ComponentModel.ISupportInitialize)(this.fieldGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView fieldGrid;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.LinkLabel lnkMoveUp;
        private System.Windows.Forms.LinkLabel lnkMoveDown;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFieldName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColDisplayName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colFieldType;
    }
}