using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;

namespace NetMiner.Gather.Log
{
    public class cSystemLog
    {
        private string m_workPath = string.Empty;

        public cSystemLog(string workPath)
        {
            m_workPath = workPath;
        }

        ~cSystemLog()
        {
        }

        public void WriteLog(cGlobalParas.LogType lType,cGlobalParas.LogClass lClass,string DTime, string strLog)
        {
            string FileName = m_workPath + "Log\\" + DateTime.Now.Year;
            if (DateTime.Now.Month.ToString().Length == 1)
                FileName = FileName + "0" + DateTime.Now.Month.ToString();
            else
                FileName = FileName + DateTime.Now.Month.ToString();

            if (DateTime.Now.Day.ToString ().Length ==1)
                FileName = FileName + "0" + DateTime.Now.Day.ToString();
            else
                FileName = FileName + DateTime.Now.Day.ToString();

            FileName =FileName + ".csv";
            

            if (!Directory.Exists(Path.GetDirectoryName(FileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(FileName));

            FileStream myStream = File.Open(FileName, FileMode.Append, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream,System.Text.Encoding.UTF8);

            string ss = "";
            ss = (int)lType + "," + (int)lClass + "," + DTime + "," + strLog;

            sw.WriteLine(ss);
            
            sw.Close();
            myStream.Close();



        }
    }
}
