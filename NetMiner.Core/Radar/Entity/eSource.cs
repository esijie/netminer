using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Core.Radar.Entity
{
    public class eSource
    {
        public eSource()
        {
        }

        ~eSource()
        {
        }

        private string m_TaskClass;
        public string TaskClass
        {
            get { return m_TaskClass; }
            set { m_TaskClass = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        //定义属性，用于监控时的计数
        private int m_GatheredCount;
        public int GatheredCount
        {
            get { return m_GatheredCount; }
            set { m_GatheredCount = value; }
        }

        private int m_Count;
        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

    }
}
