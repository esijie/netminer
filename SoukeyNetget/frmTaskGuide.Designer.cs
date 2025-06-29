namespace MinerSpider
{
    partial class frmTaskGuide
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTaskGuide));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.udThread = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.udGIntervalTime1 = new System.Windows.Forms.NumericUpDown();
            this.label37 = new System.Windows.Forms.Label();
            this.udGIntervalTime = new System.Windows.Forms.NumericUpDown();
            this.label31 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udThread)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGIntervalTime1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGIntervalTime)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(8, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(243, 384);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.udGIntervalTime1);
            this.panel2.Controls.Add(this.label37);
            this.panel2.Controls.Add(this.udGIntervalTime);
            this.panel2.Controls.Add(this.label31);
            this.panel2.Controls.Add(this.udThread);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.comboBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(260, 6);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(557, 384);
            this.panel2.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(0, 402);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(967, 8);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(23, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(286, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "网络矿工采集任务配置向导";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(27, 81);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(367, 21);
            this.textBox1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(25, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "采集任务名称：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(25, 123);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(122, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "采集任务所属分类：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(27, 142);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(367, 20);
            this.comboBox1.TabIndex = 4;
            // 
            // udThread
            // 
            this.udThread.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.udThread.Location = new System.Drawing.Point(27, 204);
            this.udThread.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.udThread.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udThread.Name = "udThread";
            this.udThread.Size = new System.Drawing.Size(124, 21);
            this.udThread.TabIndex = 24;
            this.udThread.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.Navy;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(25, 184);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(70, 12);
            this.label11.TabIndex = 25;
            this.label11.Text = "采集线程：";
            // 
            // udGIntervalTime1
            // 
            this.udGIntervalTime1.DecimalPlaces = 1;
            this.udGIntervalTime1.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.udGIntervalTime1.Location = new System.Drawing.Point(102, 271);
            this.udGIntervalTime1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udGIntervalTime1.Name = "udGIntervalTime1";
            this.udGIntervalTime1.Size = new System.Drawing.Size(52, 21);
            this.udGIntervalTime1.TabIndex = 28;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label37.Location = new System.Drawing.Point(83, 274);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(17, 12);
            this.label37.TabIndex = 30;
            this.label37.Text = "—";
            // 
            // udGIntervalTime
            // 
            this.udGIntervalTime.DecimalPlaces = 1;
            this.udGIntervalTime.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.udGIntervalTime.Location = new System.Drawing.Point(27, 271);
            this.udGIntervalTime.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udGIntervalTime.Name = "udGIntervalTime";
            this.udGIntervalTime.Size = new System.Drawing.Size(52, 21);
            this.udGIntervalTime.TabIndex = 27;
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.label31.ForeColor = System.Drawing.Color.Navy;
            this.label31.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label31.Location = new System.Drawing.Point(25, 245);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(70, 12);
            this.label31.TabIndex = 29;
            this.label31.Text = "采集间隔：";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label4);
            this.panel3.Location = new System.Drawing.Point(260, 6);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(557, 384);
            this.panel3.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "label4";
            // 
            // frmTaskGuide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 459);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTaskGuide";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "采集任务配置向导";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udThread)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGIntervalTime1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udGIntervalTime)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown udThread;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown udGIntervalTime1;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.NumericUpDown udGIntervalTime;
        private System.Windows.Forms.Label label31;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
    }
}