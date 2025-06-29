using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///功能：运行任务 具体任务信息类
///完成时间：2009-8-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Gather.Plan
{
    public class cTaskPlan
    {
    
        public cTaskPlan()
        {
            
        }

        ~cTaskPlan()
        {
        }

        #region 此两个属性给记录日志使用，其他情况均不使用
        private string m_PlanID;
        public string PlanID
        {
            get { return m_PlanID; }
            set { m_PlanID = value; }
        }

        private string m_PlanName;
        public string PlanName
        {
            get { return m_PlanName; }
            set { m_PlanName = value; }
        }

        #endregion

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
