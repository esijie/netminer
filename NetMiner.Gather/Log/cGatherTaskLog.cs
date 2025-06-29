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
            //判断TaskName的日志文件是否存在，文件命名为：任务名和日期名，每个任务
            //每日为一个日志文件
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
                    //如果出错，则表示此文件有可能被占用
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
