using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace SoukeyDataPublish.Task
{
    class cPublishTask
    {

        cXmlIO xmlConfig;
        DataView m_PublishInfos;

        public cPublishTask()
        {
        }

        ~cPublishTask()
        {
            if (xmlConfig != null)
                xmlConfig = null;
        }

        #region ����
        private string m_pName;
        public string pName
        {
            get { return m_pName; }
            set { m_pName = value; }
        }

        private int m_ThreadCount;
        public int ThreadCount
        {
            get { return m_ThreadCount; }
            set { m_ThreadCount = value; }
        }

        private bool m_IsDelRepeatRow;
        public bool IsDelRepeatRow
        {
            get { return  m_IsDelRepeatRow; }
            set { m_IsDelRepeatRow = value; }
        }

        private cGlobalParas.PublishType m_PublishType;
        public cGlobalParas.PublishType PublishType
        {
            get { return m_PublishType; }
            set { m_PublishType = value; }
        }

        private cGlobalParas.DatabaseType m_DataType;
        public cGlobalParas.DatabaseType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        private string m_DataSource;
        public string DataSource
        {
            get { return m_DataSource; }
            set { m_DataSource = value; }
        }

        private string m_DataTable;
        public string DataTable
        {
            get { return m_DataTable; }
            set { m_DataTable = value; }
        }

        private string m_InsertSql;
        public string InsertSql
        {
            get { return m_InsertSql; }
            set { m_InsertSql = value; }
        }

        private cGlobalParas.WebCode m_UrlCode;
        public cGlobalParas.WebCode UrlCode
        {
            get { return m_UrlCode; }
            set { m_UrlCode = value; }
        }

        private string m_PostUrl;
        public string PostUrl
        {
            get { return m_PostUrl; }
            set { m_PostUrl = value; }
        }

        private string m_Cookie;
        public string Cookie
        {
            get { return m_Cookie; }
            set { m_Cookie = value; }
        }

        private string m_SucceedFlag;
        public string SucceedFlag
        {
            get { return m_SucceedFlag; }
            set { m_SucceedFlag = value; }
        }

        private bool m_IsHeader;
        public bool IsHeader
        {
            get { return m_IsHeader; }
            set { m_IsHeader = value; }
        }

        private string m_Header;
        public string Header
        {
            get { return m_Header; }
            set { m_Header = value; }
        }

        #endregion

        //�½�һ������Դ�洢�ļ�
        public void NewPublishFile()
        {
            xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<PublishInfos>" +
                      "</PublishInfos>";
            xmlConfig.NewXmlFile(Program.getPrjPath() + "publish\\PublishInfo.xml", strXml);
        }

        //���ص�������Դ�ļ�����Ϣ
        public void LoadPublishData()
        {
            if (!System.IO.File.Exists(Program.getPrjPath() + "publish\\PublishInfo.xml"))
                NewPublishFile();

            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�
            m_PublishInfos = xmlConfig.GetData("PublishInfos");
        }

        public void LoadSinglePublish(string pName)
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            m_PublishInfos = xmlConfig.GetData("PublishInfos", "Name", pName);
        }

        //���ü���������Ϣ��,���ô˷�������һ�������dataview
        public DataView GetPublishInfos()
        {
            return m_PublishInfos;
        }

        //���㵱ǰ���ж��ٸ������Ѿ����
        public int GetCount()
        {
            int tCount;

            if (m_PublishInfos == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = m_PublishInfos.Count;
            }
            return tCount;
        }

        public string GetName(int index)
        {
            string dType = m_PublishInfos[index].Row["Name"].ToString();
            return dType;
        }

        public cGlobalParas.PublishType GetPublishType(int index)
        {
            int dSource =int.Parse ( m_PublishInfos[index].Row["PublishType"].ToString());
            return (cGlobalParas.PublishType)dSource ;
        }

        public int GetThreadCount(int index)
        {
            int tCount = int.Parse(m_PublishInfos[index].Row["ThreadCount"].ToString());
            return tCount;
        }

        public bool GetIsDelRepeat(int index)
        {
            string  IsDelRepeat = m_PublishInfos[index].Row["IsDelRepeatRow"].ToString();
            if (IsDelRepeat == "True")
                return true;
            else
                return false;
        }

        public cGlobalParas.DatabaseType GetDataType(int index)
        {
            int dSource = int.Parse ( m_PublishInfos[index].Row["DataType"].ToString());
            return (cGlobalParas.DatabaseType )dSource; 
        }

        public string GetDataSource(int index)
        {
            string dSource = m_PublishInfos[index].Row["DataSource"].ToString();
            return dSource;
        }

        public string GetTableName(int index)
        {
            string dSource = m_PublishInfos[index].Row["DataTable"].ToString();
            return dSource;
        }

        public string GetInsertSql(int index)
        {
            string dSql = m_PublishInfos[index].Row["InsertSql"].ToString();
            return dSql;
        }

        public int GetUrlCode(int index)
        {
            string uCode = m_PublishInfos[index].Row["UrlCode"].ToString();
            return int.Parse (uCode);
        }

        public string GetPostUrl(int index)
        {
            string dSql = m_PublishInfos[index].Row["PostUrl"].ToString();
            return dSql;
        }

        public string GetCookie(int index)
        {
            string dSql = m_PublishInfos[index].Row["Cookie"].ToString();
            return dSql;
        }

        public string GetSucceedFlag(int index)
        {
            string dSql = m_PublishInfos[index].Row["SucceedFlag"].ToString();
            return dSql;
        }

        public bool GetIsHeader(int index)
        {
            string IsHeader = m_PublishInfos[index].Row["IsHeader"].ToString();
            if (IsHeader == "True")
                return true;
            else
                return false;
        }

        public string GetHeader(int index)
        {
            string dSql = m_PublishInfos[index].Row["Header"].ToString();
            return dSql;
        }
     
        //����һ���µ�����Դ��������
        public void InsertPublishInfo()
        {
            ///�����жϴ������ִ�е�Ŀ¼�Ƿ����
            ///��Ŀ¼�ǹ̶�Ŀ¼�������ϵͳ\\publish
            string cPath = Program.getPrjPath() + "publish\\";

            if (!System.IO.Directory.Exists(cPath))
            {
                System.IO.Directory.CreateDirectory(cPath);
            }

            ///�Ƚ��������ժҪ��Ϣ���ص�index.xml�ļ���

            string txml = "";
            txml += "<Name>" + this.pName + "</Name>";
            txml += "<ThreadCount>" + this.ThreadCount + "</ThreadCount>";
            txml += "<IsDelRepeatRow>" + this.IsDelRepeatRow  + "</IsDelRepeatRow>";
            txml += "<PublishType>" + (int)this.PublishType + "</PublishType>";
            txml += "<DataType>" + (int)this.DataType + "</DataType>";
            txml += "<DataSource>" + this.DataSource + "</DataSource>";
            txml += "<DataTable>" + this.DataTable + "</DataTable>";
            txml += "<InsertSql>" + this.InsertSql + "</InsertSql>";
            txml += "<UrlCode>" + (int)this.UrlCode + "</UrlCode>";
            txml += "<PostUrl><![CDATA[" + this.PostUrl + "]]></PostUrl>";
            txml += "<Cookie><![CDATA[" + this.Cookie + "]]></Cookie>";
            txml += "<IsHeader>" +  this.IsHeader + "</IsHeader>";
            txml += "<Header><![CDATA[" + this.Header + "]]></Header>";
            txml += "<SucceedFlag><![CDATA[" + this.SucceedFlag + "]]></SucceedFlag>";

            xmlConfig = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            xmlConfig.InsertElement("PublishInfos", "Publish", txml);
            xmlConfig.Save();

        }

        public bool IsExist(string pName)
        {
            bool IsE = false;
            cXmlIO xml;

            try
            {
                xml = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�
            DataView dP = xml.GetData("PublishInfos");
            for (int i = 0; i < dP.Count; i++)
            {
                if (pName ==dP[i].Row["Name"].ToString())
                {
                    IsE=true ;
                    break ;
                }
            }

            xml = null;
            return IsE;
        }

        public void DelTask(string pName)
        {
            xmlConfig = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            xmlConfig.DeleteChildNodes("PublishInfos", "Name", pName);
            xmlConfig.Save();
            xmlConfig = null;
        }

        /// <summary>
        /// ���һ���������������xml�ļ�
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public string GetPXML(string pName)
        {
            try
            {
                xmlConfig = new cXmlIO(Program.getPrjPath() + "publish\\PublishInfo.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            m_PublishInfos = xmlConfig.GetData("PublishInfos", "Name", pName);

            if (m_PublishInfos == null || m_PublishInfos.Count == 0)
                return "";

            string txml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + "\r\n";
            txml+="<PublishInfos>" + "\r\n";
            txml += "<Name>" + pName + "</Name>" + "\r\n";
            txml += "<ThreadCount>" + this.GetThreadCount (0) + "</ThreadCount>" + "\r\n";
            txml += "<IsDelRepeatRow>" + this.GetIsDelRepeat(0) + "</IsDelRepeatRow>" + "\r\n";
            txml += "<PublishType>" + (int)this.GetPublishType(0) + "</PublishType>" + "\r\n";
            txml += "<DataType>" + (int)this.GetDataType(0) + "</DataType>" + "\r\n";
            txml += "<DataSource>" + this.GetDataSource(0) + "</DataSource>" + "\r\n";
            txml += "<DataTable>" + this.GetTableName(0) + "</DataTable>" + "\r\n";
            txml += "<InsertSql>" + this.GetInsertSql(0) + "</InsertSql>" + "\r\n";
            txml += "<UrlCode>" + this.GetUrlCode(0) + "</UrlCode>" + "\r\n";
            txml += "<PostUrl><![CDATA[" + this.GetPostUrl(0) + "]]></PostUrl>" + "\r\n";
            txml += "<Cookie><![CDATA[" + this.GetCookie(0) + "]]></Cookie>" + "\r\n";
            txml += "<IsHeader>" + this.GetIsHeader(0) + "</IsHeader>" + "\r\n";
            txml += "<Header><![CDATA[" + this.GetHeader(0) + "]]></Header>" + "\r\n";
            txml += "<SucceedFlag><![CDATA[" + this.GetSucceedFlag(0) + "]]></SucceedFlag>" + "\r\n";
            txml += "</PublishInfos>";

            return txml;
        }
    
    }


}
