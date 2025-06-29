namespace MinerSpider
{
    partial class frmAddProxy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddProxy));
            this.txtProxyPwd = new System.Windows.Forms.TextBox();
            this.txtProxyUser = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtProxyPort = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProxyServer = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.butTestProxy = new System.Windows.Forms.Button();
            this.txtProxyTestUrl = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtProxyPwd
            // 
            this.txtProxyPwd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxyPwd.Location = new System.Drawing.Point(93, 90);
            this.txtProxyPwd.Name = "txtProxyPwd";
            this.txtProxyPwd.Size = new System.Drawing.Size(223, 21);
            this.txtProxyPwd.TabIndex = 16;
            // 
            // txtProxyUser
            // 
            this.txtProxyUser.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxyUser.Location = new System.Drawing.Point(93, 63);
            this.txtProxyUser.Name = "txtProxyUser";
            this.txtProxyUser.Size = new System.Drawing.Size(223, 21);
            this.txtProxyUser.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(44, 91);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 12);
            this.label8.TabIndex = 14;
            this.label8.Text = "密码：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(32, 66);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "用户名：";
            // 
            // txtProxyPort
            // 
            this.txtProxyPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxyPort.Location = new System.Drawing.Point(93, 37);
            this.txtProxyPort.Name = "txtProxyPort";
            this.txtProxyPort.Size = new System.Drawing.Size(223, 21);
            this.txtProxyPort.TabIndex = 12;
            this.txtProxyPort.Text = "80";
            this.txtProxyPort.Leave += new System.EventHandler(this.txtProxyPort_Leave);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(44, 41);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "端口：";
            // 
            // txtProxyServer
            // 
            this.txtProxyServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxyServer.Location = new System.Drawing.Point(93, 11);
            this.txtProxyServer.Name = "txtProxyServer";
            this.txtProxyServer.Size = new System.Drawing.Size(223, 21);
            this.txtProxyServer.TabIndex = 10;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(8, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "代理服务器：";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(4, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 5);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(251, 147);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 19;
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
            this.cmdOK.Location = new System.Drawing.Point(173, 147);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 22);
            this.cmdOK.TabIndex = 18;
            this.cmdOK.Text = "确 定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // butTestProxy
            // 
            this.butTestProxy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.butTestProxy.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.butTestProxy.Location = new System.Drawing.Point(22, 148);
            this.butTestProxy.Name = "butTestProxy";
            this.butTestProxy.Size = new System.Drawing.Size(65, 21);
            this.butTestProxy.TabIndex = 22;
            this.butTestProxy.Text = "测  试";
            this.butTestProxy.UseVisualStyleBackColor = true;
            this.butTestProxy.Click += new System.EventHandler(this.butTestProxy_Click);
            // 
            // txtProxyTestUrl
            // 
            this.txtProxyTestUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxyTestUrl.Location = new System.Drawing.Point(93, 116);
            this.txtProxyTestUrl.Name = "txtProxyTestUrl";
            this.txtProxyTestUrl.Size = new System.Drawing.Size(223, 21);
            this.txtProxyTestUrl.TabIndex = 21;
            this.txtProxyTestUrl.Text = "http://www.netminer.cn";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(20, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 20;
            this.label9.Text = "测试地址：";
            // 
            // frmAddProxy
            // 
            this.AcceptButton = this.cmdOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(334, 177);
            this.Controls.Add(this.butTestProxy);
            this.Controls.Add(this.txtProxyTestUrl);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtProxyPwd);
            this.Controls.Add(this.txtProxyUser);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtProxyPort);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtProxyServer);
            this.Controls.Add(this.label5);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddProxy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "增加代理数据";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAddProxy_FormClosed);
            this.Load += new System.EventHandler(this.frmAddProxy_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtProxyPwd;
        private System.Windows.Forms.TextBox txtProxyUser;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProxyServer;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button butTestProxy;
        private System.Windows.Forms.TextBox txtProxyTestUrl;
        private System.Windows.Forms.Label label9;

    }
}