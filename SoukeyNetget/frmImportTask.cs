using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using NetMiner;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;

namespace MinerSpider
{
    public partial class frmImportTask : Form
    {
        private ResourceManager rm;

        public frmImportTask()
        {
            InitializeComponent();
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdAddTask_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = rm.GetString("Info44");

            openFileDialog1.InitialDirectory = Program.getPrjPath() + "tasks";
            openFileDialog1.Filter = "SoukeyMiner Task Files(*.smt)|*.smt|XML File(*.xml)|*.xml";


            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            

            for (int i = 0; i < this.openFileDialog1.FileNames.Length; i++)
            {
                string FileName = this.openFileDialog1.FileNames[i].ToString();

                oTask t = new oTask(Program.getPrjPath());

                ListViewItem cItem = new ListViewItem();

                try
                {

                    t.LoadTask(FileName);

                    cItem.Text = t.TaskEntity.TaskName;
                    //cItem.SubItems.Add(t.TaskEntity.TaskClass);
                    cItem.SubItems.Add(t.TaskEntity.TaskVersion.ToString());
                    cItem.SubItems.Add(rm.GetString("Info255"));
                    cItem.SubItems.Add(FileName);
                    cItem.ImageIndex = 0;

                }
                catch (System.Exception ex)
                {

                    string fName = System.IO.Path.GetFileName(FileName);
                    fName = fName.Substring(0, fName.LastIndexOf(".") - 1);

                    cItem.Text = fName;
                    cItem.SubItems.Add(rm.GetString("Info260"));
                    cItem.SubItems.Add(rm.GetString("Info260"));
                    cItem.SubItems.Add(rm.GetString("Info255"));
                    cItem.SubItems.Add(FileName);
                    cItem.ImageIndex = 1;

                }

                this.listTask.Items.Add(cItem);

                t = null;
            }

            this.staNum.Text = "0/" + this.listTask.Items.Count;


        }

        private void frmImportTask_Load(object sender, EventArgs e)
        {
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            string lVersion = Program.SominerVersionCode;
            lVersion = lVersion.Substring(0, lVersion.LastIndexOf("."));

            this.labVersion.Text = lVersion;
            this.labTaskVersion.Text = Program.SominerTaskVersionCode;

            //���زɼ�����
            oTaskClass xmlTClass = new oTaskClass(Program.getPrjPath());
            TreeNode newNode = null;
            for (int i = 0; i < xmlTClass.TaskClasses.Count; i++)
            {
                newNode = new TreeNode();
                newNode.Tag = xmlTClass.TaskClasses[i].tPath;
                newNode.Name = "C" + xmlTClass.TaskClasses[i].ID;
                newNode.Text = xmlTClass.TaskClasses[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (xmlTClass.TaskClasses[i].Children != null && xmlTClass.TaskClasses[i].Children.Count > 0)
                {
                    //�����ӷ���
                    LoadTreeClass(newNode, xmlTClass.TaskClasses[i].Children);
                }

                this.treeTClass.Nodes.Add(newNode);
                newNode = null;
            }

        }

        private void LoadTreeClass(TreeNode tNode, List<NetMiner.Core.gTask.Entity.eTaskClass> ets)
        {
            for (int i = 0; i < ets.Count; i++)
            {
                TreeNode newNode = new TreeNode();
                newNode.Tag = ets[i].tPath;
                newNode.Name = "C" + ets[i].ID;
                newNode.Text = ets[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    //�����ӷ���
                    LoadTreeClass(newNode, ets[i].Children);
                }

                tNode.Nodes.Add(newNode);
                newNode = null;
            }
        }

        private void frmImportTask_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void cmdDelTask_Click(object sender, EventArgs e)
        {
            DelTask();
        }

        private void DelTask()
        {
            if (this.listTask.Items.Count == 0 || this.listTask.SelectedItems.Count == 0)
            {
                return;
            }

            while (this.listTask.SelectedItems.Count > 0)
            {
                this.listTask.Items.Remove(this.listTask.SelectedItems[0]);
            }
        }

        private void listTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelTask();
            }
        }

        private void cmdImport_Click(object sender, EventArgs e)
        {
            this.cmdAddTask.Enabled = false;
            this.cmdDelTask.Enabled = false;
            this.cmdImport.Enabled = false;
            this.cmdExit.Enabled = false;

            this.staStatus.Text = rm.GetString("Info256");
            int errNum=0;

            for (int i = 0; i < this.listTask.Items.Count; i++)
            {
                if (delegateITask(this.listTask.Items[i])==true)
                {
                    this.staNum.Text = (i+1).ToString() + "/" + this.listTask.Items.Count;
                }
                else
                {
                    errNum++;
                    this.staErr.Text = errNum.ToString();
                }

                Application.DoEvents();
            }

            this.cmdExit.Enabled = true;
            this.staStatus.Text = rm.GetString("Info262");

        }

        private delegate bool delegateImportTask(ListViewItem cItem,out string message);

        private bool delegateITask(ListViewItem cItem)
        {
            //����һ���޸ķ������Ƶ�ί��

            string errMessage="";

            delegateImportTask sd = new delegateImportTask(this.ImportTask);

            try
            {
                //��ʼ���ú���,���Դ����� 
                IAsyncResult ir = sd.BeginInvoke(cItem,out errMessage, null, null);

                //��ʾ�ȴ��Ĵ���
                cItem.SubItems[2].Text = rm.GetString("Info256");

                Application.DoEvents();

                //ѭ������Ƿ�������첽�Ĳ��� 
                while (true)
                {
                    if (ir.IsCompleted)
                    {
                        //����˲�����رմ��� 
                        cItem.SubItems[2].Text = rm.GetString("Info257");
                        cItem.ImageIndex = 2;
                        break;
                    }
                }

                //ȡ�����ķ���ֵ 
                bool isSucceed = sd.EndInvoke(out errMessage , ir);

                if (isSucceed == false)
                {
                    cItem.SubItems[2].Text = errMessage;
                    cItem.ImageIndex = 1;
                }
                else
                {
                    cItem.SubItems[2].Text = rm.GetString("Info257");
                    cItem.ImageIndex = 2;
                }

                return isSucceed;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private bool ImportTask(ListViewItem cItem,out string errMessage)
        {
            //֧��ͬʱ����������

            string FileName = cItem.SubItems[3].Text;
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
                string TaskPath = Program.getPrjPath() + this.comTaskClass.Tag.ToString();
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
                    errMessage=rm.GetString("Info45");
                    return false;
                }

                oTaskClass cTClass = new oTaskClass(Program.getPrjPath());

                if (!cTClass.IsExist(TaskClass) && TaskClass != "")
                {
                    //��ʾ��һ�����������
                    int TaskClassID = cTClass.AddTaskClass(TaskClass);//, TaskPath);
                    cTClass = null;

                }

                try
                {
                    System.IO.File.Copy(FileName, TaskPath + "\\" + NewFileName);
                }
                catch (System.IO.IOException)
                {
                    t = null;
                    errMessage=rm.GetString("Info46");
                    return false;
                }

                oTaskIndex ti = new oTaskIndex(Program.getPrjPath());

                string tClass = this.comTaskClass.Tag.ToString();
                tClass = tClass.Replace("tasks\\", "");
                tClass = tClass.Replace("\\", "/");

                ti.LoadIndexDocument(tClass);
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


                t = null;

                errMessage = "";

                return true;
            }
            catch (NetMinerException)
            {
                errMessage= rm.GetString("Info258");
                return false;
            }

            catch (System.Exception ex)
            {
                errMessage=rm.GetString("Info48") + ex.Message ;
                return false;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.treeTClass.Visible == false)
                this.treeTClass.Visible = true;
            else
                this.treeTClass.Visible = false;
        }

        private void treeTClass_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.comTaskClass.Text = this.treeTClass.SelectedNode.Text;
            this.comTaskClass.Tag = this.treeTClass.SelectedNode.Tag;

            this.treeTClass.Visible = false;
        }
    }
}