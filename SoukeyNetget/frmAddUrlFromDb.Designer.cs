namespace MinerSpider
{
    partial class frmAddUrlFromDb
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddUrlFromDb));
            this.raExportMySql = new System.Windows.Forms.RadioButton();
            this.raExportMSSQL = new System.Windows.Forms.RadioButton();
            this.button12 = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.raExportAccess = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // raExportMySql
            // 
            this.raExportMySql.AutoSize = true;
            this.raExportMySql.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raExportMySql.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportMySql.Location = new System.Drawing.Point(112, 12);
            this.raExportMySql.Name = "raExportMySql";
            this.raExportMySql.Size = new System.Drawing.Size(52, 16);
            this.raExportMySql.TabIndex = 9;
            this.raExportMySql.Text = "MySql";
            this.raExportMySql.UseVisualStyleBackColor = true;
            // 
            // raExportMSSQL
            // 
            this.raExportMSSQL.AutoSize = true;
            this.raExportMSSQL.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raExportMSSQL.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportMSSQL.Location = new System.Drawing.Point(12, 12);
            this.raExportMSSQL.Name = "raExportMSSQL";
            this.raExportMSSQL.Size = new System.Drawing.Size(94, 16);
            this.raExportMSSQL.TabIndex = 8;
            this.raExportMSSQL.Text = "MS SqlServer";
            this.raExportMSSQL.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button12.Image = ((System.Drawing.Image)(resources.GetObject("button12.Image")));
            this.button12.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button12.Location = new System.Drawing.Point(372, 36);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(73, 21);
            this.button12.TabIndex = 11;
            this.button12.Text = "设   置";
            this.button12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // txtDataSource
            // 
            this.txtDataSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDataSource.Location = new System.Drawing.Point(12, 36);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(361, 21);
            this.txtDataSource.TabIndex = 10;
            // 
            // txtSql
            // 
            this.txtSql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSql.Location = new System.Drawing.Point(12, 87);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.Size = new System.Drawing.Size(433, 84);
            this.txtSql.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "获取网址的Sql语句";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(12, 183);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(239, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "系统将在任务每次运行前进行网址的获取";
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(380, 179);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 21);
            this.cmdCancel.TabIndex = 72;
            this.cmdCancel.Text = "取  消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Image = ((System.Drawing.Image)(resources.GetObject("cmdOK.Image")));
            this.cmdOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(309, 179);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 21);
            this.cmdOK.TabIndex = 71;
            this.cmdOK.Text = "确  定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(122, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(320, 12);
            this.label3.TabIndex = 73;
            this.label3.Text = "必须是Select，返回的信息只能有一个，且必须是网址";
            // 
            // raExportAccess
            // 
            this.raExportAccess.AutoSize = true;
            this.raExportAccess.Checked = true;
            this.raExportAccess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raExportAccess.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raExportAccess.Location = new System.Drawing.Point(145, 184);
            this.raExportAccess.Name = "raExportAccess";
            this.raExportAccess.Size = new System.Drawing.Size(94, 16);
            this.raExportAccess.TabIndex = 7;
            this.raExportAccess.TabStop = true;
            this.raExportAccess.Text = "发布到Access";
            this.raExportAccess.UseVisualStyleBackColor = true;
            this.raExportAccess.Visible = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.radioButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.radioButton1.Location = new System.Drawing.Point(170, 12);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(58, 16);
            this.radioButton1.TabIndex = 74;
            this.radioButton1.Text = "Oracle";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // frmAddUrlFromDb
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(452, 207);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSql);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.txtDataSource);
            this.Controls.Add(this.raExportMySql);
            this.Controls.Add(this.raExportMSSQL);
            this.Controls.Add(this.raExportAccess);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddUrlFromDb";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "从数据库获取网址";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton raExportMySql;
        private System.Windows.Forms.RadioButton raExportMSSQL;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton raExportAccess;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}