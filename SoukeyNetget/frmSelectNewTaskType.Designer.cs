namespace MinerSpider
{
    partial class frmSelectNewTaskType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSelectNewTaskType));
            this.label1 = new System.Windows.Forms.Label();
            this.raNormal = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.raWizard = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.IsShow = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "如果您是老用户或已经熟悉采集任务的配置可选择：";
            // 
            // raNormal
            // 
            this.raNormal.AutoSize = true;
            this.raNormal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raNormal.Location = new System.Drawing.Point(156, 69);
            this.raNormal.Name = "raNormal";
            this.raNormal.Size = new System.Drawing.Size(172, 16);
            this.raNormal.TabIndex = 1;
            this.raNormal.TabStop = true;
            this.raNormal.Text = "便捷方式建立/编辑采集任务";
            this.raNormal.UseVisualStyleBackColor = true;
            this.raNormal.CheckedChanged += new System.EventHandler(this.raNormal_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "如果您是新用户可选择：";
            // 
            // raWizard
            // 
            this.raWizard.AutoSize = true;
            this.raWizard.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.raWizard.Location = new System.Drawing.Point(156, 25);
            this.raWizard.Name = "raWizard";
            this.raWizard.Size = new System.Drawing.Size(172, 16);
            this.raWizard.TabIndex = 3;
            this.raWizard.TabStop = true;
            this.raWizard.Text = "向导方式建立/编辑采集任务";
            this.raWizard.UseVisualStyleBackColor = true;
            this.raWizard.CheckedChanged += new System.EventHandler(this.raWizard_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(-1, 90);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 5);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // IsShow
            // 
            this.IsShow.AutoSize = true;
            this.IsShow.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsShow.Location = new System.Drawing.Point(12, 104);
            this.IsShow.Name = "IsShow";
            this.IsShow.Size = new System.Drawing.Size(142, 16);
            this.IsShow.TabIndex = 5;
            this.IsShow.Text = "记住选项以后不再显示";
            this.IsShow.UseVisualStyleBackColor = true;
            this.IsShow.CheckedChanged += new System.EventHandler(this.IsShow_CheckedChanged);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(254, 102);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 21);
            this.button2.TabIndex = 7;
            this.button2.Text = "取 消";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(174, 102);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 21);
            this.button1.TabIndex = 6;
            this.button1.Text = "确 定";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmSelectNewTaskType
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(339, 131);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.IsShow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.raWizard);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.raNormal);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSelectNewTaskType";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请选择建立采集任务的方式";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmSelectNewTaskType_FormClosed);
            this.Load += new System.EventHandler(this.frmSelectNewTaskType_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton raNormal;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton raWizard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox IsShow;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}