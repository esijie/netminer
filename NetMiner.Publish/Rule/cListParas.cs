using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Publish.Rule
{
    public class cListParas
    {
        private List<cPara> m_Paras;

        public cListParas()
        {
            m_Paras = new List<cPara>();
        }

        ~cListParas()
        {
            m_Paras = null;
        }

       

        public void Add(string key,string value)
        {
            cPara p = new cPara();
            p.Key = key;
            p.Value = value;
            m_Paras.Add(p);
        }
    }
}
