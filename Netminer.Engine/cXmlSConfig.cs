using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NetMiner.Engine
{
    public class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            
            //打开配置文件
            xmlConfig = new cXmlIO(Application.StartupPath + "\\ServiceConfig.xml");
            //xmlConfig = new cXmlIO("ServiceConfig.xml");
            m_IsInstantSave = true;
        }

        ~cXmlSConfig()
        {
            xmlConfig = null;
        }

        //是否即时保存，默认为true，即时保存，即调用方法后马上保存
        //false时，只修改，不保存文件，需要调用Save方法进行保存，主要用于
        //配置修改
        private bool m_IsInstantSave;
        public bool IsInstantSave
        {
            get { return m_IsInstantSave; }
            set { m_IsInstantSave = value; }
        }

        //public string ServerPath
        //{
        //    get
        //    {
        //        return xmlConfig.GetNodeValue("Config/System/ServerPath");
        //    }
        //    set
        //    {
        //        string s = value;
        //        xmlConfig.EditNodeValue("Config/System/ServerPath", s);

        //        if (m_IsInstantSave == true)
        //            xmlConfig.Save();
        //    }
        //}

        #region 数据库配置
      
        public string DataConnection
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Database/DataConnction");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Database/DataConnction", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DataType
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Database/DataType");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Database/DataType", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }
        #endregion

        #region wcf监控端口
        public string BindAddress
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/System/BindAddress");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/System/BindAddress", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int BindPort
        {
            get
            {
                string s = xmlConfig.GetNodeValue("Config/System/BindPort");
                if (s == "")
                    return 1105;
                else
                    return int.Parse(s);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/BindPort", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #endregion
        

        #region 服务器引擎控制策略
        public int MaxDownloadSpeed
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/MaxDownloadSpeed"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/MaxDownloadSpeed", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int MaxGatherCount
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/MaxGatherCount"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/MaxGatherCount", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int MaxRunTask
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/MaxRunTask"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/MaxRunTask", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int SplitUrls
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/SplitUrls"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/SplitUrls", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int SplitLevel
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/SplitLevel"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/SplitLevel", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int SplitTasks
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/SplitTasks"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/SplitTasks", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string ReceiveEmail
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/System/ReceiveEmail");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/System/ReceiveEmail", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int MaxSplitUrlThread
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/MaxSplitUrlThread"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/MaxSplitUrlThread", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int MaxPublishDataThread
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/System/MaxPublishDataThread"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/MaxPublishDataThread", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool IsSameSubTask
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/System/IsSameSubTask") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/System/IsSameSubTask", "True");
                else
                    xmlConfig.EditNodeValue("Config/System/IsSameSubTask", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #region 电子邮件配置，设置电子邮件的目的是如果采集引擎发生了重大错误，使用电子邮件进行消息通知
        public bool IsEmail
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Email/IsEmail") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/Email/IsEmail", "True");
                else
                    xmlConfig.EditNodeValue("Config/Email/IsEmail", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string Email
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Email/Mail");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Email/Mail", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string EmailUser
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Email/User");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Email/User", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string EmailPwd
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Email/Password");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Email/Password", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool IsPopVerfy
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Email/IsPopVerfy") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/Email/IsPopVerfy", "True");
                else
                    xmlConfig.EditNodeValue("Config/Email/IsPopVerfy", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string EmailPopServer
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Email/Pop");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Email/Pop", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int EmailPopPort
        {
            get
            {
                string ss = xmlConfig.GetNodeValue("Config/Email/Port");
                if (ss == "")
                    return 0;
                else
                    return int.Parse(ss);
            }
            set
            {
                int i = value;
                xmlConfig.EditNodeValue("Config/Email/Port", i.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #endregion

        #region 工作日报
        public bool IsReport
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Report/IsReport") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/Report/IsReport", "True");
                else
                    xmlConfig.EditNodeValue("Config/Report/IsReport", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string ReportEmail
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Report/ReceiveEmail");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Report/ReceiveEmail", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string ReportSendTime
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Report/SendTime");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Report/SendTime", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }
        #endregion

        #region 分布式采集配置
        public bool IsRemote
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Remote/IsRemote") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/Remote/IsRemote", "True");
                else
                    xmlConfig.EditNodeValue("Config/Remote/IsRemote", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string RemoteServer
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Remote/RemoteServer");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Remote/RemoteServer", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool IsAuthen
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/Remote/IsAuthen") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    xmlConfig.EditNodeValue("Config/Remote/IsAuthen", "True");
                else
                    xmlConfig.EditNodeValue("Config/Remote/IsAuthen", "False");

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string RemoteUser
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Remote/User");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Remote/User", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string RemotePwd
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Remote/Pwd");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Remote/Pwd", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int RemoteMaxRunTask
        {
            get
            {
                return int.Parse(xmlConfig.GetNodeValue("Config/Remote/MaxTaskCount"));
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/Remote", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string RemotePath
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/Remote/RemotePath");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/Remote/RemotePath", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public bool isAutoStartTask
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/System/IsAutoStartTask").ToLower() == "true")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value==true)
                    xmlConfig.EditNodeValue("Config/System/IsAutoStartTask", "True");
                else
                    xmlConfig.EditNodeValue("Config/System/IsAutoStartTask", "False");
            }
        }

        public bool isGlobalRepeat
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/System/IsGlobalRepeat").ToLower() == "true")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value == true)
                    xmlConfig.EditNodeValue("Config/System/IsGlobalRepeat", "True");
                else
                    xmlConfig.EditNodeValue("Config/System/IsGlobalRepeat", "False");
            }
        }
        #endregion

        #endregion

        public void Save()
        {
            xmlConfig.Save();
        }

    }
}
