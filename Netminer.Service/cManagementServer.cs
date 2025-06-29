using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NetMiner.Interface.WCF;

namespace SoukeyService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,ConcurrencyMode = ConcurrencyMode.Multiple)]

    public class cManagementServer : iMessagePublisher
    {

        public void Regist()
        {
            RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iMessageCallback callback = OperationContext.Current.GetCallbackChannel<iMessageCallback>();
            MessageCenter.Instance.AddListener(new cMessageListener(remoteEndpointProp.Address, remoteEndpointProp.Port, callback));
        }

        public void Unregist()
        {
            RemoteEndpointMessageProperty remoteEndpointProp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            iMessageCallback callback = OperationContext.Current.GetCallbackChannel<iMessageCallback>();
            MessageCenter.Instance.RemoveListener(new cMessageListener(remoteEndpointProp.Address, remoteEndpointProp.Port, callback));
        }
    }
}
