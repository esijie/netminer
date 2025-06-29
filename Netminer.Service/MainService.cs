using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Gather.Control;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using NetMiner.Resource;
using System.ServiceModel;
using System.Threading;
using System.Net;
using NetMiner.Interface.WCF;

namespace SoukeyService
{
    //public enum MainServiceCustomCommands { StopWorker = 128, RestartWorker, CheckWorker };
    [StructLayout(LayoutKind.Sequential)]
    public struct SERVICE_STATUS
    {
        public int serviceType;
        public int currentState;
        public int controlsAccepted;
        public int win32ExitCode;
        public int serviceSpecificExitCode;
        public int checkPoint;
        public int waitHint;
    }

    public enum State
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    partial class MainService : ServiceBase
    {
        public List<ServiceHost> serviceHosts = null;
        public ServiceHost serviceHost = null;

        [DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus")]
        public static extern bool SetServiceStatus(
                        IntPtr hServiceStatus,
                        SERVICE_STATUS lpServiceStatus
                        );
        private SERVICE_STATUS myServiceStatus;

        public MainService()
        {
            InitializeComponent();
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanHandleSessionChangeEvent = true;
            ServiceName = "SMGatherService";
        }

        protected override void OnStart(string[] args)
        {
            serviceHosts = new List<ServiceHost>();
            //Thread.Sleep(20000);

            //ServiceBase.Run(new MainService());

            //先开始读取配置
            NetMiner.Engine.cXmlSConfig sCon = new NetMiner.Engine.cXmlSConfig();
            string bindAddress = sCon.BindAddress;
            int bindPort = sCon.BindPort;
            sCon = null;

            // TODO: 在此处添加代码以启动服务。

            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            cGlobal.sEngine = new NetMiner.Engine. ServerEngine.cControl();
            cGlobal.sEngine.Log += on_Log;
            bool isS = cGlobal.sEngine.Start();

            #region 加载调用的webservice接口
            serviceHost = new ServiceHost(typeof(cGatherControlImpl));

            serviceHost.Open();
            serviceHosts.Add(serviceHost);
           
            #endregion

            #region 处理wcf服务的启动事项
            string ip = bindAddress;
            IPAddress ipAddr;
            if (args.Length > 0 && IPAddress.TryParse(args[0], out ipAddr))
            {
                ip = ipAddr.ToString();
            }
            int port = bindPort;
            int tempPort;
            if (args.Length > 1 && int.TryParse(args[1], out tempPort))
            {
                port = tempPort;
            }
            string uri = string.Format("net.tcp://{0}:{1}", ip, port);
            NetTcpBinding binding = new NetTcpBinding();
            binding.ReceiveTimeout = TimeSpan.MaxValue;//设置连接自动断开的空闲时长；

            serviceHost = new ServiceHost(typeof(cMessagePublishServiceImpl));
            serviceHost.AddServiceEndpoint(typeof(iMessagePublisher), binding, uri);

            Console.WriteLine("启动消息发布服务……接入地址：{0}", uri);

            MessageCenter.Instance.ListenerAdded += new EventHandler<MessageListenerEventArgs>(Instance_ListenerAdded);
            MessageCenter.Instance.ListenerRemoved += new EventHandler<MessageListenerEventArgs>(Instance_ListenerRemoved);
            MessageCenter.Instance.NotifyError += new EventHandler<MessageNotifyErrorEventArgs>(Instance_NotifyError);

            serviceHost.Open();
            serviceHosts.Add(serviceHost);
            #endregion


        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            cGlobal.sEngine.Stop();

            serviceHosts = null;
        }

        static void on_Log(object sender, NetMiner.Core.Event.cGatherTaskLogArgs e)
        {
            int type = 0;
            if (e.TaskName.IndexOf("___") > -1)
                type = 1;
            else
                type = 0;

            MessageCenter.Instance.NotifyMessage(e.strLog );
        }

        static void Instance_NotifyError(object sender, MessageNotifyErrorEventArgs e)
        {
            Console.WriteLine("[{0}]消息发送失败！--IP:{1}; Port:{2}; Error:{3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Listener.FromIP, e.Listener.FromPort, e.Error.Message);
            Console.WriteLine("移除无效监听器……");
            MessageCenter.Instance.RemoveListener(e.Listener);
        }

        static void Instance_ListenerRemoved(object sender, MessageListenerEventArgs e)
        {
            Console.WriteLine("[{0}]取消订阅-- From: {1}:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Listener.FromIP, e.Listener.FromPort);
        }

        static void Instance_ListenerAdded(object sender, MessageListenerEventArgs e)
        {
            Console.WriteLine("[{0}]订阅消息--From: {1}:{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), e.Listener.FromIP, e.Listener.FromPort);
        }

    }
}
