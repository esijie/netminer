using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Resource;
using System.Xml;
using System.Xml.Linq;
using NetMiner.Base;
using System.Linq;
using NetMiner.Common;

///功能：处理系统所有的配置信息
///完成时间：2009-4-
///作者：一孑
///遗留问题：无
///开发计划：待定
///说明：与cSystem会有功能重复，下一步待定，所以保留
///版本：01.10.00
///修订：无
///V5.5修改为linq xml
namespace NetMiner.Core
{
    public class cXmlSConfig: XmlUnity
    {
        public cXmlSConfig(string workPath)
        {
            string cFile = workPath + "SoukeyConfig.xml";
            base.LoadXML(cFile);
        }

        ~cXmlSConfig()
        {
            base.Dispose();
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

        public cGlobalParas.CurLanguage CurrentLanguage
        {
            get
            {
                return (cGlobalParas.CurLanguage)int.Parse (base.GetValue("/Config/System/UILanguage"));
            }
            set
            {

                cGlobalParas.CurLanguage cl = value;

                base.EditValue ("/Config/System/UILanguage", ((int)cl).ToString ());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool IsFirstRun
        {
            get
            {
                if (base.GetValue("/Config/Start/First") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst==true )
                    base.EditValue("/Config/Start/First", "True");
                else
                    base.EditValue("/Config/Start/First", "False");

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool ExitIsShow
        {
            get 
            {
                if (base.GetValue("/Config/Exit/IsShow") == "0")
                    return false;
                else
                    return true;
            }
            set 
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";
            
                base.EditValue("/Config/Exit/IsShow", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        //退出选择：0-最小化 1-退出
        public int ExitSelected
        {
            get {return int.Parse (base.GetValue("/Config/Exit/Selected")); }
            set 
            {
                int i = value;
                base.EditValue("/Config/Exit/Selected", i.ToString ());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }
        
        //正则表达式的存储
        public string RegexNextPage
        {
            get 
            {
                string rNext=base.GetValue("/Config/Regex/NextPage");
                //rNext =cTool.

                if (rNext == "")
                    rNext = "((?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"]))[^<]*";

                return rNext;
            }
            set 
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/Regex/NextPage", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        //网络矿工运行模式
        public int SoMinerRunningModel
        {
            get { return int.Parse(base.GetValue("/Config/System/SominerRunningModel")); }
            set
            {
                int i = value;
                base.EditValue("/Config/System/SominerRunningModel", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool IsAutoStartRadar
        {
            get
            {
                if (base.GetValue("/Config/System/IsAutoStartRadar") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    base.EditValue("/Config/System/IsAutoStartRadar", "True");
                else
                    base.EditValue("/Config/System/IsAutoStartRadar", "False");

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }


        #region 数据库配置
        public int DataType
        {
            get { return int.Parse(base.GetValue("/Config/Database/DataType")); }
            set
            {
                int i = value;
                base.EditValue("/Config/Database/DataType", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string DataConnection
        {
            get
            {
                return base.GetValue("/Config/Database/DataConnction");
            }
            set
            {
                string s = value;
                base.EditValue("/Config/Database/DataConnction", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        #endregion

        #region 电子邮件配置
        public string Email
        {
            get
            {
                return base.GetValue("/Config/Email/Mail");
            }
            set
            {
                string s = value;
                base.EditValue("/Config/Email/Mail", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string EmailUser
        {
            get
            {
                return base.GetValue("/Config/Email/User");
            }
            set
            {
                string s = value;
                base.EditValue("/Config/Email/User", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string EmailPwd
        {
            get
            {
                return base.GetValue("/Config/Email/Password");
            }
            set
            {
                string s = value;
                base.EditValue("/Config/Email/Password", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool IsPopVerfy
        {
            get
            {
                if (base.GetValue("/Config/Email/IsPopVerfy") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    base.EditValue("/Config/Email/IsPopVerfy", "True");
                else
                    base.EditValue("/Config/Email/IsPopVerfy", "False");

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string EmailPopServer
        {
            get
            {
                return base.GetValue("/Config/Email/Pop");
            }
            set
            {
                string s = value;
                base.EditValue("/Config/Email/Pop", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public int EmailPopPort
        {
            get
            {
                string ss = base.GetValue("/Config/Email/Port");
                if (ss == "")
                    return 0;
                else
                    return int.Parse(ss);
            }
            set
            {
                int i = value;
                base.EditValue("/Config/Email/Port", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        #endregion

        public int MonitorInterval
        {
            get { return int.Parse(base.GetValue("/Config/System/MonitorInterval")); }
            set
            {
                int i = value;
                base.EditValue("/Config/System/MonitorInterval", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        //public void Save()
        //{
        //    base.Save();
        //}

        #region 日志管理

        //是否自动保存日志
        public bool AutoSaveLog
        {
            get
            {
                if (base.GetValue("/Config/System/AutoSaveLog") == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";

                base.EditValue("/Config/System/AutoSaveLog", s);

                if (m_IsInstantSave == true)

                    base.Save();
            }
        }

        public bool AutoSaveRadarLog
        {
            get
            {
                if (base.GetValue("/Config/System/AutoSaveRadarLog") == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";

                base.EditValue("/Config/System/AutoSaveRadarLog", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        //界面输出日志最大条数，以控制界面的输出日志不会太多
        //从而造成系统性能降低
        public int LogMaxNumber
        {
            get { return int.Parse(base.GetValue("/Config/System/LogMaxNumber")); }
            set
            {
                int i = value;
                base.EditValue("/Config/System/LogMaxNumber", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        #endregion

        #region 新建采集任务设置
        public int NewTaskType
        {
            get { return int.Parse(base.GetValue("/Config/Task/NewType")); }
            set
            {
                int i = value;
                base.EditValue("/Config/Task/NewType", i.ToString());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool NewTaskIsShow
        {
            get
            {
                if (base.GetValue("/Config/Task/IsShow") == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                string s = "0";
                if (value == true)
                    s = "1";
                else
                    s = "0";

                base.EditValue("/Config/Task/IsShow", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }
        #endregion

        //远程服务器地址
        public string RemoteServer
        {
            get
            {
                string rNext = base.GetValue("/Config/System/RemoteServer");

                return rNext;

                //return "http://spider.netminer.cn";
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/RemoteServer", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string RemoteServerUser
        {
            get
            {
                string rNext = base.GetValue("/Config/System/RemoteServerUser");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/RemoteServerUser", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string RemoteServerLicence
        {
            get
            {
                string rNext = base.GetValue("/Config/System/RemoteServerLicence");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/RemoteServerLicence", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public int MaxRemoteTaskCount
        {
            get
            {
                string rNext = base.GetValue("/Config/System/MaxRemoteTaskCount");

                return int.Parse(rNext);
                //return 1;

            }
            set
            {
                int s = value;
                base.EditValue("/Config/System/MaxRemoteTaskCount", s.ToString ());

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public bool IsAllowRemoteGather
        {
            get
            {
                //return true;

                if (base.GetValue("/Config/System/IsAllowRemoteGather") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    base.EditValue("/Config/System/IsAllowRemoteGather", "True");
                else
                    base.EditValue("/Config/System/IsAllowRemoteGather", "False");

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string SearchUrl
        {
            get
            {
                string rNext = base.GetValue("/Config/System/SearchUrl");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/SearchUrl", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string SearchResult
        {
            get
            {
                string rNext = base.GetValue("/Config/System/SearchResult");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/SearchResult", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        /// <summary>
        /// 代理IP自动更新标记
        /// </summary>
        public bool IsAutoUpdateProxy
        {
            get
            {
                if (base.GetValue("/Config/System/IsAutoUpdateProxy") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst==true )
                    base.EditValue("/Config/System/IsAutoUpdateProxy", "True");
                else
                    base.EditValue("/Config/System/IsAutoUpdateProxy", "False");

                if (m_IsInstantSave==true )
                    base.Save();
            }
        }

        public bool IsAuthen
        {
            get
            {
                //return false;

                if (base.GetValue("/Config/System/IsAuthen") == "True")
                    return true;
                else
                    return false;
            }
            set
            {
                bool isFirst = value;
                if (isFirst == true)
                    base.EditValue("/Config/System/IsAuthen", "True");
                else
                    base.EditValue("/Config/System/IsAuthen", "False");

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string windowsUser
        {
            get
            {
                string rNext = base.GetValue("/Config/System/User");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/User", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }

        public string windowsPwd
        {
            get
            {
                string rNext = base.GetValue("/Config/System/Pwd");

                return rNext;
            }
            set
            {
                string s = value;
                s = ToolUtil.ReplaceTrans(s);

                base.EditValue("/Config/System/Pwd", s);

                if (m_IsInstantSave == true)
                    base.Save();
            }
        }
    }
}
