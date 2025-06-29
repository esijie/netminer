using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace MinerSpider
{
    public partial class frmUploadTask : Form
    {
        private static char[] constant = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private DataTable m_d = new System.Data.DataTable();

        public frmUploadTask()
        {
            InitializeComponent();
        }

        public void IniData(string fileName,string TaskName,DataTable d)
        {
            m_d = d;
            this.txtTask.Text = TaskName;
            this.txtTask.Tag = fileName;
            this.Class1.Items.Clear ();

            for (int i = 0; i < d.Rows.Count; i++)
            {
                if (d.Rows[i]["Fid"].ToString() != "0")
                {
                    this.Class1.Items.Add(d.Rows[i]["ClassName"].ToString());
                }
            }

            this.Class1.SelectedIndex = 0;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            this.cmdApply.Enabled = false;

            //上传文件
            string url = "http://www.netminer.cn/user/hander/upload.ashx";

            string cookie = string.Empty;
            string header = string.Empty;

            header = "User-Agent: Mozilla/5.0 (Windows NT 6.1; rv:20.0) Gecko/20100101 Firefox/20.0\r\n";
            header += "Accept-Language:zh-cn\r\n";
            header += "Accept-Encoding:gzip,deflate\r\n";
            header += "Connection: Keep-Alive";

            string rHeader = string.Empty;
            Dictionary<string, string> uParas = new Dictionary<string, string>();
            uParas.Add("mode", "UploadGRule");
            uParas.Add("u", Program.RegisterUser);
            uParas.Add("TaskName", this.txtTask.Text);
            uParas.Add("ClassName", this.Class1.Text);
            uParas.Add("Integral", this.udIntegral.Value.ToString ());
            uParas.Add("remark", this.txtDesc.Text);

            string htmlSource= UploadFile(url, header, this.txtTask.Tag.ToString(), uParas);
            htmlSource = htmlSource.Replace("\0", "");

            if (htmlSource.StartsWith("True"))
            {
                MessageBox.Show("采集任务上传成功！", "网络矿工 信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (htmlSource.StartsWith ( "0"))
            {
                MessageBox.Show("采集任务名称重复，请修改采集任务名称后重新上传！", "网络矿工 信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (htmlSource.StartsWith ( "False"))
            {
                MessageBox.Show("采集任务上传失败，请确定是否软件已经与官网账号已经绑定，并且上传文件有效！", "网络矿工 信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("采集任务上传失败,可能网路繁忙导致，请稍后再试！", "网络矿工 信息",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            this.cmdApply.Enabled = true;
            
        }

        private string UploadFile(string uUrl, string strHeader, string fName, Dictionary<string, string> uParas)
        {

            string boundaryValue = "";
            string webSource = "";

            try
            {

                string[] headers = Regex.Split(strHeader, "\r\n");
                string UploadHeader = "";

                //进行上传
                boundaryValue = "---------------------------7d" + GenerateRandomNumber(12);
                bool isContentTyp = false;
                for (int j = 0; j < headers.Length; j++)
                {
                    if (headers[j] != "")
                    {
                        if (headers[j].StartsWith("Content-Type", StringComparison.CurrentCultureIgnoreCase))
                        {
                            UploadHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                            isContentTyp = true;
                        }
                        else
                        {
                            UploadHeader += headers[j] + "\r\n";
                        }
                    }
                }
                if (isContentTyp == false)
                {
                    UploadHeader += "Content-Type: multipart/form-data; boundary=" + boundaryValue + "\r\n";
                }

                //开始处理上传的参数
                string pData = "";
                foreach (KeyValuePair<string, string> de in uParas)
                {
                    string label = de.Key.ToString();
                    string value = de.Value.ToString();
                    pData += "--" + boundaryValue + "\r\n";
                    pData += "Content-Disposition: form-data; name=\"" +  label + "\"\r\n";
                    pData += "\r\n";
                    pData += NetMiner.Common.ToolUtil.UrlEncode(value, NetMiner.Resource.cGlobalParas.WebCode.utf8) + "\r\n";
                    
                }

                pData += "--" + boundaryValue + "\r\n";
                pData += "Content-Disposition: form-data; name=\"FileData\"; filename=\"" + NetMiner.Common.ToolUtil.UrlEncode(Path.GetFileName(this.txtTask.Tag.ToString()), NetMiner.Resource.cGlobalParas.WebCode.utf8) + "\"\r\n";
                pData += "Content-Type: application/octet-stream";
                pData += "\r\n";
                pData += "\r\n";

                pData += "--" + boundaryValue + "--";

                FileInfo fInfo = new FileInfo(fName);
                UploadHeader += "Content-length: " + (fInfo.Length + pData.Length) + "\r\n\r\n";

                UploadHeader += pData;

                webSource =NetMiner.Common.HttpSocket.cHttpSocket.UploadFile(uUrl, UploadHeader,NetMiner.Resource.cGlobalParas.WebCode.NoCoding  , fName);

            }
            //捕获错误不处理
            catch { }

            return webSource;

        }

        public string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random();
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(10)]);
            }
            return newRandom.ToString();
        }

        private void frmUploadTask_Load(object sender, EventArgs e)
        {

        }

    }
}
