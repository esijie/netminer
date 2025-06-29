using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.IO;

///这部分的socket通讯，是发布时候用到的
namespace NetMiner.Common.HttpSocket
{
    public enum SocketOptionLevel
    {
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 IP 套接字。
        IP = 0,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 TCP 套接字。
        Tcp = 6,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 UDP 套接字。
        Udp = 17,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 IPv6 套接字。
        IPv6 = 41,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项适用于所有套接字。
        Socket = 65535,
    }

    // 摘要:
    //     指定 System.Net.Sockets.Socket 类的实例可以使用的寻址方案。
    public enum AddressFamily
    {
        // 摘要:
        //     未知的地址族。
        Unknown = -1,
        //
        // 摘要:
        //     未指定的地址族。
        Unspecified = 0,
        //
        // 摘要:
        //     Unix 本地到主机地址。
        Unix = 1,
        //
        // 摘要:
        //     IP 版本 4 的地址。
        InterNetwork = 2,
        //
        // 摘要:
        //     ARPANET IMP 地址。
        ImpLink = 3,
        //
        // 摘要:
        //     PUP 协议的地址。
        Pup = 4,
        //
        // 摘要:
        //     MIT CHAOS 协议的地址。
        Chaos = 5,
        //
        // 摘要:
        //     IPX 或 SPX 地址。
        Ipx = 6,
        //
        // 摘要:
        //     Xerox NS 协议的地址。
        NS = 6,
        //
        // 摘要:
        //     OSI 协议的地址。
        Osi = 7,
        //
        // 摘要:
        //     ISO 协议的地址。
        Iso = 7,
        //
        // 摘要:
        //     欧洲计算机制造商协会 (ECMA) 地址。
        Ecma = 8,
        //
        // 摘要:
        //     Datakit 协议的地址。
        DataKit = 9,
        //
        // 摘要:
        //     CCITT 协议（如 X.25）的地址。
        Ccitt = 10,
        //
        // 摘要:
        //     IBM SNA 地址。
        Sna = 11,
        //
        // 摘要:
        //     DECnet 地址。
        DecNet = 12,
        //
        // 摘要:
        //     直接数据链接接口地址。
        DataLink = 13,
        //
        // 摘要:
        //     LAT 地址。
        Lat = 14,
        //
        // 摘要:
        //     NSC Hyperchannel 地址。
        HyperChannel = 15,
        //
        // 摘要:
        //     AppleTalk 地址。
        AppleTalk = 16,
        //
        // 摘要:
        //     NetBios 地址。
        NetBios = 17,
        //
        // 摘要:
        //     VoiceView 地址。
        VoiceView = 18,
        //
        // 摘要:
        //     FireFox 地址。
        FireFox = 19,
        //
        // 摘要:
        //     Banyan 地址。
        Banyan = 21,
        //
        // 摘要:
        //     本机 ATM 服务地址。
        Atm = 22,
        //
        // 摘要:
        //     IP 版本 6 的地址。
        InterNetworkV6 = 23,
        //
        // 摘要:
        //     Microsoft 群集产品的地址。
        Cluster = 24,
        //
        // 摘要:
        //     IEEE 1284.4 工作组地址。
        Ieee12844 = 25,
        //
        // 摘要:
        //     IrDA 地址。
        Irda = 26,
        //
        // 摘要:
        //     支持网络设计器 OSI 网关的协议的地址。
        NetworkDesigners = 28,
        //
        // 摘要:
        //     MAX 地址。
        Max = 29,
    }

    public enum ProxyType
    {
        None=1000,
        HttpProxy=1001,
        Scoket5=1002,
    }

    public class MyWebRequest
    {
        public MyWebRequest(Uri uri, bool bKeepAlive,bool isProxy,ProxyType  pType)
        {
            IsProxy = isProxy;
            proxyType = pType;

            Headers = new WebHeaderCollection();
            RequestUri = uri;
            
            if (uri.Port == 80)
                Headers["Host"] = uri.Host;
            else
                Headers["Host"] = uri.Host + ":" + uri.Port;
           

            KeepAlive = bKeepAlive;
            if (KeepAlive)
                Headers["Connection"] = "Keep-Alive";
            //Method = "GET";
        }

        public static MyWebRequest Create(Uri uri, MyWebRequest AliveRequest, bool bKeepAlive,bool isProxy,ProxyType pType)
        {
            if (bKeepAlive &&
                AliveRequest != null &&
                AliveRequest.response != null &&
                AliveRequest.response.KeepAlive &&
                AliveRequest.response.socket.Connected &&
                AliveRequest.RequestUri.Host == uri.Host)
            {
                AliveRequest.RequestUri = uri;
                return AliveRequest;
            }
            return new MyWebRequest(uri, bKeepAlive,isProxy ,pType );
        }

        public MyWebResponse GetResponse(NetMiner.Resource.cGlobalParas.WebCode uCode)
        {
            if (response == null || response.socket == null || response.socket.Connected == false)
            {
                response = new MyWebResponse();
                response.Connect(this);
                //response.SendRequest(this);
                response.SetTimeout(Timeout);
            }
            response.SendRequest(this, uCode);
            response.ReceiveHeader();
            return response;
        }

        public MyWebResponse GetResponseUpload(NetMiner.Resource.cGlobalParas.WebCode uCode, string fName)
        {
            if (response == null || response.socket == null || response.socket.Connected == false)
            {
                response = new MyWebResponse();
                response.Connect(this);
                //response.SendRequest(this);
                response.SetTimeout(Timeout);
            }

            response.SendRequest(this, uCode, fName);
            response.ReceiveHeader();
            return response;
        }

        public int Timeout;
        public WebHeaderCollection Headers;
        public string Header;
        public Uri RequestUri;
        public string Method;
        public MyWebResponse response;
        public bool KeepAlive;

        public bool IsProxy;
        public ProxyType proxyType;
        public string httpProxyAddress;
        public int httpProxyPort;
    }
    public class MyWebResponse
    {

        public MyWebResponse()
        {
        }



        public void Connect(MyWebRequest request)
        {
            ResponseUri = request.RequestUri;

            socket = new Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //IPEndPoint remoteEP = new IPEndPoint(Dns.Resolve(ResponseUri.Host).AddressList[0], ResponseUri.Port);

            //socket.Connect(remoteEP);

            if (request.IsProxy == true)
            {
                socket.Connect(request.httpProxyAddress,request.httpProxyPort);
            }
            else
            {
                socket.Connect(ResponseUri.Host, ResponseUri.Port);
            }

        }
        public void SendRequest(MyWebRequest request,NetMiner.Resource.cGlobalParas.WebCode uCode)
        {
            ResponseUri = request.RequestUri;

            string[] strHeaders =Regex.Split ( request.Header,"\r\n");
 
            //判断host是否存在，如果不存在，则加入

            bool isHost = false;
            for (int i = 0; i < strHeaders.Length; i++)
            {
                if (strHeaders[i].StartsWith("Host", StringComparison.CurrentCultureIgnoreCase))
                {
                    isHost = true;
                    break;
                }
            }

            string aUrl = "";
            if (request.IsProxy == true)
            {
                aUrl = ResponseUri.AbsoluteUri;
            }
            else
            {
                aUrl = ResponseUri.PathAndQuery;
            }
            if (isHost == false)
            {
                if (request.Header == "")
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n\r\n";
                }
                else
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n"
                        + request.Header;
                }
            }
            else
            {
                request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n" + request.Header;
            }

            switch (uCode)
            {
                case NetMiner.Resource.cGlobalParas.WebCode.auto :
                    socket.Send(Encoding.ASCII.GetBytes(request.Header));
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.big5:
                    socket.Send(Encoding.GetEncoding ("big5").GetBytes(request.Header));
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gb2312:
                    socket.Send(Encoding.GetEncoding("gb2312").GetBytes(request.Header));
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gbk:
                    socket.Send(Encoding.GetEncoding("gbk").GetBytes(request.Header));
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.NoCoding:
                    socket.Send(Encoding.ASCII.GetBytes(request.Header));
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.utf8:
                    socket.Send(Encoding.UTF8.GetBytes(request.Header));
                    break;
            }
            
        }

        public void SendRequest(MyWebRequest request, NetMiner.Resource.cGlobalParas.WebCode uCode, string fName)
        {
            ResponseUri = request.RequestUri;

            string[] strHeaders = Regex.Split(request.Header, "\r\n");

            //判断host是否存在，如果不存在，则加入

            bool isHost = false;
            for (int i = 0; i < strHeaders.Length; i++)
            {
                if (strHeaders[i].StartsWith("Host", StringComparison.CurrentCultureIgnoreCase))
                {
                    isHost = true;
                    break;
                }
            }

            string aUrl = "";
            if (request.IsProxy == true)
            {
                aUrl = ResponseUri.AbsoluteUri;
            }
            else
            {
                aUrl = ResponseUri.PathAndQuery;
            }
            if (isHost == false)
            {
                if (request.Header == "")
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n\r\n";
                }
                else
                {
                    request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n"
                        + "Host:" + request.Headers["Host"].ToString() + "\r\n"
                        + request.Header;
                }
            }
            else
            {
                request.Header = request.Method + " " + aUrl + " HTTP/1.1\r\n" + request.Header;
            }


            //request.Header = request.Method + " " + ResponseUri.PathAndQuery + " HTTP/1.1\r\n" + request.Header;

            //将文件用二进制读出来
            FileStream fs = new FileStream(fName, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            int fLength = (int)fs.Length;
            byte[] filebyte = new byte[fLength];
            filebyte = br.ReadBytes(fLength);
            br.Close();
            br = null;
            fs.Close();
            fs.Dispose();

            byte[] hdHead = null;
            byte[] hdEnd = null;

            string header=request.Header ;
            string[] headers=Regex.Split (header,"\r\n");
            string startHeader = "";
            string endHeader = "";

            bool isStart = true;
            for (int i=0;i<headers.Length ;i++)
            {
                
                    if (headers[i].StartsWith("Content-Type") && headers[i].IndexOf("--") == -1)
                    {
                        if (isStart == true)
                        {
                            startHeader += headers[i] + "\r\n\r\n";
                            isStart = false;
                        }
                        else
                        {
                            endHeader += headers[i] + "\r\n";
                        }
                    }
                    else
                    {
                        if (isStart == true)
                        {
                            startHeader += headers[i] + "\r\n";
                        }
                        else
                        {
                            endHeader += headers[i] + "\r\n";
                        }
                    }

            }

            //if (Regex.Replace(endHeader, "\r\n", "", RegexOptions.IgnoreCase) != ""
            //    && endHeader.EndsWith("\r\n"))
            //    endHeader = endHeader.Substring(0, endHeader.Length - 2);

            switch (uCode)
            {
                case NetMiner.Resource.cGlobalParas.WebCode.auto:
                    hdHead = Encoding.ASCII.GetBytes(startHeader);
                    hdEnd = Encoding.ASCII.GetBytes(endHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.big5:
                    hdHead = Encoding.GetEncoding("big5").GetBytes(startHeader);
                    hdEnd = Encoding.GetEncoding("big5").GetBytes(endHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gb2312:
                    hdHead = Encoding.GetEncoding("gb2312").GetBytes(startHeader);
                    hdEnd = Encoding.GetEncoding("gb2312").GetBytes(endHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.gbk:
                    hdHead = Encoding.GetEncoding("gbk").GetBytes(startHeader);
                    hdEnd = Encoding.GetEncoding("gbk").GetBytes(endHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.NoCoding:
                    hdHead = Encoding.ASCII.GetBytes(startHeader);
                    hdEnd = Encoding.ASCII.GetBytes(endHeader);
                    break;
                case NetMiner.Resource.cGlobalParas.WebCode.utf8:
                    hdHead = Encoding.UTF8.GetBytes(startHeader);
                    hdEnd = Encoding.UTF8.GetBytes(endHeader);
                    break;
            }

            byte[] sendhd = new byte[hdHead.Length + hdEnd.Length + filebyte.Length];
            hdHead.CopyTo(sendhd, 0);
            filebyte.CopyTo(sendhd, hdHead.Length);
            hdEnd.CopyTo(sendhd, hdHead.Length + filebyte.Length);
            socket.Send(sendhd);
        }

        public void SetTimeout(int Timeout)
        {
            socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.SendTimeout, Timeout * 1000);
            socket.SetSocketOption(System.Net.Sockets.SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, Timeout * 1000);
        }

        public void ReceiveHeader()
        {
            Header = "";
            Headers = new WebHeaderCollection();

            byte[] bytes = new byte[10];

            List<byte> hList = new List<byte>();
            while (socket.Receive(bytes, 0, 1, SocketFlags.None) > 0)
            {
                Header += Encoding.ASCII.GetString(bytes, 0, 1);

                hList.Add(bytes[0]);

                if (bytes[0] == '\n' && Header.EndsWith("\r\n\r\n"))
                    break;
            }

            this.HeaderBytes =new byte[hList.Count ];
            HeaderBytes = hList.ToArray();

            MatchCollection matches = new Regex("[^\r\n]+").Matches(Header.TrimEnd('\r', '\n'));
            for (int n = 1; n < matches.Count; n++)
            {
                string[] strItem = matches[n].Value.Split(new char[] { ':' }, 2);
                if (strItem.Length > 0)
                {
                    try
                    {
                        Headers[strItem[0].Trim()] = strItem[1].Trim();
                    }
                    catch { }
                }
            }
            // check if the page should be transfered to another location
            if (matches.Count > 0 && (
                matches[0].Value.IndexOf(" 302 ") != -1 ||
                matches[0].Value.IndexOf(" 301 ") != -1))
                // check if the new location is sent in the "location" header
                if (Headers["Location"] != null)
                {
                    try { ResponseUri = new Uri(Headers["Location"]); }
                    catch { ResponseUri = new Uri(ResponseUri, Headers["Location"]); }
                }
            ContentType = Headers["Content-Type"];
            if (Headers["Content-Length"] != null)
                ContentLength = int.Parse(Headers["Content-Length"]);
            KeepAlive = (Headers["Connection"] != null && Headers["Connection"].ToLower() == "keep-alive") ||
                        (Headers["Proxy-Connection"] != null && Headers["Proxy-Connection"].ToLower() == "keep-alive");
        }
        public void Close()
        {
            socket.Close();
        }
        public Uri ResponseUri;
        public string ContentType;
        public int ContentLength;
        public WebHeaderCollection Headers;
        public string Header;
        public Socket socket;
        public bool KeepAlive;

        public byte[] HeaderBytes;
    }
}
