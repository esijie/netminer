using System;
using System.Collections.Generic;
using System.Linq;
using NetMiner.Core.Task;
using NetMiner.Core.Task.Entity;
using System.Text;
using System.IO;
using NetMiner.Common;


/// <summary>
/// 用于管理采集任务、队列等之间的关系
/// </summary>
namespace NetMiner.Gather.Task
{
    public class cTaskManage
    {
        private string m_workPath;
        public cTaskManage(string workPath)
        {
            this.m_workPath = workPath;
        }

        ~cTaskManage()
        {

        }

        /// <summary>
        /// 修改采集任务分类名称
        /// </summary>
        /// <param name="TClassName"></param>
        /// <param name="NewTClassName"></param>
        /// <returns></returns>
        public bool RenameTaskClass(string TClassName, string NewTClassName)
        {
           
            string nName = string.Empty;
            nName = NewTClassName;

            oTaskClass tc = new Core.Task.oTaskClass(this.m_workPath);

            try
            {
                string OldPath = tc.GetTaskClassPathByName(TClassName);


                string NewPath = OldPath.Substring(0, OldPath.LastIndexOf("\\")) + "\\" + NewTClassName;

                if (TClassName.IndexOf('/') > 0)
                    NewTClassName = TClassName.Substring(0, TClassName.LastIndexOf("/")) + "/" + NewTClassName;

                int OldTaskClassID = 0;

                //判断新的任务路径是否存在，如果存在则报错
                if (Directory.Exists(NewPath))

                    throw new NetMinerException("新任务分类的路径已经存在，请重新修改任务分类名称！");

                //转换相对路径
                string NewRelativePath = cTool.GetRelativePath(m_workPath, NewPath);

                bool isE = tc.IsExist(NewTClassName);
                if (isE)
                    throw new NetMinerException("任务分类已经存在！");

                //获取原有分类的ID
                OldTaskClassID = tc.GetTaskClassIDByName(TClassName);


                if (OldTaskClassID == 0)
                {
                    throw new NetMinerException("未能找到需要修改分类的信息，名称修改失败！");
                }

                tc.RenameTaskClass(OldPath, TClassName, NewTClassName, NewRelativePath);

                //开始将修改任务分类的实际路径
                File.SetAttributes(OldPath, System.IO.FileAttributes.Normal);
                Directory.Move(OldPath, NewPath);
                if (Directory.Exists(OldPath))
                    Directory.Delete(OldPath);
            }
            catch { }
            finally
            {
                tc.Dispose();
                tc = null;
            }

            return true;
        }

        /// <summary>
        /// 修改采集任务的名称
        /// </summary>
        /// <param name="TClass"></param>
        /// <param name="OldTaskName"></param>
        /// <param name="NewTaskName"></param>
        /// <returns></returns>
        public bool RenameTask(string TClass, string OldTaskName, string NewTaskName)
        {
            try
            {
                //根据任务分类获取任务的所属路径
                oTaskClass tc = new oTaskClass(this.m_workPath);
                string tClassPath = "";

                //判断新的任务路径是否存在，如果存在则报错
                if (TClass == "")
                {
                    tClassPath = this.m_workPath + "tasks";
                }
                else
                {
                    tClassPath = tc.GetTaskClassPathByName(TClass);
                }

                tc = null;

                if (File.Exists(tClassPath + "\\" + NewTaskName + ".smt"))
                    throw new NetMinerException("您修改的任务名称已经存在，请重新修改！");

                oTaskIndex tIndex = new oTaskIndex(this.m_workPath);
                try
                {
                    tIndex.LoadIndexDocument(TClass);
                    bool isExistTask = tIndex.isExistTask(NewTaskName);

                    if (isExistTask)
                    {
                        throw new NetMinerException("您修改的任务名称已经存在，请重新修改！");
                    }

                    tIndex.EditIndexTaskName(OldTaskName, NewTaskName);

                }
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    tIndex.Dispose();
                    tIndex = null;
                }

                oTask t = new oTask(this.m_workPath);
                t.LoadTask(tClassPath + "\\index.xml");
                t.EditTaskName(NewTaskName);
                t.Dispose();
                t = null;

                File.Copy(tClassPath + "\\" + OldTaskName + ".smt", tClassPath + "\\" + NewTaskName + ".smt");
                File.Delete(tClassPath + "\\" + OldTaskName + ".smt");

            }
            catch (System.Exception ex)
            {
                throw ex;
                //return false;
            }

            return true;
        }

        /// <summary>
        /// 向正在运行队列中插入一个任务
        /// </summary>
        /// <param name="workPath"></param>
        /// <param name="Path"></param>
        /// <param name="File"></param>
        /// <returns></returns>
        public long InsertTaskRun(string workPath, string Path, string File)
        {

        }
    }
}
