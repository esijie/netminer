namespace SMHttpComposer
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.label1 = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.isA = new System.Windows.Forms.CheckBox();
            this.txtProxyPort = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtProxyAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.raCustomProxy = new System.Windows.Forms.RadioButton();
            this.raSystemProxy = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.raPOST = new System.Windows.Forms.RadioButton();
            this.raGet = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.comPCode = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comwCode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPostData = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.txtResponseHeader = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.txtWebSource = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.webBrowser1 = new System.Windows.Forms.WebBrowser();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(651, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(69, 22);
            this.toolStripButton1.Text = "开始请求";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(52, 22);
            this.toolStripButton2.Text = "退出";
            this.toolStripButton2.Click += new System.EventHandler(this.toolStripButton2_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 412);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(651, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "地址：";
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(59, 28);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(580, 21);
            this.txtUrl.TabIndex = 3;
            this.txtUrl.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtUrl_KeyDown);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(8, 55);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(631, 354);
            this.tabControl1.TabIndex = 4;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.isA);
            this.tabPage1.Controls.Add(this.txtProxyPort);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.txtProxyAddress);
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.raCustomProxy);
            this.tabPage1.Controls.Add(this.raSystemProxy);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.comPCode);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.comwCode);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.txtPostData);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.txtHeader);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(623, 325);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "请求信息";
            // 
            // isA
            // 
            this.isA.AutoSize = true;
            this.isA.Location = new System.Drawing.Point(264, 36);
            this.isA.Name = "isA";
            this.isA.Size = new System.Drawing.Size(96, 16);
            this.isA.TabIndex = 18;
            this.isA.Text = "是否自动跳转";
            this.isA.UseVisualStyleBackColor = true;
            // 
            // txtProxyPort
            // 
            this.txtProxyPort.Enabled = false;
            this.txtProxyPort.Location = new System.Drawing.Point(366, 58);
            this.txtProxyPort.Name = "txtProxyPort";
            this.txtProxyPort.Size = new System.Drawing.Size(138, 21);
            this.txtProxyPort.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(319, 62);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "端口：";
            // 
            // txtProxyAddress
            // 
            this.txtProxyAddress.Enabled = false;
            this.txtProxyAddress.Location = new System.Drawing.Point(149, 58);
            this.txtProxyAddress.Name = "txtProxyAddress";
            this.txtProxyAddress.Size = new System.Drawing.Size(164, 21);
            this.txtProxyAddress.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(83, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 12);
            this.label7.TabIndex = 14;
            this.label7.Text = "代理地址：";
            // 
            // raCustomProxy
            // 
            this.raCustomProxy.AutoSize = true;
            this.raCustomProxy.Location = new System.Drawing.Point(166, 37);
            this.raCustomProxy.Name = "raCustomProxy";
            this.raCustomProxy.Size = new System.Drawing.Size(83, 16);
            this.raCustomProxy.TabIndex = 13;
            this.raCustomProxy.TabStop = true;
            this.raCustomProxy.Text = "自定义代理";
            this.raCustomProxy.UseVisualStyleBackColor = true;
            this.raCustomProxy.CheckedChanged += new System.EventHandler(this.raCustomProxy_CheckedChanged);
            // 
            // raSystemProxy
            // 
            this.raSystemProxy.AutoSize = true;
            this.raSystemProxy.Checked = true;
            this.raSystemProxy.Location = new System.Drawing.Point(85, 37);
            this.raSystemProxy.Name = "raSystemProxy";
            this.raSystemProxy.Size = new System.Drawing.Size(71, 16);
            this.raSystemProxy.TabIndex = 12;
            this.raSystemProxy.TabStop = true;
            this.raSystemProxy.Text = "系统设置";
            this.raSystemProxy.UseVisualStyleBackColor = true;
            this.raSystemProxy.CheckedChanged += new System.EventHandler(this.raSystemProxy_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.raPOST);
            this.panel1.Controls.Add(this.raGet);
            this.panel1.Location = new System.Drawing.Point(85, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(105, 20);
            this.panel1.TabIndex = 11;
            // 
            // raPOST
            // 
            this.raPOST.AutoSize = true;
            this.raPOST.Location = new System.Drawing.Point(47, 2);
            this.raPOST.Name = "raPOST";
            this.raPOST.Size = new System.Drawing.Size(47, 16);
            this.raPOST.TabIndex = 3;
            this.raPOST.Text = "POST";
            this.raPOST.UseVisualStyleBackColor = true;
            this.raPOST.CheckedChanged += new System.EventHandler(this.raPOST_CheckedChanged);
            // 
            // raGet
            // 
            this.raGet.AutoSize = true;
            this.raGet.Checked = true;
            this.raGet.Location = new System.Drawing.Point(0, 2);
            this.raGet.Name = "raGet";
            this.raGet.Size = new System.Drawing.Size(41, 16);
            this.raGet.TabIndex = 2;
            this.raGet.TabStop = true;
            this.raGet.Text = "GET";
            this.raGet.UseVisualStyleBackColor = true;
            this.raGet.CheckedChanged += new System.EventHandler(this.raGet_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 12);
            this.label6.TabIndex = 10;
            this.label6.Text = "代理设置：";
            // 
            // comPCode
            // 
            this.comPCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPCode.FormattingEnabled = true;
            this.comPCode.Location = new System.Drawing.Point(494, 10);
            this.comPCode.Name = "comPCode";
            this.comPCode.Size = new System.Drawing.Size(109, 20);
            this.comPCode.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(394, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "POST Data编码：";
            // 
            // comwCode
            // 
            this.comwCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comwCode.FormattingEnabled = true;
            this.comwCode.Location = new System.Drawing.Point(260, 10);
            this.comwCode.Name = "comwCode";
            this.comwCode.Size = new System.Drawing.Size(121, 20);
            this.comwCode.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(196, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "网页编码：";
            // 
            // txtPostData
            // 
            this.txtPostData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPostData.Enabled = false;
            this.txtPostData.Location = new System.Drawing.Point(83, 257);
            this.txtPostData.Multiline = true;
            this.txtPostData.Name = "txtPostData";
            this.txtPostData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtPostData.Size = new System.Drawing.Size(520, 60);
            this.txtPostData.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 260);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "POST Data：";
            // 
            // txtHeader
            // 
            this.txtHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHeader.Location = new System.Drawing.Point(83, 88);
            this.txtHeader.Multiline = true;
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHeader.Size = new System.Drawing.Size(520, 162);
            this.txtHeader.TabIndex = 1;
            this.txtHeader.Text = resources.GetString("txtHeader.Text");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "Header：";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.White;
            this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage2.Controls.Add(this.txtResponseHeader);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(623, 325);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "响应信息";
            // 
            // txtResponseHeader
            // 
            this.txtResponseHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtResponseHeader.Location = new System.Drawing.Point(3, 3);
            this.txtResponseHeader.Name = "txtResponseHeader";
            this.txtResponseHeader.Size = new System.Drawing.Size(615, 317);
            this.txtResponseHeader.TabIndex = 0;
            this.txtResponseHeader.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage3.Controls.Add(this.txtWebSource);
            this.tabPage3.Location = new System.Drawing.Point(4, 25);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(623, 325);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "网页源码";
            // 
            // txtWebSource
            // 
            this.txtWebSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWebSource.Location = new System.Drawing.Point(0, 0);
            this.txtWebSource.Name = "txtWebSource";
            this.txtWebSource.Size = new System.Drawing.Size(621, 323);
            this.txtWebSource.TabIndex = 0;
            this.txtWebSource.Text = "";
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.Color.White;
            this.tabPage4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage4.Controls.Add(this.webBrowser1);
            this.tabPage4.Location = new System.Drawing.Point(4, 25);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(623, 325);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "网页浏览";
            // 
            // webBrowser1
            // 
            this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser1.Location = new System.Drawing.Point(0, 0);
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1";
            this.webBrowser1.ScriptErrorsSuppressed = true;
            this.webBrowser1.Size = new System.Drawing.Size(621, 323);
            this.webBrowser1.TabIndex = 0;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(651, 434);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.Text = "HTTP请求构造器";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TextBox txtHeader;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.WebBrowser webBrowser1;
        private System.Windows.Forms.RadioButton raPOST;
        private System.Windows.Forms.RadioButton raGet;
        private System.Windows.Forms.TextBox txtPostData;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ComboBox comwCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comPCode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtProxyAddress;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton raCustomProxy;
        private System.Windows.Forms.RadioButton raSystemProxy;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox isA;
        private System.Windows.Forms.RichTextBox txtWebSource;
        private System.Windows.Forms.RichTextBox txtResponseHeader;
    }
}