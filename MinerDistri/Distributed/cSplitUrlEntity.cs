using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;

namespace MinerDistri.Distributed
{
    public class cSplitUrlEntity
    {
        private eWebLink m_link;
        public eWebLink link
        {
            get { return m_link; }
            set { m_link = value; }
        }

        private cGlobalParas.SplitTaskState m_sState;
        public cGlobalParas.SplitTaskState sState
        {
            get { return m_sState; }
            set { m_sState = value; }
        }
    }
}
