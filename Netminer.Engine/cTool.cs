using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace NetMiner.Engine
{
    public static class cTool
    {
        public static string getPrjPath()
        {
            return Application.StartupPath + "\\";

        }

        //写日志到windows
        public static void WriteSystemLog(string mess, EventLogEntryType eType,string ServerName)
        {
            EventLog eLog = new EventLog();

            if (!EventLog.SourceExists(ServerName))
                EventLog.CreateEventSource(ServerName, "RunningLog");

            eLog.Source = ServerName;
            eLog.WriteEntry(mess, eType);

            //发送邮件
            BySendEmail(mess);

        }

        public static void BySendEmail(string errMessage)
        {

            cXmlSConfig cs = new cXmlSConfig();

            bool isemail = cs.IsEmail;

            if (isemail == false)
            {
                cs = null;
                return;
            }
            NetMiner.Common.Tools.cEmail e = new NetMiner.Common.Tools.cEmail();
            e.Title = "网络矿工采集引擎发生了致命错误——来自网路矿工" + "";
            e.ReceiveEmail = cs.ReceiveEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;

            string emailContent = "出错时间" + System.DateTime.Now + "\r\n";
            emailContent += "网络矿工采集引擎IP：" + NetMiner.Common.ToolUtil.GetIP() + "\r\n";
            emailContent += "错误信息：" + errMessage + "\r\n";
            emailContent += "请立即处理此错误，否则采集引擎将无法继续工作！" + "\r\n";
            emailContent += "此邮件为自动发送，来自于网络矿工数据采集软件，请勿直接回复此邮件！" + "\r\n";

            e.Content = emailContent;

            try
            {
                e.SendMail();
            }
            catch (System.Exception ex)
            {
                WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherService");
            }

            cs = null;
            e = null;

        }
    }
}
