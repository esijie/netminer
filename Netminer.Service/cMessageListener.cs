using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Interface.WCF;

namespace SoukeyService
{
    public class cMessageListener
    {
        public string FromIP { get; private set; }

        public int FromPort { get; private set; }

        private iMessageCallback _innerListener;

        public cMessageListener(string fromIP, int fromPort, iMessageCallback innerListener)
        {
            this.FromIP = fromIP;
            this.FromPort = fromPort;
            _innerListener = innerListener;
        }

        /// <summary>
        /// 通知消息；
        /// </summary>
        /// <param name="message"></param>
        public void Notify(string message)
        {
            _innerListener.OnPublish(message);
        }

        public override bool Equals(object obj)
        {
            bool eq = base.Equals(obj);
            if (!eq)
            {
                cMessageListener lstn = obj as cMessageListener;
                if (lstn._innerListener.Equals(this._innerListener))
                {
                    eq = true;
                }
            }
            return eq;
        }
    }
}
