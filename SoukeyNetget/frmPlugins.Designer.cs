namespace MinerSpider
{
    partial class frmPlugins
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPlugins));
            this.listPlugin = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmdDelPlugins = new System.Windows.Forms.Button();
            this.cmdAddPlugins = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listPlugin
            // 
            this.listPlugin.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listPlugin.FullRowSelect = true;
            this.listPlugin.GridLines = true;
            this.listPlugin.HideSelection = false;
            this.listPlugin.Location = new System.Drawing.Point(12, 41);
            this.listPlugin.Name = "listPlugin";
            this.listPlugin.Size = new System.Drawing.Size(513, 196);
            this.listPlugin.TabIndex = 0;
            this.listPlugin.UseCompatibleStateImageBehavior = false;
            this.listPlugin.View = System.Windows.Forms.View.Details;
            this.listPlugin.DoubleClick += new System.EventHandler(this.listPlugin_DoubleClick);
            this.listPlugin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listPlugin_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "插件名称";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "插件";
            this.columnHeader2.Width = 260;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "类别";
            this.columnHeader3.Width = 120;
            // 
            // cmdDelPlugins
            // 
            this.cmdDelPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdDelPlugins.Image = ((System.Drawing.Image)(resources.GetObject("cmdDelPlugins.Image")));
            this.cmdDelPlugins.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDelPlugins.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDelPlugins.Location = new System.Drawing.Point(401, 12);
            this.cmdDelPlugins.Name = "cmdDelPlugins";
            this.cmdDelPlugins.Size = new System.Drawing.Size(60, 23);
            this.cmdDelPlugins.TabIndex = 3;
            this.cmdDelPlugins.Text = "删 除";
            this.cmdDelPlugins.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdDelPlugins.UseVisualStyleBackColor = true;
            this.cmdDelPlugins.Click += new System.EventHandler(this.cmdDelPlugins_Click);
            // 
            // cmdAddPlugins
            // 
            this.cmdAddPlugins.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdAddPlugins.Image = ((System.Drawing.Image)(resources.GetObject("cmdAddPlugins.Image")));
            this.cmdAddPlugins.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdAddPlugins.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdAddPlugins.Location = new System.Drawing.Point(318, 12);
            this.cmdAddPlugins.Name = "cmdAddPlugins";
            this.cmdAddPlugins.Size = new System.Drawing.Size(77, 23);
            this.cmdAddPlugins.TabIndex = 2;
            this.cmdAddPlugins.Text = "注册插件";
            this.cmdAddPlugins.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdAddPlugins.UseVisualStyleBackColor = true;
            this.cmdAddPlugins.Click += new System.EventHandler(this.cmdAddPlugins_Click);
            // 
            // button1
            // 
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(230, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(83, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "测试插件";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(108, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(116, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "到网站检索插件";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            // 
            // button3
            // 
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(467, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(58, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "退出";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // frmPlugins
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 250);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmdDelPlugins);
            this.Controls.Add(this.cmdAddPlugins);
            this.Controls.Add(this.listPlugin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPlugins";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "插件管理";
            this.Load += new System.EventHandler(this.frmPlugins_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listPlugin;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Button cmdDelPlugins;
        private System.Windows.Forms.Button cmdAddPlugins;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}