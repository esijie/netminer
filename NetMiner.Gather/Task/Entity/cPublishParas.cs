using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Gather.Task
{
    public class cPublishPara
    {
        private string m_PublishPara;
        public string PublishPara
        {
            get { return m_PublishPara; }
            set { m_PublishPara = value; }
        }

        private string m_PublishValue;
        public string PublishValue
        {
            get { return m_PublishValue; }
            set { m_PublishValue = value; }
        }

        private cGlobalParas.PublishParaType m_PublishParaType;
        public cGlobalParas.PublishParaType PublishParaType
        {
            get { return m_PublishParaType; }
            set { m_PublishParaType = value; }
        }
    }
}
