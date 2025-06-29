using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace MinerDistri.Distributed
{
    public class cSplitedTask
    {
        private int _tID;
        public int tID
        {
            get { return _tID; }
            set { _tID = value; }
        }

        private cGlobalParas.SplitTaskState _sState;
        public cGlobalParas.SplitTaskState sState
        {
            get { return _sState; }
            set { _sState = value; }
        }

        private List<cSplitTaskEntity> _Tasks;
        public List<cSplitTaskEntity> Tasks
        {
            get { return _Tasks; }
            set { _Tasks = value; }
        }
    }
}
