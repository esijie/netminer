using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using NetMiner.IPC.Server;
using System.Runtime.InteropServices;

/// <summary>
/// 采用.Net Remoting通讯
/// </summary>
namespace NetMiner.WebEngine
{
    static class Program
    {

        private static ApplicationContext context;
        public static string g_xulPath;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RunWebEngine(args);

        }

        public static void RunWebEngine(string[] args)
        {
            //判断引擎是否已经启动，如果启动，则退出
            if (System.Diagnostics.Process.GetProcessesByName("NetMiner.WebEngine.exe").Length > 1)
            {
                Process instance = RunningInstance();
                if (instance == null)
                    return;
                else
                {
                    HandleRunningInstance(instance);
                    return;
                }
            }

            if (args.Length > 0 )
            {

                //验证用户名是否存在，如果不存在，则退出
                string userName = args[0].ToLower();

                //开始建立本地通讯
                IpcChannel nChannel = new IpcChannel("netminerIPC");
                ChannelServices.RegisterChannel(nChannel, false);

                context = new ApplicationContext();
                Application.Idle += new EventHandler(Application_Idle);
                Application.Run(context);

            }
            
        }

        public static frmMain mf;
        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);

            //IniWCF();

            //cControl control = new WebEngine.cControl();

            mf= new frmMain();
            context.MainForm = mf;
            mf.Show();
        }

        
       

        public static Process RunningInstance()
        {

            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
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

        public static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWMax);
            SetForegroundWindow(instance.MainWindowHandle);
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWMax = 3;

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";
        }
    }
}
