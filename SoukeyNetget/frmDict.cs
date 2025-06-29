using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using System.IO;
using NetMiner.Gather;
using System.Text.RegularExpressions;
using System.Threading;
using NetMiner.Core.Dict;
using NetMiner.Common.Tool;

namespace MinerSpider
{
    public partial class frmDict : Form
    {

        private string DelContent = "DictClass";
        private ResourceManager rm;

        private ArrayList m_dContent;


        private TreeNode m_LastNode;    //记录选中的节点
        private bool m_IsSaveDict = false;    //标记当前是否保存字典内容
        
        public frmDict()
        {
            InitializeComponent();

        }

        private void IniData()
        {

            DataView dClass = new DataView();
            int i = 0;
            oDict d = new oDict(Program.getPrjPath());

            TreeNode newNode;

            IEnumerable<string> dictClasses = d.GetDictClass();

            int index = 0;
            foreach(string s in dictClasses)
            {
                newNode = new TreeNode();
                newNode.Name = "C" + index.ToString();
                newNode.Text = s;
                newNode.ImageIndex = 1;
                newNode.SelectedImageIndex = 1;
                this.treeDict.Nodes["nodDict"].Nodes.Add(newNode);
                newNode = null;

                index++;
            }
            
            dClass = null;

            this.treeDict.Nodes["nodDict"].Expand();

            //设置默认选择的树形结构节点为“正在运行”
            TreeNode SelectNode = new TreeNode();
            SelectNode = this.treeDict.Nodes["nodDict"];
            this.m_LastNode = SelectNode;
            this.treeDict.SelectedNode = SelectNode;
            SelectNode = null;

            

            d = null;

        }

        private void frmDict_Load(object sender, EventArgs e)
        {
            IniData();

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));
        }
     
        private void AddDictClass()
        {
            this.treeDict.LabelEdit = true;

            TreeNode newNode;
            newNode = new TreeNode();
            newNode.Name = this.treeDict.Nodes.Count.ToString ();
            newNode.Text = rm.GetString ("Label19");
            newNode.ImageIndex = 1;
            newNode.SelectedImageIndex = 1;
            this.treeDict.Nodes["nodDict"].Nodes.Add(newNode);
            this.treeDict.ExpandAll();
            //this.treeDict.SelectedNode =newNode ;
            newNode.BeginEdit();
            newNode = null; 
            

            //this.listDict.Items.Clear();

           
        }

        private void AddDictContent()
        {
            if (this.treeDict.SelectedNode.Name =="nodDict")
            {
                MessageBox.Show(rm.GetString("Info77"), rm.GetString ("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.listDict.LabelEdit = true;
            ListViewItem item = new ListViewItem();
            item.Text = rm.GetString ("Label20");
            this.listDict.Items.Add(item);
            item.BeginEdit();
            item = null;

            m_IsSaveDict = true;
            //this.listDict.Items[this.listDict.Items.Count - 1].BeginEdit();
        }

        private void menuAddDict_Click(object sender, EventArgs e)
        {
            AddDictContent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //修改字典内容
        private void listDict_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label == null)
            {
                //表示没有进行修改
                this.listDict.Items.Remove(this.listDict.Items[e.Item]);
                this.listDict.LabelEdit = false;
                return;
            }

            if (e.Label.ToString().Trim() == "")
            {
                e.CancelEdit = true;
                this.listDict.Items.Remove(this.listDict.Items[e.Item]);
                this.listDict.LabelEdit = false;
                return;
            }

            try
            {
                m_dContent.Add(e.Label.ToString().Trim());
                this.listDict.LabelEdit = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Info78") + ex.Message, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.CancelEdit = true;
            }

        }

        private void menuDelDict_Click(object sender, EventArgs e)
        {
            DelDictContent();
        }

        private void menuDelDictClass_Click(object sender, EventArgs e)
        {
            DelDictClass();
        }

        private void listDict_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.listDict.Items.Count == 0)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                 DelDictContent();
            }
        }

        private void treeDict_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.m_LastNode != this.treeDict.SelectedNode)
            {
                SaveDict();
                this.m_LastNode = this.treeDict.SelectedNode;
            }

           

            ListViewItem item;
            int i = 0;

            this.listDict.Items.Clear();
            if (this.treeDict.SelectedNode.Name =="nodDict")
            {
                return ;
            }

            oDict d = new oDict(Program.getPrjPath());

            m_dContent = d.GetDict(this.treeDict.SelectedNode.Text.Trim());

            d = null;

            if (m_dContent == null)
            {
                m_dContent = new ArrayList();
            }

            ListViewItem[] items = new ListViewItem[m_dContent.Count];
            for (i = 0; i < m_dContent.Count; i++)
            {
                items[i] = new ListViewItem();
                items[i].Text = m_dContent[i].ToString();
                
            }
            this.listDict.Items.AddRange(items);
        }

        private void treeDict_Enter(object sender, EventArgs e)
        {
            DelContent = "DictClass";
        }

        private void listDict_Enter(object sender, EventArgs e)
        {
            DelContent = "Dict";
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (DelContent == "DictClass")
            {
                DelDictClass();
            }
            else if (DelContent =="Dict")
            {
                DelDictContent();
            }
        }

        private void DelDictClass()
        {
            if (this.treeDict.SelectedNode.Name == "nodDict")
            {
                MessageBox.Show(rm.GetString("Info79"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show(rm.GetString("Delete") + "“" + this.treeDict.SelectedNode.Text + "”？" + rm.GetString ("Quaere21"),
               rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                oDict d = new oDict(Program.getPrjPath());

                d.DelDictClass(this.treeDict.SelectedNode.Text);

                d = null;

                this.treeDict.Nodes.Remove(this.treeDict.SelectedNode);
            }
        }

        private void DelDictContent()
        {
            if (this.listDict.Items.Count == 0 || this.listDict.SelectedItems.Count ==0)
            {
                MessageBox.Show(rm.GetString ("Info80"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.listDict.SelectedItems.Count == 1)
            {

                if (MessageBox.Show(rm.GetString("Delete") + "“" + this.listDict.SelectedItems[0].Text + "”？" + rm.GetString("Quaere22"),
                    rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    m_dContent.Remove(this.listDict.SelectedItems[0].Text.ToString());

                    this.listDict.Items.Remove(this.listDict.SelectedItems[0]);

                }
            }
            else
            {
                if (MessageBox.Show("确实删除所选字典内容，删除后将无法恢复，继续？" ,
                    rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    for (int i = 0; i < this.listDict.SelectedItems.Count;i++ )
                    {
                        m_dContent.Remove(this.listDict.SelectedItems[i].Text.ToString());
                    }

                    while (this.listDict.SelectedItems.Count > 0)
                    {
                        this.listDict.Items.Remove(this.listDict.SelectedItems[0]);
                    }
                }
            }

            m_IsSaveDict = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolAddDictClass_Click(object sender, EventArgs e)
        {
            AddDictClass();
        }

        private void menuAddDictClass_Click(object sender, EventArgs e)
        {
            AddDictClass();
        }

        private void treeDict_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelDictClass();
            }
        }

        private void treeDict_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Text == "nodDict" || e.Label == null)
            {
                this.treeDict.Nodes.Remove(e.Node);
                this.treeDict.LabelEdit = false;
                return;
            }

            if (e.Label.ToString().Trim() == "")
            {
                this.treeDict.Nodes.Remove(e.Node);
                return;
            }

            oDict d = new oDict(Program.getPrjPath());
            d.AddDictClass(e.Label .ToString ().Trim ());
            d = null;

            this.treeDict.LabelEdit = false;

        }

        private void toolAddDict_Click(object sender, EventArgs e)
        {
            AddDictContent();
        }

        private void frmDict_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ImportDict();
        }

        private void ImportDict()
        {
            if (this.treeDict.SelectedNode.Name == "nodDict")
            {
                MessageBox.Show("无法将字典数据导入根节点，请选择一个字典分类再进行字典数据导入操作。", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string fileName = "";

            this.openFileDialog1.Title = "选择导入字典数据的文本文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "Text Files(*.txt)|*.txt";

            if (this.openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            fileName = this.openFileDialog1.FileName;

            //判断文件是否存在
            if (!System.IO.File.Exists(fileName))
            {
                MessageBox.Show(rm.GetString("Info235"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show (rm.GetString ("Quaere25") + this.treeDict.SelectedNode.Text ,rm.GetString("MessageboxQuaere"),MessageBoxButtons.YesNo ,MessageBoxIcon.Question )==DialogResult.No )
                return ;

            StreamReader fileReader = null;

            try
            {
                fileReader = new StreamReader(fileName, System.Text.Encoding.GetEncoding("gb2312"));
            }
            catch (System.Exception )
            {
                MessageBox.Show(rm.GetString ("Info236"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //显示等待的窗口 
            frmWaiting fWait = new frmWaiting("正在导入网址,请等待...");
            new Thread((ThreadStart)delegate
            {
                Application.Run(fWait);
            }).Start();
            //fWait.Show(this);


            try
            {
                string dicts = fileReader.ReadToEnd();
                fileReader.Close();
                fileReader = null;
                int i = 0;

                string[] ds = Regex.Split(dicts, "\r\n");

                ArrayList al = new ArrayList();
                for (i = 0; i < ds.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ds[i]))
                    {
                        al.Add(ds[i]);
                    }
                }

                string[] ds1 = (string[])al.ToArray(typeof(string));

                ListViewItem[] items = new ListViewItem[ds1.Length];

                for (i = 0; i < ds1.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ds1[i]))
                    {
                        m_dContent.Add(ds1[i]);
                        items[i] = new ListViewItem();
                        items[i].Text = ds1[i];
                    }
                }

                this.listDict.Items.AddRange(items);

               
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("字典导入发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (fWait.IsHandleCreated)
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                fWait = null;
                m_IsSaveDict = true;
            }
        }

        private void frmDict_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_dContent != null)
            {
                SaveDict();
            }
        }

        private void SaveDict()
        {
            //需要保存字典数据
            if (this.m_LastNode.Name == "nodDict")
            {
                return;
            }

            if (m_IsSaveDict == true)
            {
                oDict d = new oDict(Program.getPrjPath());
                d.Save(this.m_LastNode.Text.Trim(), m_dContent);
                d = null;
                m_dContent = null;

                m_IsSaveDict = false;
            }
        }

        private void menuImportDict_Click(object sender, EventArgs e)
        {
            ImportDict();
        }
     
    }
}