using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Resource;

namespace NetMiner.Core.Event
{
    //雷达事件
    public class cRadarEventArgs : EventArgs
    {

        public cRadarEventArgs()
        {

        }

        /// <param name="cancel">是否取消事件</param>
        public cRadarEventArgs(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }

    }

    public class cRadarStartedArgs : cRadarEventArgs
    {
        public cRadarStartedArgs()
        {

        }
    }

    public class cRadarStopArgs : cRadarEventArgs
    {
        public cRadarStopArgs()
        {

        }
    }

    public class cRadarLogArgs : cRadarEventArgs
    {
        public cRadarLogArgs(cGlobalParas.LogType LogType, string strLog, bool IsSaveError)
        {
            m_LogType = LogType;
            m_strLog = strLog;
            m_IsSaveErrorLog = IsSaveError;
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

    public class cRadarCountArgs : cRadarEventArgs
    {
        public cRadarCountArgs(string RadarName, int RCount, int GatheredCount, int Count)
        {
            m_RadarName = RadarName;
            m_RCount = RCount;
            m_GatheredCount = GatheredCount;
            m_Count = Count;
        }

        private string m_RadarName;
        public string RadarName
        {
            get { return m_RadarName; }
            set { m_RadarName = value; }
        }
        private int m_RCount;
        public int RCount
        {
            get { return m_RCount; }
            set { m_RCount = value; }
        }


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

    public class cRadarStateArgs : cRadarEventArgs
    {
        public cRadarStateArgs(string RadarName,cGlobalParas.MonitorState RState)
        {
            m_RName = RadarName;
            m_RState = RState;
        }

        private string m_RName;
        public string RName
        {
            get { return m_RName; }
            set { m_RName = value; }
        }

        private cGlobalParas.MonitorState m_RState;
        public cGlobalParas.MonitorState RState
        {
            get { return m_RState; }
            set { m_RState = value; }
        }
    }

    public class cRadarMonitorWaringArgs : cRadarEventArgs 
    {
        public cRadarMonitorWaringArgs(cGlobalParas.WarningType wType,string strWarning)
        {
            m_wType = wType;
            m_strWarning = strWarning;
        }

        private cGlobalParas.WarningType m_wType;
        public cGlobalParas.WarningType wType
        {
            get { return m_wType; }
            set { m_wType = value; }
        }

        private string m_strWarning;
        public string strWarning
        {
            get { return m_strWarning; }
            set { m_strWarning = value; }
        }

    }
}
