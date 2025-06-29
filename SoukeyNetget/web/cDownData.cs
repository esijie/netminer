using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Data;
using System.Windows.Forms;
using NetMiner.Common;

namespace MinerSpider.web
{
    public class cDownData
    {
        ContainerControl m_sender = null;
        Delegate m_senderDelegate = null;
        private string m_FileName;
        private string m_TaskName;
        private string m_tabName;

        public cDownData(ContainerControl sender, Delegate senderDelegate, string TaskName, string FileName,string tabName)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_TaskName  = TaskName ;
            m_FileName = FileName;
            m_tabName = tabName;
        }

        ~cDownData ()
        {
        }


        public void RunProcess()
        {
            Thread.CurrentThread.IsBackground = true; //make them a daemon
            ThreadRunFunction();
        }

        private void ThreadRunFunction()
        {
            string TaskName = this.m_TaskName;
            string FileName = this.m_FileName;

            m_sender.BeginInvoke(m_senderDelegate, new object[] {this.m_tabName , "开始下载数据，请等待......", false });

            //localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            //sweb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
            //if (Program.g_IsAuthen == true)
            //    sweb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            //try
            //{
            //    //永远取最后的100条数据，刷新日志窗口
            //    byte[] buffer = sweb.LoadData(TaskName, 0);
            //    buffer = ToolUtil.Decompress(buffer);

            //    MemoryStream ms = new MemoryStream(buffer);
            //    BinaryFormatter bf = new BinaryFormatter();
            //    Object o = bf.Deserialize(ms);
            //    DataSet ds = (DataSet)o;

            //    if (ds == null || ds.Tables.Count == 0)
            //    {
            //        m_sender.BeginInvoke(m_senderDelegate, new object[] { this.m_tabName, "没有采集的数据", true });
            //        return;
            //    }

            //    DataTable d = ds.Tables[0];
            //    m_sender.BeginInvoke(m_senderDelegate, new object[] { this.m_tabName, "开始将数据导出到csv文件，请等待", false });

            //    //将datata保存到csv文件
            //    ExportCSV(FileName, d);

            //    m_sender.BeginInvoke(m_senderDelegate, new object[] { this.m_tabName, "数据下载并成功导出！", true });

            //}
            //catch
            //{
            //    return;
            //}
        }

        public void ExportCSV(string FileName, DataTable d)
        {
            FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                //写标题 
                for (i = 0; i < d.Columns.Count; i++)
                {
                    str += d.Columns[i].ColumnName;
                    str += ",";
                }

                //去掉最后一个分隔符
                str = str.Substring(0, str.Length - 1);

                sw.WriteLine(str);

                Count = d.Rows.Count;
                //写内容 
                for (i = 0; i < d.Rows.Count; i++)
                {
                    for (int j = 0; j < d.Columns.Count; j++)
                    {
                        tempStr += d.Rows[i][j];
                        tempStr += ",";
                    }

                    //去掉最后一个分隔符
                    tempStr = tempStr.Substring(0, tempStr.Length - 1);

                    sw.WriteLine(tempStr);
                    tempStr = "";

                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                sw.Close();
                myStream.Close();

            }

            return;
        }

    }
}
