using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Interface.WCF;
using System.ServiceModel;

namespace SominerMonitor
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class cSubscriber : IDisposable
    {
        private string _serviceUri;
        private cMessageListener _listener = new cMessageListener();
        private iMessagePublisher _serviceProxy;

        public cSubscriber(string serviceUri)
        {
            _serviceUri = serviceUri;
            _listener.SendMessage += this.on_SendMessage;
        }

        private void on_SendMessage(object sender, eMessageEvent e)
        {
            if (e_SendMessage != null)
                e_SendMessage(sender, e);
        }

        public void Subscribe()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            _serviceProxy = DuplexChannelFactory<iMessagePublisher>.CreateChannel(_listener, binding, new EndpointAddress(_serviceUri));
            _serviceProxy.Regist();
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
        private event EventHandler<eMessageEvent> e_SendMessage;
        public event EventHandler<eMessageEvent> SendMessage
        {
            add { lock (m_eventLock) { e_SendMessage += value; } }
            remove { lock (m_eventLock) { e_SendMessage -= value; } }
        }
    }
}
