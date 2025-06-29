using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace MinerDistri.Distributed
{
    public class cSplitTaskEntity
    {
        public int TaskID;
        public string TaskName;
        public cGlobalParas.TaskState tState;
        public string StartDate;
        public string EndDate;
        public string ClientID;
        public string sPath;
    }
}
