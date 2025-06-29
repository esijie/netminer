namespace MinerSpider
{
    partial class frmAddGatherRule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddGatherRule));
            this.comMultiPage = new System.Windows.Forms.ComboBox();
            this.raMultiPage = new System.Windows.Forms.RadioButton();
            this.butOpenPath = new System.Windows.Forms.Button();
            this.txtSaveFilePath = new System.Windows.Forms.TextBox();
            this.label57 = new System.Windows.Forms.Label();
            this.cmdEndWildcard = new System.Windows.Forms.Button();
            this.cmdStartWildcard = new System.Windows.Forms.Button();
            this.comNavLevel = new System.Windows.Forms.ComboBox();
            this.raNavPage = new System.Windows.Forms.RadioButton();
            this.raPage = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.IsMergeData = new System.Windows.Forms.CheckBox();
            this.txtRegion = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.comGetType = new System.Windows.Forms.ComboBox();
            this.label32 = new System.Windows.Forms.Label();
            this.comLimit = new System.Windows.Forms.ComboBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtGetEnd = new System.Windows.Forms.TextBox();
            this.txtGetStart = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.raNonePage = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.IsAutoDownloadOnlyImage = new System.Windows.Forms.CheckBox();
            this.raSmart = new System.Windows.Forms.RadioButton();
            this.raXPath = new System.Windows.Forms.RadioButton();
            this.raGather = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.IsAutoDownloadFileImage = new System.Windows.Forms.CheckBox();
            this.cmdVisual = new System.Windows.Forms.Button();
            this.txtXPath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comHNodeTextType = new System.Windows.Forms.ComboBox();
            this.cmdApply = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.rmenuStartWildcard = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuStartWildcardNum = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuStartWildcardLetter = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuStartWildcardAny = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuStartWildcardRegex = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEndWildcard = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuEndWildcardNum = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEndWildcardLetter = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEndWildcardAny = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuEndWildcardRegex = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cmdDownDealRule = new System.Windows.Forms.Button();
            this.cmdUpDealRule = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.dataDataRules = new System.Windows.Forms.DataGridView();
            this.ExportLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ExportRuleType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ExportRule = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtGetTitleName = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.rmenuStartWildcard.SuspendLayout();
            this.rmenuEndWildcard.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataDataRules)).BeginInit();
            this.SuspendLayout();
            // 
            // comMultiPage
            // 
            this.comMultiPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comMultiPage.Enabled = false;
            this.comMultiPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comMultiPage.FormattingEnabled = true;
            this.comMultiPage.Location = new System.Drawing.Point(498, 44);
            this.comMultiPage.Name = "comMultiPage";
            this.comMultiPage.Size = new System.Drawing.Size(194, 20);
            this.comMultiPage.TabIndex = 5;
            // 
            // raMultiPage
            // 
            this.raMultiPage.AutoSize = true;
            this.raMultiPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raMultiPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raMultiPage.Location = new System.Drawing.Point(368, 46);
            this.raMultiPage.Name = "raMultiPage";
            this.raMultiPage.Size = new System.Drawing.Size(124, 16);
            this.raMultiPage.TabIndex = 4;
            this.raMultiPage.TabStop = true;
            this.raMultiPage.Text = "多页采集 页面名称";
            this.raMultiPage.UseVisualStyleBackColor = true;
            this.raMultiPage.CheckedChanged += new System.EventHandler(this.raMultiPage_CheckedChanged);
            // 
            // butOpenPath
            // 
            this.butOpenPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butOpenPath.Image = ((System.Drawing.Image)(resources.GetObject("butOpenPath.Image")));
            this.butOpenPath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butOpenPath.Location = new System.Drawing.Point(668, 28);
            this.butOpenPath.Name = "butOpenPath";
            this.butOpenPath.Size = new System.Drawing.Size(21, 21);
            this.butOpenPath.TabIndex = 115;
            this.butOpenPath.UseVisualStyleBackColor = true;
            this.butOpenPath.Click += new System.EventHandler(this.butOpenPath_Click);
            // 
            // txtSaveFilePath
            // 
            this.txtSaveFilePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSaveFilePath.Enabled = false;
            this.txtSaveFilePath.Location = new System.Drawing.Point(445, 28);
            this.txtSaveFilePath.Name = "txtSaveFilePath";
            this.txtSaveFilePath.Size = new System.Drawing.Size(224, 21);
            this.txtSaveFilePath.TabIndex = 4;
            // 
            // label57
            // 
            this.label57.AutoSize = true;
            this.label57.Enabled = false;
            this.label57.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label57.Location = new System.Drawing.Point(355, 32);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(89, 12);
            this.label57.TabIndex = 111;
            this.label57.Text = "下载文件存储：";
            // 
            // cmdEndWildcard
            // 
            this.cmdEndWildcard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdEndWildcard.Image = ((System.Drawing.Image)(resources.GetObject("cmdEndWildcard.Image")));
            this.cmdEndWildcard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdEndWildcard.Location = new System.Drawing.Point(670, 113);
            this.cmdEndWildcard.Name = "cmdEndWildcard";
            this.cmdEndWildcard.Size = new System.Drawing.Size(20, 45);
            this.cmdEndWildcard.TabIndex = 15;
            this.cmdEndWildcard.UseVisualStyleBackColor = true;
            this.cmdEndWildcard.Click += new System.EventHandler(this.cmdEndWildcard_Click);
            // 
            // cmdStartWildcard
            // 
            this.cmdStartWildcard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStartWildcard.Image = ((System.Drawing.Image)(resources.GetObject("cmdStartWildcard.Image")));
            this.cmdStartWildcard.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdStartWildcard.Location = new System.Drawing.Point(669, 59);
            this.cmdStartWildcard.Name = "cmdStartWildcard";
            this.cmdStartWildcard.Size = new System.Drawing.Size(20, 45);
            this.cmdStartWildcard.TabIndex = 13;
            this.cmdStartWildcard.UseVisualStyleBackColor = true;
            this.cmdStartWildcard.Click += new System.EventHandler(this.cmdStartWildcard_Click);
            // 
            // comNavLevel
            // 
            this.comNavLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comNavLevel.Enabled = false;
            this.comNavLevel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comNavLevel.FormattingEnabled = true;
            this.comNavLevel.Location = new System.Drawing.Point(263, 44);
            this.comNavLevel.Name = "comNavLevel";
            this.comNavLevel.Size = new System.Drawing.Size(99, 20);
            this.comNavLevel.TabIndex = 3;
            // 
            // raNavPage
            // 
            this.raNavPage.AutoSize = true;
            this.raNavPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raNavPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raNavPage.Location = new System.Drawing.Point(147, 46);
            this.raNavPage.Name = "raNavPage";
            this.raNavPage.Size = new System.Drawing.Size(112, 16);
            this.raNavPage.TabIndex = 2;
            this.raNavPage.Text = "导航页 导航层级";
            this.raNavPage.UseVisualStyleBackColor = true;
            this.raNavPage.CheckedChanged += new System.EventHandler(this.raNavPage_CheckedChanged);
            // 
            // raPage
            // 
            this.raPage.AutoSize = true;
            this.raPage.Checked = true;
            this.raPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.raPage.Location = new System.Drawing.Point(23, 46);
            this.raPage.Name = "raPage";
            this.raPage.Size = new System.Drawing.Size(118, 16);
            this.raPage.TabIndex = 0;
            this.raPage.TabStop = true;
            this.raPage.Text = "采集页（内容页）";
            this.raPage.UseVisualStyleBackColor = true;
            this.raPage.CheckedChanged += new System.EventHandler(this.raPage_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(430, 215);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(185, 12);
            this.label13.TabIndex = 103;
            this.label13.Text = "典型应用：分页显示的新闻或文章";
            // 
            // IsMergeData
            // 
            this.IsMergeData.AutoSize = true;
            this.IsMergeData.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsMergeData.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsMergeData.Location = new System.Drawing.Point(108, 213);
            this.IsMergeData.Name = "IsMergeData";
            this.IsMergeData.Size = new System.Drawing.Size(322, 16);
            this.IsMergeData.TabIndex = 12;
            this.IsMergeData.Text = "如果自动翻页，则根据自动翻页采集数据的结果进行合并";
            this.IsMergeData.UseVisualStyleBackColor = true;
            // 
            // txtRegion
            // 
            this.txtRegion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtRegion.Enabled = false;
            this.txtRegion.Location = new System.Drawing.Point(445, 165);
            this.txtRegion.Name = "txtRegion";
            this.txtRegion.Size = new System.Drawing.Size(245, 21);
            this.txtRegion.TabIndex = 10;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label35.Location = new System.Drawing.Point(364, 171);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(77, 12);
            this.label35.TabIndex = 100;
            this.label35.Text = "正则表达式：";
            // 
            // comGetType
            // 
            this.comGetType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comGetType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comGetType.FormattingEnabled = true;
            this.comGetType.Location = new System.Drawing.Point(109, 29);
            this.comGetType.Name = "comGetType";
            this.comGetType.Size = new System.Drawing.Size(245, 20);
            this.comGetType.TabIndex = 3;
            this.comGetType.SelectedIndexChanged += new System.EventHandler(this.comGetType_SelectedIndexChanged);
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label32.Location = new System.Drawing.Point(16, 33);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(89, 12);
            this.label32.TabIndex = 98;
            this.label32.Text = "采集数据类型：";
            // 
            // comLimit
            // 
            this.comLimit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comLimit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comLimit.FormattingEnabled = true;
            this.comLimit.Location = new System.Drawing.Point(108, 164);
            this.comLimit.Name = "comLimit";
            this.comLimit.Size = new System.Drawing.Size(244, 20);
            this.comLimit.TabIndex = 9;
            this.comLimit.SelectedIndexChanged += new System.EventHandler(this.comLimit_SelectedIndexChanged);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(40, 166);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(65, 12);
            this.label19.TabIndex = 97;
            this.label19.Text = "匹配条件：";
            // 
            // txtGetEnd
            // 
            this.txtGetEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGetEnd.Location = new System.Drawing.Point(108, 113);
            this.txtGetEnd.Multiline = true;
            this.txtGetEnd.Name = "txtGetEnd";
            this.txtGetEnd.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetEnd.Size = new System.Drawing.Size(561, 45);
            this.txtGetEnd.TabIndex = 8;
            // 
            // txtGetStart
            // 
            this.txtGetStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGetStart.Location = new System.Drawing.Point(108, 59);
            this.txtGetStart.Multiline = true;
            this.txtGetStart.Name = "txtGetStart";
            this.txtGetStart.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtGetStart.Size = new System.Drawing.Size(561, 45);
            this.txtGetStart.TabIndex = 7;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(40, 62);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(65, 12);
            this.label16.TabIndex = 94;
            this.label16.Text = "起始位置：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(40, 116);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(65, 12);
            this.label17.TabIndex = 95;
            this.label17.Text = "终止位置：";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.label18.ForeColor = System.Drawing.Color.Navy;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(20, 12);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(98, 13);
            this.label18.TabIndex = 96;
            this.label18.Text = "采集数据名称：";
            // 
            // raNonePage
            // 
            this.raNonePage.AutoSize = true;
            this.raNonePage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raNonePage.Location = new System.Drawing.Point(396, 11);
            this.raNonePage.Name = "raNonePage";
            this.raNonePage.Size = new System.Drawing.Size(298, 16);
            this.raNonePage.TabIndex = 1;
            this.raNonePage.TabStop = true;
            this.raNonePage.Text = "此数据不从网页中获取，而是由数据加工规则制定。";
            this.raNonePage.UseVisualStyleBackColor = true;
            this.raNonePage.CheckedChanged += new System.EventHandler(this.raNonePage_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.raNonePage);
            this.groupBox2.Controls.Add(this.IsAutoDownloadOnlyImage);
            this.groupBox2.Controls.Add(this.raSmart);
            this.groupBox2.Controls.Add(this.raXPath);
            this.groupBox2.Controls.Add(this.raGather);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.IsAutoDownloadFileImage);
            this.groupBox2.Controls.Add(this.cmdVisual);
            this.groupBox2.Controls.Add(this.comGetType);
            this.groupBox2.Controls.Add(this.txtXPath);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.butOpenPath);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.comHNodeTextType);
            this.groupBox2.Controls.Add(this.cmdEndWildcard);
            this.groupBox2.Controls.Add(this.IsMergeData);
            this.groupBox2.Controls.Add(this.cmdStartWildcard);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.comLimit);
            this.groupBox2.Controls.Add(this.txtRegion);
            this.groupBox2.Controls.Add(this.txtSaveFilePath);
            this.groupBox2.Controls.Add(this.label57);
            this.groupBox2.Controls.Add(this.txtGetStart);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.txtGetEnd);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label35);
            this.groupBox2.Location = new System.Drawing.Point(10, 70);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(697, 238);
            this.groupBox2.TabIndex = 119;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "采集规则配置";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // IsAutoDownloadOnlyImage
            // 
            this.IsAutoDownloadOnlyImage.AutoSize = true;
            this.IsAutoDownloadOnlyImage.Enabled = false;
            this.IsAutoDownloadOnlyImage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsAutoDownloadOnlyImage.Location = new System.Drawing.Point(550, 191);
            this.IsAutoDownloadOnlyImage.Name = "IsAutoDownloadOnlyImage";
            this.IsAutoDownloadOnlyImage.Size = new System.Drawing.Size(82, 16);
            this.IsAutoDownloadOnlyImage.TabIndex = 129;
            this.IsAutoDownloadOnlyImage.Text = "仅下载图片";
            this.IsAutoDownloadOnlyImage.UseVisualStyleBackColor = true;
            // 
            // raSmart
            // 
            this.raSmart.AutoSize = true;
            this.raSmart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raSmart.Location = new System.Drawing.Point(318, 11);
            this.raSmart.Name = "raSmart";
            this.raSmart.Size = new System.Drawing.Size(70, 16);
            this.raSmart.TabIndex = 2;
            this.raSmart.TabStop = true;
            this.raSmart.Text = "智能采集";
            this.raSmart.UseVisualStyleBackColor = true;
            this.raSmart.CheckedChanged += new System.EventHandler(this.raSmart_CheckedChanged);
            // 
            // raXPath
            // 
            this.raXPath.AutoSize = true;
            this.raXPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raXPath.Location = new System.Drawing.Point(210, 11);
            this.raXPath.Name = "raXPath";
            this.raXPath.Size = new System.Drawing.Size(82, 16);
            this.raXPath.TabIndex = 1;
            this.raXPath.Text = "可视化配置";
            this.raXPath.UseVisualStyleBackColor = true;
            this.raXPath.CheckedChanged += new System.EventHandler(this.raXPath_CheckedChanged);
            // 
            // raGather
            // 
            this.raGather.AutoSize = true;
            this.raGather.Checked = true;
            this.raGather.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raGather.Location = new System.Drawing.Point(109, 11);
            this.raGather.Name = "raGather";
            this.raGather.Size = new System.Drawing.Size(70, 16);
            this.raGather.TabIndex = 0;
            this.raGather.TabStop = true;
            this.raGather.Text = "常规配置";
            this.raGather.UseVisualStyleBackColor = true;
            this.raGather.CheckedChanged += new System.EventHandler(this.raGather_CheckedChanged);
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label4.Location = new System.Drawing.Point(25, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 30);
            this.label4.TabIndex = 128;
            this.label4.Text = "（所采集数据紧跟的字符）";
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label3.Location = new System.Drawing.Point(22, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 30);
            this.label3.TabIndex = 127;
            this.label3.Text = "（所采集数据前面的字符）";
            // 
            // IsAutoDownloadFileImage
            // 
            this.IsAutoDownloadFileImage.AutoSize = true;
            this.IsAutoDownloadFileImage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsAutoDownloadFileImage.Location = new System.Drawing.Point(108, 191);
            this.IsAutoDownloadFileImage.Name = "IsAutoDownloadFileImage";
            this.IsAutoDownloadFileImage.Size = new System.Drawing.Size(436, 16);
            this.IsAutoDownloadFileImage.TabIndex = 11;
            this.IsAutoDownloadFileImage.Text = "如果此采集内容中包含文件（PDF、Office、压缩文件）或图片，则自动下载。";
            this.IsAutoDownloadFileImage.UseVisualStyleBackColor = true;
            this.IsAutoDownloadFileImage.CheckedChanged += new System.EventHandler(this.IsAutoDownloadImage_CheckedChanged);
            this.IsAutoDownloadFileImage.Click += new System.EventHandler(this.IsAutoDownloadImage_Click);
            // 
            // cmdVisual
            // 
            this.cmdVisual.Enabled = false;
            this.cmdVisual.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdVisual.Image = ((System.Drawing.Image)(resources.GetObject("cmdVisual.Image")));
            this.cmdVisual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdVisual.Location = new System.Drawing.Point(597, 164);
            this.cmdVisual.Name = "cmdVisual";
            this.cmdVisual.Size = new System.Drawing.Size(94, 21);
            this.cmdVisual.TabIndex = 117;
            this.cmdVisual.Text = "可视化提取";
            this.cmdVisual.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdVisual.UseVisualStyleBackColor = true;
            this.cmdVisual.Visible = false;
            this.cmdVisual.Click += new System.EventHandler(this.cmdVisual_Click);
            // 
            // txtXPath
            // 
            this.txtXPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtXPath.Enabled = false;
            this.txtXPath.Location = new System.Drawing.Point(107, 83);
            this.txtXPath.Multiline = true;
            this.txtXPath.Name = "txtXPath";
            this.txtXPath.Size = new System.Drawing.Size(583, 76);
            this.txtXPath.TabIndex = 6;
            this.txtXPath.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 123;
            this.label2.Text = "节点属性：";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(18, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 121;
            this.label1.Text = "xPath表达式：";
            this.label1.Visible = false;
            // 
            // comHNodeTextType
            // 
            this.comHNodeTextType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comHNodeTextType.Enabled = false;
            this.comHNodeTextType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comHNodeTextType.FormattingEnabled = true;
            this.comHNodeTextType.Location = new System.Drawing.Point(109, 58);
            this.comHNodeTextType.Name = "comHNodeTextType";
            this.comHNodeTextType.Size = new System.Drawing.Size(580, 20);
            this.comHNodeTextType.TabIndex = 5;
            this.comHNodeTextType.Visible = false;
            // 
            // cmdApply
            // 
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Image = ((System.Drawing.Image)(resources.GetObject("cmdApply.Image")));
            this.cmdApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdApply.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdApply.Location = new System.Drawing.Point(566, 503);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(65, 22);
            this.cmdApply.TabIndex = 1;
            this.cmdApply.Text = "确 定";
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
            this.cmdCancel.Location = new System.Drawing.Point(639, 503);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "取 消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // rmenuStartWildcard
            // 
            this.rmenuStartWildcard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuStartWildcardNum,
            this.rmenuStartWildcardLetter,
            this.rmenuStartWildcardAny,
            this.toolStripSeparator7,
            this.rmenuStartWildcardRegex});
            this.rmenuStartWildcard.Name = "contextMenuStrip2";
            this.rmenuStartWildcard.Size = new System.Drawing.Size(173, 98);
            // 
            // rmenuStartWildcardNum
            // 
            this.rmenuStartWildcardNum.Name = "rmenuStartWildcardNum";
            this.rmenuStartWildcardNum.Size = new System.Drawing.Size(172, 22);
            this.rmenuStartWildcardNum.Text = "数字通配符";
            this.rmenuStartWildcardNum.Click += new System.EventHandler(this.rmenuStartWildcardNum_Click);
            // 
            // rmenuStartWildcardLetter
            // 
            this.rmenuStartWildcardLetter.Name = "rmenuStartWildcardLetter";
            this.rmenuStartWildcardLetter.Size = new System.Drawing.Size(172, 22);
            this.rmenuStartWildcardLetter.Text = "字母通配符";
            this.rmenuStartWildcardLetter.Click += new System.EventHandler(this.rmenuStartWildcardLetter_Click);
            // 
            // rmenuStartWildcardAny
            // 
            this.rmenuStartWildcardAny.Name = "rmenuStartWildcardAny";
            this.rmenuStartWildcardAny.Size = new System.Drawing.Size(172, 22);
            this.rmenuStartWildcardAny.Text = "任意通配符";
            this.rmenuStartWildcardAny.Click += new System.EventHandler(this.rmenuStartWildcardAny_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(169, 6);
            // 
            // rmenuStartWildcardRegex
            // 
            this.rmenuStartWildcardRegex.Name = "rmenuStartWildcardRegex";
            this.rmenuStartWildcardRegex.Size = new System.Drawing.Size(172, 22);
            this.rmenuStartWildcardRegex.Text = "自定义正则通配符";
            this.rmenuStartWildcardRegex.Click += new System.EventHandler(this.rmenuStartWildcardRegex_Click);
            // 
            // rmenuEndWildcard
            // 
            this.rmenuEndWildcard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuEndWildcardNum,
            this.rmenuEndWildcardLetter,
            this.rmenuEndWildcardAny,
            this.toolStripSeparator8,
            this.rmenuEndWildcardRegex});
            this.rmenuEndWildcard.Name = "rmenuEndWildcard";
            this.rmenuEndWildcard.Size = new System.Drawing.Size(173, 98);
            // 
            // rmenuEndWildcardNum
            // 
            this.rmenuEndWildcardNum.Name = "rmenuEndWildcardNum";
            this.rmenuEndWildcardNum.Size = new System.Drawing.Size(172, 22);
            this.rmenuEndWildcardNum.Text = "数字通配符";
            this.rmenuEndWildcardNum.Click += new System.EventHandler(this.rmenuEndWildcardNum_Click);
            // 
            // rmenuEndWildcardLetter
            // 
            this.rmenuEndWildcardLetter.Name = "rmenuEndWildcardLetter";
            this.rmenuEndWildcardLetter.Size = new System.Drawing.Size(172, 22);
            this.rmenuEndWildcardLetter.Text = "字母通配符";
            this.rmenuEndWildcardLetter.Click += new System.EventHandler(this.rmenuEndWildcardLetter_Click);
            // 
            // rmenuEndWildcardAny
            // 
            this.rmenuEndWildcardAny.Name = "rmenuEndWildcardAny";
            this.rmenuEndWildcardAny.Size = new System.Drawing.Size(172, 22);
            this.rmenuEndWildcardAny.Text = "任意通配符";
            this.rmenuEndWildcardAny.Click += new System.EventHandler(this.rmenuEndWildcardAny_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(169, 6);
            // 
            // rmenuEndWildcardRegex
            // 
            this.rmenuEndWildcardRegex.Name = "rmenuEndWildcardRegex";
            this.rmenuEndWildcardRegex.Size = new System.Drawing.Size(172, 22);
            this.rmenuEndWildcardRegex.Text = "自定义正则通配符";
            this.rmenuEndWildcardRegex.Click += new System.EventHandler(this.rmenuEndWildcardRegex_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cmdDownDealRule);
            this.groupBox3.Controls.Add(this.cmdUpDealRule);
            this.groupBox3.Controls.Add(this.button17);
            this.groupBox3.Controls.Add(this.button4);
            this.groupBox3.Controls.Add(this.dataDataRules);
            this.groupBox3.Location = new System.Drawing.Point(10, 314);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(697, 180);
            this.groupBox3.TabIndex = 122;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "指定此数据项采集完成后的加工规则";
            // 
            // cmdDownDealRule
            // 
            this.cmdDownDealRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDownDealRule.Image = ((System.Drawing.Image)(resources.GetObject("cmdDownDealRule.Image")));
            this.cmdDownDealRule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDownDealRule.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDownDealRule.Location = new System.Drawing.Point(638, 104);
            this.cmdDownDealRule.Name = "cmdDownDealRule";
            this.cmdDownDealRule.Size = new System.Drawing.Size(56, 23);
            this.cmdDownDealRule.TabIndex = 3;
            this.cmdDownDealRule.Text = "下移";
            this.cmdDownDealRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDownDealRule.UseVisualStyleBackColor = true;
            this.cmdDownDealRule.Click += new System.EventHandler(this.cmdDownDealRule_Click);
            // 
            // cmdUpDealRule
            // 
            this.cmdUpDealRule.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdUpDealRule.Image = ((System.Drawing.Image)(resources.GetObject("cmdUpDealRule.Image")));
            this.cmdUpDealRule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdUpDealRule.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdUpDealRule.Location = new System.Drawing.Point(638, 75);
            this.cmdUpDealRule.Name = "cmdUpDealRule";
            this.cmdUpDealRule.Size = new System.Drawing.Size(56, 23);
            this.cmdUpDealRule.TabIndex = 2;
            this.cmdUpDealRule.Text = "上移";
            this.cmdUpDealRule.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdUpDealRule.UseVisualStyleBackColor = true;
            this.cmdUpDealRule.Click += new System.EventHandler(this.cmdUpDealRule_Click);
            // 
            // button17
            // 
            this.button17.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button17.Image = ((System.Drawing.Image)(resources.GetObject("button17.Image")));
            this.button17.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button17.Location = new System.Drawing.Point(638, 17);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(56, 23);
            this.button17.TabIndex = 0;
            this.button17.Text = "增加";
            this.button17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button4.Location = new System.Drawing.Point(638, 46);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "删除";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // dataDataRules
            // 
            this.dataDataRules.BackgroundColor = System.Drawing.Color.White;
            this.dataDataRules.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataDataRules.ColumnHeadersHeight = 21;
            this.dataDataRules.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataDataRules.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ExportLevel,
            this.ExportRuleType,
            this.ExportRule});
            this.dataDataRules.Location = new System.Drawing.Point(13, 17);
            this.dataDataRules.Name = "dataDataRules";
            this.dataDataRules.RowHeadersWidth = 21;
            this.dataDataRules.RowTemplate.Height = 23;
            this.dataDataRules.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataDataRules.Size = new System.Drawing.Size(619, 157);
            this.dataDataRules.TabIndex = 2;
            this.dataDataRules.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataDataRules_EditingControlShowing);
            this.dataDataRules.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataDataRules_RowsAdded);
            this.dataDataRules.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dataDataRules_RowsRemoved);
            // 
            // ExportLevel
            // 
            this.ExportLevel.FillWeight = 40F;
            this.ExportLevel.HeaderText = "序号";
            this.ExportLevel.Name = "ExportLevel";
            this.ExportLevel.ReadOnly = true;
            this.ExportLevel.Width = 40;
            // 
            // ExportRuleType
            // 
            this.ExportRuleType.FillWeight = 80F;
            this.ExportRuleType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExportRuleType.HeaderText = "输出规则";
            this.ExportRuleType.Name = "ExportRuleType";
            this.ExportRuleType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ExportRuleType.Width = 240;
            // 
            // ExportRule
            // 
            this.ExportRule.FillWeight = 80F;
            this.ExportRule.HeaderText = "条件";
            this.ExportRule.Name = "ExportRule";
            this.ExportRule.Width = 240;
            // 
            // txtGetTitleName
            // 
            this.txtGetTitleName.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.txtGetTitleName.FormattingEnabled = true;
            this.txtGetTitleName.Location = new System.Drawing.Point(117, 8);
            this.txtGetTitleName.Name = "txtGetTitleName";
            this.txtGetTitleName.Size = new System.Drawing.Size(576, 20);
            this.txtGetTitleName.TabIndex = 0;
            this.txtGetTitleName.SelectedIndexChanged += new System.EventHandler(this.txtGetTitleName_SelectedIndexChanged);
            // 
            // frmAddGatherRule
            // 
            this.AcceptButton = this.cmdApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 537);
            this.Controls.Add(this.comMultiPage);
            this.Controls.Add(this.raPage);
            this.Controls.Add(this.raMultiPage);
            this.Controls.Add(this.raNavPage);
            this.Controls.Add(this.txtGetTitleName);
            this.Controls.Add(this.comNavLevel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdApply);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddGatherRule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "采集规则信息维护";
            this.Load += new System.EventHandler(this.frmAddGatherRule_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.rmenuStartWildcard.ResumeLayout(false);
            this.rmenuEndWildcard.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataDataRules)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comMultiPage;
        private System.Windows.Forms.RadioButton raMultiPage;
        private System.Windows.Forms.Button butOpenPath;
        private System.Windows.Forms.TextBox txtSaveFilePath;
        private System.Windows.Forms.Label label57;
        private System.Windows.Forms.Button cmdEndWildcard;
        private System.Windows.Forms.Button cmdStartWildcard;
        private System.Windows.Forms.ComboBox comNavLevel;
        private System.Windows.Forms.RadioButton raNavPage;
        private System.Windows.Forms.RadioButton raPage;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox IsMergeData;
        private System.Windows.Forms.TextBox txtRegion;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.ComboBox comGetType;
        private System.Windows.Forms.Label label32;
        private System.Windows.Forms.ComboBox comLimit;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtGetEnd;
        private System.Windows.Forms.TextBox txtGetStart;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ContextMenuStrip rmenuStartWildcard;
        private System.Windows.Forms.ToolStripMenuItem rmenuStartWildcardNum;
        private System.Windows.Forms.ToolStripMenuItem rmenuStartWildcardLetter;
        private System.Windows.Forms.ToolStripMenuItem rmenuStartWildcardAny;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem rmenuStartWildcardRegex;
        private System.Windows.Forms.ContextMenuStrip rmenuEndWildcard;
        private System.Windows.Forms.ToolStripMenuItem rmenuEndWildcardNum;
        private System.Windows.Forms.ToolStripMenuItem rmenuEndWildcardLetter;
        private System.Windows.Forms.ToolStripMenuItem rmenuEndWildcardAny;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem rmenuEndWildcardRegex;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.RadioButton raNonePage;
        private System.Windows.Forms.Button cmdVisual;
        private System.Windows.Forms.RadioButton raXPath;
        private System.Windows.Forms.RadioButton raGather;
        private System.Windows.Forms.TextBox txtXPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comHNodeTextType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IsAutoDownloadFileImage;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton raSmart;
        private System.Windows.Forms.CheckBox IsAutoDownloadOnlyImage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dataDataRules;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExportLevel;
        private System.Windows.Forms.DataGridViewComboBoxColumn ExportRuleType;
        private System.Windows.Forms.DataGridViewTextBoxColumn ExportRule;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button cmdDownDealRule;
        private System.Windows.Forms.Button cmdUpDealRule;
        private System.Windows.Forms.ComboBox txtGetTitleName;
    }
}