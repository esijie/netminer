using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;
using System.Threading;

namespace NetMiner.Core.Log
{
    public class cSystemLog:IDisposable
    {
        private string m_workPath = string.Empty;

        public cSystemLog(string workPath)
        {
            m_workPath = workPath;
        }

        ~cSystemLog()
        {
        }

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {

              
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion


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
            while (Err < countErr && isS == false)
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

            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);

            Log = TaskName + "," + DateTime.Now.ToString() + "," + (int)LogType + "," + strLog;

            sw.WriteLine(Log);

            sw.Close();
            sw.Dispose();
            myStream.Close();
            myStream.Dispose();

        }

        private void LoadSystemLog()
        {
            List<NetMiner.Core.Log.Entity.eLog> Logs = new List<Entity.eLog>();

            string lPath = this.m_workPath + NetMiner.Constant.g_LogPath;

            //ϵͳ��־Ϊ8λ�ļ����ģ��ҿ�������ת�����ڵ�
            DirectoryInfo di = new DirectoryInfo(lPath);
            FileInfo[] fis2 = di.GetFiles();   //�й�Ŀ¼�µ��ļ�   

            for (int i2 = 0; i2 < fis2.Length; i2++)
            {
                string fPath = System.IO.Path.GetFullPath(fis2[i2].FullName);

                string fileName = System.IO.Path.GetFileName(fis2[i2].FullName);

                string fName = fileName.Substring(0, fileName.IndexOf("."));
                string eName = System.IO.Path.GetExtension(fis2[i2].FullName);

                //��չ��һ��ΪCSV
                if (fName.Length == 8 && eName.EndsWith("csv", StringComparison.CurrentCultureIgnoreCase))
                {
                    try
                    {
                        DateTime d = DateTime.Parse(fName.Substring(0, 4) + "-" + fName.Substring(4, 2) + "-" + fName.Substring(6, 2));

                        Entity.eLog log = new Entity.eLog();

                        log.logFile = fName;
                        System.IO.FileInfo f = new FileInfo(fis2[i2].FullName);
                        log.LogSize = f.Length;
                        log.LogPath= fPath;

                        Logs.Add(log);

                    }
                    catch (System.Exception)
                    {

                    }

                }

            }

        }

    }
}
