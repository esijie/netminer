using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;

namespace SoukeySplitService
{
    [Serializable]
    public class cTaskData
    {
        public long TaskID { get; set; }
        public string TaskName { get; set; }
        public cGlobalParas.TaskState TaskState { get; set; }
        public int UrlCount { get; set; }
        public int ErrCount { get; set; }
        public int GatherUrlCount { get; set; }
    }
}
