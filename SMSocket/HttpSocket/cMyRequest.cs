using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Net.Security;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;

///Socket实现HTTP请求类，在此处理基础信息，实现了request的基本操作
///Socket采用的是异步通讯的方式，在此可以实时监控Socket通讯效率，实现上传及下载
///速度的监控及限速
namespace SMSocket.HttpSocket
{
  
    public class MyWebRequest
    {

        #region  属性
        public int Timeout;       //设置超时时间
        public string Header;     //设置请求的头信息
        internal WebHeaderCollection Headers;
        public Uri RequestUri;    //请求的Uri
        public string Method;

        public webCode uCode;
        public webCode wCode;

        public MyWebResponse response;
        public bool KeepAlive;

        public bool IsProxy{get;set;}
        public ProxyType proxyType;
        public string httpProxyAddress;
        public int httpProxyPort;

        internal bool m_IsSsl;
        public bool AllowAutoRedirect;
    
        #endregion

        /// <summary>
        /// 创建一个http请求，并返回创建成功的请求，如果当前已经存在
        /// 请求，则返回已经存在的请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="AliveRequest"></param>
        /// <param name="bKeepAlive"></param>
        /// <param name="isProxy"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static MyWebRequest Create(Uri uri, MyWebRequest AliveRequest, bool bKeepAlive)
        {

            if (bKeepAlive &&
                AliveRequest != null &&
                AliveRequest.response != null &&
                //AliveRequest.response.KeepAlive &&
                AliveRequest.response.socket.Connected &&
                AliveRequest.RequestUri.Host == uri.Host)
            {
                AliveRequest.RequestUri = uri;
                return AliveRequest;
            }
            return new MyWebRequest(uri, bKeepAlive);
        }

        /// <summary>
        /// 构建一个请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="keepAlive"></param>
        public MyWebRequest(Uri uri,bool keepAlive)
        {
            uCode = webCode.auto;
            wCode = webCode.auto;
            AllowAutoRedirect = true;
            proxyType = ProxyType.None;

            if (uri.AbsoluteUri.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
                m_IsSsl = true;
            else
                m_IsSsl = false;

            Headers = new WebHeaderCollection();
            RequestUri = uri;

            if (uri.Port == 80)
                Headers["Host"] = uri.Host;
            else
                Headers["Host"] = uri.Host + ":" + uri.Port;

            KeepAlive = keepAlive;
            if (KeepAlive)
                Headers["Connection"] = "Keep-Alive";

        }

        ~MyWebRequest()
        {
            
        }

        public void Dispose()
        {
            this.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 请求响应
        /// </summary>
        /// <param name="uCode">网页编码</param>
        /// <returns></returns>
        public void GetResponse()
        {
            if (response == null || response.socket == null || response.socket.Connected == false)
            {
                response = new MyWebResponse(this, this.wCode, this.uCode);
                response.Connect(this);

                response.SendRequest(this, this.uCode);
            }
            else
            {
                //如果连接还存在，则直接发送请求
                response.SendRequest(this);
            }

            //接收数据
            response.Receive(this);

        }

    }
}
