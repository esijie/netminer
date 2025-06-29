namespace SoukeyNetget
{
    partial class frmAddUrlWizard
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddUrlWizard));
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.txtWebLink = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.cmdNext = new System.Windows.Forms.Button();
            this.cmdPre = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.button15 = new System.Windows.Forms.Button();
            this.numMaxNextPage = new System.Windows.Forms.NumericUpDown();
            this.label33 = new System.Windows.Forms.Label();
            this.IsDoPostBack = new System.Windows.Forms.CheckBox();
            this.txtNextPage = new System.Windows.Forms.TextBox();
            this.IsAutoNextPage = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxNextPage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 19);
            this.label1.TabIndex = 24;
            this.label1.Text = "欢迎使用网络矿工采集网址配置向导";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.radioButton4);
            this.panel1.Controls.Add(this.radioButton3);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtWebLink);
            this.panel1.Controls.Add(this.radioButton2);
            this.panel1.Controls.Add(this.radioButton1);
            this.panel1.Location = new System.Drawing.Point(9, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(630, 292);
            this.panel1.TabIndex = 25;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(21, 17);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(275, 16);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "如果采集的数据就在当前的网址中，请选择此项";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(21, 40);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(515, 16);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.Text = "采集的数据不在数据的网址中，但通过输入的网址可以点击打开需要采集的页面，请选择此项";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // txtWebLink
            // 
            this.txtWebLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWebLink.HideSelection = false;
            this.txtWebLink.Location = new System.Drawing.Point(21, 197);
            this.txtWebLink.Multiline = true;
            this.txtWebLink.Name = "txtWebLink";
            this.txtWebLink.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWebLink.Size = new System.Drawing.Size(573, 78);
            this.txtWebLink.TabIndex = 4;
            this.txtWebLink.Text = "http://";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(563, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "请求采集分为GET/POST两种方式，GET请求可以看到网址的变化，POST请求看不到网址的变化，您的请求属";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "于哪一种呢？";
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(127, 82);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(41, 16);
            this.radioButton3.TabIndex = 7;
            this.radioButton3.Text = "GET";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(176, 82);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(47, 16);
            this.radioButton4.TabIndex = 8;
            this.radioButton4.Text = "POST";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(37, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(557, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "当然还有一种情况就是AJAX，如果是这种情况，通常采集的数据在网页源码中是查找不到的，实际是通过";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(185, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "其他的地址进行数据的请求，您可";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(220, 129);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(41, 12);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "点击我";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(262, 129);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(197, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "打开嗅探器，来查找真实的请求地址";
            // 
            // cmdNext
            // 
            this.cmdNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNext.Image = ((System.Drawing.Image)(resources.GetObject("cmdNext.Image")));
            this.cmdNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdNext.Location = new System.Drawing.Point(526, 359);
            this.cmdNext.Name = "cmdNext";
            this.cmdNext.Size = new System.Drawing.Size(65, 22);
            this.cmdNext.TabIndex = 27;
            this.cmdNext.Text = "下一步";
            this.cmdNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdNext.UseVisualStyleBackColor = true;
            // 
            // cmdPre
            // 
            this.cmdPre.Enabled = false;
            this.cmdPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPre.Image = ((System.Drawing.Image)(resources.GetObject("cmdPre.Image")));
            this.cmdPre.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdPre.Location = new System.Drawing.Point(449, 359);
            this.cmdPre.Name = "cmdPre";
            this.cmdPre.Size = new System.Drawing.Size(65, 22);
            this.cmdPre.TabIndex = 26;
            this.cmdPre.Text = "上一步";
            this.cmdPre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdPre.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(-2, 339);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(648, 5);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 150);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(557, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "如果您输入的网址是多个，且这些网址都有一定的规律，譬如：数字变化、时间变化，您可以尝试用网址";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(37, 168);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(173, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "参数来替换，简化网址输入操作";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label39);
            this.panel2.Controls.Add(this.button15);
            this.panel2.Controls.Add(this.numMaxNextPage);
            this.panel2.Controls.Add(this.label33);
            this.panel2.Controls.Add(this.IsDoPostBack);
            this.panel2.Controls.Add(this.txtNextPage);
            this.panel2.Controls.Add(this.IsAutoNextPage);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Location = new System.Drawing.Point(9, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(630, 292);
            this.panel2.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(485, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "您输入的网址需要进行翻页操作么？如果需要翻页则在此设置，如果不需要则跳过此环节：";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label39.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label39.Location = new System.Drawing.Point(229, 201);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(269, 12);
            this.label39.TabIndex = 85;
            this.label39.Text = "0 代表不限制最大页码，由系统根据翻页规则决定";
            // 
            // button15
            // 
            this.button15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button15.Enabled = false;
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button15.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button15.ForeColor = System.Drawing.Color.Navy;
            this.button15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button15.Location = new System.Drawing.Point(516, 107);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(103, 57);
            this.button15.TabIndex = 86;
            this.button15.Text = "配置翻页规则";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // numMaxNextPage
            // 
            this.numMaxNextPage.Enabled = false;
            this.numMaxNextPage.Location = new System.Drawing.Point(103, 197);
            this.numMaxNextPage.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numMaxNextPage.Name = "numMaxNextPage";
            this.numMaxNextPage.Size = new System.Drawing.Size(120, 21);
            this.numMaxNextPage.TabIndex = 84;
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label33.Location = new System.Drawing.Point(11, 201);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(89, 12);
            this.label33.TabIndex = 83;
            this.label33.Text = "翻页最大页码：";
            // 
            // IsDoPostBack
            // 
            this.IsDoPostBack.AutoSize = true;
            this.IsDoPostBack.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsDoPostBack.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsDoPostBack.Location = new System.Drawing.Point(16, 80);
            this.IsDoPostBack.Name = "IsDoPostBack";
            this.IsDoPostBack.Size = new System.Drawing.Size(250, 16);
            this.IsDoPostBack.TabIndex = 87;
            this.IsDoPostBack.Text = "翻页规则是__doPostBack函数，需系统解析\r\n";
            this.IsDoPostBack.UseVisualStyleBackColor = true;
            // 
            // txtNextPage
            // 
            this.txtNextPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNextPage.BackColor = System.Drawing.SystemColors.Control;
            this.txtNextPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNextPage.Enabled = false;
            this.txtNextPage.Location = new System.Drawing.Point(16, 107);
            this.txtNextPage.Multiline = true;
            this.txtNextPage.Name = "txtNextPage";
            this.txtNextPage.Size = new System.Drawing.Size(501, 57);
            this.txtNextPage.TabIndex = 82;
            // 
            // IsAutoNextPage
            // 
            this.IsAutoNextPage.AutoSize = true;
            this.IsAutoNextPage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsAutoNextPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsAutoNextPage.Location = new System.Drawing.Point(16, 38);
            this.IsAutoNextPage.Name = "IsAutoNextPage";
            this.IsAutoNextPage.Size = new System.Drawing.Size(298, 16);
            this.IsAutoNextPage.TabIndex = 81;
            this.IsAutoNextPage.Text = "需根据下一页规则自动翻页连续采集，下一页规则：";
            this.IsAutoNextPage.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(30, 61);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(437, 12);
            this.label10.TabIndex = 88;
            this.label10.Text = "当把鼠标放置在下一页链接上，查看链接地址包含doPostBack的字样，则选择此项";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Location = new System.Drawing.Point(9, 41);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(630, 292);
            this.panel3.TabIndex = 30;
            // 
            // frmAddUrlWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 394);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdNext);
            this.Controls.Add(this.cmdPre);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddUrlWizard";
            this.Text = "增加您需要采集的网址信息";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxNextPage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWebLink;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Button cmdPre;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.NumericUpDown numMaxNextPage;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.CheckBox IsDoPostBack;
        private System.Windows.Forms.TextBox txtNextPage;
        private System.Windows.Forms.CheckBox IsAutoNextPage;
        private System.Windows.Forms.Panel panel3;
    }
}