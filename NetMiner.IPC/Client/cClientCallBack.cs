using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NetMiner.IPC
{
    /// <summary>
    /// 服务端的回调
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class cClientCallBack : iCallback
    {
        public void OnReceiveMessage(string message)
        {
            if (e_RMessage != null)
            {
                e_RMessage(this, new rMessageEvent(message));
            }
        }

        private object m_eventLock = new object();
        private event EventHandler<rMessageEvent> e_RMessage;
        public event EventHandler<rMessageEvent> RMessage
        {
            add { lock (m_eventLock) { e_RMessage += value; } }
            remove { lock (m_eventLock) { e_RMessage -= value; } }
        }
    }

   
}
