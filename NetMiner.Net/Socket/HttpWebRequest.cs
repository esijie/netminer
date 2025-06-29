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
using System.Collections;
using NetMiner.Resource;

///Socket实现HTTP请求类，在此处理基础信息，实现了request的基本操作
///Socket采用的是异步通讯的方式，在此可以实时监控Socket通讯效率，实现上传及下载
///速度的监控及限速
namespace NetMiner.Net.Socket
{

    public class HttpWebRequest
    {
        #region  属性
        private int m_TimeOut;
        /// <summary>
        /// 返回及设置超时时间
        /// </summary>
        public int Timeout
        {
            get { return m_TimeOut; }
            set { m_TimeOut = value; }
        }

        private Hashtable m_Headers;
        /// <summary>
        /// 设置头信息
        /// </summary>
        public Hashtable Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        public Uri RequestUri;    //请求的Uri

        private cGlobalParas.RequestMethod m_methor;
        public cGlobalParas.RequestMethod method
        {
            get { return m_methor; }
            set { m_methor = value; }
        }

        ///// <summary>
        ///// 页面编码
        ///// </summary>
        //public enCodingCode pageCode{get;set;}

        public long ContentLength { get; set; }

        public HttpWebResponse response;
        public bool KeepAlive;

        /// <summary>
        /// 代理类型
        /// </summary>
        public cGlobalParas.ProxyType proxyType;

        //public long ContentLength;

        /// <summary>
        /// 代理服务器地址
        /// </summary>
        public string ProxyAddress { get; set; }
        /// <summary>
        /// 代理服务器端口
        /// </summary>
        public int ProxyPort { get; set; }

        public string Cookie { get; set; }

        public byte[] postData { get; set; }


        public X509Certificate2Collection Credentials { get; set; }

        internal bool m_IsSsl;
        /// <summary>
        /// 是否允许重定向
        /// </summary>
        public bool AllowAutoRedirect;

        #endregion

        /// <summary>
        /// 创建一个http请求链接，如果当前已经存在，则直接返回
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="AliveRequest"></param>
        /// <param name="bKeepAlive"></param>
        /// <param name="isProxy"></param>
        /// <param name="pType"></param>
        /// <returns></returns>
        public static HttpWebRequest Create(Uri uri, HttpWebRequest AliveRequest, bool bKeepAlive)
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
            return new HttpWebRequest(uri, bKeepAlive);
        }

        /// <summary>
        /// 构建一个请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="keepAlive"></param>
        public HttpWebRequest(Uri uri, bool keepAlive)
        {
            AllowAutoRedirect = true;
            proxyType = cGlobalParas.ProxyType.SystemProxy;
            Headers = new Hashtable();
            Credentials = new X509Certificate2Collection();

            if (uri.AbsoluteUri.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
                m_IsSsl = true;
            else
                m_IsSsl = false;

            RequestUri = uri;

            if (uri.Port == 80)
                Headers.Add ("Host", uri.Host);
            else
                Headers.Add ("Host",uri.Host + ":" + uri.Port);

            KeepAlive = keepAlive;
            if (KeepAlive)
                Headers.Add("Connection", "Keep-Alive");

        }

        ~HttpWebRequest()
        {
            //this.Dispose();
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
        public HttpWebResponse GetResponse()
        {
            if (response == null || response.socket == null || response.socket.Connected == false)
            {
                response = new HttpWebResponse(this);
                response.Connect(this);

                response.SendRequest(this);
            }
            else
            {
                //如果连接还存在，则直接发送请求
                response.SendRequestContinue(this);
            }

            //接收数据
            response.Receive(this);

            response.Close();

            return response;

        }

    }
}
