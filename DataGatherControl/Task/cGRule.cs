using System;
using System.Collections.Generic;
using System.Text;

namespace DataGatherControl.Task
{
    public class cGRule
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_xPath;
        public string xPath
        {
            get { return m_xPath; }
            set { m_xPath = value; }
        }

        private cGlobal.GatherDataType m_gType;
        public cGlobal.GatherDataType gType
        {
            get { return m_gType; }
            set { m_gType = value; }
        }
    }
}
