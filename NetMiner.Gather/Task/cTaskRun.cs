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

///���ܣ��������������ļ�����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
///2013��11��1�ս���taskrun�Ĵ����޸ģ������˶���������״̬
///�ı��棬�ɰ汾��֧�ֲɼ�������Ϊһ�����ɸ�Ԥ����������
///�����˶Բɼ���������״̬������������������ʱ�жϣ�����¼
///�Ѿ�������ʧ�ܷ����ļ�¼��
namespace NetMiner.Gather.Task
{

    ///��Ӧ�ó����Tasks�£����TaskRunn.xml����¼��ǰ�������е�������������ʱ����Ҫ���ش��ļ��е�������Ϣ
    ///������Ҫά����������ִ�е���Ϣ��ÿ�����������󣬶�����д���ļ������п��ƣ���������Ŀ������������������
    ///����ִ�У�����ά���Լ���״̬����Ҫ���ں�����չ�ķ���
    ///taskrun.xml��һ������̷߳��ʵ��ļ��������ڲ���֮ǰ��Ҫ���ж������̶߳���Ĳ�������
    ///�Ա��������Ϣ����ʧ�ܵ���������ԣ��Դ������������Ӧ�þ�����ͷŴ������Դ��
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

        //���ص�������������ļ�����Ϣ
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

            //��ȡTaskClass�ڵ�
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

        //    //��ȡTaskClass�ڵ�
        //    Tasks = xmlConfig.GetData("Tasks");
        //}

        //���ü���������Ϣ��,���ô˷�������һ�������dataview
        public DataView GetTasks()
        {
            return Tasks;
        }

        //����ִ�е�����ID����������Ϣ
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

            //��ȡTaskClass�ڵ�,���ܷ��ص�Ҳ��DataView,������ֻ����һ����¼
            //��������Ϊ�˸��õļ��ݷ��ʲ���
            Tasks = xmlConfig.GetData("Tasks","TaskID",TaskID.ToString () );
        }

        /// <summary>
        /// ͳһ���²ɼ��������е�״̬
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

        //���㵱ǰ���ж��ٸ�������������
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

        #region �����ƶ��������Ż�ȡ�����������Ϣ

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

        //�����������ʵ���ļ��洢·��
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

        //���ش�������Ҫ�ɼ�����ҳ��ַ����
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

        //���ش����񵼺���������ַ����
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

        //���ش������Ѿ��ɼ���ҳ�ĵ�ַ����
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

        ///�����������*****************************************************************************************
        ///���Ȳɼ�����������ɺ����ݲ�û�����������ת�Ƶ�����ɶ���
        ///��ζ���ڴ��л����ڴ����ݣ��������õȡ���taskrun��ȷ�Ѿ�ɾ����
        ///�����ݣ�����Ѿ���ͬ���ˣ����ԣ������������񣬾ͻᵼ������
        ///������ͻ�������޷�ִ�У�����maxid��Ҫ�����ڴ��е����ݽ�������
        ///ͬʱ������inserttaskrun��deltask�п��ܻ��ɶ���̲߳�����Ҳ����
        ///��ɲ�ͬ�����������Ҳ��Ҫ�������ǰgetnewid�ǲ�����ʱ���Ž��еģ��������Ǳ������ظ������⡣
        ///***********************************************************************



        public cGlobalParas.TaskState GetTaskState(string workPath,int index)
        {
            cGlobalParas.TaskState TaskState;

            try
            {
                //�ڴ��ж�һ����������ļ��������ļ��Ƿ����
                //��������ڣ������ʧ��״̬��˵��
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
            ///�����жϴ������ִ�е�Ŀ¼�Ƿ����
            ///��Ŀ¼�ǹ̶�Ŀ¼�������ϵͳ\\Task\\run
            

            string RunPath = workPath + "Tasks\\run";
            Int64 maxID;

            if (!System.IO.Directory.Exists(RunPath))
            {
                System.IO.Directory.CreateDirectory(RunPath);
            }

            try
            {

                ///�Ƚ��������ժҪ��Ϣ���ص�TaskRun.xml�ļ���
                oTask t = new oTask(m_workPath);
                t.LoadTask(Path + "\\" + File);

                //��ʼ����xml�ڵ�����
                LoadTaskRunData();
                maxID = GetNewID();

                StringBuilder sb = new StringBuilder();

                string tRunxml = "";
                sb.Append("<TaskID>" + maxID + "</TaskID>");
                sb.Append("<TaskClass>" + t.TaskEntity.TaskClass + "</TaskClass>");
                sb.Append("<TaskName>" + t.TaskEntity.TaskName + "</TaskName>");
                sb.Append( "<TaskPath>" + Path + "</TaskPath>");
                //����������ݾ���runniing
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

                ///TrueUrlCount��ʾ����ɼ�����ַ�д��ڵ�����ַ������Ҫ�ɼ�����ַ���޷����ݹ�ʽ���˳�����
                ///��Ҫ�ɼ����񲻶�ִ�У����ϸ��ݲɼ��Ĺ�����м���ɼ���ַ��������������Ҫ�ٴμ�¼��ֵ
                ///��¼��ֵ��Ŀ����Ϊ�˿��Ը��õĸ��ٲɼ��Ľ��ȣ���Urlcount�����޸ģ���Ϊ��ֵҪ��������ֽ�
                ///ʹ�ã�����ı���UrlCount����ܵ�������ֽ�ʧ�ܣ�����Ӫ�����ʼ����ʱ�򣬴�ֵͬUrlCount����ֵ��
                ///������������Ӫʱά��
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

                ///������������xml�ļ��ĸ�ʽ��Task�����ʽ��ȫһ�۸�����������ʽ��ȫ��ͬ
                ///������ʽ�ǰ���Task����ǰ�ļ���Taskrun�е�id����������������Ŀ����֧��ͬһ������
                ///���Խ����������ʵ����Ҳ���ǵ�����������е�ʱ���û�Ҳ�����޸Ĵ����������
                ///һ��ʵ����ʼ���С�
                System.IO.File.Copy(Path + "\\" + File, RunPath + "\\" + "Task" + maxID + ".rst", true);

                //�ļ�������ȥ����Ҫ�޸��ļ��е�TaskID�����Ǻ�TaskRun�е�TaskID����������
                //�ڼ����ļ���ʱ������,ϵͳ��ID����Ψһ����
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
