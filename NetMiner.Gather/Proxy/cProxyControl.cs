using System;
using System.Collections.Generic;
using System.Text;
using System.Web ;
using System.Net ;
using System.Threading;

///代理共享类
///将配置的代理信息进行加载，并根据采集的情况
///进行代理信息的索取，做到代理轮询的操作
///2010-12-10 创建

namespace NetMiner.Gather.Proxy
{
    public class cProxyControl
    {
        private List<cProxyInfo> m_Proxys;
        private int m_MaxCount = 0;
        private int m_CurIndex = 0;

        private readonly Object m_AddLock = new Object();
        private string m_workPath = string.Empty;

        public cProxyControl(string workPath)
        {
            m_workPath = workPath;

            m_Proxys = new List<cProxyInfo>();

            LoadProxy();

        }

        public cProxyControl(string workPath, string address,int port,string User,string Pwd)
        {
            m_workPath = workPath;
            m_Proxys = new List<cProxyInfo>();

            cProxyInfo pInfo = new cProxyInfo();
            pInfo.ProxyServer = address;
            pInfo.ServerPort = port.ToString ();
            pInfo.User = User;
            pInfo.Pwd = Pwd;
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

            m_Proxys = new List<cProxyInfo>();

            cProxy cp = new cProxy(m_workPath);
            int m_MaxCount = cp.GetCount();

            if(m_MaxCount>0) 
            {
                cProxyInfo pInfo = new cProxyInfo();
                pInfo.ProxyServer = cp.GetProxyServer(0);
                pInfo.ServerPort = cp.GetProxyPort(0);
                pInfo.User = cp.GetUser(0);
                pInfo.Pwd = cp.GetPwd(0);

                m_Proxys.Add(pInfo);
            }
            cp = null;

            this.m_MaxCount = 1;
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
                m_Proxys = new List<cProxyInfo>();
                LoadProxy();
            }
            catch { }
            finally { Monitor.Exit(m_AddLock); }
        }

        private void LoadProxy()
        {
            cProxy cp = new cProxy(m_workPath);
            int m_MaxCount = cp.GetCount();
            this.m_MaxCount = m_MaxCount;

            for (int i = 0; i < m_MaxCount; i++)
            {
                cProxyInfo pInfo = new cProxyInfo();
                pInfo.ProxyServer = cp.GetProxyServer(i);
                pInfo.ServerPort = cp.GetProxyPort(i);
                pInfo.User = cp.GetUser(i);
                pInfo.Pwd = cp.GetPwd(i);

                m_Proxys.Add(pInfo);
            }

            cp = null;
        }

        

        public cProxyInfo GetProxy()
        {
            cProxyInfo p = null;

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

        public cProxyInfo GetFirstProxy()
        {
            cProxyInfo p=null;

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
