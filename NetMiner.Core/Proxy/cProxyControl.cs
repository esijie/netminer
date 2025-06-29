using System;
using System.Collections.Generic;
using System.Text;
using System.Web ;
using System.Net ;
using System.Threading;
using NetMiner.Core.Proxy.Entity;

///代理共享类
///将配置的代理信息进行加载，并根据采集的情况
///进行代理信息的索取，做到代理轮询的操作
///2010-12-10 创建

namespace NetMiner.Core.Proxy
{
    public class cProxyControl
    {
        private List<eProxy> m_Proxys;
        private int m_MaxCount = 0;
        private int m_CurIndex = 0;

        private readonly Object m_AddLock = new Object();
        private string m_workPath = string.Empty;

        public cProxyControl(string workPath)
        {
            m_workPath = workPath;

            m_Proxys = new List<eProxy>();

            LoadProxy();

        }

        public cProxyControl(string workPath, string address,int port,string User,string Pwd)
        {
            m_workPath = workPath;
            m_Proxys = new List<eProxy>();

            eProxy pInfo = new eProxy();
            pInfo.ProxyServer = address;
            pInfo.ProxyPort = port.ToString ();
            pInfo.User = User;
            pInfo.Password = Pwd;
            m_Proxys.Add(pInfo);

            this.m_MaxCount = 1;
        }

        

        /// <summary>
        /// 仅加载一条代理信息的记录进行代理服务器的处理
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="Index">0</param>
        public cProxyControl(string workPath,int Index)
        {
            m_workPath = workPath;

            m_Proxys = new List<eProxy>();

            oProxy cp = new oProxy(workPath);

            eProxy pInfo = cp.LoadFirstProxy();
            if (pInfo!=null)
                m_Proxys.Add(pInfo);
            cp.Dispose();
            cp = null;

            this.m_MaxCount = m_Proxys.Count;
        }

        ~cProxyControl()
        {
            m_Proxys = null;
        }

        //更新代理IP
        public void UpdateProxy()
        {
            Monitor.Enter(m_AddLock);

            try
            {
                m_MaxCount = 0;
                m_CurIndex = 0;
                m_Proxys = new List<eProxy>();
                LoadProxy();
            }
            catch { }
            finally { Monitor.Exit(m_AddLock); }
        }

        private void LoadProxy()
        {
            oProxy cp = new oProxy(this.m_workPath);
            IEnumerable<eProxy> eps = cp.LoadProxyData();
            int count = 0;
            foreach(eProxy ep in eps)
            {
                m_Proxys.Add(ep);
                count++;

            }
            this.m_MaxCount = count;
            cp.Dispose();
            cp = null;
        }

        

        public eProxy GetProxy()
        {
            eProxy p = null;

            Monitor.Enter(m_AddLock);

            try
            {
                if (this.m_MaxCount > 0)
                {
                    p = m_Proxys[m_CurIndex];

                    if (m_CurIndex + 1 == this.m_MaxCount)
                        m_CurIndex = 0;
                    else
                        m_CurIndex++;
                }
                
            }
            catch { }
            finally 
            {
                Monitor.Exit(m_AddLock);
            }

            return p; 
        }

        public eProxy GetFirstProxy()
        {
            eProxy p =null;

            Monitor.Enter(m_AddLock);
            try
            {
                if (this.m_MaxCount > 0)
                {
                    return m_Proxys[0];
                }
            }
            catch { }
            finally
            {
                Monitor.Exit(m_AddLock);
            }

            return p; 

        }
    }
}
