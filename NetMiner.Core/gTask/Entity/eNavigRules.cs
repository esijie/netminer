using System;
using System.Collections.Generic;
using System.Text;

///功能：导航规则集合类，主要用于临时存储导航规则
/// 用于任务信息维护管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
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
