using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Publish.Rule
{
    public class cPublishGlobalPara
    {
        private string m_Label;
        public string Label
        {
            get { return m_Label; }
            set { m_Label = value; }
        }

        private string m_value;
        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        private cGlobalParas.PublishGlobalParaPage m_pgPage;
        public cGlobalParas.PublishGlobalParaPage pgPage
        {
            get { return m_pgPage; }
            set { m_pgPage = value; }
        }
    }
}
