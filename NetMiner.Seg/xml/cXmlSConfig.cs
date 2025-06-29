using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

///���ܣ�����ϵͳ���е�������Ϣ
///���ʱ�䣺2009-4-
///���ߣ�һ��
///�������⣺��
///�����ƻ�������
///˵������cSystem���й����ظ�����һ�����������Ա���
///�汾��01.10.00
///�޶�����
namespace NetMiner.Seg.xml
{
    class cXmlSConfig
    {

        cXmlIO xmlConfig;

        public cXmlSConfig(string xmlFile)
        {
            //�������ļ�
            xmlConfig = new cXmlIO(xmlFile);
            m_IsInstantSave = true;
        }

        ~cXmlSConfig()
        {
            xmlConfig = null;
        }

        //�Ƿ�ʱ���棬Ĭ��Ϊtrue����ʱ���棬�����÷��������ϱ���
        //falseʱ��ֻ�޸ģ��������ļ�����Ҫ����Save�������б��棬��Ҫ����
        //�����޸�
        private bool m_IsInstantSave;
        public bool IsInstantSave
        {
            get { return m_IsInstantSave; }
            set { m_IsInstantSave = value; }
        }

        #region ϵͳ����
        public int GatherThread
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/SystemConfig/GatherThread");
                return int.Parse(i);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/GatherThread", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int AnalyThread
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/SystemConfig/AnalyThread");
                return int.Parse(i);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/AnalyThread", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }

        }

        public int TrackingThread
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/SystemConfig/TrackingThread");
                return int.Parse(i);
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/TrackingThread", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }

        }

        public string WorkDirectory
        {
            get
            {
                return xmlConfig.GetNodeValue("System/SystemConfig/WorkDirectory");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/WorkDirectory", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string LogPath
        {
            get
            {
                return xmlConfig.GetNodeValue("System/SystemConfig/LogPath");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/LogPath", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string PagePath
        {
            get
            {
                return xmlConfig.GetNodeValue("System/SystemConfig/PageSavePath");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/SystemConfig/PageSavePath", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DictPath
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Segment/DictPath");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Segment/DictPath", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string CorpusDirectory
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Segment/CorpusDirectory");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Segment/CorpusDirectory", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        #endregion

        #region ���ݿ�����
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

        public string DataPort
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/DataPort");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/DataPort", s);

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public string DataCharacter
        {
            get
            {
                return xmlConfig.GetNodeValue("System/Database/Character");
            }
            set
            {
                string s = value;
                xmlConfig.EditNodeValue("System/Database/Character", s);

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

        #region �����ʼ�����
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

        #region �����������
        public int MonitorInterval
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/Tracking/MonitorInterval");
                return int.Parse(i.ToString());
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/Tracking/MonitorInterval", s.ToString());

                if (m_IsInstantSave == true)
                    xmlConfig.Save();
            }
        }

        public int TrackingInterval
        {
            get
            {
                string i = xmlConfig.GetNodeValue("System/Tracking/TrackingInterval");
                return int.Parse(i.ToString());
            }
            set
            {
                int s = value;
                xmlConfig.EditNodeValue("System/Tracking/TrackingInterval", s.ToString());

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
