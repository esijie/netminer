using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Resource;
using System.Data;
using NetMiner.Core.Entity;

///功能：发布任务入口 事件定义
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Publish
{
    public class cPublishControl : IDisposable 
    {

        //发布做的简单了，实际应该按照采集的模式来进行，可以进行各种监控
        //并且如果发布的 数据量大的时候可以多线程发布，但总感觉这种发布模式
        //实用性不是很大，所以就先做一个发布功能，可以让系统跑起来，感觉一下
        //试用的效果,后面会慢慢进行修改.

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

        /// 增加运行区所有的任务
        /// 相当于初始化数据
        public bool AddPublishTask(NetMiner.Core.Entity.cTaskDataList taskDataList)
        {
            //根据运行区数据进行采集任务的添加
            //如果有任务加载出错，则忽略错误，继续加载，确保所有的任务都可以加载成功
            bool IsSucceed = true;

            for (int i = 0; i < taskDataList.TaskCount; i++)
            {
                //在此不能增加进入发布状态的采集任务
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

        /// 删除指定的采集任务
        public void Remove(cPublish pTask)
        {
            m_PublishManage.Remove(pTask);
        }

        
        /// <summary>
        /// 增加发布任务,用于发布数据，同时启动此任务
        /// </summary>
        /// <param name="pT"></param>
        public void startPublish(cPublish pT)
        {
            m_PublishManage.AddPublishTask(pT );
        }

        /// <summary>
        /// 采集任务的发布是自动的，不能停止，但对于数据加工工具而言，是可以停止的
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

        //增加发布任务,用于发布临时采集的数据，同时启动此任务
        //private void startSaveTempData(cPublish pT)
        //{
        //    m_PublishManage.AddSaveTempDataTask(pT);
        //}

        #region IDisposable 成员
        private bool m_disposed;
        /// <summary>
        /// 释放由 Download 的当前实例使用的所有资源
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

                // 在此释放非托管资源

                m_disposed = true;
            }
        }


        #endregion

    }




}
