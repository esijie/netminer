using System;
using System.Collections.Generic;
using NetMiner.Gather.Control;
using NetMiner.Gather.Task;
using NetMiner.Resource;
using System.Data;
using System.IO;
using MinerDistri.Management;
using System.Diagnostics;
using System.Windows.Forms;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;


///分布式采集，客户端的采集引擎，安装为服务，用于接收分布式采集引擎分解后的采集任务
///数据采集操作
namespace NetMiner.Engine.ClientEngine
{
    public class cSplitControl
    {
        private cGatherControl m_GatherControl;
        private string m_EPath = string.Empty;
        private string m_RegisterUser = string.Empty;
        public List<cSplitRunningInfo> m_rInfos;
        private string m_conn;

        private string m_RemoteServer = string.Empty;
        private bool m_IsRemoteAuthen = false;
        private string m_RemoteUser = string.Empty;
        private string m_RemotePwd = string.Empty;
        private bool m_isClientRunning = false;
        private string m_workPath = string.Empty;

        private bool m_IsReport = false;
        private string m_ReportEmail = string.Empty;
        private string m_ReportTimer = string.Empty;

        private bool m_isGather = false;

        private cGlobalParas.VersionType m_EngineVersion;

        private bool m_isRemoteGahter = false;
        private string m_RemotePath = string.Empty;
        private int m_RemoteMaxCount = 0;

        private const string g_RemoteTaskClass = "远程";
        private const string g_RemoteTaskPath = "tasks\\RemoteTask";

        //每次上传文件的数量
        private const int UploadFileLength = 5120000;


        //采集引擎用的定时器
        private System.Threading.Timer m_GatherEngine;


        public cSplitControl()
        {
            cXmlSConfig sCon = new cXmlSConfig();
            m_EPath = sCon.RemotePath;
            sCon = null;

            m_rInfos = new List<cSplitRunningInfo>();

            NetMiner.Base.cHashTree tmpUrls = null; 
            m_GatherControl = new cGatherControl(m_EPath,false,ref tmpUrls);

            //采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
            m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized += tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged += tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError += tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted += tManage_TaskAbort;
            m_GatherControl.TaskManage.Log += tManage_Log;
            m_GatherControl.TaskManage.GData += tManage_GData;
            m_GatherControl.TaskManage.GUrlCount += tManage_GUrlCount;
            m_GatherControl.Completed += m_Gather_Completed;

            //加载运行区的数据,运行区的数据主要是根据taskrun.xml(默认在Tasks\\TaskRun.xml)文件中
            //的内容进行加载,

            cTaskDataList gList = new cTaskDataList();


            //根据加载的运行区的任务信息,开始初始化采集任务
            try
            {
                gList.LoadTaskRunData(m_EPath);

                //在此增加采集运行中的任务
                bool IsAddRTaskSucceed = m_GatherControl.AddGatherTask(gList);

            }
            catch (System.Exception ex)
            {

            }

        }

        ~cSplitControl()
        {
            m_GatherControl.TaskManage.TaskCompleted -= tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted -= tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized -= tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged -= tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped -= tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError -= tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed -= tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted -= tManage_TaskAbort;
            m_GatherControl.TaskManage.Log -= tManage_Log;
            m_GatherControl.TaskManage.GUrlCount -= tManage_GUrlCount;
            m_GatherControl.TaskManage.GData -= tManage_GData;
            m_GatherControl.Completed -= m_Gather_Completed;
            m_GatherControl = null;
        }

        public cGatherControl GatherControl
        {
            get { return m_GatherControl; }
        }

        /// <summary>
        /// 启动采集服务
        /// </summary>
        /// <returns></returns>
        public bool StartClientGather()
        {

            //先加载配置文件的信息
            //加载配置文件信息 每次服务启动时加载，确保可以修改规则有效
            #region 初始化服务器引擎的基本信息
            this.m_workPath = Application.StartupPath + "\\";
            //WriteSystemLog(m_workPath, EventLogEntryType.Error);

            cXmlSConfig sCon = new cXmlSConfig();
            m_conn = NetMiner.Common.ToolUtil.DecodingDBCon(sCon.DataConnection);
            

            this.m_IsReport = sCon.IsReport;
            this.m_ReportEmail = sCon.ReportEmail;
            this.m_ReportTimer = sCon.ReportSendTime;

            //加载分布式采集的操作
            m_isRemoteGahter = sCon.IsRemote;
            m_RemoteServer = sCon.RemoteServer;
            m_IsRemoteAuthen = sCon.IsAuthen;
            m_RemoteUser = sCon.RemoteUser;
            m_RemotePwd = sCon.RemotePwd;
            m_RemotePath = sCon.RemotePath;
            m_RemoteMaxCount = sCon.RemoteMaxRunTask;
            sCon = null;

            //取消版本验证，因为这个采集只对采集引擎分布式操作
            //无需验证版本授权，采集即可。
            //先进行版本验证
            //启动服务的时候开始验证版本
            //try
            //{
            //    cVersion v = new cVersion(this.m_workPath);


            //    if (v.ReadRegisterInfo(this.m_workPath))
            //    {
            //        this.m_RegisterUser = v.RegisterInfo.User;
            //        this.m_EngineVersion = v.SominerVersion;

            //        v = null;
            //    }
            //    else
            //    {
            //        v = null;
            //        cTool.WriteSystemLog("版本未激活，请先激活版本", EventLogEntryType.Error, "SMGatherClient");
            //        return false;
            //    }
            //}
            //catch (System.Exception ex)
            //{
            //    cTool.WriteSystemLog("检查软件激活状态失败，可能您还未激活软件，请激活软件后重新启动服务！", EventLogEntryType.Error, "SMGatherClient");
            //    return false;
            //}

            //if (this.m_EngineVersion != cGlobalParas.VersionType.Server && this.m_EngineVersion != cGlobalParas.VersionType.DistriServer)
            //{
            //    cTool.WriteSystemLog("版本未激活，请先激活版本", EventLogEntryType.Error, "SMGatherClient");
            //    return false;
            //}

            #endregion

            //更新控制器中的代理IP
            m_GatherControl.TaskManage.UpdateProxy();


            //获取当前的注册用户，因为本引擎无需注册，因此就获取本机IP即可
            m_RegisterUser = NetMiner.Common.ToolUtil.GetHtmlSource(this.m_RemoteServer + "/hander/getip.ashx",true);

          
            #region 本引擎分布式采集任务队列的判断
            //创建客户端启动引擎

            //清除本地引擎的所有数据
            oTaskRun tr = new oTaskRun(this.m_RemotePath);
            IEnumerable< eTaskRun> ers= tr.LoadTaskRunData();
            foreach(eTaskRun er in ers)
            {
                string rTaskID = er.TaskID.ToString();
                //删除taskrun节点
                tr.DelTask(long.Parse(rTaskID));

                //删除run中的任务实例文件
                string FileName = this.m_RemotePath + "Tasks\\run\\" + "Task" + rTaskID + ".rst";
                System.IO.File.Delete(FileName);

                //删除run中的任务实例排重库文件
                FileName = this.m_RemotePath + "Tasks\\run\\" + "Task" + rTaskID + ".db";
                System.IO.File.Delete(FileName);

                //删除run中的采集数据排重文件
                FileName = this.m_RemotePath + "Tasks\\run\\" + "data" + rTaskID + ".db";
                System.IO.File.Delete(FileName);

                //删除run中的任务实例文件
                FileName = this.m_RemotePath + "data\\" + er.TaskName + "-" + rTaskID + ".xml";
                System.IO.File.Delete(FileName);

            }
            tr = null;

            //清除本地引擎远程分类下的任务
            oTaskIndex remoteTasks = new oTaskIndex(this.m_RemotePath);
            IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= remoteTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
            foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
            {
                oTask t = new oTask(this.m_RemotePath);
                t.DeleTask(this.m_RemotePath + g_RemoteTaskPath, et.TaskName);
                t = null;
            }

            //删除taskcompleted中的任务
            oTaskComplete tc = new oTaskComplete(this.m_RemotePath);
            tc.Clear();
            tc = null;

            #endregion

            /// 如果是分布式采集引擎，则需要启动本地的采集服务，用于进行分布式采集操作
            /// 启动一个定时器，定期检测本地是否有需要采集的分布式任务，注意：是分布式任务
            ///为何要分开，是因为在不支持分布式采集的情况下，企业版照常可以工作
            m_GatherEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_GatherEngine_CallBack), null, 30000, 30000);
            

            m_isClientRunning = true;
            return true;
        }

        /// <summary>
        /// 停止分布式采集引擎
        /// </summary>
        /// <returns></returns>
        public bool StopClientGather()
        {
            if (m_isClientRunning == false)
                return true;

            m_isClientRunning = false;

            //再判断分布式的采集任务
            if (this.m_EngineVersion == cGlobalParas.VersionType.DistriServer)
            {
                //判断是否有正在运行的任务，如果有，则停止
                oTaskIndex xmlTasks = new oTaskIndex(this.m_RemotePath);
                IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
                foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                {
                    for (int j = 0; j < this.GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; j++)
                    {
                        if (this.GatherControl.TaskManage.TaskListControl.RunningTaskList[j].TaskName == et.TaskName)
                        {
                            try
                            {
                                //如果存在，则停止
                                cGatherTask gt = GatherControl.TaskManage.FindTask(GatherControl.TaskManage.TaskListControl.RunningTaskList[j].TaskID);
                                GatherControl.Stop(gt);
                            }
                            catch (System.Exception ex)
                            {
                                cTool.WriteSystemLog("分布式采集任务停止失败，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherClient");
                            }
                        }
                    }
                }
            }

            return true;
        }

        //采集引擎定时器，只采集分布式任务
        private void m_GatherEngine_CallBack(object State)
        {

            if (m_isRemoteGahter == true && m_isGather == false)
            {
                m_isGather = true;

                try
                {

                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "开始轮询", false));

                    localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
                    
                    sweb.Url = this.m_RemoteServer + "/NetMiner.WebService.asmx";
                    if (this.m_IsRemoteAuthen == true)
                        sweb.Credentials = new System.Net.NetworkCredential(this.m_RemoteUser, this.m_RemotePwd);

                    try
                    {

                        int result = sweb.ActiveClient(this.m_RegisterUser);
                        if (result == 2)
                        {
                            //清除本地的远程任务
                            ClearRemoteTask();
                        }
                    }
                    catch { }

                    oTaskIndex xmlTasks = new oTaskIndex(this.m_RemotePath);
                    IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);
                    int tCount = xmlTasks.GetTaskCount();
                    int rCount = 0;

                    if (tCount < this.m_RemoteMaxCount)
                    {
                        try
                        {
                            #region 从远程获取采集任务
                            localhost.cRemoteTaskEntity getRemoteTask = sweb.GetTaskName(this.m_RegisterUser);

                            if (getRemoteTask != null)
                            {
                                //获取的任务是个zip压缩文件
                                string fName = getRemoteTask.TaskFileName;
                                byte[] taskByte = sweb.GetTaskFile(fName);

                                //先将压缩文件存在本地临时目录
                                string tmpFile = this.m_RemotePath + "tmp\\" + Path.GetFileName(fName);
                                if (!Directory.Exists(this.m_RemotePath + "tmp\\"))
                                    Directory.CreateDirectory(this.m_RemotePath + "tmp\\");
                                if (File.Exists(tmpFile))
                                {
                                    File.Delete(tmpFile);
                                }

                                MemoryStream ms = new MemoryStream(taskByte);
                                FileStream fs = new FileStream(tmpFile, FileMode.Create);
                                ms.WriteTo(fs);
                                fs.Flush();
                                ms.Flush();
                                ms.Close();
                                fs.Close();

                                //获取到文件后开始解压缩
                                NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                                zCompress.DeCompressZIP(tmpFile, this.m_RemotePath + "tmp\\");

                                string FileName = this.m_RemotePath + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                                    + "\\" + Path.GetFileNameWithoutExtension(tmpFile) + ".smt";

                                //插入index数据
                                oTask t = new oTask(this.m_RemotePath);
                                t.LoadTask(FileName);
                                //t.TaskEntity.TaskClass = g_RemoteTaskClass;

                                oTaskIndex ti = new oTaskIndex(this.m_workPath);

                                //先判断索引文件中是否存在此任务，如果存在，则跳过，不进行下载操作
                                if (!ti.isExistTask(Path.GetFileNameWithoutExtension(fName)))
                                {
                                    //只有任务在本地的索引文件不存在，方可进行下载操作
                                    try
                                    {
                                        t.TaskEntity.TaskName = Path.GetFileNameWithoutExtension(fName);
                                        t.TaskEntity.RunType = cGlobalParas.TaskRunType.OnlyGather;
                                        t.TaskEntity.ExportFile = "";
                                        t.TaskEntity.DataSource = "";
                                        t.TaskEntity.ExportType = cGlobalParas.PublishType.NoPublish;
                                        t.TaskEntity.DataSource = "";
                                        t.TaskEntity.DataTableName = "";
                                        t.TaskEntity.IsErrorLog = true;

                                        //将获取任务的信息存储在采集任务备注中
                                        t.TaskEntity.TaskDemo = getRemoteTask.ID + "," + getRemoteTask.TaskName + "," + getRemoteTask.GatherTaskType + "," + getRemoteTask.TaskFileName;

                                        //t.SaveTask(this.m_RemotePath + g_RemoteTaskPath + "\\" + t.TaskName + ".smt");
                                        try
                                        {
                                            t.Save(this.m_RemotePath + g_RemoteTaskPath + "\\", cGlobalParas.opType.Add, true);
                                        }
                                        catch (System.Exception)
                                        {
                                            //表示有重名了，并且是出错了，这个任务已经被下载过，但没有执行。
                                            t.Save(this.m_RemotePath + g_RemoteTaskPath + "\\", cGlobalParas.opType.Add, false);
                                        }
                                        t = null;

                                        bool isUpdateSucc = false;
                                        if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                                            isUpdateSucc = sweb.UpdateSplitTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);
                                        else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                                            isUpdateSucc = sweb.UpdateTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart, 0);

                                        if (isUpdateSucc == false)
                                        {
                                            //更新任务状态失败
                                            cTool.WriteSystemLog("任务下载后更新状态失败,任务名称：" + Path.GetFileNameWithoutExtension(fName), EventLogEntryType.Error, "SMGatherClient");
                                        }

                                        //判断是否有排重库，如果有，则存入排重库
                                        string urlDb = this.m_RemotePath + "tmp\\" + Path.GetFileNameWithoutExtension(tmpFile)
                                            + "\\远程-" + Path.GetFileNameWithoutExtension(tmpFile) + ".db";

                                        if (File.Exists(urlDb))
                                        {
                                            //表示存在排重库
                                            File.Copy(urlDb, this.m_RemotePath + "urls\\" + Path.GetFileName(urlDb), true);
                                        }

                                        ////成功存储到本地
                                        //if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.Normal)
                                        //    sweb.UpdateTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                                        //else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.SplitTask)
                                        //    sweb.UpdateSplitTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteUnstart);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        //重置状态
                                        if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                                            sweb.UpdateTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);
                                        else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                                            sweb.UpdateSplitTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);

                                        //抛出错误
                                        throw ex;

                                    }
                                }
                                else
                                {
                                    //表示索引文件已经存在此任务，将远程任务状态重置
                                    if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                                        sweb.UpdateTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);
                                    else if (getRemoteTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                                        sweb.UpdateSplitTaskState(getRemoteTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.UnStart, 0);

                                }

                                if (File.Exists(tmpFile))
                                    File.Delete(tmpFile);

                                string tPath = Path.GetDirectoryName(tmpFile) + "\\" + Path.GetFileNameWithoutExtension(tmpFile);
                                if (Directory.Exists(tPath))
                                    Directory.Delete(tPath, true);


                                if (e_Log != null)
                                    e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "下载采集任务成功：" + tmpFile, false));

                            }
                            getRemoteTask = null;

                            #endregion
                        }
                        catch (System.Exception ex)
                        {
                            throw new Exception("从远程获取任务出错：" + ex.Message);
                        }

                    }
                    //在此自动执行远程的采集任务，如果已经执行完毕，则自动上传

                    List<Int64> cCompleteTaskID = new List<long>();

                    //获取远程任务的名称
                    Dictionary<string, cGlobalParas.TaskState> rTask = new Dictionary<string, cGlobalParas.TaskState>();

                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "队列数据：" + tCount.ToString(), false));


                    try
                    {
                        #region 获取远程任务执行的状态
                        foreach(NetMiner.Core.gTask.Entity.eTaskIndex et in eIndexs)
                        {
                            if (e_Log != null)
                                e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "队列数据：" + et.TaskName, false));


                            bool isExist = false;
                            bool isExistComplete = false;
                            bool isExistStop = false;

                            //判断running的队列
                            for (int j = 0; j < this.GatherControl.TaskManage.TaskListControl.RunningTaskList.Count; j++)
                            {
                                if (this.GatherControl.TaskManage.TaskListControl.RunningTaskList[j].TaskName ==et.TaskName)
                                {
                                    isExist = true;
                                    rCount++;
                                    break;
                                }
                            }

                            if (isExist == true)
                            {
                                rTask.Add(et.TaskName, cGlobalParas.TaskState.Running);
                            }
                            else
                            {
                                //判断停止的任务队列
                                
                                //判断已经暂停的队列
                                for (int j = 0; j < this.GatherControl.TaskManage.TaskListControl.StoppedTaskList.Count; j++)
                                {
                                    if (this.GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskName == et.TaskName)
                                    {
                                        isExistStop = true;
                                        rTask.Add(et.TaskName, this.GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskState);

                                        if (this.GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskState == cGlobalParas.TaskState.Failed)
                                        {
                                            //在此处理错误任务的问题
                                            DealFaild(this.GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskID, this.GatherControl.TaskManage.TaskListControl.StoppedTaskList[j].TaskName);
                                        }

                                        rCount++;
                                        break;
                                       
                                    }
                                }
                                
                                if (isExistStop == false )
                                {
                                    //判断已经完成的任务队列
                                
                                        //开始判断是否已经执行结束
                                        oTaskComplete t = new oTaskComplete(this.m_RemotePath);
                                        IEnumerable<eTaskCompleted> eTasks= t.LoadTaskData();

                                        foreach(eTaskCompleted ec in eTasks)
                                        {
                                            if (ec.TaskName == et.TaskName)
                                            {
                                                isExistComplete = true;
                                                //rTask.Add(xmlTasks.GetTaskName(i), cGlobalParas.TaskState.Completed);
                                                cCompleteTaskID.Add(ec.TaskID);
                                                break;
                                            }
                                        }
                                        t = null;

                                        if (isExistComplete == false)
                                        {
                                            //在此需要判断下是否在完成的队列中
                                            bool isE = false;
                                            for (int j = 0; j < this.GatherControl.TaskManage.TaskListControl.CompletedTaskList.Count; j++)
                                            {
                                                if (this.GatherControl.TaskManage.TaskListControl.CompletedTaskList[j].TaskName ==et.TaskName)
                                                {
                                                    isE = true;
                                                    rCount++;
                                                    break;
                                                }
                                            }
                                            if (isE == false)
                                                rTask.Add(et.TaskName, cGlobalParas.TaskState.UnStart);
                                            else
                                                rTask.Add(et.TaskName, cGlobalParas.TaskState.Running);
                                        }
                                        else
                                            rTask.Add(et.TaskName, cGlobalParas.TaskState.Completed);
                                    
                                }
                            }
                                
                        }
                        #endregion

                    }
                    catch (System.Exception ex)
                    {
                        throw new Exception("获取任务状态出错：" + ex.Message);
                    }

                    //在此执行采集任务
                    foreach (KeyValuePair<string, cGlobalParas.TaskState> kv in rTask)
                    {

                        if (e_Log != null)
                            e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "任务状态：" + kv.Key + ":" + kv.Value.ToString(), false));


                        if (kv.Value == cGlobalParas.TaskState.Completed)
                        {
                            //处理已经完成的任务

                            if (e_Log != null)
                                e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "上传采集任务：" + kv.Key + ":" + kv.Value.ToString(), false));


                            #region 上传采集任务
                            for (int n = 0; n < cCompleteTaskID.Count; n++)
                            {
                                oTaskComplete tc = new oTaskComplete(this.m_RemotePath);
                                eTaskCompleted ec= tc.LoadSingleTask(cCompleteTaskID[n]);
                                if (ec!= null)
                                {
                                    string dFile = ec.TempFile;
                                    string tName = ec.TaskName;
                                    long uTaskID = ec.TaskID;

                                    tc = null;
                                    try
                                    {


                                        //获取任务的远程信息
                                        oTask t = new oTask(this.m_RemotePath);
                                        string FileName = this.m_RemotePath + g_RemoteTaskPath
                                            + "\\" + tName + ".smt";
                                        t.LoadTask(FileName);
                                        string TaskDemo = t.TaskEntity.TaskDemo;
                                        t = null;

                                        localhost.cRemoteTaskEntity uRTask = new localhost.cRemoteTaskEntity();
                                        string[] TaskDemos = TaskDemo.Split(',');
                                        uRTask.ID = int.Parse(TaskDemos[0]);
                                        uRTask.TaskName = TaskDemos[1];
                                        uRTask.GatherTaskType = int.Parse(TaskDemos[2]);
                                        uRTask.TaskFileName = TaskDemos[3];

                                        //上传文件前先进行压缩处理
                                        Dictionary <string,int> uFiles = new Dictionary<string, int>();
                                        uFiles.Add(dFile, (int)cGlobalParas.FileType.File);

                                        string logTask = this.m_RemotePath + "log\\task" + tName + ".csv";
                                        if (File.Exists(logTask))
                                            uFiles.Add(logTask, (int)cGlobalParas.FileType.File);

                                        string dbTask = this.m_RemotePath + "urls\\远程-" + tName + ".db";
                                        if (File.Exists(dbTask))
                                            uFiles.Add(dbTask, (int)cGlobalParas.FileType.File);

                                        //获取下载图片的目录
                                        string imgPath = this.m_RemotePath + "data\\" + tName + "_file";
                                        if (Directory.Exists(imgPath))
                                            uFiles.Add(imgPath, (int)cGlobalParas.FileType.Directory);

                                        string tmpFile = this.m_RemotePath + "tmp\\" + tName + ".zip";
                                        NetMiner.Common.Tool.cZipCompression zCompress = new NetMiner.Common.Tool.cZipCompression();
                                        zCompress.CompressZIP(uFiles, tmpFile);

                                        //上传数据
                                        FileStream fs = new FileStream(tmpFile, FileMode.Open, FileAccess.Read);
                                        BinaryReader br = new BinaryReader(fs);
                                        byte[] bytes = br.ReadBytes((int)fs.Length);
                                        fs.Flush();
                                        fs.Close();

                                        int fileLength = bytes.Length;
                                        int UploadCount = fileLength / UploadFileLength;
                                        if ((fileLength % UploadFileLength) > 0)
                                            UploadCount++;
                                        int startIndex = 0;
                                        for (int i = 0; i < UploadCount; i++)
                                        {
                                            //复制字节数
                                            byte[] tmpBytes = null;
                                            int byteLen = 0;
                                            if ((startIndex + UploadFileLength) < fileLength)
                                                byteLen = UploadFileLength;
                                            else
                                                byteLen = fileLength - startIndex;

                                            tmpBytes = new byte[byteLen];
                                            Array.Copy(bytes, startIndex, tmpBytes, 0, byteLen);

                                            sweb.PushTaskResult(this.m_RegisterUser, tmpBytes, tName, startIndex);

                                            startIndex += UploadFileLength;

                                        }

                                        //数据上传结束后，马上调用远程服务，进行数据处理
                                        sweb.DealUploadZIP(this.m_RegisterUser, tName, uRTask.ID, uTaskID, uRTask.GatherTaskType);

                                        //清理本地数据
                                        //CloseTabBySilent(cCompleteTaskID[n], tName);

                                        //删除数据文件
                                        File.Delete(dFile);

                                        File.Delete(dbTask);

                                        //删除日志文件
                                        File.Delete(logTask);

                                        //删除图片文件
                                        if (Directory.Exists(imgPath))
                                            Directory.Delete(imgPath, true);

                                        File.Delete(tmpFile);


                                    }
                                    catch (System.Exception ex)
                                    {
                                        cTool.WriteSystemLog(ec.TaskName + "上传数据失败，请检查数据文件是否丢失，远程服务器是否连接正常！", EventLogEntryType.Error, "SMGatherClient");
                                    }
                                    finally
                                    {
                                        //删除队列中的数据
                                        cGatherTask gt = this.GatherControl.TaskManage.FindTask(uTaskID);
                                        if (gt != null)
                                            this.GatherControl.TaskManage.TaskListControl.CompletedTaskList.Remove(gt);
                                        gt = null;

                                        //无论是否正确，都将文件删除，否则会阻止系统继续运行

                                        //删除taskcompleter
                                        tc = new oTaskComplete(this.m_RemotePath);
                                        tc.DelTask(cCompleteTaskID[n]);
                                        tc = null;

                                        //删除remoteclass中的文件，表示已经运行
                                        oTask t = new oTask(this.m_RemotePath);
                                        t.DeleTask(this.m_RemotePath + g_RemoteTaskPath, tName);
                                        t = null;
                                    }
                                }
                            }
                            #endregion

                        }
                        else if (kv.Value == cGlobalParas.TaskState.UnStart && rCount < this.m_RemoteMaxCount)
                        {
                            try
                            {
                                if (e_Log != null)
                                    e_Log(this, new cGatherTaskLogArgs(0, "___", cGlobalParas.LogType.Info, "启动采集任务：" + kv.Key + ":" + kv.Value.ToString(), false));


                                #region 启动远程采集任务
                                //启动任务，启动任务前，先将任务的发布类型置为空
                                StartTask(kv.Key.ToString());
                                
                                #endregion
                            }
                            catch (System.Exception ex)
                            {
                                throw new Exception("启动采集任务出错：" + ex.Message);
                            }
                        }
                      

                    }

                    xmlTasks = null;


                }
                catch (System.Exception ex)
                {
                    cTool.WriteSystemLog(ex.Message, EventLogEntryType.Error, "SMGatherClient");
                }

                m_isGather = false;
            }

        }

        private void DealFaild(long TaskID,string TaskName)
        {
            //先删除队列中的数据
            cGatherTask gt = this.GatherControl.TaskManage.FindTask(TaskID);
            if (gt != null)
                this.GatherControl.Remove(gt);
            gt = null;

            //需要判断当前执行的任务是不是远程任务，如果是远程任务，则需要修改状态
            oTask t = new oTask(this.m_RemotePath);
            string FileName = this.m_RemotePath + g_RemoteTaskPath
                + "\\" + TaskName + ".smt";
            t.LoadTask(FileName);
            string TaskDemo = t.TaskEntity.TaskDemo;

            t.DeleTask(this.m_RemotePath + g_RemoteTaskPath, TaskName);

            t = null;



            //分布式采集引擎，专用于分布式采集任务的执行
            cRemoteTaskEntity rTask = new cRemoteTaskEntity();
            string[] TaskDemos = TaskDemo.Split(',');
            rTask.ID = int.Parse(TaskDemos[0]);
            rTask.TaskName = TaskDemos[1];
            rTask.GatherTaskType = int.Parse(TaskDemos[2]);
            rTask.TaskFileName = TaskDemos[3];

            //远程更新，不是本地更新
            localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            
            sweb.Url = this.m_RemoteServer + "/NetMiner.WebService.asmx";
            if (this.m_IsRemoteAuthen == true)
                sweb.Credentials = new System.Net.NetworkCredential(this.m_RemoteUser, this.m_RemotePwd);

            try
            {
                sweb.ActiveClient(this.m_RegisterUser);
            }
            catch { }

            try
            {
                bool isUpdateSucc = false;
                if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                    isUpdateSucc = sweb.UpdateTaskState(rTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.Failed, TaskID);
                else if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                    isUpdateSucc = sweb.UpdateSplitTaskState(rTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.Failed, TaskID);

                if (isUpdateSucc == false)
                {
                    cTool.WriteSystemLog("运行时更新TaskID及状态失败,采集任务：" + TaskName, EventLogEntryType.Error, "SMGatherClient");
                }
            }
            catch (System.Exception ex)
            {
                //如果再次更新出错，则表示本地执行的分布式任务在服务器中已经删除
                cTool.WriteSystemLog("分布式采集引擎出错，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherClient");
            }

            sweb = null;
        }

        public void StartTask(string TaskName)
        {
            cGatherTask gt = null;
            string tClassName = g_RemoteTaskClass;
            gt = AddRunTask(tClassName, TaskName);
            if (gt == null)
            {
                //表示启动任务被用户中断，也有可能是因为错误造成，或者采集任务文件丢失

            }
            else
            {
                Int64 TaskID = gt.TaskID;

                //启动此任务
                GatherControl.Start(gt);
                gt = null;
            }
        }

        public void StopTask(long TaskID)
        {
            cGatherTask t = null;
            t = m_GatherControl.TaskManage.FindTask(TaskID);

            if (t == null)
                return;

            //停止此任务
            m_GatherControl.Stop(t);

        }

        public void DelTask(long TaskID)
        {
            cGatherTask gt = m_GatherControl.TaskManage.FindTask(TaskID);
            DelTask(TaskID, gt.TaskName);
            m_GatherControl.Remove(gt);
        }

        //public void ResetTask(Int64 TaskID)
        //{
        //    //重置任务 将指定的任务恢复成为默认的状态
        //    cGatherTask t = m_GatherControl.TaskManage.FindTask(TaskID);
        //    t.ResetTask();

        //    t = null;

        //}

        public cGlobalParas.TaskState GetTaskState(long TaskID)
        {
            cGatherTask t = m_GatherControl.TaskManage.FindTask(TaskID);
            return t.TaskState;
        }

        public void DelTask(long TaskID,string TaskName)
        {
            //删除taskrun节点
            oTaskRun tr = new oTaskRun(this.m_EPath);
            tr.LoadTaskRunData();
            tr.DelTask(TaskID);
            tr = null;

            ////删除已经加载到采集任务控制器中的任务
            //m_GatherControl.TaskManage.TaskListControl.DelTask(t);

            //删除run中的任务实例文件
            string FileName = this.m_EPath + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //删除run中的任务实例排重库文件
            FileName = this.m_EPath + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //删除run中的采集数据排重文件
            FileName = this.m_EPath + "Tasks\\run\\" + "data" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //删除run中的任务实例文件
            FileName = this.m_EPath + "data\\" + TaskName + "-" + TaskID + ".xml";
            System.IO.File.Delete(FileName);

            tr = null;
        }

        //public void StartTask(Int64 TaskID, string TaskName, int SelectedIndex)
        //{
        //    cGatherTask t = null;

        //    //判断当前选择的树节点
          
        //    ///如果是选择的任务分类节点，点击此按钮首先先将此任务加载到运行区，然后调用
        //    ///starttask方法，启动任务。
        //    string tClassName = "";
        //    tClassName = "远程";

        //    t = AddRunTask(tClassName, TaskName);

        //    //如果是新增的任务，则传进来的TaskID是任务的编号，并不是任务执行的编号（即不是由时间自动产生的任务）
        //    //是一个递增的整数，所以，需要重新更新传入的TaskID

        //    if (t == null)
        //    {
        //        //表示启动任务被用户中断，也有可能是因为错误造成
        //        return;
        //    }

        //    TaskID = t.TaskID;

        //    if (t == null)
        //    {
        //        //表示任务启动失败
        //        return;
        //    }

        //    try
        //    {
        //        //启动此任务
        //        m_GatherControl.Start(t);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return;
        //    }

        //    t = null;
        //}

        public cGatherTask AddRunTask(string tClassName, string tName)
        {

            //将选择的任务添加到运行区
            //首先判断此任务是否已经添加到运行区,
            //如果已经添加到运行区则需要询问是否再起一个运行实例
            bool IsExist = false;
            cGlobalParas.TaskType tType = cGlobalParas.TaskType.HtmlByUrl;
            Int64 TaskID = 0;

            oTaskRun tr = new oTaskRun(this.m_EPath);
            oTaskClass tc = new oTaskClass(this.m_EPath);
            cTaskData tData = new cTaskData();

            string tPath = "";

            if (tClassName == "")
            {
                tPath = this.m_EPath + "tasks";
            }
            else
            {
                tPath = this.m_EPath + tc.GetTaskClassPathByName(tClassName);
            }

            tc = null;

            string tFileName = tName + ".smt";
            Int64 NewID;

            //获取最大的执行ID
            try
            {
                NewID = tr.InsertTaskRun(this.m_EPath, tClassName, tPath, tFileName);


                eTaskRun er= tr.LoadSingleTask(NewID);

                tData = new cTaskData();
                tData.TaskID = er.TaskID;
                tData.TaskName = er.TaskName;
                tData.TaskClass = er.TaskClass;
                tData.TaskType = er.TaskType;
                tData.RunType = er.TaskRunType;
                tData.TempDataFile = er.TempFile;
                tData.TaskState = er.TaskState;
                tData.UrlCount = er.UrlCount;
                tData.UrlNaviCount = er.UrlNaviCount;
                tData.ThreadCount = er.ThreadCount;
                tData.GatheredUrlCount = er.GatheredUrlCount;
                tData.GatherErrUrlCount = er.ErrUrlCount;
                tData.GatherDataCount = er.RowsCount;

                //添加任务到运行区
                m_GatherControl.AddGatherTask(tData);

                tData = null;

                //任务添加到运行区后,需要再添加到任务执行列表中
                tr = null;
            }
            catch (System.Exception ex)
            {
                return null;
            }

            //创建一个datatable，用于保存采集的临时数据
            cSplitRunningInfo rinfo = new cSplitRunningInfo();
            rinfo.TaskID = NewID;
            rinfo.dt.TableName = NewID.ToString ();
            rinfo.TaskName = tName;
            this.m_rInfos.Add(rinfo);

            return m_GatherControl.TaskManage.FindTask(NewID);

        }


        #region 事件处理
        private void tManage_Completed(object sender, cTaskEventArgs e)
        {

            SaveTempData(e.TaskID, e.TaskName);

            UpdateTaskComplete(e.TaskID,cGlobalParas.GatherResult.GatherSucceed);

        }

        private void tManage_TaskStart(object sender, cTaskEventArgs e)
        {
            //需要判断当前执行的任务是不是远程任务，如果是远程任务，则需要修改状态
            oTask t = new oTask(this.m_EPath);
            t.LoadTask(e.TaskID);
            //string tClass = t.TaskEntity.TaskClass;
            string TaskDemo = t.TaskEntity.TaskDemo;
            t = null;
            
            //分布式采集引擎，专用于分布式采集任务的执行
            
            cRemoteTaskEntity rTask = new cRemoteTaskEntity();
            string[] TaskDemos = TaskDemo.Split(',');
            rTask.ID = int.Parse(TaskDemos[0]);
            rTask.TaskName = TaskDemos[1];
            rTask.GatherTaskType = int.Parse(TaskDemos[2]);
            rTask.TaskFileName = TaskDemos[3];

            //远程更新，不是本地更新
            localhost.NetMinerWebService sweb = new localhost.NetMinerWebService();
            sweb.Url = this.m_RemoteServer + "/NetMiner.WebService.asmx";
            if (this.m_IsRemoteAuthen == true)
                sweb.Credentials = new System.Net.NetworkCredential(this.m_RemoteUser, this.m_RemotePwd);

            try
            {
                sweb.ActiveClient(this.m_RegisterUser);
            }
            catch { }

            try
            {
                bool isUpdateSucc=false ;
                if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
                    isUpdateSucc=sweb.UpdateTaskState(rTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteRunning, e.TaskID);
                else if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
                    isUpdateSucc=sweb.UpdateSplitTaskState(rTask.ID, this.m_RegisterUser, (int)cGlobalParas.TaskState.RemoteRunning, e.TaskID);

                if (isUpdateSucc==false )
                {
                    cTool.WriteSystemLog("运行时更新TaskID及状态失败,采集任务：" + e.TaskName, EventLogEntryType.Error, "SMGatherClient");
                }
            }
            catch (System.Exception ex)
            {
                //如果再次更新出错，则表示本地执行的分布式任务在服务器中已经删除
                cTool.WriteSystemLog("分布式采集引擎出错，错误信息：" + ex.Message, EventLogEntryType.Error, "SMGatherClient");
            }
           
            sweb = null;

            
            //if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriTask)
               
            //    c.UpdateTaskID(rTask.ID, e.TaskID,this.m_RegisterUser);
            //else if (rTask.GatherTaskType == (int)cGlobalParas.GatherTaskType.DistriSplitTask)
            //    c.UpdateSplitTaskID(rTask.ID, e.TaskID);
            
        }

        private void tManage_TaskInitialized(object sender, TaskInitializedEventArgs e)
        {
            

        }

        private void tManage_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            
        }

        private void tManage_TaskStop(object sender, cTaskEventArgs e)
        {
            SaveTempData(e.TaskID, e.TaskName);

        }

        //单个Url采集发生错误，不进行界面响应，记录日志即可，日志由其他事件记录完成
        //当错误达到一定的数量后，会由后台线程触发任务失败的事件，由任务失败事件完成
        //临时数据的存储
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {
           

        }

        private void tManage_TaskFailed(object sender, cTaskEventArgs e)
        {
            SaveTempData(e.TaskID, e.TaskName);

            UpdateTaskComplete(e.TaskID,cGlobalParas.GatherResult.GatherFailed);
        }

        private void tManage_TaskAbort(object sender, cTaskEventArgs e)
        {

           

        }

        private void m_Gather_Completed(object sender, EventArgs e)
        {
            //任务采集完成，则启动消息通知窗体，通知用户


        }

        //写日志事件
        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            for (int i = 0; i < this.m_rInfos.Count; i++)
            {
                if (m_rInfos[i].TaskID == e.TaskID)
                    m_rInfos[i].Logs = System.DateTime.Now.ToString () + " " + e.strLog;
            }

            //if (e_Log != null)
            //    e_Log(this, new cGatherTaskLogArgs(e.TaskID, e.TaskName, e.LogType, e.strLog, false));
        }

        //写数据事件
        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {
            for (int i = 0; i < this.m_rInfos.Count; i++)
            {
                if (m_rInfos[i].TaskID == e.TaskID)
                    m_rInfos[i].dt=e.gData;
            }
        }

        private void tManage_GUrlCount(object sender, cGatherUrlCounterArgs e)
        {

        }

        #endregion

        private readonly object m_fileLock = new object();

        private string SaveTempData(Int64 TaskID,string TaskName)
        {
            string FileName = string.Empty;

            try
            {
              
                    FileName = this.m_EPath + "\\data\\" + TaskName + "-" + TaskID + ".xml"; ;

                    for (int i = 0; i < this.m_rInfos.Count; i++)
                    {
                        if (this.m_rInfos[i].TaskID == TaskID)
                        {
                            m_rInfos[i].dt.WriteXml(FileName, XmlWriteMode.WriteSchema);
                         
                            //删除此集合
                            m_rInfos.Remove(m_rInfos[i]);
                            break;

                        }
                    }
            }
            catch (System.Exception ex)
            {
                return TaskID.ToString() + "-" + ex.Message;
            }
            finally
            {

            }
            return TaskID.ToString();
        }

        public void UpdateTaskComplete(long TaskID,cGlobalParas.GatherResult tState)
        {
            //判断任务是否为增量任务，如果是增量任务，则不能删除taskrun中的信息
            //因为增量任务永远不会过时
            oTaskRun tr = new oTaskRun(this.m_EPath);
            eTaskRun er= tr.LoadSingleTask(TaskID);

            eTaskCompleted ec = new eTaskCompleted();

            ec.TaskID = er.TaskID;
            ec.TaskName = er.TaskName;
            ec.GatherResult = tState;
            ec.TaskType = er.TaskType;
            ec.TaskRunType = er.TaskRunType;
            ec.ExportFile = er.ExportFile;
            ec.TempFile = er.TempFile;
            ec.UrlCount = er.UrlCount;
            ec.GatheredUrlCount = er.GatheredUrlCount;
            ec.PublishType = er.PublishType;
            ec.CompleteDate = System.DateTime.Now;

            //将已经完成的任务添加到完成任务的索引文件中
            oTaskComplete t = new oTaskComplete(this.m_EPath);
            t.InsertTaskComplete(ec, cGlobalParas.GatherResult.GatherSucceed);
            t = null;

            //删除taskrun节点

            tr.LoadTaskRunData();
            tr.DelTask(TaskID);

            //删除run中的任务实例文件
            string FileName = this.m_EPath + "Tasks\\run\\" + "Task" + TaskID + ".rst";
            System.IO.File.Delete(FileName);

            //删除run中的任务实例排重库文件
            FileName = this.m_EPath + "Tasks\\run\\" + "Task" + TaskID + ".db";
            System.IO.File.Delete(FileName);

            //删除run中的采集数据排重文件
            FileName = this.m_EPath + "Tasks\\run\\" + "data" + TaskID + ".db";
            System.IO.File.Delete(FileName);
        }

        /// <summary>
        /// 采集日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        public event EventHandler<cGatherTaskLogArgs> Log
        {
            add { e_Log += value; }
            remove { e_Log -= value; }
        }

        private void ClearRemoteTask()
        {
            oTaskIndex xmlTasks = new oTaskIndex(this.m_RemotePath);
            //这是一个特殊的默认分类，特指从服务器下载的采集任务
            IEnumerable<NetMiner.Core.gTask.Entity.eTaskIndex> eIndexs= xmlTasks.GetTaskDataByClass(NetMiner.Constant.g_RemoteTaskClass);

            foreach(NetMiner.Core.gTask.Entity.eTaskIndex ei in eIndexs)
            {
                //开始逐一删除任务
                oTask t = new oTask(this.m_RemotePath);

                oTaskClass tc = new oTaskClass(this.m_RemotePath);
                string tPath = this.m_EPath + tc.GetTaskClassPathByName( g_RemoteTaskClass);
                tc = null;
                t.DeleTask(tPath, ei.TaskName);
                t = null;

                //判断此任务在正在运行的队列中是否存在，如果存在，停止，删除
            }
            xmlTasks = null;


        }
    }
}
