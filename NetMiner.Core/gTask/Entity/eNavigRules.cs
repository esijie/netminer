using System;
using System.Collections.Generic;
using System.Text;

///���ܣ��������򼯺��࣬��Ҫ������ʱ�洢��������
/// ����������Ϣά������
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
    public class eNavigRules
    {
        public eNavigRules()
        {
            m_NavigRule = new List<eNavigRule>();
        }

        ~eNavigRules()
        {
            m_NavigRule = null;
        }

        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private List<eNavigRule> m_NavigRule;
        public List<eNavigRule> NavigRule
        {
            get { return m_NavigRule; }
            set { m_NavigRule = value; }
        }
    }
}
