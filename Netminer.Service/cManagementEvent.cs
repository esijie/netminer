using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoukeyService
{
    public class MessageListenerEventArgs : EventArgs
    {
        public cMessageListener Listener { get; private set; }

        public MessageListenerEventArgs(cMessageListener listener)
        {
            this.Listener = listener;
        }
    }

    public class MessageNotifyErrorEventArgs : EventArgs
    {
        public cMessageListener Listener { get; private set; }

        public Exception Error { get; private set; }

        public MessageNotifyErrorEventArgs(cMessageListener listener, Exception error)
        {
            this.Listener = listener;
            this.Error = error;
        }


    }
}
