using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace NetMiner.Net.Socket
{
   
    internal class cAsyncSocket
    {
        public static Hashtable g_Host = new Hashtable();
        public cAsyncSocket(string workPath)
        {
           
        }

        ~cAsyncSocket()
        {
        }

        public static string GetHostIP(string domain)
        {
            string s=string.Empty ;
            foreach (DictionaryEntry de in g_Host)
            {
                if (de.Key.ToString() ==domain)
                {
                    s=de.Value.ToString () ;
                    break ;
                }
            }
            return s;
        }

        public static void AddHostIP(string domain, string ip)
        {
            g_Host.Add(domain, ip);
        }

        System.Net.Sockets. Socket socket;
        const int RECEIVECOUNT = 4096;
        List<byte> m_Bytes = new List<byte>();
        ManualResetEvent wait = new ManualResetEvent(false );
        string websource = string.Empty;

        public string GetHtml(string url)
        {
            socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP;

            Uri ResponseUri = new Uri(url);

            string domain = ResponseUri.Host.Trim();
            string ip = GetHostIP(domain);
            IPAddress ipAdd=null;
            if (ip == "")
            {
                ipAdd = Dns.GetHostAddresses(ResponseUri.Host.Trim())[0];
            }
            else
            {
                ipAdd = IPAddress.Parse(ip);
            }

            remoteEP = new IPEndPoint(ipAdd, ResponseUri.Port);

          
            //连接远程
            socket.Connect(remoteEP);

            //发送数据
            string strHeader = "GET /13/1214/20/9G34NR3D0001124J.html HTTP/1.1\r\n"
                + "Host: www.163.com\r\n"
                + "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0\r\n\r\n";

            byte[] sBytes = Encoding.ASCII.GetBytes(strHeader);

            //发送信息
            socket.Send(sBytes);

            //开始接受头信息
            SocketStateObject so2 = new SocketStateObject(socket);
            IAsyncResult ia = socket.BeginReceive(so2.buffer, 0, SocketStateObject.BUFFER_SIZE, 0, new AsyncCallback(AsynchReadCallback), so2);
            wait.WaitOne();

            byte[] bytes=new byte[m_Bytes.Count ];
            bytes=m_Bytes.ToArray ();

            websource= ConvertCommonSource(bytes, "");

            return websource;
        }


        private void AsynchReadCallback(System.IAsyncResult ar)
        {
            SocketStateObject so = (SocketStateObject)ar.AsyncState;
            System.Net.Sockets.Socket s = so.WorkSocket;

            try
            {
                // sanity check
                if (s == null || !s.Connected)
                {
                    wait.Set();
                    return;
                }
                int read = s.EndReceive(ar);
                if (read > 0)
                {
                    m_Bytes.AddRange(so.buffer);

                    if (m_Bytes.Count > 16000)
                    {
                        wait.Set();
                        return;
                    }
                    // and start recieving more
                    s.BeginReceive(so.buffer, 0, SocketStateObject.BUFFER_SIZE, 0, new AsyncCallback(AsynchReadCallback), so);
                }
                else
                {
                    wait.Set();
                }
            }
            catch { wait.Set(); }
        }

     

        private string ConvertCommonSource(byte[] bytes, string webCode)
        {
            Encoding wCode;
            Stream respStream = new MemoryStream(bytes);
            //定义一个字节数组
            byte[] buffer = new byte[0x400];

            //定义一个流，将数据读出来
            MemoryStream mReader = new MemoryStream();
            for (int i = respStream.Read(buffer, 0, buffer.Length); i > 0; i = respStream.Read(buffer, 0, buffer.Length))
            {
                mReader.Write(buffer, 0, i);
            }

            string strChar = Encoding.ASCII.GetString(mReader.ToArray());

            if (webCode == "")
            {
                Match charSetMatch = Regex.Match(strChar, "(?<=charset=)([^<]+?)(?=[\"|\']+?)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string webCharSet = charSetMatch.ToString();
                webCharSet = Regex.Replace(webCharSet, "[\"|']", "");
                if (webCharSet != "")
                    wCode = System.Text.Encoding.GetEncoding(webCharSet);
                else
                    wCode = Encoding.Default;
            }
            else
            {
                wCode = Encoding.GetEncoding(webCode);
            }

            mReader.Seek((long)0, SeekOrigin.Begin);
            StreamReader reader = new StreamReader(mReader, wCode);
            string strWebData = reader.ReadToEnd();

            mReader.Close();
            mReader.Dispose();
            reader.Close();
            reader.Dispose();

            return strWebData;
        }
    }

    
}
