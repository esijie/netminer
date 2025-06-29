using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gecko;
using Gecko.Events;

namespace NetMiner.WebEngine
{
    public partial class cGeckoBrowser : Gecko.GeckoWebBrowser
    {
        private bool m_isDocumentCompleted;
        public bool isDocumentCompleted
        {
            get { return m_isDocumentCompleted; }
            set { m_isDocumentCompleted = value; }
        }


        public cGeckoBrowser()
        {
            InitializeComponent();
        }

        public cGeckoBrowser(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public GeckoDocument GetDocument()
        {
            try
            {
                GeckoDocument htm = this.Document;
                return htm;
            }
            catch
            {

            }

            return null;
        }

        protected override void OnDocumentCompleted(GeckoDocumentCompletedEventArgs e)
        {
            base.OnDocumentCompleted(e);

            if (this.IsBusy == false && this.IsAjaxBusy == false)
            {
                m_isDocumentCompleted = true;
            }
        }
    }
}
