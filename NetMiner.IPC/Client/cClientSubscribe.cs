using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using NetMiner.IPC.Server;
using System.Net;
using System.IO;

namespace NetMiner.IPC.Client
{
    /// <summary>
    /// 客户端操作类
    /// </summary>
    public class cClientSubscribe:IDisposable
    {
        private string _serviceAddress;
        private cClientCallBack _listener = new cClientCallBack();
        private iServer _serviceProxy;

        public cClientSubscribe(string serviceAddress)
        {
            _serviceAddress = serviceAddress;
            _listener.RMessage += this.on_ReceiveMessage;
        }

        private void on_ReceiveMessage(object sender, rMessageEvent e)
        {
            if (e_ReceiveMessage != null)
                e_ReceiveMessage(sender, e);
        }

        /// <summary>
        /// 向服务器注册客户端
        /// </summary>
        public void Subscribe()
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.MaxReceivedMessageSize = 2147483647;
            
            _serviceProxy = DuplexChannelFactory<iServer>.CreateChannel(_listener, binding, new EndpointAddress(_serviceAddress));

            
            string name = Dns.GetHostName();
            IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
            _serviceProxy.Regist(ipadrlist[0].ToString());
        }

        public eResponse GetHtml(string Url)
        {
            eResponse response = _serviceProxy.GetHtml(Url);
            return response;
        }

        public bool isConnected()
        {
            try
            {
                bool isConnected = _serviceProxy.HeartBeat();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Clear()
        {

        }

        #region IDisposable 成员

        public void Dispose()
        {
            try
            {
                _serviceProxy.Unregist();
                (_serviceProxy as IDisposable).Dispose();
            }
            catch { }
            _listener = null;
            _serviceProxy = null;
        }

        #endregion

        private object m_eventLock = new object();
        private event EventHandler<rMessageEvent> e_ReceiveMessage;
        public event EventHandler<rMessageEvent> ReceiveMessage
        {
            add { lock (m_eventLock) { e_ReceiveMessage += value; } }
            remove { lock (m_eventLock) { e_ReceiveMessage -= value; } }
        }
    }
}
