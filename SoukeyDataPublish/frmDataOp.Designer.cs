﻿namespace SoukeyDataPublish
{
    partial class frmDataOp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDataOp));
            this.myData = new SoukeyDataPublish.CustomControl.uconMyDataOp();
            this.SuspendLayout();
            // 
            // myData
            // 
            this.myData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myData.Location = new System.Drawing.Point(0, 0);
            this.myData.Name = "myData";
            this.myData.Size = new System.Drawing.Size(874, 509);
            this.myData.TabIndex = 0;
            // 
            // frmDataOp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 509);
            this.Controls.Add(this.myData);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDataOp";
            this.Load += new System.EventHandler(this.frmDataOp_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private SoukeyDataPublish.CustomControl.uconMyDataOp myData;
    }
}