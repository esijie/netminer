using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Resource;

namespace NetMiner.Core.Event
{
    #region ���巢���¼�
    //�����¼�
    public class cPublishEventArgs : EventArgs
    {
        public cPublishEventArgs()
        {

        }
    
        /// <param name="cancel">�Ƿ�ȡ���¼�</param>
        public cPublishEventArgs(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// �Ƿ�ȡ���¼�
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

    //����״̬�ı��¼�
    public class PublishStartedEventArgs : cPublishEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="old_state">�ɵ�״̬</param>
        /// <param name="new_statue">�µ�״̬</param>
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
        /// <param name="old_state">�ɵ�״̬</param>
        /// <param name="new_statue">�µ�״̬</param>
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
    /// ������ɴ����¼�
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
        /// �ɼ�����ı��
        /// </summary>
        public Int64 TaskID
        {
            get { return m_TaskID; }
            set { m_TaskID = value; }
        }

        private string m_TaskName;
        /// <summary>
        /// �ɼ����������
        /// </summary>
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private bool m_IsDel;
        /// <summary>
        /// �Ƿ�ɾ����ʱ�ļ�
        /// </summary>
        public bool IsDel
        {
            get { return m_IsDel; }
            set { m_IsDel = value; }
        }

        private string m_TmpFileName;
        /// <summary>
        /// ��ʱ�ļ������ƣ�ע�⣬�������ʱ�ļ�����ָ�ɼ�������������ʱ�ļ��ĵ�ַ�������Ĭ�ϵĵ�ַ�����ٴ�Ϊ��
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
        /// <param name="old_state">�ɵ�״̬</param>
        /// <param name="new_statue">�µ�״̬</param>
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


    //������־�¼�
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

    //�����¼� ������̨�߳���ɵ����
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

    //����һ���¼�������淵�س��������
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

    //����һ���¼�������web�������Դ��
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

    //����һ����Ϣ�¼�����������淴Ӧ��̨�߳�ִ�е�Ч��
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
    /// ��ʱ��ר���ڸ��·���״̬�����ڲɼ����ݣ��������������
    /// ʱ��Ч�����������з����������Ч
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
