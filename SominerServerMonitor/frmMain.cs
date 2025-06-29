using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.ServiceModel;
using NetMiner.Base;

namespace SominerServerMonitor
{
    public partial class frmMain : Form
    {
        private bool m_IsExist = false;
        private string m_conFile = string.Empty;

        private System.Threading.Timer m_ServerEngine;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            this.UpdateStateSend += new UpdateStateEventHandler(this.updateStateSend);

            m_conFile = Program.getPrjPath() + "sysConfig.xml";

            //加载托盘图标
            this.notifyIcon1.Visible = true;

            this.notifyIcon1.ShowBalloonTip(2, "网络矿工采集服务监控器启动", "服务监控信息", ToolTipIcon.Info); 

            //加载已经存在的采集引擎
            IniData();

            //创建一个定时器，用于更新UI显示
            m_ServerEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_ServerEngine_CallBack), null, 20000, 20000);

        }

        private bool m_IsRunning = false;
        private void m_ServerEngine_CallBack(object State)
        {
            if (m_IsRunning == false )
            {
                m_IsRunning = true;

                for (int i = 0; i < this.treeView1.Nodes["nodRoot"].Nodes.Count; i++)
                {
                    string url = this.treeView1.Nodes["nodRoot"].Nodes[i].Text;
                    if (isConnect(url))
                    {
                        this.OnUpdateStateSend(this, new UpdateStateEventArgs(i, 1));
                    }
                    else
                    {
                        this.OnUpdateStateSend(this, new UpdateStateEventArgs(i,2));
                    }
                }

                m_IsRunning = false;
            }

        }

        private bool isConnect(string url)
        {
            bool isCon = false;
            //开始尝试连接服务器
            EndpointAddress eAdd = new EndpointAddress(url);
            DistriGather.iGatherClient client = null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                isCon = client.isConnect();
              
            }
            catch (System.Exception)
            {
                isCon =false ;
            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                    client.Close();
            }

            return isCon;
        }

        private void IniData()
        {
            this.treeView1.Nodes["nodRoot"].Nodes.Clear();

            cXmlIO xml = new cXmlIO(m_conFile);

            DataView dv = xml.GetData("descendant::Engines");

            for (int i = 0; i < dv.Count; i++)
            {
                string serverAdd=string.Empty ;
                TreeNode tNode = new TreeNode();
                tNode.Name = i.ToString ();
                tNode.Text = dv[i].Row["Server"].ToString();
                serverAdd= dv[i].Row["Server"].ToString();
                

                //开始尝试连接服务器
                EndpointAddress eAdd = new EndpointAddress(serverAdd);
                DistriGather.iGatherClient client=null;
                
                try
                {
                    client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                    
                    if (client.isConnect())
                    {
                        tNode.ImageIndex =1;
                        tNode.SelectedImageIndex = 1;
                    }
                    else
                    {
                        tNode.ImageIndex = 2;
                        tNode.SelectedImageIndex = 2;
                    }
                }
                catch (System.Exception)
                {
                    tNode.ImageIndex = 2;
                    tNode.SelectedImageIndex = 2;
                }
                finally
                {
                    if (client!=null && client.State == CommunicationState.Opened)
                        client.Close();
                }

                this.treeView1.Nodes["nodRoot"].Nodes.Add(tNode);
            }

            xml = null;

            this.sta1.Text = "分布式采集引擎：" + this.treeView1.Nodes["nodRoot"].Nodes.Count.ToString ();

            this.treeView1.ExpandAll();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.treeView1.SelectedNode == null || this.treeView1.SelectedNode.Name == "nodRoot")
            {
                this.dataGridView1.Rows.Clear();
                return;
            }

            LoadEngineInfo(this.treeView1.SelectedNode.Text);

            this.sta2.Text = "当前选中：" + this.treeView1.SelectedNode.Text;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_IsExist == false)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void rMenuExit_Click(object sender, EventArgs e)
        {
            m_IsExist = true;
            this.Close();
        }

        private void treeView1_Click(object sender, EventArgs e)
        {
           

        }

        //加载分布式采集引擎的共组哦情况
        private void LoadEngineInfo(string url)
        {
            this.dataGridView1.Rows.Clear();

            EndpointAddress eAdd = new EndpointAddress(url);
            DistriGather.iGatherClient client = null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                DistriGather.cTaskData[] tdatas = client.GetTaskList ();
                for (int i = 0; i < tdatas.Length; i++)
                {
                    this.dataGridView1.Rows.Add(tdatas[i].TaskIDk__BackingField,tdatas[i].TaskNamek__BackingField, tdatas[i].TaskStatek__BackingField, tdatas[i].UrlCountk__BackingField, tdatas[i].GatherUrlCountk__BackingField);
                }

            }
            catch (System.Exception)
            {
            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                    client.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmAddConnnect f = new frmAddConnnect();
            f.rUrl = this.getUrl;
            f.ShowDialog();
            f.Dispose();
        }

        private void getUrl(string url)
        {
            cXmlIO xml = new cXmlIO(m_conFile);
            string strxml=string.Empty ;
            strxml="<Server>" + url + "</Server>";
            xml.InsertElement("Engines", "Engine", strxml);
            xml.Save();
            xml = null;

            TreeNode tNode = new TreeNode();
            tNode.Name = (this.treeView1.Nodes["nodRoot"].Nodes.Count +1).ToString ();
            tNode.Text = url;
            string serverAdd= url;

            //开始尝试连接服务器
            EndpointAddress eAdd = new EndpointAddress(serverAdd);
            DistriGather.iGatherClient client=null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                if (client.isConnect())
                {
                    tNode.ImageIndex = 1;
                    tNode.SelectedImageIndex = 1;
                }
                else
                {
                    tNode.ImageIndex = 2;
                    tNode.SelectedImageIndex = 2;
                }
            }
            catch (System.Exception)
            {
                tNode.ImageIndex = 2;
                tNode.SelectedImageIndex = 2;

            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                client.Close();
            }

            this.treeView1.Nodes["nodRoot"].Nodes.Add(tNode);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode == null || this.treeView1.SelectedNode.Name == "nodRoot")
            {
                return;
            }

            if (MessageBox.Show("您确认删除分布式采集引擎的监控，引擎地址：" + this.treeView1.SelectedNode.Text + " ？", "网络矿工 询问",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            cXmlIO xml = new cXmlIO(m_conFile);
            xml.DeleteChildNodes("Engines", "Server", this.treeView1.SelectedNode.Text);
            xml.Save();
            xml = null;

            this.treeView1.Nodes["nodRoot"].Nodes.Remove(this.treeView1.SelectedNode);

        }

        public delegate void UpdateStateHandler(UpdateStateEventArgs e);
        private void updateStateSend(object sender, UpdateStateEventArgs e)
        {
            UpdateStateHandler handler = new UpdateStateHandler(UpdateUI);
            this.Invoke(handler, new object[] { e });
        }

        private void UpdateUI(UpdateStateEventArgs e )
        {
            this.treeView1.Nodes["nodRoot"].Nodes[e.index].ImageIndex = e.imageIndex;
            this.treeView1.Nodes["nodRoot"].Nodes[e.index].SelectedImageIndex = e.imageIndex;
        }


        //定义代理更新UI显示
        public delegate void UpdateStateEventHandler(object sender, UpdateStateEventArgs e);
        public event UpdateStateEventHandler UpdateStateSend;
        public void OnUpdateStateSend(object sender, UpdateStateEventArgs e)
        {
            if (UpdateStateSend != null)
                this.UpdateStateSend(sender, e);
        }

        private void rMenuStopTask_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;

            long taskID = long.Parse(this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            //开始尝试连接服务器
            EndpointAddress eAdd = new EndpointAddress(this.treeView1.SelectedNode.Text );
            DistriGather.iGatherClient client = null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                client.StopTask(taskID);
            }
            catch (System.Exception)
            {
               
            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                    client.Close();
            }
        }

        private void rMenuResetTask_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;

            long taskID = long.Parse(this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            //开始尝试连接服务器
            EndpointAddress eAdd = new EndpointAddress(this.treeView1.SelectedNode.Text);
            DistriGather.iGatherClient client = null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                client.ResetTask(taskID);
            }
            catch (System.Exception)
            {

            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                    client.Close();
            }
        }

        private void rMenuDelTask_Click(object sender, EventArgs e)
        {
            bool isS=DelTask();

            if (isS == true)
            {
                this.dataGridView1.Focus();
                SendKeys.Send("{Del}");
            }
        }

        private bool DelTask()
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return true ;

            bool iss = true ;
            long taskID = long.Parse(this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
            //开始尝试连接服务器
            EndpointAddress eAdd = new EndpointAddress(this.treeView1.SelectedNode.Text);
            DistriGather.iGatherClient client = null;

            try
            {
                client = new DistriGather.iGatherClient("wsHttpBinding_Con", eAdd);
                client.DelTask(taskID);
            }
            catch (System.Exception)
            {
                iss = false;
            }
            finally
            {
                if (client != null && client.State == CommunicationState.Opened)
                    client.Close();

                
            }

            return iss;
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                bool iss=DelTask();
                if (iss==false )
                    e.SuppressKeyPress = true;
            }
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count == 0)
                return;

            switch (this.dataGridView1.SelectedRows[0].Cells[2].Value.ToString())
            {
                case "Started":
                    this.rMenuStopTask.Enabled = true;
                    this.rMenuResetTask.Enabled = false;
                    this.rMenuDelTask.Enabled = false;
                    break;
                case "Failed":
                    this.rMenuStopTask.Enabled = false ;
                    this.rMenuResetTask.Enabled = true ;
                    this.rMenuDelTask.Enabled = true;
                    break;
                default :
                    this.rMenuStopTask.Enabled = true  ;
                    this.rMenuResetTask.Enabled = true ;
                    this.rMenuDelTask.Enabled = true;
                    break;
            }

        }

        private void rMenuOpen_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }


    }

    //定义一个消息的事件，用于传递需要更新的Url地址信息
    public class UpdateStateEventArgs : EventArgs
    {
        public UpdateStateEventArgs(int index,int imageIndex)
        {
            _index = index;
            _imageIndex = imageIndex;
        }

        private int _index;
        public int index
        {
            get { return _index; }
            set { _index = value; }
        }

        private int _imageIndex;
        public int imageIndex
        {
            get { return _imageIndex; }
            set { _imageIndex = value; }
        }

    }
}
