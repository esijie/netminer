using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Common.Tool;
using NetMiner.Core;

namespace SoukeyDataPublish
{
    static class Program
    {
        private static ApplicationContext context;
        private static string[] m_args;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            //检测是否指定了界面语言
            CultureInfo cLanguage = null;

            try
            {
                cXmlSConfig Config = new cXmlSConfig(Program.getPrjPath ());
                cGlobalParas.CurLanguage cl = Config.CurrentLanguage;
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
                Config = null;
            }
            catch (System.Exception)
            {

            }

            if (cLanguage != null)
            {
                Thread.CurrentThread.CurrentUICulture = cLanguage;
                Thread.CurrentThread.CurrentCulture = cLanguage;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            m_args = args;

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
                cVersion sV = new cVersion(Program.getPrjPath());
                sV.ReadRegisterInfo();

                SominerVersion = (cGlobalParas.VersionType)sV.SominerVersion;
                Program.RegisterUser = sV.RegisterInfo.User;

                //if (SominerVersion == cGlobalParas.VersionType.Personal)
                //{
                //    //标识已经在线激活，需要判断主版本号是否一致，如果不一致，则需要
                //    //告知用户无法升级使用

                //    //获取当前版本号
                //    string CurVersionCode = cTool.Mearger(SominerVersionCode, sV.SominerVersionCode);
                //    string RegVersionCode = sV.SominerVersionCode;

                //    SominerVersionCode = cTool.Mearger(SominerVersionCode, sV.SominerVersionCode);
                //    SominerRegVersionCode = sV.SominerVersionCode;

                //    //如果主版本不对，或者版本的类别不对，则都禁止使用
                //    if (CurVersionCode != RegVersionCode)
                //    {
                //        MessageBox.Show(rm.GetString("Error38"), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //        Environment.Exit(0);
                //    }


                //}
                if (SominerVersion == cGlobalParas.VersionType.Cloud)
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
                else if (SominerVersion == cGlobalParas.VersionType.Cloud)
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
                    //Deadline = sV.GetFirstRunTime(Program.SominerVersionCode);
                    //Deadline = Deadline.AddMonths(1);

                    //string CurVersionCode = SominerVersionCode;
                    //string RegVersionCode = sV.SominerVersionCode;
                    SominerVersionCode = sV.SominerVersionCode;
                    SominerRegVersionCode = sV.SominerVersionCode;
                }

                sV = null;
              

                frmMain mf;

                if (m_args.Length == 0)
                {
                    mf=new frmMain();
                }
                else
                {
                    cGlobalParas.DatabaseType dType = (cGlobalParas.DatabaseType)(int.Parse (m_args[0].ToString()));
                    string dbCon = m_args[1].ToString();
                    string sql = m_args[2].ToString();

                    if (System.IO.File.Exists(dbCon))
                    {
                        mf = new frmMain(dType,dbCon ,sql);
                    }
                    else
                    {
                        mf=new frmMain();
                    }
                }
                context.MainForm = mf;
                frmStart sf = (frmStart)context.Tag;

                //初始化对象并开始启动运行区的任务
                sf.label3.Text = rm.GetString("Info69");
                Application.DoEvents();
                //mf.IniForm();

                sf.Close();                                 //关闭启动窗体
                sf.Dispose();

                rm = null;

                mf.Show();                                  //启动主程序窗体

            }
        }

        //全局变量，标识此系统的版本
        public static cGlobalParas.VersionType SominerVersion;
        public static string SominerRegVersionCode = "";
        //定义一个全局变量，用于记录当前注册用户，如果为试用版，则为空
        public static string RegisterUser = string.Empty;

        //全局软件发布版本号，用于系统验证使用
        public static string SominerVersionCode = "5.4.1.02";


        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";

        }

    }

    
}