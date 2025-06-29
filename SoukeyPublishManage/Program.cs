using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetMiner.Resource;
using System.Reflection;
using System.Resources;
using System.Globalization;
using NetMiner.Common;

namespace SoukeyPublishManage
{
    static class Program
    {
        private static ApplicationContext context;
        private static string[] m_args;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmStart sf = new frmStart();
            sf.Show();
            context = new ApplicationContext();
            context.Tag = sf;
            Application.Idle += new EventHandler(Application_Idle); //注册程序运行空闲去执行主程序窗体相应初始化代码
            Application.Run(context);
        }

        private static void Application_Idle(object sender, EventArgs e)
        {

            Application.Idle -= new EventHandler(Application_Idle);
            if (context.MainForm == null)
            {
                frmMain mf;

                mf = new frmMain();
                
                context.MainForm = mf;
                frmStart sf = (frmStart)context.Tag;
               
                Application.DoEvents();

                mf.IniData(); 

                sf.Close();                                 //关闭启动窗体
                sf.Dispose();

                mf.Show();                                  //启动主程序窗体

            }
        }

        //全局变量，标识此系统的版本
        public static cGlobalParas.VersionType SominerVersion;

        //全局软件发布版本号，用于系统验证使用
        public static string SominerVersionCode = "5.0.1.02";

        public static DateTime Deadline;

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";

        }
    }
}
