namespace MinerSpider
{
    partial class frmRunningTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRunningTask));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dataTask = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmmenuStartTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmmenuStopTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuOverTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuBrowserData = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuEditData = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuClearCompleted = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuRefer = new System.Windows.Forms.ToolStripMenuItem();
            this.rmmenuDelTask = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1 = new SoukeyControl.CustomControl.cMyTabControl(this.components);
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rMenuExportTxt = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuExportWord = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuExportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuPublishData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuStartTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuStopTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuCloseTabPage = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip5 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuSaveLog = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip4.SuspendLayout();
            this.contextMenuStrip5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dataTask);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer2.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer2.Size = new System.Drawing.Size(752, 350);
            this.splitContainer2.SplitterDistance = 168;
            this.splitContainer2.TabIndex = 1;
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
            this.dataTask.ContextMenuStrip = this.contextMenuStrip2;
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
            this.dataTask.Size = new System.Drawing.Size(752, 168);
            this.dataTask.StandardTab = true;
            this.dataTask.TabIndex = 0;
            this.dataTask.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataTask_CellBeginEdit);
            this.dataTask.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTask_CellClick);
            this.dataTask.Click += new System.EventHandler(this.dataTask_Click);
            this.dataTask.DoubleClick += new System.EventHandler(this.dataTask_DoubleClick);
            this.dataTask.Enter += new System.EventHandler(this.dataTask_Enter);
            this.dataTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTask_KeyDown);
            this.dataTask.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataTask_MouseMove);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmmenuStartTask,
            this.rmmenuStopTask,
            this.rmenuOverTask,
            this.toolStripSeparator5,
            this.rmenuBrowserData,
            this.rmenuEditData,
            this.rmenuClearCompleted,
            this.toolStripSeparator15,
            this.rMenuRefer,
            this.rmmenuDelTask});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(245, 192);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // rmmenuStartTask
            // 
            this.rmmenuStartTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuStartTask.Image")));
            this.rmmenuStartTask.Name = "rmmenuStartTask";
            this.rmmenuStartTask.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.rmmenuStartTask.Size = new System.Drawing.Size(244, 22);
            this.rmmenuStartTask.Text = "启动任务";
            this.rmmenuStartTask.Click += new System.EventHandler(this.rmmenuStartTask_Click);
            // 
            // rmmenuStopTask
            // 
            this.rmmenuStopTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuStopTask.Image")));
            this.rmmenuStopTask.Name = "rmmenuStopTask";
            this.rmmenuStopTask.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.rmmenuStopTask.Size = new System.Drawing.Size(244, 22);
            this.rmmenuStopTask.Text = "停止任务";
            this.rmmenuStopTask.Click += new System.EventHandler(this.rmmenuStopTask_Click);
            // 
            // rmenuOverTask
            // 
            this.rmenuOverTask.Image = ((System.Drawing.Image)(resources.GetObject("rmenuOverTask.Image")));
            this.rmenuOverTask.Name = "rmenuOverTask";
            this.rmenuOverTask.Size = new System.Drawing.Size(244, 22);
            this.rmenuOverTask.Text = "强制结束此任务";
            this.rmenuOverTask.Click += new System.EventHandler(this.rmenuOverTask_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(241, 6);
            // 
            // rmenuBrowserData
            // 
            this.rmenuBrowserData.Name = "rmenuBrowserData";
            this.rmenuBrowserData.Size = new System.Drawing.Size(244, 22);
            this.rmenuBrowserData.Text = "查看数据";
            this.rmenuBrowserData.Click += new System.EventHandler(this.rmenuBrowserData_Click);
            // 
            // rmenuEditData
            // 
            this.rmenuEditData.Image = ((System.Drawing.Image)(resources.GetObject("rmenuEditData.Image")));
            this.rmenuEditData.Name = "rmenuEditData";
            this.rmenuEditData.Size = new System.Drawing.Size(244, 22);
            this.rmenuEditData.Text = "编辑/发布数据";
            this.rmenuEditData.Click += new System.EventHandler(this.rmenuEditData_Click);
            // 
            // rmenuClearCompleted
            // 
            this.rmenuClearCompleted.Image = ((System.Drawing.Image)(resources.GetObject("rmenuClearCompleted.Image")));
            this.rmenuClearCompleted.Name = "rmenuClearCompleted";
            this.rmenuClearCompleted.Size = new System.Drawing.Size(244, 22);
            this.rmenuClearCompleted.Text = "自动清理无效的已采集任务数据";
            this.rmenuClearCompleted.Visible = false;
            this.rmenuClearCompleted.Click += new System.EventHandler(this.rmenuClearCompleted_Click);
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(241, 6);
            // 
            // rMenuRefer
            // 
            this.rMenuRefer.Image = ((System.Drawing.Image)(resources.GetObject("rMenuRefer.Image")));
            this.rMenuRefer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuRefer.Name = "rMenuRefer";
            this.rMenuRefer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.rMenuRefer.Size = new System.Drawing.Size(244, 22);
            this.rMenuRefer.Text = "刷新";
            this.rMenuRefer.Click += new System.EventHandler(this.rMenuRefer_Click);
            // 
            // rmmenuDelTask
            // 
            this.rmmenuDelTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuDelTask.Image")));
            this.rmmenuDelTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmmenuDelTask.Name = "rmmenuDelTask";
            this.rmmenuDelTask.Size = new System.Drawing.Size(244, 22);
            this.rmmenuDelTask.Text = "删除        Delete";
            this.rmmenuDelTask.Click += new System.EventHandler(this.rmmenuDelTask_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.ContextMenuStrip = this.contextMenuStrip4;
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.ItemSize = new System.Drawing.Size(0, 20);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(752, 178);
            this.tabControl1.TabIndex = 13;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tabControl1_MouseDown);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rMenuExportTxt,
            this.rMenuExportExcel,
            this.rMenuExportWord,
            this.rMenuExportCSV,
            this.rmenuPublishData,
            this.toolStripSeparator6,
            this.rMenuStartTask,
            this.rMenuStopTask,
            this.toolStripSeparator2,
            this.rMenuCloseTabPage});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(158, 214);
            this.contextMenuStrip4.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip4_Opening);
            // 
            // rMenuExportTxt
            // 
            this.rMenuExportTxt.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportTxt.Image")));
            this.rMenuExportTxt.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuExportTxt.Name = "rMenuExportTxt";
            this.rMenuExportTxt.Size = new System.Drawing.Size(157, 22);
            this.rMenuExportTxt.Text = "导出文本";
            this.rMenuExportTxt.Click += new System.EventHandler(this.rMenuExportTxt_Click);
            // 
            // rMenuExportExcel
            // 
            this.rMenuExportExcel.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportExcel.Image")));
            this.rMenuExportExcel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuExportExcel.Name = "rMenuExportExcel";
            this.rMenuExportExcel.Size = new System.Drawing.Size(157, 22);
            this.rMenuExportExcel.Text = "导出Excel";
            this.rMenuExportExcel.Click += new System.EventHandler(this.rMenuExportExcel_Click);
            // 
            // rMenuExportWord
            // 
            this.rMenuExportWord.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportWord.Image")));
            this.rMenuExportWord.Name = "rMenuExportWord";
            this.rMenuExportWord.Size = new System.Drawing.Size(157, 22);
            this.rMenuExportWord.Text = "导出Word文档";
            this.rMenuExportWord.Click += new System.EventHandler(this.rMenuExportWord_Click);
            // 
            // rMenuExportCSV
            // 
            this.rMenuExportCSV.Name = "rMenuExportCSV";
            this.rMenuExportCSV.Size = new System.Drawing.Size(157, 22);
            this.rMenuExportCSV.Text = "导出CSV文件";
            this.rMenuExportCSV.Click += new System.EventHandler(this.rMenuExportCSV_Click);
            // 
            // rmenuPublishData
            // 
            this.rmenuPublishData.Image = ((System.Drawing.Image)(resources.GetObject("rmenuPublishData.Image")));
            this.rmenuPublishData.Name = "rmenuPublishData";
            this.rmenuPublishData.Size = new System.Drawing.Size(157, 22);
            this.rmenuPublishData.Text = "编辑/发布数据";
            this.rmenuPublishData.Click += new System.EventHandler(this.rmenuPublishData_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(154, 6);
            // 
            // rMenuStartTask
            // 
            this.rMenuStartTask.Image = ((System.Drawing.Image)(resources.GetObject("rMenuStartTask.Image")));
            this.rMenuStartTask.Name = "rMenuStartTask";
            this.rMenuStartTask.Size = new System.Drawing.Size(157, 22);
            this.rMenuStartTask.Text = "继续";
            this.rMenuStartTask.Click += new System.EventHandler(this.rMenuStartTask_Click);
            // 
            // rMenuStopTask
            // 
            this.rMenuStopTask.Image = ((System.Drawing.Image)(resources.GetObject("rMenuStopTask.Image")));
            this.rMenuStopTask.Name = "rMenuStopTask";
            this.rMenuStopTask.Size = new System.Drawing.Size(157, 22);
            this.rMenuStopTask.Text = "停止";
            this.rMenuStopTask.Click += new System.EventHandler(this.rMenuStopTask_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(154, 6);
            // 
            // rMenuCloseTabPage
            // 
            this.rMenuCloseTabPage.Image = ((System.Drawing.Image)(resources.GetObject("rMenuCloseTabPage.Image")));
            this.rMenuCloseTabPage.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuCloseTabPage.Name = "rMenuCloseTabPage";
            this.rMenuCloseTabPage.Size = new System.Drawing.Size(157, 22);
            this.rMenuCloseTabPage.Text = "关闭此选项卡";
            this.rMenuCloseTabPage.Click += new System.EventHandler(this.rMenuCloseTabPage_Click);
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
            this.imageList1.Images.SetKeyName(19, "A07.gif");
            this.imageList1.Images.SetKeyName(20, "StopPublish");
            this.imageList1.Images.SetKeyName(21, "A15.gif");
            // 
            // contextMenuStrip5
            // 
            this.contextMenuStrip5.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuSaveLog});
            this.contextMenuStrip5.Name = "contextMenuStrip5";
            this.contextMenuStrip5.Size = new System.Drawing.Size(173, 26);
            // 
            // rmenuSaveLog
            // 
            this.rmenuSaveLog.Image = ((System.Drawing.Image)(resources.GetObject("rmenuSaveLog.Image")));
            this.rmenuSaveLog.Name = "rmenuSaveLog";
            this.rmenuSaveLog.Size = new System.Drawing.Size(172, 22);
            this.rmenuSaveLog.Text = "保存数据采集日志";
            this.rmenuSaveLog.Click += new System.EventHandler(this.rmenuSaveLog_Click);
            // 
            // frmRunningTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 350);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.splitContainer2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmRunningTask";
            this.Text = "正在运行的任务";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTaskContent_FormClosed);
            this.Load += new System.EventHandler(this.frmTaskContent_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip4.ResumeLayout(false);
            this.contextMenuStrip5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private SoukeyControl.CustomControl.cMyTabControl tabControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip5;
        private System.Windows.Forms.ToolStripMenuItem rmenuSaveLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip4;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportTxt;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportExcel;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportCSV;
        private System.Windows.Forms.ToolStripMenuItem rMenuCloseTabPage;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem rmmenuStartTask;
        private System.Windows.Forms.ToolStripMenuItem rmmenuStopTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem rmenuBrowserData;
        private System.Windows.Forms.ToolStripMenuItem rmenuEditData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem rmmenuDelTask;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        internal System.Windows.Forms.DataGridView dataTask;
        private System.Windows.Forms.ToolStripMenuItem rMenuStopTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem rmenuClearCompleted;
        private System.Windows.Forms.ToolStripMenuItem rmenuPublishData;
        private System.Windows.Forms.ToolStripMenuItem rMenuStartTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportWord;
        private System.Windows.Forms.ToolStripMenuItem rmenuOverTask;
        private System.Windows.Forms.ToolStripMenuItem rMenuRefer;
    }
}