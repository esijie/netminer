using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;

namespace NetMiner.Gather.Control
{
    public partial class frmSelectCert : Form
    {
        public delegate void ReturnCert(X509Certificate2 cert);
        public ReturnCert rCert;
        X509Store _store = new X509Store(StoreName.My, StoreLocation.CurrentUser);

        public frmSelectCert()
        {
            InitializeComponent();
        }

        private void frmSelectCert_Load(object sender, EventArgs e)
        {
            _store.Open(OpenFlags.ReadWrite);

            for (int i = 0; i < _store.Certificates.Count; i++)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.Text = i.ToString();
                cItem.SubItems.Add(_store.Certificates[i].GetIssuerName());
                cItem.SubItems.Add(_store.Certificates[i].GetName());
                this.listView1.Items.Add(cItem);
            }
        }

        private void frmSelectCert_FormClosing(object sender, FormClosingEventArgs e)
        {
            _store = null;
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count == 0)
            {
                MessageBox.Show("不存在证书，请先通过证书管理导入证书再进行选择，点击“取消”按钮退出！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (this.listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请先选择证书，再确定退出！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            rCert(_store.Certificates[int.Parse(this.listView1.SelectedItems[0].Text.ToString())]);
            this.Close();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
