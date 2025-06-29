using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Interface.WCF;
using System.ServiceModel;

namespace SominerMonitor
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class cMessageListener : iMessageCallback
    {
        #region IMessageListener 成员

        public void OnPublish(string message)
        {
            //Console.WriteLine("[{0}]收到消息：{1}", DateTime.Now.ToString("yy-MM-dd HH:mm:ss"), message);
            if (e_SendMessage != null)
            {
                e_SendMessage(this, new eMessageEvent(message));
            }
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
