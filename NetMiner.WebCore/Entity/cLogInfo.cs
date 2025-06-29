using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.WebCore.Entity
{
    public class cLogInfo
    {
        private int m_ID;
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
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

        private cGlobalParas.LogType m_lType;
        public cGlobalParas.LogType lType
        {
            get { return m_lType; }
            set { m_lType = value; }
        }

        private string m_strLog;
        public string strLog
        {
            get { return m_strLog; }
            set { m_strLog = value; }
        }

        private string m_LogDate;
        public string LogDate
        {
            get { return m_LogDate; }
            set { m_LogDate = value; }
        }
    }
}
