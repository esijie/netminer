namespace SoukeyDataPublish
{
    partial class frmToolbox
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("采集完成的数据");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Access数据库");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("MSSqlServer数据库", 8, 8);
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("MySql数据库", 9, 9);
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("导入外部数据", new System.Windows.Forms.TreeNode[] {
            treeNode2,
            treeNode3,
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("雷达监控数据", 10, 10);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmToolbox));
            this.treeMenu = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuDelData = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuReferData = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMenu
            // 
            this.treeMenu.ContextMenuStrip = this.contextMenuStrip1;
            this.treeMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMenu.HideSelection = false;
            this.treeMenu.ImageIndex = 0;
            this.treeMenu.ImageList = this.imageList1;
            this.treeMenu.Location = new System.Drawing.Point(0, 0);
            this.treeMenu.Name = "treeMenu";
            treeNode1.ImageKey = "logo16.png";
            treeNode1.Name = "SoukeyData";
            treeNode1.SelectedImageKey = "logo16.png";
            treeNode1.Text = "采集完成的数据";
            treeNode2.ImageKey = "A02.gif";
            treeNode2.Name = "AccessData";
            treeNode2.SelectedImageKey = "A02.gif";
            treeNode2.Text = "Access数据库";
            treeNode3.ImageIndex = 8;
            treeNode3.Name = "SqlServerData";
            treeNode3.SelectedImageIndex = 8;
            treeNode3.Text = "MSSqlServer数据库";
            treeNode4.ImageIndex = 9;
            treeNode4.Name = "MySqlData";
            treeNode4.SelectedImageIndex = 9;
            treeNode4.Text = "MySql数据库";
            treeNode5.ImageKey = "A44.gif";
            treeNode5.Name = "OtherData";
            treeNode5.SelectedImageKey = "A44.gif";
            treeNode5.Text = "导入外部数据";
            treeNode6.ImageIndex = 10;
            treeNode6.Name = "nodRadar";
            treeNode6.SelectedImageIndex = 10;
            treeNode6.Text = "雷达监控数据";
            this.treeMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode5,
            treeNode6});
            this.treeMenu.SelectedImageIndex = 0;
            this.treeMenu.Size = new System.Drawing.Size(208, 392);
            this.treeMenu.TabIndex = 5;
            this.treeMenu.DoubleClick += new System.EventHandler(this.treeMenu_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuDelData,
            this.rmenuReferData});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // rmenuDelData
            // 
            this.rmenuDelData.Image = ((System.Drawing.Image)(resources.GetObject("rmenuDelData.Image")));
            this.rmenuDelData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmenuDelData.Name = "rmenuDelData";
            this.rmenuDelData.Size = new System.Drawing.Size(136, 22);
            this.rmenuDelData.Text = "删除此数据";
            this.rmenuDelData.Click += new System.EventHandler(this.rmenuDelData_Click);
            // 
            // rmenuReferData
            // 
            this.rmenuReferData.Image = ((System.Drawing.Image)(resources.GetObject("rmenuReferData.Image")));
            this.rmenuReferData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rmenuReferData.Name = "rmenuReferData";
            this.rmenuReferData.Size = new System.Drawing.Size(136, 22);
            this.rmenuReferData.Text = "刷新";
            this.rmenuReferData.Click += new System.EventHandler(this.rmenuReferData_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "A42.gif");
            this.imageList1.Images.SetKeyName(1, "A43.gif");
            this.imageList1.Images.SetKeyName(2, "logo.gif");
            this.imageList1.Images.SetKeyName(3, "A44.gif");
            this.imageList1.Images.SetKeyName(4, "data");
            this.imageList1.Images.SetKeyName(5, "task");
            this.imageList1.Images.SetKeyName(6, "A02.gif");
            this.imageList1.Images.SetKeyName(7, "A17.gif");
            this.imageList1.Images.SetKeyName(8, "A46.gif");
            this.imageList1.Images.SetKeyName(9, "A46.ico");
            this.imageList1.Images.SetKeyName(10, "radar.gif");
            this.imageList1.Images.SetKeyName(11, "A24.gif");
            this.imageList1.Images.SetKeyName(12, "logo16.png");
            // 
            // frmToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(208, 392);
            this.Controls.Add(this.treeMenu);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmToolbox";
            this.Text = "数据中心";
            this.Load += new System.EventHandler(this.frmToolbox_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeMenu;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem rmenuDelData;
        private System.Windows.Forms.ToolStripMenuItem rmenuReferData;
        private System.Windows.Forms.ImageList imageList1;
    }
}