namespace MinerSpider
{
    partial class frmMonitor
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMonitor));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataTask = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolmenuStartRadar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolmenuStopRadar = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolmenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.toolmenuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolmenuDel = new System.Windows.Forms.ToolStripMenuItem();
            this.myLog = new SoukeyControl.CustomControl.cMyTextLog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rMenuDelUrlDB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataTask);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.myLog);
            this.splitContainer1.Size = new System.Drawing.Size(836, 395);
            this.splitContainer1.SplitterDistance = 278;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataTask
            // 
            this.dataTask.AllowUserToAddRows = false;
            this.dataTask.AllowUserToOrderColumns = true;
            this.dataTask.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Linen;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            this.dataTask.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataTask.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.dataTask.BackgroundColor = System.Drawing.Color.White;
            this.dataTask.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataTask.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataTask.ColumnHeadersHeight = 20;
            this.dataTask.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataTask.ContextMenuStrip = this.contextMenuStrip1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataTask.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataTask.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataTask.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataTask.GridColor = System.Drawing.SystemColors.AppWorkspace;
            this.dataTask.Location = new System.Drawing.Point(0, 0);
            this.dataTask.Margin = new System.Windows.Forms.Padding(0);
            this.dataTask.Name = "dataTask";
            this.dataTask.ReadOnly = true;
            this.dataTask.RowHeadersVisible = false;
            this.dataTask.RowTemplate.Height = 23;
            this.dataTask.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataTask.Size = new System.Drawing.Size(836, 278);
            this.dataTask.StandardTab = true;
            this.dataTask.TabIndex = 1;
            this.dataTask.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataTask_DataError);
            this.dataTask.DoubleClick += new System.EventHandler(this.dataTask_DoubleClick);
            this.dataTask.Enter += new System.EventHandler(this.dataTask_Enter);
            this.dataTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTask_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolmenuStartRadar,
            this.toolmenuStopRadar,
            this.toolStripSeparator1,
            this.toolmenuNew,
            this.toolmenuEdit,
            this.toolmenuDel,
            this.toolStripSeparator2,
            this.rMenuDelUrlDB});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(197, 170);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // toolmenuStartRadar
            // 
            this.toolmenuStartRadar.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuStartRadar.Image")));
            this.toolmenuStartRadar.Name = "toolmenuStartRadar";
            this.toolmenuStartRadar.Size = new System.Drawing.Size(196, 22);
            this.toolmenuStartRadar.Text = "启动雷达监控";
            this.toolmenuStartRadar.Click += new System.EventHandler(this.toolmenuStartRadar_Click);
            // 
            // toolmenuStopRadar
            // 
            this.toolmenuStopRadar.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuStopRadar.Image")));
            this.toolmenuStopRadar.Name = "toolmenuStopRadar";
            this.toolmenuStopRadar.Size = new System.Drawing.Size(196, 22);
            this.toolmenuStopRadar.Text = "停止雷达监控";
            this.toolmenuStopRadar.Click += new System.EventHandler(this.toolmenuStopRadar_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // toolmenuNew
            // 
            this.toolmenuNew.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuNew.Image")));
            this.toolmenuNew.Name = "toolmenuNew";
            this.toolmenuNew.Size = new System.Drawing.Size(196, 22);
            this.toolmenuNew.Text = "新建雷达监控规则";
            this.toolmenuNew.Click += new System.EventHandler(this.toolmenuNew_Click);
            // 
            // toolmenuEdit
            // 
            this.toolmenuEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuEdit.Image")));
            this.toolmenuEdit.Name = "toolmenuEdit";
            this.toolmenuEdit.Size = new System.Drawing.Size(196, 22);
            this.toolmenuEdit.Text = "编辑雷达监控规则";
            this.toolmenuEdit.Click += new System.EventHandler(this.toolmenuEdit_Click);
            // 
            // toolmenuDel
            // 
            this.toolmenuDel.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuDel.Image")));
            this.toolmenuDel.Name = "toolmenuDel";
            this.toolmenuDel.Size = new System.Drawing.Size(196, 22);
            this.toolmenuDel.Text = "删除雷达监控规则";
            this.toolmenuDel.Click += new System.EventHandler(this.toolmenuDel_Click);
            // 
            // myLog
            // 
            this.myLog.BackColor = System.Drawing.Color.White;
            this.myLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myLog.Location = new System.Drawing.Point(0, 0);
            this.myLog.Name = "myLog";
            this.myLog.ReadOnly = true;
            this.myLog.Size = new System.Drawing.Size(836, 113);
            this.myLog.TabIndex = 0;
            this.myLog.Text = "";
            this.myLog.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.myLog_LinkClicked);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "task");
            this.imageList1.Images.SetKeyName(1, "run");
            this.imageList1.Images.SetKeyName(2, "export");
            this.imageList1.Images.SetKeyName(3, "OK");
            this.imageList1.Images.SetKeyName(4, "tree");
            this.imageList1.Images.SetKeyName(5, "started");
            this.imageList1.Images.SetKeyName(6, "pause");
            this.imageList1.Images.SetKeyName(7, "stop");
            this.imageList1.Images.SetKeyName(8, "logo");
            this.imageList1.Images.SetKeyName(9, "error");
            this.imageList1.Images.SetKeyName(10, "taskplan");
            this.imageList1.Images.SetKeyName(11, "planrunning");
            this.imageList1.Images.SetKeyName(12, "PlanCompleted");
            this.imageList1.Images.SetKeyName(13, "disabledplan");
            this.imageList1.Images.SetKeyName(14, "log");
            this.imageList1.Images.SetKeyName(15, "folder");
            this.imageList1.Images.SetKeyName(16, "radar");
            this.imageList1.Images.SetKeyName(17, "radarrule");
            this.imageList1.Images.SetKeyName(18, "radarlog");
            // 
            // rMenuDelUrlDB
            // 
            this.rMenuDelUrlDB.Image = ((System.Drawing.Image)(resources.GetObject("rMenuDelUrlDB.Image")));
            this.rMenuDelUrlDB.Name = "rMenuDelUrlDB";
            this.rMenuDelUrlDB.Size = new System.Drawing.Size(196, 22);
            this.rMenuDelUrlDB.Text = "删除此规则的监控记录";
            this.rMenuDelUrlDB.Click += new System.EventHandler(this.rMenuDelUrlDB_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(193, 6);
            // 
            // frmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 395);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMonitor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.WindowsDefaultBounds;
            this.Text = "监控雷达";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMonitor_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMonitor_FormClosed);
            this.Load += new System.EventHandler(this.frmMonitor_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private SoukeyControl.CustomControl.cMyTextLog myLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolmenuNew;
        private System.Windows.Forms.ToolStripMenuItem toolmenuStartRadar;
        private System.Windows.Forms.ToolStripMenuItem toolmenuStopRadar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolmenuEdit;
        private System.Windows.Forms.ToolStripMenuItem toolmenuDel;
        private System.Windows.Forms.ImageList imageList1;
        internal System.Windows.Forms.DataGridView dataTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem rMenuDelUrlDB;
    }
}