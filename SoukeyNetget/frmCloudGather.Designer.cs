namespace MinerSpider
{
    partial class frmCloudGather
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCloudGather));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labUser = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labIntegral = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labAllowCount = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rMenuAddTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuStart = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuGetData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuDelTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolAddTask = new System.Windows.Forms.LinkLabel();
            this.toolStart = new System.Windows.Forms.LinkLabel();
            this.toolStop = new System.Windows.Forms.LinkLabel();
            this.toolGetData = new System.Windows.Forms.LinkLabel();
            this.toolDelTask = new System.Windows.Forms.LinkLabel();
            this.CloudData = new SoukeyControl.CustomControl.cMyDataGridView(this.components);
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CloudData)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Image = ((System.Drawing.Image)(resources.GetObject("label1.Image")));
            this.label1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(270, 28);
            this.label1.TabIndex = 1;
            this.label1.Text = "欢迎使用网络矿工云采集服务";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Location = new System.Drawing.Point(4, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1067, 5);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(288, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "我的账号：";
            // 
            // labUser
            // 
            this.labUser.AutoSize = true;
            this.labUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labUser.Location = new System.Drawing.Point(347, 19);
            this.labUser.Name = "labUser";
            this.labUser.Size = new System.Drawing.Size(65, 12);
            this.labUser.TabIndex = 65;
            this.labUser.Text = "labVersion";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(418, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 66;
            this.label3.Text = "积分：";
            // 
            // labIntegral
            // 
            this.labIntegral.AutoSize = true;
            this.labIntegral.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labIntegral.Location = new System.Drawing.Point(460, 19);
            this.labIntegral.Name = "labIntegral";
            this.labIntegral.Size = new System.Drawing.Size(65, 12);
            this.labIntegral.TabIndex = 67;
            this.labIntegral.Text = "labVersion";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(531, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(137, 12);
            this.label4.TabIndex = 68;
            this.label4.Text = "还可通过云服务器采集：";
            // 
            // labAllowCount
            // 
            this.labAllowCount.AutoSize = true;
            this.labAllowCount.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labAllowCount.Location = new System.Drawing.Point(662, 19);
            this.labAllowCount.Name = "labAllowCount";
            this.labAllowCount.Size = new System.Drawing.Size(65, 12);
            this.labAllowCount.TabIndex = 69;
            this.labAllowCount.Text = "labVersion";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label10.Location = new System.Drawing.Point(733, 19);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(29, 12);
            this.Label10.TabIndex = 70;
            this.Label10.Text = "记录";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rMenuAddTask,
            this.toolStripSeparator1,
            this.rMenuStart,
            this.rMenuStop,
            this.toolStripSeparator2,
            this.rMenuGetData,
            this.toolStripSeparator3,
            this.rMenuDelTask});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(161, 132);
            // 
            // rMenuAddTask
            // 
            this.rMenuAddTask.Image = ((System.Drawing.Image)(resources.GetObject("rMenuAddTask.Image")));
            this.rMenuAddTask.Name = "rMenuAddTask";
            this.rMenuAddTask.Size = new System.Drawing.Size(160, 22);
            this.rMenuAddTask.Text = "增加云采集任务";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // rMenuStart
            // 
            this.rMenuStart.Image = ((System.Drawing.Image)(resources.GetObject("rMenuStart.Image")));
            this.rMenuStart.Name = "rMenuStart";
            this.rMenuStart.Size = new System.Drawing.Size(160, 22);
            this.rMenuStart.Text = "启动任务";
            // 
            // rMenuStop
            // 
            this.rMenuStop.Image = ((System.Drawing.Image)(resources.GetObject("rMenuStop.Image")));
            this.rMenuStop.Name = "rMenuStop";
            this.rMenuStop.Size = new System.Drawing.Size(160, 22);
            this.rMenuStop.Text = "停止任务";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(157, 6);
            // 
            // rMenuGetData
            // 
            this.rMenuGetData.Image = ((System.Drawing.Image)(resources.GetObject("rMenuGetData.Image")));
            this.rMenuGetData.Name = "rMenuGetData";
            this.rMenuGetData.Size = new System.Drawing.Size(160, 22);
            this.rMenuGetData.Text = "获取数据";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(157, 6);
            // 
            // rMenuDelTask
            // 
            this.rMenuDelTask.Image = ((System.Drawing.Image)(resources.GetObject("rMenuDelTask.Image")));
            this.rMenuDelTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuDelTask.Name = "rMenuDelTask";
            this.rMenuDelTask.Size = new System.Drawing.Size(160, 22);
            this.rMenuDelTask.Text = "删除任务";
            // 
            // toolAddTask
            // 
            this.toolAddTask.Image = ((System.Drawing.Image)(resources.GetObject("toolAddTask.Image")));
            this.toolAddTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolAddTask.Location = new System.Drawing.Point(14, 51);
            this.toolAddTask.Name = "toolAddTask";
            this.toolAddTask.Size = new System.Drawing.Size(96, 21);
            this.toolAddTask.TabIndex = 73;
            this.toolAddTask.TabStop = true;
            this.toolAddTask.Text = "增加采集任务";
            this.toolAddTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolAddTask.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // toolStart
            // 
            this.toolStart.Enabled = false;
            this.toolStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStart.Image")));
            this.toolStart.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStart.Location = new System.Drawing.Point(202, 51);
            this.toolStart.Name = "toolStart";
            this.toolStart.Size = new System.Drawing.Size(73, 21);
            this.toolStart.TabIndex = 74;
            this.toolStart.TabStop = true;
            this.toolStart.Text = "启动采集";
            this.toolStart.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolStop
            // 
            this.toolStop.Enabled = false;
            this.toolStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStop.Image")));
            this.toolStop.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStop.Location = new System.Drawing.Point(281, 51);
            this.toolStop.Name = "toolStop";
            this.toolStop.Size = new System.Drawing.Size(73, 21);
            this.toolStop.TabIndex = 75;
            this.toolStop.TabStop = true;
            this.toolStop.Text = "停止采集";
            this.toolStop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolGetData
            // 
            this.toolGetData.Enabled = false;
            this.toolGetData.Image = ((System.Drawing.Image)(resources.GetObject("toolGetData.Image")));
            this.toolGetData.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolGetData.Location = new System.Drawing.Point(367, 51);
            this.toolGetData.Name = "toolGetData";
            this.toolGetData.Size = new System.Drawing.Size(73, 21);
            this.toolGetData.TabIndex = 76;
            this.toolGetData.TabStop = true;
            this.toolGetData.Text = "获取数据";
            this.toolGetData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolDelTask
            // 
            this.toolDelTask.Enabled = false;
            this.toolDelTask.Image = ((System.Drawing.Image)(resources.GetObject("toolDelTask.Image")));
            this.toolDelTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolDelTask.Location = new System.Drawing.Point(116, 51);
            this.toolDelTask.Name = "toolDelTask";
            this.toolDelTask.Size = new System.Drawing.Size(73, 21);
            this.toolDelTask.TabIndex = 77;
            this.toolDelTask.TabStop = true;
            this.toolDelTask.Text = "删除采集";
            this.toolDelTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // CloudData
            // 
            this.CloudData.AllowUserToAddRows = false;
            this.CloudData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CloudData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CloudData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6,
            this.Column7,
            this.Column8});
            this.CloudData.ContextMenuStrip = this.contextMenuStrip1;
            this.CloudData.gData = null;
            this.CloudData.Location = new System.Drawing.Point(4, 75);
            this.CloudData.Name = "CloudData";
            this.CloudData.ReadOnly = true;
            this.CloudData.RowHeadersWidth = 21;
            this.CloudData.RowTemplate.Height = 23;
            this.CloudData.Size = new System.Drawing.Size(772, 301);
            this.CloudData.TabIndex = 71;
            this.CloudData.TaskName = null;
            this.CloudData.TaskRunID = ((long)(0));
            // 
            // Column1
            // 
            this.Column1.FillWeight = 21F;
            this.Column1.HeaderText = "";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 21;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "状态";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "任务名称";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 200;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "启动运行时间";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 120;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "完成时间";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 120;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "采集记录数量";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Width = 120;
            // 
            // Column7
            // 
            this.Column7.HeaderText = "采集网址数量";
            this.Column7.Name = "Column7";
            this.Column7.ReadOnly = true;
            this.Column7.Width = 120;
            // 
            // Column8
            // 
            this.Column8.HeaderText = "错误数量";
            this.Column8.Name = "Column8";
            this.Column8.ReadOnly = true;
            this.Column8.Width = 80;
            // 
            // frmCloudGather
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(778, 378);
            this.Controls.Add(this.toolDelTask);
            this.Controls.Add(this.toolGetData);
            this.Controls.Add(this.toolStop);
            this.Controls.Add(this.toolStart);
            this.Controls.Add(this.toolAddTask);
            this.Controls.Add(this.CloudData);
            this.Controls.Add(this.Label10);
            this.Controls.Add(this.labAllowCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labIntegral);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCloudGather";
            this.Text = "云采集";
            this.Load += new System.EventHandler(this.frmCloudGather_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CloudData)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labUser;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labIntegral;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labAllowCount;
        private System.Windows.Forms.Label Label10;
        private SoukeyControl.CustomControl.cMyDataGridView CloudData;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rMenuAddTask;
        private System.Windows.Forms.ToolStripMenuItem rMenuStart;
        private System.Windows.Forms.ToolStripMenuItem rMenuStop;
        private System.Windows.Forms.ToolStripMenuItem rMenuGetData;
        private System.Windows.Forms.ToolStripMenuItem rMenuDelTask;
        private System.Windows.Forms.LinkLabel toolAddTask;
        private System.Windows.Forms.LinkLabel toolStart;
        private System.Windows.Forms.LinkLabel toolStop;
        private System.Windows.Forms.LinkLabel toolGetData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.LinkLabel toolDelTask;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
    }
}