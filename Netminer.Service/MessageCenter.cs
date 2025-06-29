using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SoukeyService
{
    public class MessageCenter
    {
        #region MessageCenter 的单例实现
        private static readonly object _syncLock = new object();//线程同步锁；
        private static MessageCenter _instance;
        /// <summary>
        /// 返回 MessageCenter 的唯一实例；
        /// </summary>
        public static MessageCenter Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new MessageCenter();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 保证单例的私有构造函数；
        /// </summary>
        private MessageCenter() { }

        #endregion

        public event EventHandler<MessageListenerEventArgs> ListenerAdded;

        public event EventHandler<MessageListenerEventArgs> ListenerRemoved;

        public event EventHandler<MessageNotifyErrorEventArgs> NotifyError;

        private List<cMessageListener> _listeners = new List<cMessageListener>(0);

        public void AddListener(cMessageListener listener)
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
                this.ListenerAdded(this, new MessageListenerEventArgs(listener));
            }
        }

        public void RemoveListener(cMessageListener listener)
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
                this.ListenerRemoved(this, new MessageListenerEventArgs(listener));
            }
        }

        public void NotifyMessage(string message)
        {
            cMessageListener[] listeners = _listeners.ToArray();
            foreach (cMessageListener lstn in listeners)
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

        private void OnNotifyError(cMessageListener listener, Exception error)
        {
            if (this.NotifyError == null)
            {
                return;
            }
            MessageNotifyErrorEventArgs args = new MessageNotifyErrorEventArgs(listener, error);
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                this.NotifyError(this, state as MessageNotifyErrorEventArgs);
            }, args);
        }
    }
}
