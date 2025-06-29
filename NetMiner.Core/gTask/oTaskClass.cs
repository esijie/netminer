using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.IO;
using NetMiner.Common;
using NetMiner.Core.gTask.Entity;
using NetMiner.Base;
using NetMiner;
using System.Xml.Linq;
using System.Linq;

///���ܣ��ɼ�������� ����
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����
namespace NetMiner.Core.gTask
{
    public class oTaskClass:XmlUnity
    {
        private string m_workPath = string.Empty;
        

        /// <summary>
        /// ��ͬ����������
        /// </summary>
        /// <param name="tPath"></param>
        public oTaskClass(string workPath)
        {
            m_workPath = workPath;
            TaskClasses = new List<eTaskClass>();

            string className = workPath + NetMiner.Constant.g_TaskClassFile;
            if (!File.Exists(className))
            {
                //�Զ�����һ�����������ļ�
                NewClassFile(className);
            }

            base.LoadXML(className);

            //��ȡ����һ���Ԫ����
            IEnumerable<XElement> xes = base.GetElements
                (base.GetElement("/TaskConfig/TaskClasses"), "TaskClass");

            foreach(XElement xe in xes)
            {
                eTaskClass eClass = new eTaskClass();
                eClass.ID = int.Parse(xe.Element("ID").Value.ToString());
                eClass.Name = xe.Element("Name").Value.ToString();
                eClass.tPath = xe.Element("Path").Value.ToString();
                eClass.Children = LoadTaskClass(xe);
                TaskClasses.Add(eClass);
            }
           
        }

        ~oTaskClass()
        {
            base.Dispose();
        }

        private List<eTaskClass> LoadTaskClass(XElement xele)
        {
            int count= base.GetNodesCount(xele, "Children");

            if (count == 0)
                return null;
            else
            {
                IEnumerable<XElement> xes =  base.GetElements(xele.Element("Children"), "TaskClass");

                List<eTaskClass> ets = new List<eTaskClass>();

                foreach (XElement xe in xes)
                {
                    eTaskClass eClass = new eTaskClass();
                    eClass.ID = int.Parse(xe.Element("ID").Value.ToString());
                    eClass.Name = xe.Element("Name").Value.ToString();
                    eClass.tPath = xe.Element("Path").Value.ToString();
                    eClass.Children = LoadTaskClass(xe);
                    ets.Add(eClass);
                }

                return ets;
            }
        }

        #region ����
        //����һ��������
      

        public List<eTaskClass> TaskClasses
        {
            get;
            set;
        }
        #endregion

        public void NewClassFile(string fName)
        {
            XElement xe = new XElement("TaskConfig");
            xe.Add(new XElement("TaskClasses"));
            xe.Add(new XElement("Tasks", new XElement("Task")));

            base.NewXML(fName, xe);
        }

        /// <summary>
        /// �����·��������Ϊ���·��
        /// </summary>
        /// <param name="tPath"></param>
        /// <param name="oldName"></param>
        /// <param name="newClass"></param>
        /// <param name="newPath"></param>
        public void RenameTaskClass(string tPath,string oldName, string newPath, string newClass)
        {
            XElement xe=base.SearchElement("TaskClass", "Path", tPath);

            base.EditValue(xe.Element("Name"), newClass);
            base.EditValue(xe.Element("Path"), newPath);


            //�жϴ˽ڵ����Ƿ����ӽڵ㲢��ʼ�޸�
            if (xe.Element("Children")!=null)
            {
                //��ʾ���ӽڵ�
                IEnumerable<XElement> cXe = xe.Element("Children"). Elements("TaskClass");
                foreach(XElement ce in cXe)
                {
                    string oPath = ce.Element("Path").Value.ToString();
                    string nPath = oPath.Replace(tPath, newPath);


                    RenameTaskClass(oPath, ce.Element("Name").Value.ToString(), nPath, ce.Element("Name").Value.ToString());
                }
            }

            base.Save();
        }

        public List<eTaskClass> GetAllTaskClass(List<eTaskClass> tClasses)
        {
            List<eTaskClass> ets = new List<eTaskClass>();

            for (int i = 0; i < tClasses.Count; i++)
            {
                if (tClasses[i].Children != null && tClasses[i].Children.Count > 0)
                {
                    List<eTaskClass> ets1 = GetAllTaskClass(tClasses[i].Children);
                    ets.AddRange(ets1);
                }
                else
                {
                    ets.Add(tClasses[i]);
                }

            }
            return ets;
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
            if (Name == NetMiner.Constant.g_RemoteTaskClass)
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
        /// ����ָ����Task�������Ʒ�������������洢��·�������ص������·��
        /// </summary>
        /// <param name="workPath">����·��</param>
        /// <param name="Name">��������</param>
        /// <returns></returns>
        public string GetTaskClassPathByName(string Name)
        {
            if (Name == NetMiner.Constant.g_RemoteTaskClass)
            {
                return m_workPath + NetMiner.Constant.g_RemoteTaskPath;
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
                return  "tasks";
            else
                return  et.tPath;
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
            //�ֽ�ɼ�����
            string[] tClasses = TaskClassName.Split('/');
            string tPath = string.Empty;
            string parentPath = string.Empty;
            parentPath= tPath = "tasks";
           
            string rMaxID = string.Empty;
            //�𼶵���
            for (int i =0;i<tClasses.Length;i++)
            {
                tPath += "\\" + tClasses[i];


                bool isExist= base.isExist("TaskClass", "Path",tPath);

                if (isExist==false)
                {
                    //��ʼ����Ŀ¼
                    //��ȡ��һ���ڵ㣬�ҵ����ڵ�
                    XElement xe = null;
                    int MaxID = -1;
                    string nodeName = string.Empty;
                    if (i == 0)
                    {
                        //��һ�����������
                        MaxID = base.GetMaxID(base.xDoc.Root.Element("TaskClasses"), "TaskClass");
                        nodeName = "TaskClasses";
                        xe = base.xDoc.Root;
                    }
                    else
                    {
                        xe = base.SearchElement("TaskClass", "Path", parentPath);
                        if (xe.Element("Children") == null)
                        {
                            MaxID = int.Parse (rMaxID +  "00");
                        }
                        else
                            //��ȡ���ID
                            MaxID = base.GetMaxID(xe.Element("Children"), "TaskClass");
                        nodeName = "Children";
                    }

                    rMaxID= AddTaskClass( xe, nodeName,tClasses[i],tPath,MaxID);
                }
                else
                {
                    rMaxID +=base.SearchElement("TaskClass", "Path", tPath).Element("ID").Value.ToString();
                }

                parentPath += "\\" + tClasses[i];
            }

            return int.Parse (rMaxID);

        }

        private string AddTaskClass(XElement parentXE, string nodeName, string ClassName,string classPath, int MaxID)
        {
            MaxID = MaxID + 1;
            string mID =MaxID.ToString().Length == 1 ? "0" + MaxID.ToString() : MaxID.ToString();

            //mID = rMaxID + mID;

            XElement xe = new XElement("TaskClass");
            xe.Add(new XElement("ID", mID));
            xe.Add(new XElement("Name", ClassName));
            xe.Add(new XElement("Path", classPath));

            XElement pXE = parentXE.Element(nodeName);

            if (pXE==null)
            {
                //�����ǵ�һ���ӽڵ�
                base.AddElement(parentXE, new XElement("Children", xe));
            }
            else
            {
                base.AddElement(pXE, xe);
            }
            
            //��ʼ����Ŀ¼
            classPath = this.m_workPath + classPath;
            if (!Directory.Exists(classPath))
            {
                Directory.CreateDirectory(classPath);
            }

            return mID;

        }
       

        /// <summary>
        /// ɾ��ָ���ķ����ļ���������������Ƕ༶����/�ָ�
        /// </summary>
        /// <param name="TClassName"></param>
        /// <returns></returns>
        public bool DelTaskClass(string tClassID)
        {
            try
            {
                XElement xe = base.SearchElement("TaskClass", "ID", tClassID);
                string classPath = xe.Element("Path").Value.ToString();
                base.RemoveElement(xe);

                classPath = this.m_workPath + classPath;
                System.IO.Directory.Delete(classPath, true);
            }
            catch { return false; }
            return true;
        }

        //���������������������������½�һ�����࣬����ԭ�з��������
        //Ǩ�ƹ��������޸�������Ϣ�����񣬲�ɾ��ԭ������
        

        public void EditTaskClassPath( List<eTaskClass> ets ,string tPath)
        {
            for (int i=0;i<ets.Count;i++)
            {
                string path = tPath + "\\" + ets[i].Name;
                XElement xeChild = base.SearchElement("TaskClass", "ID", ets[i].ID.ToString());
                xeChild.Element("Path").Value = path;
                //xmlConfig.EditNodeValue(nodeList, "ID", ets[i].ID.ToString(), "Path", path);

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    EditTaskClassPath( ets[i].Children, path);
                }
            }
        }

        public eTaskClass FindTaskClass(List<eTaskClass> ets, int ID)
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
