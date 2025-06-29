using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;
using NetMiner.Resource;

namespace SoukeyLog
{
    static class Program
    {
        private static ApplicationContext context;


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
                ResourceManager rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

                //检测系统版本
                //cVersion sV = new cVersion();
                //sV.ReadRegisterInfo();

                //SominerVersion = (cGlobalParas.VersionType)sV.SominerVersion;
                //if (SominerVersion == cGlobalParas.VersionType.Professional)
                //{
                //    //标识已经在线激活，需要判断主版本号是否一致，如果不一致，则需要
                //    //告知用户无法升级使用

                //    //获取当前版本号
                //    string CurVersionCode = NetMiner.Common.cTool.Mearger(SominerVersionCode, sV.SominerVersionCode);
                //    string RegVersionCode = sV.SominerVersionCode;

                //    if (CurVersionCode != RegVersionCode)
                //    {
                //        MessageBox.Show(rm.GetString("Error28"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        Environment.Exit(0);
                //    }

                //}
                //else
                //{
                //    //表示为试用版本
                //    //Deadline = sV.GetFirstRunTime(Program.SominerVersionCode);
                //    //Deadline = Deadline.AddMonths(1);
                //}
                //sV = null;

                frmMain mf;

                mf = new frmMain();
                
                context.MainForm = mf;
                frmStart sf = (frmStart)context.Tag;

                //初始化对象并开始启动运行区的任务
                //sf.label3.Text = rm.GetString("Info69");
                Application.DoEvents();
                mf.IniForm();

                sf.Close();                                 //关闭启动窗体
                sf.Dispose();

                rm = null;

                mf.Show();                                  //启动主程序窗体

            }
        }

        //全局变量，标识此系统的版本
        public static cGlobalParas.VersionType SominerVersion;

        //全局软件发布版本号，用于系统验证使用
        public static string SominerVersionCode = "5.5.1.02";

        public static DateTime Deadline;

        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";

        }
    }
}