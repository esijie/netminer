using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///���ܣ��ɼ����� �ɼ���־ �洢����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eWebpageCutFlag
    {
        #region ���������
        public eWebpageCutFlag()
        {
            m_ExportRules = new List<eFieldRule>();
        }

        ~eWebpageCutFlag()
        {
        }
        #endregion

        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_StartPos;
        public string StartPos
        {
            get { return m_StartPos; }
            set { m_StartPos = value; }
        }

        private string m_EndPos;
        public string EndPos
        {
            get { return m_EndPos; }
            set { m_EndPos = value; }
        }

        private bool m_loopFlag;
        public bool loopFlag
        {
            get { return m_loopFlag; }
            set { m_loopFlag = value; }
        }

        private string m_Content;
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        //��ҳ�ɼ����ݵ��޶���ʶ
        private cGlobalParas.LimitSign m_LimitSign;
        public cGlobalParas.LimitSign LimitSign
        {
            get { return m_LimitSign; }
            set { m_LimitSign = value; }
        }

        private string m_RegionExpression;
        public string RegionExpression
        {
            get { return m_RegionExpression; }
            set { m_RegionExpression = value; }
        }

        //�жϴ��ֶ��Ƿ���Ҫ�������ݺϲ�������ϲ���������������ֶ���ͬ�Ż���кϲ�
        private bool m_IsMergeData;
        public bool IsMergeData
        {
            get { return m_IsMergeData; }
            set { m_IsMergeData = value; }
        }

        //�ɼ����ݵ�����
        private cGlobalParas.GDataType m_DataType;
        public cGlobalParas.GDataType DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

       
        ///������Ϣ������汾1.6��������Ҫ����չ����������Ŀ��ƹ���
        ///ԭ�й����������ϣ�����ɾ��
        private List<eFieldRule> m_ExportRules;
        public List<eFieldRule> ExportRules
        {
            get { return m_ExportRules; }
            set { m_ExportRules = value; }
        }

        //����������1.63�����ӣ�Ŀ����Ϊ��֧�ֶ�㼶�����ݲɼ�
        //�����㼶��Ĭ��Ϊ��0 �����ɼ�����ҳ����
        private int m_NavLevel;
        public int NavLevel
        {
            get { return m_NavLevel; }
            set { m_NavLevel = value; }
        }

        //�ɼ�����V2.6���ӣ����������ļ��ĵ�ַ����������
        //�����ļ�����ĵ�ַ
        private string m_DownloadFileSavePath;
        public string DownloadFileSavePath
        {
            get { return m_DownloadFileSavePath; }
            set { m_DownloadFileSavePath = value; }
        }

        //private string m_DownloadFileDealType;
        //public string  DownloadFileDealType
        //{
        //    get { return m_DownloadFileDealType; }
        //    set { m_DownloadFileDealType = value; }
        //}

        private string m_MultiPageName;
        public string MultiPageName
        {
            get { return m_MultiPageName; }
            set { m_MultiPageName = value; }
        }

        //����Ϊ3.1����

        /// <summary>
        /// �ɼ���������ҳ������
        /// </summary>
        private cGlobalParas.GatherRuleByPage m_RuleByPage;
        public cGlobalParas.GatherRuleByPage RuleByPage
        {
            get { return m_RuleByPage; }
            set { m_RuleByPage = value; }
        }

        /// <summary>
        /// �ɼ��������Normal-����ɼ���XPath-xPath�ɼ�
        /// </summary>
        private cGlobalParas.GatherRuleType m_GatherRuleType;
        public cGlobalParas.GatherRuleType GatherRuleType
        {
            get { return m_GatherRuleType; }
            set { m_GatherRuleType = value; }
        }

        private string m_XPath;
        public string XPath
        {
            get { return m_XPath; }
            set { m_XPath = value; }
        }

        /// <summary>
        /// �ɼ��ڵ�����ԣ�ֻ�в�XPathʱ��Ч
        /// </summary>
        private string m_NodePrty;
        public string NodePrty
        {
            get { return m_NodePrty; }
            set { m_NodePrty = value; }
        }

        private bool m_IsAutoDownloadFileImage;
        public bool IsAutoDownloadFileImage
        {
            get { return m_IsAutoDownloadFileImage; }
            set { m_IsAutoDownloadFileImage = value; }
        }

        //����Ϊ3.11����

        //����ΪV5.0����
        private bool m_IsAutoDownloadOnlyImage;
        public bool IsAutoDownloadOnlyImage
        {
            get { return m_IsAutoDownloadOnlyImage; }
            set { m_IsAutoDownloadOnlyImage = value; }
        }
    }
}
