namespace SoukeyPublishManage
{
    partial class frmDBRule
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDBRule));
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.raAccess = new System.Windows.Forms.RadioButton();
            this.raSqlserver = new System.Windows.Forms.RadioButton();
            this.raMySql = new System.Windows.Forms.RadioButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPK = new System.Windows.Forms.TextBox();
            this.IsRepeat = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdEditSql = new System.Windows.Forms.Button();
            this.raUpdate = new System.Windows.Forms.RadioButton();
            this.raSql = new System.Windows.Forms.RadioButton();
            this.cmdInsertPara = new System.Windows.Forms.Button();
            this.raGetLastID = new System.Windows.Forms.RadioButton();
            this.raGetValues = new System.Windows.Forms.RadioButton();
            this.cmdDelSql = new System.Windows.Forms.Button();
            this.cmdAddSql = new System.Windows.Forms.Button();
            this.listSql = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtSql = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.系统时间ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.IsSave = new System.Windows.Forms.TextBox();
            this.txtRemark = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Location = new System.Drawing.Point(86, 12);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(478, 21);
            this.txtName.TabIndex = 3;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "规则名称：";
            // 
            // raAccess
            // 
            this.raAccess.AutoSize = true;
            this.raAccess.Checked = true;
            this.raAccess.Location = new System.Drawing.Point(86, 41);
            this.raAccess.Name = "raAccess";
            this.raAccess.Size = new System.Drawing.Size(59, 16);
            this.raAccess.TabIndex = 4;
            this.raAccess.TabStop = true;
            this.raAccess.Text = "Access";
            this.raAccess.UseVisualStyleBackColor = true;
            this.raAccess.Click += new System.EventHandler(this.raAccess_Click);
            // 
            // raSqlserver
            // 
            this.raSqlserver.AutoSize = true;
            this.raSqlserver.Location = new System.Drawing.Point(161, 41);
            this.raSqlserver.Name = "raSqlserver";
            this.raSqlserver.Size = new System.Drawing.Size(95, 16);
            this.raSqlserver.TabIndex = 5;
            this.raSqlserver.Text = "MS SqlServer";
            this.raSqlserver.UseVisualStyleBackColor = true;
            this.raSqlserver.Click += new System.EventHandler(this.raSqlserver_Click);
            // 
            // raMySql
            // 
            this.raMySql.AutoSize = true;
            this.raMySql.Location = new System.Drawing.Point(271, 41);
            this.raMySql.Name = "raMySql";
            this.raMySql.Size = new System.Drawing.Size(53, 16);
            this.raMySql.TabIndex = 6;
            this.raMySql.Text = "MySql";
            this.raMySql.UseVisualStyleBackColor = true;
            this.raMySql.Click += new System.EventHandler(this.raMySql_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(12, 93);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(574, 327);
            this.tabControl1.TabIndex = 7;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.ImageIndex = 0;
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(566, 297);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "发布配置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPK);
            this.groupBox1.Controls.Add(this.IsRepeat);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmdEditSql);
            this.groupBox1.Controls.Add(this.raUpdate);
            this.groupBox1.Controls.Add(this.raSql);
            this.groupBox1.Controls.Add(this.cmdInsertPara);
            this.groupBox1.Controls.Add(this.raGetLastID);
            this.groupBox1.Controls.Add(this.raGetValues);
            this.groupBox1.Controls.Add(this.cmdDelSql);
            this.groupBox1.Controls.Add(this.cmdAddSql);
            this.groupBox1.Controls.Add(this.listSql);
            this.groupBox1.Controls.Add(this.txtSql);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(1, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(557, 291);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "发布配置";
            // 
            // txtPK
            // 
            this.txtPK.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPK.Enabled = false;
            this.txtPK.Location = new System.Drawing.Point(408, 76);
            this.txtPK.Name = "txtPK";
            this.txtPK.Size = new System.Drawing.Size(76, 21);
            this.txtPK.TabIndex = 41;
            // 
            // IsRepeat
            // 
            this.IsRepeat.AutoSize = true;
            this.IsRepeat.Location = new System.Drawing.Point(79, 78);
            this.IsRepeat.Name = "IsRepeat";
            this.IsRepeat.Size = new System.Drawing.Size(324, 16);
            this.IsRepeat.TabIndex = 40;
            this.IsRepeat.Text = "插入前进行重复性判断，如重复则取消插入并获取主键：";
            this.IsRepeat.UseVisualStyleBackColor = true;
            this.IsRepeat.CheckedChanged += new System.EventHandler(this.IsRepeat_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(14, 252);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(491, 36);
            this.label3.TabIndex = 39;
            this.label3.Text = "注意：\r\n1、数据表必须已经存在，否则会发布失败！\r\n2、sql语句必须合法，系统仅对参数部分进行替换，如果sql语句不合法，会出现发布错误！";
            // 
            // cmdEditSql
            // 
            this.cmdEditSql.Enabled = false;
            this.cmdEditSql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdEditSql.Image = ((System.Drawing.Image)(resources.GetObject("cmdEditSql.Image")));
            this.cmdEditSql.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdEditSql.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdEditSql.Location = new System.Drawing.Point(421, 122);
            this.cmdEditSql.Name = "cmdEditSql";
            this.cmdEditSql.Size = new System.Drawing.Size(60, 22);
            this.cmdEditSql.TabIndex = 38;
            this.cmdEditSql.Text = "修 改";
            this.cmdEditSql.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdEditSql.UseVisualStyleBackColor = true;
            this.cmdEditSql.Click += new System.EventHandler(this.cmdEditSql_Click);
            // 
            // raUpdate
            // 
            this.raUpdate.AutoSize = true;
            this.raUpdate.Location = new System.Drawing.Point(205, 122);
            this.raUpdate.Name = "raUpdate";
            this.raUpdate.Size = new System.Drawing.Size(119, 16);
            this.raUpdate.TabIndex = 37;
            this.raUpdate.Text = "根据主键进行更新";
            this.raUpdate.UseVisualStyleBackColor = true;
            this.raUpdate.Visible = false;
            this.raUpdate.Click += new System.EventHandler(this.raUpdate_Click);
            // 
            // raSql
            // 
            this.raSql.AutoSize = true;
            this.raSql.Checked = true;
            this.raSql.Location = new System.Drawing.Point(80, 100);
            this.raSql.Name = "raSql";
            this.raSql.Size = new System.Drawing.Size(77, 16);
            this.raSql.TabIndex = 36;
            this.raSql.TabStop = true;
            this.raSql.Text = "仅执行sql";
            this.raSql.UseVisualStyleBackColor = true;
            this.raSql.Click += new System.EventHandler(this.raSql_Click);
            // 
            // cmdInsertPara
            // 
            this.cmdInsertPara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdInsertPara.Location = new System.Drawing.Point(501, 18);
            this.cmdInsertPara.Name = "cmdInsertPara";
            this.cmdInsertPara.Size = new System.Drawing.Size(46, 56);
            this.cmdInsertPara.TabIndex = 35;
            this.cmdInsertPara.Text = "插 入参 数";
            this.cmdInsertPara.UseVisualStyleBackColor = true;
            this.cmdInsertPara.Click += new System.EventHandler(this.cmdInsertPara_Click);
            // 
            // raGetLastID
            // 
            this.raGetLastID.AutoSize = true;
            this.raGetLastID.Location = new System.Drawing.Point(80, 122);
            this.raGetLastID.Name = "raGetLastID";
            this.raGetLastID.Size = new System.Drawing.Size(119, 16);
            this.raGetLastID.TabIndex = 34;
            this.raGetLastID.Text = "获取插入记录的ID";
            this.raGetLastID.UseVisualStyleBackColor = true;
            this.raGetLastID.Click += new System.EventHandler(this.raGetLastID_Click);
            // 
            // raGetValues
            // 
            this.raGetValues.AutoSize = true;
            this.raGetValues.Location = new System.Drawing.Point(163, 100);
            this.raGetValues.Name = "raGetValues";
            this.raGetValues.Size = new System.Drawing.Size(83, 16);
            this.raGetValues.TabIndex = 33;
            this.raGetValues.Text = "获取返回值";
            this.raGetValues.UseVisualStyleBackColor = true;
            this.raGetValues.Click += new System.EventHandler(this.raGetValues_Click);
            // 
            // cmdDelSql
            // 
            this.cmdDelSql.Enabled = false;
            this.cmdDelSql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDelSql.Image = ((System.Drawing.Image)(resources.GetObject("cmdDelSql.Image")));
            this.cmdDelSql.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelSql.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDelSql.Location = new System.Drawing.Point(487, 122);
            this.cmdDelSql.Name = "cmdDelSql";
            this.cmdDelSql.Size = new System.Drawing.Size(60, 22);
            this.cmdDelSql.TabIndex = 32;
            this.cmdDelSql.Text = "删 除";
            this.cmdDelSql.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelSql.UseVisualStyleBackColor = true;
            this.cmdDelSql.Click += new System.EventHandler(this.cmdDelSql_Click);
            // 
            // cmdAddSql
            // 
            this.cmdAddSql.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAddSql.Image = ((System.Drawing.Image)(resources.GetObject("cmdAddSql.Image")));
            this.cmdAddSql.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAddSql.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdAddSql.Location = new System.Drawing.Point(355, 122);
            this.cmdAddSql.Name = "cmdAddSql";
            this.cmdAddSql.Size = new System.Drawing.Size(60, 22);
            this.cmdAddSql.TabIndex = 31;
            this.cmdAddSql.Text = "增 加";
            this.cmdAddSql.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAddSql.UseVisualStyleBackColor = true;
            this.cmdAddSql.Click += new System.EventHandler(this.cmdAddSql_Click);
            // 
            // listSql
            // 
            this.listSql.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader2,
            this.columnHeader3});
            this.listSql.FullRowSelect = true;
            this.listSql.GridLines = true;
            this.listSql.HideSelection = false;
            this.listSql.Location = new System.Drawing.Point(78, 150);
            this.listSql.Name = "listSql";
            this.listSql.Size = new System.Drawing.Size(469, 99);
            this.listSql.TabIndex = 30;
            this.listSql.UseCompatibleStateImageBehavior = false;
            this.listSql.View = System.Windows.Forms.View.Details;
            this.listSql.Click += new System.EventHandler(this.listSql_Click);
            this.listSql.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listSql_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "步骤";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "是否判断重复";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "主键";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "类型";
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "sql";
            this.columnHeader3.Width = 280;
            // 
            // txtSql
            // 
            this.txtSql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSql.Location = new System.Drawing.Point(79, 18);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSql.Size = new System.Drawing.Size(423, 56);
            this.txtSql.TabIndex = 29;
            this.txtSql.TextChanged += new System.EventHandler(this.txtSql_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 28;
            this.label2.Text = "sql语句：";
            // 
            // cmdApply
            // 
            this.cmdApply.Enabled = false;
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Image = ((System.Drawing.Image)(resources.GetObject("cmdApply.Image")));
            this.cmdApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdApply.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdApply.Location = new System.Drawing.Point(447, 424);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(118, 22);
            this.cmdApply.TabIndex = 10;
            this.cmdApply.Text = "保 存 (Ctrl+S)";
            this.cmdApply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(376, 424);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 9;
            this.cmdCancel.Text = "取 消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Image = ((System.Drawing.Image)(resources.GetObject("cmdOK.Image")));
            this.cmdOK.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(301, 424);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 22);
            this.cmdOK.TabIndex = 8;
            this.cmdOK.Text = "确 定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.系统时间ToolStripMenuItem,
            this.toolStripSeparator1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 32);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // 系统时间ToolStripMenuItem
            // 
            this.系统时间ToolStripMenuItem.Name = "系统时间ToolStripMenuItem";
            this.系统时间ToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.系统时间ToolStripMenuItem.Text = "{系统参数:当前时间}";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // IsSave
            // 
            this.IsSave.Location = new System.Drawing.Point(33, 426);
            this.IsSave.Name = "IsSave";
            this.IsSave.Size = new System.Drawing.Size(100, 21);
            this.IsSave.TabIndex = 11;
            this.IsSave.Visible = false;
            this.IsSave.TextChanged += new System.EventHandler(this.IsSave_TextChanged);
            // 
            // txtRemark
            // 
            this.txtRemark.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRemark.Location = new System.Drawing.Point(86, 63);
            this.txtRemark.Name = "txtRemark";
            this.txtRemark.Size = new System.Drawing.Size(478, 21);
            this.txtRemark.TabIndex = 13;
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(38, 67);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(41, 12);
            this.label24.TabIndex = 12;
            this.label24.Text = "备注：";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A45.gif");
            this.imageList1.Images.SetKeyName(1, "A22.png");
            // 
            // frmDBRule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 458);
            this.Controls.Add(this.txtRemark);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.IsSave);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.raMySql);
            this.Controls.Add(this.raSqlserver);
            this.Controls.Add(this.raAccess);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDBRule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据库发布模版";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDBRule_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDBRule_FormClosed);
            this.Load += new System.EventHandler(this.frmDBRule_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton raAccess;
        private System.Windows.Forms.RadioButton raSqlserver;
        private System.Windows.Forms.RadioButton raMySql;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton raGetLastID;
        private System.Windows.Forms.RadioButton raGetValues;
        private System.Windows.Forms.Button cmdDelSql;
        private System.Windows.Forms.Button cmdAddSql;
        private System.Windows.Forms.ListView listSql;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdInsertPara;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.RadioButton raSql;
        private System.Windows.Forms.RadioButton raUpdate;
        private System.Windows.Forms.ToolStripMenuItem 系统时间ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button cmdEditSql;
        private System.Windows.Forms.TextBox IsSave;
        private System.Windows.Forms.TextBox txtRemark;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.CheckBox IsRepeat;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.TextBox txtPK;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ImageList imageList1;
    }
}