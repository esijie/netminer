using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using NetMiner.Gather.Task;
using NetMiner.Gather.Task.Entity;
using NetMiner.Common;

///���ܣ��ɼ�������� ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Gather.Task
{
    public class oTaskClass
    {
        cXmlIO xmlConfig;
        private string m_workPath = string.Empty;

        //����һ��������
        public List<cTaskIndex> Task
        {
            get { return Task; }
            set { Task = value; }
        }

        public List<eTaskClass> TaskClasses
        {
            get;
            set;
        }

        public void NewClassFile(string fName)
        {

            string strXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                       "<TaskConfig><TaskClasses></TaskClasses><Tasks><Task></Task></Tasks></TaskConfig>";
            xmlConfig = new cXmlIO();
            xmlConfig.NewXmlFile(fName, strXml);
            xmlConfig = null;

        }

        /// <summary>
        /// ��ͬ����������
        /// </summary>
        /// <param name="tPath"></param>
        public oTaskClass(string tPath)
        {
            m_workPath = tPath;
            TaskClasses = new List<eTaskClass>();

            string className = tPath + "tasks\\TaskClass.xml";
            if (!File.Exists(className))
            {
                //�Զ�����һ�����������ļ�
                NewClassFile(className);
            }

            try
            {
                xmlConfig = new cXmlIO(tPath + "tasks\\TaskClass.xml");

                //��ȡTaskClass�ڵ�
                //TaskClass = xmlConfig.GetData("descendant::TaskClasses");

                XmlDocument xmlDoc = xmlConfig.XmlDoc;

                XmlNodeList nodeList=  xmlDoc.GetElementsByTagName("TaskClasses");

                if (nodeList==null || nodeList.Count ==0)
                    return ;

                for (int i=0;i<nodeList[0].ChildNodes.Count ;i++)
                {
                    eTaskClass eClass = new eTaskClass();
                    eClass.ID = int.Parse(nodeList[0].ChildNodes[i]["id"].InnerText);
                    eClass.Name = nodeList[0].ChildNodes[i]["Name"].InnerText;
                    eClass.tPath = nodeList[0].ChildNodes[i]["Path"].InnerText;

                    if (nodeList[0].ChildNodes[i]["Childrens"] != null && nodeList[0].ChildNodes[i]["Childrens"].ChildNodes.Count > 0)
                    {
                        eClass.Children = LoadTaskClass(nodeList[0].ChildNodes[i]["Childrens"].ChildNodes);
                    }
                    else
                        eClass.Children = null;

                    TaskClasses.Add(eClass);
                }
            
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        private List<eTaskClass> LoadTaskClass(XmlNodeList nodeList)
        {
            
            List<eTaskClass> ets = new List<eTaskClass>();

            for (int i = 0; i < nodeList.Count ; i++)
            {
                eTaskClass eTask = new eTaskClass();
                eTask.ID = int.Parse(nodeList[i]["id"].InnerText);
                eTask.Name = nodeList[i]["Name"].InnerText;
                eTask.tPath = nodeList[i]["Path"].InnerText;
                if (nodeList[i]["Childrens"] != null && nodeList[i]["Childrens"].ChildNodes.Count  > 0)
                    eTask.Children = LoadTaskClass(nodeList[i]["Childrens"].ChildNodes);
                else
                    eTask.Children = null;
                ets.Add(eTask);
            }

            return ets;
        }

        public List<eTaskClass> GetAllTaskClass(List<eTaskClass> tClasses)
        {
            List<eTaskClass> ets = new List<eTaskClass>();

            for (int i = 0; i < tClasses.Count; i++)
            {
                if (tClasses[i].Children != null || tClasses[i].Children.Count == 0)
                    ets.Add(tClasses[i]);
                else
                {
                    List<eTaskClass> ets1 = GetAllTaskClass(tClasses[i].Children);
                    ets.AddRange(ets1);
                }

            }
            return ets;
        }

        //public oTaskClass()
        //{
        //    try
        //    {
        //        xmlConfig = new cXmlIO(m_workPath + "tasks\\TaskClass.xml");

        //        //��ȡTaskClass�ڵ�
        //        TaskClass = xmlConfig.GetData("descendant::TaskClasses");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        ~oTaskClass()
        {
            xmlConfig = null;
        }

        public int GetTaskClassCount(int cID)
        {
            eTaskClass et = null;
            for (int i = 0; i < TaskClasses.Count; i++)
            {
                if (TaskClasses[i].ID == cID)
                {
                    et = TaskClasses[i];
                    break;
                }

                if (TaskClasses[i].Children != null)
                {
                    et = FindTaskClass(TaskClasses[i].Children, cID);
                    if (et != null)
                        break;
                }
            }

            if (et == null)
                return 0;
            else
                return et.Children.Count;
        }

        //����ָ�������IDȡ��������ƣ���һ�������ķ���·��
        public string GetTaskClassNameByID(string ID)
        {
            string TName = string.Empty;
            TName = getTaskName(TaskClasses, int.Parse(ID));
            return TName;
        }

        private string getTaskName(List<eTaskClass> ets, int ID)
        {
            eTaskClass et = null;
            string tName = string.Empty;

            for (int i = 0; i < ets.Count; i++)
            {
                if (ets[i].ID == ID)
                {
                    et = ets[i];
                    tName =et.Name;
                    break;
                }

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    tName = getTaskName(ets[i].Children, ID);
                }

                if (!string.IsNullOrEmpty(tName))
                {
                    tName = ets[i].Name + "/" + tName;
                    break;
                }
            }

            return tName;
        }

        //����ָ����ID����TaskClassPath
        public string GetTaskClassPathByID(int ID)
        {
            int i = 0;

            eTaskClass et = null;
            for (i = 0; i < TaskClasses.Count ;i++)
            {
                if (TaskClasses[i].ID == ID)
                {
                    et = TaskClasses[i];
                    break;
                }

                if (TaskClasses[i].Children != null)
                {
                    et = FindTaskClass(TaskClasses[i].Children, ID);
                    if (et != null)
                        break;
                }
            }

            if (et == null)
                return m_workPath + "tasks";
            else
                return m_workPath + et.tPath;
        
        }

        public int GetTaskClassIDByName(string Name)
        {
            if (Name == cTool.g_RemoteTaskClass)
            {
                return -1;
            }

            string[] tNames = Name.Split('/');
            int Level = 0;
            string cName = tNames[Level];
            string taskPath = string.Empty;
            eTaskClass et = null;

            for (int i = 0; i < TaskClasses.Count; i++)
            {
                bool isExist = false;

                if (TaskClasses[i].Name == cName)
                {
                    isExist = true;
                    if (tNames.Length == Level + 1)
                    {
                        et = TaskClasses[i];
                        break;
                    }
                }

                if (isExist)
                {
                    et = null;
                    if (tNames.Length > Level + 1)
                    {
                        if (TaskClasses[i].Children != null)
                        {
                            et = FindClassByName(TaskClasses[i].Children, tNames, Level + 1);
                        }
                    }
                }
                if (isExist==true && et != null)
                    break;
            }

            if (et == null)
                return 0;
            else
                return et.ID;
        }

        /// <summary>
        /// ����ָ����Task�������Ʒ�������������洢��·��
        /// </summary>
        /// <param name="workPath">����·��</param>
        /// <param name="Name">��������</param>
        /// <returns></returns>
        public string GetTaskClassPathByName(string Name)
        {
            if (Name == cTool.g_RemoteTaskClass)
            {
                return m_workPath + cTool.g_RemoteTaskPath;
            }

            string[] tNames = Name.Split('/');
            int Level = 0;
            string cName = tNames[Level];
            string taskPath = string.Empty;
            eTaskClass et=null;

            for (int i=0;i<TaskClasses.Count ;i++)
            {
                bool isExist = false;

                if (TaskClasses[i].Name ==cName )
                {
                    isExist = true;
                    if (tNames.Length == Level + 1)
                    {
                        et = TaskClasses[i];
                        break;
                    }
                }

                //һ��Ŀ¼ƥ���ϲŻ�����Ӽ�
                if (isExist ==true )
                { 
                    if (tNames.Length > Level+1)
                    {
                        if (TaskClasses[i].Children != null)
                        {
                            et = FindClassByName(TaskClasses[i].Children, tNames, Level + 1);
                        }
                    }
                }
                if (et != null)
                    break;
            }

            if (et == null)
                return m_workPath + "tasks";
            else
                return m_workPath + et.tPath;
        }

        //�ж���������Ƿ����
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskClassName">����Ƕ����������/�ָ�</param>
        /// <param name="ClassID"></param>
        /// <returns></returns>
        public bool IsExist(string TaskClassName)
        {
            if (string.IsNullOrEmpty(TaskClassName))
                return true;

            bool isExist = false;

            string[] taskClasses = TaskClassName.Split('/');
            int level = 0;

            if (TaskClasses == null || TaskClasses.Count==0)
                return false;

            //�ȱȽ϶���Ŀ¼
            for (int i = 0; i < TaskClasses.Count; i++)
            {

                if (TaskClasses[i].Name == taskClasses[level])
                {
                    if(taskClasses.Length>1)
                    {
                        //��ʾ���Ӽ�Ŀ¼
                        if (TaskClasses[i].Children != null && TaskClasses[i].Children.Count > 0)
                        {
                            isExist = isExistChildren(TaskClasses[i].Children, taskClasses, level + 1);
                            if (isExist == true)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        isExist = true;
                        break;
                    }
                }
               
            }

            return isExist;
        }

        private bool isExistChildren(List<eTaskClass> ets,string[] TaskClassName,int level)
        {
            bool isExist = false;

            for (int i = 0; i < ets.Count;i++ )
            {
                if (ets[i].Name == TaskClassName[level] && TaskClassName.Length == level + 1)
                {
                    isExist = true;
                }

                if (ets[i].Children != null && ets[i].Children.Count > 0 && TaskClassName.Length >level +1)
                {
                    isExist = isExistChildren(ets[i].Children, TaskClassName, level + 1);
                }

                if (isExist == true)
                    break;
            }

            return isExist;

        }

        //���ӷ���ڵ㣬�����ӳɹ����򷵻���ӳɹ���ķ���ڵ�ID
        //ϵͳ�д洢��·��ȫ���������·����������洢����·��
        //ϵͳ�����Ǿ���·��������·�������·����ת���ڷ������ڲ����
        //ϵͳ���⿴���Ǿ���·��
        public int AddTaskClass(string TaskClassName)
        {
            //ת�����·��
            
 
            //�ֽ�ɼ�����
            string[] tClasses = TaskClassName.Split('/');

            int tCount = TaskClasses.Count;
            int level=0;

            bool isExistClass = false;

            int MaxID = 10;
            string fClassName=string.Empty ;
            List<eTaskClass> drv = null;

            int fID = 0;

            //�������жϵ�һ�������Ƿ���ڣ����������ʼ�����Ӽ�����
            //��Ҫ�ж��½�������������Ƿ��Ѿ�����
            for (int i = 0; i < TaskClasses.Count; i++)
            {
                if (TaskClasses[i].Name == tClasses[level])
                {
                    //throw new SoukeyException("��������Ѿ����ڣ�");
                    isExistClass = true;
                    fClassName= tClasses[level];
                    drv = TaskClasses[i].Children;
                    fID = TaskClasses[i].ID;
                    break;
                     
                }
            }
            string TaskClassPath = "tasks\\" + tClasses[level] ;


            if (isExistClass==false)
            {
                //ֻ��һ��Ŀ¼����ʼ����һ��Ŀ¼
                string strTaskClass = "";

                if (TaskClasses.Count > 0)
                {
                    int index = TaskClasses.Count - 1;
                    MaxID = TaskClasses[index].ID + 1;
                }
                else
                {
                }

                strTaskClass = "<id>" + MaxID + "</id>";
                strTaskClass += "<Name>" + tClasses[level] + "</Name>";
                strTaskClass += "<Path>" + TaskClassPath + "</Path>";
                xmlConfig.InsertElement("TaskConfig/TaskClasses", "TaskClass", strTaskClass);
                xmlConfig.Save();


                //�����������������Ŀ¼�������ļ�
                if (!System.IO.Directory.Exists(TaskClassPath))
                {
                    System.IO.Directory.CreateDirectory(TaskClassPath);
                }

                Task.cTaskIndex tIndex = new Task.cTaskIndex(m_workPath);
                tIndex.NewIndexFile(TaskClassPath);
                tIndex = null;

                fID = MaxID;
            }
            else
            {
                if (tClasses.Length ==level+1)
                {
                    throw new SoukeyException("��������Ѿ����ڣ�");
                }
            }

            //�жϵ�ǰ�Ƿ�Ϊ��ͼ����࣬���������ʼ�����Ӽ�����
            if (tClasses.Length >level+1)
            {
                MaxID= AddChildrenClass(fID, tClasses[level], drv, tClasses, level + 1);
            }

            return MaxID;

        }

        /// <summary>
        /// �Ӽ�Ŀ¼�Ľ���
        /// </summary>
        /// <param name="fClassName"></param>
        /// <param name="dv"></param>
        /// <param name="tClassName"></param>
        /// <param name="tPath"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        private int AddChildrenClass(int fClassID, string fClassName, List<eTaskClass> dv, string[] tClassName, int level)
        {

            int fID=0;

            string cName = tClassName[level];
            bool isExistClass = false;
            int MaxID = int.Parse (fClassID.ToString() + "10");
            string strTaskClass = string.Empty;
            List<eTaskClass> drv = null;

            string TaskClassPath = string.Empty;

            if (fClassName.IndexOf ("/")>0)
            {
                string[] ss = fClassName.Split('/');
                for (int m=0;m<ss.Length;m++)
                {
                    TaskClassPath += ss[m] + "\\";
                }
                TaskClassPath = "tasks\\" + TaskClassPath + cName;
            }
            else
                TaskClassPath = "tasks\\" + fClassName + "\\" + cName;

            if (dv==null ||  dv.Count==0)
            {
                //��ʾ�����ڴ˷��࣬��Ҫ���н������ڴ˽���Ϊ�ӷ���

                strTaskClass += "<TaskClass>";
                strTaskClass += "<id>" + MaxID + "</id>";
                strTaskClass += "<Name>" + cName + "</Name>";
                strTaskClass += "<Path>" + TaskClassPath + "</Path>";
                strTaskClass += "</TaskClass>";
                xmlConfig.InsertElement("TaskClass", "ID", fClassID.ToString (), "Childrens", strTaskClass);
                xmlConfig.Save();

                //�����������������Ŀ¼�������ļ�
                if (!System.IO.Directory.Exists(TaskClassPath))
                {
                    System.IO.Directory.CreateDirectory(TaskClassPath);
                }

                Task.cTaskIndex tIndex = new Task.cTaskIndex(m_workPath);
                tIndex.NewIndexFile(TaskClassPath);
                tIndex = null;

                isExistClass = true;

                fID=MaxID;
            }
            else
            {

                for (int i = 0; i < dv.Count; i++)
                {
                    if (dv[i].Name == cName)
                    {
                        //throw new SoukeyException("��������Ѿ����ڣ�");
                        isExistClass = true;
                        //fClassName = dv[i].Name ;
                        fID = dv[i].ID;
                        drv = dv[i].Children;
                    }
                }

           

                if (isExistClass == false)
                {
                    int index = dv.Count - 1;
                    MaxID = dv[index].ID + 1;
                    strTaskClass += "<id>" + MaxID + "</id>";
                    strTaskClass += "<Name>" + cName + "</Name>";
                    strTaskClass += "<Path>" + TaskClassPath + "</Path>";
                    xmlConfig.AppendElement("TaskClass", "ID", fClassID.ToString(), "TaskClass", strTaskClass);
                    xmlConfig.Save();

                    //�����������������Ŀ¼�������ļ�
                    if (!System.IO.Directory.Exists(TaskClassPath))
                    {
                        System.IO.Directory.CreateDirectory(TaskClassPath);
                    }

                    Task.cTaskIndex tIndex = new Task.cTaskIndex(m_workPath);
                    tIndex.NewIndexFile(TaskClassPath);
                    tIndex = null;

                    fID=MaxID;
                }
                else
                {
                    if (tClassName.Length ==level+1)
                    {
                        throw new SoukeyException("��������Ѿ����ڣ�");
                    }
                }

            }

            if (tClassName.Length >level+1)
            {
                MaxID=AddChildrenClass(fID, fClassName + "/" + tClassName[level], drv, tClassName, level + 1);
            }
          

            return MaxID;
        }

        /// <summary>
        /// ɾ��ָ���ķ����ļ���������������Ƕ༶����/�ָ�
        /// </summary>
        /// <param name="TClassName"></param>
        /// <returns></returns>
        public bool DelTaskClass(int tClassID)
        {
            bool isS = false;

            XmlDocument xmlDoc = xmlConfig.XmlDoc;

            XmlNodeList nodeList = xmlDoc.GetElementsByTagName("TaskClasses");

            if (nodeList == null || nodeList.Count == 0)
                return true;

            string FilePath = string.Empty;

            for (int i = 0; i < nodeList[0].ChildNodes.Count; i++)
            {
                if (tClassID.ToString () == nodeList[0].ChildNodes[i]["id"].InnerText)
                {
                    //ɾ��
                    FilePath = nodeList[0].ChildNodes[i]["Path"].InnerText;
                    nodeList[0].ChildNodes[i].ParentNode.RemoveChild(nodeList[0].ChildNodes[i]);
                    xmlConfig.Save();
                    System.IO.Directory.Delete(m_workPath + FilePath, true);
                    isS = true;
                    break;
                }
                
                if (nodeList[0].ChildNodes[i]["Childrens"] != null && nodeList[0].ChildNodes[i]["Childrens"].ChildNodes.Count > 0)
                {
                    //��ʼ���Լ�����
                    isS = DelChildrenTaskClass(nodeList[0].ChildNodes[i]["Childrens"].ChildNodes,tClassID);
                }
                
            }

            //����ɾ��TaskClass.xml�е�������������ڵ�
            //string FilePath = this.GetTaskClassPathByName(TClassName);
            //xmlConfig.DeleteChildNodes("TaskClasses", "Name", TClassName);
            //xmlConfig.Save();

            //System.IO.Directory.Delete (FilePath ,true );
            return isS;

        }

        public bool DelChildrenTaskClass(XmlNodeList nodeList,int cID)
        {
            bool isS = false;

            string FilePath = string.Empty;

            for (int i = 0; i < nodeList.Count; i++)
            {
                if (cID.ToString () == nodeList[i]["id"].InnerText)
                {
                    //ɾ��
                    FilePath = nodeList[i]["Path"].InnerText;

                    XmlNode xNode=null;
                    if (nodeList[i].ParentNode.ChildNodes.Count == 1)
                    {
                        //��ʾ�����һ���ӷ��࣬ɾ���󣬻��轫<Childrens>ɾ����
                        xNode = nodeList[i].ParentNode.ParentNode;
                        xNode.RemoveChild(nodeList[i].ParentNode);
                    }
                    else
                        nodeList[i].ParentNode.RemoveChild(nodeList[i]);
                    xmlConfig.Save();
                    System.IO.Directory.Delete(m_workPath + FilePath, true);
                    isS = true;
                    break;
                }

                if (nodeList[i]["Childrens"] != null && nodeList[i]["Childrens"].ChildNodes.Count > 0)
                {
                    //��ʼ���Լ�����
                    isS = DelChildrenTaskClass(nodeList[i]["Childrens"].ChildNodes, cID);
                }

            }

            return isS;
        }

        //���������������������������½�һ�����࣬����ԭ�з��������
        //Ǩ�ƹ��������޸�������Ϣ�����񣬲�ɾ��ԭ������
        public bool RenameTaskClass(string TClassName, string NewTClassName)
        {
            //try
            //{
                string nName = string.Empty;
                nName = NewTClassName;

                string OldPath = GetTaskClassPathByName(TClassName);

                string NewPath = OldPath.Substring(0, OldPath.LastIndexOf("\\")) + "\\" + NewTClassName;

                if (TClassName.IndexOf ('/')>0)
                    NewTClassName = TClassName.Substring(0, TClassName.LastIndexOf ("/")) + "/" + NewTClassName;

                int OldTaskClassID = 0;

                //�ж��µ�����·���Ƿ���ڣ���������򱨴�
                if (Directory.Exists(NewPath))
                    throw new SoukeyException("����������·���Ѿ����ڣ��������޸�����������ƣ�");

                //ת�����·��
                string NewRelativePath = cTool.GetRelativePath(m_workPath, NewPath);

                bool isE = IsExist(NewTClassName);
                if (isE)
                    throw new SoukeyException("��������Ѿ����ڣ�");
               
                        //��ȡԭ�з����ID
                OldTaskClassID = GetTaskClassIDByName(TClassName);
              

                if (OldTaskClassID == 0)
                {
                    throw new SoukeyException("δ���ҵ���Ҫ�޸ķ������Ϣ�������޸�ʧ�ܣ�");
                }

                //��ʼ�޸���������µ������������������
                cTaskIndex xmlTasks = new cTaskIndex(m_workPath);
                xmlTasks.GetTaskDataByClass(TClassName);

                //��ʼ��ʼ���˷����µ�����
                int count = xmlTasks.GetTaskClassCount();

                for (int i = 0; i < count; i++)
                {
                    cTask t = new cTask(m_workPath);
                    t.LoadTask(OldPath + "\\" + xmlTasks.GetTaskName(i) + ".smt");
                    t.TaskClass = NewTClassName;
                    t.SaveTaskFile(OldPath + "\\");
                }

                xmlTasks = null;

                XmlNodeList nodeList = xmlConfig.XmlDoc.GetElementsByTagName("TaskClasses");

                //��ʼ�޸�taskclass.xml�ļ��е��������������Ϣ
                xmlConfig.EditNodeValue(nodeList, "id", OldTaskClassID.ToString(), "Name", nName);
                xmlConfig.EditNodeValue(nodeList, "id", OldTaskClassID.ToString(), "Path", NewRelativePath);

            // ��Ҫ�޸Ĵ˷��������е�·��
            eTaskClass et = FindTaskClass(TaskClasses, OldTaskClassID);

                if (et.Children != null && et.Children.Count > 0)
                {
                    for (int i = 0; i < et.Children.Count; i++)
                    {
                        string path = NewRelativePath + "\\" + et.Children[i].Name;
                        xmlConfig.EditNodeValue(nodeList, "id", et.Children[i].ID.ToString (), "Path", path);

                        if (et.Children[i].Children != null && et.Children[i].Children.Count > 0)
                        {
                        EditTaskClassPath(nodeList , et.Children[i].Children, path);
                        }
                    }
                }

                //xmlConfig.EditNodeValue(nodeList, "id", OldTaskClassID.ToString(), "Path", NewRelativePath);
                xmlConfig.Save();
                xmlConfig = null;

                //��ʼ���޸���������ʵ��·��
                File.SetAttributes(OldPath, System.IO.FileAttributes.Normal);
                Directory.Move(OldPath, NewPath);
                if (Directory.Exists (OldPath ))
                    Directory.Delete(OldPath);

            //}
            //catch (System.Exception ex)
            //{
            //    throw ex;
            //    return false;
            //}

            return true;
        }

        private void EditTaskClassPath(XmlNodeList nodeList, List<eTaskClass> ets ,string tPath)
        {
            for (int i=0;i<ets.Count;i++)
            {
                string path = tPath + "\\" + ets[i].Name;
                xmlConfig.EditNodeValue(nodeList, "id", ets[i].ID.ToString(), "Path", path);

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    EditTaskClassPath(nodeList, ets[i].Children, path);
                }
            }
        }

        //public int GetTaskClassCount()
        //{
        //    return TaskClasses.Count;
        //}

        private eTaskClass FindTaskClass(List<eTaskClass> ets, int ID)
        {
            eTaskClass et = null;

            for(int i=0;i<ets.Count ;i++)
            {
                if (ets[i].ID==ID)
                {
                    et= ets[i];
                    break;
                }
                
                if (ets[i].Children!=null && ets[i].Children.Count >0)
                {
                    et = FindTaskClass(ets[i].Children, ID);
                }

                if (et != null)
                    break;
            }

            return et;
        }

        private eTaskClass FindClassByName(List<eTaskClass> ets, string[] tNames, int Level)
        {
            eTaskClass et = null;
            for (int i = 0; i < ets.Count; i++)
            {
                bool isExist = false;

                if (ets[i].Name == tNames[Level])
                {
                    isExist = true;
                    if (tNames.Length ==Level+1)
                    {
                    et = ets[i];
                    break;
                    }
                }

                if (isExist)
                {
                    if (ets[i].Children != null && tNames.Length > Level + 1)
                    {
                        et = FindClassByName(ets[i].Children, tNames, Level + 1);
                        if (et != null)
                            break;
                    }
                }
            }

            return et;
        }
    }
}
