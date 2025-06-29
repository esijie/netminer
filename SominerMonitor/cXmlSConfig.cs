using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SominerMonitor
{
    public class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            //打开配置文件
            xmlConfig = new cXmlIO(Application.StartupPath + "\\monitorconfig.xml");
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

        public bool AutoConnect
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/System/IsAutoConnect") == "0")
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

                xmlConfig.EditNodeValue("Config/System/IsAutoConnect", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

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

        public string GatherAddress
        {
            get
            {
                return xmlConfig.GetNodeValue("Config/System/GatherAddress");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("Config/System/GatherAddress", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int GatherPort
        {
            get
            {
                string s = xmlConfig.GetNodeValue("Config/System/GatherPort");
                if (s == "")
                    return 1105;
                else
                    return int.Parse(s);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("Config/System/GatherPort", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }
 
        public void Save()
        {
            xmlConfig.Save();
        }

    }
}
