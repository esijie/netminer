using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using NetMiner.Resource;
using NetMiner.Gather.Task;
using NetMiner.Gather.Control;

namespace SoukeySplitService
{
    [ServiceContract(Namespace = "http://www.netminer.cn")]
    public interface iGather
    {
        [OperationContract]
        bool isConnect();
        [OperationContract]
        bool StartTask(string TaskName);
        [OperationContract]
        bool StopTask(long tID);
        [OperationContract]
        bool DelTask(long tID);
        //[OperationContract]
        //bool ResetTask(long tID);
        [OperationContract]
        int GetTaskState(long tID);
        [OperationContract]
        List<cTaskData> GetTaskList();
        [OperationContract]
        List<cTaskData> GetTaskRunList();
      
    }
    
}
