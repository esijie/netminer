using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.WebCore.Entity
{
    public class cTaskList
    {
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

        private cGlobalParas.TaskState m_State;
        public cGlobalParas.TaskState State
        {
            get { return m_State; }
            set { m_State = value; }
        }

        private string m_StartDate;
        public string StartDate
        {
            get { return m_StartDate; }
            set { m_StartDate = value; }
        }

        private string m_EndDate;
        public string EndDate
        {
            get { return m_EndDate; }
            set { m_EndDate = value; }
        }

        private int m_gCount;
        public int gCount
        {
            get { return m_gCount; }
            set { m_gCount = value; }
        }

        private int m_ErrCount;
        public int ErrCount
        {
            get { return m_ErrCount; }
            set { m_ErrCount = value; }
        }

        private int m_DoUID;
        public int DoUID
        {
            get { return m_DoUID; }
            set { m_DoUID = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }
    }
}
