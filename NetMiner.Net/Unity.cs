using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Net.Common;
using NetMiner.Resource;
using System.Diagnostics;
using System.Reflection;
using NetMiner.IPC;
using NetMiner.IPC.Client;
//using Gecko;
using System.Net;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 从V5.5版本开始，web请求统一至此接口，支持浏览器获取网页源码
/// </summary>
namespace NetMiner.Net
{
    public static class Unity
    {
        private static cClientSubscribe sub;
        private static bool m_isConnectWebEngine
        {
            get { if (sub == null) return false; else return sub.isConnected(); }
        }

        public static eResponse RequestUri(string workPath, eRequest request, bool isGecko)
        {
            eResponse response = new Common.eResponse();

            if (isGecko)
            {
                //调用浏览器进行网页源码的获取
                //检测web引擎的进程是否存在，如果不存在则启动

                //注册连接，并开始请求
                if (m_isConnectWebEngine == false)
                {
                    sub = null;
                    ConnectServer();
                }

                //开始调用网页源码

                NetMiner.IPC.Server.eResponse res = null;

                try
                {
                    res = sub.GetHtml(request.uri.AbsoluteUri);
                }
                catch (System.Exception ex)
                {
                    throw ex;
                }

                string cookie = res.Cookie;
                CookieCollection cCookie = new CookieCollection();
                foreach (string sc in cookie.Split(';'))
                {
                    string ss = sc.Trim();

                    string s1 = ss.Substring(0, ss.IndexOf("="));
                    string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

                    if (s2.IndexOf(",") > 0 || s2.IndexOf(";") > 0)
                    {
                        s2 = s2.Replace(",", "%2c");
                        s2 = s2.Replace(";", "%3b");
                    }

                    cCookie.Add(new System.Net.Cookie(s1, s2, "/"));
                }
                response.rUri = new Uri(request.uri.AbsoluteUri);
                response.cookie = cCookie;

                response.Body = res.HtmlSource;

            }
            else
            {
                if (request.ProxyType==cGlobalParas.ProxyType.Socket5)
                {
                    //调用Scoket进行数据获取
                }
                else
                {
                    //正常调用获取数据
                    Native.HttpUnity hunity = new Native.HttpUnity(workPath);
                    response= hunity.RequestUri(request.uri, request, request.Method, request.webProxy);
                    hunity.Dispose();
                    hunity = null;

                }
            }

            return response;
        }

        
        private static void ConnectServer()
        {

            if (System.Diagnostics.Process.GetProcessesByName("NetMiner.WebEngine").Length >= 1)
            {
                Process instance = RunningInstance();
                if (instance == null)
                {
                    //启动web引擎
                    System.Diagnostics.Process.Start("NetMiner.WebEngine.exe", "yijie");
                    System.Threading.Thread.Sleep(3000);
                }
            }
            else
            {
                System.Diagnostics.Process.Start("NetMiner.WebEngine.exe", "yijie");
                System.Threading.Thread.Sleep(3000);
            }

                string serviceUri = "net.pipe://localhost/netminerWebEngineService/";

            try
            {
                sub = new cClientSubscribe(serviceUri);
                sub.ReceiveMessage += on_GetHtmlMessage;
                sub.Subscribe();
            }
            catch (Exception ex)
            {
                return;
            }

        }

        private static void on_GetHtmlMessage(object sender, rMessageEvent e)
        {
            
        }

        public static void DownloadFile()
        {

        }

        public static Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName("NetMiner.WebEngine");
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        //public static void HandleRunningInstance(Process instance)
        //{
        //    ShowWindowAsync(instance.MainWindowHandle, WS_SHOWMax);
        //    SetForegroundWindow(instance.MainWindowHandle);
        //}
    }
}
