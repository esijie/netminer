using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data ;
using NetMiner.Common;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Resource;

///���ܣ��ɼ������������
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.Entity
{
    #region cTaskDataList �����

    /// <summary>
    /// ����������ݣ���TaskRunʵ��ӳ��
    /// </summary>
    public class cTaskDataList
    {
        private int m_TaskCount ;

        public cTaskDataList()
        {
            m_TaskDataList = new List<cTaskData>();
        }

        //��ʼ���������Ĳɼ�������Ϣ.
        public void LoadTaskRunData(string workPath)
        {
            oTaskRun t = new oTaskRun(workPath);
            IEnumerable<eTaskRun> ers= t.LoadTaskRunData();
            cTaskData tData;
            foreach(eTaskRun er in ers)
            {
                tData = new cTaskData();
                tData.TaskID = er.TaskID;
                tData.TaskName = er.TaskName;
                tData.TaskClassPath = er.TaskClassPath;
                tData.TaskClass = er.TaskClass;
                tData.TaskType = er.TaskType;
                tData.RunType = er.TaskRunType;
                tData.PublishType = er.PublishType;
                tData.TempDataFile = er.TempFile;
                if (er.TaskState == cGlobalParas.TaskState.Running || er.TaskState==cGlobalParas.TaskState.Started)
                    tData.TaskState = cGlobalParas.TaskState.ErrStop;
                else
                    tData.TaskState = er.TaskState;
                tData.ThreadCount = er.ThreadCount;
                tData.UrlCount = er.UrlCount;
                tData.UrlNaviCount = er.UrlNaviCount;
                tData.GatheredUrlCount = 0;
                tData.GatheredUrlNaviCount = er.GatheredUrlNaviCount;
                tData.GatherErrUrlCount = 0;
                tData.GatherDataCount = er.RowsCount;
                tData.StartTimer = er.StartDateTime;

                tData.GatheredErrUrlNaviCount = er.ErrUrlNaviCount; 
                m_TaskDataList.Add(tData);
                tData = null;
            }

            m_TaskCount = t.GetCount();
            t = null;
        }

        public int TaskCount
        {
            get { return m_TaskCount; }
        }

        public cTaskDataList(string FileName)
        {
            m_TaskDataList = new List<cTaskData>();
        }

        private List<cTaskData> m_TaskDataList;

        public List<cTaskData> TaskDataList
        {
            get { return m_TaskDataList; }
            set { m_TaskDataList = value; }
        }

        ~cTaskDataList()
        {

        }

    }

    #endregion

   

}
