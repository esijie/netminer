namespace MinerSpider
{
    partial class frmAddThreadProxy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddThreadProxy));
            this.threadDataGrid = new System.Windows.Forms.DataGridView();
            this.threadIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProxyType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.pAdd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pPort = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.threadDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // threadDataGrid
            // 
            this.threadDataGrid.AllowUserToAddRows = false;
            this.threadDataGrid.BackgroundColor = System.Drawing.Color.White;
            this.threadDataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.threadDataGrid.ColumnHeadersHeight = 21;
            this.threadDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.threadDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.threadIndex,
            this.ProxyType,
            this.pAdd,
            this.pPort});
            this.threadDataGrid.Location = new System.Drawing.Point(12, 12);
            this.threadDataGrid.Name = "threadDataGrid";
            this.threadDataGrid.RowHeadersWidth = 21;
            this.threadDataGrid.RowTemplate.Height = 23;
            this.threadDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.threadDataGrid.Size = new System.Drawing.Size(475, 186);
            this.threadDataGrid.TabIndex = 3;
            // 
            // threadIndex
            // 
            this.threadIndex.FillWeight = 40F;
            this.threadIndex.HeaderText = "线程";
            this.threadIndex.Name = "threadIndex";
            this.threadIndex.ReadOnly = true;
            this.threadIndex.Width = 40;
            // 
            // ProxyType
            // 
            this.ProxyType.FillWeight = 80F;
            this.ProxyType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ProxyType.HeaderText = "代理类型";
            this.ProxyType.Name = "ProxyType";
            this.ProxyType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ProxyType.Width = 160;
            // 
            // pAdd
            // 
            this.pAdd.FillWeight = 80F;
            this.pAdd.HeaderText = "地址";
            this.pAdd.Name = "pAdd";
            this.pAdd.Width = 160;
            // 
            // pPort
            // 
            this.pPort.HeaderText = "端口";
            this.pPort.Name = "pPort";
            this.pPort.Width = 80;
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(422, 218);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 21;
            this.cmdCancel.Text = "取 消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdApply
            // 
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Image = ((System.Drawing.Image)(resources.GetObject("cmdApply.Image")));
            this.cmdApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdApply.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdApply.Location = new System.Drawing.Point(349, 218);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(65, 22);
            this.cmdApply.TabIndex = 20;
            this.cmdApply.Text = "确 定";
            this.cmdApply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(2, 204);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(498, 8);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(10, 221);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(345, 19);
            this.label1.TabIndex = 23;
            this.label1.Text = "此方法适用稳定的代理服务，否则请使用代理IP轮询机制。";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.FillWeight = 40F;
            this.dataGridViewTextBoxColumn1.HeaderText = "线程";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 40;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.FillWeight = 80F;
            this.dataGridViewComboBoxColumn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dataGridViewComboBoxColumn1.HeaderText = "代理类型";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewComboBoxColumn1.Width = 120;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.FillWeight = 80F;
            this.dataGridViewTextBoxColumn2.HeaderText = "地址";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 120;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "端口";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 80;
            // 
            // frmAddThreadProxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.threadDataGrid);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddThreadProxy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "为每个线程设置独立的代理";
            this.Load += new System.EventHandler(this.frmAddThreadProxy_Load);
            ((System.ComponentModel.ISupportInitialize)(this.threadDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView threadDataGrid;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn threadIndex;
        private System.Windows.Forms.DataGridViewComboBoxColumn ProxyType;
        private System.Windows.Forms.DataGridViewTextBoxColumn pAdd;
        private System.Windows.Forms.DataGridViewTextBoxColumn pPort;
    }
}