namespace MinerSpider
{
    partial class frmAddPlanTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddPlanTask));
            this.raSoukeyTask = new System.Windows.Forms.RadioButton();
            this.raOtherTask = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button7 = new System.Windows.Forms.Button();
            this.comTaskClass = new System.Windows.Forms.TextBox();
            this.treeTClass = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listTask = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.comTableName = new System.Windows.Forms.ComboBox();
            this.button12 = new System.Windows.Forms.Button();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.raMySqlTask = new System.Windows.Forms.RadioButton();
            this.raMSSQLTask = new System.Windows.Forms.RadioButton();
            this.raAccessTask = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txtPara = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.raDataTask = new System.Windows.Forms.RadioButton();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // raSoukeyTask
            // 
            resources.ApplyResources(this.raSoukeyTask, "raSoukeyTask");
            this.raSoukeyTask.Checked = true;
            this.raSoukeyTask.Name = "raSoukeyTask";
            this.raSoukeyTask.TabStop = true;
            this.raSoukeyTask.UseVisualStyleBackColor = true;
            this.raSoukeyTask.CheckedChanged += new System.EventHandler(this.raSoukeyTask_CheckedChanged);
            // 
            // raOtherTask
            // 
            resources.ApplyResources(this.raOtherTask, "raOtherTask");
            this.raOtherTask.Name = "raOtherTask";
            this.raOtherTask.UseVisualStyleBackColor = true;
            this.raOtherTask.CheckedChanged += new System.EventHandler(this.raOtherTask_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.comTaskClass);
            this.panel1.Controls.Add(this.treeTClass);
            this.panel1.Controls.Add(this.listTask);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // button7
            // 
            resources.ApplyResources(this.button7, "button7");
            this.button7.Name = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // comTaskClass
            // 
            this.comTaskClass.BackColor = System.Drawing.Color.White;
            resources.ApplyResources(this.comTaskClass, "comTaskClass");
            this.comTaskClass.Name = "comTaskClass";
            this.comTaskClass.ReadOnly = true;
            // 
            // treeTClass
            // 
            resources.ApplyResources(this.treeTClass, "treeTClass");
            this.treeTClass.ImageList = this.imageList1;
            this.treeTClass.Name = "treeTClass";
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
            // listTask
            // 
            this.listTask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.listTask.FullRowSelect = true;
            resources.ApplyResources(this.listTask, "listTask");
            this.listTask.MultiSelect = false;
            this.listTask.Name = "listTask";
            this.listTask.UseCompatibleStateImageBehavior = false;
            this.listTask.View = System.Windows.Forms.View.Details;
            this.listTask.DoubleClick += new System.EventHandler(this.listTask_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.panel3);
            this.groupBox1.Controls.Add(this.panel2);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.comTableName);
            this.panel3.Controls.Add(this.button12);
            this.panel3.Controls.Add(this.txtDataSource);
            this.panel3.Controls.Add(this.raMySqlTask);
            this.panel3.Controls.Add(this.raMSSQLTask);
            this.panel3.Controls.Add(this.raAccessTask);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label6);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // comTableName
            // 
            resources.ApplyResources(this.comTableName, "comTableName");
            this.comTableName.FormattingEnabled = true;
            this.comTableName.Name = "comTableName";
            this.comTableName.DropDown += new System.EventHandler(this.comTableName_DropDown);
            // 
            // button12
            // 
            resources.ApplyResources(this.button12, "button12");
            this.button12.Name = "button12";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // txtDataSource
            // 
            this.txtDataSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtDataSource, "txtDataSource");
            this.txtDataSource.Name = "txtDataSource";
            // 
            // raMySqlTask
            // 
            resources.ApplyResources(this.raMySqlTask, "raMySqlTask");
            this.raMySqlTask.Name = "raMySqlTask";
            this.raMySqlTask.UseVisualStyleBackColor = true;
            // 
            // raMSSQLTask
            // 
            resources.ApplyResources(this.raMSSQLTask, "raMSSQLTask");
            this.raMSSQLTask.Name = "raMSSQLTask";
            this.raMSSQLTask.UseVisualStyleBackColor = true;
            // 
            // raAccessTask
            // 
            resources.ApplyResources(this.raAccessTask, "raAccessTask");
            this.raAccessTask.Checked = true;
            this.raAccessTask.Name = "raAccessTask";
            this.raAccessTask.TabStop = true;
            this.raAccessTask.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txtPara);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.txtFileName);
            this.panel2.Controls.Add(this.label2);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // txtPara
            // 
            this.txtPara.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtPara, "txtPara");
            this.txtPara.Name = "txtPara";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtFileName
            // 
            this.txtFileName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txtFileName, "txtFileName");
            this.txtFileName.Name = "txtFileName";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // raDataTask
            // 
            resources.ApplyResources(this.raDataTask, "raDataTask");
            this.raDataTask.Name = "raDataTask";
            this.raDataTask.TabStop = true;
            this.raDataTask.UseVisualStyleBackColor = true;
            this.raDataTask.CheckedChanged += new System.EventHandler(this.raDataTask_CheckedChanged);
            // 
            // cmdCancel
            // 
            resources.ApplyResources(this.cmdCancel, "cmdCancel");
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOK
            // 
            resources.ApplyResources(this.cmdOK, "cmdOK");
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.UseVisualStyleBackColor = true;
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // frmAddPlanTask
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.raDataTask);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.raOtherTask);
            this.Controls.Add(this.raSoukeyTask);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddPlanTask";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmAddPlanTask_FormClosed);
            this.Load += new System.EventHandler(this.frmAddPlanTask_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton raSoukeyTask;
        private System.Windows.Forms.RadioButton raOtherTask;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView listTask;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOK;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPara;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFileName;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.RadioButton raDataTask;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ComboBox comTableName;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.RadioButton raMySqlTask;
        private System.Windows.Forms.RadioButton raMSSQLTask;
        private System.Windows.Forms.RadioButton raAccessTask;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox comTaskClass;
        private System.Windows.Forms.TreeView treeTClass;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ImageList imageList1;
    }
}