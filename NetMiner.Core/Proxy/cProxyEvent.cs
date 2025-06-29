using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;


namespace NetMiner.Core.Proxy
{
    public class cProxyEventArgs : EventArgs
    {

        public cProxyEventArgs()
        {

        }

        /// <param name="cancel">是否取消事件</param>
        public cProxyEventArgs(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }
    }

    public class cStartVerifyArgs : cProxyEventArgs
    {
        public cStartVerifyArgs()
        {

        }
    }

    public class cStopVerifyArgs : cProxyEventArgs
    {
        public cStopVerifyArgs()
        {

        }
    }

    public class cCompletedVerifyArgs : cProxyEventArgs
    {
        public cCompletedVerifyArgs()
        {

        }
    }

    public class cShowInfoArgs : cProxyEventArgs
    {
        public cShowInfoArgs(string url,cGlobalParas.VerifyProxyState cState,int second)
        {
            _url = url;
            _state=cState;
            _secoond=second;
        }

        private string _url;
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        private cGlobalParas.VerifyProxyState _state;
        public cGlobalParas.VerifyProxyState State
        {
            get{return _state;}
            set{_state=value;}
        }

        private int _secoond;
        public int Second
        {
            get{return _secoond;}
            set{_secoond=value;}
        }
    }
}
