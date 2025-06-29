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
using NetMiner.Core.Task;

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
namespace NetMiner.Gather.Task
{

    ///在应用程序根Tasks下，存放TaskRunn.xml，记录当前正在运行的任务，启动启动时，需要加载此文件中的任务信息
    ///此类主要维护处理任务执行的信息，每个任务启动后，都必须写此文件来进行控制，这样做的目的是任务可以脱离界面
    ///进行执行，并且维护自己的状态，主要用于后期扩展的方便
    ///taskrun.xml是一个多个线程访问的文件，所以在操作之前需要区判断其他线程对其的操作类型
    ///以避免出现信息共享失败的情况，所以，对此类操作如果完成应该尽快的释放此类的资源。
    public class cTaskRun
    {
        cXmlIO xmlConfig;
        DataView Tasks;
        private string m_workPath = string.Empty;

        public cTaskRun(string workPath)
        {
            m_workPath = workPath;
        }

        ~cTaskRun()
        {
        }

        //返回的是运行区任务的集合信息
        public void LoadTaskRunData()
        {
            try
            {
                xmlConfig = new cXmlIO(m_workPath + "Tasks\\TaskRun.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //获取TaskClass节点
            Tasks = xmlConfig.GetData("Tasks");
        }

        //public void LoadTaskRunData()
        //{
        //    try
        //    {
        //        xmlConfig = new cXmlIO(m_workPath  + "Tasks\\TaskRun.xml");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }

        //    //获取TaskClass节点
        //    Tasks = xmlConfig.GetData("Tasks");
        //}

        //调用加载任务信息后,调用此方法返回一个任务的dataview
        public DataView GetTasks()
        {
            return Tasks;
        }

        //根据执行的任务ID加载任务信息
        public void LoadSingleTask(Int64 TaskID)
        {
            try
            {
                xmlConfig = new cXmlIO(m_workPath + "Tasks\\TaskRun.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            //获取TaskClass节点,尽管返回的也是DataView,但其中只包含一条记录
            //这样做是为了更好的兼容访问操作
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }

        /// <summary>
        /// 统一更新采集任务运行的状态
        /// </summary>
        /// <param name="TaskID"></param>
        /// <param name="Count"></param>
        /// <param name="tSate"></param>
        public void EditTaskState(Int64 TaskID,cGlobalParas.TaskState tSate)
        {
            try
            {
                xmlConfig = new cXmlIO(m_workPath + "Tasks\\TaskRun.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
         
            Tasks = xmlConfig.GetData("Tasks", "TaskID", TaskID.ToString());
            bool isModify= xmlConfig.EditNodeValue("Tasks", "TaskID", TaskID.ToString(), "TaskState", ((int)tSate).ToString());
            //xmlConfig.Save();
            if (isModify==true )
                Save(xmlConfig);
            xmlConfig = null;

        }

        public void EditTaskState(Int64 TaskID, int Count, cGlobalParas.TaskState tSate)
        {
            try
            {
                xmlConfig = new cXmlIO(m_workPath + "Tasks\\TaskRun.xml");
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            Tasks = xmlConfig.GetData("Tasks", "TaskID", TaskID.ToString());
            xmlConfig.EditNodeValue("Tasks", "TaskID", TaskID.ToString(), "TaskState", ((int)tSate).ToString());
            bool isModify = xmlConfig.EditNodeValue("Tasks", "TaskID", TaskID.ToString(), "RowsCount", Count.ToString());
            //xmlConfig.Save()
            if (isModify ==true )
                Save(xmlConfig);
            xmlConfig = null;

        }

        //计算当前共有多少个任务处于运行区
        public int GetTaskClassCount()
        {
            int tCount;

            if (Tasks == null)
            {
                tCount = 0;
            }
            else
            {
                tCount = Tasks.Count;
            }
            return tCount;
        }

        #region 根据制定的索引号获取运行任务的信息

        public Int64 GetTaskID(int index)
        {

            Int64 tid = 0;
            try
            {
                tid = Int64.Parse(Tasks[index].Row["TaskID"].ToString());
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            return tid;
        }

        public string GetTaskName(int index)
        {
            string TName = Tasks[index].Row["TaskName"].ToString();
            return TName;
        }

        public string GetTaskClass(int index)
        {
            string TName = Tasks[index].Row["TaskClass"].ToString();
            return TName;
        }

        //此运行任务的实际文件存储路径
        public string GetTaskPath(int index)
        {
            string TPath = Tasks[index].Row["TaskPath"].ToString();
            return TPath;
        }

        public cGlobalParas.TaskType GetTaskType(int index)
        {
            cGlobalParas.TaskType tType = (cGlobalParas.TaskType)int.Parse(Tasks[index].Row["TaskType"].ToString());
            return tType;
        }

        public cGlobalParas.TaskRunType GetTaskRunType(int index)
        {
            cGlobalParas.TaskRunType TRunType =(cGlobalParas.TaskRunType)int.Parse (Tasks[index].Row["RunType"].ToString());
            return TRunType;
        }

        public string GetTempFile(int index)
        {
            string tempFile = Tasks[index].Row["tempFile"].ToString();
            return tempFile;
        }

        public string GetExportFile(int index)
        {
            string ExportFile = Tasks[index].Row["ExportFile"].ToString();
            return ExportFile;
        }

        //public bool GetIsLogin(int index)
        //{
        //    bool Isl = (Tasks[index].Row["IsLogin"].ToString() == "True" ? true : false);
        //    return Isl;
        //}

        public cGlobalParas.PublishType GetPublishType (int index)
        {
            cGlobalParas.PublishType pType = (cGlobalParas.PublishType)int.Parse(Tasks[index].Row["PublishType"].ToString());
            return pType;
        }

        //返回此任务需要采集的网页地址数量
        public int GetUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["UrlCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["UrlCount"].ToString() == "")
                {
                    WebLinkCount = 0;
                }
                else
                {
                    throw ex;
                }
            }
            return WebLinkCount;
        }

        public int GetRowsCount(int index)
        {
            int RowsCount;
            try
            {
                RowsCount = int.Parse(Tasks[index].Row["RowsCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["RowsCount"].ToString() == "")
                {
                    RowsCount = 0;
                }
                else
                {
                    throw ex;
                }
            }
            return RowsCount;
        }

        public DateTime GetStartTimer(int index)
        {
            string sDate = Tasks[index].Row["StartDate"].ToString();
            try
            {
                return DateTime.Parse(sDate);
            }
            catch
            {
                return System.DateTime.Now;
            }
        }

        //返回此任务导航出来的网址数量
        public int GetUrlNaviCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["UrlNaviCount"].ToString());
            }
            catch (System.Exception ex)
            {
                if (Tasks[index].Row["TrueUrlCount"].ToString() == "")
                {
                    WebLinkCount = 0;
                }
                else
                {
                    throw ex;
                }
            }
            return WebLinkCount;
        }

        //返回此任务已经采集网页的地址数量
        public int GetGatheredUrlCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["GatheredUrlCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }

        public int GetGatheredUrlNaviCount(int index)
        {
            int WebLinkCount;
            try
            {
                WebLinkCount = int.Parse(Tasks[index].Row["GatheredUrlNaviCount"].ToString());
            }
            catch
            {
                WebLinkCount = 0;
            }
            return WebLinkCount;
        }

        public int GetErrUrlCount(int index)
        {
            int ErrUrlCount;
            try
            {
                ErrUrlCount = int.Parse(Tasks[index].Row["ErrUrlCount"].ToString());
            }
            catch
            {
                ErrUrlCount = 0;
            }
            return ErrUrlCount;
        }

        public int GetErrUrlNaviCount(int index)
        {
            int ErrUrlCount;
            try
            {
                ErrUrlCount = int.Parse(Tasks[index].Row["ErrUrlNaviCount"].ToString());
            }
            catch
            {
                ErrUrlCount = 0;
            }
            return ErrUrlCount;
        }

        public int GetThreadCount(int index)
        {
            int ThreadCount;
            if (Tasks[index].Row["ThreadCount"].ToString() == null || Tasks[index].Row["ThreadCount"].ToString() == "")
            {
                ThreadCount = 0;
            }
            else
            {
                ThreadCount = int.Parse(Tasks[index].Row["ThreadCount"].ToString());
            }
            return ThreadCount;

        }

        public int GetCount()
        {
            int RunCount;

            try
            {
                RunCount = Tasks.Count;
            }
            catch
            {
                RunCount = 0;
            }
            return RunCount;
        }

        ///这里存在问题*****************************************************************************************
        ///首先采集任务下载完成后，数据并没有清除，而是转移到了完成队列
        ///意味这内存中还存在此数据，用于重置等。但taskrun中确已经删除了
        ///此数据，这就已经不同步了，所以，如果再添加任务，就会导致数据
        ///发生冲突，任务无法执行，所以maxid需要按照内存中的数据进行增加
        ///同时，由于inserttaskrun和deltask有可能会由多个线程操作，也可能
        ///造成不同步，这个问题也需要解决。当前getnewid是采用了时间编号进行的，这样做是避免编号重复的问题。
        ///***********************************************************************



        public cGlobalParas.TaskState GetTaskState(string workPath,int index)
        {
            cGlobalParas.TaskState TaskState;

            try
            {
                //在此判断一下这个运行文件的物理文件是否存在
                //如果不存在，则进行失败状态的说明
                string fName = workPath + "tasks\\run\\task" + this.GetTaskID(index) + ".rst";
                if (File.Exists(fName))
                {
                    TaskState = (cGlobalParas.TaskState)int.Parse(Tasks[index].Row["TaskState"].ToString());
                }
                else
                {
                    TaskState = cGlobalParas.TaskState.Failed;
                }
            }
            catch
            {
                TaskState = cGlobalParas.TaskState.UnStart;
            }

            return TaskState;
        }

        public long GetRunTime(int index)
        {
            long RunTime;

            try
            {
                RunTime = long.Parse(Tasks[index].Row["RunTime"].ToString());
            }
            catch
            {
                RunTime = 0;
            }
            return RunTime;
        }

        #endregion

        public Int64 GetNewID()
        {
            Int64 id=Int64.Parse ( DateTime.Now.ToFileTime().ToString ());

            return id;
        }

        public void NewTaskRunFile()
        {
            xmlConfig = new cXmlIO();
            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                      "<Tasks>" +
                      "</Tasks>";
            xmlConfig.NewXmlFile(m_workPath + "tasks\\taskrun.xml", strXml);
        }

        private readonly Object m_taskFileLock = new Object();

        
        public Int64 InsertTaskRun(string workPath, string Path, string File)
        {
            ///首先判断存放任务执行的目录是否存在
            ///此目录是固定目录，存放在系统\\Task\\run
            

            string RunPath = workPath + "Tasks\\run";
            Int64 maxID;

            if (!System.IO.Directory.Exists(RunPath))
            {
                System.IO.Directory.CreateDirectory(RunPath);
            }

            try
            {

                ///先将此任务的摘要信息加载到TaskRun.xml文件中
                oTask t = new oTask(m_workPath);
                t.LoadTask(Path + "\\" + File);

                //开始构造xml节点内容
                LoadTaskRunData();
                maxID = GetNewID();

                StringBuilder sb = new StringBuilder();

                string tRunxml = "";
                sb.Append("<TaskID>" + maxID + "</TaskID>");
                sb.Append("<TaskClass>" + t.TaskEntity.TaskClass + "</TaskClass>");
                sb.Append("<TaskName>" + t.TaskEntity.TaskName + "</TaskName>");
                sb.Append( "<TaskPath>" + Path + "</TaskPath>");
                //插入进来数据就是runniing
                sb.Append( "<TaskState>" + (int)cGlobalParas.TaskState.UnStart + "</TaskState>");
                sb.Append("<TaskType>" + t.TaskEntity.TaskType + "</TaskType>");
                sb.Append( "<RunType>" + t.TaskEntity.RunType + "</RunType>");
                sb.Append( "<ExportFile>" + t.TaskEntity.ExportFile + "</ExportFile>");
                if (t.TaskEntity.IsSaveSingleFile)
                {
                    sb.Append( "<tempFile>" + t.TaskEntity.SavePath + "\\" + t.TaskEntity.TempDataFile + "</tempFile>");
                }
                else
                {
                    sb.Append( "<tempFile>" + t.TaskEntity.SavePath + "\\" + t.TaskEntity.TaskName + "-" + maxID + ".xml" + "</tempFile>");
                }
                sb.Append( "<StartDate>" + DateTime.Now + "</StartDate>");
                sb.Append( "<EndDate></EndDate>");
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
                sb.Append( "<PublishType>" + t.TaskEntity.ExportType + "</PublishType>");

                tRunxml = sb.ToString();
                xmlConfig.InsertElement("Tasks", "Task", tRunxml);
                //xmlConfig.Save();
                Save(xmlConfig);
                xmlConfig = null;

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
            
            xmlConfig.DeleteChildNodes("Tasks", "TaskID", TaskID.ToString());
            //xmlConfig.Save();
            Save(xmlConfig);
          
            
        }

    }
}
