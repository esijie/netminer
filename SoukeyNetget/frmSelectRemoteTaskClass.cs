using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MinerSpider
{
    public partial class frmSelectRemoteTaskClass : Form
    {

        public delegate void ReturnTaskClass(int taskClassID);
        public ReturnTaskClass RTaskClass;

        public frmSelectRemoteTaskClass()
        {
            InitializeComponent();
        }

        private void frmSelectRemoteTaskClass_Load(object sender, EventArgs e)
        {
            //加载数据
            //localhost.NetMinerWebService sWeb = new localhost.NetMinerWebService();
            //sWeb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";

            //if (Program.g_IsAuthen == true)
            //    sWeb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            //DataTable d = sWeb.GetAllTaskClass();

            //if (d == null)
            //    return;

            ////增加任务
            //for (int i = 0; i < d.Rows.Count; i++)
            //{
            //    if (d.Rows[i]["FID"].ToString() == "0")
            //    {
            //        TreeNode tNode = new TreeNode();
            //        tNode.Name = d.Rows[i]["ID"].ToString();
            //        tNode.Text = d.Rows[i]["ClassName"].ToString();
            //        tNode.ImageIndex = 1;
            //        tNode.SelectedImageIndex = 1;

            //        if (ExistsChildNode(d, d.Rows[i]["FID"].ToString()))
            //            AddNode(tNode, d, d.Rows[i]["ID"].ToString());

            //        this.treeView1.Nodes["nodRoot"].Nodes.Add(tNode);
            //    }
            //}

            //this.treeView1.ExpandAll();

            //sWeb = null;
        }

        private bool ExistsChildNode(DataTable tb, string fid)
        {
            DataRow[] row = tb.Select("FID=" + fid);

            if (row.Length > 0)
                return true;

            return false;
        }

        private void AddNode(TreeNode tNode, DataTable d, string FID)
        {
            for (int i = 0; i < d.Rows.Count; i++)
            {
                if (d.Rows[i]["FID"].ToString() == FID)
                {
                    TreeNode nNode = new TreeNode();
                    nNode.Name = d.Rows[i]["ID"].ToString();
                    nNode.Text = d.Rows[i]["ClassName"].ToString();
                    nNode.ImageIndex = 1;
                    nNode.SelectedImageIndex = 1;

                    if (ExistsChildNode(d, d.Rows[i]["FID"].ToString()))
                        AddNode(tNode, d, d.Rows[i]["ID"].ToString());

                    tNode.Nodes.Add(nNode);

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode.Name == "nodRoot")
                RTaskClass(0);
            else
                RTaskClass(int.Parse(this.treeView1.SelectedNode.Name));

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RTaskClass(-1);
            this.Close();
        }
    }
}
