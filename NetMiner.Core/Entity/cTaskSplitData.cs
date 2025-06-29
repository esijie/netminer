using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;

///���ܣ��ɼ����� �ֽ������� ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.Entity
{
    //����ֽ�����,�ֽ�������һ��������������ִ�е��߳������зֽ�
    //���ڼ�¼�ɼ������е�ʵʱ��������
    /// <summary>
    /// �ֽ��������������
    /// </summary>
    public class cTaskSplitData
    {

        #region ���� ����
        public cTaskSplitData()
        {
            m_CurIndex = 0;
            m_GatheredUrlCount = 0;
            m_GatheredErrUrlCount = 0;
        }

        ~cTaskSplitData()
        {
        }
        #endregion

        #region �ɼ��ֽ���������

        //��ʼ�ɼ���λ������
        private int m_BeginIndex;
        public int BeginIndex
        {
            get { return m_BeginIndex; }
            set { m_BeginIndex = value; }
        }

        //�����ɼ���λ������
        private int m_EndIndex;
        public int EndIndex
        {
            get { return m_EndIndex; }
            set { m_EndIndex = value; }
        }

        private int m_CurIndex;
        public int CurIndex
        {
            get { return m_CurIndex; }
            set { m_CurIndex = value; }
        }

        //��ǰ���ڲɼ���Url��ַ
        private string m_CurUrl;
        public string CurUrl
        {
            get { return m_CurUrl; }
            set { m_CurUrl = value; }
        }
        
        /// <summary>
        /// �ֽ�����󣬻�ȡ����ַ����������ַ���������޸ģ�ֻ��
        /// ͨ������������
        /// </summary>
        public int UrlCount
        {
            get { return (EndIndex - BeginIndex+1); }
        }

        /// <summary>
        /// �Ѿ��ɼ������������������õ���ַ��������������������ַ����
        /// </summary>
        private int m_GatheredUrlCount;
        public int GatheredUrlCount
        {
            get { return m_GatheredUrlCount; }
            set { m_GatheredUrlCount = value; }
        }

        /// <summary>
        /// �ɼ��������ַ�����������õ���ַ��������������������ַ
        /// </summary>
        private int m_GatheredErrUrlCount;
        public int GatheredErrUrlCount
        {
            get { return m_GatheredErrUrlCount; }
            set { m_GatheredErrUrlCount = value; }
        }

        /// <summary>
        /// ���ݵ���ҳ������������ַ��������Ҫ�ɼ�
        /// </summary>
        private int m_UrlNaviCount;
        public int UrlNaviCount
        {
            get { return m_UrlNaviCount; }
            set { m_UrlNaviCount = value; }
        }

        /// <summary>
        /// ���ݵ������򣬵�����������ַ���Ѿ��ɼ�������
        /// </summary>
        private int m_GatheredUrlNaviCount;
        public int GatheredUrlNaviCount
        {
            get { return m_GatheredUrlNaviCount; }
            set { m_GatheredUrlNaviCount = value; }
        }

        /// <summary>
        /// ���ݵ������򣬵�����������ַ���ɼ��������������
        /// </summary>
        private int m_GatheredErrUrlNaviCount;
        public int GatheredErrUrlNaviCount
        {
            get { return m_GatheredErrUrlNaviCount; }
            set { m_GatheredErrUrlNaviCount = value; }
        }

        //�˷ֽ�������Ҫ�ɼ�����ҳ��ַ
        private List<eWebLink> m_Weblink;
        public List<eWebLink> Weblink
        {
            get { return m_Weblink; }
            set { m_Weblink = value; }
        }

        //�˷ֽ�����ɼ���ҳ��ַ���ݵĽ�ȡ��ʶ
        private List<eWebpageCutFlag> m_CutFlag;
        public List<eWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }
        #endregion

    }
}
