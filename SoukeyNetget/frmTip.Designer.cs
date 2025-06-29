namespace MinerSpider
{
    partial class frmTip
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTip));
            this.txtHelp = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtFindKey = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.cmdNext = new System.Windows.Forms.Button();
            this.cmdPre = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtHelp
            // 
            this.txtHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHelp.Location = new System.Drawing.Point(12, 36);
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.Size = new System.Drawing.Size(534, 217);
            this.txtHelp.TabIndex = 0;
            this.txtHelp.Text = "";
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(486, 11);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(60, 19);
            this.button1.TabIndex = 24;
            this.button1.Text = "查 询";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // txtFindKey
            // 
            this.txtFindKey.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFindKey.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtFindKey.Location = new System.Drawing.Point(12, 11);
            this.txtFindKey.Name = "txtFindKey";
            this.txtFindKey.Size = new System.Drawing.Size(478, 21);
            this.txtFindKey.TabIndex = 23;
            this.txtFindKey.Text = "请输入查询问题的关键字";
            this.txtFindKey.Enter += new System.EventHandler(this.txtFindKey_Enter);
            this.txtFindKey.Leave += new System.EventHandler(this.txtFindKey_Leave);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(12, 258);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(183, 21);
            this.button2.TabIndex = 25;
            this.button2.Text = "从中心数据库更新帮助信息";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(481, 258);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(65, 21);
            this.button3.TabIndex = 26;
            this.button3.Text = "关  闭";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            // 
            // cmdNext
            // 
            this.cmdNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdNext.Image = ((System.Drawing.Image)(resources.GetObject("cmdNext.Image")));
            this.cmdNext.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdNext.Location = new System.Drawing.Point(410, 258);
            this.cmdNext.Name = "cmdNext";
            this.cmdNext.Size = new System.Drawing.Size(65, 22);
            this.cmdNext.TabIndex = 28;
            this.cmdNext.Text = "下一条";
            this.cmdNext.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdNext.UseVisualStyleBackColor = true;
            // 
            // cmdPre
            // 
            this.cmdPre.Enabled = false;
            this.cmdPre.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPre.Image = ((System.Drawing.Image)(resources.GetObject("cmdPre.Image")));
            this.cmdPre.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdPre.Location = new System.Drawing.Point(333, 258);
            this.cmdPre.Name = "cmdPre";
            this.cmdPre.Size = new System.Drawing.Size(65, 22);
            this.cmdPre.TabIndex = 27;
            this.cmdPre.Text = "上一条";
            this.cmdPre.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdPre.UseVisualStyleBackColor = true;
            // 
            // frmTip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(558, 289);
            this.Controls.Add(this.cmdNext);
            this.Controls.Add(this.cmdPre);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtFindKey);
            this.Controls.Add(this.txtHelp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTip";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网络矿工动态帮助";
            this.Load += new System.EventHandler(this.frmTip_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtHelp;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtFindKey;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button cmdNext;
        private System.Windows.Forms.Button cmdPre;
    }
}