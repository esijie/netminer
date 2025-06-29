using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using NetMiner.Common;
using NetMiner.Resource;

namespace NetMiner.Core.Plugin
{
    public class cPlugin
    {
        cXmlIO xmlConfig;
        DataView m_Proxys;
        private string m_workPath = string.Empty;

        public cPlugin(string workPath)
        {
            m_workPath = workPath;
            try
            {
                //判断此文件是否存在
                if (!System.IO.File.Exists(m_workPath + "plugins\\plugin.xml"))
                {
                    NewPluginFile();
                }

                xmlConfig = new cXmlIO(m_workPath + "plugins\\plugin.xml");

                //获取Proxys节点
                m_Proxys = xmlConfig.GetData("Plugins");

                //获取TaskClass节点
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        ~cPlugin()
        {
            xmlConfig = null;
        }

        #region 新建一个Plugin文件
        public void NewPluginFile()
        {

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<Plugins>" +
                       "</Plugins>";
            xmlConfig.NewXmlFile(m_workPath + "\\plugins\\plugin.xml", strXml);

        }

        #endregion

        public void InsertPlugin(string strXml)
        {
            xmlConfig.InsertElement("Plugins", "Plugin", strXml);
            xmlConfig.Save();
        }

        public void Dele()
        {
            xmlConfig.DeleteNodeChile("Proxys");
        }

        public void DelPlugin(string pName)
        {
            xmlConfig.DeleteChildNodes("Plugins", "Name", pName);
            xmlConfig.Save();
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
            xmlConfig.Save();
        }

        public string GetPluginName(int index)
        {
            string ProxyServer = m_Proxys[index].Row["Name"].ToString();
            return ProxyServer;
        }

        public string GetPlugin(int index)
        {
            string ProxyPort = m_Proxys[index].Row["File"].ToString();
            return ProxyPort;
        }

        public cGlobalParas.PluginsType GetPluginType(int index)
        {
            string pType = m_Proxys[index].Row["Type"].ToString();
            return (cGlobalParas.PluginsType)int.Parse (pType);
        }
    }


}
