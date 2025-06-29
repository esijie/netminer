namespace MinerSpider
{
    partial class frmTaskContent
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTaskContent));
            this.dataTask = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmmenuStartTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuCopyTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuPasteTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuRefer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator18 = new System.Windows.Forms.ToolStripSeparator();
            this.rmmenuEditTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmmenuRenameTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmmenuDelTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.rmmenuNewTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuAddPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuUploadTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuUploadWebTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rmenuImportTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuExportTask = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.rmmenuClearTaskDB = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
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
            this.dataTask.Size = new System.Drawing.Size(860, 465);
            this.dataTask.StandardTab = true;
            this.dataTask.TabIndex = 1;
            this.dataTask.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataTask_CellBeginEdit);
            this.dataTask.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTask_CellClick);
            this.dataTask.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTask_CellEndEdit);
            this.dataTask.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataTask_EditingControlShowing);
            this.dataTask.DoubleClick += new System.EventHandler(this.dataTask_DoubleClick);
            this.dataTask.Enter += new System.EventHandler(this.dataTask_Enter);
            this.dataTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTask_KeyDown);
            this.dataTask.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataTask_MouseDown);
            this.dataTask.MouseMove += new System.Windows.Forms.MouseEventHandler(this.dataTask_MouseMove);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmmenuStartTask,
            this.rmmenuClearTaskDB,
            this.toolStripSeparator5,
            this.rmenuCopyTask,
            this.rmenuPasteTask,
            this.rMenuRefer,
            this.toolStripSeparator18,
            this.rmmenuEditTask,
            this.rmmenuRenameTask,
            this.rmmenuDelTask,
            this.toolStripSeparator3,
            this.rmmenuNewTask,
            this.rmenuAddPlan,
            this.rmenuUploadTask,
            this.toolStripSeparator7,
            this.rmenuUploadWebTask,
            this.toolStripSeparator1,
            this.rmenuImportTask,
            this.rmenuExportTask});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(197, 364);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
            // 
            // rmmenuStartTask
            // 
            this.rmmenuStartTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuStartTask.Image")));
            this.rmmenuStartTask.Name = "rmmenuStartTask";
            this.rmmenuStartTask.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.rmmenuStartTask.Size = new System.Drawing.Size(196, 22);
            this.rmmenuStartTask.Text = "启动任务";
            this.rmmenuStartTask.Click += new System.EventHandler(this.rmmenuStartTask_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(193, 6);
            // 
            // rmenuCopyTask
            // 
            this.rmenuCopyTask.Image = ((System.Drawing.Image)(resources.GetObject("rmenuCopyTask.Image")));
            this.rmenuCopyTask.Name = "rmenuCopyTask";
            this.rmenuCopyTask.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.rmenuCopyTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuCopyTask.Text = "复制";
            this.rmenuCopyTask.Click += new System.EventHandler(this.rmenuCopyTask_Click);
            // 
            // rmenuPasteTask
            // 
            this.rmenuPasteTask.Image = ((System.Drawing.Image)(resources.GetObject("rmenuPasteTask.Image")));
            this.rmenuPasteTask.Name = "rmenuPasteTask";
            this.rmenuPasteTask.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.rmenuPasteTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuPasteTask.Text = "粘贴";
            this.rmenuPasteTask.Click += new System.EventHandler(this.rmenuPasteTask_Click);
            // 
            // rMenuRefer
            // 
            this.rMenuRefer.Image = ((System.Drawing.Image)(resources.GetObject("rMenuRefer.Image")));
            this.rMenuRefer.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuRefer.Name = "rMenuRefer";
            this.rMenuRefer.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.rMenuRefer.Size = new System.Drawing.Size(196, 22);
            this.rMenuRefer.Text = "刷新";
            this.rMenuRefer.Click += new System.EventHandler(this.rMenuRefer_Click);
            // 
            // toolStripSeparator18
            // 
            this.toolStripSeparator18.Name = "toolStripSeparator18";
            this.toolStripSeparator18.Size = new System.Drawing.Size(193, 6);
            // 
            // rmmenuEditTask
            // 
            this.rmmenuEditTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuEditTask.Image")));
            this.rmmenuEditTask.Name = "rmmenuEditTask";
            this.rmmenuEditTask.Size = new System.Drawing.Size(196, 22);
            this.rmmenuEditTask.Text = "编辑";
            this.rmmenuEditTask.Click += new System.EventHandler(this.rmmenuEditTask_Click);
            // 
            // rmmenuRenameTask
            // 
            this.rmmenuRenameTask.Name = "rmmenuRenameTask";
            this.rmmenuRenameTask.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.rmmenuRenameTask.Size = new System.Drawing.Size(196, 22);
            this.rmmenuRenameTask.Text = "重命名";
            this.rmmenuRenameTask.Click += new System.EventHandler(this.rmmenuRenameTask_Click);
            // 
            // rmmenuDelTask
            // 
            this.rmmenuDelTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuDelTask.Image")));
            this.rmmenuDelTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmmenuDelTask.Name = "rmmenuDelTask";
            this.rmmenuDelTask.Size = new System.Drawing.Size(196, 22);
            this.rmmenuDelTask.Text = "删除        Delete";
            this.rmmenuDelTask.Click += new System.EventHandler(this.rmmenuDelTask_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(193, 6);
            // 
            // rmmenuNewTask
            // 
            this.rmmenuNewTask.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuNewTask.Image")));
            this.rmmenuNewTask.Name = "rmmenuNewTask";
            this.rmmenuNewTask.Size = new System.Drawing.Size(196, 22);
            this.rmmenuNewTask.Text = "新建采集任务";
            this.rmmenuNewTask.Click += new System.EventHandler(this.rmmenuNewTask_Click);
            // 
            // rmenuAddPlan
            // 
            this.rmenuAddPlan.Image = ((System.Drawing.Image)(resources.GetObject("rmenuAddPlan.Image")));
            this.rmenuAddPlan.Name = "rmenuAddPlan";
            this.rmenuAddPlan.Size = new System.Drawing.Size(196, 22);
            this.rmenuAddPlan.Text = "新建任务执行计划";
            this.rmenuAddPlan.Click += new System.EventHandler(this.rmenuAddPlan_Click);
            // 
            // rmenuUploadTask
            // 
            this.rmenuUploadTask.Image = ((System.Drawing.Image)(resources.GetObject("rmenuUploadTask.Image")));
            this.rmenuUploadTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmenuUploadTask.Name = "rmenuUploadTask";
            this.rmenuUploadTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuUploadTask.Text = "上传采集任务";
            this.rmenuUploadTask.Click += new System.EventHandler(this.rmenuUploadTask_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(193, 6);
            // 
            // rmenuUploadWebTask
            // 
            this.rmenuUploadWebTask.Name = "rmenuUploadWebTask";
            this.rmenuUploadWebTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuUploadWebTask.Text = "将采集任务发布到网站";
            this.rmenuUploadWebTask.Click += new System.EventHandler(this.rmenuUploadWebTask_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(193, 6);
            // 
            // rmenuImportTask
            // 
            this.rmenuImportTask.Name = "rmenuImportTask";
            this.rmenuImportTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuImportTask.Text = "导入采集任务";
            this.rmenuImportTask.Click += new System.EventHandler(this.rmenuImportTask_Click);
            // 
            // rmenuExportTask
            // 
            this.rmenuExportTask.Image = ((System.Drawing.Image)(resources.GetObject("rmenuExportTask.Image")));
            this.rmenuExportTask.Name = "rmenuExportTask";
            this.rmenuExportTask.Size = new System.Drawing.Size(196, 22);
            this.rmenuExportTask.Text = "导出采集任务";
            this.rmenuExportTask.Click += new System.EventHandler(this.rmenuExportTask_Click);
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
            this.imageList1.Images.SetKeyName(22, "PublishByDb");
            this.imageList1.Images.SetKeyName(23, "PublishByWeb");
            // 
            // rmmenuClearTaskDB
            // 
            this.rmmenuClearTaskDB.Image = ((System.Drawing.Image)(resources.GetObject("rmmenuClearTaskDB.Image")));
            this.rmmenuClearTaskDB.Name = "rmmenuClearTaskDB";
            this.rmmenuClearTaskDB.Size = new System.Drawing.Size(196, 22);
            this.rmmenuClearTaskDB.Text = "清除排重库";
            this.rmmenuClearTaskDB.Click += new System.EventHandler(this.rmmenuClearTaskDB_Click);
            // 
            // frmTaskContent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(860, 465);
            this.Controls.Add(this.dataTask);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTaskContent";
            this.Text = "采集任务";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmTaskContent_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView dataTask;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem rmmenuStartTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem rmenuCopyTask;
        private System.Windows.Forms.ToolStripMenuItem rmenuPasteTask;
        private System.Windows.Forms.ToolStripMenuItem rMenuRefer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator18;
        private System.Windows.Forms.ToolStripMenuItem rmmenuEditTask;
        private System.Windows.Forms.ToolStripMenuItem rmmenuRenameTask;
        private System.Windows.Forms.ToolStripMenuItem rmmenuDelTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem rmmenuNewTask;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddPlan;
        private System.Windows.Forms.ToolStripMenuItem rmenuUploadTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem rmenuUploadWebTask;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem rmenuImportTask;
        private System.Windows.Forms.ToolStripMenuItem rmenuExportTask;
        private System.Windows.Forms.ToolStripMenuItem rmmenuClearTaskDB;
    }
}