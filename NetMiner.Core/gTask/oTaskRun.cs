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
namespace NetMiner.Core.gTask
{

    ///��Ӧ�ó����Tasks�£����TaskRunn.xml����¼��ǰ�������е�������������ʱ����Ҫ���ش��ļ��е�������Ϣ
    ///������Ҫά����������ִ�е���Ϣ��ÿ�����������󣬶�����д���ļ������п��ƣ���������Ŀ������������������
    ///����ִ�У�����ά���Լ���״̬����Ҫ���ں�����չ�ķ���
    ///taskrun.xml��һ������̷߳��ʵ��ļ��������ڲ���֮ǰ��Ҫ���ж������̶߳���Ĳ�������
    ///�Ա��������Ϣ����ʧ�ܵ���������ԣ��Դ������������Ӧ�þ�����ͷŴ������Դ��
    public class oTaskRun:XmlUnity
    {
        DataView Tasks;
        private string m_workPath = string.Empty;
        private List<eTaskRun> m_TaskRunList;

        /// <summary>
        /// ��ʼ��������TaskRun����
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
                //��ʾ�п��ܱ�ռ�ã���������
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

        //���ص�������������ļ�����Ϣ
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

        //����ִ�е�����ID����������Ϣ
        public eTaskRun LoadSingleTask(Int64 TaskID)
        {
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            return Convert(xe);
        }

        /// <summary>
        /// �жϴ����������ж������Ƿ���ڣ�ע����Ҫ��Ŀ¼����V5.5��ʼ�������������ظ�
        /// </summary>
        /// <param name="TaskName">�ɼ���������</param>
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
        /// ͳһ���²ɼ��������е�״̬
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
                //˵���������ʵ���Ѿ���ɾ���ˣ������ϲ��������������������Ȼ���ڣ����߳���ɵ�����
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

        #region �����ƶ��������Ż�ȡ�����������Ϣ
        public int GetCount()
        {
            return base.GetNodesCount("Task");
        }

        ///�����������*****************************************************************************************
        ///���Ȳɼ�����������ɺ����ݲ�û�����������ת�Ƶ�����ɶ���
        ///��ζ���ڴ��л����ڴ����ݣ��������õȡ���taskrun��ȷ�Ѿ�ɾ����
        ///�����ݣ�����Ѿ���ͬ���ˣ����ԣ������������񣬾ͻᵼ������
        ///������ͻ�������޷�ִ�У�����maxid��Ҫ�����ڴ��е����ݽ�������
        ///ͬʱ������inserttaskrun��deltask�п��ܻ��ɶ���̲߳�����Ҳ����
        ///��ɲ�ͬ�����������Ҳ��Ҫ�������ǰgetnewid�ǲ�����ʱ���Ž��еģ��������Ǳ������ظ������⡣
        ///***********************************************************************

        //public cGlobalParas.TaskState GetTaskState(string workPath,int index)
        //{
        //    cGlobalParas.TaskState TaskState;

        //    try
        //    {
        //        //�ڴ��ж�һ����������ļ��������ļ��Ƿ����
        //        //��������ڣ������ʧ��״̬��˵��
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
        /// �ɼ�����ʱ������TaskRun�Ĳ�������
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="tClass">�����������·��</param>
        /// <param name="Path">������Ǿ��Ե�ַ</param>
        /// <param name="File"></param>
        /// <returns></returns>
        public Int64 InsertTaskRun(string workPath, string tClass, string Path, string File)
        {
            ///�����жϴ������ִ�е�Ŀ¼�Ƿ����
            ///��Ŀ¼�ǹ̶�Ŀ¼�������ϵͳ\\Task\\run
            

            string RunPath = workPath + NetMiner.Constant.g_TaskRunPath;
            Int64 maxID;
            try
            {

                ///�Ƚ��������ժҪ��Ϣ���ص�TaskRun.xml�ļ���
                oTask t = new oTask(m_workPath);
                t.LoadTask(Path + "\\" + File);

                //��ʼ����xml�ڵ�����
                LoadTaskRunData();
                maxID = GetNewID();

                StringBuilder sb = new StringBuilder();

                sb.Append("<TaskID>" + maxID + "</TaskID>");
                sb.Append("<TaskClass>" + tClass + "</TaskClass>");
                sb.Append("<TaskName>" + t.TaskEntity.TaskName + "</TaskName>");
                sb.Append( "<TaskPath>" + Path + "</TaskPath>");
                //����������ݾ���runniing
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
                sb.Append( "<PublishType>" + (int)t.TaskEntity.ExportType + "</PublishType>");

                XElement xe = XElement.Load(new StringReader("<Task>" + sb.ToString() + "</Task>"));

                base.AddElement(base.xDoc.Root, xe);

                base.Save();

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
            XElement xe = base.SearchElement("Task", "TaskID", TaskID.ToString());
            base.RemoveElement(xe);
            base.Save();

        }

    }
}
