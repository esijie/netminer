using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;
using System.Threading;

namespace NetMiner.Engine.ClientEngine
{
    public class cSplitRunningInfo
    {
        public cSplitRunningInfo()
        {
            m_Logs=new StringBuilder ();
            m_dt = new DataTable();
        }

        ~cSplitRunningInfo()
        {
            m_Logs = null;
            m_dt = null;
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private StringBuilder m_Logs;
        public string Logs
        {
            get { return m_Logs.ToString(); }
            set { m_Logs.Append(value); }
        }

        private DataTable m_dt;
        public DataTable dt
        {
            get { return m_dt; }
            set 
            { 
                DataTable tmp = new DataTable();
                tmp = value;
                Monitor.Enter(m_dt);
                try
                {
                    if (tmp != null)
                    {
                        m_dt.Merge(tmp);
                    }
                }
                catch (System.Exception)
                {
                    //仅捕获错误，不做任何处理，只要保障不跳出程序即可
                }
                finally
                {
                    Monitor.Exit(m_dt);
                }
            }
        }

   
    }
}
