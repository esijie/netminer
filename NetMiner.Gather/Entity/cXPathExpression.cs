using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Gather.Entity
{
    public class cXPathExpression
    {
        private string m_XPath;
        public string XPath
        {
            get { return m_XPath; }
            set { m_XPath = value; }
        }

        private int m_ColIndex;
        public int ColIndex
        {
            get { return m_ColIndex; }
            set { m_ColIndex = value; }
        }

        private string m_iFrameIndex;
        public string iFrameIndex
        {
            get { return m_iFrameIndex; }
            set { m_iFrameIndex = value; }
        }

        private cGlobalParas.HtmlNodeTextType m_NodePrty;
        public cGlobalParas.HtmlNodeTextType NodePrty
        {
            get { return m_NodePrty; }
            set { m_NodePrty = value; }
        }
    }
}
