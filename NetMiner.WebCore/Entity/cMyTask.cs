using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.WebCore.Entity
{
    public class cMyTask
    {
        private int m_TID;
        public int TID
        {
            get { return m_TID; }
            set { m_TID = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private string m_SavePath;
        public string SavePath
        {
            get { return m_SavePath; }
            set { m_SavePath = value; }
        }

        private cGlobalParas.TaskState m_tState;
        public cGlobalParas.TaskState tState
        {
            get { return m_tState; }
            set { m_tState = value; }
        }

        //private string m_StartDate;
        //public string StateDate
        //{
        //    get { return m_StartDate; }
        //    set { m_StartDate = value; }
        //}

        private string m_EndDate;
        /// <summary>
        /// 最后一次运行完毕的时间
        /// </summary>
        public string EndDate
        {
            get { return m_EndDate; }
            set { m_EndDate = value; }
        }

        private int m_SplitState;
        public int SplitTask
        {
            get { return m_SplitState; }
            set { m_SplitState = value; }
        }

    }
}
