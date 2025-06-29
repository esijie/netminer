using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using SoukeyCommon;
using SoukeyCommon.Tool;

///功能：代理信息的维护
///完成时间：2010-12-1
///作者：一孑
///遗留问题：无
///说明：
///版本：02.10.0.03
///修订：无
namespace SoukeyNetget
{
    class cProxy
    {
        cXmlIO xmlConfig;
        DataView m_Proxys;
        private string m_pFile = Program.getPrjPath() + "tasks\\proxy.db";

        #region 构造 析构 函数
        public cProxy()
        {
            try
            {
                
                //判断此文件是否存在
                if (!System.IO.File.Exists(m_pFile))
                {
                    NewProxyFile();
                }
                string strXML = cFile.ReadFileBinary(m_pFile);
                xmlConfig = new cXmlIO();
                xmlConfig.LoadXML(strXML);
                
                //获取Proxys节点
                m_Proxys = xmlConfig.GetData("Proxys");

                //获取TaskClass节点
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        ~cProxy()
        {
            xmlConfig = null;
        }

        #endregion

        #region 新建一个Proxy文件,
        public void NewProxyFile()
        {

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<Proxys>" +
                       "</Proxys>";

            cFile.SaveFileBinary(m_pFile, strXml, true);
            //xmlConfig.NewXmlFile(m_pFile, strXml);

        }

        #endregion

        public void InsertProxy(string strXml)
        {
            xmlConfig.InsertElement("Proxys", "Proxy", strXml);
        }

        public void Dele()
        {
            xmlConfig.DeleteNodeChile("Proxys");
        }

      

        public int GetCount()
        {
            int tCount;
            if (m_Proxys == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = m_Proxys.Count;
            }
            return tCount;
        }

        public void Save()
        {
            cFile.SaveFileBinary(m_pFile, xmlConfig.InnerXml, true);
        }

        public string GetProxyServer(int index)
        {
            string ProxyServer = m_Proxys[index].Row["Server"].ToString();
            return ProxyServer;
        }

        public string GetProxyPort(int index)
        {
            string ProxyPort = m_Proxys[index].Row["Port"].ToString();
            return ProxyPort;
        }

        public string GetUser(int index)
        {
            string User = m_Proxys[index].Row["User"].ToString();
            return User;
        }

        public string GetPwd(int index)
        {
            string Pwd = m_Proxys[index].Row["Password"].ToString();
            return Pwd;
        }
    }
}
