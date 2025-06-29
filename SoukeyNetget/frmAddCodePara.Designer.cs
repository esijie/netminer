namespace MinerSpider
{
    partial class frmAddCodePara
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddCodePara));
            this.isAutoDM = new System.Windows.Forms.CheckBox();
            this.cmdSetPlugins1 = new System.Windows.Forms.Button();
            this.cmdBrowserPlugins1 = new System.Windows.Forms.Button();
            this.txtDMPlugin = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtDMUrl = new System.Windows.Forms.TextBox();
            this.label29 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // isAutoDM
            // 
            this.isAutoDM.AutoSize = true;
            this.isAutoDM.Location = new System.Drawing.Point(12, 62);
            this.isAutoDM.Name = "isAutoDM";
            this.isAutoDM.Size = new System.Drawing.Size(72, 16);
            this.isAutoDM.TabIndex = 57;
            this.isAutoDM.Text = "自动打码";
            this.isAutoDM.UseVisualStyleBackColor = true;
            this.isAutoDM.CheckedChanged += new System.EventHandler(this.isAutoDM_CheckedChanged);
            // 
            // cmdSetPlugins1
            // 
            this.cmdSetPlugins1.Enabled = false;
            this.cmdSetPlugins1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdSetPlugins1.Image = ((System.Drawing.Image)(resources.GetObject("cmdSetPlugins1.Image")));
            this.cmdSetPlugins1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdSetPlugins1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdSetPlugins1.Location = new System.Drawing.Point(369, 98);
            this.cmdSetPlugins1.Name = "cmdSetPlugins1";
            this.cmdSetPlugins1.Size = new System.Drawing.Size(60, 21);
            this.cmdSetPlugins1.TabIndex = 56;
            this.cmdSetPlugins1.Text = "设置";
            this.cmdSetPlugins1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdSetPlugins1.UseVisualStyleBackColor = true;
            this.cmdSetPlugins1.Click += new System.EventHandler(this.cmdSetPlugins1_Click);
            // 
            // cmdBrowserPlugins1
            // 
            this.cmdBrowserPlugins1.Enabled = false;
            this.cmdBrowserPlugins1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdBrowserPlugins1.Image = ((System.Drawing.Image)(resources.GetObject("cmdBrowserPlugins1.Image")));
            this.cmdBrowserPlugins1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdBrowserPlugins1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdBrowserPlugins1.Location = new System.Drawing.Point(297, 98);
            this.cmdBrowserPlugins1.Name = "cmdBrowserPlugins1";
            this.cmdBrowserPlugins1.Size = new System.Drawing.Size(73, 21);
            this.cmdBrowserPlugins1.TabIndex = 55;
            this.cmdBrowserPlugins1.Text = "浏览...";
            this.cmdBrowserPlugins1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdBrowserPlugins1.UseVisualStyleBackColor = true;
            this.cmdBrowserPlugins1.Click += new System.EventHandler(this.cmdBrowserPlugins1_Click);
            // 
            // txtDMPlugin
            // 
            this.txtDMPlugin.Enabled = false;
            this.txtDMPlugin.Location = new System.Drawing.Point(12, 98);
            this.txtDMPlugin.Name = "txtDMPlugin";
            this.txtDMPlugin.Size = new System.Drawing.Size(288, 21);
            this.txtDMPlugin.TabIndex = 53;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(13, 82);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(65, 12);
            this.label40.TabIndex = 51;
            this.label40.Text = "插件地址：";
            // 
            // txtDMUrl
            // 
            this.txtDMUrl.Location = new System.Drawing.Point(12, 26);
            this.txtDMUrl.Multiline = true;
            this.txtDMUrl.Name = "txtDMUrl";
            this.txtDMUrl.Size = new System.Drawing.Size(417, 33);
            this.txtDMUrl.TabIndex = 50;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(10, 10);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(65, 12);
            this.label29.TabIndex = 49;
            this.label29.Text = "打码地址：";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(-9, 128);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(481, 5);
            this.groupBox1.TabIndex = 60;
            this.groupBox1.TabStop = false;
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(364, 139);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(65, 21);
            this.button2.TabIndex = 59;
            this.button2.Text = "取 消";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(293, 139);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 21);
            this.button1.TabIndex = 58;
            this.button1.Text = "确 定";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmAddCodePara
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(439, 172);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.isAutoDM);
            this.Controls.Add(this.cmdSetPlugins1);
            this.Controls.Add(this.cmdBrowserPlugins1);
            this.Controls.Add(this.txtDMPlugin);
            this.Controls.Add(this.label40);
            this.Controls.Add(this.txtDMUrl);
            this.Controls.Add(this.label29);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddCodePara";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置打码参数";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox isAutoDM;
        private System.Windows.Forms.Button cmdSetPlugins1;
        private System.Windows.Forms.Button cmdBrowserPlugins1;
        private System.Windows.Forms.TextBox txtDMPlugin;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox txtDMUrl;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}