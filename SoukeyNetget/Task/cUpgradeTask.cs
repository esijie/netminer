using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

///���ܣ��������������࣬��ǰ֧�ֵ�����汾ʱ1.61 ����汾����������ǰһ���汾������汾
/// ������Ծʽ���������ܻᷢ������
///���ʱ�䣺2009-8-30
///���ߣ�һ��
///�������⣺��
///�����ƻ�����һ���ֵ䲿��Ҫǿ��
///˵����
///�汾��01.60.00
///�޶�����
///
///�޶� 2010-04-15  ����汾����Ϊ��1.8

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

        //�����ɴ��������汾�ţ�ע���1.3��ʼ���������಻����ǰ����
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        //ָ�����������������
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="FileName">�������������ļ�</param>
        /// <param name="IsBackup">�Ƿ񱸷�</param>
        /// <param name="IsTask">�Ƿ�Ϊϵͳ���������ϵͳ��������Ҫά��������Ϣ��������Ǿ�ֻ�����ļ�</param>
        public void UpdradeTask(string FileName ,bool IsBackup,bool IsSystemTask)
        {
            //�ж��Ƿ���б���
            if (IsBackup == true)
            {
                if (File.Exists(FileName + ".bak"))
                    File.Delete(FileName + ".bak");

                File.Copy(FileName, FileName + ".bak");
            }


            //���������ļ�
            cXmlIO Old_Task = new cXmlIO(FileName);
            Single TaskVersion =Single.Parse ("0");

            cTask t = new cTask();

            //�ж�����汾��
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

            //��ʼ�½�һ������
            t.New();

            #region �˲����������κΰ汾�����ڵ�
            t.TaskID = Int64.Parse(Old_Task.GetNodeValue("Task/BaseInfo/ID"));
            t.TaskName = Old_Task.GetNodeValue("Task/BaseInfo/Name");
            t.TaskVersion = this.SupportTaskVersion;



            t.TaskDemo = Old_Task.GetNodeValue("Task/BaseInfo/TaskDemo");
            t.TaskClass = Old_Task.GetNodeValue("Task/BaseInfo/Class");
            t.TaskType = Old_Task.GetNodeValue("Task/BaseInfo/Type");
            t.RunType = Old_Task.GetNodeValue("Task/BaseInfo/RunType");

            //���������·��������Ҫ����ϵͳ·��
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



            //����������ݿ�ʼ�п��ܾͳ���������ش���
            //����������1.2�汾�����ӵģ��߼����á���㵼������������1.3�汾�г��ֵ�
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

            //�˲�����1.3�汾�д��ڵģ���Ҫ����
            //���ظ߼�������Ϣ
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

            //��������������汾1.6�д��ڵ�,�貹��
            t.IsDelTempData = false;
            t.IsSaveSingleFile = false;
            t.TempDataFile = "";
            t.IsDataProcess = false;
            t.IsExportGUrl = false;
            
            //��Ϊ1.62����
            t.IsExportGDateTime = false;

            //��Ϊ1.63����
            t.GIntervalTime = 0;

            //��Ϊ1.8�汾����
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

                        //�ڴ˴����������������������ǰ�汾�д��ڵ���������ֻ��һ�㵼��
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

                            //1.63���ӵ�����
                            nRule.IsGather = false;
                            nRule.GatherStartPos = "";
                            nRule.GatherEndPos = "";

                            w.NavigRules.Add(nRule);
                        }
                        catch
                        {
                            //ע�⣺������1.6 ��ʼ NavigationRules_Rule ��Ϊ NavigationRules_NavigationRule
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

                                //��Ϊ1.62����
                                nRule.NaviStartPos = "";
                                nRule.NaviEndPos = "";

                                //1.63���ӵ�����
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

                        //����汾��ͬʱ��ɵĴ��󣬲��񲻴���

                        c.RegionExpression = dw[i].Row["RegionExpression"].ToString();

                        try
                        {
                            //1.6�汾��ǰ�ĵ���������һ�������Կ��������ַ�ʽ���У���������һ����������
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
                            //��ʶ�Ѿ�֧���˶�����������
                            //���������������
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
                //��ʶ�ɼ�����ʧ�ܣ��п���������û�����òɼ����������£�����Ҫ���Դ˴���

            }
            dw = null;

            Old_Task = null;


            //��ȡ���ļ���Ŀ¼����
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
