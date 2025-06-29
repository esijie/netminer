namespace MinerSpider
{
    partial class frmRegex
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRegex));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.rtbRegex = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.IsRightToLeft = new System.Windows.Forms.CheckBox();
            this.IsMultiline = new System.Windows.Forms.CheckBox();
            this.IsCulture = new System.Windows.Forms.CheckBox();
            this.IsCase = new System.Windows.Forms.CheckBox();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.label5 = new System.Windows.Forms.Label();
            this.IsWordwrap = new System.Windows.Forms.CheckBox();
            this.rtbSource = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.labInfo = new System.Windows.Forms.Label();
            this.IsFindCase = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtFindStr = new System.Windows.Forms.TextBox();
            this.cmdFindNext = new System.Windows.Forms.Button();
            this.cmdFindPre = new System.Windows.Forms.Button();
            this.lstResult = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label3 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolOptional = new System.Windows.Forms.ToolStripMenuItem();
            this.toolExcute = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolIsFind = new System.Windows.Forms.ToolStripMenuItem();
            this.toolExit = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Location = new System.Drawing.Point(0, 413);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(847, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 25);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtbRegex);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.IsRightToLeft);
            this.splitContainer1.Panel1.Controls.Add(this.IsMultiline);
            this.splitContainer1.Panel1.Controls.Add(this.IsCulture);
            this.splitContainer1.Panel1.Controls.Add(this.IsCase);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer1.Size = new System.Drawing.Size(847, 388);
            this.splitContainer1.SplitterDistance = 88;
            this.splitContainer1.TabIndex = 2;
            // 
            // rtbRegex
            // 
            this.rtbRegex.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbRegex.Location = new System.Drawing.Point(86, 8);
            this.rtbRegex.Name = "rtbRegex";
            this.rtbRegex.Size = new System.Drawing.Size(746, 54);
            this.rtbRegex.TabIndex = 6;
            this.rtbRegex.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "正则表达式：";
            // 
            // IsRightToLeft
            // 
            this.IsRightToLeft.AutoSize = true;
            this.IsRightToLeft.Location = new System.Drawing.Point(338, 66);
            this.IsRightToLeft.Name = "IsRightToLeft";
            this.IsRightToLeft.Size = new System.Drawing.Size(72, 16);
            this.IsRightToLeft.TabIndex = 4;
            this.IsRightToLeft.Text = "从右至左";
            this.IsRightToLeft.UseVisualStyleBackColor = true;
            // 
            // IsMultiline
            // 
            this.IsMultiline.AutoSize = true;
            this.IsMultiline.Location = new System.Drawing.Point(282, 66);
            this.IsMultiline.Name = "IsMultiline";
            this.IsMultiline.Size = new System.Drawing.Size(48, 16);
            this.IsMultiline.TabIndex = 3;
            this.IsMultiline.Text = "多行";
            this.IsMultiline.UseVisualStyleBackColor = true;
            // 
            // IsCulture
            // 
            this.IsCulture.AutoSize = true;
            this.IsCulture.Location = new System.Drawing.Point(178, 66);
            this.IsCulture.Name = "IsCulture";
            this.IsCulture.Size = new System.Drawing.Size(96, 16);
            this.IsCulture.TabIndex = 2;
            this.IsCulture.Text = "忽略区域差异";
            this.IsCulture.UseVisualStyleBackColor = true;
            // 
            // IsCase
            // 
            this.IsCase.AutoSize = true;
            this.IsCase.Location = new System.Drawing.Point(86, 66);
            this.IsCase.Name = "IsCase";
            this.IsCase.Size = new System.Drawing.Size(84, 16);
            this.IsCase.TabIndex = 1;
            this.IsCase.Text = "忽略大小写";
            this.IsCase.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.lstResult);
            this.splitContainer3.Panel2.Controls.Add(this.label3);
            this.splitContainer3.Size = new System.Drawing.Size(845, 294);
            this.splitContainer3.SplitterDistance = 150;
            this.splitContainer3.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.label5);
            this.splitContainer2.Panel1.Controls.Add(this.IsWordwrap);
            this.splitContainer2.Panel1.Controls.Add(this.rtbSource);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.labInfo);
            this.splitContainer2.Panel2.Controls.Add(this.IsFindCase);
            this.splitContainer2.Panel2.Controls.Add(this.label4);
            this.splitContainer2.Panel2.Controls.Add(this.txtFindStr);
            this.splitContainer2.Panel2.Controls.Add(this.cmdFindNext);
            this.splitContainer2.Panel2.Controls.Add(this.cmdFindPre);
            this.splitContainer2.Panel2MinSize = 30;
            this.splitContainer2.Size = new System.Drawing.Size(841, 146);
            this.splitContainer2.SplitterDistance = 114;
            this.splitContainer2.SplitterWidth = 2;
            this.splitContainer2.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(190, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(485, 12);
            this.label5.TabIndex = 16;
            this.label5.Text = "系统在进行采集数据匹配时会自动去除回车换行符号，如果是测试采集规则，建议选中此项";
            // 
            // IsWordwrap
            // 
            this.IsWordwrap.AutoSize = true;
            this.IsWordwrap.Location = new System.Drawing.Point(86, 6);
            this.IsWordwrap.Name = "IsWordwrap";
            this.IsWordwrap.Size = new System.Drawing.Size(96, 16);
            this.IsWordwrap.TabIndex = 11;
            this.IsWordwrap.Text = "去除回车换行";
            this.IsWordwrap.UseVisualStyleBackColor = true;
            this.IsWordwrap.CheckedChanged += new System.EventHandler(this.IsWordwrap_CheckedChanged);
            // 
            // rtbSource
            // 
            this.rtbSource.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbSource.AutoWordSelection = true;
            this.rtbSource.HideSelection = false;
            this.rtbSource.Location = new System.Drawing.Point(86, 23);
            this.rtbSource.Name = "rtbSource";
            this.rtbSource.Size = new System.Drawing.Size(746, 92);
            this.rtbSource.TabIndex = 10;
            this.rtbSource.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "源文件：";
            // 
            // labInfo
            // 
            this.labInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labInfo.AutoSize = true;
            this.labInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.labInfo.Location = new System.Drawing.Point(555, -15);
            this.labInfo.Name = "labInfo";
            this.labInfo.Size = new System.Drawing.Size(0, 12);
            this.labInfo.TabIndex = 18;
            // 
            // IsFindCase
            // 
            this.IsFindCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.IsFindCase.AutoSize = true;
            this.IsFindCase.Location = new System.Drawing.Point(463, 3);
            this.IsFindCase.Name = "IsFindCase";
            this.IsFindCase.Size = new System.Drawing.Size(84, 16);
            this.IsFindCase.TabIndex = 17;
            this.IsFindCase.Text = "忽略大小写";
            this.IsFindCase.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "查找：";
            // 
            // txtFindStr
            // 
            this.txtFindStr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtFindStr.Location = new System.Drawing.Point(84, 1);
            this.txtFindStr.Name = "txtFindStr";
            this.txtFindStr.Size = new System.Drawing.Size(231, 21);
            this.txtFindStr.TabIndex = 13;
            // 
            // cmdFindNext
            // 
            this.cmdFindNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdFindNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdFindNext.Image = ((System.Drawing.Image)(resources.GetObject("cmdFindNext.Image")));
            this.cmdFindNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdFindNext.Location = new System.Drawing.Point(392, 0);
            this.cmdFindNext.Name = "cmdFindNext";
            this.cmdFindNext.Size = new System.Drawing.Size(65, 21);
            this.cmdFindNext.TabIndex = 15;
            this.cmdFindNext.Text = "下一个";
            this.cmdFindNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdFindNext.UseVisualStyleBackColor = true;
            this.cmdFindNext.Click += new System.EventHandler(this.cmdFindNext_Click);
            // 
            // cmdFindPre
            // 
            this.cmdFindPre.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdFindPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdFindPre.Image = ((System.Drawing.Image)(resources.GetObject("cmdFindPre.Image")));
            this.cmdFindPre.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdFindPre.Location = new System.Drawing.Point(321, 0);
            this.cmdFindPre.Name = "cmdFindPre";
            this.cmdFindPre.Size = new System.Drawing.Size(65, 21);
            this.cmdFindPre.TabIndex = 14;
            this.cmdFindPre.Text = "上一个";
            this.cmdFindPre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdFindPre.UseVisualStyleBackColor = true;
            this.cmdFindPre.Click += new System.EventHandler(this.cmdFindPre_Click);
            // 
            // lstResult
            // 
            this.lstResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstResult.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lstResult.FullRowSelect = true;
            this.lstResult.Location = new System.Drawing.Point(84, 13);
            this.lstResult.MultiSelect = false;
            this.lstResult.Name = "lstResult";
            this.lstResult.Size = new System.Drawing.Size(746, 121);
            this.lstResult.TabIndex = 1;
            this.lstResult.UseCompatibleStateImageBehavior = false;
            this.lstResult.View = System.Windows.Forms.View.Details;
            this.lstResult.SelectedIndexChanged += new System.EventHandler(this.lstResult_SelectedIndexChanged);
            this.lstResult.Click += new System.EventHandler(this.lstResult_Click);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "匹配结果";
            this.columnHeader1.Width = 600;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "位置";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "长度";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "匹配结果：";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolOptional,
            this.toolExit});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(847, 25);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolOptional
            // 
            this.toolOptional.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolExcute,
            this.toolStripSeparator1,
            this.toolIsFind});
            this.toolOptional.Name = "toolOptional";
            this.toolOptional.Size = new System.Drawing.Size(62, 21);
            this.toolOptional.Text = "操作(&O)";
            // 
            // toolExcute
            // 
            this.toolExcute.Name = "toolExcute";
            this.toolExcute.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.toolExcute.Size = new System.Drawing.Size(143, 22);
            this.toolExcute.Text = "执行";
            this.toolExcute.Click += new System.EventHandler(this.toolExcute_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(140, 6);
            // 
            // toolIsFind
            // 
            this.toolIsFind.Name = "toolIsFind";
            this.toolIsFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.toolIsFind.Size = new System.Drawing.Size(143, 22);
            this.toolIsFind.Text = "查找";
            this.toolIsFind.Click += new System.EventHandler(this.toolIsFind_Click);
            // 
            // toolExit
            // 
            this.toolExit.Name = "toolExit";
            this.toolExit.Size = new System.Drawing.Size(59, 21);
            this.toolExit.Text = "退出(&E)";
            this.toolExit.Click += new System.EventHandler(this.toolExit_Click);
            // 
            // frmRegex
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 435);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmRegex";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "正则表达式分析器 ";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRegex_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRegex_FormClosed);
            this.Load += new System.EventHandler(this.frmRegex_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.Panel2.PerformLayout();
            this.splitContainer3.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            this.splitContainer2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox IsMultiline;
        private System.Windows.Forms.CheckBox IsCulture;
        private System.Windows.Forms.CheckBox IsCase;
        private System.Windows.Forms.CheckBox IsRightToLeft;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lstResult;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        internal System.Windows.Forms.RichTextBox rtbRegex;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Label label5;
        internal System.Windows.Forms.RichTextBox rtbSource;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox IsFindCase;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtFindStr;
        private System.Windows.Forms.Button cmdFindNext;
        private System.Windows.Forms.Button cmdFindPre;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolOptional;
        private System.Windows.Forms.ToolStripMenuItem toolExcute;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolIsFind;
        private System.Windows.Forms.ToolStripMenuItem toolExit;
        private System.Windows.Forms.Label labInfo;
        public System.Windows.Forms.CheckBox IsWordwrap;
    }
}