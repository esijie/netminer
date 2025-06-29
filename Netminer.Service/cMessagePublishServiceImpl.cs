using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Interface.WCF;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace SoukeyService
{
    public class cMessagePublishServiceImpl :iMessagePublisher
    {
        #region IMessagePublishService 成员
        /// <summary>
        /// 注册；
        /// </summary>
        public void Regist()
        {
            RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iMessageCallback callback = OperationContext.Current.GetCallbackChannel<iMessageCallback>();
            MessageCenter.Instance.AddListener(new cMessageListener(remoteEndpointProp.Address, remoteEndpointProp.Port, callback));
            Thread.Sleep(10 * 1000);
        }

        /// <summary>
        /// 注销；
        /// </summary>
        public void Unregist()
        {
            RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iMessageCallback callback = OperationContext.Current.GetCallbackChannel<iMessageCallback>();
            MessageCenter.Instance.RemoveListener(new cMessageListener(remoteEndpointProp.Address, remoteEndpointProp.Port, callback));
        }

      
        #endregion
    }
}
