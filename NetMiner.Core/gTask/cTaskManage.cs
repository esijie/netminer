using System;
using System.Collections.Generic;
using System.Linq;
using NetMiner.Core.gTask;
using NetMiner.Core.gTask.Entity;
using System.Text;
using System.IO;
using NetMiner.Common;
using NetMiner.Resource;

/// <summary>
/// 用于管理采集任务、队列等之间的关系
/// </summary>
namespace NetMiner.Core.gTask
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
        /// 修改采集任务分类名称，路径为相对路径，名称为完整的名称
        /// </summary>
        /// <param name="TClassName"></param>
        /// <param name="NewTClassName"></param>
        /// <returns></returns>
        public bool RenameTaskClass(string TClassName,string oldPath, string NewTClassName)
        {
            string nName = string.Empty;
            nName = NewTClassName;

            string newPath = oldPath.Replace(TClassName, NewTClassName);
            oTaskClass tc = new oTaskClass(this.m_workPath);

            try
            {
                //判断新的任务路径是否存在，如果存在则报错
                if (Directory.Exists(this.m_workPath + newPath))
                    throw new NetMinerException("新任务分类的路径已经存在，请重新修改任务分类名称！");

                
                tc.RenameTaskClass(oldPath, TClassName,newPath,  NewTClassName);

                //开始将修改任务分类的实际路径
                File.SetAttributes(this.m_workPath + oldPath, System.IO.FileAttributes.Normal);
                Directory.Move(this.m_workPath + oldPath, this.m_workPath + newPath);
                if (Directory.Exists(this.m_workPath + oldPath))
                    Directory.Delete(this.m_workPath + oldPath);
            }
            catch(System.Exception ex)
            {
                throw ex;
            }
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
        /// <param name="TaskClassPath">任务所属分类的路径，相对地址</param>
        /// <param name="OldTaskName"></param>
        /// <param name="NewTaskName"></param>
        /// <returns></returns>
        public bool RenameTask(string TaskClassPath, string OldTaskName, string NewTaskName)
        {
            try
            {
            
                //判断新的任务路径是否存在，如果存在则报错
                if (TaskClassPath == "")
                {
                    TaskClassPath = this.m_workPath + "tasks";
                }
                else
                {
                    TaskClassPath = this.m_workPath + TaskClassPath;
                }


                if (File.Exists(TaskClassPath + "\\" + NewTaskName + ".smt"))
                    throw new NetMinerException("您修改的任务名称已经存在，请重新修改！");

                oTaskIndex tIndex = new oTaskIndex(this.m_workPath, TaskClassPath + "\\index.xml");
                try
                {
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
                t.LoadTask(TaskClassPath + "\\" + OldTaskName + ".smt");
                t.TaskEntity.TaskName = NewTaskName;
                t.SaveTask(TaskClassPath + "\\" + OldTaskName + ".smt");
                t.Dispose();
                t = null;

                File.Copy(TaskClassPath + "\\" + OldTaskName + ".smt", TaskClassPath + "\\" + NewTaskName + ".smt");
                File.Delete(TaskClassPath + "\\" + OldTaskName + ".smt");

            }
            catch (System.Exception ex)
            {
                throw ex;
                //return false;
            }

            return true;
        }

        /// <summary>
        /// 采集任务移动，系统会自动维护Index文件，传入相对路径，返回的是绝对路径
        /// </summary>
        public string CopyTask(string oldTaskClassPath, string newTaskClassPath, string TaskName, NetMiner.Resource.cGlobalParas.CopyType copyType)
        {
            if (oldTaskClassPath == newTaskClassPath && copyType==cGlobalParas.CopyType.Move)
                return "";

            oldTaskClassPath = this.m_workPath + oldTaskClassPath;
            newTaskClassPath = this.m_workPath + newTaskClassPath;

            string newFileName = newTaskClassPath + "\\" + TaskName + ".smt";

            oTaskIndex ti = new oTaskIndex(this.m_workPath, oldTaskClassPath + "\\index.xml");
            eTaskIndex ei = null;

            try
            {
                //判断新目录下是否存在此任务
                if (copyType == cGlobalParas.CopyType.Move)
                {
                    if (File.Exists(newFileName))
                        throw new NetMinerException("此目录下已经存在同名的采集任务，无法移动！");

                    ei = ti.DeleTaskIndex(TaskName);

                }
                else if (copyType == cGlobalParas.CopyType.Copy)
                {
                    if (File.Exists(newFileName))
                    {
                        newFileName = newTaskClassPath + "\\" + TaskName + "-复制.smt";
                    }

                    ei = ti.GetTaskIndex(TaskName);

                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                ti.Dispose();
                ti = null;
            }
            
            try
            {
                if (copyType == cGlobalParas.CopyType.Move)
                {
                    System.IO.File.Move(oldTaskClassPath + "\\" + TaskName + ".smt", newFileName );
                }
                else if (copyType == cGlobalParas.CopyType.Copy)
                {
                    try
                    {
                        System.IO.File.Copy(oldTaskClassPath + "\\" + TaskName + ".smt", newFileName, false);
                    }
                    catch(System.IO.IOException ex) 
                    {
                        throw new NetMinerException("有可能已经存在此采集任务，无法覆盖！");
                    }
                    //当拷贝过去任务的时候，有可能发生任务名称变化，则需要修改任务名称
                    if (Path.GetFileNameWithoutExtension(oldTaskClassPath + "\\" + TaskName + ".smt")!=
                        Path.GetFileNameWithoutExtension( newFileName))
                    {
                        //修改任务的名称
                        oTask t = new oTask(this.m_workPath);
                        t.LoadTask( newFileName);
                        t.TaskEntity.TaskName = Path.GetFileNameWithoutExtension(newTaskClassPath + "\\" + newFileName);
                        t.SaveTask( newFileName);
                        t.Dispose();
                        t = null;
                    }

                }

                ti = new oTaskIndex(this.m_workPath, newTaskClassPath + "\\index.xml");
                ei.TaskName = Path.GetFileNameWithoutExtension( newFileName);
                ti.InsertTaskIndex(ei);
               

            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (ti != null)
                {
                    ti.Dispose();
                    ti = null;
                }
            }

            return  newFileName;

        }


        
    }
}
