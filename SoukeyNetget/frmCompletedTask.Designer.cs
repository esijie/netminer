namespace MinerSpider
{
    partial class frmCompletedTask
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCompletedTask));
            this.dataTask = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.rmenuEditData = new System.Windows.Forms.ToolStripMenuItem();
            this.rmenuClearCompleted = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator15 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuExportCSV = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuExportExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.rMenuRefer = new System.Windows.Forms.ToolStripMenuItem();
            this.rmmenuDelTask = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.rMenuExportWord = new System.Windows.Forms.ToolStripMenuItem();
            this.rMenuExportTxt = new System.Windows.Forms.ToolStripMenuItem();
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
            this.dataTask.Size = new System.Drawing.Size(612, 297);
            this.dataTask.StandardTab = true;
            this.dataTask.TabIndex = 2;
            this.dataTask.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataTask_CellDoubleClick);
            this.dataTask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataTask_KeyDown);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rmenuEditData,
            this.rmenuClearCompleted,
            this.toolStripSeparator15,
            this.rMenuExportTxt,
            this.rMenuExportWord,
            this.rMenuExportCSV,
            this.rMenuExportExcel,
            this.toolStripSeparator1,
            this.rMenuRefer,
            this.rmmenuDelTask});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(245, 214);
            this.contextMenuStrip2.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip2_Opening);
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
            // 
            // toolStripSeparator15
            // 
            this.toolStripSeparator15.Name = "toolStripSeparator15";
            this.toolStripSeparator15.Size = new System.Drawing.Size(241, 6);
            // 
            // rMenuExportCSV
            // 
            this.rMenuExportCSV.Name = "rMenuExportCSV";
            this.rMenuExportCSV.Size = new System.Drawing.Size(244, 22);
            this.rMenuExportCSV.Text = "导出CSV";
            this.rMenuExportCSV.Click += new System.EventHandler(this.rMenuExportCSV_Click);
            // 
            // rMenuExportExcel
            // 
            this.rMenuExportExcel.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportExcel.Image")));
            this.rMenuExportExcel.Name = "rMenuExportExcel";
            this.rMenuExportExcel.Size = new System.Drawing.Size(244, 22);
            this.rMenuExportExcel.Text = "导出Excel";
            this.rMenuExportExcel.Click += new System.EventHandler(this.rMenuExportExcel_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(241, 6);
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
            // rMenuExportWord
            // 
            this.rMenuExportWord.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportWord.Image")));
            this.rMenuExportWord.Name = "rMenuExportWord";
            this.rMenuExportWord.Size = new System.Drawing.Size(244, 22);
            this.rMenuExportWord.Text = "导出Word文档";
            this.rMenuExportWord.Click += new System.EventHandler(this.rMenuExportWord_Click);
            // 
            // rMenuExportTxt
            // 
            this.rMenuExportTxt.Image = ((System.Drawing.Image)(resources.GetObject("rMenuExportTxt.Image")));
            this.rMenuExportTxt.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.rMenuExportTxt.Name = "rMenuExportTxt";
            this.rMenuExportTxt.Size = new System.Drawing.Size(244, 22);
            this.rMenuExportTxt.Text = "导出文本";
            this.rMenuExportTxt.Click += new System.EventHandler(this.rMenuExportTxt_Click);
            // 
            // frmCompletedTask
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(612, 297);
            this.Controls.Add(this.dataTask);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmCompletedTask";
            this.Text = "采集历史";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCompletedTask_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataTask)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView dataTask;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem rmenuEditData;
        private System.Windows.Forms.ToolStripMenuItem rmenuClearCompleted;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator15;
        private System.Windows.Forms.ToolStripMenuItem rMenuRefer;
        private System.Windows.Forms.ToolStripMenuItem rmmenuDelTask;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportCSV;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportExcel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportWord;
        private System.Windows.Forms.ToolStripMenuItem rMenuExportTxt;
    }
}