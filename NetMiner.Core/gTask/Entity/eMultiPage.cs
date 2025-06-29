using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Core.gTask.Entity;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eMultiPage
    {

        public eMultiPage()
        {
            m_WebPageCutFlags = new List<eWebpageCutFlag>();
        }

        ~eMultiPage()
        {
            m_WebPageCutFlags = null;
        }

        private string m_Url;
        public string Url
        {
            get { return m_Url; }
            set { m_Url = value; }
        }

        private List<eWebpageCutFlag> m_WebPageCutFlags;
        public List<eWebpageCutFlag> WebPageCutFlags
        {
            get { return m_WebPageCutFlags; }
            set { m_WebPageCutFlags = value; }
        }
    }
}
