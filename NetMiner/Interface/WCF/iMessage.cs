using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace NetMiner.Interface.WCF
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(iMessageCallback))]
    public interface iMessagePublisher 
    {
        [OperationContract]
        ///登记消息监听器
        void Regist();

        /// <summary>
        /// 注销消息监听器；
        /// </summary>
        [OperationContract]
        void Unregist();
    

    }

    [ServiceContract(Namespace = "http://www.netminer.cn")]
    public interface iTaskControl
    {
        [OperationContract]
        void StartTask(string TaskName);
        [OperationContract]
        void StopTask(string TaskName);
        [OperationContract]
        void SetTaskExtPara(string TaskName,string Paras);
    }

    public interface iMessageCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnPublish(string message);
    }
}
