using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eMultiPageRules
    {
        public eMultiPageRules()
        {
            m_MultiPageRule=new List<eMultiPageRule> ();
        }

        ~eMultiPageRules()
        {
            m_MultiPageRule = null;
        }

        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private List<eMultiPageRule> m_MultiPageRule;
        public List<eMultiPageRule> MultiPageRule
        {
            get { return m_MultiPageRule; }
            set { m_MultiPageRule = value; }
        }
    }
}
