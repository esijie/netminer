using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Net;
using System.Runtime.Serialization;
using System.Collections;
using System.IO;

namespace NetMiner.IPC.Server
{

    /// <summary>
    /// 服务端接口，通过接口向客户端提供该有的支持服务
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(iCallback))]
    public interface iServer
    {
        [OperationContract]
        ///登记消息监听器
        void Regist(string clientName);

        /// <summary>
        /// 注销消息监听器；
        /// </summary>
        [OperationContract]
        void Unregist();

        /// <summary>
        /// 获取网页源码
        /// </summary>
        /// <param name="TaskName"></param>
        [OperationContract]
        eResponse GetHtml(string Url);

        bool HeartBeat();

        /// <summary>
        /// 清除浏览器的Cookie
        /// </summary>
        [OperationContract]
        void ClearCookie();
    }

    [ServiceContract]
    [DataContract]
    [Serializable]
    public class eResponse
    {     
        [DataMember]
        public string HtmlSource { get; set; }
        [DataMember]
        public string Cookie { get; set; }
      
    }

}
