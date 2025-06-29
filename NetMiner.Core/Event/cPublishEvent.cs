using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Resource;

namespace NetMiner.Core.Event
{
    #region 定义发布事件
    //任务事件
    public class cPublishEventArgs : EventArgs
    {
        public cPublishEventArgs()
        {

        }
    
        /// <param name="cancel">是否取消事件</param>
        public cPublishEventArgs(bool cancel)
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

    public class PublishErrorEventArgs : cPublishEventArgs
    {
        public PublishErrorEventArgs(Int64 TaskID, string TaskName, Exception error)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
            m_Error = error;
        }

        private Exception m_Error;
 
        public Exception Error
        {
            get { return m_Error; }
            set { m_Error = value; }
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
    }

    //任务状态改变事件
    public class PublishStartedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishStartedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;

            m_TaskName = TaskName;
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
        
    }

    public class PublishStopEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishStopEventArgs(Int64 TaskID, string TaskName,DataTable errData)
        {
            m_TaskID = TaskID;
            m_d = errData;
            m_TaskName = TaskName;
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

        private DataTable m_d;
        public DataTable d
        {
            get { return m_d; }
            set { m_d = value; }
        }

    }

    /// <summary>
    /// 发布完成触发事件
    /// </summary>
    public class PublishCompletedEventArgs : cPublishEventArgs
    {
        
        public PublishCompletedEventArgs(Int64 TaskID, string TaskName,bool isDelData,string tmpFile)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
           
            m_IsDel = isDelData;
            m_TmpFileName = tmpFile;
        }

        private Int64 m_TaskID;
        /// <summary>
        /// 采集任务的编号
        /// </summary>
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        /// <summary>
        /// 采集任务的名称
        /// </summary>
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private bool m_IsDel;
        /// <summary>
        /// 是否删除临时文件
        /// </summary>
        public bool IsDel
        {
            get { return m_IsDel; }
            set { m_IsDel = value; }
        }

        private string m_TmpFileName;
        /// <summary>
        /// 临时文件的名称，注意，这里的临时文件是特指采集任务甚至了临时文件的地址，如果是默认的地址，则再次为空
        /// </summary>
        public string TmpFileName
        {
            get { return m_TmpFileName; }
            set { m_TmpFileName = value; }
        }
    }

    public class PublishFailedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">旧的状态</param>
        /// <param name="new_statue">新的状态</param>
        public PublishFailedEventArgs(Int64 TaskID, string TaskName)
        {
            m_TaskID = TaskID;
            m_TaskName = TaskName;
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

    }


    //发布日志事件
    public class PublishLogEventArgs : cPublishEventArgs
    {
        public PublishLogEventArgs(Int64 TaskID, cGlobalParas.LogType LogType, string strLog,bool IsSaveError)
        {
            m_TaskID = TaskID;
            m_strLog = strLog;
            m_LogType = LogType;
            m_IsSaveErr = IsSaveError;
        }

        private cGlobalParas.LogType m_LogType;
        public cGlobalParas.LogType LogType
        {
            get { return m_LogType; }
            set { m_LogType = value; }
        }

        private bool m_IsSaveErr;
        public bool IsSaveErr
        {
            get { return m_IsSaveErr; }
            set { m_IsSaveErr = value; }
        }

        private string m_strLog;
        public string strLog
        {
            get { return m_strLog; }
            set { m_strLog = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }
    }

    //计数事件 反馈后台线程完成的情况
    public class DoCountEventArgs : cPublishEventArgs
    {
        public DoCountEventArgs(int Count, int DoneCount, int ErrCount, int CurRowIndex)
        {
            m_Count = Count;
            m_DoneCount = DoneCount;
            m_ErrCount = ErrCount;
            m_CurRowIndex = CurRowIndex;
        }

        private int m_Count;
        public int Count
        {
            get { return m_Count; }
            set { m_Count = value; }
        }

        private int m_DoneCount;
        public int DoneCount
        {
            get { return m_DoneCount; }
            set { m_DoneCount = value; }
        }

        private int m_ErrCount;
        public int ErrCount
        {
            get { return m_ErrCount; }
            set { m_ErrCount = value; }
        }

        private int m_CurRowIndex;
        public int CurRowIndex
        {
            get { return m_CurRowIndex; }
            set { m_CurRowIndex = value; }
        }
    }

    //定义一个事件，想界面返回出错的数据
    public class PublishErrDataEventArgs : cPublishEventArgs
    {
        public PublishErrDataEventArgs(DataTable ErrData)
        {
            m_ErrData = new DataTable();
            m_ErrData = ErrData;
        }

        ~PublishErrDataEventArgs()
        {
            m_ErrData = null;
        }

        private DataTable m_ErrData;
        public DataTable ErrData
        {
            get { return m_ErrData; }
            set { m_ErrData = value; }
        }

    }

    //定义一个事件，返回web发布后的源码
    public class PublishSourceEventArgs : cPublishEventArgs
    {
        public PublishSourceEventArgs(string HtmlSource)
        {
            m_HtmlSource = HtmlSource;
        }

        ~PublishSourceEventArgs()
        {

        }

        private string m_HtmlSource;
        public string HtmlSource
        {
            get { return m_HtmlSource; }
            set { m_HtmlSource = value; }
        }

    }

    //定义一个消息事件，用于向界面反应后台线程执行的效率
    public class RunTimeEventArgs : cPublishEventArgs
    {
        public RunTimeEventArgs(string str)
        {
            m_str = str;
        }

        private string m_str;
        public string str
        {
            get { return m_str; }
            set { m_str = value; }
        }
    }

    /// <summary>
    /// 此时间专用于更新发布状态，仅在采集数据，并且输出到界面
    /// 时有效，其他的所有发布情况均无效
    /// </summary>
    public class UpdateStateArgs : cPublishEventArgs
    {
        public UpdateStateArgs(object[] row, Int64 TaskID, cGlobalParas.PublishResult isPublishSucceed)
        {
            m_Row = row;
            m_TaskID = TaskID;
            m_isPublishSucceed = isPublishSucceed;
        }

        private object[] m_Row;
        public object[] Row
        {
            get { return m_Row; }
            set { m_Row = value; }
        }

        private Int64 m_TaskID;
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private cGlobalParas.PublishResult m_isPublishSucceed;
        public cGlobalParas.PublishResult  isPublishSucceed
        {
            get { return m_isPublishSucceed; }
            set { m_isPublishSucceed = value; }
        }
    }
    #endregion
}
