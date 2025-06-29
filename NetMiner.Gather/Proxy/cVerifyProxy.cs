using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Collections;
using NetMiner.Resource;
using NetMiner.Gather;
using NetMiner.Gather.Gather;

namespace NetMiner.Gather.Proxy
{
    public class cVerifyProxy
    {
        //最大验证的线程为20
        private int m_MaxThreadCount = 10;
        private Thread[] threadsRun;
        private Queue queueURLS;
        private bool m_ThreadsRunning = false;
        private int SleepConnectTime = 1;
        private int SleepFetchTime = 2;
        //当前取出Uri错误的次数
        private int m_DequeueErr;
        private int m_MaxDequeueErr = 100;

        private int _Count = 0;
        private string m_workPath;

        public cVerifyProxy(string workPath)
        {
            m_workPath = workPath;
            m_DoneCount = 0;
            this.queueURLS = new Queue();
        }

        ~cVerifyProxy()
        {
        }

        private string _verifyUrl;
        public string VerifyUrl
        {
            get { return _verifyUrl; }
            set { _verifyUrl = value; }
        }

        private string _verifyResult;
        public string VerifyResult
        {
            get { return _verifyResult; }
            set{_verifyResult=value;}
        }

        private int m_ThreadCount;
        private int ThreadCount
        {
            get { return m_ThreadCount; }
            set
            {
                m_ThreadCount = value;

                try
                {
                    //按照规定的线程数进行线程创建
                    for (int nIndex = 0; nIndex < value; nIndex++)
                    {
                        if (threadsRun[nIndex] == null || threadsRun[nIndex].ThreadState != ThreadState.Suspended)
                        {
                            threadsRun[nIndex] = new Thread(new ThreadStart(Work));
                            threadsRun[nIndex].Name = nIndex.ToString();
                            threadsRun[nIndex].Start();
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
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


        //启动爬虫
        public bool Start(List<cProxyInfo> Proxys)
        {

            this.threadsRun = new Thread[this.m_MaxThreadCount];

            //数据压入待检验的队列
            for (int i=0;i<Proxys.Count ;i++)
                this.EnqueueUri(Proxys[i], false);

            _Count = queueURLS.Count;
         
            //将运行标记置为：true
            m_ThreadsRunning = true;

            //设置当前的线程数，同时启动线程
            ThreadCount = this.m_MaxThreadCount;

            //触发成功启动事件
            this.OnStartedEventSend(this, new cStartVerifyArgs());

            return true;
        }

        public bool Stop()
        {
            m_ThreadsRunning = false;
            this.threadsRun = null;
            this.OnStopEventSend(this, new cStopVerifyArgs());

            return true;
        }

        //验证代理IP
        private bool VerifyProxy(cProxyInfo p)
        {
            try
            {
                cProxyControl pControl = new cProxyControl(this.m_workPath, p.ProxyServer, int.Parse(p.ServerPort), p.User, p.Pwd);
                //判断网页编码
                cGatherWeb g = new cGatherWeb(this.m_workPath, ref pControl, true, true,cGlobalParas.ProxyType.TaskConfig ,"",0);
                g.IsUrlAutoRedirect = true;
                string cookie = "";
                string webSource = g.GetHtml(this.VerifyUrl, cGlobalParas.WebCode.auto, false,false, cGlobalParas.WebCode.auto, 
                    ref cookie, "", "", false, false, "",false,"","","");
                pControl = null;

                if (this.VerifyResult.Trim() == "")
                    return true;
                else
                {
                    if (webSource.IndexOf(this.VerifyResult) > -1)
                        return true;
                    else
                        return false;
                }
            }
            catch { return false; }
        }

        public void Work()
        {
            while (m_ThreadsRunning && int.Parse(Thread.CurrentThread.Name) < this.ThreadCount)
            {
                DateTime mstartTick = DateTime.Now;

                cProxyInfo pInfo = DequeueUri();
                if (pInfo != null)
                {
                    if (SleepConnectTime > 0)
                    {

                        try
                        {
                            bool isS = VerifyProxy(pInfo);

                            m_DoneCount++;

                            TimeSpan usticks = DateTime.Now - mstartTick;

                            if (isS == true)
                            {
                                this.onShowMessageSend(this, new cShowInfoArgs(pInfo.ProxyServer, cGlobalParas.VerifyProxyState.Succeed, (int)usticks.TotalMilliseconds));
                            }
                            else
                            {
                                this.onShowMessageSend(this, new cShowInfoArgs(pInfo.ProxyServer, cGlobalParas.VerifyProxyState.Faild, (int)usticks.TotalMilliseconds));

                            }
                        }
                        catch
                        {
                            m_ErrCount++;
                            this.onShowMessageSend(this, new cShowInfoArgs(pInfo.ProxyServer, cGlobalParas.VerifyProxyState.Faild, 0));

                        }

                    }
                }
                else
                {
                    //线程等待
                    Thread.Sleep(SleepFetchTime * 1000);
                    if (_Count ==m_DoneCount+m_ErrCount)
                    {
                        //表示任务结束
                        m_ThreadsRunning = false;
                        this.OnCompletedEventSend(this, new cCompletedVerifyArgs());
                    }
                }

            }

        }

        cProxyInfo DequeueUri()
        {
            cProxyInfo uri = null;
            lock (queueURLS.SyncRoot)
            {
                try
                {
                    uri = (cProxyInfo)queueURLS.Dequeue();
                }
                catch (Exception)
                {
                }
            }
            return uri;
        }

        //只要是传入进来的代理IP，就不会有重复，所以，在此无需判断重复的问题
        bool EnqueueUri(cProxyInfo uri, bool bCheckRepetition)
        {
    
            lock (queueURLS.SyncRoot)
            {
                try
                {
                    queueURLS.Enqueue(uri);
                }
                catch (Exception)
                {
                }
            }

            return true;
        }

        #region 定义代理事件，用于UI的方法触发

        //定义代理用于表示任务已成功启动
        public delegate void StartedEventHandler(object sender, cStartVerifyArgs e);
        public event StartedEventHandler StartVerifySend;
        public void OnStartedEventSend(object sender, cStartVerifyArgs e)
        {
            if (StartVerifySend != null)
                this.StartVerifySend(sender, e);
        }

        //定义代理用于表示任务已成功结束
        public delegate void StopEventHandler(object sender, cStopVerifyArgs e);
        public event StopEventHandler StopVerifySend;
        public void OnStopEventSend(object sender, cStopVerifyArgs e)
        {
            if (StopVerifySend != null)
                this.StopVerifySend(sender, e);
        }

        //定义代理用于表示任务已成功暂停
        public delegate void CompletedEventHandler(object sender, cCompletedVerifyArgs e);
        public event CompletedEventHandler CompletedVerifyEventSend;
        public void OnCompletedEventSend(object sender, cCompletedVerifyArgs e)
        {
            if (CompletedVerifyEventSend != null)
                this.CompletedVerifyEventSend(sender, e);
        }

        //定义一个代理用于反馈后台线程执行的情况
        public delegate void ShowStateEventHandler(object sender, cShowInfoArgs e);
        public event ShowStateEventHandler ShowMessageSend;
        public void onShowMessageSend(object sender, cShowInfoArgs e)
        {
            if (ShowMessageSend != null)
                this.ShowMessageSend(sender, e);
        }

        #endregion
    }

   
}
