using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataGatherControl.xml
{
    public class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            //打开配置文件
            xmlConfig = new cXmlIO(cTool.getPrjPath() + "config.xml");
            m_IsInstantSave = true;
        }

        ~cXmlSConfig()
        {
            xmlConfig = null;
        }

        //是否即时保存，默认为true，即时保存，
        private bool m_IsInstantSave;
        public bool IsInstantSave
        {
            get { return m_IsInstantSave; }
            set { m_IsInstantSave = value; }
        }

        /// <summary>
        /// 线程最大数量
        /// </summary>
        public int ThreadCount
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/SystemConfig/ThreadCount");
                return int.Parse(i);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/ThreadCount", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #region 数据库配置
        public string DataServer
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/DataServer");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/DataServer", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DatabaseName
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/DatabaseName");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/DatabaseName", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DataUser
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/User");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/User", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DataPassword
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/Password");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/Password", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #endregion

        public void Save()
        {
            xmlConfig.Save();
        }

    }
}
