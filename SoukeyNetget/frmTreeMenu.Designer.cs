namespace MinerSpider
{
    partial class frmTreeMenu
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("正在运行", 1, 1);
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("采集任务", 25, 25);
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("执行日志", 18, 18);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("远程服务器", 28, 28, new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("云采", 31, 31);
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("采集任务执行计划");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("采集任务管理", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("发布至网站", 30, 30);
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("发布至数据库", 20, 20);
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("发布模板管理", 12, 12, new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTreeMenu));
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.treeMenu = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuAddTaskClass = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuRenameTaskClass = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuDelTaskClass = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator16 = new System.Windows.Forms.ToolStripSeparator();
            this.menuAddTask = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuAddRadarRule = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAddTaskPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEditTask = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuRadarData = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdCloseInfo = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.treeMenu);
            this.splitContainer3.Panel1MinSize = 300;
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.BackColor = System.Drawing.SystemColors.Control;
            this.splitContainer3.Panel2.Controls.Add(this.txtLog);
            this.splitContainer3.Panel2.Controls.Add(this.panel1);
            this.splitContainer3.Panel2MinSize = 0;
            this.splitContainer3.Size = new System.Drawing.Size(221, 434);
            this.splitContainer3.SplitterDistance = 325;
            this.splitContainer3.TabIndex = 2;
            // 
            // treeMenu
            // 
            this.treeMenu.AllowDrop = true;
            this.treeMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeMenu.ContextMenuStrip = this.contextMenuStrip1;
            this.treeMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMenu.HideSelection = false;
            this.treeMenu.ImageIndex = 0;
            this.treeMenu.ImageList = this.imageList1;
            this.treeMenu.Location = new System.Drawing.Point(0, 0);
            this.treeMenu.Name = "treeMenu";
            treeNode1.ImageIndex = 1;
            treeNode1.Name = "nodRemoteRunning";
            treeNode1.SelectedImageIndex = 1;
            treeNode1.Text = "正在运行";
            treeNode2.ImageIndex = 25;
            treeNode2.Name = "nodRemoteTask";
            treeNode2.SelectedImageIndex = 25;
            treeNode2.Text = "采集任务";
            treeNode3.ImageIndex = 18;
            treeNode3.Name = "nodRemoteLog";
            treeNode3.SelectedImageIndex = 18;
            treeNode3.Text = "执行日志";
            treeNode4.ImageIndex = 28;
            treeNode4.Name = "nodRemote";
            treeNode4.SelectedImageIndex = 28;
            treeNode4.Text = "远程服务器";
            treeNode5.ImageIndex = 31;
            treeNode5.Name = "nodCloud";
            treeNode5.SelectedImageIndex = 31;
            treeNode5.Text = "云采";
            treeNode6.ImageKey = "taskplan";
            treeNode6.Name = "nodPlanRunning";
            treeNode6.SelectedImageKey = "taskplan";
            treeNode6.Text = "采集任务执行计划";
            treeNode6.ToolTipText = "查看采集任务定制执行计划";
            treeNode7.ImageKey = "Applications.ico";
            treeNode7.Name = "nodTaskClass";
            treeNode7.SelectedImageKey = "Applications.ico";
            treeNode7.Text = "采集任务管理";
            treeNode7.ToolTipText = "查看已配置的采集规则信息";
            treeNode8.ImageIndex = 30;
            treeNode8.Name = "nodPublishByWeb";
            treeNode8.SelectedImageIndex = 30;
            treeNode8.Text = "发布至网站";
            treeNode9.ImageIndex = 20;
            treeNode9.Name = "nodPublishByDB";
            treeNode9.SelectedImageIndex = 20;
            treeNode9.Text = "发布至数据库";
            treeNode10.ImageIndex = 12;
            treeNode10.Name = "nodPublishTemplate";
            treeNode10.SelectedImageIndex = 12;
            treeNode10.Text = "发布模板管理";
            this.treeMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode7,
            treeNode10});
            this.treeMenu.SelectedImageIndex = 0;
            this.treeMenu.Size = new System.Drawing.Size(221, 325);
            this.treeMenu.TabIndex = 0;
            this.treeMenu.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeMenu_AfterLabelEdit);
            this.treeMenu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMenu_AfterSelect);
            this.treeMenu.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeMenu_NodeMouseClick);
            this.treeMenu.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeMenu_DragDrop);
            this.treeMenu.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeMenu_DragEnter);
            this.treeMenu.DragOver += new System.Windows.Forms.DragEventHandler(this.treeMenu_DragOver);
            this.treeMenu.DragLeave += new System.EventHandler(this.treeMenu_DragLeave);
            this.treeMenu.Enter += new System.EventHandler(this.treeMenu_Enter);
            this.treeMenu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeMenu_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuAddTaskClass,
            this.rmenuRenameTaskClass,
            this.rmenuDelTaskClass,
            this.toolStripSeparator16,
            this.menuAddTask,
            this.rmenuAddRadarRule,
            this.menuAddTaskPlan,
            this.menuEditTask,
            this.toolStripSeparator1,
            this.menuRadarData});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 192);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
            // 
            // rmenuAddTaskClass
            // 
            this.rmenuAddTaskClass.Image = ((System.Drawing.Image)(resources.GetObject("rmenuAddTaskClass.Image")));
            this.rmenuAddTaskClass.Name = "rmenuAddTaskClass";
            this.rmenuAddTaskClass.Size = new System.Drawing.Size(208, 22);
            this.rmenuAddTaskClass.Text = "添加分类";
            this.rmenuAddTaskClass.Click += new System.EventHandler(this.rmenuAddTaskClass_Click);
            // 
            // rmenuRenameTaskClass
            // 
            this.rmenuRenameTaskClass.Enabled = false;
            this.rmenuRenameTaskClass.Name = "rmenuRenameTaskClass";
            this.rmenuRenameTaskClass.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.rmenuRenameTaskClass.Size = new System.Drawing.Size(208, 22);
            this.rmenuRenameTaskClass.Text = "重命名";
            this.rmenuRenameTaskClass.Click += new System.EventHandler(this.rmenuRenameTaskClass_Click);
            // 
            // rmenuDelTaskClass
            // 
            this.rmenuDelTaskClass.Enabled = false;
            this.rmenuDelTaskClass.Image = ((System.Drawing.Image)(resources.GetObject("rmenuDelTaskClass.Image")));
            this.rmenuDelTaskClass.Name = "rmenuDelTaskClass";
            this.rmenuDelTaskClass.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.rmenuDelTaskClass.Size = new System.Drawing.Size(208, 22);
            this.rmenuDelTaskClass.Text = "删除分类";
            this.rmenuDelTaskClass.Click += new System.EventHandler(this.rmenuDelTaskClass_Click);
            // 
            // toolStripSeparator16
            // 
            this.toolStripSeparator16.Name = "toolStripSeparator16";
            this.toolStripSeparator16.Size = new System.Drawing.Size(205, 6);
            // 
            // menuAddTask
            // 
            this.menuAddTask.Name = "menuAddTask";
            this.menuAddTask.Size = new System.Drawing.Size(208, 22);
            this.menuAddTask.Text = "新建采集任务";
            this.menuAddTask.Click += new System.EventHandler(this.menuAddTask_Click);
            // 
            // rmenuAddRadarRule
            // 
            this.rmenuAddRadarRule.Image = ((System.Drawing.Image)(resources.GetObject("rmenuAddRadarRule.Image")));
            this.rmenuAddRadarRule.Name = "rmenuAddRadarRule";
            this.rmenuAddRadarRule.Size = new System.Drawing.Size(208, 22);
            this.rmenuAddRadarRule.Text = "新建监控规则";
            this.rmenuAddRadarRule.Click += new System.EventHandler(this.rmenuAddRadarRule_Click);
            // 
            // menuAddTaskPlan
            // 
            this.menuAddTaskPlan.Image = ((System.Drawing.Image)(resources.GetObject("menuAddTaskPlan.Image")));
            this.menuAddTaskPlan.Name = "menuAddTaskPlan";
            this.menuAddTaskPlan.Size = new System.Drawing.Size(208, 22);
            this.menuAddTaskPlan.Text = "新建任务执行计划";
            this.menuAddTaskPlan.Click += new System.EventHandler(this.menuAddTaskPlan_Click);
            // 
            // menuEditTask
            // 
            this.menuEditTask.Image = ((System.Drawing.Image)(resources.GetObject("menuEditTask.Image")));
            this.menuEditTask.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.menuEditTask.Name = "menuEditTask";
            this.menuEditTask.Size = new System.Drawing.Size(208, 22);
            this.menuEditTask.Text = "统一修改此分类下的任务";
            this.menuEditTask.Visible = false;
            this.menuEditTask.Click += new System.EventHandler(this.menuEditTask_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // menuRadarData
            // 
            this.menuRadarData.Enabled = false;
            this.menuRadarData.Image = ((System.Drawing.Image)(resources.GetObject("menuRadarData.Image")));
            this.menuRadarData.Name = "menuRadarData";
            this.menuRadarData.Size = new System.Drawing.Size(208, 22);
            this.menuRadarData.Text = "查看雷达数据";
            this.menuRadarData.Click += new System.EventHandler(this.menuRadarData_Click);
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
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(0, 21);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(221, 84);
            this.txtLog.TabIndex = 2;
            this.txtLog.Text = "";
            this.txtLog.WordWrap = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.Controls.Add(this.cmdCloseInfo);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(221, 21);
            this.panel1.TabIndex = 1;
            // 
            // cmdCloseInfo
            // 
            this.cmdCloseInfo.BackColor = System.Drawing.Color.Transparent;
            this.cmdCloseInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cmdCloseInfo.Dock = System.Windows.Forms.DockStyle.Right;
            this.cmdCloseInfo.FlatAppearance.BorderSize = 0;
            this.cmdCloseInfo.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.cmdCloseInfo.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.cmdCloseInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCloseInfo.Image = ((System.Drawing.Image)(resources.GetObject("cmdCloseInfo.Image")));
            this.cmdCloseInfo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCloseInfo.Location = new System.Drawing.Point(206, 0);
            this.cmdCloseInfo.Name = "cmdCloseInfo";
            this.cmdCloseInfo.Size = new System.Drawing.Size(15, 21);
            this.cmdCloseInfo.TabIndex = 1;
            this.cmdCloseInfo.UseVisualStyleBackColor = false;
            this.cmdCloseInfo.Click += new System.EventHandler(this.cmdCloseInfo_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(6, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "系统信息";
            // 
            // frmTreeMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(221, 434);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.splitContainer3);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmTreeMenu";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockLeft;
            this.Text = "树形菜单";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmTreeMenu_FormClosed);
            this.Load += new System.EventHandler(this.frmTreeMenu_Load);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cmdCloseInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddTaskClass;
        private System.Windows.Forms.ToolStripMenuItem rmenuRenameTaskClass;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelTaskClass;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator16;
        private System.Windows.Forms.ToolStripMenuItem menuAddTask;
        private System.Windows.Forms.ToolStripMenuItem menuAddTaskPlan;
        private System.Windows.Forms.ToolStripMenuItem rmenuAddRadarRule;
        internal System.Windows.Forms.TreeView treeMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuRadarData;
        private System.Windows.Forms.ToolStripMenuItem menuEditTask;
    }
}