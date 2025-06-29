using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Core.Event;

///���ܣ������������ �������� ��Ӧ�¼� �˹���ʵ�ֵĺܼ�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ����Ҫ���Ʒ�������ģ�飬�Ʊش˹���Ҫ��������
///˵������ 
///�汾��01.10.00
///�޶�����
///2013-2-26 �������������кϲ����ɼ�����ķ��������ݼӹ����������еķ���
///ͬʱȡ������������
namespace NetMiner.Publish
{
    public class cPublishManage
    {
        List<cPublish> m_ListPublish;
        private  cEventProxy m_EventProxy;

        public cPublishManage()
        {
            m_ListPublish = new List<cPublish>();
            m_EventProxy = new cEventProxy();
        }

        ~cPublishManage()
        {
        }

        public List<cPublish> ListPublish
        {
            get { return m_ListPublish; }
        }


        public void AddPublishingTask(cPublish pt)
        {
            //��ӵ�������
            ListPublish.Add(pt);
            TaskInit(pt);
        }

        public cPublish FindTask(Int64 TaskID)
        {
            for (int i = 0; i < ListPublish.Count; i++)
            {
                if (ListPublish[i].TaskID == TaskID)
                    return ListPublish[i];
            }
            return null;
        }

        public void Remove(cPublish pTask)
        {
            if (pTask != null)
            {
                if (pTask.PublishManage.Equals(this))
                {
                    pTask.PublishCompleted -= this.onPublishCompleted;
                    pTask.PublishFailed -= this.onPublishFailed;
                    pTask.PublishStarted -= this.onPublishStarted;
                    pTask.PublishError -= this.onPublishError;
                    pTask.PublishLog -= this.onPublishLog;
                    pTask.RuntimeInfo -= this.onRunTimeInfo;
                    pTask.DoCount -= this.onDoCount;
                    pTask.PublishErrorData -= this.onPublishErrData;
                    pTask.PublishSource -= this.onPublishSource;
                    pTask.PublishStop -= this.onPublishStop;
                    pTask.UpdateState -= this.onUpdateState;
                }

                ListPublish.Remove(pTask);
            }
        }

        public void AddPublishTask(cPublish pt)
        {
            //��ӵ�������
            ListPublish.Add(pt);
            TaskInit(pt);

            //����������
            pt.startPublic();
        }

        //public void AddSaveTempDataTask(cPublish pt)
        //{
        //    ListPublish.Add(pt);
        //    TaskTempSaveInit(pt);

        //    //����������
        //    pt.startSaveTempData();
        //}

        /// <summary>
        /// ����ǰ���ڽ��еķ�������ȫ��ֹͣ
        /// </summary>
        public void StopPublish()
        {
            foreach (cPublish p in ListPublish)
            {
                p.StopPublish();
            }
        }

        public void StopPublish(cPublish pt)
        {
            pt.StopPublish();
        }

        public void OverPublish(cPublish pt)
        {
            pt.OverPublish();
        }

        public void Abort()
        {

            foreach (cPublish p in ListPublish)
            {
                p.PublishCompleted -= this.onPublishCompleted;
                p.PublishFailed -= this.onPublishFailed;
                p.PublishStarted -= this.onPublishStarted;
                p.PublishError -= this.onPublishError;
                p.PublishLog -= this.onPublishLog;
                p.RuntimeInfo -= this.onRunTimeInfo;
                p.DoCount -= this.onDoCount;
                p.PublishErrorData -= this.onPublishErrData;
                p.PublishSource -= this.onPublishSource;
                p.PublishStop -= this.onPublishStop;
                p.UpdateState -= this.onUpdateState;
                p.Abort();
            }
        }

        //ע����ʱ�洢������¼���ϵͳ�Զ�ִ�У������û���Ԥ
        private void TaskTempSaveInit(cPublish pTask)
        {
            if (pTask.PublishManage.Equals(this))
            {
                //pTask.PublishTempDataCompleted += this.onPublishTempDataCompleted;
                pTask.PublishError += this.onPublishError;
            }
        }

        //ע�ᷢ��������¼�
        private void TaskInit(cPublish pTask)
        {

            if (pTask.PublishManage.Equals(this))
            {
                pTask.PublishCompleted  += this.onPublishCompleted;
                pTask.PublishFailed  += this.onPublishFailed;
                pTask.PublishStarted  += this.onPublishStarted;
                pTask.PublishError  += this.onPublishError;
                //pTask.PublishTempDataCompleted += this.onPublishTempDataCompleted;
                pTask.PublishLog += this.onPublishLog;
                pTask.RuntimeInfo += this.onRunTimeInfo;
                pTask.DoCount += this.onDoCount;
                pTask.PublishErrorData += this.onPublishErrData;
                pTask.PublishSource += this.onPublishSource;
                pTask.PublishStop += this.onPublishStop;
                pTask.UpdateState += this.onUpdateState;
            }

        }

        private void onPublishLog(object sender, PublishLogEventArgs e)
        {
            if (e_PublishLog != null && !e.Cancel)
            {
                e_PublishLog(sender, e);
            }

        }

        private void onPublishCompleted(object sender, PublishCompletedEventArgs e)
        {

            //�ӵ�ǰ�б���ɾ���˼�¼
            cPublish pt = (cPublish)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishCompleted != null && !e.Cancel)
            {
                e_PublishCompleted(sender, e);
            }

        }

        private void onPublishFailed(object sender, PublishFailedEventArgs e)
        {
            //�ӵ�ǰ�б���ɾ���˼�¼
            cPublish pt = (cPublish)sender;
            m_ListPublish.Remove(pt);
            pt = null;

            if (e_PublishFailed != null && !e.Cancel)
            {
                e_PublishFailed(sender, e);
            }

        }

        private void onPublishStarted(object sender, PublishStartedEventArgs e)
        {
            if (e_PublishStarted != null && !e.Cancel)
            {
                e_PublishStarted(sender, e);
            }
        }

        private void onPublishStop(object sender, PublishStopEventArgs e)
        {
            if (e_PublishStop != null && !e.Cancel)
            {
                e_PublishStop(sender, e);
            }
        }

        private void onUpdateState(object sender, UpdateStateArgs e)
        {
            if (e_UpdateState != null && !e.Cancel)
            {
                e_UpdateState(sender, e);
            }
        }

        private void onPublishError(object sender, PublishErrorEventArgs e)
        { 
            //�ӵ�ǰ�б���ɾ���˼�¼
            //cPublish pt = (cPublish)sender;
            //m_ListPublish.Remove(pt);
            //pt = null;

            if (e_PublishError != null && !e.Cancel)
            {
                e_PublishError(sender, e);
            }

        }

        //private void onPublishTempDataCompleted(object sender, PublishTempDataCompletedEventArgs e)
        //{

        //    //�ӵ�ǰ�б���ɾ���˼�¼����ʱ���ݵı���Ҳ����Ϊһ������������ִ�е�
        //    //���ԣ�������Ϻ���Ҫɾ��������
        //    cPublish pt = (cPublish)sender;
        //    m_ListPublish.Remove(pt);
        //    pt = null;

        //    if (e_PublishTempDataCompleted != null && !e.Cancel)
        //    {
        //        e_PublishTempDataCompleted(sender, e);
        //    }
        //}

        private void onRunTimeInfo(object sender, RunTimeEventArgs e)
        {
            if (e_RuntimeInfo != null && !e.Cancel)
            {
                e_RuntimeInfo(sender, e);
            }
        }

        private void onDoCount(object sender, DoCountEventArgs e)
        {
            if (e_DoCount != null && !e.Cancel)
            {
                e_DoCount(sender, e);
            }
        }

        private void onPublishErrData(object sender, PublishErrDataEventArgs e)
        {
            if (e_PublishErrorData != null && !e.Cancel)
            {
                e_PublishErrorData(sender, e);
            }
        }

        private void onPublishSource(object sender, PublishSourceEventArgs e)
        {
            if (e_PublishSource != null && !e.Cancel)
            {
                e_PublishSource(sender, e);
            }
        }

        #region �¼�

        /// �������� ����¼�
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        public event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { e_PublishCompleted += value; }
            remove { e_PublishCompleted -= value; }
        }

        /// �������� ʧ���¼�
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        public event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { e_PublishFailed += value; }
            remove { e_PublishFailed -= value; }
        }

        /// �������� ��ʼ�ɼ��¼�
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        public event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { e_PublishStarted += value; }
            remove { e_PublishStarted -= value; }
        }

        //��������ֹͣ�¼�
        private event EventHandler<PublishStopEventArgs> e_PublishStop;
        public event EventHandler<PublishStopEventArgs> PublishStop
        {
            add { e_PublishStop += value; }
            remove { e_PublishStop -= value; }
        }

        ///�������� �����¼�
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        public event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { e_PublishError += value; }
            remove { e_PublishError -= value; }
        }

        ///��ʱ������������¼�
        //private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        //public event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        //{
        //    add { e_PublishTempDataCompleted += value; }
        //    remove { e_PublishTempDataCompleted -= value; }
        //}

        //���񷢲���־�¼�
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        public event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { e_PublishLog += value; }
            remove { e_PublishLog -= value; }
        }

        /// <summary>
        /// �̹߳���Ч���¼�
        /// </summary>
        private event EventHandler<RunTimeEventArgs> e_RuntimeInfo;
        public event EventHandler<RunTimeEventArgs> RuntimeInfo
        {
            add { e_RuntimeInfo += value; }
            remove { e_RuntimeInfo -= value; }
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        private event EventHandler<DoCountEventArgs> e_DoCount;
        public event EventHandler<DoCountEventArgs> DoCount
        {
            add { e_DoCount += value; }
            remove { e_DoCount -= value; }
        }

        /// <summary>
        /// ���ط���ʧ�ܵĴ��������¼�
        /// </summary>
        private event EventHandler<PublishErrDataEventArgs> e_PublishErrorData;
        public event EventHandler<PublishErrDataEventArgs> PublishErrorData
        {
            add { e_PublishErrorData += value; }
            remove { e_PublishErrorData -= value; }
        }

        /// <summary>
        /// �������һ��web�����������ҳԴ��
        /// </summary>
        private event EventHandler<PublishSourceEventArgs> e_PublishSource;
        public event EventHandler<PublishSourceEventArgs> PublishSource
        {
            add { e_PublishSource += value; }
            remove { e_PublishSource -= value; }
        }

        private event EventHandler<UpdateStateArgs> e_UpdateState;
        public event EventHandler<UpdateStateArgs> UpdateState
        {
            add { e_UpdateState += value; }
            remove { e_UpdateState -= value; }
        }
        #endregion
    }
}
