using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace NetMiner.WebEngine
{
    public partial class frmBrowser : Form
    {
        public frmBrowser()
        {
            InitializeComponent();

            IniBrowser();
        }

        private void IniBrowser()
        {
            if (!Gecko.Xpcom.IsInitialized)
                Gecko.Xpcom.Initialize(Program.g_xulPath);
            this._ActiveWebBrowser.Navigate("about:blank");
        }

        private void _ActiveWebBrowser_CreateWindow(object sender, Gecko.GeckoCreateWindowEventArgs e)
        {
            e.Cancel = true;
        }

        private void _ActiveWebBrowser_CreateWindow2(object sender, Gecko.GeckoCreateWindow2EventArgs e)
        {
            e.Cancel = true;
        }

        public string GetUrl(string url)
        {
            this._ActiveWebBrowser.isDocumentCompleted = false;

            this._ActiveWebBrowser.Navigate(url);

            while (!this._ActiveWebBrowser.isDocumentCompleted)
            {
                Thread.Sleep(50);
                Application.DoEvents();
            }

            return this._ActiveWebBrowser.Document.Body.OuterHtml;
        }
    }
}
