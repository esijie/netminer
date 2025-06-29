using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Gecko;
using System.Threading;
using System.ServiceModel;
using NetMiner.Common;
using NetMiner.IPC.Server;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Gecko.DOM;

namespace NetMiner.WebEngine
{
    public partial class frmMain : Form
    {

        //private Queue<T> m_requestList;
        public static ServiceHost g_WCFService;


        public frmMain()
        {
            InitializeComponent();

            bool is64;
            is64 = Environment.Is64BitOperatingSystem;

            if (is64)
            {
                Program.g_xulPath = Program.getPrjPath() + "resource\\xulrunner\\64";
            }
            else
                Program.g_xulPath = Program.getPrjPath() + "resource\\xulrunner\\32";


            //IniBrowser(xulRunnerPath);

            IniWCF();
        }

        private void IniWCF()
        {
            NetNamedPipeBinding namePipe = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            g_WCFService = new ServiceHost(typeof(cControl));
            g_WCFService.AddServiceEndpoint(typeof(iServer), namePipe, "net.pipe://localhost/netminerWebEngineService/");
            g_WCFService.Open();
        }

        private void CloseWCF()
        {
            g_WCFService.Close();
            g_WCFService = null;
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseWCF();

            Environment.Exit(0);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmBrowser f = new WebEngine.frmBrowser();
            f.MdiParent = this;
            f.Show();
        }
    }
}
