namespace MinerSpider
{
    partial class frmSmartGRule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSmartGRule));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAllow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolOkExit = new System.Windows.Forms.ToolStripButton();
            this.toolCancleExit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.DomTree = new System.Windows.Forms.TreeView();
            this.tabPage6 = new System.Windows.Forms.TabPage();
            this.txtSource = new System.Windows.Forms.RichTextBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.IsWebCode = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.isMatchUrl = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.isMulti = new System.Windows.Forms.CheckBox();
            this.raCustom = new System.Windows.Forms.RadioButton();
            this.raXPath = new System.Windows.Forms.RadioButton();
            this.dataRule = new System.Windows.Forms.DataGridView();
            this.gName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.gRule = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.RichTextBox();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPage6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataRule)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAllow,
            this.toolStripSeparator1,
            this.toolStripButton1,
            this.toolStripSeparator2,
            this.toolOkExit,
            this.toolCancleExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(962, 25);
            this.toolStrip1.TabIndex = 21;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAllow
            // 
            this.toolAllow.Image = ((System.Drawing.Image)(resources.GetObject("toolAllow.Image")));
            this.toolAllow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAllow.Name = "toolAllow";
            this.toolAllow.Size = new System.Drawing.Size(52, 22);
            this.toolAllow.Text = "匹配";
            this.toolAllow.Click += new System.EventHandler(this.toolAllow_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(82, 22);
            this.toolStripButton1.Text = "U码转文字";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolOkExit
            // 
            this.toolOkExit.Image = ((System.Drawing.Image)(resources.GetObject("toolOkExit.Image")));
            this.toolOkExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolOkExit.Name = "toolOkExit";
            this.toolOkExit.Size = new System.Drawing.Size(76, 22);
            this.toolOkExit.Text = "确定退出";
            this.toolOkExit.Click += new System.EventHandler(this.toolOkExit_Click);
            // 
            // toolCancleExit
            // 
            this.toolCancleExit.Image = ((System.Drawing.Image)(resources.GetObject("toolCancleExit.Image")));
            this.toolCancleExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCancleExit.Name = "toolCancleExit";
            this.toolCancleExit.Size = new System.Drawing.Size(76, 22);
            this.toolCancleExit.Text = "取消退出";
            this.toolCancleExit.Click += new System.EventHandler(this.toolCancleExit_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(962, 399);
            this.splitContainer1.SplitterDistance = 155;
            this.splitContainer1.TabIndex = 22;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage6);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.ImageList = this.imageList1;
            this.tabControl2.Location = new System.Drawing.Point(0, 18);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(962, 137);
            this.tabControl2.TabIndex = 3;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.splitContainer2);
            this.tabPage4.ImageIndex = 5;
            this.tabPage4.Location = new System.Drawing.Point(4, 23);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(954, 110);
            this.tabPage4.TabIndex = 0;
            this.tabPage4.Text = "网页";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.webBrowser1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.DomTree);
            this.splitContainer2.Size = new System.Drawing.Size(948, 104);
            this.splitContainer2.SplitterDistance = 659;
            this.splitContainer2.TabIndex = 0;
            // 
            // webBrowser1
            // 
            this.webBrowser1.AllowNavigation = false;
            this.webBrowser1.AllowWebBrowserDrop = false;
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 18);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(659, 104);
            this.webBrowser1.TabIndex = 0;
            this.webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webBrowser1_DocumentCompleted);
            this.webBrowser1.NewWindow += new System.ComponentModel.CancelEventHandler(this.webBrowser1_NewWindow);
            this.webBrowser1.ProgressChanged += new System.Windows.Forms.WebBrowserProgressChangedEventHandler(this.webBrowser1_ProgressChanged);
            // 
            // DomTree
            // 
            this.DomTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DomTree.HideSelection = false;
            this.DomTree.Location = new System.Drawing.Point(0, 0);
            this.DomTree.Name = "DomTree";
            this.DomTree.Size = new System.Drawing.Size(285, 104);
            this.DomTree.TabIndex = 3;
            // 
            // tabPage6
            // 
            this.tabPage6.Controls.Add(this.txtSource);
            this.tabPage6.ImageIndex = 6;
            this.tabPage6.Location = new System.Drawing.Point(4, 23);
            this.tabPage6.Name = "tabPage6";
            this.tabPage6.Size = new System.Drawing.Size(954, 110);
            this.tabPage6.TabIndex = 2;
            this.tabPage6.Text = "源代码";
            this.tabPage6.UseVisualStyleBackColor = true;
            // 
            // txtSource
            // 
            this.txtSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSource.Location = new System.Drawing.Point(0, 0);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(954, 110);
            this.txtSource.TabIndex = 0;
            this.txtSource.Text = "";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "find32.png");
            this.imageList1.Images.SetKeyName(1, "addons32 - Copy - Copy.png");
            this.imageList1.Images.SetKeyName(2, "arrow_right.png");
            this.imageList1.Images.SetKeyName(3, "2.png");
            this.imageList1.Images.SetKeyName(4, "file.gif");
            this.imageList1.Images.SetKeyName(5, "A52.gif");
            this.imageList1.Images.SetKeyName(6, "NewRandomGeometry.png");
            this.imageList1.Images.SetKeyName(7, "A18.png");
            this.imageList1.Images.SetKeyName(8, "ico_world_link.png");
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.txtUrl);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.button1);
            this.splitContainer3.Size = new System.Drawing.Size(962, 18);
            this.splitContainer3.SplitterDistance = 876;
            this.splitContainer3.TabIndex = 2;
            // 
            // txtUrl
            // 
            this.txtUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUrl.Location = new System.Drawing.Point(0, 0);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(876, 21);
            this.txtUrl.TabIndex = 0;
            this.txtUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrl_KeyDown);
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 18);
            this.button1.TabIndex = 0;
            this.button1.Text = "转  到";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(962, 240);
            this.panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(960, 238);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.splitContainer4);
            this.tabPage1.ImageIndex = 3;
            this.tabPage1.Location = new System.Drawing.Point(4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(952, 212);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "分析";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 3);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.IsWebCode);
            this.splitContainer4.Panel1.Controls.Add(this.button6);
            this.splitContainer4.Panel1.Controls.Add(this.isMatchUrl);
            this.splitContainer4.Panel1.Controls.Add(this.button5);
            this.splitContainer4.Panel1.Controls.Add(this.isMulti);
            this.splitContainer4.Panel1.Controls.Add(this.raCustom);
            this.splitContainer4.Panel1.Controls.Add(this.raXPath);
            this.splitContainer4.Panel1.Controls.Add(this.dataRule);
            this.splitContainer4.Panel1.Controls.Add(this.button4);
            this.splitContainer4.Panel1.Controls.Add(this.button3);
            this.splitContainer4.Panel1.Controls.Add(this.button2);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.txtResult);
            this.splitContainer4.Size = new System.Drawing.Size(946, 206);
            this.splitContainer4.SplitterDistance = 631;
            this.splitContainer4.TabIndex = 10;
            // 
            // IsWebCode
            // 
            this.IsWebCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.IsWebCode.AutoSize = true;
            this.IsWebCode.Location = new System.Drawing.Point(496, 8);
            this.IsWebCode.Name = "IsWebCode";
            this.IsWebCode.Size = new System.Drawing.Size(132, 16);
            this.IsWebCode.TabIndex = 16;
            this.IsWebCode.Text = "测试时取出网页代码";
            this.IsWebCode.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button6.Location = new System.Drawing.Point(330, 5);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(190, 21);
            this.button6.TabIndex = 15;
            this.button6.Text = "生成Json采集规则【循环匹配】";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Visible = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // isMatchUrl
            // 
            this.isMatchUrl.AutoSize = true;
            this.isMatchUrl.Location = new System.Drawing.Point(358, 21);
            this.isMatchUrl.Name = "isMatchUrl";
            this.isMatchUrl.Size = new System.Drawing.Size(324, 16);
            this.isMatchUrl.TabIndex = 14;
            this.isMatchUrl.Text = "匹配链接地址，而不是匹配文本（可用于配制导航规则）";
            this.isMatchUrl.UseVisualStyleBackColor = true;
            this.isMatchUrl.Visible = false;
            this.isMatchUrl.CheckedChanged += new System.EventHandler(this.isMatchUrl_CheckedChanged);
            // 
            // button5
            // 
            this.button5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button5.Location = new System.Drawing.Point(314, 40);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(115, 21);
            this.button5.TabIndex = 13;
            this.button5.Text = "自动获取列表规则";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // isMulti
            // 
            this.isMulti.AutoSize = true;
            this.isMulti.Location = new System.Drawing.Point(4, 48);
            this.isMulti.Name = "isMulti";
            this.isMulti.Size = new System.Drawing.Size(288, 16);
            this.isMulti.TabIndex = 12;
            this.isMulti.Text = "多条记录，请通过鼠标捕获第一条和最后一条记录";
            this.isMulti.UseVisualStyleBackColor = true;
            // 
            // raCustom
            // 
            this.raCustom.AutoSize = true;
            this.raCustom.Location = new System.Drawing.Point(160, 27);
            this.raCustom.Name = "raCustom";
            this.raCustom.Size = new System.Drawing.Size(167, 16);
            this.raCustom.TabIndex = 11;
            this.raCustom.Text = "鼠标点击自动匹配前后标识";
            this.raCustom.UseVisualStyleBackColor = true;
            this.raCustom.CheckedChanged += new System.EventHandler(this.raCustom_CheckedChanged);
            // 
            // raXPath
            // 
            this.raXPath.AutoSize = true;
            this.raXPath.Checked = true;
            this.raXPath.Location = new System.Drawing.Point(4, 28);
            this.raXPath.Name = "raXPath";
            this.raXPath.Size = new System.Drawing.Size(149, 16);
            this.raXPath.TabIndex = 10;
            this.raXPath.TabStop = true;
            this.raXPath.Text = "鼠标点击自动匹配XPath";
            this.raXPath.UseVisualStyleBackColor = true;
            this.raXPath.CheckedChanged += new System.EventHandler(this.raXPath_CheckedChanged);
            // 
            // dataRule
            // 
            this.dataRule.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataRule.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataRule.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gName,
            this.gType,
            this.gRule});
            this.dataRule.Location = new System.Drawing.Point(4, 69);
            this.dataRule.Name = "dataRule";
            this.dataRule.RowTemplate.Height = 23;
            this.dataRule.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataRule.Size = new System.Drawing.Size(624, 134);
            this.dataRule.TabIndex = 5;
            // 
            // gName
            // 
            this.gName.HeaderText = "采集数据名称";
            this.gName.Name = "gName";
            this.gName.Width = 120;
            // 
            // gType
            // 
            this.gType.HeaderText = "类别";
            this.gType.Name = "gType";
            this.gType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.gType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // gRule
            // 
            this.gRule.HeaderText = "规则";
            this.gRule.Name = "gRule";
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Location = new System.Drawing.Point(131, 5);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(193, 21);
            this.button4.TabIndex = 9;
            this.button4.Text = "生成表格采集规则【循环匹配】";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Location = new System.Drawing.Point(3, 5);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 21);
            this.button3.TabIndex = 8;
            this.button3.Text = "生成文章采集规则";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(559, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 21);
            this.button2.TabIndex = 6;
            this.button2.Text = "测  试";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtResult
            // 
            this.txtResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResult.Location = new System.Drawing.Point(0, 0);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(311, 206);
            this.txtResult.TabIndex = 7;
            this.txtResult.Text = "";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "采集名称";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 120;
            // 
            // dataGridViewComboBoxColumn1
            // 
            this.dataGridViewComboBoxColumn1.HeaderText = "类别";
            this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            this.dataGridViewComboBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewComboBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "规则";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // frmSmartGRule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(962, 424);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSmartGRule";
            this.Text = "采集规则自动化配置";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmSmartGRule_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPage6.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel1.PerformLayout();
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataRule)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolOkExit;
        private System.Windows.Forms.ToolStripButton toolCancleExit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TabPage tabPage6;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.TreeView DomTree;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ToolStripButton toolAllow;
        private System.Windows.Forms.RichTextBox txtSource;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataRule;
        private System.Windows.Forms.RichTextBox txtResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn gName;
        private System.Windows.Forms.DataGridViewComboBoxColumn gType;
        private System.Windows.Forms.DataGridViewTextBoxColumn gRule;
        private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.RadioButton raCustom;
        private System.Windows.Forms.RadioButton raXPath;
        private System.Windows.Forms.CheckBox isMulti;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.CheckBox isMatchUrl;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox IsWebCode;
        private System.Windows.Forms.ImageList imageList1;
    }
}