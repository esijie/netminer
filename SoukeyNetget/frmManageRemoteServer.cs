using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using NetMiner.Common;

namespace MinerSpider
{
    public partial class frmManageRemoteServer : DockContent
    {
        public frmManageRemoteServer()
        {
            InitializeComponent();
        }

        private void frmManageRemoteServer_Load(object sender, EventArgs e)
        {
            string Url= Program.ConnectServer ;
            if (Url.EndsWith ("/"))
                Url+= "login.aspx";
            else
                Url+= "/login.aspx";

            this.webBrowser1.Navigate(Url);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().EndsWith("login.aspx"))
            {
                cXmlSConfig xmlCon = new cXmlSConfig(Program.getPrjPath());
                string user = xmlCon.RemoteServerUser;
                string licence = xmlCon.RemoteServerLicence;
                xmlCon = null;

                if (user != "" && licence != "")
                {
                    //自动登录
                    System.Windows.Forms.HtmlDocument document = this.webBrowser1.Document;
                    if (document == null)
                    {
                        return;
                    }
                    document.All["txtUser"].SetAttribute("Value", user);            //设置用户名
                    document.All["txtLicence"].SetAttribute("Value", licence);      //设置密码

                    //document.All["Button2"].RaiseEvent("onClick");        //登录事件
                    //document.All["form1"].InvokeMember("submit");  //提交表单
                    document.All["Button2"].InvokeMember("click");
                }
            }
        }
    }
}
