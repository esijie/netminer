namespace MinerSpider
{
    partial class frmVisual
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVisual));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this._ActiveWebBrowser = new SoukeyControl.CustomControl.cMyBrowser(this.components);
            this.WebBrowserTab = new System.Windows.Forms.TabControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.raPage = new System.Windows.Forms.RadioButton();
            this.raClick = new System.Windows.Forms.RadioButton();
            this.raUrl = new System.Windows.Forms.RadioButton();
            this.isMulti = new System.Windows.Forms.CheckBox();
            this.raOuterHtml = new System.Windows.Forms.RadioButton();
            this.raInnerText = new System.Windows.Forms.RadioButton();
            this.raInnerHtml = new System.Windows.Forms.RadioButton();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtxPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.txtUrl = new System.Windows.Forms.ComboBox();
            this.butUrl = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAllowLink = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolOkExit = new System.Windows.Forms.ToolStripButton();
            this.toolCancleExit = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.staInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.tabPage3.SuspendLayout();
            this.WebBrowserTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage3
            // 
            this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage3.Controls.Add(this._ActiveWebBrowser);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(997, 225);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "网页浏览";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // _ActiveWebBrowser
            // 
            this._ActiveWebBrowser.BlockEvent = false;
            this._ActiveWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ActiveWebBrowser.FrameEventsPropagateToMainWindow = false;
            this._ActiveWebBrowser.Location = new System.Drawing.Point(0, 0);
            this._ActiveWebBrowser.Name = "_ActiveWebBrowser";
            this._ActiveWebBrowser.Size = new System.Drawing.Size(995, 223);
            this._ActiveWebBrowser.TabIndex = 0;
            this._ActiveWebBrowser.UseHttpActivityObserver = false;
            // 
            // WebBrowserTab
            // 
            this.WebBrowserTab.Controls.Add(this.tabPage3);
            this.WebBrowserTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.WebBrowserTab.Location = new System.Drawing.Point(0, 0);
            this.WebBrowserTab.Name = "WebBrowserTab";
            this.WebBrowserTab.SelectedIndex = 0;
            this.WebBrowserTab.Size = new System.Drawing.Size(1005, 251);
            this.WebBrowserTab.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 47);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.WebBrowserTab);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer2.Panel2.Controls.Add(this.panel1);
            this.splitContainer2.Size = new System.Drawing.Size(1005, 452);
            this.splitContainer2.SplitterDistance = 251;
            this.splitContainer2.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.raPage);
            this.panel1.Controls.Add(this.raClick);
            this.panel1.Controls.Add(this.raUrl);
            this.panel1.Controls.Add(this.isMulti);
            this.panel1.Controls.Add(this.raOuterHtml);
            this.panel1.Controls.Add(this.raInnerText);
            this.panel1.Controls.Add(this.raInnerHtml);
            this.panel1.Controls.Add(this.txtResult);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.txtxPath);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1005, 197);
            this.panel1.TabIndex = 0;
            // 
            // raPage
            // 
            this.raPage.AutoSize = true;
            this.raPage.Location = new System.Drawing.Point(523, 5);
            this.raPage.Name = "raPage";
            this.raPage.Size = new System.Drawing.Size(83, 16);
            this.raPage.TabIndex = 10;
            this.raPage.Text = "滚动到页尾";
            this.raPage.UseVisualStyleBackColor = true;
            this.raPage.Visible = false;
            this.raPage.Click += new System.EventHandler(this.raPage_Click);
            // 
            // raClick
            // 
            this.raClick.AutoSize = true;
            this.raClick.Location = new System.Drawing.Point(470, 5);
            this.raClick.Name = "raClick";
            this.raClick.Size = new System.Drawing.Size(47, 16);
            this.raClick.TabIndex = 9;
            this.raClick.Text = "点击";
            this.raClick.UseVisualStyleBackColor = true;
            this.raClick.Visible = false;
            this.raClick.Click += new System.EventHandler(this.raClick_Click);
            // 
            // raUrl
            // 
            this.raUrl.AutoSize = true;
            this.raUrl.Location = new System.Drawing.Point(99, 5);
            this.raUrl.Name = "raUrl";
            this.raUrl.Size = new System.Drawing.Size(71, 16);
            this.raUrl.TabIndex = 8;
            this.raUrl.Text = "捕获元素";
            this.raUrl.UseVisualStyleBackColor = true;
            this.raUrl.Click += new System.EventHandler(this.raUrl_Click);
            // 
            // isMulti
            // 
            this.isMulti.AutoSize = true;
            this.isMulti.Location = new System.Drawing.Point(176, 5);
            this.isMulti.Name = "isMulti";
            this.isMulti.Size = new System.Drawing.Size(288, 16);
            this.isMulti.TabIndex = 7;
            this.isMulti.Text = "多条记录，请通过鼠标捕获第一条和最后一条记录";
            this.isMulti.UseVisualStyleBackColor = true;
            this.isMulti.CheckedChanged += new System.EventHandler(this.isMulti_CheckedChanged);
            // 
            // raOuterHtml
            // 
            this.raOuterHtml.AutoSize = true;
            this.raOuterHtml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raOuterHtml.Location = new System.Drawing.Point(352, 57);
            this.raOuterHtml.Name = "raOuterHtml";
            this.raOuterHtml.Size = new System.Drawing.Size(76, 16);
            this.raOuterHtml.TabIndex = 6;
            this.raOuterHtml.Text = "OuterHtml";
            this.raOuterHtml.UseVisualStyleBackColor = true;
            // 
            // raInnerText
            // 
            this.raInnerText.AutoSize = true;
            this.raInnerText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raInnerText.Location = new System.Drawing.Point(261, 57);
            this.raInnerText.Name = "raInnerText";
            this.raInnerText.Size = new System.Drawing.Size(76, 16);
            this.raInnerText.TabIndex = 5;
            this.raInnerText.Text = "InnerText";
            this.raInnerText.UseVisualStyleBackColor = true;
            // 
            // raInnerHtml
            // 
            this.raInnerHtml.AutoSize = true;
            this.raInnerHtml.Checked = true;
            this.raInnerHtml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.raInnerHtml.Location = new System.Drawing.Point(178, 57);
            this.raInnerHtml.Name = "raInnerHtml";
            this.raInnerHtml.Size = new System.Drawing.Size(76, 16);
            this.raInnerHtml.TabIndex = 4;
            this.raInnerHtml.TabStop = true;
            this.raInnerHtml.Text = "InnerHtml";
            this.raInnerHtml.UseVisualStyleBackColor = true;
            // 
            // txtResult
            // 
            this.txtResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtResult.Location = new System.Drawing.Point(99, 78);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResult.Size = new System.Drawing.Size(900, 114);
            this.txtResult.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(99, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 21);
            this.button1.TabIndex = 2;
            this.button1.Text = "测 试";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtxPath
            // 
            this.txtxPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtxPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtxPath.Location = new System.Drawing.Point(99, 28);
            this.txtxPath.Name = "txtxPath";
            this.txtxPath.Size = new System.Drawing.Size(901, 21);
            this.txtxPath.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "xPath表达式：";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtUrl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.butUrl);
            this.splitContainer1.Size = new System.Drawing.Size(1005, 22);
            this.splitContainer1.SplitterDistance = 943;
            this.splitContainer1.TabIndex = 16;
            // 
            // txtUrl
            // 
            this.txtUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtUrl.FormattingEnabled = true;
            this.txtUrl.Location = new System.Drawing.Point(0, 0);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(943, 20);
            this.txtUrl.TabIndex = 0;
            this.txtUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrl_KeyDown);
            // 
            // butUrl
            // 
            this.butUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.butUrl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butUrl.Image = ((System.Drawing.Image)(resources.GetObject("butUrl.Image")));
            this.butUrl.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.butUrl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butUrl.Location = new System.Drawing.Point(0, 0);
            this.butUrl.Name = "butUrl";
            this.butUrl.Size = new System.Drawing.Size(58, 22);
            this.butUrl.TabIndex = 2;
            this.butUrl.Text = "转到";
            this.butUrl.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.butUrl.UseVisualStyleBackColor = true;
            this.butUrl.Click += new System.EventHandler(this.butUrl_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAllowLink,
            this.toolStripSeparator1,
            this.toolOkExit,
            this.toolCancleExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1005, 25);
            this.toolStrip1.TabIndex = 18;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAllowLink
            // 
            this.toolAllowLink.Image = ((System.Drawing.Image)(resources.GetObject("toolAllowLink.Image")));
            this.toolAllowLink.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolAllowLink.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAllowLink.Name = "toolAllowLink";
            this.toolAllowLink.Size = new System.Drawing.Size(76, 22);
            this.toolAllowLink.Text = "开始捕获";
            this.toolAllowLink.Click += new System.EventHandler(this.toolAllowLink_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staInfo,
            this.ProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 499);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1005, 22);
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // staInfo
            // 
            this.staInfo.Name = "staInfo";
            this.staInfo.Size = new System.Drawing.Size(888, 17);
            this.staInfo.Spring = true;
            this.staInfo.Text = "就绪";
            this.staInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // frmVisual
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 521);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmVisual";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "可视化采集配置器";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmVisual_Load);
            this.tabPage3.ResumeLayout(false);
            this.WebBrowserTab.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabControl WebBrowserTab;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ComboBox txtUrl;
        private System.Windows.Forms.Button butUrl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolOkExit;
        private System.Windows.Forms.ToolStripButton toolCancleExit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtxPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolStripButton toolAllowLink;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.RadioButton raOuterHtml;
        private System.Windows.Forms.RadioButton raInnerText;
        private System.Windows.Forms.RadioButton raInnerHtml;
        private System.Windows.Forms.CheckBox isMulti;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel staInfo;
        private System.Windows.Forms.ToolStripProgressBar ProgressBar1;
        private SoukeyControl.CustomControl.cMyBrowser _ActiveWebBrowser;
        private System.Windows.Forms.RadioButton raPage;
        private System.Windows.Forms.RadioButton raClick;
        private System.Windows.Forms.RadioButton raUrl;
    }
}