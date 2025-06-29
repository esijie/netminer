namespace MinerSpider
{
    partial class frmImportTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImportTask));
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList2 = new System.Windows.Forms.ImageList(this.components);
            this.cmdAddTask = new System.Windows.Forms.Button();
            this.cmdDelTask = new System.Windows.Forms.Button();
            this.cmdImport = new System.Windows.Forms.Button();
            this.cmdExit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.labVersion = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.labTaskVersion = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.staStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.staErr = new System.Windows.Forms.ToolStripStatusLabel();
            this.staNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.comTaskClass = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.treeTClass = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listTask
            // 
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader3,
            this.columnHeader5});
            this.listTask.FullRowSelect = true;
            this.listTask.GridLines = true;
            this.listTask.LargeImageList = this.imageList2;
            this.listTask.Location = new System.Drawing.Point(12, 88);
            this.listTask.Name = "listTask";
            this.listTask.Size = new System.Drawing.Size(575, 245);
            this.listTask.SmallImageList = this.imageList2;
            this.listTask.TabIndex = 0;
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            this.listTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listTask_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "名称";
            this.columnHeader1.Width = 180;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "版本";
            this.columnHeader4.Width = 80;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "导入状态";
            this.columnHeader3.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "文件位置";
            this.columnHeader5.Width = 160;
            // 
            // imageList2
            // 
            this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
            this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList2.Images.SetKeyName(0, "跟踪.ico");
            this.imageList2.Images.SetKeyName(1, "A12.gif");
            this.imageList2.Images.SetKeyName(2, "A32.gif");
            // 
            // cmdAddTask
            // 
            this.cmdAddTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAddTask.Image = ((System.Drawing.Image)(resources.GetObject("cmdAddTask.Image")));
            this.cmdAddTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAddTask.Location = new System.Drawing.Point(14, 60);
            this.cmdAddTask.Name = "cmdAddTask";
            this.cmdAddTask.Size = new System.Drawing.Size(104, 22);
            this.cmdAddTask.TabIndex = 3;
            this.cmdAddTask.Text = "增加采集任务";
            this.cmdAddTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAddTask.UseVisualStyleBackColor = true;
            this.cmdAddTask.Click += new System.EventHandler(this.cmdAddTask_Click);
            // 
            // cmdDelTask
            // 
            this.cmdDelTask.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDelTask.Image = ((System.Drawing.Image)(resources.GetObject("cmdDelTask.Image")));
            this.cmdDelTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelTask.Location = new System.Drawing.Point(134, 60);
            this.cmdDelTask.Name = "cmdDelTask";
            this.cmdDelTask.Size = new System.Drawing.Size(63, 22);
            this.cmdDelTask.TabIndex = 4;
            this.cmdDelTask.Text = "删  除";
            this.cmdDelTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelTask.UseVisualStyleBackColor = true;
            this.cmdDelTask.Click += new System.EventHandler(this.cmdDelTask_Click);
            // 
            // cmdImport
            // 
            this.cmdImport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdImport.Image = ((System.Drawing.Image)(resources.GetObject("cmdImport.Image")));
            this.cmdImport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdImport.Location = new System.Drawing.Point(424, 60);
            this.cmdImport.Name = "cmdImport";
            this.cmdImport.Size = new System.Drawing.Size(87, 22);
            this.cmdImport.TabIndex = 5;
            this.cmdImport.Text = "开始导入";
            this.cmdImport.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdImport.UseVisualStyleBackColor = true;
            this.cmdImport.Click += new System.EventHandler(this.cmdImport_Click);
            // 
            // cmdExit
            // 
            this.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdExit.Image = ((System.Drawing.Image)(resources.GetObject("cmdExit.Image")));
            this.cmdExit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdExit.Location = new System.Drawing.Point(522, 60);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(65, 22);
            this.cmdExit.TabIndex = 6;
            this.cmdExit.Text = "退  出";
            this.cmdExit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "您正在使用的网络矿工版本为：";
            // 
            // labVersion
            // 
            this.labVersion.AutoSize = true;
            this.labVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labVersion.ForeColor = System.Drawing.Color.Black;
            this.labVersion.Location = new System.Drawing.Point(193, 8);
            this.labVersion.Name = "labVersion";
            this.labVersion.Size = new System.Drawing.Size(22, 13);
            this.labVersion.TabIndex = 8;
            this.labVersion.Text = "2.1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(250, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(127, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "支持采集任务版本为：";
            // 
            // labTaskVersion
            // 
            this.labTaskVersion.AutoSize = true;
            this.labTaskVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labTaskVersion.ForeColor = System.Drawing.Color.Black;
            this.labTaskVersion.Location = new System.Drawing.Point(383, 8);
            this.labTaskVersion.Name = "labTaskVersion";
            this.labTaskVersion.Size = new System.Drawing.Size(22, 13);
            this.labTaskVersion.TabIndex = 10;
            this.labTaskVersion.Text = "2.0";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staStatus,
            this.staErr,
            this.staNum});
            this.statusStrip1.Location = new System.Drawing.Point(0, 336);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(599, 26);
            this.statusStrip1.TabIndex = 11;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // staStatus
            // 
            this.staStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.staStatus.Name = "staStatus";
            this.staStatus.Size = new System.Drawing.Size(507, 21);
            this.staStatus.Spring = true;
            this.staStatus.Text = "当前状态：未导入";
            this.staStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // staErr
            // 
            this.staErr.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.staErr.Image = ((System.Drawing.Image)(resources.GetObject("staErr.Image")));
            this.staErr.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.staErr.Name = "staErr";
            this.staErr.Size = new System.Drawing.Size(33, 21);
            this.staErr.Text = "0";
            // 
            // staNum
            // 
            this.staNum.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.staNum.Image = ((System.Drawing.Image)(resources.GetObject("staNum.Image")));
            this.staNum.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.staNum.Name = "staNum";
            this.staNum.Size = new System.Drawing.Size(44, 21);
            this.staNum.Text = "0/0";
            this.staNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "将采集任务导入至此目录下：";
            // 
            // comTaskClass
            // 
            this.comTaskClass.Location = new System.Drawing.Point(180, 32);
            this.comTaskClass.Name = "comTaskClass";
            this.comTaskClass.Size = new System.Drawing.Size(391, 21);
            this.comTaskClass.TabIndex = 13;
            // 
            // button7
            // 
            this.button7.Image = ((System.Drawing.Image)(resources.GetObject("button7.Image")));
            this.button7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button7.Location = new System.Drawing.Point(566, 31);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(21, 23);
            this.button7.TabIndex = 82;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // treeTClass
            // 
            this.treeTClass.ImageIndex = 0;
            this.treeTClass.ImageList = this.imageList1;
            this.treeTClass.Location = new System.Drawing.Point(180, 52);
            this.treeTClass.Name = "treeTClass";
            this.treeTClass.SelectedImageIndex = 0;
            this.treeTClass.Size = new System.Drawing.Size(391, 153);
            this.treeTClass.TabIndex = 83;
            this.treeTClass.Visible = false;
            this.treeTClass.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeTClass_AfterSelect);
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
            this.imageList1.Images.SetKeyName(19, "radar01");
            this.imageList1.Images.SetKeyName(20, "data");
            this.imageList1.Images.SetKeyName(21, "access");
            this.imageList1.Images.SetKeyName(22, "mssqlserver");
            this.imageList1.Images.SetKeyName(23, "Advance");
            this.imageList1.Images.SetKeyName(24, "DiscTracker.ico");
            this.imageList1.Images.SetKeyName(25, "Applications.ico");
            this.imageList1.Images.SetKeyName(26, "document.ico");
            this.imageList1.Images.SetKeyName(27, "Library.ico");
            this.imageList1.Images.SetKeyName(28, "rdp.gif");
            this.imageList1.Images.SetKeyName(29, "A46.ico");
            this.imageList1.Images.SetKeyName(30, "A52.gif");
            this.imageList1.Images.SetKeyName(31, "cloud.ico");
            // 
            // frmImportTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 362);
            this.Controls.Add(this.treeTClass);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.comTaskClass);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.labTaskVersion);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labVersion);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.cmdImport);
            this.Controls.Add(this.cmdDelTask);
            this.Controls.Add(this.cmdAddTask);
            this.Controls.Add(this.listTask);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImportTask";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导入采集任务";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmImportTask_FormClosed);
            this.Load += new System.EventHandler(this.frmImportTask_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button cmdAddTask;
        private System.Windows.Forms.Button cmdDelTask;
        private System.Windows.Forms.Button cmdImport;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labTaskVersion;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel staStatus;
        private System.Windows.Forms.ToolStripStatusLabel staNum;
        private System.Windows.Forms.ToolStripStatusLabel staErr;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ImageList imageList2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox comTaskClass;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TreeView treeTClass;
        private System.Windows.Forms.ImageList imageList1;
    }
}