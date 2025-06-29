using System;
using System.Collections.Generic;
using System.Text;

namespace MinerSpider.Help
{
    public class cHelpContent
    {
        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_Content;
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }
    }
}
