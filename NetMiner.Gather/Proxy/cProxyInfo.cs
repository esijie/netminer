using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Proxy
{
    public class cProxyInfo
    {
        public cProxyInfo()
        {
        }

        ~cProxyInfo()
        {

        }

        private string m_ProxyServer;
        public string ProxyServer
        {
            get { return m_ProxyServer; }
            set { m_ProxyServer = value; }
        }

        private string m_ServerPort;
        public string ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }

        private string m_User;
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Pwd;
        public string Pwd
        {
            get { return m_Pwd; }
            set { m_Pwd = value; }
        }
    }
}
