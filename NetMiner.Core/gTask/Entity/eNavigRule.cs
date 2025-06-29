using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

///�޶���2010-12-5 �ɼ���������ΪV2/0�������˵���ҳ��ҳ�Ĺ��ܣ�������
///��ԭ�е�����ҳ��ҳ������ͻ�����ԣ���������Щ�����ʣ���ע�⣡
namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    /// <summary>
    /// ����������
    /// </summary>
    public class eNavigRule
    {

        //����Ӧ��Url��ַ
        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private int m_Level;
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        #region ��Ϊ�ɼ�����V2.0����
        //����ҳ����ķ�ҳ����
        private bool m_IsNext;
        public bool IsNext
        {
            get { return m_IsNext; }
            set { m_IsNext = value; }
        }

        private string m_NextRule;
        public string NextRule
        {
            get { return m_NextRule; }
            set { m_NextRule = value; }
        }

        //�ɼ�����V3.0���ӣ��޶��Զ���ҳ�����ҳ��
        private string m_NextMaxPage;
        public string NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        //private bool m_IsNextDoPostBack;
        //public bool IsNextDoPostBack
        //{
        //    get { return m_IsNextDoPostBack; }
        //    set { m_IsNextDoPostBack = value; }
        //}

        #endregion

        //����������ҳ�淭ҳ����
        private bool m_IsNaviNextPage;
        public bool IsNaviNextPage
        {
            get { return m_IsNaviNextPage; }
            set { m_IsNaviNextPage = value; }
        }

        private string m_NaviNextPage;
        public string NaviNextPage
        {
            get { return m_NaviNextPage; }
            set { m_NaviNextPage = value; }
        }

        //�ɼ�����V3.0���ӣ��޶��Զ���ҳ�����ҳ��
        private string m_NaviNextMaxPage;
        public string NaviNextMaxPage
        {
            get { return m_NaviNextMaxPage; }
            set { m_NaviNextMaxPage = value; }
        }

        //private bool m_IsNaviNextDoPostBack;
        //public bool IsNaviNextDoPostBack
        //{
        //    get { return m_IsNaviNextDoPostBack; }
        //    set { m_IsNaviNextDoPostBack = value; }
        //}

        //����������ȡ��Χ���ر�ע�⣺��ͬ�㼶�ĵ�����ȡ�ķ�ΧҲ�ǲ�һ���ġ�
        private string m_NaviStartPos;
        public string NaviStartPos
        {
            get { return m_NaviStartPos; }
            set { m_NaviStartPos = value; }
        }

        private string m_NaviEndPos;
        public string NaviEndPos
        {
            get { return m_NaviEndPos; }
            set { m_NaviEndPos = value; }
        }

        private string m_NavigRule;
        public string NavigRule
        {
            get { return m_NavigRule; }
            set { m_NavigRule = value; }
        }

        //������1.63�汾�У����������˲ɼ����ܣ���ʵ���˷�ҳ�ɼ�
        private bool m_IsGather;
        public bool IsGather
        {
            get { return m_IsGather; }
            set { m_IsGather = value; }
        }

        private string m_GatherStartPos;
        public string GatherStartPos
        {
            get { return m_GatherStartPos; }
            set { m_GatherStartPos = value; }
        }

        private string m_GatherEndPos;
        public string GatherEndPos
        {
            get { return m_GatherEndPos; }
            set { m_GatherEndPos = value; }
        }

        //����������ҳʱ���¼��ǰҳ��Ĳ���������������ֻ����ҵ����ַ������ʱ��ʹ�ã������ط�����
        //ʹ������������
        private int m_NextCurrentPage;
        public int NextCurrentPage
        {
            get { return m_NextCurrentPage; }
            set { m_NextCurrentPage = value; }
        }

        private int m_NaviNextCurrentPage;
        public int NaviNextCurrentPage
        {
            get { return m_NaviNextCurrentPage; }
            set { m_NaviNextCurrentPage = value; }
        }

        //V5.2����
        private cGlobalParas.NaviRunRule m_RunRule;
        public cGlobalParas.NaviRunRule RunRule
        {
            get { return m_RunRule; }
            set { m_RunRule = value; }
        }

        private string m_OtherNaviRule;
        public string OtherNaviRule
        {
            get { return m_OtherNaviRule; }
            set { m_OtherNaviRule = value; }
        }
    }
}
