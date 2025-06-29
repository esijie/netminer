namespace MinerSpider
{
    partial class frmProxy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProxy));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStart = new System.Windows.Forms.ToolStripButton();
            this.toolStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolmenuSave = new System.Windows.Forms.ToolStripButton();
            this.toolmenuAdd = new System.Windows.Forms.ToolStripButton();
            this.toolDel = new System.Windows.Forms.ToolStripButton();
            this.toolDelAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolExit = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.sta1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sta2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sta3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.sta4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.IsAutoUpdate = new System.Windows.Forms.CheckBox();
            this.IsDel = new System.Windows.Forms.CheckBox();
            this.listProxy = new SoukeyControl.CustomControl.cMyListview(this.components);
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolImport,
            this.toolStripSeparator1,
            this.toolStart,
            this.toolStop,
            this.toolStripSeparator3,
            this.toolmenuSave,
            this.toolmenuAdd,
            this.toolDel,
            this.toolDelAll,
            this.toolStripSeparator2,
            this.toolExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(765, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolImport
            // 
            this.toolImport.Image = ((System.Drawing.Image)(resources.GetObject("toolImport.Image")));
            this.toolImport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolImport.Name = "toolImport";
            this.toolImport.Size = new System.Drawing.Size(129, 22);
            this.toolImport.Text = "从文本导入代理地址";
            this.toolImport.Click += new System.EventHandler(this.toolImport_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStart
            // 
            this.toolStart.Image = ((System.Drawing.Image)(resources.GetObject("toolStart.Image")));
            this.toolStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStart.Name = "toolStart";
            this.toolStart.Size = new System.Drawing.Size(76, 22);
            this.toolStart.Text = "开始验证";
            this.toolStart.Click += new System.EventHandler(this.toolStart_Click);
            // 
            // toolStop
            // 
            this.toolStop.Enabled = false;
            this.toolStop.Image = ((System.Drawing.Image)(resources.GetObject("toolStop.Image")));
            this.toolStop.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStop.Name = "toolStop";
            this.toolStop.Size = new System.Drawing.Size(76, 22);
            this.toolStop.Text = "停止验证";
            this.toolStop.Click += new System.EventHandler(this.toolStop_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolmenuSave
            // 
            this.toolmenuSave.Enabled = false;
            this.toolmenuSave.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuSave.Image")));
            this.toolmenuSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolmenuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolmenuSave.Name = "toolmenuSave";
            this.toolmenuSave.Size = new System.Drawing.Size(50, 22);
            this.toolmenuSave.Text = "保存";
            this.toolmenuSave.Click += new System.EventHandler(this.toolmenuSave_Click);
            // 
            // toolmenuAdd
            // 
            this.toolmenuAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolmenuAdd.Image")));
            this.toolmenuAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolmenuAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolmenuAdd.Name = "toolmenuAdd";
            this.toolmenuAdd.Size = new System.Drawing.Size(52, 22);
            this.toolmenuAdd.Text = "增加";
            this.toolmenuAdd.Click += new System.EventHandler(this.toolmenuAdd_Click);
            // 
            // toolDel
            // 
            this.toolDel.Image = ((System.Drawing.Image)(resources.GetObject("toolDel.Image")));
            this.toolDel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDel.Name = "toolDel";
            this.toolDel.Size = new System.Drawing.Size(49, 22);
            this.toolDel.Text = "删除";
            this.toolDel.Click += new System.EventHandler(this.toolDel_Click);
            // 
            // toolDelAll
            // 
            this.toolDelAll.Image = ((System.Drawing.Image)(resources.GetObject("toolDelAll.Image")));
            this.toolDelAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelAll.Name = "toolDelAll";
            this.toolDelAll.Size = new System.Drawing.Size(76, 22);
            this.toolDelAll.Text = "全部删除";
            this.toolDelAll.Click += new System.EventHandler(this.toolDelAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolExit
            // 
            this.toolExit.Image = ((System.Drawing.Image)(resources.GetObject("toolExit.Image")));
            this.toolExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExit.Name = "toolExit";
            this.toolExit.Size = new System.Drawing.Size(52, 22);
            this.toolExit.Text = "退出";
            this.toolExit.Click += new System.EventHandler(this.toolExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sta1,
            this.sta2,
            this.sta3,
            this.sta4});
            this.statusStrip1.Location = new System.Drawing.Point(0, 389);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(765, 26);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // sta1
            // 
            this.sta1.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sta1.Name = "sta1";
            this.sta1.Size = new System.Drawing.Size(96, 21);
            this.sta1.Text = "当前状态：正常";
            // 
            // sta2
            // 
            this.sta2.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sta2.Name = "sta2";
            this.sta2.Size = new System.Drawing.Size(587, 21);
            this.sta2.Spring = true;
            this.sta2.Text = "0";
            this.sta2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // sta3
            // 
            this.sta3.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sta3.Image = ((System.Drawing.Image)(resources.GetObject("sta3.Image")));
            this.sta3.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.sta3.Name = "sta3";
            this.sta3.Size = new System.Drawing.Size(32, 21);
            this.sta3.Text = "0";
            // 
            // sta4
            // 
            this.sta4.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.sta4.Image = ((System.Drawing.Image)(resources.GetObject("sta4.Image")));
            this.sta4.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.sta4.Name = "sta4";
            this.sta4.Size = new System.Drawing.Size(35, 21);
            this.sta4.Text = "0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel1.Controls.Add(this.textBox2);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.textBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.IsAutoUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.IsDel);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listProxy);
            this.splitContainer1.Size = new System.Drawing.Size(765, 364);
            this.splitContainer1.SplitterDistance = 66;
            this.splitContainer1.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(443, 32);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(212, 21);
            this.textBox2.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(340, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "验证成功标记：";
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(106, 32);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(212, 21);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "http://www.netminer.cn";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "代理验证地址：";
            // 
            // IsAutoUpdate
            // 
            this.IsAutoUpdate.AutoSize = true;
            this.IsAutoUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IsAutoUpdate.Location = new System.Drawing.Point(294, 9);
            this.IsAutoUpdate.Name = "IsAutoUpdate";
            this.IsAutoUpdate.Size = new System.Drawing.Size(249, 16);
            this.IsAutoUpdate.TabIndex = 1;
            this.IsAutoUpdate.Text = "验证完成后，自动更新采集任务代理IP队列";
            this.IsAutoUpdate.UseVisualStyleBackColor = true;
            this.IsAutoUpdate.Visible = false;
            // 
            // IsDel
            // 
            this.IsDel.AutoSize = true;
            this.IsDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.IsDel.Location = new System.Drawing.Point(12, 9);
            this.IsDel.Name = "IsDel";
            this.IsDel.Size = new System.Drawing.Size(273, 16);
            this.IsDel.TabIndex = 0;
            this.IsDel.Text = "验证代理IP有效性的同时自动删除无效的代理IP";
            this.IsDel.UseVisualStyleBackColor = true;
            // 
            // listProxy
            // 
            this.listProxy.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listProxy.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProxy.FullRowSelect = true;
            this.listProxy.GridLines = true;
            this.listProxy.Location = new System.Drawing.Point(0, 0);
            this.listProxy.MultiSelect = false;
            this.listProxy.Name = "listProxy";
            this.listProxy.Size = new System.Drawing.Size(765, 294);
            this.listProxy.TabIndex = 2;
            this.listProxy.UseCompatibleStateImageBehavior = false;
            this.listProxy.View = System.Windows.Forms.View.Details;
            this.listProxy.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProxy_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "IP";
            this.columnHeader1.Width = 247;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "端口";
            this.columnHeader2.Width = 137;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "用户名";
            this.columnHeader3.Width = 147;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "密码";
            this.columnHeader4.Width = 104;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "验证状态";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "访问时长";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmProxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 415);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmProxy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "代理IP管理";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProxy_FormClosing);
            this.Load += new System.EventHandler(this.frmProxy_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolImport;
        private System.Windows.Forms.ToolStripButton toolStart;
        private System.Windows.Forms.ToolStripButton toolStop;
        private System.Windows.Forms.ToolStripButton toolDel;
        private System.Windows.Forms.ToolStripButton toolExit;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private SoukeyControl.CustomControl.cMyListview listProxy;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ToolStripStatusLabel sta1;
        private System.Windows.Forms.ToolStripStatusLabel sta2;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox IsDel;
        private System.Windows.Forms.CheckBox IsAutoUpdate;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripStatusLabel sta3;
        private System.Windows.Forms.ToolStripStatusLabel sta4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ToolStripButton toolmenuSave;
        private System.Windows.Forms.ToolStripButton toolmenuAdd;
        private System.Windows.Forms.ToolStripButton toolDelAll;
    }
}