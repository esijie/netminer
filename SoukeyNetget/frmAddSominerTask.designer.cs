namespace MinerSpider
{
    partial class frmAddSominerTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddSominerTask));
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.treeTClass = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.comTaskClass = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listTask
            // 
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listTask.FullRowSelect = true;
            this.listTask.Location = new System.Drawing.Point(10, 39);
            this.listTask.MultiSelect = false;
            this.listTask.Name = "listTask";
            this.listTask.Size = new System.Drawing.Size(434, 155);
            this.listTask.TabIndex = 11;
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            this.listTask.DoubleClick += new System.EventHandler(this.listTask_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "任务名称";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "采集内容";
            this.columnHeader2.Width = 260;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "任务分类：";
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(1, 199);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(452, 5);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            // 
            // cmdCancel
            // 
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(379, 214);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 21);
            this.cmdCancel.TabIndex = 14;
            this.cmdCancel.Text = "取 消";
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
            this.cmdOK.Location = new System.Drawing.Point(306, 214);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(65, 21);
            this.cmdOK.TabIndex = 13;
            this.cmdOK.Text = "确 定";
            this.cmdOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // treeTClass
            // 
            this.treeTClass.ImageIndex = 0;
            this.treeTClass.ImageList = this.imageList1;
            this.treeTClass.Location = new System.Drawing.Point(78, 33);
            this.treeTClass.Name = "treeTClass";
            this.treeTClass.SelectedImageIndex = 0;
            this.treeTClass.Size = new System.Drawing.Size(365, 168);
            this.treeTClass.TabIndex = 15;
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
            // 
            // comTaskClass
            // 
            this.comTaskClass.Location = new System.Drawing.Point(78, 11);
            this.comTaskClass.Name = "comTaskClass";
            this.comTaskClass.Size = new System.Drawing.Size(351, 21);
            this.comTaskClass.TabIndex = 16;
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(423, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(21, 23);
            this.button1.TabIndex = 18;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // frmAddSominerTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 246);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comTaskClass);
            this.Controls.Add(this.treeTClass);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listTask);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddSominerTask";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "增加网络矿工采集任务";
            this.Load += new System.EventHandler(this.frmAddSominerTask_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TreeView treeTClass;
        private System.Windows.Forms.TextBox comTaskClass;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ImageList imageList1;
    }
}