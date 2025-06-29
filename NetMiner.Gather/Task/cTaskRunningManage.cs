using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using System.Xml;
using System.Xml.Linq;
using NetMiner.Resource;
using System.IO;
using NetMiner.Common.Tool;

/// <summary>
/// 采集任务运行管理
/// </summary>
namespace NetMiner.Gather.Task
{
    public class cTaskRunningManage
    {
        private string m_workPath;
        public cTaskRunningManage (string workPath)
        {
            m_workPath = workPath;
        }

        ~cTaskRunningManage()
        {

        }

        /// <summary>
        /// 向正在运行队列中插入一个任务
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="Path"></param>
        /// <param name="File"></param>
        /// <returns></returns>
        //public long InsertTaskRun(string workPath, string Path, string File)
        //{
        //    ///首先判断存放任务执行的目录是否存在
        //    ///此目录是固定目录，存放在系统\\Task\\run
        //    string RunPath = workPath + NetMiner.Constant.g_TaskRunPath;
        //    long TaskID = DateTime.Now.ToFileTime();

        //    try
        //    {

        //        ///先将此任务的摘要信息加载到TaskRun.xml文件中
        //        oTask t = new oTask(m_workPath);
        //        t.LoadTask(Path + "\\" + File);
                

        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("<TaskID>" + TaskID + "</TaskID>");
        //        sb.Append("<TaskName>" + t.TaskEntity.TaskName + "</TaskName>");
        //        sb.Append("<TaskPath>" + Path + "</TaskPath>");
        //        //插入进来数据就是runniing
        //        sb.Append("<TaskState>" + (int)cGlobalParas.TaskState.UnStart + "</TaskState>");
        //        sb.Append("<TaskType>" + t.TaskEntity.TaskType + "</TaskType>");
        //        sb.Append("<RunType>" + t.TaskEntity.RunType + "</RunType>");
        //        sb.Append("<ExportFile>" + t.TaskEntity.ExportFile + "</ExportFile>");
        //        if (t.TaskEntity.IsSaveSingleFile)
        //        {
        //            sb.Append("<tempFile>" + t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile + "</tempFile>");
        //        }
        //        else
        //        {
        //            sb.Append("<tempFile>" + t.TaskEntity.SavePath + "\\" + t.TaskEntity.TaskName + "-" + TaskID + ".xml" + "</tempFile>");
        //        }
        //        sb.Append("<StartDate>" + DateTime.Now + "</StartDate>");
        //        sb.Append("<EndDate></EndDate>");
        //        sb.Append("<ThreadCount>" + t.TaskEntity.ThreadCount + "</ThreadCount>");
        //        sb.Append("<UrlCount>" + t.TaskEntity.UrlCount + "</UrlCount>");

        //        ///TrueUrlCount表示如果采集的网址中存在导航网址，则需要采集的网址是无法根据公式极端出来的
        //        ///需要采集任务不断执行，不断根据采集的规则进行计算采集网址的总数，所以需要再次记录此值
        //        ///记录此值的目的是为了可以更好的跟踪采集的进度，但Urlcount不能修改，因为此值要进行任务分解
        //        ///使用，如果改变了UrlCount则可能导致任务分解失败，在运营任务初始化的时候，此值同UrlCount，此值的
        //        ///更改在任务运营时维护
        //        sb.Append("<UrlNaviCount>" + t.TaskEntity.UrlCount + "</UrlNaviCount>");

        //        sb.Append("<GatheredUrlCount>0</GatheredUrlCount>");
        //        sb.Append("<GatheredUrlNaviCount>0</GatheredUrlNaviCount>");
        //        sb.Append("<ErrUrlCount>0</ErrUrlCount>");
        //        sb.Append("<ErrUrlNaviCount>0</ErrUrlNaviCount>");
        //        sb.Append("<RowsCount>0</RowsCount>");
        //        sb.Append("<PublishType>" + t.TaskEntity.ExportType + "</PublishType>");

        //        t.Dispose();
        //        t = null;

        //        XElement xe = XElement.Load(new StringReader("Task" + sb.ToString() + "</task>"));

        //        oTaskRun tr = new oTaskRun(this.m_workPath);
        //        tr.InsertTaskRun(xe);
        //        tr.Dispose();
        //        tr = null;

        //        ///运行区的任务xml文件的格式与Task任务格式完全一眼个，但命名方式完全不同
        //        ///命名格式是按照Task＋当前文件在Taskrun中的id来命名，这样做的目的是支持同一个任务
        //        ///可以建立多个运行实例，也就是当这个任务运行的时候，用户也可以修改此任务后建立另
        //        ///一个实例开始运行。
        //        System.IO.File.Copy(Path + "\\" + File, RunPath + "\\" + "Task" + TaskID + ".rst", true);

        //        //文件拷贝过去后，需要修改文件中的TaskID，以吻合TaskRun中的TaskID索引，否则
        //        //在加载文件的时候会出错,系统用ID来做唯一索引
        //        string strTempXML = cFile.ReadFileBinary(RunPath + "\\" + "Task" + TaskID + ".rst");

        //        t = new oTask(this.m_workPath);
        //        t.LoadTask(TaskID);
        //        t.EditTaskID(TaskID);
        //        t.Dispose();

        //        t = null;

        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }

        //    return TaskID;

        //}

        public long InsertTaskComplete(long TaskID, cGlobalParas.GatherResult tSate)
        {
            ///首先判断存放任务执行的目录是否存在
            ///此目录是固定目录，存放在系统\\data

            ///先将此任务的摘要信息加载到index.xml文件中
            oTaskRun tr = new oTaskRun(m_workPath);
            eTaskRun er= tr.LoadSingleTask(TaskID);

            eTaskCompleted ec = new eTaskCompleted();

            ec.TaskID = er.TaskID;
            ec.TaskName = er.TaskName;
            ec.GatherResult = tSate;
            ec.TaskType = er.TaskType;
            ec.TaskRunType = er.TaskRunType;
            ec.ExportFile = er.ExportFile;
            ec.TempFile = er.TempFile;
            ec.UrlCount = er.UrlCount;
            ec.GatheredUrlCount = er.GatheredUrlCount;
            ec.PublishType = er.PublishType;
            ec.CompleteDate = System.DateTime.Now;

            tr.Dispose();
            tr = null;

            oTaskComplete tc = new oTaskComplete(this.m_workPath);
            tc.InsertTaskComplete(ec, tSate);
            tc.Dispose();
            tc = null;

            return TaskID;

        }
    }
}
