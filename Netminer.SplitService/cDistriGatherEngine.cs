using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Gather.Control;

namespace SoukeySplitService
{
    public class cDistriGatherEngine:iGather
    {
        cDistriGatherEngine()
        {
        }

        ~cDistriGatherEngine()
        {
        }

        public bool isConnect()
        {
            if (cGlobal.sEngine != null)
                return true;
            else
                return false;
        }

        public bool StartTask(string TaskName)
        {
            cGlobal.sEngine.StartTask(TaskName);
            return true;
        }

        public bool StopTask(long tID)
        {
            cGlobal.sEngine.StopTask(tID);
            return true;
        }

        //public bool ResetTask(long tID)
        //{
        //    cGlobal.sEngine.ResetTask(tID);
        //    return true;
        //}

        public int GetTaskState(long tID)
        {
            return (int)cGlobal.sEngine.GetTaskState(tID);
        }

        public List<cTaskData> GetTaskList()
        {
            List<cTaskData> tDatas = new List<cTaskData>();
            for (int i = 0; i < cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; i++)
            {
                cTaskData tData = new cTaskData();
                tData.TaskID = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskID;
                tData.TaskName = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskName;
                tData.TaskState = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskState;
                tData.UrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].UrlCount;
                tData.ErrCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatherErrUrlCount;
                tData.GatherUrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredUrlCount;
                tDatas.Add(tData);
            }

            for (int i = 0; i < cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count; i++)
            {
                cTaskData tData = new cTaskData();
                tData.TaskID = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].TaskID;
                tData.TaskName = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].TaskName;
                tData.TaskState = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].TaskState;
                tData.UrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].UrlCount;
                tData.ErrCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].GatherErrUrlCount;
                tData.GatherUrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.StoppedTaskList[i].GatheredUrlCount;
                tDatas.Add(tData);
            }

            for (int i = 0; i < cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList.Count; i++)
            {
                cTaskData tData = new cTaskData();
                tData.TaskID = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].TaskID;
                tData.TaskName = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].TaskName;
                tData.TaskState = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].TaskState;
                tData.UrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].UrlCount;
                tData.ErrCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].GatherErrUrlCount;
                tData.GatherUrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.CompletedTaskList[i].GatheredUrlCount;
                tDatas.Add(tData);
            }

            return tDatas;
        }

        public List<cTaskData> GetTaskRunList()
        {
            List<cTaskData> tDatas = new List<cTaskData>();
            for (int i = 0; i < cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; i++)
            {
                cTaskData tData = new cTaskData();
                tData.TaskID = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskID;
                tData.TaskName = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskName;
                tData.TaskState = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].TaskState;
                tData.UrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].UrlCount;
                tData.ErrCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatherErrUrlCount;
                tData.GatherUrlCount = cGlobal.sEngine.GatherControl.TaskManage.TaskListControl.RunningTaskList[i].GatheredUrlCount;
                tDatas.Add(tData);
            }
            return tDatas;
        }

        public bool DelTask(long TaskID)
        {
            cGlobal.sEngine.DelTask(TaskID);
            return true;
        }

      
   
    }
}
