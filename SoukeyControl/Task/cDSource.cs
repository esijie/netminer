using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Gather;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Base;

namespace SoukeyControl.Task
{
    class cDSource
    {

        private string m_xmlFile =string.Empty ; // = Program.getPrjPath() + "Radar\\data.xml";
        cXmlIO xmlConfig;
        DataView m_DataInfos;
        private string m_workPath = string.Empty;

        public cDSource(string workPath)
        {
            //���ļ�
            m_workPath=workPath ;
            m_xmlFile = workPath + "Radar\\data.xml";

            if (IsExist() == false)
            {
                string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                              "<DataSources>" +
                              "</DataSources>";

                xmlConfig = new cXmlIO();
                xmlConfig.NewXmlFile(this.m_xmlFile, strXml);

            }
            else
            {
                xmlConfig = new cXmlIO(this.m_xmlFile);
            }
         
            
        }

        ~cDSource()
        {
            xmlConfig = null;
        }

        #region


        public cGlobalParas.DatabaseType GetDataType(int index)
        {
            cGlobalParas.DatabaseType dType = (cGlobalParas.DatabaseType)int.Parse(m_DataInfos[index].Row["DataType"].ToString());
            return dType;
        }

        public string GetDataConnction(int index)
        {
            string TName = m_DataInfos[index].Row["DataConnction"].ToString();
            return TName;
        }

        #endregion


        public void LoadData()
        {
            m_DataInfos = xmlConfig.GetData("DataSources");
        }

        public int GetCount()
        {
            int tCount;
            if (m_DataInfos == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = m_DataInfos.Count;
            }
            return tCount;
        }

        public void InsertDataSource(cGlobalParas.DatabaseType DataType, string Conn)
        {
            string strXml = "";
            strXml += "<DataType>" + (int)DataType + "</DataType>";
            strXml += "<DataConnction>" + Conn + "</DataConnction>";


            xmlConfig.InsertElement("DataSources", "DataSource", strXml);
            xmlConfig.Save();
        }

        public void DelDataSource(cGlobalParas.DatabaseType DataType, string Conn)
        {
            xmlConfig.DeleteChildNodes("DataSources", "DataConnction", Conn);
            xmlConfig.Save();
        }

        
        //�ж������ļ��Ƿ���ڣ��������������н���
        public bool IsExist()
        {

            if (System.IO.File.Exists(this.m_xmlFile))
                return true;
            else
            {
                return false;
            }

            
        }

    }
}
