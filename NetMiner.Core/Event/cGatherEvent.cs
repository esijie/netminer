using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Common;
using NetMiner.Resource;

///���ܣ� ����ɼ���������¼�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.Event
{

        #region ����ɼ�������ص��¼�

        //�����ʼ���¼�
        public class TaskInitializedEventArgs : EventArgs
        {

            public TaskInitializedEventArgs(Int64 TaskID)
            {
                m_TaskID = TaskID;
            }

            private Int64 m_TaskID;
            public Int64 TaskID
            {
                get { return m_TaskID; }
                set { m_TaskID = value; }
            }
        }

        //�����̴߳�����Ӧ�¼�
        public class TaskThreadErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="error">������쳣</param>
            public TaskThreadErrorEventArgs(Exception error)
            {
                m_Error = error;
            }

            private Exception m_Error;
            /// <summary>
            /// ������Ϣ
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
        }

        //���������Ӧ�¼�
        public class TaskErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="dtc">��������ķֿ�</param>
            /// <param name="error">������쳣</param>
            //public TaskErrorEventArgs(Int64 TaskID,string taskName, cGatherTaskSplit dtc, Exception error)
            public TaskErrorEventArgs(Int64 TaskID, string taskName,  Exception error)
            {
                m_TaskID = TaskID;
                m_Error = error;
                //m_ErrorThread = dtc;
                m_TaskName = taskName;
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

            private Exception m_Error;
            //private cGatherTaskSplit m_ErrorThread;
            /// <summary>
            /// ������Ϣ
            /// </summary>
            public Exception Error
            {
                get { return m_Error; }
                set { m_Error = value; }
            }
            /// <summary>
            /// ��������ķֿ�
            /// </summary>
            //public cGatherTaskSplit ErrorThread
            //{
            //    get { return m_ErrorThread; }
            //    set { m_ErrorThread = value; }
            //}
        }

        //�ɼ�����״̬�ı��¼�
        public class TaskStateChangedEventArgs : cTaskEventArgs
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="old_state">�ɵ�״̬</param>
            /// <param name="new_statue">�µ�״̬</param>
            public TaskStateChangedEventArgs(Int64 TaskID, cGlobalParas.TaskState oldState, cGlobalParas.TaskState newState)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                m_OldState = oldState;
                m_NewState = newState;
            }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}

            private cGlobalParas.TaskState m_OldState;
            private cGlobalParas.TaskState m_NewState;
            /// <summary>
            /// �ɵ�״̬
            /// </summary>
            public cGlobalParas.TaskState OldState
            {
                get { return m_OldState; }
                set { m_OldState = value; }
            }
            /// <summary>
            /// �µ�״̬
            /// </summary>
            public cGlobalParas.TaskState NewState
            {
                get { return m_NewState; }
                set { m_NewState = value; }
            }
        }

        //�����¼�
        public class cTaskEventArgs : EventArgs
        {

            public cTaskEventArgs()
            {

            }

            /// <param name="cancel">�Ƿ�ȡ���¼�</param>
            public cTaskEventArgs(Int64 TaskID,string TaskName, bool cancel)
            {
                m_TaskID = TaskID;
                m_TaskName = TaskName;
                m_Cancel = cancel;
             
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

        //�ɼ�������� �¼� 
        //�����ÿ��ַ�ɼ�������ɴ���
        public class cGatherDataEventArgs : cTaskEventArgs
        {
            public cGatherDataEventArgs(Int64 TaskID,string TaskName, DataTable cData)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                base.TaskName = TaskName;
                m_gData = cData;
            }

            public cGatherDataEventArgs(Int64 TaskID, string TaskName, DataTable cData,bool isPublish)
            {
                base.TaskID = TaskID;
                base.TaskName = TaskName;
                if (isPublish == true)
                {
                    DataColumn col = new DataColumn();
                    col.ColumnName = "isPublished";
                    m_gData = cData;
                    if (m_gData.Columns[m_gData.Columns.Count - 1].ColumnName != "isPublished")
                    {
                        m_gData.Columns.Add(col);
                        for (int i = 0; i < m_gData.Rows.Count; i++)
                        {
                            m_gData.Rows[i][m_gData.Columns.Count - 1] = cGlobalParas.PublishResult.UnPublished.ToString();
                        }
                    }
                }
                else
                    m_gData = cData;
            }

            private DataTable m_gData;
            public DataTable gData
            {
                get { return m_gData; }
                set { m_gData = value; }
            }

        private int m_UrlCount;
        public int UrlCount
        {
            get
            {
                return m_UrlCount;
            }
            set { m_UrlCount = value; }
        }

            //private Int64 m_TaskID;
            //public Int64 TaskID
            //{
            //    get { return m_TaskID; }
            //    set { m_TaskID = value; }
            //}
        }

        //�ɼ���־�¼�
        public class cGatherTaskLogArgs : cTaskEventArgs
        {
            public cGatherTaskLogArgs(Int64 TaskID,string TaskName, cGlobalParas.LogType LogType, string strLog ,bool IsSaveError)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                base.TaskName = TaskName;
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

        //�ɼ������������¼�
        public class cGatherUrlCountArgs : cTaskEventArgs
        {
            public cGatherUrlCountArgs(Int64 TaskID, cGlobalParas.UpdateUrlCountType uType,int UrlCount)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                m_UrlCount = UrlCount;
                m_uType = uType;
            }

            private int m_UrlCount;
            public int UrlCount
            {
                    get { return m_UrlCount; }
                    set { m_UrlCount = value; }
            }

            private cGlobalParas.UpdateUrlCountType m_uType;
            public cGlobalParas.UpdateUrlCountType uType
            {
                    get { return m_uType; }
                    set { m_uType = value; }
            }
        
        }

    public class cGatherUrlCounterArgs : cTaskEventArgs
    {
        public cGatherUrlCounterArgs(Int64 TaskID, int UrlCount,int GUrlCount,int ErrCount)
        {
            //m_TaskID = TaskID;
            base.TaskID = TaskID;
            m_UrlCount = UrlCount;
            m_GUrlCount = GUrlCount;
            m_ErrCount = ErrCount;
        }

        private int m_UrlCount;
        public int UrlCount
        {
            get { return m_UrlCount; }
            set { m_UrlCount = value; }
        }

        private int m_GUrlCount;
        public int GUrlCount
        {
            get { return m_GUrlCount; }
            set { m_GUrlCount = value; }
        }
        private int m_ErrCount;
        public int ErrCount
        {
            get { return m_ErrCount; }
            set { m_ErrCount = value; }
        }

    }

        //����Cookie
        public class cUpdateCookieArgs : cTaskEventArgs
        {
            public cUpdateCookieArgs(int index,string newCookie)
            {
                m_index = index;
                m_NewCookie = newCookie;
            }

            private int m_index;
            public int Index
            {
                get { return m_index; }
                set { m_index = value; }
            }

            private string m_NewCookie;
            public string newCookie
            {
                get { return m_NewCookie; }
                set { m_NewCookie = value; }
            }

          
        }

        /// <summary>
        /// �����ֲ�ʽ����ͻ���������Ϣ
        /// </summary>
        public class cPublishClientInfoArgs : cTaskEventArgs
        {
        }

        public class cDownFileArgs:cTaskEventArgs
        {
            public cDownFileArgs(Int64 TaskID, string TaskName, string fileUrl)
            {
                //m_TaskID = TaskID;
                base.TaskID = TaskID;
                base.TaskName = TaskName;
                m_fileUrl = fileUrl;
            }

            private string m_fileUrl;
            public string fileUrl
            {
                get { return m_fileUrl; }
                set { m_fileUrl = value; }
            }

        }
    #endregion

}
