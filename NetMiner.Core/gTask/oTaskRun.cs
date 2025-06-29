using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Common.Tool;
using System.Threading;
using NetMiner;
using NetMiner.Core.gTask;
using NetMiner.Base;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using NetMiner.Core.gTask.Entity;

///功能：运行任务索引文件管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
///2013年11月1日进行taskrun的代码修改，增加了对运行任务状态
///的保存，旧版本仅支持采集，发布为一个不可干预的事务，现在
///增加了对采集及发布的状态管理，发布操作可以随时中断，并记录
///已经发布和失败发布的记录。
namespace NetMiner.Core.gTask
{

    ///在应用程序根Tasks下，存放TaskRunn.xml，记录当前正在运行的任务，启动启动时，需要加载此文件中的任务信息
    ///此类主要维护处理任务执行的信息，每个任务启动后，都必须写此文件来进行控制，这样做的目的是任务可以脱离界面
    ///进行执行，并且维护自己的状态，主要用于后期扩展的方便
    ///taskrun.xml是一个多个线程访问的文件，所以在操作之前需要区判断其他线程对其的操作类型
    ///以避免出现信息共享失败的情况，所以，对此类操作如果完成应该尽快的释放此类的资源。
    public class oTaskRun:XmlUnity
    {
        DataView Tasks;
        private string m_workPath = string.Empty;
        private List<eTaskRun> m_TaskRunList;

        /// <summary>
        /// 初始化即加载TaskRun数据
        /// </summary>
        /// <param name="workPath"></param>
        public oTaskRun(string workPath)
        {
            m_workPath = workPath;

            string fileName = m_workPath + NetMiner.Constant.g_TaskRunFile;
            if (!File.Exists(fileName))
                NewTaskRunFile();

            int AgainNumber = 0;

        Again:
            try
            {
                base.LoadXML(m_workPath + NetMiner.Constant.g_TaskRunFile);
            }
            catch (System.IO.IOException ex)
            {
                //表示有可能被占用，重试三次
                AgainNumber++;

                if (AgainNumber > 3)
                {
                    throw ex;
                }
                else
                {
                    Thread.Sleep(100);
                    goto Again;
                
                }
            }
        }

        ~oTaskRun()
        {
            base.Dispose();
        }

        public List<eTaskRun> TaskRunList
        {
            get { return m_TaskRunList; }
            set { m_TaskRunList = value; }
        }

        //返回的是运行区任务的集合信息
        public IEnumerable<eTaskRun> LoadTaskRunData()
        {
           
            IEnumerable<XElement> xes = base.GetAllElement("Task");
            IEnumerable<eTaskRun> eTasks = xes.Select<XElement, eTaskRun>(
                    s => Convert(s));
            return eTasks;
        }

        private eTaskRun Convert(XElement s)
        {
            eTaskRun ec = new eTaskRun();
            ec.TaskID = long.Parse(s.Element("TaskID").Value.ToString());
            ec.TaskName = s.Element("TaskName").Value.ToString();
            ec.TaskClass = s.Element("TaskClass").Value.ToString();
            ec.TaskClassPath= s.Element("TaskPath").Value.ToString();
            ec.TaskState = (cGlobalParas.TaskState)int.Parse(s.Element("TaskState").Value.ToString());
            ec.TaskType = (cGlobalParas.TaskType)int.Parse(s.Element("TaskType").Value.ToString());
            ec.TaskRunType = (cGlobalParas.TaskRunType)int.Parse(s.Element("RunType").Value.ToString());
            ec.ExportFile = s.Element("ExportFile").Value.ToString();
            ec.TempFile = s.Element("tempFile").Value.ToString();
            ec.UrlCount = int.Parse(s.Element("UrlCount").Value.ToString());
            ec.PublishType = (cGlobalParas.PublishType)int.Parse(s.Element("PublishType").Value.ToString());
            ec.GatheredUrlCount = int.Parse(s.Element("GatheredUrlCount").Value.ToString());
            ec.StartDateTime = DateTime.Parse(s.Element("StartDateTime").Value.ToString());

            DateTime endDT = DateTime.Parse("1970-1-1");
            if ( DateTime.TryParse(s.Element("EndDateTime").Value.ToString(),out endDT))
                ec.EndDateTime = endDT;

            ec.TaskClass = s.Element("TaskClass").Value.ToString();
            ec.TaskClassPath= s.Element("TaskPath").Value.ToString();
            ec.RowsCount = int.Parse(s.Element("RowsCount").Value.ToString());
            ec.UrlNaviCount = int.Parse(s.Element("UrlNaviCount").Value.ToString());
            ec.GatheredUrlCount= int.Parse(s.Element("GatheredUrlCount").Value.ToString());
            ec.GatheredUrlNaviCount = int.Parse(s.Element("GatheredUrlNaviCount").Value.ToString());
            ec.ErrUrlCount = int.Parse(s.Element("ErrUrlCount").Value.ToString());
            ec.ErrUrlNaviCount = int.Parse(s.Element("ErrUrlNaviCount").Value.ToString());
            ec.ThreadCount= int.Parse(s.Element("ThreadCount").Value.ToString());
            ec.Process=(cGlobalParas.TaskProcess) int.Parse(s.Element("TaskProcess").Value.ToString());

            return ec;
        }

        public void ResetTaskRun(long TaskID,int urlCount)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
          

            base.EditValue(xe.Element("TaskState"), ((int)cGlobalParas.TaskState.UnStart).ToString());
            base.EditValue(xe.Element("UrlNaviCount"), urlCount.ToString());
            base.EditValue(xe.Element("GatheredUrlCount"), "0");
            base.EditValue(xe.Element("GatheredUrlNaviCount"), "0");
            base.EditValue(xe.Element("ErrUrlCount"), "0");
            base.EditValue(xe.Element("ErrUrlNaviCount"), "0");
            base.EditValue(xe.Element("TaskProcess"), ((int)cGlobalParas.TaskProcess.Gather).ToString());
            base.Save();
        }

        public void InsertTaskRun(XElement xe)
        {
            base.LoadXML(m_workPath + NetMiner.Constant.g_TaskRunFile);
            base.AddElement(base.xDoc.Root, xe);
            base.Save();
        }

        //根据执行的任务ID加载任务信息
        public eTaskRun LoadSingleTask(Int64 TaskID)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            return Convert(xe);
        }

        /// <summary>
        /// 判断此任务在运行队列中是否存在，注意需要跟目录，从V5.5开始允许任务名称重复
        /// </summary>
        /// <param name="TaskName">采集任务名称</param>
        /// <returns></returns>
        public bool isExist(string tClassName, string TaskName)
        {
            IEnumerable<XElement> xes= base.SearchAllElement("Task", "TaskClass", tClassName);

            bool isE = false;
            foreach(XElement xe in xes)
            {
                if (xe.Element("TaskName").Value==TaskName)
                {
                    isE = true;
                    break;
                }
            }

            return isE;
            //return base.isExist("Task", "TaskName", TaskName);
        }


        /// <summary>
        /// 统一更新采集任务运行的状态
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="Count"></param>
        /// <param name="tSate"></param>
        public void EditTaskState(Int64 TaskID,cGlobalParas.TaskState tSate)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            base.EditValue(xe.Element("TaskState"), ((int)tSate).ToString());
            base.Save();
        }

        public void EditTaskState(Int64 TaskID, int Count, cGlobalParas.TaskState tSate)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());

            if (xe==null)
            {
                //说明这个运行实例已经被删除了，理论上不会这样，但这类情况依然存在，多线程造成的问题
                return;
            }

            base.EditValue(xe.Element("TaskState"), ((int)tSate).ToString());
            base.EditValue(xe.Element("RowsCount"), ((int)Count).ToString());
            base.Save();
        }

        public void EditTaskProcess(int TaskID,cGlobalParas.TaskProcess tProcess)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            base.EditValue(xe.Element("TaskProcess"), ((int)tProcess).ToString());
            base.Save();
        }

        #region 根据制定的索引号获取运行任务的信息
        public int GetCount()
        {
            return base.GetNodesCount("Task");
        }

        ///这里存在问题*****************************************************************************************
        ///首先采集任务下载完成后，数据并没有清除，而是转移到了完成队列
        ///意味这内存中还存在此数据，用于重置等。但taskrun中确已经删除了
        ///此数据，这就已经不同步了，所以，如果再添加任务，就会导致数据
        ///发生冲突，任务无法执行，所以maxid需要按照内存中的数据进行增加
        ///同时，由于inserttaskrun和deltask有可能会由多个线程操作，也可能
        ///造成不同步，这个问题也需要解决。当前getnewid是采用了时间编号进行的，这样做是避免编号重复的问题。
        ///***********************************************************************

        //public cGlobalParas.TaskState GetTaskState(string workPath,int index)
        //{
        //    cGlobalParas.TaskState TaskState;

        //    try
        //    {
        //        //在此判断一下这个运行文件的物理文件是否存在
        //        //如果不存在，则进行失败状态的说明
        //        string fName = workPath + "tasks\\run\\task" + this.GetTaskID(index) + ".rst";
        //        if (File.Exists(fName))
        //        {
        //            TaskState = (cGlobalParas.TaskState)int.Parse(Tasks[index].Row["TaskState"].ToString());
        //        }
        //        else
        //        {
        //            TaskState = cGlobalParas.TaskState.Failed;
        //        }
        //    }
        //    catch
        //    {
        //        TaskState = cGlobalParas.TaskState.UnStart;
        //    }

        //    return TaskState;
        //}

        

        #endregion

        public Int64 GetNewID()
        {
            Int64 id=Int64.Parse ( DateTime.Now.ToFileTime().ToString ());
            return id;
        }

        private void NewTaskRunFile()
        {
            XElement xe = new XElement("Tasks");
            base.NewXML(m_workPath + NetMiner.Constant.g_TaskRunFile ,xe);
        }

        private readonly Object m_taskFileLock = new Object();

        /// <summary>
        /// 采集运行时，插入TaskRun的操作方法
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="tClass">传入的是完整路径</param>
        /// <param name="Path">传入的是绝对地址</param>
        /// <param name="File"></param>
        /// <returns></returns>
        public Int64 InsertTaskRun(string workPath, string tClass, string Path, string File)
        {
            ///首先判断存放任务执行的目录是否存在
            ///此目录是固定目录，存放在系统\\Task\\run
            

            string RunPath = workPath + NetMiner.Constant.g_TaskRunPath;
            Int64 maxID;
            try
            {

                ///先将此任务的摘要信息加载到TaskRun.xml文件中
                oTask t = new oTask(m_workPath);
                t.LoadTask(Path + "\\" + File);

                //开始构造xml节点内容
                LoadTaskRunData();
                maxID = GetNewID();

                StringBuilder sb = new StringBuilder();

                sb.Append("<TaskID>" + maxID + "</TaskID>");
                sb.Append("<TaskClass>" + tClass + "</TaskClass>");
                sb.Append("<TaskName>" + t.TaskEntity.TaskName + "</TaskName>");
                sb.Append( "<TaskPath>" + Path + "</TaskPath>");
                //插入进来数据就是runniing
                sb.Append( "<TaskState>" + (int)cGlobalParas.TaskState.UnStart + "</TaskState>");
                sb.Append("<TaskType>" + (int)t.TaskEntity.TaskType + "</TaskType>");
                sb.Append("<TaskProcess>" + (int)cGlobalParas.TaskProcess.Gather + "</TaskProcess>");
                sb.Append( "<RunType>" + (int)t.TaskEntity.RunType + "</RunType>");
                sb.Append( "<ExportFile>" + t.TaskEntity.ExportFile + "</ExportFile>");
                if (t.TaskEntity.IsSaveSingleFile)
                {
                    sb.Append( "<tempFile>" + t.TaskEntity.TempDataFile + "</tempFile>");
                }
                else
                {
                    sb.Append( "<tempFile>" + this.m_workPath + NetMiner.Constant.g_TaskDataPath + "\\" + t.TaskEntity.TaskName + "-" + maxID + ".xml" + "</tempFile>");
                }
                sb.Append("<StartDateTime>" + DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss") + "</StartDateTime>");
                sb.Append("<EndDateTime></EndDateTime>");
                sb.Append( "<ThreadCount>" + t.TaskEntity.ThreadCount + "</ThreadCount>");
                sb.Append( "<UrlCount>" + t.TaskEntity.UrlCount + "</UrlCount>");

                ///TrueUrlCount表示如果采集的网址中存在导航网址，则需要采集的网址是无法根据公式极端出来的
                ///需要采集任务不断执行，不断根据采集的规则进行计算采集网址的总数，所以需要再次记录此值
                ///记录此值的目的是为了可以更好的跟踪采集的进度，但Urlcount不能修改，因为此值要进行任务分解
                ///使用，如果改变了UrlCount则可能导致任务分解失败，在运营任务初始化的时候，此值同UrlCount，此值的
                ///更改在任务运营时维护
                sb.Append("<UrlNaviCount>" + t.TaskEntity.UrlCount + "</UrlNaviCount>");

                sb.Append( "<GatheredUrlCount>0</GatheredUrlCount>");
                sb.Append( "<GatheredUrlNaviCount>0</GatheredUrlNaviCount>");
                sb.Append( "<ErrUrlCount>0</ErrUrlCount>");
                sb.Append( "<ErrUrlNaviCount>0</ErrUrlNaviCount>");
                sb.Append( "<RowsCount>0</RowsCount>");
                sb.Append( "<PublishType>" + (int)t.TaskEntity.ExportType + "</PublishType>");

                XElement xe = XElement.Load(new StringReader("<Task>" + sb.ToString() + "</Task>"));

                base.AddElement(base.xDoc.Root, xe);

                base.Save();

                ///运行区的任务xml文件的格式与Task任务格式完全一眼个，但命名方式完全不同
                ///命名格式是按照Task＋当前文件在Taskrun中的id来命名，这样做的目的是支持同一个任务
                ///可以建立多个运行实例，也就是当这个任务运行的时候，用户也可以修改此任务后建立另
                ///一个实例开始运行。
                System.IO.File.Copy(Path + "\\" + File, RunPath + "\\" + "Task" + maxID + ".rst", true);

                //文件拷贝过去后，需要修改文件中的TaskID，以吻合TaskRun中的TaskID索引，否则
                //在加载文件的时候会出错,系统用ID来做唯一索引
                string strTempXML = cFile.ReadFileBinary(RunPath + "\\" + "Task" + maxID + ".rst");
                cXmlIO xmlFile = new cXmlIO();
                xmlFile.LoadXML(strTempXML);
                string tID = xmlFile.GetNodeValue("Task/BaseInfo/ID");
                xmlFile.EditNode("ID", tID, maxID.ToString());
                cFile.SaveFileBinary(RunPath + "\\" + "Task" + maxID + ".rst", xmlFile.InnerXml, true);
                xmlFile = null;

                t = null;

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return maxID ;

        }

        public static bool g_RunSaving=false ;
        private void Save(cXmlIO xmlDoc)
        {
            if (g_RunSaving == false)
            {
                g_RunSaving = true;
                xmlDoc.Save();
                g_RunSaving = false;
            }
            else
            {
                Thread.Sleep(50);
                Save(xmlDoc);
            }
        }

        public void DelTask(Int64 TaskID)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            base.RemoveElement(xe);
            base.Save();

        }

    }
}
