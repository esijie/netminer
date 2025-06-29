namespace SoukeyPublishManage
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolAdd = new System.Windows.Forms.ToolStripSplitButton();
            this.toolMenuAddWeb = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMenuAddDb = new System.Windows.Forms.ToolStripMenuItem();
            this.toolEdit = new System.Windows.Forms.ToolStripButton();
            this.toolDel = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolExport = new System.Windows.Forms.ToolStripButton();
            this.toolImport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolExit = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.listTemplate = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolUpgrade = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolAdd,
            this.toolEdit,
            this.toolDel,
            this.toolStripSeparator1,
            this.toolExport,
            this.toolImport,
            this.toolUpgrade,
            this.toolStripSeparator2,
            this.toolExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(813, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolAdd
            // 
            this.toolAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMenuAddWeb,
            this.toolMenuAddDb});
            this.toolAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolAdd.Image")));
            this.toolAdd.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAdd.Name = "toolAdd";
            this.toolAdd.Size = new System.Drawing.Size(63, 22);
            this.toolAdd.Text = "添加";
            // 
            // toolMenuAddWeb
            // 
            this.toolMenuAddWeb.Name = "toolMenuAddWeb";
            this.toolMenuAddWeb.Size = new System.Drawing.Size(182, 22);
            this.toolMenuAddWeb.Text = "添加Web发布模版";
            this.toolMenuAddWeb.Click += new System.EventHandler(this.toolMenuAddWeb_Click);
            // 
            // toolMenuAddDb
            // 
            this.toolMenuAddDb.Name = "toolMenuAddDb";
            this.toolMenuAddDb.Size = new System.Drawing.Size(182, 22);
            this.toolMenuAddDb.Text = "添加数据库发布模版";
            this.toolMenuAddDb.Click += new System.EventHandler(this.toolMenuAddDb_Click);
            // 
            // toolEdit
            // 
            this.toolEdit.Image = ((System.Drawing.Image)(resources.GetObject("toolEdit.Image")));
            this.toolEdit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdit.Name = "toolEdit";
            this.toolEdit.Size = new System.Drawing.Size(47, 22);
            this.toolEdit.Text = "编辑";
            this.toolEdit.Click += new System.EventHandler(this.toolEdit_Click);
            // 
            // toolDel
            // 
            this.toolDel.Image = ((System.Drawing.Image)(resources.GetObject("toolDel.Image")));
            this.toolDel.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolDel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDel.Name = "toolDel";
            this.toolDel.Size = new System.Drawing.Size(48, 22);
            this.toolDel.Text = "删除";
            this.toolDel.Click += new System.EventHandler(this.toolDel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolExport
            // 
            this.toolExport.Image = ((System.Drawing.Image)(resources.GetObject("toolExport.Image")));
            this.toolExport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolExport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExport.Name = "toolExport";
            this.toolExport.Size = new System.Drawing.Size(49, 22);
            this.toolExport.Text = "导出";
            this.toolExport.Click += new System.EventHandler(this.toolExport_Click);
            // 
            // toolImport
            // 
            this.toolImport.Image = ((System.Drawing.Image)(resources.GetObject("toolImport.Image")));
            this.toolImport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolImport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolImport.Name = "toolImport";
            this.toolImport.Size = new System.Drawing.Size(43, 22);
            this.toolImport.Text = "导入";
            this.toolImport.Click += new System.EventHandler(this.toolImport_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolExit
            // 
            this.toolExit.Image = ((System.Drawing.Image)(resources.GetObject("toolExit.Image")));
            this.toolExit.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExit.Name = "toolExit";
            this.toolExit.Size = new System.Drawing.Size(51, 22);
            this.toolExit.Text = "退出";
            this.toolExit.Click += new System.EventHandler(this.toolExit_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 414);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(813, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // listTemplate
            // 
            this.listTemplate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listTemplate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listTemplate.FullRowSelect = true;
            this.listTemplate.GridLines = true;
            this.listTemplate.Location = new System.Drawing.Point(0, 25);
            this.listTemplate.Name = "listTemplate";
            this.listTemplate.Size = new System.Drawing.Size(813, 389);
            this.listTemplate.SmallImageList = this.imageList1;
            this.listTemplate.TabIndex = 2;
            this.listTemplate.UseCompatibleStateImageBehavior = false;
            this.listTemplate.View = System.Windows.Forms.View.Details;
            this.listTemplate.DoubleClick += new System.EventHandler(this.listTemplate_DoubleClick);
            this.listTemplate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listTemplate_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "模板名称";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "类别";
            this.columnHeader2.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "备注";
            this.columnHeader3.Width = 300;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Node.gif");
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // toolUpgrade
            // 
            this.toolUpgrade.Image = ((System.Drawing.Image)(resources.GetObject("toolUpgrade.Image")));
            this.toolUpgrade.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolUpgrade.Name = "toolUpgrade";
            this.toolUpgrade.Size = new System.Drawing.Size(99, 22);
            this.toolUpgrade.Text = "升级发布模板";
            this.toolUpgrade.Click += new System.EventHandler(this.toolUpgrade_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(813, 436);
            this.Controls.Add(this.listTemplate);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络矿工数据采集软件——发布模版管理工具";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ListView listTemplate;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ToolStripButton toolEdit;
        private System.Windows.Forms.ToolStripButton toolDel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolExport;
        private System.Windows.Forms.ToolStripButton toolImport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton toolExit;
        private System.Windows.Forms.ToolStripSplitButton toolAdd;
        private System.Windows.Forms.ToolStripMenuItem toolMenuAddWeb;
        private System.Windows.Forms.ToolStripMenuItem toolMenuAddDb;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripButton toolUpgrade;
    }
}