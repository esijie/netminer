namespace MinerSpider
{
    partial class frmRader
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRader));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSource = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listRange = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdDelTask = new System.Windows.Forms.Button();
            this.cmdAddTask = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.tabRule = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtRuleContent = new System.Windows.Forms.TextBox();
            this.cmdDelRule = new System.Windows.Forms.Button();
            this.cmdAddRule = new System.Windows.Forms.Button();
            this.RuleNum = new System.Windows.Forms.NumericUpDown();
            this.comRule = new System.Windows.Forms.ComboBox();
            this.comDataSource = new System.Windows.Forms.ComboBox();
            this.listRule = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.tabOP = new System.Windows.Forms.TabPage();
            this.groupEmail = new System.Windows.Forms.GroupBox();
            this.txtDealEmail = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupPage = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupData = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtRadarDb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.comTableName = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.raSendEmail = new System.Windows.Forms.RadioButton();
            this.raSaveUrl = new System.Windows.Forms.RadioButton();
            this.raSavePage = new System.Windows.Forms.RadioButton();
            this.raNoDeal = new System.Windows.Forms.RadioButton();
            this.tabAdvance = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.raTmpInvalid = new System.Windows.Forms.RadioButton();
            this.raInvalid = new System.Windows.Forms.RadioButton();
            this.InvalidDate = new System.Windows.Forms.DateTimePicker();
            this.raInvalidByDate = new System.Windows.Forms.RadioButton();
            this.raNoInvalid = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtWarningEmail = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.raWarningEmail = new System.Windows.Forms.RadioButton();
            this.raWarningTrayIcon = new System.Windows.Forms.RadioButton();
            this.raNoWarning = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.RuleName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.IsSave = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.leToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.采集任务TaskNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.urlUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.监控时间MonitorDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label13 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.tabSource.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabRule.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RuleNum)).BeginInit();
            this.tabOP.SuspendLayout();
            this.groupEmail.SuspendLayout();
            this.groupPage.SuspendLayout();
            this.groupData.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabAdvance.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabSource);
            this.tabControl1.Controls.Add(this.tabRule);
            this.tabControl1.Controls.Add(this.tabOP);
            this.tabControl1.Controls.Add(this.tabAdvance);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(12, 48);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(588, 378);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSource
            // 
            this.tabSource.Controls.Add(this.groupBox2);
            this.tabSource.ImageIndex = 0;
            this.tabSource.Location = new System.Drawing.Point(4, 26);
            this.tabSource.Name = "tabSource";
            this.tabSource.Padding = new System.Windows.Forms.Padding(3);
            this.tabSource.Size = new System.Drawing.Size(580, 348);
            this.tabSource.TabIndex = 0;
            this.tabSource.Text = "监控源";
            this.tabSource.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listRange);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmdDelTask);
            this.groupBox2.Controls.Add(this.cmdAddTask);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.listTask);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(573, 339);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "监控源设置";
            // 
            // listRange
            // 
            this.listRange.Location = new System.Drawing.Point(4, 225);
            this.listRange.Name = "listRange";
            this.listRange.Size = new System.Drawing.Size(563, 108);
            this.listRange.SmallImageList = this.imageList1;
            this.listRange.TabIndex = 19;
            this.listRange.UseCompatibleStateImageBehavior = false;
            this.listRange.View = System.Windows.Forms.View.List;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A52.gif");
            this.imageList1.Images.SetKeyName(1, "A36.gif");
            this.imageList1.Images.SetKeyName(2, "ToolboxWindow.ico");
            this.imageList1.Images.SetKeyName(3, "A54.gif");
            this.imageList1.Images.SetKeyName(4, "A43.gif");
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Red;
            this.label4.Location = new System.Drawing.Point(67, 198);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(199, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "监控范围由您添加的监控源自动获取";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "监控范围";
            // 
            // cmdDelTask
            // 
            this.cmdDelTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDelTask.Image = ((System.Drawing.Image)(resources.GetObject("cmdDelTask.Image")));
            this.cmdDelTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelTask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDelTask.Location = new System.Drawing.Point(507, 32);
            this.cmdDelTask.Name = "cmdDelTask";
            this.cmdDelTask.Size = new System.Drawing.Size(60, 23);
            this.cmdDelTask.TabIndex = 16;
            this.cmdDelTask.Text = "删  除";
            this.cmdDelTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelTask.UseVisualStyleBackColor = true;
            this.cmdDelTask.Click += new System.EventHandler(this.cmdDelTask_Click);
            // 
            // cmdAddTask
            // 
            this.cmdAddTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAddTask.Image = ((System.Drawing.Image)(resources.GetObject("cmdAddTask.Image")));
            this.cmdAddTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAddTask.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdAddTask.Location = new System.Drawing.Point(435, 32);
            this.cmdAddTask.Name = "cmdAddTask";
            this.cmdAddTask.Size = new System.Drawing.Size(60, 23);
            this.cmdAddTask.TabIndex = 15;
            this.cmdAddTask.Text = "添  加";
            this.cmdAddTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAddTask.UseVisualStyleBackColor = true;
            this.cmdAddTask.Click += new System.EventHandler(this.cmdAddTask_Click_1);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.Red;
            this.label3.Location = new System.Drawing.Point(6, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(423, 39);
            this.label3.TabIndex = 14;
            this.label3.Text = "如果选择多个采集任务，则仅对相同的采集数据名称进行监控。如果多个任务，没有相同的采集数据名称，则无法实现监控。";
            // 
            // listTask
            // 
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader1,
            this.columnHeader8});
            this.listTask.FullRowSelect = true;
            this.listTask.Location = new System.Drawing.Point(6, 61);
            this.listTask.MultiSelect = false;
            this.listTask.Name = "listTask";
            this.listTask.Size = new System.Drawing.Size(561, 125);
            this.listTask.TabIndex = 13;
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            this.listTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listTask_KeyDown);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "分类";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "任务名称";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "监控内容";
            this.columnHeader8.Width = 260;
            // 
            // tabRule
            // 
            this.tabRule.Controls.Add(this.groupBox3);
            this.tabRule.ImageIndex = 1;
            this.tabRule.Location = new System.Drawing.Point(4, 26);
            this.tabRule.Name = "tabRule";
            this.tabRule.Padding = new System.Windows.Forms.Padding(3);
            this.tabRule.Size = new System.Drawing.Size(580, 348);
            this.tabRule.TabIndex = 1;
            this.tabRule.Text = "监控规则";
            this.tabRule.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.txtRuleContent);
            this.groupBox3.Controls.Add(this.cmdDelRule);
            this.groupBox3.Controls.Add(this.cmdAddRule);
            this.groupBox3.Controls.Add(this.RuleNum);
            this.groupBox3.Controls.Add(this.comRule);
            this.groupBox3.Controls.Add(this.comDataSource);
            this.groupBox3.Controls.Add(this.listRule);
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(573, 342);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "监控规则";
            // 
            // txtRuleContent
            // 
            this.txtRuleContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRuleContent.Location = new System.Drawing.Point(322, 15);
            this.txtRuleContent.Name = "txtRuleContent";
            this.txtRuleContent.Size = new System.Drawing.Size(148, 20);
            this.txtRuleContent.TabIndex = 23;
            // 
            // cmdDelRule
            // 
            this.cmdDelRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDelRule.Image = ((System.Drawing.Image)(resources.GetObject("cmdDelRule.Image")));
            this.cmdDelRule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelRule.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDelRule.Location = new System.Drawing.Point(500, 42);
            this.cmdDelRule.Name = "cmdDelRule";
            this.cmdDelRule.Size = new System.Drawing.Size(60, 23);
            this.cmdDelRule.TabIndex = 18;
            this.cmdDelRule.Text = "删  除";
            this.cmdDelRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelRule.UseVisualStyleBackColor = true;
            this.cmdDelRule.Click += new System.EventHandler(this.cmdDelRule_Click);
            // 
            // cmdAddRule
            // 
            this.cmdAddRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAddRule.Image = ((System.Drawing.Image)(resources.GetObject("cmdAddRule.Image")));
            this.cmdAddRule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAddRule.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdAddRule.Location = new System.Drawing.Point(428, 42);
            this.cmdAddRule.Name = "cmdAddRule";
            this.cmdAddRule.Size = new System.Drawing.Size(60, 23);
            this.cmdAddRule.TabIndex = 17;
            this.cmdAddRule.Text = "添  加";
            this.cmdAddRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAddRule.UseVisualStyleBackColor = true;
            this.cmdAddRule.Click += new System.EventHandler(this.cmdAddRule_Click);
            // 
            // RuleNum
            // 
            this.RuleNum.Location = new System.Drawing.Point(481, 16);
            this.RuleNum.Name = "RuleNum";
            this.RuleNum.Size = new System.Drawing.Size(79, 20);
            this.RuleNum.TabIndex = 15;
            // 
            // comRule
            // 
            this.comRule.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comRule.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comRule.FormattingEnabled = true;
            this.comRule.Location = new System.Drawing.Point(160, 15);
            this.comRule.Name = "comRule";
            this.comRule.Size = new System.Drawing.Size(148, 21);
            this.comRule.TabIndex = 14;
            this.comRule.SelectedIndexChanged += new System.EventHandler(this.comRule_SelectedIndexChanged);
            // 
            // comDataSource
            // 
            this.comDataSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comDataSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comDataSource.FormattingEnabled = true;
            this.comDataSource.Location = new System.Drawing.Point(6, 15);
            this.comDataSource.Name = "comDataSource";
            this.comDataSource.Size = new System.Drawing.Size(148, 21);
            this.comDataSource.TabIndex = 13;
            // 
            // listRule
            // 
            this.listRule.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader7});
            this.listRule.FullRowSelect = true;
            this.listRule.Location = new System.Drawing.Point(6, 71);
            this.listRule.Name = "listRule";
            this.listRule.Size = new System.Drawing.Size(554, 265);
            this.listRule.TabIndex = 8;
            this.listRule.UseCompatibleStateImageBehavior = false;
            this.listRule.View = System.Windows.Forms.View.Details;
            this.listRule.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listRule_KeyDown);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "监控项";
            this.columnHeader3.Width = 160;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "规则";
            this.columnHeader4.Width = 120;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "内容";
            this.columnHeader5.Width = 160;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "数量";
            this.columnHeader7.Width = 80;
            // 
            // tabOP
            // 
            this.tabOP.Controls.Add(this.groupEmail);
            this.tabOP.Controls.Add(this.groupPage);
            this.tabOP.Controls.Add(this.groupData);
            this.tabOP.Controls.Add(this.groupBox4);
            this.tabOP.ImageIndex = 4;
            this.tabOP.Location = new System.Drawing.Point(4, 26);
            this.tabOP.Name = "tabOP";
            this.tabOP.Size = new System.Drawing.Size(580, 348);
            this.tabOP.TabIndex = 2;
            this.tabOP.Text = "处理方式";
            this.tabOP.UseVisualStyleBackColor = true;
            // 
            // groupEmail
            // 
            this.groupEmail.Controls.Add(this.txtDealEmail);
            this.groupEmail.Controls.Add(this.label11);
            this.groupEmail.Enabled = false;
            this.groupEmail.Location = new System.Drawing.Point(0, 295);
            this.groupEmail.Name = "groupEmail";
            this.groupEmail.Size = new System.Drawing.Size(573, 49);
            this.groupEmail.TabIndex = 6;
            this.groupEmail.TabStop = false;
            this.groupEmail.Text = "接收信箱";
            // 
            // txtDealEmail
            // 
            this.txtDealEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDealEmail.Location = new System.Drawing.Point(86, 18);
            this.txtDealEmail.Name = "txtDealEmail";
            this.txtDealEmail.Size = new System.Drawing.Size(481, 20);
            this.txtDealEmail.TabIndex = 1;
            this.txtDealEmail.TextChanged += new System.EventHandler(this.txtDealEmail_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "电子信箱：";
            // 
            // groupPage
            // 
            this.groupPage.Controls.Add(this.button3);
            this.groupPage.Controls.Add(this.txtSavePath);
            this.groupPage.Controls.Add(this.label7);
            this.groupPage.Enabled = false;
            this.groupPage.Location = new System.Drawing.Point(0, 241);
            this.groupPage.Name = "groupPage";
            this.groupPage.Size = new System.Drawing.Size(573, 50);
            this.groupPage.TabIndex = 5;
            this.groupPage.TabStop = false;
            this.groupPage.Text = "网页保存配置";
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(503, 19);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 21);
            this.button3.TabIndex = 2;
            this.button3.Text = "浏览...";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // txtSavePath
            // 
            this.txtSavePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSavePath.Location = new System.Drawing.Point(111, 19);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(393, 20);
            this.txtSavePath.TabIndex = 1;
            this.txtSavePath.TextChanged += new System.EventHandler(this.txtSavePath_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "网页保存地址：";
            // 
            // groupData
            // 
            this.groupData.Controls.Add(this.button1);
            this.groupData.Controls.Add(this.label8);
            this.groupData.Controls.Add(this.txtRadarDb);
            this.groupData.Controls.Add(this.label6);
            this.groupData.Controls.Add(this.label5);
            this.groupData.Controls.Add(this.txtSql);
            this.groupData.Controls.Add(this.label10);
            this.groupData.Controls.Add(this.label9);
            this.groupData.Controls.Add(this.comTableName);
            this.groupData.Enabled = false;
            this.groupData.Location = new System.Drawing.Point(0, 70);
            this.groupData.Name = "groupData";
            this.groupData.Size = new System.Drawing.Size(573, 165);
            this.groupData.TabIndex = 4;
            this.groupData.TabStop = false;
            this.groupData.Text = "数据库配置";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(524, 83);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(43, 36);
            this.button1.TabIndex = 11;
            this.button1.Text = "插入参数";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label8
            // 
            this.label8.ForeColor = System.Drawing.Color.Red;
            this.label8.Location = new System.Drawing.Point(86, 136);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(481, 26);
            this.label8.TabIndex = 10;
            this.label8.Text = "输入表名为新建数据表。新建数据表sql语句将自动产生，如果是存入已有数据表，请务必确保sql语句正确，否则会保存失败。";
            // 
            // txtRadarDb
            // 
            this.txtRadarDb.BackColor = System.Drawing.Color.White;
            this.txtRadarDb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRadarDb.Location = new System.Drawing.Point(86, 32);
            this.txtRadarDb.Name = "txtRadarDb";
            this.txtRadarDb.ReadOnly = true;
            this.txtRadarDb.Size = new System.Drawing.Size(481, 20);
            this.txtRadarDb.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "数据库：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(86, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(339, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "无法修改数据库，请在菜单\"参数设置\"中\"监控数据\"指定数据库";
            // 
            // txtSql
            // 
            this.txtSql.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSql.Location = new System.Drawing.Point(86, 83);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSql.Size = new System.Drawing.Size(439, 50);
            this.txtSql.TabIndex = 6;
            this.txtSql.TextChanged += new System.EventHandler(this.txtSql_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 5;
            this.label10.Text = "SQL语句：";
            this.label10.Click += new System.EventHandler(this.label10_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "选择表名：";
            // 
            // comTableName
            // 
            this.comTableName.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comTableName.FormattingEnabled = true;
            this.comTableName.Location = new System.Drawing.Point(86, 56);
            this.comTableName.Name = "comTableName";
            this.comTableName.Size = new System.Drawing.Size(481, 21);
            this.comTableName.TabIndex = 2;
            this.comTableName.SelectedIndexChanged += new System.EventHandler(this.comTableName_SelectedIndexChanged);
            this.comTableName.DropDown += new System.EventHandler(this.comTableName_DropDown);
            this.comTableName.TextChanged += new System.EventHandler(this.comTableName_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.raSendEmail);
            this.groupBox4.Controls.Add(this.raSaveUrl);
            this.groupBox4.Controls.Add(this.raSavePage);
            this.groupBox4.Controls.Add(this.raNoDeal);
            this.groupBox4.Location = new System.Drawing.Point(0, 0);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(573, 69);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "处理方式";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // raSendEmail
            // 
            this.raSendEmail.AutoSize = true;
            this.raSendEmail.Location = new System.Drawing.Point(346, 41);
            this.raSendEmail.Name = "raSendEmail";
            this.raSendEmail.Size = new System.Drawing.Size(133, 17);
            this.raSendEmail.TabIndex = 3;
            this.raSendEmail.Text = "将信息发送指定信箱";
            this.raSendEmail.UseVisualStyleBackColor = true;
            this.raSendEmail.CheckedChanged += new System.EventHandler(this.raSendEmail_CheckedChanged);
            // 
            // raSaveUrl
            // 
            this.raSaveUrl.AutoSize = true;
            this.raSaveUrl.Location = new System.Drawing.Point(213, 41);
            this.raSaveUrl.Name = "raSaveUrl";
            this.raSaveUrl.Size = new System.Drawing.Size(109, 17);
            this.raSaveUrl.TabIndex = 2;
            this.raSaveUrl.Text = "仅保存网页地址";
            this.raSaveUrl.UseVisualStyleBackColor = true;
            this.raSaveUrl.CheckedChanged += new System.EventHandler(this.raSaveUrl_CheckedChanged);
            // 
            // raSavePage
            // 
            this.raSavePage.AutoSize = true;
            this.raSavePage.Location = new System.Drawing.Point(16, 41);
            this.raSavePage.Name = "raSavePage";
            this.raSavePage.Size = new System.Drawing.Size(181, 17);
            this.raSavePage.TabIndex = 1;
            this.raSavePage.Text = "保存网页快照并记录网页地址";
            this.raSavePage.UseVisualStyleBackColor = true;
            this.raSavePage.CheckedChanged += new System.EventHandler(this.raSavePage_CheckedChanged);
            // 
            // raNoDeal
            // 
            this.raNoDeal.AutoSize = true;
            this.raNoDeal.Checked = true;
            this.raNoDeal.Location = new System.Drawing.Point(16, 16);
            this.raNoDeal.Name = "raNoDeal";
            this.raNoDeal.Size = new System.Drawing.Size(482, 17);
            this.raNoDeal.TabIndex = 0;
            this.raNoDeal.TabStop = true;
            this.raNoDeal.Text = "按照采集任务配置进行数据保存（仅支持\"直接入库\"操作,其他数据保存方式暂不支持）";
            this.raNoDeal.UseVisualStyleBackColor = true;
            this.raNoDeal.CheckedChanged += new System.EventHandler(this.raNoDeal_CheckedChanged);
            // 
            // tabAdvance
            // 
            this.tabAdvance.Controls.Add(this.groupBox5);
            this.tabAdvance.Controls.Add(this.groupBox7);
            this.tabAdvance.ImageIndex = 2;
            this.tabAdvance.Location = new System.Drawing.Point(4, 26);
            this.tabAdvance.Name = "tabAdvance";
            this.tabAdvance.Size = new System.Drawing.Size(580, 348);
            this.tabAdvance.TabIndex = 3;
            this.tabAdvance.Text = "高级设置";
            this.tabAdvance.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.raTmpInvalid);
            this.groupBox5.Controls.Add(this.raInvalid);
            this.groupBox5.Controls.Add(this.InvalidDate);
            this.groupBox5.Controls.Add(this.raInvalidByDate);
            this.groupBox5.Controls.Add(this.raNoInvalid);
            this.groupBox5.Location = new System.Drawing.Point(0, 112);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(573, 102);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "监控规则有效期";
            // 
            // raTmpInvalid
            // 
            this.raTmpInvalid.AutoSize = true;
            this.raTmpInvalid.Location = new System.Drawing.Point(121, 19);
            this.raTmpInvalid.Name = "raTmpInvalid";
            this.raTmpInvalid.Size = new System.Drawing.Size(109, 17);
            this.raTmpInvalid.TabIndex = 4;
            this.raTmpInvalid.TabStop = true;
            this.raTmpInvalid.Text = "暂时置其为无效";
            this.raTmpInvalid.UseVisualStyleBackColor = true;
            // 
            // raInvalid
            // 
            this.raInvalid.AutoSize = true;
            this.raInvalid.Location = new System.Drawing.Point(17, 70);
            this.raInvalid.Name = "raInvalid";
            this.raInvalid.Size = new System.Drawing.Size(229, 17);
            this.raInvalid.TabIndex = 3;
            this.raInvalid.Text = "监控到信息后停止检测，置其为无效。";
            this.raInvalid.UseVisualStyleBackColor = true;
            this.raInvalid.CheckedChanged += new System.EventHandler(this.raInvalid_CheckedChanged);
            // 
            // InvalidDate
            // 
            this.InvalidDate.Enabled = false;
            this.InvalidDate.Location = new System.Drawing.Point(96, 42);
            this.InvalidDate.Name = "InvalidDate";
            this.InvalidDate.Size = new System.Drawing.Size(152, 20);
            this.InvalidDate.TabIndex = 2;
            this.InvalidDate.ValueChanged += new System.EventHandler(this.InvalidDate_ValueChanged);
            // 
            // raInvalidByDate
            // 
            this.raInvalidByDate.AutoSize = true;
            this.raInvalidByDate.Location = new System.Drawing.Point(17, 44);
            this.raInvalidByDate.Name = "raInvalidByDate";
            this.raInvalidByDate.Size = new System.Drawing.Size(73, 17);
            this.raInvalidByDate.TabIndex = 1;
            this.raInvalidByDate.Text = "截止于：";
            this.raInvalidByDate.UseVisualStyleBackColor = true;
            this.raInvalidByDate.CheckedChanged += new System.EventHandler(this.raInvalidByDate_CheckedChanged);
            // 
            // raNoInvalid
            // 
            this.raNoInvalid.AutoSize = true;
            this.raNoInvalid.Checked = true;
            this.raNoInvalid.Location = new System.Drawing.Point(17, 19);
            this.raNoInvalid.Name = "raNoInvalid";
            this.raNoInvalid.Size = new System.Drawing.Size(73, 17);
            this.raNoInvalid.TabIndex = 0;
            this.raNoInvalid.TabStop = true;
            this.raNoInvalid.Text = "永远有效";
            this.raNoInvalid.UseVisualStyleBackColor = true;
            this.raNoInvalid.CheckedChanged += new System.EventHandler(this.raNoInvalid_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtWarningEmail);
            this.groupBox7.Controls.Add(this.label12);
            this.groupBox7.Controls.Add(this.raWarningEmail);
            this.groupBox7.Controls.Add(this.raWarningTrayIcon);
            this.groupBox7.Controls.Add(this.raNoWarning);
            this.groupBox7.Location = new System.Drawing.Point(0, 0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(573, 106);
            this.groupBox7.TabIndex = 1;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "预警规则";
            // 
            // txtWarningEmail
            // 
            this.txtWarningEmail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWarningEmail.Enabled = false;
            this.txtWarningEmail.Location = new System.Drawing.Point(135, 68);
            this.txtWarningEmail.Name = "txtWarningEmail";
            this.txtWarningEmail.Size = new System.Drawing.Size(432, 20);
            this.txtWarningEmail.TabIndex = 4;
            this.txtWarningEmail.TextChanged += new System.EventHandler(this.txtWarningEmail_TextChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Enabled = false;
            this.label12.Location = new System.Drawing.Point(14, 71);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(115, 13);
            this.label12.TabIndex = 3;
            this.label12.Text = "接收预警信息信箱：";
            // 
            // raWarningEmail
            // 
            this.raWarningEmail.AutoSize = true;
            this.raWarningEmail.Location = new System.Drawing.Point(184, 42);
            this.raWarningEmail.Name = "raWarningEmail";
            this.raWarningEmail.Size = new System.Drawing.Size(145, 17);
            this.raWarningEmail.TabIndex = 2;
            this.raWarningEmail.TabStop = true;
            this.raWarningEmail.Text = "发送电子邮件预警信息";
            this.raWarningEmail.UseVisualStyleBackColor = true;
            this.raWarningEmail.CheckedChanged += new System.EventHandler(this.raWarningEmail_CheckedChanged);
            // 
            // raWarningTrayIcon
            // 
            this.raWarningTrayIcon.AutoSize = true;
            this.raWarningTrayIcon.Location = new System.Drawing.Point(17, 42);
            this.raWarningTrayIcon.Name = "raWarningTrayIcon";
            this.raWarningTrayIcon.Size = new System.Drawing.Size(145, 17);
            this.raWarningTrayIcon.TabIndex = 1;
            this.raWarningTrayIcon.TabStop = true;
            this.raWarningTrayIcon.Text = "通过闪烁托盘图标预警";
            this.raWarningTrayIcon.UseVisualStyleBackColor = true;
            this.raWarningTrayIcon.CheckedChanged += new System.EventHandler(this.raWarningTrayIcon_CheckedChanged);
            // 
            // raNoWarning
            // 
            this.raNoWarning.AutoSize = true;
            this.raNoWarning.Checked = true;
            this.raNoWarning.Location = new System.Drawing.Point(17, 19);
            this.raNoWarning.Name = "raNoWarning";
            this.raNoWarning.Size = new System.Drawing.Size(121, 17);
            this.raNoWarning.TabIndex = 0;
            this.raNoWarning.TabStop = true;
            this.raNoWarning.Text = "仅保存数据不预警";
            this.raNoWarning.UseVisualStyleBackColor = true;
            this.raNoWarning.CheckedChanged += new System.EventHandler(this.raNoWarning_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "任务名称";
            // 
            // RuleName
            // 
            this.RuleName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RuleName.Location = new System.Drawing.Point(78, 13);
            this.RuleName.Name = "RuleName";
            this.RuleName.Size = new System.Drawing.Size(513, 20);
            this.RuleName.TabIndex = 2;
            this.RuleName.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(3, 432);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(611, 5);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            // 
            // cmdApply
            // 
            this.cmdApply.Enabled = false;
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Image = ((System.Drawing.Image)(resources.GetObject("cmdApply.Image")));
            this.cmdApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdApply.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdApply.Location = new System.Drawing.Point(519, 451);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(65, 24);
            this.cmdApply.TabIndex = 7;
            this.cmdApply.Text = "应 用";
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
            this.cmdCancel.Location = new System.Drawing.Point(440, 451);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 24);
            this.cmdCancel.TabIndex = 6;
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
            this.cmdOK.Location = new System.Drawing.Point(362, 451);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 24);
            this.cmdOK.TabIndex = 5;
            this.cmdOK.Text = "确 定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // IsSave
            // 
            this.IsSave.Location = new System.Drawing.Point(221, 454);
            this.IsSave.Name = "IsSave";
            this.IsSave.Size = new System.Drawing.Size(43, 20);
            this.IsSave.TabIndex = 8;
            this.IsSave.Visible = false;
            this.IsSave.TextChanged += new System.EventHandler(this.IsSave_TextChanged);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leToolStripMenuItem,
            this.采集任务TaskNameToolStripMenuItem,
            this.urlUrlToolStripMenuItem,
            this.监控时间MonitorDateToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(233, 92);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // leToolStripMenuItem
            // 
            this.leToolStripMenuItem.Name = "leToolStripMenuItem";
            this.leToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.leToolStripMenuItem.Text = "监控规则名称:{MonitorName}";
            // 
            // 采集任务TaskNameToolStripMenuItem
            // 
            this.采集任务TaskNameToolStripMenuItem.Name = "采集任务TaskNameToolStripMenuItem";
            this.采集任务TaskNameToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.采集任务TaskNameToolStripMenuItem.Text = "采集任务:{TaskName}";
            // 
            // urlUrlToolStripMenuItem
            // 
            this.urlUrlToolStripMenuItem.Name = "urlUrlToolStripMenuItem";
            this.urlUrlToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.urlUrlToolStripMenuItem.Text = "网页地址:{Url}";
            // 
            // 监控时间MonitorDateToolStripMenuItem
            // 
            this.监控时间MonitorDateToolStripMenuItem.Name = "监控时间MonitorDateToolStripMenuItem";
            this.监控时间MonitorDateToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.监控时间MonitorDateToolStripMenuItem.Text = "监控时间:{MonitorDate}";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.Red;
            this.label13.Location = new System.Drawing.Point(6, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(164, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "组合关键词请使用“/”进行分割";
            // 
            // frmRader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 486);
            this.Controls.Add(this.IsSave);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.RuleName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRader";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "监控规则配置";
            this.Load += new System.EventHandler(this.frmRader_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRader_FormClosed);
            this.tabControl1.ResumeLayout(false);
            this.tabSource.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabRule.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RuleNum)).EndInit();
            this.tabOP.ResumeLayout(false);
            this.groupEmail.ResumeLayout(false);
            this.groupEmail.PerformLayout();
            this.groupPage.ResumeLayout(false);
            this.groupPage.PerformLayout();
            this.groupData.ResumeLayout(false);
            this.groupData.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabAdvance.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSource;
        private System.Windows.Forms.TabPage tabRule;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox RuleName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabPage tabOP;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton raNoDeal;
        private System.Windows.Forms.RadioButton raSavePage;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ListView listRule;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ComboBox comDataSource;
        private System.Windows.Forms.ComboBox comRule;
        private System.Windows.Forms.NumericUpDown RuleNum;
        private System.Windows.Forms.Button cmdDelRule;
        private System.Windows.Forms.Button cmdAddRule;
        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdDelTask;
        private System.Windows.Forms.Button cmdAddTask;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listRange;
        private System.Windows.Forms.TabPage tabAdvance;
        private System.Windows.Forms.RadioButton raSaveUrl;
        private System.Windows.Forms.GroupBox groupPage;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupData;
        private System.Windows.Forms.ComboBox comTableName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.RadioButton raWarningEmail;
        private System.Windows.Forms.RadioButton raWarningTrayIcon;
        private System.Windows.Forms.RadioButton raNoWarning;
        private System.Windows.Forms.RadioButton raSendEmail;
        private System.Windows.Forms.GroupBox groupEmail;
        private System.Windows.Forms.TextBox txtDealEmail;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtWarningEmail;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox IsSave;
        private System.Windows.Forms.TextBox txtRuleContent;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRadarDb;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem urlUrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 采集任务TaskNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 监控时间MonitorDateToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton raNoInvalid;
        private System.Windows.Forms.RadioButton raInvalidByDate;
        private System.Windows.Forms.DateTimePicker InvalidDate;
        private System.Windows.Forms.RadioButton raInvalid;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RadioButton raTmpInvalid;
        private System.Windows.Forms.Label label13;
    }
}