using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetMiner.IPC.Server
{
    /// <summary>
    /// 客户端的监听者，因为采用了双工通讯，这里定义了一个客户端的监听者试题
    /// </summary>
    public class cClientListener
    {
        public string FromIP { get; private set; }

        private iCallback _innerListener;

        public cClientListener(string fromIP, iCallback innerListener)
        {
            this.FromIP = fromIP;
            _innerListener = innerListener;
        }

        /// <summary>
        /// 通知消息；
        /// </summary>
        /// <param name="message"></param>
        public void Notify(string message)
        {
            _innerListener.OnReceiveMessage(message);
        }

        public override bool Equals(object obj)
        {
            bool eq = base.Equals(obj);
            if (!eq)
            {
                cClientListener lstn = obj as cClientListener;
                if (lstn._innerListener.Equals(this._innerListener))
                {
                    eq = true;
                }
            }
            return eq;
        }
    }
}
