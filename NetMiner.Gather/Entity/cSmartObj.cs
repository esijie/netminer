using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Gather.Entity
{
    public class cSmartObj
    {

        private int m_ColIndex;
        public int ColIndex
        {
            get { return m_ColIndex; }
            set { m_ColIndex = value; }
        }

        private cGlobalParas.GDataType m_GType;
        public cGlobalParas.GDataType GType
        {
            get { return m_GType; }
            set { m_GType = value; }
        }
    }
}
