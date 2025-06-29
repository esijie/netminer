namespace NetMiner.WebEngine
{
    partial class frmBrowser
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
            this._ActiveWebBrowser = new NetMiner.WebEngine.cGeckoBrowser(this.components);
            this.SuspendLayout();
            // 
            // _ActiveWebBrowser
            // 
            this._ActiveWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ActiveWebBrowser.FrameEventsPropagateToMainWindow = false;
            this._ActiveWebBrowser.isDocumentCompleted = false;
            this._ActiveWebBrowser.Location = new System.Drawing.Point(0, 0);
            this._ActiveWebBrowser.Name = "_ActiveWebBrowser";
            this._ActiveWebBrowser.Size = new System.Drawing.Size(284, 261);
            this._ActiveWebBrowser.TabIndex = 1;
            this._ActiveWebBrowser.UseHttpActivityObserver = false;
            this._ActiveWebBrowser.CreateWindow += new System.EventHandler<Gecko.GeckoCreateWindowEventArgs>(this._ActiveWebBrowser_CreateWindow);
            this._ActiveWebBrowser.CreateWindow2 += new System.EventHandler<Gecko.GeckoCreateWindow2EventArgs>(this._ActiveWebBrowser_CreateWindow2);
            // 
            // frmBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this._ActiveWebBrowser);
            this.Name = "frmBrowser";
            this.Text = "frmBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        internal cGeckoBrowser _ActiveWebBrowser;
    }
}