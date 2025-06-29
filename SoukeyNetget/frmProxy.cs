using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using NetMiner.Gather;
using NetMiner.Core.Proxy;
using NetMiner.Core.Proxy.Entity;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmProxy : Form
    {
        private Hashtable m_HashUrl = new Hashtable();
        cVerifyProxy vProxy = new cVerifyProxy(Program.getPrjPath ());

        public delegate void startHandler (cStartVerifyArgs e);
        public delegate void stopHandler(cStopVerifyArgs e);
        public delegate void showInfoHandler(cShowInfoArgs e);
        public delegate void completedHandler(cCompletedVerifyArgs e);
     

        public frmProxy()
        {
            InitializeComponent();
        }

        private void frmProxy_Load(object sender, EventArgs e)
        {
       

            vProxy.StartVerifySend += new cVerifyProxy.StartedEventHandler(this.on_Started);
            vProxy.StopVerifySend += new cVerifyProxy.StopEventHandler(this.on_Stopped);
            vProxy.ShowMessageSend += new cVerifyProxy.ShowStateEventHandler(this.on_ShowInfo);
            vProxy.CompletedVerifyEventSend += new cVerifyProxy.CompletedEventHandler(this.on_Completed);

            this.splitContainer1.SplitterDistance = 66;

            #region 加载代理信息
            oProxy conProxy = new oProxy(Program.getPrjPath());

            IEnumerable<eProxy> proxys = conProxy.LoadProxyData();

            foreach(eProxy proxy in proxys)
            {
                if (!m_HashUrl.ContainsKey(proxy.ProxyServer))
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = proxy.ProxyServer;
                    item.Text = proxy.ProxyServer;
                    item.SubItems.Add(proxy.ProxyPort);
                    item.SubItems.Add(proxy.User);
                    item.SubItems.Add(proxy.Password);
                    item.SubItems.Add("");
                    item.SubItems.Add("");

                    //加入哈希表，防止地址重复
                    m_HashUrl.Add(proxy.ProxyServer, proxy.ProxyServer);

                    this.listProxy.Items.Add(item);

                }
            }


            this.sta2.Text = this.listProxy.Items.Count + " IP";
            conProxy = null;
            #endregion
        }

        private void SaveProxy()
        {
            #region 保存代理信息

            oProxy conProxy = new oProxy(Program.getPrjPath());
            conProxy.Dele();

            //构建xml
            for (int i = 0; i < this.listProxy.Items.Count; i++)
            {
                //string sXml = "<Server>" + this.listProxy.Items[i].Text + "</Server>";
                //sXml += "<Port>" + this.listProxy.Items[i].SubItems[1].Text.ToString() + "</Port>";
                //sXml += "<User>" + this.listProxy.Items[i].SubItems[2].Text.ToString() + "</User>";
                //sXml += "<Password>" + this.listProxy.Items[i].SubItems[3].Text.ToString() + "</Password>";


                eProxy proxy = new eProxy();
                proxy.ProxyServer = this.listProxy.Items[i].Text;
                proxy.ProxyPort = this.listProxy.Items[i].SubItems[1].Text.ToString();
                proxy.User = this.listProxy.Items[i].SubItems[2].Text.ToString();
                proxy.Password = this.listProxy.Items[i].SubItems[3].Text.ToString();
                conProxy.InsertProxy(proxy);
            }

            conProxy.Save();
            conProxy = null;

            #endregion
        }

        private void toolStart_Click(object sender, EventArgs e)
        {
            vProxy.VerifyUrl = this.textBox1.Text;
            vProxy.VerifyResult = this.textBox2.Text;

            List<eProxy> m_Proxys=new List<eProxy> ();

            for (int i=0;i<this.listProxy.Items.Count ;i++)
            {
                eProxy p = new eProxy();
                p.ProxyServer = this.listProxy.Items[i].Text;
                p.ProxyPort = this.listProxy.Items[i].SubItems[1].Text;
                p.User = this.listProxy.Items[i].SubItems[2].Text;
                p.Password = this.listProxy.Items[i].SubItems[3].Text;
                m_Proxys.Add(p);

                this.listProxy.Items[i].SubItems[4].Text = "";
                this.listProxy.Items[i].SubItems[5].Text = "";
            }

            vProxy.Start(m_Proxys);
        }

        private void toolImport_Click(object sender, EventArgs e)
        {
            string fName = string.Empty;

            this.openFileDialog1.Title = "选择IP文件[每行一个代理IP]";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "All Files(*.txt)|*.txt";

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fName = this.openFileDialog1.FileName;
                StreamReader fileReader = null;

                

                try
                {
                    fileReader = new StreamReader(fName, System.Text.Encoding.GetEncoding("gb2312"));
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("文件打开失败，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string str = fileReader.ReadToEnd();
                fileReader.Close();
                fileReader = null;

                oProxy cp = new oProxy(Program.getPrjPath());

                try
                {
                    string[] Urls = Regex.Split(str, "\r\n");

                    int i = 0;

                    //定义一个临时hashtable
                    Hashtable tmpIP = new Hashtable();

                    for (i = 0; i < Urls.Length; i++)
                    {
                        string ss = Urls[i].Replace("\r", "").Replace("\n", "").Trim();
                        if (ss != "" && !m_HashUrl.ContainsKey(ss.Substring(0, ss.IndexOf(":"))))
                        {
                            tmpIP.Add(ss,ss);

                            m_HashUrl.Add(ss.Substring(0, ss.IndexOf(":")), ss.Substring(0, ss.IndexOf(":")));
                          
                        }
                    }

                    //开始添加数据
                    ListViewItem[] items = new ListViewItem[tmpIP.Count];
                    int index = 0;

                    foreach (DictionaryEntry de in tmpIP)
                    {
                        string ss = de.Value.ToString();
                        items[index] = new ListViewItem();
                        items[index].Name = ss.Substring(0, ss.IndexOf(":"));
                        items[index].Text = ss.Substring(0, ss.IndexOf(":"));
                        items[index].SubItems.Add(ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1));
                        items[index].SubItems.Add("");
                        items[index].SubItems.Add("");
                        items[index].SubItems.Add("");
                        items[index].SubItems.Add("");

                        //string sXml = "<Server>" + ss.Substring(0, ss.IndexOf(":")) + "</Server>";
                        //sXml += "<Port>" + ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1) + "</Port>";
                        //sXml += "<User></User>";
                        //sXml += "<Password></Password>";

                        eProxy proxy = new eProxy();
                        proxy.ProxyServer = ss.Substring(0, ss.IndexOf(":"));
                        proxy.ProxyPort = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);
                        proxy.User = "";
                        proxy.Password = "";
                        cp.InsertProxy(proxy);

                        index++;
                    }

                    cp.Save();

                    this.listProxy.Items.AddRange(items);
                    Urls = null;

                    this.sta2.Text = this.listProxy.Items.Count + " IP";
                }
                catch
                {
                    MessageBox.Show("文档格式不合法，每行一个IP地址+端口号，不能有空行，尤其是结尾处！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void Started(cStartVerifyArgs e)
        {
            this.toolStart.Enabled = false;
            this.toolStop.Enabled = true;

            this.toolImport.Enabled = false;
            this.toolmenuSave.Enabled = false;
            this.toolDel.Enabled = false;
            this.toolmenuAdd.Enabled = false;
            this.toolDelAll.Enabled = false;
        }

        public void Stopped(cStopVerifyArgs e)
        {
            this.toolStart.Enabled = true;
            this.toolStop.Enabled = false;

            this.toolImport.Enabled = true;
            this.toolmenuSave.Enabled = true ;
            this.toolDel.Enabled = true ;
            this.toolmenuAdd.Enabled = true ;
            this.toolDelAll.Enabled = true;

            if (this.IsDel.Checked ==true )
                DelFaild();

            if (this.IsAutoUpdate.Checked == true)
                AutoUpdate();
        }

        public void ShowInfo(cShowInfoArgs e)
        {
            this.listProxy.Items[e.Url].SubItems[4].Text = e.State.GetDescription();
            this.listProxy.Items[e.Url].SubItems[5].Text = ((float)e.Second/1000).ToString ("#0.##") + "秒";

            int count;
            if (e.State == cGlobalParas.VerifyProxyState.Succeed)
            {
                count = int.Parse(this.sta3.Text)+1;
                this.sta3.Text = count.ToString();
            }
            else if (e.State == cGlobalParas.VerifyProxyState.Faild)
            {
                count = int.Parse(this.sta4.Text) + 1;
                this.listProxy.Items[e.Url].ForeColor = Color.Red;
                this.sta4.Text = count.ToString();
            }
        }

        public void Completed(cCompletedVerifyArgs e)
        {
            this.toolStart.Enabled = true;
            this.toolStop.Enabled = false;

            this.toolImport.Enabled = true;
            this.toolmenuSave.Enabled = true;
            this.toolDel.Enabled = true;
            this.toolmenuAdd.Enabled = true;

            if (this.IsDel.Checked == true)
                DelFaild();

            if (this.IsAutoUpdate.Checked == true)
                AutoUpdate();
        }

        private void DelFaild()
        {
            List<string> dItems = new List<string>();
            for (int i = 0; i < this.listProxy.Items.Count; i++)
            {
                if (this.listProxy.Items[i].SubItems[4].Text == "失败")
                {
                    dItems.Add(this.listProxy.Items[i].Name);
                }
            }

            for (int i = 0; i < dItems.Count; i++)
            {
                this.listProxy.Items.RemoveByKey(dItems[i]);
            }

            oProxy cp = new oProxy(Program.getPrjPath());
            cp.Dele();

            for (int i = 0; i < this.listProxy.Items.Count; i++)
            {
                if (this.listProxy.Items[i].SubItems[4].Text == "成功")
                {
                    //string sXml = "<Server>" + this.listProxy.Items[i].Text  + "</Server>";
                    //sXml += "<Port>" + this.listProxy.Items[i].SubItems[1].Text  + "</Port>";
                    //sXml += "<User>" + this.listProxy.Items[i].SubItems[2].Text + "</User>";
                    //sXml += "<Password>" + this.listProxy.Items[i].SubItems[3].Text + "</Password>";

                    eProxy proxy = new eProxy();
                    proxy.ProxyServer = this.listProxy.Items[i].Text;
                    proxy.ProxyPort = this.listProxy.Items[i].SubItems[1].Text.ToString();
                    proxy.User = this.listProxy.Items[i].SubItems[2].Text.ToString();
                    proxy.Password = this.listProxy.Items[i].SubItems[3].Text.ToString();
                    cp.InsertProxy(proxy);

                }
            }

            cp.Save();
            cp = null;
            this.sta2.Text= this.listProxy.Items.Count + " IP";
        }

        private void AutoUpdate()
        {

        }

        private void on_Started(object sender, cStartVerifyArgs e)
        {
            startHandler handler = new startHandler(Started);
            this.Invoke(handler, new object[] { e });
        }

        private void on_Stopped(object sender, cStopVerifyArgs e)
        {
            stopHandler handler = new stopHandler(Stopped);
            this.Invoke(handler, new object[] { e });
        }

        private void on_ShowInfo(object sender, cShowInfoArgs e)
        {
            showInfoHandler handler = new showInfoHandler(ShowInfo);
            this.Invoke(handler, new object[] { e });
        }

        private void on_Completed(object sender, cCompletedVerifyArgs e)
        {
            completedHandler handler = new completedHandler(Completed);
            this.Invoke(handler, new object[] { e });
        }

        private void frmProxy_FormClosing(object sender, FormClosingEventArgs e)
        {
            vProxy.StartVerifySend -= new cVerifyProxy.StartedEventHandler(this.on_Started);
            vProxy.StopVerifySend -= new cVerifyProxy.StopEventHandler(this.on_Stopped);
            vProxy.ShowMessageSend -= new cVerifyProxy.ShowStateEventHandler(this.on_ShowInfo);
            vProxy.CompletedVerifyEventSend -= new cVerifyProxy.CompletedEventHandler(this.on_Completed);
            vProxy = null;
        }

        private void toolStop_Click(object sender, EventArgs e)
        {
            vProxy.Stop();
        }

        private void toolDel_Click(object sender, EventArgs e)
        {
            DelProxy();
        }

        private void DelProxy()
        {
            for (int i = 0; i < this.listProxy.SelectedItems.Count; i++)
            {
                m_HashUrl.Remove(this.listProxy.SelectedItems[i].Name);
                this.listProxy.Items.RemoveByKey(this.listProxy.SelectedItems[i].Name);
                
            }
            this.toolmenuSave.Enabled = true;
        }

        private void toolmenuAdd_Click(object sender, EventArgs e)
        {
            frmAddProxy f = new frmAddProxy();
            f.rProxy = this.AddProxy;
            f.ShowDialog();
            f.Dispose();
        }

        private void AddProxy(ListViewItem cItem)
        {
            if (cItem != null && !m_HashUrl.ContainsKey(cItem.Name ))
            {
                this.listProxy.Items.Add(cItem);
                m_HashUrl.Add(cItem.Name, cItem.Name);
                this.toolmenuSave.Enabled = true;
            }
        }

        private void toolmenuSave_Click(object sender, EventArgs e)
        {
            SaveProxy();
            this.toolmenuSave.Enabled = false;
        }

        private void toolDelAll_Click(object sender, EventArgs e)
        {
            this.listProxy.Items.Clear();
            m_HashUrl.Clear();
            this.toolmenuSave.Enabled = true;
        }

        private void listProxy_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelProxy();
            }
        }
    }
}
