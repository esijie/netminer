using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.IPC.Server;

namespace NetMiner.IPC
{
    /// <summary>
    /// 客户连接事件
    /// </summary>
    public class ClientListenerEventArgs : EventArgs
    {
        public cClientListener Listener { get; private set; }

        public ClientListenerEventArgs(cClientListener listener)
        {
            this.Listener = listener;
        }
    }

    /// <summary>
    /// 消息通知事件，即服务器想客户端发送消息
    /// </summary>
    public class MessageNotifyErrorEventArgs : EventArgs
    {
        public cClientListener Listener { get; private set; }

        public Exception Error { get; private set; }

        public MessageNotifyErrorEventArgs(cClientListener listener, Exception error)
        {
            this.Listener = listener;
            this.Error = error;
        }


    }

    /// <summary>
    /// 回调的事件，向客户端发送消息
    /// </summary>
    public class rMessageEvent : EventArgs
    {
        public rMessageEvent(string Message)
        {
            m_Message = Message;
        }

        private string m_Message;
        public string Message
        {
            get { return m_Message; }
            set { m_Message = value; }
        }
    }

}
