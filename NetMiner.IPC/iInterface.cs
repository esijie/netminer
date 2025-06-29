using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace NetMiner.IPC
{

    /// <summary>
    /// callback用于返回浏览器的相关信息
    /// </summary>
    public interface iCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnReceiveMessage(string message);
    }
}
