using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource ;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eThreadProxy
    {
        private int m_Index;
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        private cGlobalParas.ProxyType m_pType;
        public cGlobalParas.ProxyType pType
        {
            get { return m_pType; }
            set { m_pType = value; }
        }

        private string m_Address;
        public string Address
        {
            get { return m_Address; }
            set { m_Address = value; }
        }

        private int m_Port;
        public int Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

    }
}
