using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

///功能：任务升级操作类，当前支持的任务版本时1.61 任务版本升级仅限于前一个版本，如果版本
/// 进行跳跃式升级，可能会发生错误
///完成时间：2009-8-30
///作者：一孑
///遗留问题：无
///开发计划：下一版字典部分要强化
///说明：
///版本：01.60.00
///修订：无
///
///修订 2010-04-15  任务版本升级为：1.8

namespace SoukeyNetget.Task
{
    class cUpgradeTask
    {
        public cUpgradeTask()
        {
        }

        ~cUpgradeTask()
        {

        }

        private Single m_SupportTaskVersion = Single.Parse("1.8");

        //此类别可处理的任务版本号，注意从1.3开始，任务处理类不再向前兼容
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        //指定任务进行升级操作
        /// <summary>
        /// 升级任务操作
        /// </summary>
        /// <param name="FileName">待升级的任务文件</param>
        /// <param name="IsBackup">是否备份</param>
        /// <param name="IsTask">是否为系统任务，如果是系统任务则需要维护分类信息，如果不是就只保存文件</param>
        public void UpdradeTask(string FileName ,bool IsBackup,bool IsSystemTask)
        {
            //判断是否进行备份
            if (IsBackup == true)
            {
                if (File.Exists(FileName + ".bak"))
                    File.Delete(FileName + ".bak");

                File.Copy(FileName, FileName + ".bak");
            }


            //加载任务文件
            cXmlIO Old_Task = new cXmlIO(FileName);
            Single TaskVersion =Single.Parse ("0");

            cTask t = new cTask();

            //判断任务版本号
            try
            {
                TaskVersion = Single.Parse(Old_Task.GetNodeValue("Task/BaseInfo/Version"));
            }
            catch (System.Exception)
            {
                TaskVersion = Single.Parse("1.0");
            }

            if (TaskVersion >= this.SupportTaskVersion)
            {
                Old_Task = null;
                return;
            }

            //开始新建一个任务
            t.New();

            #region 此部分内容是任何版本都存在的
            t.TaskID = Int64.Parse(Old_Task.GetNodeValue("Task/BaseInfo/ID"));
            t.TaskName = Old_Task.GetNodeValue("Task/BaseInfo/Name");
            t.TaskVersion = this.SupportTaskVersion;



            t.TaskDemo = Old_Task.GetNodeValue("Task/BaseInfo/TaskDemo");
            t.TaskClass = Old_Task.GetNodeValue("Task/BaseInfo/Class");
            t.TaskType = Old_Task.GetNodeValue("Task/BaseInfo/Type");
            t.RunType = Old_Task.GetNodeValue("Task/BaseInfo/RunType");

            //因存的是相对路径，所以要加上系统路径
            t.SavePath = Program.getPrjPath() + Old_Task.GetNodeValue("Task/BaseInfo/SavePath");
            t.UrlCount = int.Parse(Old_Task.GetNodeValue("Task/BaseInfo/UrlCount").ToString());
            t.ThreadCount = int.Parse(Old_Task.GetNodeValue("Task/BaseInfo/ThreadCount"));
            t.Cookie = Old_Task.GetNodeValue("Task/BaseInfo/Cookie");
            t.DemoUrl = Old_Task.GetNodeValue("Task/BaseInfo/DemoUrl");
            t.StartPos = Old_Task.GetNodeValue("Task/BaseInfo/StartPos");
            t.EndPos = Old_Task.GetNodeValue("Task/BaseInfo/EndPos");
            t.WebCode = Old_Task.GetNodeValue("Task/BaseInfo/WebCode");
            t.IsLogin = (Old_Task.GetNodeValue("Task/BaseInfo/IsLogin") == "True" ? true : false);
            t.LoginUrl = Old_Task.GetNodeValue("Task/BaseInfo/LoginUrl");
            t.IsUrlEncode = (Old_Task.GetNodeValue("Task/BaseInfo/IsUrlEncode") == "True" ? true : false);
            t.UrlEncode = Old_Task.GetNodeValue("Task/BaseInfo/UrlEncode");
            #endregion



            //从下面的内容开始有可能就出现任务加载错误，
            //导出部分是1.2版本中增加的，高级设置、多层导航及触发器是1.3版本中出现的
            //
            if (Old_Task.GetNodeValue("Task/Result/ExportType") == "")
                t.ExportType = ((int)cGlobalParas.PublishType.PublishTxt).ToString();
            else
                t.ExportType = Old_Task.GetNodeValue("Task/Result/ExportType");

            if (Old_Task.GetNodeValue("Task/Result/ExportFileName") == "")
                t.ExportFile = Program.getPrjPath() + "data\\" + t.TaskName + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".txt";
            else
                t.ExportFile = Old_Task.GetNodeValue("Task/Result/ExportFileName");

            if (Old_Task.GetNodeValue("Task/Result/DataSource") == "")
                t.DataSource = "";
            else
                t.DataSource = Old_Task.GetNodeValue("Task/Result/DataSource");

            if (Old_Task.GetNodeValue("Task/Result/DataTableName") == "")
                t.DataTableName = "";
            else
                t.DataTableName = Old_Task.GetNodeValue("Task/Result/DataTableName");

            if (Old_Task.GetNodeValue("Task/Result/InsertSql") == "")
                t.InsertSql = "";
            else
                t.InsertSql = Old_Task.GetNodeValue("Task/Result/InsertSql");

            if (Old_Task.GetNodeValue("Task/Result/ExportUrl") == "")
                t.ExportUrl = "";
            else
                t.ExportUrl = Old_Task.GetNodeValue("Task/Result/ExportUrl");

            if (Old_Task.GetNodeValue("Task/Result/ExportUrlCode") == "")
                t.ExportUrlCode = "";
            else
                t.ExportUrlCode = Old_Task.GetNodeValue("Task/Result/ExportUrlCode");

            if (Old_Task.GetNodeValue("Task/Result/ExportCookie") == "")
                t.ExportCookie = "";
            else
                t.ExportCookie = Old_Task.GetNodeValue("Task/Result/ExportCookie");

            //此部分是1.3版本中存在的，需要升级
            //加载高级配置信息
            if (Old_Task.GetNodeValue("Task/Advance/GatherAgainNumber") == "")
                t.GatherAgainNumber = 3;
            else
                t.GatherAgainNumber = int.Parse(Old_Task.GetNodeValue("Task/Advance/GatherAgainNumber"));

            if (Old_Task.GetNodeValue("Task/Advance/IsIgnore404") == "")
                t.IsIgnore404 = true;
            else
                t.IsIgnore404 = (Old_Task.GetNodeValue("Task/Advance/IsIgnore404") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsErrorLog") == "")
                t.IsErrorLog = false;
            else
                t.IsErrorLog = (Old_Task.GetNodeValue("Task/Advance/IsErrorLog") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsDelRepeatRow") == "")
                t.IsDelRepRow = false;
            else
                t.IsDelRepRow = (Old_Task.GetNodeValue("Task/Advance/IsDelRepeatRow") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsExportHeader") == "")
                t.IsExportHeader = true;
            else
                t.IsExportHeader = (Old_Task.GetNodeValue("Task/Advance/IsExportHeader") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/IsTrigger") == "")
                t.IsTrigger = false;
            else
                t.IsTrigger = (Old_Task.GetNodeValue("Task/Advance/IsTrigger") == "True" ? true : false);

            if (Old_Task.GetNodeValue("Task/Advance/TriggerType")=="")
                t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString();
            else
                t.TriggerType = Old_Task.GetNodeValue("Task/Advance/TriggerType");

            //以下内容是任务版本1.6中存在的,需补充
            t.IsDelTempData = false;
            t.IsSaveSingleFile = false;
            t.TempDataFile = "";
            t.IsDataProcess = false;
            t.IsExportGUrl = false;
            
            //此为1.62增加
            t.IsExportGDateTime = false;

            //此为1.63增加
            t.GIntervalTime = 0;

            //此为1.8版本增加
            t.IsCustomHeader = false;
            t.IsPublishHeader = false;

            DataView dw = new DataView();
            int i;

            dw = Old_Task.GetData("descendant::WebLinks");
            cWebLink w;

            DataView dn;

            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    w = new cWebLink();
                    w.id = i;
                    w.Weblink = dw[i].Row["Url"].ToString();
                    if (dw[i].Row["IsNag"].ToString() == "True")
                        w.IsNavigation = true;
                    else
                        w.IsNavigation = false;

                    if (dw[i].Row["IsNextPage"].ToString() == "True")
                        w.IsNextpage = true;
                    else
                        w.IsNextpage = false;

                    w.NextPageRule = dw[i].Row["NextPageRule"].ToString();
                    w.NextPageUrl = "";
                    w.IsGathered = int.Parse((dw[i].Row["IsGathered"].ToString() == null || dw[i].Row["IsGathered"].ToString() == "") ? "2031" : dw[i].Row["IsGathered"].ToString());

                    if (dw[i].Row["IsNag"].ToString() == "True")
                    {

                        //在此处理导航的升级操作，如果以前版本中存在导航规则，则只是一层导航
                        try
                        {
                            cNavigRule nRule = new cNavigRule();
                            nRule.Url = dw[i].Row["Url"].ToString();
                            nRule.Level = 1;
                            nRule.NavigRule = dw[i].Row["NagRule"].ToString();

                            nRule.IsNaviNextPage = false;
                            nRule.NaviNextPage = "";

                            nRule.NaviStartPos = "";
                            nRule.NaviEndPos = "";

                            //1.63增加的内容
                            nRule.IsGather = false;
                            nRule.GatherStartPos = "";
                            nRule.GatherEndPos = "";

                            w.NavigRules.Add(nRule);
                        }
                        catch
                        {
                            //注意：从任务1.6 开始 NavigationRules_Rule 改为 NavigationRules_NavigationRule
                            try
                            {
                                dn = dw[i].CreateChildView("WebLink_NavigationRules")[0].CreateChildView("NavigationRules_Rule");
                            }
                            catch (System.Exception)
                            {
                                dn = dw[i].CreateChildView("WebLink_NavigationRules")[0].CreateChildView("NavigationRules_NavigationRule");
                            }
                            cNavigRule nRule;

                            for (int m = 0; m < dn.Count; m++)
                            {
                                nRule = new cNavigRule();
                                nRule.Url = dn[m].Row["Url"].ToString();
                                nRule.Level = int.Parse(dn[m].Row["Level"].ToString());
                                try
                                {
                                    nRule.IsNaviNextPage = (dn[m].Row["IsNaviNextPage"].ToString() == "True" ? true : false);
                                    nRule.NaviNextPage = dn[m].Row["NaviNextPage"].ToString();
                                }
                                catch
                                {
                                    nRule.IsNaviNextPage = false;
                                    nRule.NaviNextPage = "";
                                }
                                nRule.NavigRule = dn[m].Row["NagRule"].ToString();

                                //此为1.62增加
                                nRule.NaviStartPos = "";
                                nRule.NaviEndPos = "";

                                //1.63增加的内容
                                nRule.IsGather = false;
                                nRule.GatherStartPos = "";
                                nRule.GatherEndPos = "";

                                w.NavigRules.Add(nRule);
                            }
                        }
                        dn = null;

                    }
                    else
                    {
                    }

                    t.WebpageLink.Add(w);
                    w = null;
                }
            }

            dw = null;
            dw = new DataView();
            try
            {
                dw = Old_Task.GetData("descendant::GatherRules");
                Task.cWebpageCutFlag c;
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        c = new Task.cWebpageCutFlag();
                        c.Title = dw[i].Row["Title"].ToString();
                        c.DataType = int.Parse((dw[i].Row["DataType"].ToString() == null || dw[i].Row["DataType"].ToString() == "") ? "0" : dw[i].Row["DataType"].ToString());
                        c.StartPos = dw[i].Row["StartFlag"].ToString();
                        c.EndPos = dw[i].Row["EndFlag"].ToString();
                        c.LimitSign = int.Parse((dw[i].Row["LimitSign"].ToString() == null || dw[i].Row["LimitSign"].ToString() == "") ? "0" : dw[i].Row["LimitSign"].ToString());

                        //处理版本不同时造成的错误，捕获不处理

                        c.RegionExpression = dw[i].Row["RegionExpression"].ToString();

                        try
                        {
                            //1.6版本以前的导出规则是一条，所以可以用这种方式进行，及仅建立一个导航规则
                            cFieldRules cfs = new cFieldRules();
                            cFieldRule cf = new cFieldRule();
                            cf.Field = dw[i].Row["Title"].ToString();
                            cf.FieldRuleType = int.Parse((dw[i].Row["ExportLimit"].ToString() == null || dw[i].Row["ExportLimit"].ToString() == "") ? "0" : dw[i].Row["ExportLimit"].ToString());
                            cf.FieldRule = dw[i].Row["ExportExpression"].ToString();

                            cfs.Field = dw[i].Row["Title"].ToString();
                            cfs.FieldRule.Add(cf);

                        }
                        catch (System.Exception)
                        {
                            //标识已经支持了多条导出规则
                            //加载数据输出规则
                            if (dw[i].DataView.Table.ChildRelations.Count != 0)
                            {
                                dn = dw[i].CreateChildView("GatherRule_ExportRules")[0].CreateChildView("ExportRules_ExportRule");

                                cFieldRule fRule;

                                for (int n = 0; n < dn.Count; n++)
                                {
                                    fRule = new cFieldRule();
                                    fRule.Field = dn[n].Row["ExortField"].ToString();
                                    fRule.FieldRuleType = int.Parse(dn[n].Row["ExortRuleType"].ToString());
                                    fRule.FieldRule = dn[n].Row["ExortRuleCondition"].ToString();

                                    c.ExportRules.Add(fRule);
                                }

                            }

                        }

                        c.IsMergeData = false;
                        c.NavLevel = 0;

                        t.WebpageCutFlag.Add(c);
                        c = null;
                    }
                }
            }
            catch (System.Exception)
            {
                //标识采集规则失败，有可能是由于没有配置采集规则所导致，但需要忽略此错误

            }
            dw = null;

            Old_Task = null;


            //获取此文件的目录传入
            string FilePath = Path.GetDirectoryName(FileName);

            if (IsSystemTask == true)
            {
                t.DeleTask(FilePath, t.TaskName);

                t.Save(FilePath + "\\");
            }
            else
            {
                t.SaveTaskFile(FilePath + "\\");
            }
            t = null;

        }

    }
}
