using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;
using System.Threading;

namespace NetMiner.Gather.Log
{
    public class cGatherTaskLog
    {
        private string m_workPath = string.Empty;
        public cGatherTaskLog(string workPath)
        {
            this.m_workPath = workPath;
        }

        public cGatherTaskLog(string workPath,string TaskName)
        {
            this.m_workPath = workPath;
            //�ж�TaskName����־�ļ��Ƿ���ڣ��ļ�����Ϊ������������������ÿ������
            //ÿ��Ϊһ����־�ļ�
            string FileName = m_workPath + "Log\\" + TaskName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".csv";

        }

        ~cGatherTaskLog()
        {

        }

        public void WriteLog(string TaskName, cGlobalParas.LogType LogType, string strLog)
        {

            string FileName = m_workPath + "Log\\task" + TaskName + ".csv";
            string Log = "";

            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            FileStream myStream = null;
            int countErr = 3;
            int Err = 0;

            bool isS = false;
            while (Err < countErr && isS ==false )
            {
                try
                {
                    myStream = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Write);
                    isS = true;
                }
                catch (System.Exception)
                {
                    //����������ʾ���ļ��п��ܱ�ռ��
                    Thread.Sleep(200);
                    Err++;
                }
            }

            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));

            Log = TaskName + "," + DateTime.Now.ToString() + "," + (int)LogType + "," + strLog;
                   
            sw.WriteLine(Log);

            sw.Close();
            sw.Dispose();
            myStream.Close();
            myStream.Dispose();

        }
    }
}
