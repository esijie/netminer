namespace MinerSpider
{
    partial class frmAddDataEditRule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddDataEditRule));
            this.label1 = new System.Windows.Forms.Label();
            this.raString = new System.Windows.Forms.RadioButton();
            this.raEncoding = new System.Windows.Forms.RadioButton();
            this.raWebPage = new System.Windows.Forms.RadioButton();
            this.raCondition = new System.Windows.Forms.RadioButton();
            this.raDataEdit = new System.Windows.Forms.RadioButton();
            this.raValue = new System.Windows.Forms.RadioButton();
            this.raDataGather = new System.Windows.Forms.RadioButton();
            this.ExportRuleType = new System.Windows.Forms.ComboBox();
            this.raOther = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtOldValue = new System.Windows.Forms.TextBox();
            this.txtNewValue = new System.Windows.Forms.TextBox();
            this.txtRule = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.comSynDb = new System.Windows.Forms.ComboBox();
            this.butInsertPara = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.重命名参数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名参数获取采集数据ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名参数当前日期ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名参数自动编号ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名参数当前采集任务名称ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.重命名参数输入采集数据规则名称ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.格式化参数数字ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.格式化参数货币ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.格式化参数长日期ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.raPlugin = new System.Windows.Forms.RadioButton();
            this.raDBRule = new System.Windows.Forms.RadioButton();
            this.raWatermark = new System.Windows.Forms.RadioButton();
            this.butSetWatermark = new System.Windows.Forms.Button();
            this.numOcrScale = new System.Windows.Forms.NumericUpDown();
            this.butSetOCR = new System.Windows.Forms.Button();
            this.raDownload = new System.Windows.Forms.RadioButton();
            this.isOcrNumber = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numOcrScale)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据加工规则：";
            // 
            // raString
            // 
            this.raString.AutoSize = true;
            this.raString.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raString.Location = new System.Drawing.Point(144, 12);
            this.raString.Name = "raString";
            this.raString.Size = new System.Drawing.Size(94, 16);
            this.raString.TabIndex = 1;
            this.raString.Text = "字符串处理类";
            this.raString.UseVisualStyleBackColor = true;
            this.raString.CheckedChanged += new System.EventHandler(this.raString_CheckedChanged);
            // 
            // raEncoding
            // 
            this.raEncoding.AutoSize = true;
            this.raEncoding.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raEncoding.Location = new System.Drawing.Point(267, 12);
            this.raEncoding.Name = "raEncoding";
            this.raEncoding.Size = new System.Drawing.Size(82, 16);
            this.raEncoding.TabIndex = 2;
            this.raEncoding.Text = "编码解码类";
            this.raEncoding.UseVisualStyleBackColor = true;
            this.raEncoding.CheckedChanged += new System.EventHandler(this.raEncoding_CheckedChanged);
            // 
            // raWebPage
            // 
            this.raWebPage.AutoSize = true;
            this.raWebPage.Checked = true;
            this.raWebPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raWebPage.Location = new System.Drawing.Point(18, 12);
            this.raWebPage.Name = "raWebPage";
            this.raWebPage.Size = new System.Drawing.Size(106, 16);
            this.raWebPage.TabIndex = 0;
            this.raWebPage.TabStop = true;
            this.raWebPage.Text = "网页代码处理类";
            this.raWebPage.UseVisualStyleBackColor = true;
            this.raWebPage.CheckedChanged += new System.EventHandler(this.raWebPage_CheckedChanged);
            // 
            // raCondition
            // 
            this.raCondition.AutoSize = true;
            this.raCondition.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raCondition.Location = new System.Drawing.Point(365, 12);
            this.raCondition.Name = "raCondition";
            this.raCondition.Size = new System.Drawing.Size(82, 16);
            this.raCondition.TabIndex = 3;
            this.raCondition.Text = "条件判断类";
            this.raCondition.UseVisualStyleBackColor = true;
            this.raCondition.CheckedChanged += new System.EventHandler(this.raCondition_CheckedChanged);
            // 
            // raDataEdit
            // 
            this.raDataEdit.AutoSize = true;
            this.raDataEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raDataEdit.Location = new System.Drawing.Point(18, 39);
            this.raDataEdit.Name = "raDataEdit";
            this.raDataEdit.Size = new System.Drawing.Size(82, 16);
            this.raDataEdit.TabIndex = 4;
            this.raDataEdit.Text = "数据再加工";
            this.raDataEdit.UseVisualStyleBackColor = true;
            this.raDataEdit.CheckedChanged += new System.EventHandler(this.raDataEdit_CheckedChanged);
            // 
            // raValue
            // 
            this.raValue.AutoSize = true;
            this.raValue.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raValue.Location = new System.Drawing.Point(267, 39);
            this.raValue.Name = "raValue";
            this.raValue.Size = new System.Drawing.Size(58, 16);
            this.raValue.TabIndex = 6;
            this.raValue.Text = "固定值";
            this.raValue.UseVisualStyleBackColor = true;
            this.raValue.CheckedChanged += new System.EventHandler(this.raValue_CheckedChanged);
            // 
            // raDataGather
            // 
            this.raDataGather.AutoSize = true;
            this.raDataGather.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raDataGather.Location = new System.Drawing.Point(144, 39);
            this.raDataGather.Name = "raDataGather";
            this.raDataGather.Size = new System.Drawing.Size(106, 16);
            this.raDataGather.TabIndex = 5;
            this.raDataGather.Text = "采集数据组合类";
            this.raDataGather.UseVisualStyleBackColor = true;
            this.raDataGather.CheckedChanged += new System.EventHandler(this.raDataGather_CheckedChanged);
            // 
            // ExportRuleType
            // 
            this.ExportRuleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ExportRuleType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ExportRuleType.FormattingEnabled = true;
            this.ExportRuleType.Location = new System.Drawing.Point(105, 93);
            this.ExportRuleType.Name = "ExportRuleType";
            this.ExportRuleType.Size = new System.Drawing.Size(342, 20);
            this.ExportRuleType.TabIndex = 8;
            this.ExportRuleType.SelectedIndexChanged += new System.EventHandler(this.ExportRuleType_SelectedIndexChanged);
            // 
            // raOther
            // 
            this.raOther.AutoSize = true;
            this.raOther.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raOther.Location = new System.Drawing.Point(365, 39);
            this.raOther.Name = "raOther";
            this.raOther.Size = new System.Drawing.Size(82, 16);
            this.raOther.TabIndex = 7;
            this.raOther.Text = "文章加工类";
            this.raOther.UseVisualStyleBackColor = true;
            this.raOther.CheckedChanged += new System.EventHandler(this.raOther_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "源值：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(259, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "新值：";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(18, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "规则：";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtOldValue
            // 
            this.txtOldValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOldValue.Enabled = false;
            this.txtOldValue.Location = new System.Drawing.Point(105, 146);
            this.txtOldValue.Name = "txtOldValue";
            this.txtOldValue.Size = new System.Drawing.Size(148, 21);
            this.txtOldValue.TabIndex = 9;
            // 
            // txtNewValue
            // 
            this.txtNewValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewValue.Enabled = false;
            this.txtNewValue.Location = new System.Drawing.Point(299, 146);
            this.txtNewValue.Name = "txtNewValue";
            this.txtNewValue.Size = new System.Drawing.Size(148, 21);
            this.txtNewValue.TabIndex = 10;
            // 
            // txtRule
            // 
            this.txtRule.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRule.Enabled = false;
            this.txtRule.Location = new System.Drawing.Point(105, 119);
            this.txtRule.Name = "txtRule";
            this.txtRule.Size = new System.Drawing.Size(269, 21);
            this.txtRule.TabIndex = 11;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(382, 183);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 13;
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
            this.cmdApply.Location = new System.Drawing.Point(309, 183);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(65, 22);
            this.cmdApply.TabIndex = 12;
            this.cmdApply.Text = "确 定";
            this.cmdApply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // comSynDb
            // 
            this.comSynDb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comSynDb.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comSynDb.FormattingEnabled = true;
            this.comSynDb.Location = new System.Drawing.Point(105, 119);
            this.comSynDb.Name = "comSynDb";
            this.comSynDb.Size = new System.Drawing.Size(269, 20);
            this.comSynDb.TabIndex = 14;
            this.comSynDb.Visible = false;
            this.comSynDb.SelectedIndexChanged += new System.EventHandler(this.comSynDb_SelectedIndexChanged);
            // 
            // butInsertPara
            // 
            this.butInsertPara.Enabled = false;
            this.butInsertPara.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butInsertPara.Location = new System.Drawing.Point(374, 118);
            this.butInsertPara.Name = "butInsertPara";
            this.butInsertPara.Size = new System.Drawing.Size(73, 23);
            this.butInsertPara.TabIndex = 15;
            this.butInsertPara.Text = "插入参数";
            this.butInsertPara.UseVisualStyleBackColor = true;
            this.butInsertPara.Click += new System.EventHandler(this.butInsertPara_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.重命名参数ToolStripMenuItem,
            this.重命名参数获取采集数据ToolStripMenuItem,
            this.重命名参数当前日期ToolStripMenuItem,
            this.重命名参数自动编号ToolStripMenuItem,
            this.重命名参数当前采集任务名称ToolStripMenuItem,
            this.重命名参数输入采集数据规则名称ToolStripMenuItem,
            this.toolStripSeparator1,
            this.格式化参数数字ToolStripMenuItem,
            this.格式化参数货币ToolStripMenuItem,
            this.格式化参数长日期ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(269, 208);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // 重命名参数ToolStripMenuItem
            // 
            this.重命名参数ToolStripMenuItem.Name = "重命名参数ToolStripMenuItem";
            this.重命名参数ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数ToolStripMenuItem.Text = "重命名参数：获取当前标题";
            // 
            // 重命名参数获取采集数据ToolStripMenuItem
            // 
            this.重命名参数获取采集数据ToolStripMenuItem.Name = "重命名参数获取采集数据ToolStripMenuItem";
            this.重命名参数获取采集数据ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数获取采集数据ToolStripMenuItem.Text = "重命名参数：获取采集数据";
            // 
            // 重命名参数当前日期ToolStripMenuItem
            // 
            this.重命名参数当前日期ToolStripMenuItem.Name = "重命名参数当前日期ToolStripMenuItem";
            this.重命名参数当前日期ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数当前日期ToolStripMenuItem.Text = "重命名参数：当前日期";
            // 
            // 重命名参数自动编号ToolStripMenuItem
            // 
            this.重命名参数自动编号ToolStripMenuItem.Name = "重命名参数自动编号ToolStripMenuItem";
            this.重命名参数自动编号ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数自动编号ToolStripMenuItem.Text = "重命名参数：自动编号";
            // 
            // 重命名参数当前采集任务名称ToolStripMenuItem
            // 
            this.重命名参数当前采集任务名称ToolStripMenuItem.Name = "重命名参数当前采集任务名称ToolStripMenuItem";
            this.重命名参数当前采集任务名称ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数当前采集任务名称ToolStripMenuItem.Text = "重命名参数：当前采集任务名称";
            // 
            // 重命名参数输入采集数据规则名称ToolStripMenuItem
            // 
            this.重命名参数输入采集数据规则名称ToolStripMenuItem.Name = "重命名参数输入采集数据规则名称ToolStripMenuItem";
            this.重命名参数输入采集数据规则名称ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.重命名参数输入采集数据规则名称ToolStripMenuItem.Text = "重命名参数：输入采集数据规则名称";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(265, 6);
            // 
            // 格式化参数数字ToolStripMenuItem
            // 
            this.格式化参数数字ToolStripMenuItem.Name = "格式化参数数字ToolStripMenuItem";
            this.格式化参数数字ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.格式化参数数字ToolStripMenuItem.Text = "格式化参数：数字";
            // 
            // 格式化参数货币ToolStripMenuItem
            // 
            this.格式化参数货币ToolStripMenuItem.Name = "格式化参数货币ToolStripMenuItem";
            this.格式化参数货币ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.格式化参数货币ToolStripMenuItem.Text = "格式化参数：货币";
            // 
            // 格式化参数长日期ToolStripMenuItem
            // 
            this.格式化参数长日期ToolStripMenuItem.Name = "格式化参数长日期ToolStripMenuItem";
            this.格式化参数长日期ToolStripMenuItem.Size = new System.Drawing.Size(268, 22);
            this.格式化参数长日期ToolStripMenuItem.Text = "格式化参数：长日期";
            // 
            // raPlugin
            // 
            this.raPlugin.AutoSize = true;
            this.raPlugin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raPlugin.Location = new System.Drawing.Point(18, 65);
            this.raPlugin.Name = "raPlugin";
            this.raPlugin.Size = new System.Drawing.Size(106, 16);
            this.raPlugin.TabIndex = 16;
            this.raPlugin.TabStop = true;
            this.raPlugin.Text = "数据加工插件类";
            this.raPlugin.UseVisualStyleBackColor = true;
            this.raPlugin.CheckedChanged += new System.EventHandler(this.raPlugin_CheckedChanged);
            // 
            // raDBRule
            // 
            this.raDBRule.AutoSize = true;
            this.raDBRule.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raDBRule.Location = new System.Drawing.Point(144, 65);
            this.raDBRule.Name = "raDBRule";
            this.raDBRule.Size = new System.Drawing.Size(94, 16);
            this.raDBRule.TabIndex = 17;
            this.raDBRule.TabStop = true;
            this.raDBRule.Text = "采集执行规则";
            this.raDBRule.UseVisualStyleBackColor = true;
            this.raDBRule.CheckedChanged += new System.EventHandler(this.raDBRule_CheckedChanged);
            // 
            // raWatermark
            // 
            this.raWatermark.AutoSize = true;
            this.raWatermark.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raWatermark.Location = new System.Drawing.Point(267, 65);
            this.raWatermark.Name = "raWatermark";
            this.raWatermark.Size = new System.Drawing.Size(82, 16);
            this.raWatermark.TabIndex = 18;
            this.raWatermark.TabStop = true;
            this.raWatermark.Text = "图片处理类";
            this.raWatermark.UseVisualStyleBackColor = true;
            this.raWatermark.CheckedChanged += new System.EventHandler(this.raWatermark_CheckedChanged);
            // 
            // butSetWatermark
            // 
            this.butSetWatermark.Enabled = false;
            this.butSetWatermark.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSetWatermark.Location = new System.Drawing.Point(374, 118);
            this.butSetWatermark.Name = "butSetWatermark";
            this.butSetWatermark.Size = new System.Drawing.Size(73, 23);
            this.butSetWatermark.TabIndex = 19;
            this.butSetWatermark.Text = "设置水印";
            this.butSetWatermark.UseVisualStyleBackColor = true;
            this.butSetWatermark.Visible = false;
            this.butSetWatermark.Click += new System.EventHandler(this.butSetWatermark_Click);
            // 
            // numOcrScale
            // 
            this.numOcrScale.DecimalPlaces = 1;
            this.numOcrScale.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numOcrScale.Location = new System.Drawing.Point(105, 119);
            this.numOcrScale.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numOcrScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numOcrScale.Name = "numOcrScale";
            this.numOcrScale.Size = new System.Drawing.Size(269, 21);
            this.numOcrScale.TabIndex = 20;
            this.numOcrScale.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numOcrScale.Visible = false;
            // 
            // butSetOCR
            // 
            this.butSetOCR.Enabled = false;
            this.butSetOCR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butSetOCR.Location = new System.Drawing.Point(374, 118);
            this.butSetOCR.Name = "butSetOCR";
            this.butSetOCR.Size = new System.Drawing.Size(73, 23);
            this.butSetOCR.TabIndex = 21;
            this.butSetOCR.Text = "测试OCR";
            this.butSetOCR.UseVisualStyleBackColor = true;
            this.butSetOCR.Visible = false;
            this.butSetOCR.Click += new System.EventHandler(this.butSetOCR_Click);
            // 
            // raDownload
            // 
            this.raDownload.AutoSize = true;
            this.raDownload.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raDownload.Location = new System.Drawing.Point(365, 65);
            this.raDownload.Name = "raDownload";
            this.raDownload.Size = new System.Drawing.Size(70, 16);
            this.raDownload.TabIndex = 22;
            this.raDownload.Text = "下载处理";
            this.raDownload.UseVisualStyleBackColor = true;
            this.raDownload.CheckedChanged += new System.EventHandler(this.raDownload_CheckedChanged);
            // 
            // isOcrNumber
            // 
            this.isOcrNumber.AutoSize = true;
            this.isOcrNumber.Location = new System.Drawing.Point(105, 173);
            this.isOcrNumber.Name = "isOcrNumber";
            this.isOcrNumber.Size = new System.Drawing.Size(84, 16);
            this.isOcrNumber.TabIndex = 23;
            this.isOcrNumber.Text = "只识别数字";
            this.isOcrNumber.UseVisualStyleBackColor = true;
            this.isOcrNumber.Visible = false;
            // 
            // frmAddDataEditRule
            // 
            this.AcceptButton = this.cmdApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(470, 222);
            this.Controls.Add(this.isOcrNumber);
            this.Controls.Add(this.raDownload);
            this.Controls.Add(this.raWatermark);
            this.Controls.Add(this.raDBRule);
            this.Controls.Add(this.raPlugin);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.txtNewValue);
            this.Controls.Add(this.txtOldValue);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.raOther);
            this.Controls.Add(this.ExportRuleType);
            this.Controls.Add(this.raDataGather);
            this.Controls.Add(this.raValue);
            this.Controls.Add(this.raDataEdit);
            this.Controls.Add(this.raCondition);
            this.Controls.Add(this.raWebPage);
            this.Controls.Add(this.raEncoding);
            this.Controls.Add(this.raString);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRule);
            this.Controls.Add(this.comSynDb);
            this.Controls.Add(this.butSetOCR);
            this.Controls.Add(this.butSetWatermark);
            this.Controls.Add(this.butInsertPara);
            this.Controls.Add(this.numOcrScale);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddDataEditRule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "数据加工规则";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAddDataEditRule_FormClosing);
            this.Load += new System.EventHandler(this.frmAddDataEditRule_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numOcrScale)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton raString;
        private System.Windows.Forms.RadioButton raEncoding;
        private System.Windows.Forms.RadioButton raWebPage;
        private System.Windows.Forms.RadioButton raCondition;
        private System.Windows.Forms.RadioButton raDataEdit;
        private System.Windows.Forms.RadioButton raValue;
        private System.Windows.Forms.RadioButton raDataGather;
        private System.Windows.Forms.ComboBox ExportRuleType;
        private System.Windows.Forms.RadioButton raOther;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtOldValue;
        private System.Windows.Forms.TextBox txtNewValue;
        private System.Windows.Forms.TextBox txtRule;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.ComboBox comSynDb;
        private System.Windows.Forms.Button butInsertPara;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数获取采集数据ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数当前日期ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数自动编号ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数当前采集任务名称ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem 格式化参数数字ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 格式化参数货币ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 格式化参数长日期ToolStripMenuItem;
        private System.Windows.Forms.RadioButton raPlugin;
        private System.Windows.Forms.RadioButton raDBRule;
        private System.Windows.Forms.RadioButton raWatermark;
        private System.Windows.Forms.Button butSetWatermark;
        private System.Windows.Forms.NumericUpDown numOcrScale;
        private System.Windows.Forms.Button butSetOCR;
        private System.Windows.Forms.RadioButton raDownload;
        private System.Windows.Forms.ToolStripMenuItem 重命名参数输入采集数据规则名称ToolStripMenuItem;
        private System.Windows.Forms.CheckBox isOcrNumber;
    }
}