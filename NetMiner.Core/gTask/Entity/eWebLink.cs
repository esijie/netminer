using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

///���ܣ��ɼ����� URL�洢 ����
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
    public class eWebLink
    {

        #region ���� ����
        public eWebLink()
        {
            m_IsGathered =(int) cGlobalParas.UrlGatherResult.UnGather;
            m_NavigRules = new List<eNavigRule>();
            m_MultiPageRules = new List<eMultiPageRule>();
        }

        ~eWebLink()
        {
            m_NavigRules = null;
            m_MultiPageRules = null;
        }

        #endregion

        /// <summary>
        /// ע�⣺��Ϊ����������Ƴ�����һ���������
        /// </summary>
        /// <returns></returns>
        public eWebLink DeepClone()
        {

            return NetMiner.Base.Tool.DeepCopyWithReflection<eWebLink>(this);
        }

        

        #region ����
        private int m_id;
        public int id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_Weblink;
        /// <summary>
        /// ���õĲɼ���ַ
        /// </summary>
        public string Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        //�Ƿ�Ϊ����ҳ������ǵ���ҳ����Ҫ���ݵ�����������������Ҳ����ȡ
        private bool m_IsNavigation;
        public bool IsNavigation
        {
            get { return m_IsNavigation; }
            set { m_IsNavigation = value; }
        }

        //��㵼��������һ��������
        private List<eNavigRule> m_NavigRules;
        public List<eNavigRule> NavigRules
        {
            get { return m_NavigRules; }
            set { m_NavigRules = value; }
        }

        //�Ƿ���ȡ��һҳ��ʶ
        //ע�⣺�ڴ˵���һҳ��ʶ��ָ�ǵ���ҳ����һҳ��ҳ��ʾ
        //����ǵ���ҳ������һҳ��־��¼�ڵ���ҳ������
        private bool m_IsNextPage;
        public bool IsNextpage
        {
            get { return m_IsNextPage; }
            set { m_IsNextPage = value; }
        }

        //��һҳ��ʶ
        private string m_NextPageRule;
        public string NextPageRule
        {
            get { return m_NextPageRule; }
            set { m_NextPageRule = value; }
        }

        //�ɼ�����V3.0���ӣ������Զ���ҳ�����ҳ��
        private string m_NextMaxPage;
        public string  NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        //��¼̽�⵽����һҳ��Url
        private string m_NextPageUrl;
        public string NextPageUrl
        {
            get { return m_NextPageUrl; }
            set { m_NextPageUrl = value; }
        }

        //��ʶ��ǰ��ҳ��ַ�Ƿ��Ѿ��ɼ�,Ĭ��cGlobalParas.UrlGatherResult.UnGather
        private int m_IsGathered;
        /// <summary>
        /// ��¼��ǰ��ַ�ɼ��Ľ�� ��ӦcGlobalParas.GatherResult
        /// </summary>
        public int IsGathered
        {
            get { return m_IsGathered; }
            set { m_IsGathered = value; }
        }
       

        private string m_CurrentRunning;
        public string CurrentRunning
        {
            get { return m_CurrentRunning; }
            set { m_CurrentRunning = value; }
        }

        //����󹤲ɼ�����V3.0���ӣ���ҳ�ɼ�������һ����־
        private bool m_IsMultiGather;
        public bool IsMultiGather
        {
            get { return m_IsMultiGather; }
            set { m_IsMultiGather = value; }
        }

        //V5.0.1���ӣ����ж�ҳ���ݵ�1��1��ǿ�ƺϲ�
        private bool m_IsData121;
        public bool IsData121
        {
            get { return m_IsData121; }
            set { m_IsData121 = value; }
        }


        //��ַ�Ķ�ҳ�ɼ�����
        private List<eMultiPageRule> m_MultiPageRules;
        public List<eMultiPageRule> MultiPageRules
        {
            get { return m_MultiPageRules; }
            set { m_MultiPageRules = value; }
        }

        /// <summary>
        /// ��¼��ǰ��ַ����·ҳ�棬��ǰ���ڷֲ�ʽ�ɼ���ʹ��
        /// </summary>
        private string m_referUrl;
        public string referUrl
        {
            get { return m_referUrl; }
            set { m_referUrl = value; }
        }

        #endregion

        

    }
}
