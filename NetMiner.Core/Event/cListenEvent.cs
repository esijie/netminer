using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.Plan.Entity;

namespace NetMiner.Core.Event
{
    //提醒图标，用于显示托盘图标的信息
    public class ShowInfoEventArgs : EventArgs
    {
        public ShowInfoEventArgs(string Title, string strInfo)
        {
            m_Title = Title;
            m_strInfo = strInfo;
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_strInfo;
        public string strInfo
        {
            get { return m_strInfo; }
            set { m_strInfo = value; }
        }
    }

    //任务初始化事件
    public class cListenInitializedEventArgs : EventArgs
    {
        public cListenInitializedEventArgs()
        {
        }

        public cListenInitializedEventArgs(cGlobalParas.MessageType MessType)
        {
            m_MessType = MessType;
        }

        private cGlobalParas.MessageType m_MessType;
        public cGlobalParas.MessageType MessType
        {
            get { return m_MessType; }
            set { m_MessType = value; }
        }

    }

    public class cCommandEventArgs : cListenInitializedEventArgs
    {
        public cCommandEventArgs(cGlobalParas.MessageType MessType)
        {
            base.MessType = MessType;
        }
    }

    //运行任务触发事件
    public class cRunTaskEventArgs : cListenInitializedEventArgs
    {
    

        public cRunTaskEventArgs(cGlobalParas.MessageType MessType ,string RunName ,string RunPara)
        {
            base.MessType = MessType;
            m_RunName = RunName;
            m_RunPara = RunPara;
        }

        private string m_RunName;
        public string RunName
        {
            get { return m_RunName; }
            set { m_RunName = value; }
        }

        private string m_RunPara;
        public string RunPara
        {
            get { return m_RunPara; }
            set { m_RunPara = value; }
        }
    }

    //触发新增运行任务事件
    public class cAddRunTaskEventArgs : EventArgs
    {
        public cAddRunTaskEventArgs()
        {
        }

        public cAddRunTaskEventArgs(eTaskPlan RTask)
        {
            m_RunTask = RTask;
        }

        private eTaskPlan m_RunTask;
        public eTaskPlan RunTask
        {
            get { return m_RunTask; }
            set { m_RunTask = value; }
        }
    }

    //监听失败事件
    public class cListenErrorEventArgs : EventArgs
    {
        public cListenErrorEventArgs()
        {
        }

        public cListenErrorEventArgs(string Mess)
        {
            m_Message = Mess;
        }

        private string m_Message;
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }

    }


    //运行任务日志事件
    public class cRunTaskLogArgs : EventArgs
    {
        public cRunTaskLogArgs(string TaskName, cGlobalParas.LogType LogType, string strLog, bool IsSaveError)
        {
            m_LogType = LogType;
            m_strLog = strLog;
            m_IsSaveErrorLog = IsSaveError;
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private string m_strLog;
        public string strLog
        {
            get { return m_strLog; }
            set { m_strLog = value; }
        }

        private bool m_IsSaveErrorLog;
        public bool IsSaveErrorLog
        {
            get { return m_IsSaveErrorLog; }
            set { m_IsSaveErrorLog = value; }
        }

        private cGlobalParas.LogType m_LogType;
        public cGlobalParas.LogType LogType
        {
            get { return m_LogType; }
            set { m_LogType = value; }
        }

    }
}
