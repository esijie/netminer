using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SominerMonitor
{
    public class eMessageEvent : EventArgs
    {
        public eMessageEvent(string strInfo)
        {
            m_strInfo = strInfo;
        }

        private string m_strInfo;
        public string strInfo
        {
            get { return m_strInfo; }
            set { m_strInfo = value; }
        }
    }
}
