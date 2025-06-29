using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Publish.Rule
{
    public class cPara
    {
        private string m_Key;
        public string Key
        {
            get { return m_Key; }
            set { m_Key = value; }
        }

        private string m_Value;
        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }
}
