using System;
using System.Collections.Generic;
using System.Text;

namespace MinerDistri.Management
{
    public class cRemoteTaskEntity
    {
        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private int _TID;
        public int TID
        {
            get { return _TID; }
            set { _TID = value; }
        }

        private string _TaskFileName;
        /// <summary>
        /// 完整的任务文件路径+名称
        /// </summary>
        public string TaskFileName
        {
            get { return _TaskFileName; }
            set { _TaskFileName = value; }
        }

        private string _TaskName;
        public string TaskName
        {
            get { return _TaskName; }
            set { _TaskName = value; }
        }

        private int _GatherTaskType;
        public int GatherTaskType
        {
            get { return _GatherTaskType; }
            set { _GatherTaskType = value; }
        }

        private bool _isSubRun;
        public bool isSubRun
        {
            get { return _isSubRun; }
            set { _isSubRun = value; }
        }
    }
}
