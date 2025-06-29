using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Task
{
    public class cMultiPageRules
    {
        public cMultiPageRules()
        {
            m_MultiPageRule=new List<cMultiPageRule> ();
        }

        ~cMultiPageRules()
        {
            m_MultiPageRule = null;
        }

        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private List<cMultiPageRule> m_MultiPageRule;
        public List<cMultiPageRule> MultiPageRule
        {
            get { return m_MultiPageRule; }
            set { m_MultiPageRule = value; }
        }
    }
}
