using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Resource;

namespace NetMiner.Core.gTask.Entity
{
    [Serializable]
    public class eTaskRun
    {
        public long TaskID { get; set; }
        public string TaskName { get; set; }
        public string TaskClass { get; set; }
        /// <summary>
        /// 是一个相对路径
        /// </summary>
        public string TaskClassPath { get; set; }
        public cGlobalParas.TaskType TaskType { get; set; }
        public cGlobalParas.TaskRunType TaskRunType { get; set; }
        public string TempFile { get; set; }
        public string ExportFile { get; set; }
        public cGlobalParas.PublishType PublishType { get; set; }
        public cGlobalParas.TaskState TaskState { get; set; }
        /// <summary>
        /// 当前采集任务的进程
        /// </summary>
        public cGlobalParas.TaskProcess Process { get; set; }
        public int UrlCount { get; set; }
        public int RowsCount { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int UrlNaviCount { get; set; }
        public int GatheredUrlCount { get; set; }
        public int GatheredUrlNaviCount { get; set; }
        public int ErrUrlCount { get; set; }
        public int ErrUrlNaviCount { get; set; }
        public int ThreadCount { get; set; }
    }
}
