using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eTriggerTask
    {

        private cGlobalParas.RunTaskType m_RunTaskType;
        public cGlobalParas.RunTaskType RunTaskType
        {
            get { return m_RunTaskType; }
            set { m_RunTaskType = value; }
        }

        private string m_RunTaskName;
        public string RunTaskName
        {
            get { return m_RunTaskName; }
            set { m_RunTaskName = value; }
        }

        private string m_RunTaskPara;
        public string RunTaskPara
        {
            get { return m_RunTaskPara; }
            set { m_RunTaskPara = value; }
        }
    }
}
