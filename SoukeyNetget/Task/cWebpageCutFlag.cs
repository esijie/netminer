using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��ɼ����� �ɼ���־ �洢����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace SoukeyNetget.Task
{
    public class cWebpageCutFlag
    {
        #region ���������
        public cWebpageCutFlag()
        {
            m_ExportRules = new List<cFieldRule>();
        }

        ~cWebpageCutFlag()
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
        private int m_LimitSign;
        public int LimitSign
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
        private int m_DataType;
        public int DataType
        {
            get { return m_DataType; }
            set { m_DataType = value; }
        }

        ///������Ϣ������汾1.6��������Ҫ����չ����������Ŀ��ƹ���
        ///ԭ�й����������ϣ�����ɾ��
        private List<cFieldRule> m_ExportRules;
        public List<cFieldRule> ExportRules
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
    }
}
