using System.Text;
using Soukey;
using Soukey.Task;
using Soukey.Gather;
using System.Data.SqlClient;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using SoukeyResource;
using System.ServiceProcess;

namespace SoukeyService
{
 
    partial class GatherClientServer : ServiceBase
    {
        SoukeyEngine.Management.cControl sEngine = null;
        public GatherClientServer()
        {
            InitializeComponent();

            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.CanHandleSessionChangeEvent = true;
            ServiceName = "SMGatherClient";
            this.
            sEngine = new SoukeyEngine.Management.cControl();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: 在此处添加代码以启动服务。
            bool isS = sEngine.StartClientGather();
        }

        protected override void OnStop()
        {
            // TODO: 在此处添加代码以执行停止服务所需的关闭操作。
            bool isS = sEngine.StopClientGather();
        }
    }
}
