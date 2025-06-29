namespace MinerSpider
{
    partial class frmSetWatermark
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSetWatermark));
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.button16 = new System.Windows.Forms.Button();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.comFontType = new System.Windows.Forms.ComboBox();
            this.cmdFontColor = new System.Windows.Forms.Button();
            this.comWatermarkPos = new System.Windows.Forms.ComboBox();
            this.IsFontItalic = new System.Windows.Forms.CheckBox();
            this.IsFontWeight = new System.Windows.Forms.CheckBox();
            this.upFontSize = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.txtWatermark = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upFontSize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.button16);
            this.groupBox12.Controls.Add(this.pictureBox2);
            this.groupBox12.Controls.Add(this.pictureBox1);
            this.groupBox12.Controls.Add(this.comFontType);
            this.groupBox12.Controls.Add(this.cmdFontColor);
            this.groupBox12.Controls.Add(this.comWatermarkPos);
            this.groupBox12.Controls.Add(this.IsFontItalic);
            this.groupBox12.Controls.Add(this.IsFontWeight);
            this.groupBox12.Controls.Add(this.upFontSize);
            this.groupBox12.Controls.Add(this.label19);
            this.groupBox12.Controls.Add(this.txtWatermark);
            this.groupBox12.Controls.Add(this.label18);
            this.groupBox12.Location = new System.Drawing.Point(9, 6);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.Size = new System.Drawing.Size(724, 201);
            this.groupBox12.TabIndex = 2;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "图片水印设置";
            // 
            // button16
            // 
            this.button16.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button16.Location = new System.Drawing.Point(329, 100);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(75, 23);
            this.button16.TabIndex = 11;
            this.button16.Text = ">> 加水印";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox2.Location = new System.Drawing.Point(419, 43);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(299, 145);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(16, 45);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(299, 145);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.DoubleClick += new System.EventHandler(this.pictureBox1_DoubleClick);
            // 
            // comFontType
            // 
            this.comFontType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comFontType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comFontType.FormattingEnabled = true;
            this.comFontType.Location = new System.Drawing.Point(317, 17);
            this.comFontType.Name = "comFontType";
            this.comFontType.Size = new System.Drawing.Size(113, 20);
            this.comFontType.TabIndex = 8;
            // 
            // cmdFontColor
            // 
            this.cmdFontColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmdFontColor.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.cmdFontColor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdFontColor.Location = new System.Drawing.Point(527, 17);
            this.cmdFontColor.Name = "cmdFontColor";
            this.cmdFontColor.Size = new System.Drawing.Size(22, 22);
            this.cmdFontColor.TabIndex = 7;
            this.cmdFontColor.Text = "A";
            this.cmdFontColor.UseVisualStyleBackColor = true;
            this.cmdFontColor.Click += new System.EventHandler(this.cmdFontColor_Click);
            // 
            // comWatermarkPos
            // 
            this.comWatermarkPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comWatermarkPos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comWatermarkPos.FormattingEnabled = true;
            this.comWatermarkPos.Location = new System.Drawing.Point(556, 17);
            this.comWatermarkPos.Name = "comWatermarkPos";
            this.comWatermarkPos.Size = new System.Drawing.Size(158, 20);
            this.comWatermarkPos.TabIndex = 6;
            // 
            // IsFontItalic
            // 
            this.IsFontItalic.Appearance = System.Windows.Forms.Appearance.Button;
            this.IsFontItalic.AutoSize = true;
            this.IsFontItalic.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsFontItalic.Font = new System.Drawing.Font("宋体", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.IsFontItalic.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsFontItalic.Location = new System.Drawing.Point(506, 17);
            this.IsFontItalic.Name = "IsFontItalic";
            this.IsFontItalic.Size = new System.Drawing.Size(22, 22);
            this.IsFontItalic.TabIndex = 5;
            this.IsFontItalic.Text = "I";
            this.IsFontItalic.UseVisualStyleBackColor = true;
            // 
            // IsFontWeight
            // 
            this.IsFontWeight.Appearance = System.Windows.Forms.Appearance.Button;
            this.IsFontWeight.AutoSize = true;
            this.IsFontWeight.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.IsFontWeight.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold);
            this.IsFontWeight.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.IsFontWeight.Location = new System.Drawing.Point(485, 17);
            this.IsFontWeight.Name = "IsFontWeight";
            this.IsFontWeight.Size = new System.Drawing.Size(22, 22);
            this.IsFontWeight.TabIndex = 4;
            this.IsFontWeight.Text = "B";
            this.IsFontWeight.UseVisualStyleBackColor = true;
            // 
            // upFontSize
            // 
            this.upFontSize.Location = new System.Drawing.Point(436, 17);
            this.upFontSize.Maximum = new decimal(new int[] {
            72,
            0,
            0,
            0});
            this.upFontSize.Minimum = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.upFontSize.Name = "upFontSize";
            this.upFontSize.Size = new System.Drawing.Size(45, 21);
            this.upFontSize.TabIndex = 3;
            this.upFontSize.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(274, 20);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(41, 12);
            this.label19.TabIndex = 2;
            this.label19.Text = "字体：";
            // 
            // txtWatermark
            // 
            this.txtWatermark.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWatermark.Location = new System.Drawing.Point(98, 17);
            this.txtWatermark.Name = "txtWatermark";
            this.txtWatermark.Size = new System.Drawing.Size(160, 21);
            this.txtWatermark.TabIndex = 1;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(27, 21);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(65, 12);
            this.label18.TabIndex = 0;
            this.label18.Text = "水印文字：";
            // 
            // button4
            // 
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button4.Location = new System.Drawing.Point(668, 224);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(65, 21);
            this.button4.TabIndex = 11;
            this.button4.Text = "取 消";
            this.button4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(597, 224);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 21);
            this.button3.TabIndex = 10;
            this.button3.Text = "确 定";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // frmSetWatermark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(748, 258);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox12);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetWatermark";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设置水印格式";
            this.Load += new System.EventHandler(this.frmSetWatermark_Load);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upFontSize)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox12;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comFontType;
        private System.Windows.Forms.Button cmdFontColor;
        private System.Windows.Forms.ComboBox comWatermarkPos;
        private System.Windows.Forms.CheckBox IsFontItalic;
        private System.Windows.Forms.CheckBox IsFontWeight;
        private System.Windows.Forms.NumericUpDown upFontSize;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtWatermark;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}