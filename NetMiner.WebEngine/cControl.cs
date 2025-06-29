using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetMiner.IPC.Server;
using System.ServiceModel;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace NetMiner.WebEngine
{
    public class cControl:cServerImpl
    {
        private frmMain m_Main;
        private ServiceHost g_WCFService;
        public cControl()
        {

        }

        ~cControl()
        {
            
            
        }

        public override eResponse GetHtml(string Url)
        {
            frmBrowser f = new WebEngine.frmBrowser();
            f.MdiParent = Program.mf;
            f.Show();

            eResponse response = new eResponse();
            response.HtmlSource= f.GetUrl(Url);
            response.Cookie = f._ActiveWebBrowser.Document.Cookie;

            //关闭此窗体
            f.Dispose();
            f = null;

            return response;
        }

    }
}
