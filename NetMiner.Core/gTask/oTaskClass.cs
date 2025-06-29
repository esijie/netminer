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

///功能：采集任务类别 管理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：无
namespace NetMiner.Core.gTask
{
    public class oTaskClass:XmlUnity
    {
        private string m_workPath = string.Empty;
        

        /// <summary>
        /// 会同步加载数据
        /// </summary>
        /// <param name="tPath"></param>
        public oTaskClass(string workPath)
        {
            m_workPath = workPath;
            TaskClasses = new List<eTaskClass>();

            string className = workPath + NetMiner.Constant.g_TaskClassFile;
            if (!File.Exists(className))
            {
                //自动创建一个分类索引文件
                NewClassFile(className);
            }

            base.LoadXML(className);

            //先取出第一层的元素来
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

        #region 属性
        //定义一个集合类
      

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
        /// 传入的路径都必须为相对路径
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


            //判断此节点下是否还有子节点并开始修改
            if (xe.Element("Children")!=null)
            {
                //表示有子节点
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

        //根据指定的类别ID取回类别名称，是一个完整的分类路径
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

        //根据指定的ID返回TaskClassPath
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
        /// 根据指定的Task分类名称返回任务分类所存储的路径，返回的是相对路径
        /// </summary>
        /// <param name="workPath">工作路径</param>
        /// <param name="Name">分类名称</param>
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

                //一级目录匹配上才会进入子级
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

        

        //判断任务分类是否存在
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TaskClassName">如果是多个分类请用/分割</param>
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

            //先比较顶级目录
            for (int i = 0; i < TaskClasses.Count; i++)
            {

                if (TaskClasses[i].Name == taskClasses[level])
                {
                    if(taskClasses.Length>1)
                    {
                        //表示有子级目录
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

        //增加分类节点，如果添加成功，则返回添加成功后的分类节点ID
        //系统中存储的路径全部都是相对路径，不允许存储绝对路径
        //系统参数是绝对路径，绝对路径到相对路径的转换在方法在内部完成
        //系统对外看都是绝对路径
        public int AddTaskClass(string TaskClassName)
        {
            //分解采集分类
            string[] tClasses = TaskClassName.Split('/');
            string tPath = string.Empty;
            string parentPath = string.Empty;
            parentPath= tPath = "tasks";
           
            string rMaxID = string.Empty;
            //逐级递深
            for (int i =0;i<tClasses.Length;i++)
            {
                tPath += "\\" + tClasses[i];


                bool isExist= base.isExist("TaskClass", "Path",tPath);

                if (isExist==false)
                {
                    //开始建立目录
                    //获取上一级节点，找到父节点
                    XElement xe = null;
                    int MaxID = -1;
                    string nodeName = string.Empty;
                    if (i == 0)
                    {
                        //第一层分类的最大编号
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
                            //获取最大ID
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
                //表明是第一个子节点
                base.AddElement(parentXE, new XElement("Children", xe));
            }
            else
            {
                base.AddElement(pXE, xe);
            }
            
            //开始建立目录
            classPath = this.m_workPath + classPath;
            if (!Directory.Exists(classPath))
            {
                Directory.CreateDirectory(classPath);
            }

            return mID;

        }
       

        /// <summary>
        /// 删除指定的分类文件，分类名称如果是多级请用/分割
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

        //任务分类改名，任务分类改名就是新建一个分类，并把原有分类的任务都
        //迁移过来，并修改任务信息的任务，并删除原有内容
        

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
