using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Gather;
using NetMiner.Common;

namespace NetMiner.Gather.Radar
{
    public class cRadarUrl
    {
        private NetMiner.Base.cHashTree m_Urls;
        private string m_rName;
        private string m_workPath = string.Empty;

        public cRadarUrl(string workPath, string rName)
        {
            m_workPath = workPath;
            if (rName == "")
                return;

            m_rName = rName;
            m_Urls = new NetMiner.Base.cHashTree();
            m_Urls.Open(workPath + "monitor\\" + rName + ".db");
        }

        ~cRadarUrl()
        {
            if (this.m_rName == "")
                return;

            m_Urls.Save(m_workPath + "monitor\\" + m_rName + ".db");
            m_Urls = null;
        }

        public bool AddUrl(string url)
        {
            if (m_Urls == null)
                return false;

            return m_Urls.Add(ref url, false);
        }
    }
}
