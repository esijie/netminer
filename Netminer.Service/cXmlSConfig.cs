using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SoukeyService
{
    public class cXmlSConfig
    {
        cXmlIO xmlConfig;

        public cXmlSConfig()
        {
            //打开配置文件
            xmlConfig = new cXmlIO(Application.StartupPath +  "\\ServiceConfig.xml");
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

        public bool AutoSaveLog
        {
            get
            {
                if (xmlConfig.GetNodeValue("Config/System/AutoSaveLog") == "0")
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

                xmlConfig.EditNodeValue("Config/System/AutoSaveLog", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

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

        #endregion
 
        public void Save()
        {
            xmlConfig.Save();
        }

    }
}
