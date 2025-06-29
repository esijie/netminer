using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using System.Net;
using Newtonsoft;
using Newtonsoft.Json;
using System.Resources;
using System.Reflection;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.gTask;
using NetMiner.Resource;

namespace MinerSpider
{
    public partial class frmFindTask : Form
    {
        private ResourceManager rm;
        public delegate void FindTask(string taskName);
        public FindTask FTask;

        public frmFindTask()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked == true)
            {
                FTask(this.txtKey.Text);
                this.Activate();
            }
            else if (this.radioButton2.Checked == true)
            {
                SearchWebTask();
            }
        }

        private void SearchWebTask()
        {
            this.listView1.Items.Clear();

            string url = "http://www.netminer.cn/hander/searchresource.ashx";
            string postData = "mode=searchtask&word=" + System.Web.HttpUtility.UrlEncode(this.txtKey.Text, Encoding.UTF8);
            string cookie = string.Empty;
            string header = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0\r\n"
                + "Content-Type: application/x-www-form-urlencoded";

            string rHeader = string.Empty;
            string htmlSource = NetMiner.Net.Socket.HttpUnity.GetHtml(url, cGlobalParas.RequestMethod.Post, postData,
               cGlobalParas.WebCode.auto, ref cookie, header, cGlobalParas.ProxyType.None, "", 80,
                out rHeader);

            if (string.IsNullOrEmpty(htmlSource))
                return;

            DataTable d = JsonConvert.DeserializeObject<DataTable>(htmlSource);
            for (int i = 0; i < d.Rows.Count; i++)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Name = d.Rows[i]["ID"].ToString();
                cItem.Text = d.Rows[i]["gName"].ToString();
                cItem.SubItems.Add(d.Rows[i]["DownIntegral"].ToString());
                cItem.SubItems.Add(d.Rows[i]["Version"].ToString());
                this.listView1.Items.Add(cItem);
            }

        }

        private void txtKey_TextChanged(object sender, EventArgs e)
        {
            if (this.txtKey.Text == "")
            {
                this.button1.Enabled = false;
            }
            else
            {
                this.button1.Enabled = true;
            }
        }

        private void frmFindTask_Load(object sender, EventArgs e)
        {
            //������������ʾ����Դ�ļ�
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }

        private void frmFindTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked == true)
            {
                this.Height = 115;
                this.button1.Text = "������һ��";
            }
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (this.radioButton2.Checked == true)
            {
                this.Height = 370;
                this.button1.Text = "��������";
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count ==0)
                return ;
           
            if (MessageBox.Show("����ǰ������Ҫ�����Ĺ����˻����а�(����������û�����ͨ������-��Դ�������أ����ֹ���������)��ͬʱ���ָ���Ĳɼ�����������Ҫ���֣�ϵͳ���Զ����п۳���"
                + "�Ƿ����زɼ�����" + this.listView1.SelectedItems[0].Text + "��", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            

            //��ʼ��֤���
            string url = "http://www.netminer.cn/user/hander/upload.ashx";
            string postData = "mode=CheckUser&u=" + Program.RegisterUser;
            string cookie = string.Empty;
            string header = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0\r\n"
                + "Content-Type: application/x-www-form-urlencoded";

            string rHeader = string.Empty;

            string htmlSource = NetMiner.Net.Socket.HttpUnity .GetHtml(url, cGlobalParas.RequestMethod.Post, postData,
                cGlobalParas.WebCode.auto, ref cookie, header, cGlobalParas.ProxyType.None, "", 80,
                out rHeader);

            if (htmlSource.ToLower() != "true")
            {
                MessageBox.Show("���Ĳ�Ʒδ����վ�˺Ű󶨣����ȵ�¼��·�󹤹���վ�㣬ע���˺ţ���¼����Ա���ģ�ʵ�ֲ�Ʒ�󶨡�", "����� ��Ϣ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //��ȡ�ļ�����������ֲ��������ȡΪfalse
            url = "http://www.netminer.cn/hander/searchresource.ashx";
            postData = "mode=downloadtask&user=" + Program.RegisterUser + "&id=" + this.listView1.SelectedItems[0].Name ;
            cookie = string.Empty;
            header = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0\r\n"
                + "Content-Type: application/x-www-form-urlencoded";

            htmlSource = NetMiner.Net.Socket.HttpUnity.GetHtml(url, cGlobalParas.RequestMethod.Post, postData,
               cGlobalParas.WebCode.auto, ref cookie, header, cGlobalParas.ProxyType.None, "", 80,
               out rHeader);

            if (htmlSource == "false")
            {
                MessageBox.Show("���Ļ��ֲ������޷����ش�����", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string dUrl = "http://www.netminer.cn/hander/searchresource.ashx<POST:ASCII>mode=downfile&fileName=" + System.Web.HttpUtility.UrlEncode(htmlSource,Encoding.UTF8) + "</POST>";

            //��ʼ�����ļ������˲�������Ѿ�����
            NetMiner.Gather.Control.cGatherWeb gweb = new NetMiner.Gather.Control.cGatherWeb(Program.getPrjPath ());
            //eHeader h=new eHeader ();
            //h.Label ="User-Agent";
            //h.LabelValue ="Mozilla/5.0 (Windows NT 6.1; WOW64; rv:38.0) Gecko/20100101 Firefox/38.0";
            //gweb.Headers.Add(h);
            //h = new eHeader();
            //h.Label = "Content-Type";
            //h.LabelValue = "application/x-www-form-urlencoded";
            //gweb.Headers.Add(h);
            string fileName=Program.getPrjPath () + "tmp\\" + this.listView1.SelectedItems[0].Text + ".smt";
            //gweb.IsCustomHeader = true;
            gweb.DownloadFile(dUrl, "", fileName, NetMiner.Resource.cGlobalParas.DownloadFileDealType.SaveCover,
                cookie, "", false, "", null);

            gweb = null;

            try
            {
                //��ʼ�����ļ�
                ImportTask(fileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("�����ѳɹ����أ��洢��tmpĿ¼�£�������ʧ�ܣ�������Ϣ��" + ex.Message + "������ݴ�����Ϣ�Ŵ�����µ��롣", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("�������سɹ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        public void ImportTask(string tName)
        {


            string FileName = tName;
            string TaskClass = "";
            string NewFileName = "";

            //��֤�����ʽ�Ƿ���ȷ
            try
            {
                oTask t = new oTask(Program.getPrjPath());
                t.LoadTask(FileName);

                if (t.TaskEntity.TaskName != "")
                {
                    NewFileName = t.TaskEntity.TaskName + ".smt";
                }

                //��������ķ��ർ��ָ����Ŀ¼
                string TaskPath = Program.getPrjPath() + "tasks\\";
                if (TaskClass != "")
                {
                    TaskPath += TaskClass;
                }

                if (NewFileName == "")
                {
                    NewFileName = "task" + DateTime.Now.ToFileTime().ToString() + ".smt";
                }


                if (FileName == TaskPath + NewFileName)
                {
                    MessageBox.Show(rm.GetString("Info45"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                oTaskClass cTClass = new oTaskClass(Program.getPrjPath());

                if (!cTClass.IsExist(TaskClass) && TaskClass != "")
                {
                    //��ʾ��һ�����������
                    int TaskClassID = cTClass.AddTaskClass(TaskClass);//, TaskPath);
                    cTClass = null;

                    MessageBox.Show ("���ص���������һ���µĲɼ����࣬ϵͳ������������񣬵���ɹ�����ˢ�²ɼ�������࣬�Ա����������,�����ȷ����������������","����� ��Ϣ",MessageBoxButtons.OK ,MessageBoxIcon.Information );

                }

                bool isExistTask = false;
                string TargetFileName = TaskPath + "\\" + NewFileName;

                if (System.IO.File.Exists(TargetFileName))
                {
                    isExistTask = true;
                    if (MessageBox.Show("�Ѿ�������ͬ���ƵĲɼ������Ƿ񸲸ǣ�", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        return;
                }

                try
                {
                    System.IO.File.Copy(FileName, TaskPath + "\\" + NewFileName, true);
                }
                catch (System.IO.IOException ex)
                {
                    t = null;
                    MessageBox.Show("��������������" + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //�������һ���Ѿ����ڵĲɼ������򲻲��������ļ�
                if (isExistTask == false)
                {
                    oTaskIndex ti = new oTaskIndex(Program.getPrjPath(),Program.getPrjPath() + NetMiner.Constant.g_TaskPath + "\\index.xml");
                    eTaskIndex ei = new eTaskIndex();

                    ei.ID = 0;
                    ei.TaskName = t.TaskEntity.TaskName;
                    ei.TaskType = t.TaskEntity.TaskType;
                    ei.TaskRunType = t.TaskEntity.RunType;
                    ei.ExportFile = t.TaskEntity.ExportFile;
                    ei.PublishType = t.TaskEntity.ExportType;
                    ei.TaskState = NetMiner.Resource.cGlobalParas.TaskState.UnStart;
                    ei.WebLinkCount = t.TaskEntity.UrlCount;

                    //�������������ļ�
                    ti.InsertTaskIndex(ei);
                    ti.Dispose();
                    ti = null;
                }

                t = null;


            }
            catch (NetMiner. NetMinerException)
            {
                if (MessageBox.Show(rm.GetString("Quaere17"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    frmUpgradeTask fu = new frmUpgradeTask(FileName);
                    fu.ShowDialog();
                    fu.Dispose();
                    fu = null;
                }

                return;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info48") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}