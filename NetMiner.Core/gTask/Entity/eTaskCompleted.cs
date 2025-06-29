using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eTaskCompleted
    {
        public long TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskClass { get; set; }
        public cGlobalParas.GatherResult GatherResult { get; set; }
        public cGlobalParas.TaskType TaskType { get; set; }
        public cGlobalParas.TaskRunType TaskRunType { get; set; }
        public string TempFile { get; set; }
        public string ExportFile { get; set; }
        public int UrlCount { get; set; }
        public int GatheredUrlCount { get; set; }
        public cGlobalParas.PublishType PublishType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public int RowsCount { get; set; }
    }
}
