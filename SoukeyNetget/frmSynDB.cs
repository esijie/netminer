using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text .RegularExpressions ;

namespace MinerSpider
{
    public partial class frmSynDB : Form
    {
        public frmSynDB()
        {
            InitializeComponent();
        }

        private void frmSynDB_Load(object sender, EventArgs e)
        {
            cSynDb sDb = new cSynDb();
            int count = sDb.GetSynCount();

            this.listDb.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                this.listDb.Items.Add(sDb.GetSynName(i));
            }

            sDb = null;
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolAddDb_Click(object sender, EventArgs e)
        {
            frmAddSynDb f = new frmAddSynDb();
            f.rSName = this.GetSName;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetSName(string sName)
        {
            try
            {
                cSynDb sDb = new cSynDb();
                sDb.AddSynDb(sName);
                sDb = null;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("增加同义词库出错，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.listDb.Items.Add(sName);
        }

        private void toolDelDb_Click(object sender, EventArgs e)
        {
            if (this.listDb.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择需要删除的同义词库！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.listDb.SelectedIndex == 0)
            {
                MessageBox.Show("系统同义词库无法删除！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("您确认删除同义词库：" + this.listDb.SelectedItem.ToString() + "，删除后将无法恢复，是否继续？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.No)
                return;

            string sName = this.listDb.SelectedItem.ToString();
            cSynDb sDb = new cSynDb();
            sDb.DelSynDb(sName);
            sDb = null;

            string fName = Program.getPrjPath() + "dict\\" + sName + ".txt";
            File.Delete(fName);

            this.listDb.Items.Remove(this.listDb.SelectedItem);
        }

        private void listDb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listDb.SelectedItem == null)
            {
                this.txtDb.Text = "";
                return;
            }
            string fName = Program.getPrjPath() + "dict\\" + this.listDb.SelectedItem.ToString() + ".txt";

            if (!File.Exists(fName))
            {
                this.txtDb.Text = "";
                this.toolSaveDb.Enabled = false;
                return;
            }

            FileStream myStream = File.Open(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            StreamReader sr = new StreamReader(myStream, System.Text.Encoding.UTF8);

            string str = "";
            string strLine = "";
            while (strLine != null)
            {
                strLine = sr.ReadLine();
                if (strLine != null && strLine.Length > 0)
                {
                   
                    str += strLine + "\r\n";
                    
                }
            }


            sr.Close();
            sr.Dispose();
            myStream.Close();
            myStream.Dispose();

            this.txtDb.Text = str;
            this.toolSaveDb.Enabled = false;
        }

        private void txtDb_TextChanged(object sender, EventArgs e)
        {
            this.toolSaveDb.Enabled = true;
        }

        private void toolSaveDb_Click(object sender, EventArgs e)
        {
            string fName = Program.getPrjPath() + "dict\\" + this.listDb.SelectedItem.ToString() + ".txt";
            string str = this.txtDb.Text;
            string[] ss = Regex.Split(str, "\r\n");

            FileStream myStream = File.Open(fName, FileMode.Create, FileAccess.Write , FileShare.ReadWrite );
            StreamWriter  sr = new StreamWriter(myStream, System.Text.Encoding.UTF8);
            for (int i = 0; i < ss.Length; i++)
            {
                sr.WriteLine(ss[i]);
            }

            sr.Close();
            myStream.Close();

            this.toolSaveDb.Enabled = false;
            
        }
    }
}
