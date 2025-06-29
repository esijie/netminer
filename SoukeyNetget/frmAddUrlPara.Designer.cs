namespace MinerSpider
{
    partial class frmAddUrlPara
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAddUrlPara));
            this.label1 = new System.Windows.Forms.Label();
            this.txtStart = new System.Windows.Forms.TextBox();
            this.txtEnd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtStep = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdApply = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fromComeUrl = new System.Windows.Forms.ToolStripMenuItem();
            this.fromGData = new System.Windows.Forms.ToolStripMenuItem();
            this.取页面表单值FormValueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMaxCount = new System.Windows.Forms.TextBox();
            this.txtPageSize = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "起始值：";
            // 
            // txtStart
            // 
            this.txtStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStart.Location = new System.Drawing.Point(91, 19);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(291, 21);
            this.txtStart.TabIndex = 1;
            // 
            // txtEnd
            // 
            this.txtEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtEnd.Location = new System.Drawing.Point(91, 46);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(291, 21);
            this.txtEnd.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "结束值：";
            // 
            // txtStep
            // 
            this.txtStep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStep.Location = new System.Drawing.Point(91, 128);
            this.txtStep.Name = "txtStep";
            this.txtStep.Size = new System.Drawing.Size(291, 21);
            this.txtStep.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "每次递增：";
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdCancel.Image = ((System.Drawing.Image)(resources.GetObject("cmdCancel.Image")));
            this.cmdCancel.ImageAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(317, 156);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(65, 22);
            this.cmdCancel.TabIndex = 21;
            this.cmdCancel.Text = "取 消";
            this.cmdCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdApply
            // 
            this.cmdApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdApply.Image = ((System.Drawing.Image)(resources.GetObject("cmdApply.Image")));
            this.cmdApply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdApply.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdApply.Location = new System.Drawing.Point(244, 156);
            this.cmdApply.Name = "cmdApply";
            this.cmdApply.Size = new System.Drawing.Size(65, 22);
            this.cmdApply.TabIndex = 20;
            this.cmdApply.Text = "确 定";
            this.cmdApply.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdApply.UseVisualStyleBackColor = true;
            this.cmdApply.Click += new System.EventHandler(this.cmdApply_Click);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(345, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 21);
            this.button1.TabIndex = 22;
            this.button1.Text = "参数";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromComeUrl,
            this.fromGData,
            this.取页面表单值FormValueToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(330, 92);
            this.contextMenuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextMenuStrip1_ItemClicked);
            // 
            // fromComeUrl
            // 
            this.fromComeUrl.Name = "fromComeUrl";
            this.fromComeUrl.Size = new System.Drawing.Size(329, 22);
            this.fromComeUrl.Text = "从来路页面获取数据{FromValue:在此输入正则}";
            // 
            // fromGData
            // 
            this.fromGData.Name = "fromGData";
            this.fromGData.Size = new System.Drawing.Size(329, 22);
            this.fromGData.Text = "从采集数据中获取{在此输入采集规则名称}";
            // 
            // 取页面表单值FormValueToolStripMenuItem
            // 
            this.取页面表单值FormValueToolStripMenuItem.Name = "取页面表单值FormValueToolStripMenuItem";
            this.取页面表单值FormValueToolStripMenuItem.Size = new System.Drawing.Size(329, 22);
            this.取页面表单值FormValueToolStripMenuItem.Text = "取页面表单值{FormValue:在此输入参数名称}";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "最大记录数：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "每页记录数：";
            // 
            // txtMaxCount
            // 
            this.txtMaxCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxCount.Enabled = false;
            this.txtMaxCount.Location = new System.Drawing.Point(91, 73);
            this.txtMaxCount.Name = "txtMaxCount";
            this.txtMaxCount.Size = new System.Drawing.Size(258, 21);
            this.txtMaxCount.TabIndex = 26;
            this.txtMaxCount.TextChanged += new System.EventHandler(this.txtMaxCount_TextChanged);
            this.txtMaxCount.Enter += new System.EventHandler(this.txtMaxCount_Enter);
            // 
            // txtPageSize
            // 
            this.txtPageSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPageSize.Enabled = false;
            this.txtPageSize.Location = new System.Drawing.Point(91, 100);
            this.txtPageSize.Name = "txtPageSize";
            this.txtPageSize.Size = new System.Drawing.Size(258, 21);
            this.txtPageSize.TabIndex = 27;
            this.txtPageSize.TextChanged += new System.EventHandler(this.txtPageSize_TextChanged);
            this.txtPageSize.Enter += new System.EventHandler(this.txtPageSize_Enter);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(345, 100);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(39, 21);
            this.button2.TabIndex = 28;
            this.button2.Text = "参数";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(11, 161);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(228, 12);
            this.label6.TabIndex = 29;
            this.label6.Text = "最大页码的计算，适合用于Ajax请求。";
            // 
            // frmAddUrlPara
            // 
            this.AcceptButton = this.cmdApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(402, 186);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtPageSize);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txtMaxCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtEnd);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdApply);
            this.Controls.Add(this.txtStep);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtStart);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddUrlPara";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "配置网址参数";
            this.Load += new System.EventHandler(this.frmAddUrlPara_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStart;
        private System.Windows.Forms.TextBox txtEnd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtStep;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdApply;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fromComeUrl;
        private System.Windows.Forms.ToolStripMenuItem fromGData;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxCount;
        private System.Windows.Forms.TextBox txtPageSize;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripMenuItem 取页面表单值FormValueToolStripMenuItem;
    }
}