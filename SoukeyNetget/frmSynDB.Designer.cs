namespace MinerSpider
{
    partial class frmSynDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSynDB));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAddDb = new System.Windows.Forms.ToolStripButton();
            this.toolDelDb = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolSaveDb = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolExit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listDb = new System.Windows.Forms.ListBox();
            this.txtDb = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAddDb,
            this.toolDelDb,
            this.toolStripSeparator1,
            this.toolSaveDb,
            this.toolStripSeparator2,
            this.toolExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(694, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAddDb
            // 
            this.toolAddDb.Image = ((System.Drawing.Image)(resources.GetObject("toolAddDb.Image")));
            this.toolAddDb.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolAddDb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAddDb.Name = "toolAddDb";
            this.toolAddDb.Size = new System.Drawing.Size(76, 22);
            this.toolAddDb.Text = "添加词库";
            this.toolAddDb.Click += new System.EventHandler(this.toolAddDb_Click);
            // 
            // toolDelDb
            // 
            this.toolDelDb.Image = ((System.Drawing.Image)(resources.GetObject("toolDelDb.Image")));
            this.toolDelDb.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolDelDb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDelDb.Name = "toolDelDb";
            this.toolDelDb.Size = new System.Drawing.Size(73, 22);
            this.toolDelDb.Text = "删除词库";
            this.toolDelDb.Click += new System.EventHandler(this.toolDelDb_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolSaveDb
            // 
            this.toolSaveDb.Enabled = false;
            this.toolSaveDb.Image = ((System.Drawing.Image)(resources.GetObject("toolSaveDb.Image")));
            this.toolSaveDb.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolSaveDb.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSaveDb.Name = "toolSaveDb";
            this.toolSaveDb.Size = new System.Drawing.Size(98, 22);
            this.toolSaveDb.Text = "保存词库数据";
            this.toolSaveDb.Click += new System.EventHandler(this.toolSaveDb_Click);
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
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listDb);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.txtDb);
            this.splitContainer1.Size = new System.Drawing.Size(694, 351);
            this.splitContainer1.SplitterDistance = 230;
            this.splitContainer1.TabIndex = 1;
            // 
            // listDb
            // 
            this.listDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listDb.FormattingEnabled = true;
            this.listDb.ItemHeight = 12;
            this.listDb.Location = new System.Drawing.Point(0, 0);
            this.listDb.Name = "listDb";
            this.listDb.Size = new System.Drawing.Size(230, 351);
            this.listDb.TabIndex = 0;
            this.listDb.SelectedIndexChanged += new System.EventHandler(this.listDb_SelectedIndexChanged);
            // 
            // txtDb
            // 
            this.txtDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDb.Location = new System.Drawing.Point(0, 0);
            this.txtDb.Multiline = true;
            this.txtDb.Name = "txtDb";
            this.txtDb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDb.Size = new System.Drawing.Size(460, 351);
            this.txtDb.TabIndex = 0;
            this.txtDb.TextChanged += new System.EventHandler(this.txtDb_TextChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 376);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(694, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(679, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "同义词库为一行一组同义词，词与词之间请用逗号,分割。";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            this.toolStripProgressBar1.Visible = false;
            // 
            // frmSynDB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 398);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSynDB";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "同义词库管理";
            this.Load += new System.EventHandler(this.frmSynDB_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolAddDb;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox listDb;
        private System.Windows.Forms.TextBox txtDb;
        private System.Windows.Forms.ToolStripButton toolDelDb;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolSaveDb;
        private System.Windows.Forms.ToolStripButton toolExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
    }
}