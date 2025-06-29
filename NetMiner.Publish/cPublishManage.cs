using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Core.Event;

///功能：发布任务管理 启动任务 响应事件 此功能实现的很简单
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：下一步需要完善发布功能模块，势必此功能要继续完善
///说明：无 
///版本：01.10.00
///修订：无
///2013-2-26 将发布操作进行合并，采集任务的发布和数据加工发布工具中的发布
///同时取消触发器操作
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
            //添加到对列中
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
            //添加到对列中
            ListPublish.Add(pt);
            TaskInit(pt);

            //启动此任务
            pt.startPublic();
        }

        //public void AddSaveTempDataTask(cPublish pt)
        //{
        //    ListPublish.Add(pt);
        //    TaskTempSaveInit(pt);

        //    //启动此任务
        //    pt.startSaveTempData();
        //}

        /// <summary>
        /// 将当前正在进行的发布任务全部停止
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

        //注册临时存储任务的事件，系统自动执行，无需用户干预
        private void TaskTempSaveInit(cPublish pTask)
        {
            if (pTask.PublishManage.Equals(this))
            {
                //pTask.PublishTempDataCompleted += this.onPublishTempDataCompleted;
                pTask.PublishError += this.onPublishError;
            }
        }

        //注册发布任务的事件
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

            //从当前列表中删除此记录
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
            //从当前列表中删除此记录
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
            //从当前列表中删除此记录
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

        //    //从当前列表中删除此记录，临时数据的保存也是作为一个发布任务来执行的
        //    //所以，保存完毕后，需要删除此任务
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

        #region 事件

        /// 发布任务 完成事件
        private event EventHandler<PublishCompletedEventArgs> e_PublishCompleted;
        public event EventHandler<PublishCompletedEventArgs> PublishCompleted
        {
            add { e_PublishCompleted += value; }
            remove { e_PublishCompleted -= value; }
        }

        /// 发布任务 失败事件
        private event EventHandler<PublishFailedEventArgs> e_PublishFailed;
        public event EventHandler<PublishFailedEventArgs> PublishFailed
        {
            add { e_PublishFailed += value; }
            remove { e_PublishFailed -= value; }
        }

        /// 发布任务 开始采集事件
        private event EventHandler<PublishStartedEventArgs> e_PublishStarted;
        public event EventHandler<PublishStartedEventArgs> PublishStarted
        {
            add { e_PublishStarted += value; }
            remove { e_PublishStarted -= value; }
        }

        //发布任务停止事件
        private event EventHandler<PublishStopEventArgs> e_PublishStop;
        public event EventHandler<PublishStopEventArgs> PublishStop
        {
            add { e_PublishStop += value; }
            remove { e_PublishStop -= value; }
        }

        ///发布任务 错误事件
        private event EventHandler<PublishErrorEventArgs> e_PublishError;
        public event EventHandler<PublishErrorEventArgs> PublishError
        {
            add { e_PublishError += value; }
            remove { e_PublishError -= value; }
        }

        ///临时发布数据完成事件
        //private event EventHandler<PublishTempDataCompletedEventArgs> e_PublishTempDataCompleted;
        //public event EventHandler<PublishTempDataCompletedEventArgs> PublishTempDataCompleted
        //{
        //    add { e_PublishTempDataCompleted += value; }
        //    remove { e_PublishTempDataCompleted -= value; }
        //}

        //任务发布日志事件
        private event EventHandler<PublishLogEventArgs> e_PublishLog;
        public event EventHandler<PublishLogEventArgs> PublishLog
        {
            add { e_PublishLog += value; }
            remove { e_PublishLog -= value; }
        }

        /// <summary>
        /// 线程工作效率事件
        /// </summary>
        private event EventHandler<RunTimeEventArgs> e_RuntimeInfo;
        public event EventHandler<RunTimeEventArgs> RuntimeInfo
        {
            add { e_RuntimeInfo += value; }
            remove { e_RuntimeInfo -= value; }
        }

        /// <summary>
        /// 计数事件
        /// </summary>
        private event EventHandler<DoCountEventArgs> e_DoCount;
        public event EventHandler<DoCountEventArgs> DoCount
        {
            add { e_DoCount += value; }
            remove { e_DoCount -= value; }
        }

        /// <summary>
        /// 返回发布失败的错误数据事件
        /// </summary>
        private event EventHandler<PublishErrDataEventArgs> e_PublishErrorData;
        public event EventHandler<PublishErrDataEventArgs> PublishErrorData
        {
            add { e_PublishErrorData += value; }
            remove { e_PublishErrorData -= value; }
        }

        /// <summary>
        /// 返回最后一次web发布请求的网页源码
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
