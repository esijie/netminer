namespace MinerSpider
{
    partial class frmAddPlugins
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddPlugins));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPluginName = new System.Windows.Forms.TextBox();
            this.txtPlugin = new System.Windows.Forms.TextBox();
            this.comType = new System.Windows.Forms.ComboBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdBrowser = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "插件名称：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "插件：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "插件类型：";
            // 
            // txtPluginName
            // 
            this.txtPluginName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPluginName.Location = new System.Drawing.Point(90, 21);
            this.txtPluginName.Name = "txtPluginName";
            this.txtPluginName.Size = new System.Drawing.Size(294, 21);
            this.txtPluginName.TabIndex = 3;
            // 
            // txtPlugin
            // 
            this.txtPlugin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPlugin.Location = new System.Drawing.Point(90, 50);
            this.txtPlugin.Name = "txtPlugin";
            this.txtPlugin.Size = new System.Drawing.Size(222, 21);
            this.txtPlugin.TabIndex = 4;
            // 
            // comType
            // 
            this.comType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comType.FormattingEnabled = true;
            this.comType.Location = new System.Drawing.Point(90, 79);
            this.comType.Name = "comType";
            this.comType.Size = new System.Drawing.Size(293, 20);
            this.comType.TabIndex = 5;
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(318, 109);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 23);
            this.cmdCancel.TabIndex = 7;
            this.cmdCancel.Text = "取  消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            this.cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdOK.Image = ((System.Drawing.Image)(resources.GetObject("cmdOK.Image")));
            this.cmdOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(247, 109);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 23);
            this.cmdOK.TabIndex = 6;
            this.cmdOK.Text = "确  定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdBrowser
            // 
            this.cmdBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdBrowser.Image = ((System.Drawing.Image)(resources.GetObject("cmdBrowser.Image")));
            this.cmdBrowser.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdBrowser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdBrowser.Location = new System.Drawing.Point(311, 50);
            this.cmdBrowser.Name = "cmdBrowser";
            this.cmdBrowser.Size = new System.Drawing.Size(73, 21);
            this.cmdBrowser.TabIndex = 8;
            this.cmdBrowser.Text = "浏览...";
            this.cmdBrowser.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdBrowser.UseVisualStyleBackColor = true;
            this.cmdBrowser.Click += new System.EventHandler(this.cmdBrowser_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmAddPlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 141);
            this.Controls.Add(this.cmdBrowser);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.comType);
            this.Controls.Add(this.txtPlugin);
            this.Controls.Add(this.txtPluginName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddPlugins";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插件信息";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAddPlugins_FormClosing);
            this.Load += new System.EventHandler(this.frmAddPlugins_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPluginName;
        private System.Windows.Forms.TextBox txtPlugin;
        private System.Windows.Forms.ComboBox comType;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Button cmdBrowser;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}