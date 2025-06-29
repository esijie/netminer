using System.Text;
using NetMiner.Gather;
using NetMiner.Gather.Task;
using NetMiner.Gather.Control;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using NetMiner.Resource;
using System.ServiceProcess;
using System.ServiceModel;
using System.Threading;

namespace SoukeySplitService
{
 
    partial class GatherClientServer : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public GatherClientServer()
        {
            InitializeComponent();

            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanHandleSessionChangeEvent = true;
            ServiceName = "SMGatherClient";

        }

        protected override void OnStart(string[] args)
        {
            //ServiceBase.Run(new GatherClientServer());

            // TODO: 在此处添加代码以启动服务。

            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            serviceHost = new ServiceHost(typeof(cDistriGatherEngine));

            cGlobal.sEngine = new NetMiner.Engine.ClientEngine.cSplitControl();
            bool isS = cGlobal.sEngine.StartClientGather();

            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            bool isS = cGlobal.sEngine.StopClientGather();
            cGlobal.sEngine = null;
        }
    }
}
