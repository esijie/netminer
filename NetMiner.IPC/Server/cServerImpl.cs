using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.IO;

namespace NetMiner.IPC.Server
{
    /// <summary>
    /// 服务端实现类
    /// </summary>
    public class cServerImpl : iServer
    {
        /// <summary>
        /// 注册一个客户端
        /// </summary>
        public void Regist(string clientName)
        {
            //RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iCallback callback = OperationContext.Current.GetCallbackChannel<iCallback>();

            //在客户中心中新增一个回调实体
            ClientCenter.Instance.AddListener(new cClientListener(clientName, callback));
            Thread.Sleep(1* 1000);
        }

        /// <summary>
        /// 取消一个客户端
        /// </summary>
        public void Unregist()
        {
            RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iCallback callback = OperationContext.Current.GetCallbackChannel<iCallback>();
            ClientCenter.Instance.RemoveListener(new cClientListener(remoteEndpointProp.Address, callback));

        }

        /// <summary>
        /// 调用网页源码
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public virtual eResponse GetHtml(string Url)
        {
            return null;
        }

        public virtual bool HeartBeat()
        {
            return true;
        }

        /// <summary>
        /// 清除浏览器Cookie
        /// </summary>
        public virtual void ClearCookie()
        {

        }
    }
}
