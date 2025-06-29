using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Common.Tool;

//程序入口类
namespace MinerSpider
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        /// 
        private static ApplicationContext context;

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RunSoMiner(args);
        }

        public static void RunSoMiner(string[] args)
        {

            //首先检测只允许启动一个网络矿工

            bool blnIsRunning;
            //Mutex mutexApp = new Mutex(false, "Global\\" + Assembly.GetExecutingAssembly().FullName, out   blnIsRunning);
            //if (!blnIsRunning)

            if (args.Length > 0 && args[1].ToLower() == "nosingle")
            {

            }
            else
            {
                if (System.Diagnostics.Process.GetProcessesByName("MinerSpider").Length > 1)
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
            }

            frmStart sf = new frmStart();
            sf.Show();
            context = new ApplicationContext();
            context.Tag = sf;
            Application.Idle += new EventHandler(Application_Idle); //注册程序运行空闲去执行主程序窗体相应初始化代码
            Application.Run(context);
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

        private static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            if (context.MainForm == null)
            {
                frmStart sf = (frmStart)context.Tag;

                if (cLanguage != null)
                {
                    Thread.CurrentThread.CurrentUICulture = cLanguage;
                    Thread.CurrentThread.CurrentCulture = cLanguage;
                }

                //判断当前的系统是32位还是64位
                bool is64;
                is64 = Environment.Is64BitOperatingSystem;

                if (is64)
                {
                    g_xulPath = g_xulPath + "\\64";
                }
                else
                    g_xulPath = g_xulPath + "\\32";

                ResourceManager rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

                //检测系统版本
                cVersion sV = new cVersion(Program.getPrjPath());
                
                sV.ReadRegisterInfo();

                g_RegInfo = sV.RegisterInfo;

                SominerVersion = (cGlobalParas.VersionType)sV.SominerVersion;
                Program.RegisterUser = sV.RegisterInfo.User;

                if (sV.RegisterInfo.User!="sominer")
                sf.rUser.Text = sV.RegisterInfo.User;
                Application.DoEvents();

                //在此先判断授权编码是不是一个日期，如果是一个日期，则表示是登录用户
                
                if(DateTime.TryParse(g_RegInfo.Keys,out g_EndServiceDate))
                {
                    //表示是一个登录的用户
                    Program.g_isLogin = true;

                }


                if (SominerVersion == cGlobalParas.VersionType.Program )
                {
                    //标识已经在线激活，需要判断主版本号是否一致，如果不一致，则需要
                    //告知用户无法升级使用

                    //获取当前版本号
                    string CurVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    string RegVersionCode = sV.SominerVersionCode;

                    SominerVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    SominerRegVersionCode = sV.SominerVersionCode;

                    //如果主版本不对，或者版本的类别不对，则都禁止使用
                    if (CurVersionCode != RegVersionCode)
                    {
                        MessageBox.Show(rm.GetString("Error38"), rm.GetString("MessageboxError"),MessageBoxButtons.OK ,MessageBoxIcon.Error );
                        Environment.Exit(0);
                    }
                   
                }
                else if (SominerVersion == cGlobalParas.VersionType.Ultimate)
                {
                    //标识已经在线激活，需要判断主版本号是否一致，如果不一致，则需要
                    //告知用户无法升级使用

                    //获取当前版本号
                    string CurVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    string RegVersionCode = sV.SominerVersionCode;

                    SominerVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    SominerRegVersionCode = sV.SominerVersionCode;

                    //如果主版本不对，或者版本的类别不对，则都禁止使用
                    if (CurVersionCode != RegVersionCode)
                    {
                        MessageBox.Show(rm.GetString("Error38"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }


                }
                else if (SominerVersion == cGlobalParas.VersionType.Enterprise)
                {
                    //获取当前版本号
                    string CurVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    string RegVersionCode = sV.SominerVersionCode;

                    SominerVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    SominerRegVersionCode = sV.SominerVersionCode;

                    //如果主版本不对，或者版本的类别不对，则都禁止使用
                    if (CurVersionCode != RegVersionCode)
                    {
                        MessageBox.Show(rm.GetString("Error38"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                else if (SominerVersion==cGlobalParas.VersionType.Cloud)
                {
                    //获取当前版本号
                    string CurVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    string RegVersionCode = sV.SominerVersionCode;

                    SominerVersionCode = ToolUtil.Mearger(SominerVersionCode, sV.SominerVersionCode);
                    SominerRegVersionCode = sV.SominerVersionCode;

                    //如果主版本不对，或者版本的类别不对，则都禁止使用
                    if (CurVersionCode != RegVersionCode)
                    {
                        MessageBox.Show(rm.GetString("Error38"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Environment.Exit(0);
                    }
                }
                else if (SominerVersion == cGlobalParas.VersionType.Free)
                {

                    //表示为免费版本
                    SominerVersionCode = sV.SominerVersionCode;
                    SominerRegVersionCode = sV.SominerVersionCode;
                }
               
                sV = null;

                //检测是否指定了界面语言
                try
                {
                    cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath());
                    cGlobalParas.CurLanguage cl = Config.CurrentLanguage;
                    g_SerarchUrl = Config.SearchUrl;
                    g_SearchResult = Config.SearchResult;

                    switch (cl)
                    {
                        case cGlobalParas.CurLanguage.Auto:
                            break;
                        case cGlobalParas.CurLanguage.enUS:
                            cLanguage = new CultureInfo("en-US");
                            break;
                        case cGlobalParas.CurLanguage.zhCN:
                            cLanguage = new CultureInfo("zh-CN");
                            break;
                        default:
                            break;
                    }

             

                    if (SominerVersion == cGlobalParas.VersionType.Enterprise)
                    {
                        //定义远程服务器地址
                        Program.ConnectServer = Config.RemoteServer;
                        Program.g_IsAuthen = Config.IsAuthen;
                        Program.g_WindowsUser = Config.windowsUser;
                        Program.g_WindowsPwd = Config.windowsPwd;

                    }
                    else if (SominerVersion==cGlobalParas.VersionType.Cloud)
                    {
                        //定义远程服务器地址
                        Program.ConnectServer = "http://spider.netminer.cn";
                        Program.g_IsAuthen = false;
                        Program.g_WindowsUser = string.Empty ;
                        Program.g_WindowsPwd = string.Empty ;
                    }

                    Config = null;
                }
                catch (System.Exception)
                {

                }


                //系统正常初始化工作
                frmMain mf = new frmMain ();
                context.MainForm = mf;
                
                //初始化界面信息
                sf.label3.Text = rm.GetString ("Info69");
                Application.DoEvents();
                mf.IniForm();
                mf.IniHelp();

                //初始化代理信息
                //sf.label3.Text = rm.GetString("Info269");
                //Application.DoEvents();
                //mf.IniProxy();

                //初始化对象并开始启动运行区的任务
                sf.label3.Text =  rm.GetString ("Info70");
                Application.DoEvents();

                //初始化对象并开始启动运行区的任务
                sf.label3.Text = rm.GetString("Info71");
                Application.DoEvents();
                mf.IniTaskData ();

                sf.label3.Text = rm.GetString ("Info239");
                Application.DoEvents();
                //mf.IniMonitor ();

                //初始化远程服务器管理窗口
                mf.IniRemoteServer();

                mf.IniSetForm();

                //加载帮助信息
                SominerHelp = new Help.cHelpInfo();

                rm = null;

                
                
                //根据运行模式进行判断，当前是否为
                //后台服务的运行模式
                mf.WindowState = FormWindowState.Maximized;
                mf.Show();                                  //启动主程序窗体

                sf.Close();                                 //关闭启动窗体
                sf.Dispose();


            }
        }

        //全局变量，标识此系统的版本
        public static cGlobalParas.VersionType SominerVersion;

        //全局软件发布版本号，用于系统验证使用
        public static string SominerShowVersion = "2018";
        public static string SominerVersionCode = "5.5.0.02";
        public static string SominerServerVersion = "5.5.0.06";
        public static string SominerRegVersionCode="";
        //public static string SominerVersionPersonal = "个人版 V2018";
        public static string SominerVersionProgram = "开发版 V5.5";
        public static string SominerVersionFree = "免费版 V5.5";
        public static string SominerVersionEnter = "企业版 V5.5";
        public static string SominerVersionUltimate = "旗舰版 V5.5";
        public static string SominerVersionCloud = "云采版 V5.5";

        //定义一个远程任务分类
        public const string g_RemoteTaskClass = "远程";
        public const string g_RemoteTaskPath = "tasks\\RemoteTask";
        public static cVersion.StructRegisterInfo g_RegInfo;

        //定义一个全局变量，用于记录当前注册用户，如果为试用版，则为空
        public static string RegisterUser = string.Empty;

        //定义一个全局变量，指示当前是否为静默运行方式
        //public static bool IsSilent = false;


        //定义一个全局变量，记录远程服务器的地址
        public static string ConnectServer;
        public static bool g_IsAuthen = false;
        public static string g_WindowsUser;
        public static string g_WindowsPwd;

        //定义一个全局变量，记录云采集的地址
        public static string g_CloudServer ="http://spider.netminer.cn";

        //定义一个全局变量，记录是否连接到远程服务器
        public static cGlobalParas.RegResult IsConnectRemote = cGlobalParas.RegResult.Faild;

        public static string SominerTaskVersionCode = "5.5";

        //定义一个静态的全局变量，用于加载动态帮助
        public static Help.cHelpInfo SominerHelp;

        //定义静态全局变量，用于识别当前语言区域
        public static CultureInfo cLanguage = null;

        //允许执行的分布式采集任务数量
        //public static int g_MaxRunDistriTaskCount=1;

        //定义一个全局的搜索Url
        public static string g_SerarchUrl = string.Empty;
        public static string g_SearchResult = string.Empty;

        public static string g_xulPath = Program.getPrjPath() + "resource\\xulrunner";

        /// <summary>
        /// 如果以用户名授权，则在此记录授权的结束时间
        /// </summary>
        public static DateTime g_EndServiceDate = System.DateTime.Now;

        public static bool g_isLogin = false;

        public static string g_AESKey = "Esijie--20101105";

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";
        }

        /// <summary>
        /// 获取当前系统标识的版本号，注意：是系统标识，不是激活的版本号
        /// </summary>
        /// <returns></returns>
        static public cGlobalParas.VersionType GetCurrentVersion()
        {
            string sVersion = Program.SominerRegVersionCode;
            sVersion = sVersion.Substring(sVersion.LastIndexOf(".") + 1, sVersion.Length - sVersion.LastIndexOf(".") - 1);

            if (sVersion.Length == 1)
                sVersion = "0" + sVersion;
            switch (sVersion)
            {

                case "01":
                    return cGlobalParas.VersionType.Program;
                case "02":
                    return cGlobalParas.VersionType.Free;
                case "03":
                    return cGlobalParas.VersionType.Cloud;
                case "04":
                    return cGlobalParas.VersionType.Ultimate;
                case "05":
                    return cGlobalParas.VersionType.Enterprise;
                default:
                    return cGlobalParas.VersionType.Free;
            }
        }
    }
}