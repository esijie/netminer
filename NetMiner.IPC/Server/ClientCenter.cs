using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace NetMiner.IPC.Server
{
    /// <summary>
    /// 管理客户实例，允许有多个客户接入获取源码
    /// </summary>
    public class ClientCenter
    {
        #region ClientCenter 的单例实现
        private static readonly object _syncLock = new object();//线程同步锁；
        private static ClientCenter _instance;
        /// <summary>
        /// 返回 MessageCenter 的唯一实例；
        /// </summary>
        public static ClientCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ClientCenter();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 保证单例的私有构造函数；
        /// </summary>
        private ClientCenter() { }

        #endregion

        public event EventHandler<ClientListenerEventArgs> ListenerAdded;

        public event EventHandler<ClientListenerEventArgs> ListenerRemoved;

        public event EventHandler<MessageNotifyErrorEventArgs> NotifyError;

        private List<cClientListener> _listeners = new List<cClientListener>(0);

        public void AddListener(cClientListener listener)
        {
            lock (_syncLock)
            {
                if (_listeners.Contains(listener))
                {
                    throw new InvalidOperationException("重复注册相同的监听器！");
                }
                _listeners.Add(listener);
            }

            if (this.ListenerAdded != null)
            {
                this.ListenerAdded(this, new ClientListenerEventArgs(listener));
            }
        }

        public void RemoveListener(cClientListener listener)
        {
            lock (_syncLock)
            {
                if (_listeners.Contains(listener))
                {
                    this._listeners.Remove(listener);
                }
                else
                {
                    throw new InvalidOperationException("要移除的监听器不存在！");
                }
            }
            if (this.ListenerRemoved != null)
            {
                this.ListenerRemoved(this, new ClientListenerEventArgs(listener));
            }
        }

        public void NotifyMessage(string message)
        {
            cClientListener[] listeners = _listeners.ToArray();
            foreach (cClientListener lstn in listeners)
            {
                try
                {
                    lstn.Notify(message);
                }
                catch (Exception ex)
                {
                    OnNotifyError(lstn, ex);
                }
            }
        }

        private void OnNotifyError(cClientListener listener, Exception error)
        {
            if (this.NotifyError == null)
            {
                return;
            }
            MessageNotifyErrorEventArgs args = new MessageNotifyErrorEventArgs(listener, error);
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                this.NotifyError(this, state as MessageNotifyErrorEventArgs);
            }, args);
        }
    }
}
