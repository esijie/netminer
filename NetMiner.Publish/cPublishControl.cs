using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Resource;
using System.Data;
using NetMiner.Core.Entity;

///���ܣ������������ �¼�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Publish
{
    public class cPublishControl : IDisposable 
    {

        //�������ļ��ˣ�ʵ��Ӧ�ð��ղɼ���ģʽ�����У����Խ��и��ּ��
        //������������� ���������ʱ����Զ��̷߳��������ܸо����ַ���ģʽ
        //ʵ���Բ��Ǻܴ����Ծ�����һ���������ܣ�������ϵͳ���������о�һ��
        //���õ�Ч��,��������������޸�.

        private cPublishManage m_PublishManage;
        private string m_workPath = string.Empty;

        public cPublishControl(string workPath)
        {
            this.m_workPath = workPath;
            m_PublishManage = new cPublishManage();
        }

        ~cPublishControl()
        {

        }

        public cPublishManage PublishManage
        {
            get { return m_PublishManage; }
        }

        /// �������������е�����
        /// �൱�ڳ�ʼ������
        public bool AddPublishTask(NetMiner.Core.Entity.cTaskDataList taskDataList)
        {
            //�������������ݽ��вɼ���������
            //�����������س�������Դ��󣬼������أ�ȷ�����е����񶼿��Լ��سɹ�
            bool IsSucceed = true;

            for (int i = 0; i < taskDataList.TaskCount; i++)
            {
                //�ڴ˲������ӽ��뷢��״̬�Ĳɼ�����
                if (taskDataList.TaskDataList[i].TaskState == cGlobalParas.TaskState.Publishing ||
                    taskDataList.TaskDataList[i].TaskState == cGlobalParas.TaskState.PublishStop ||
                    taskDataList.TaskDataList[i].TaskState == cGlobalParas.TaskState.PublishFailed)
                {
                    try
                    {
                        cPublish pt = new cPublish(this.m_workPath, this.PublishManage, taskDataList.TaskDataList[i].TaskID, new DataTable());
                        m_PublishManage.AddPublishingTask(pt);

                    }
                    catch (System.Exception)
                    {
                        IsSucceed = false;
                    }
                }
            }

            return IsSucceed;
        }

        /// ɾ��ָ���Ĳɼ�����
        public void Remove(cPublish pTask)
        {
            m_PublishManage.Remove(pTask);
        }

        
        /// <summary>
        /// ���ӷ�������,���ڷ������ݣ�ͬʱ����������
        /// </summary>
        /// <param name="pT"></param>
        public void startPublish(cPublish pT)
        {
            m_PublishManage.AddPublishTask(pT );
        }

        /// <summary>
        /// �ɼ�����ķ������Զ��ģ�����ֹͣ�����������ݼӹ����߶��ԣ��ǿ���ֹͣ��
        /// </summary>
        public void StopPublish()
        {
            m_PublishManage.StopPublish();
        }

        public void StopPublish(cPublish pt)
        {
            m_PublishManage.StopPublish(pt);
        }

        public void OverPublish(cPublish pt)
        {
            m_PublishManage.OverPublish(pt);
        }

        public void Abort()
        {
            m_PublishManage.Abort();
        }

        //���ӷ�������,���ڷ�����ʱ�ɼ������ݣ�ͬʱ����������
        //private void startSaveTempData(cPublish pT)
        //{
        //    m_PublishManage.AddSaveTempDataTask(pT);
        //}

        #region IDisposable ��Ա
        private bool m_disposed;
        /// <summary>
        /// �ͷ��� Download �ĵ�ǰʵ��ʹ�õ�������Դ
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                 
                  
                }

                // �ڴ��ͷŷ��й���Դ

                m_disposed = true;
            }
        }


        #endregion

    }




}
