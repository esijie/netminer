using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using NetMiner.Core.Proxy;
using NetMiner.Common;
using NetMiner.Resource;
using NetMiner.Core.gTask;
using NetMiner.Core;
using NetMiner.Core.Event;
using NetMiner.Core.Entity;
using NetMiner.Core.Radar;
using NetMiner.Core.Radar.Entity;
using NetMiner.Gather.Control;
using NetMiner.Net.Common;
using NetMiner.Net;

namespace NetMiner.Gather.Radar
{
    public class cRadarManage
    {
        //��ʼ��Ĭ���ִ�
        private int m_Round = 0;

        //����һ��ֵ���жϵ�ǰ�Ƿ������״�
        private bool m_IsStarted = false;

        //����һ��ֵ���жϵ�ǰ�Ƿ��������м������
        private bool m_IsRunning = false;
        //�Ƿ���вɼ�
        private bool m_IsGathering = false;

        //����һ����ʱ���������״����ѯ����
        private System.Threading.Timer m_RadarEngine;

        //��ʼ���ɹ���־
        private bool m_IsInitialized = false;

        //����һ���״���Ϣ�ļ���
        private List<eRadar> m_RadarList;

        //����һ���ɼ�����������������״�����Ĳɼ�����
        private cGatherControl m_GatherControl;

        //��¼��ǰ��Ҫ������ݵĹ���
        private eRadar m_CurrentRadar;

        //��¼��ǰ�������е��״���
        private int m_CurrentRadarGatherIndex = 0;

        //����һ��ֵ��ʾ�Ƿ�洢��־�¼�
        private bool m_IsErrorLog = false;

        //����һ��ֵ�ж��Ƿ����±�
        private bool IsNewTable = true;

        //����һ��ֵ�����м�ؼ�������ã�ϵͳĬ��Ϊ10����
        private int m_MonitorInterval=10;

        private int m_LastTime;

        //����һ��ֵ����ʾ��¼��ǰ�Ĺ���·��
        private string m_workPath = string.Empty;

        public cRadarManage(string workPath)
        {

            this.m_workPath = workPath;

            //��ʼ���ɼ���������������Ӧ���¼�
            //��ʼ��һ���ɼ�����Ŀ�����,�ɼ������ɴ˿�����������ɼ�����
            //����
            NetMiner.Base.cHashTree tmpUrls = null;
            m_GatherControl = new cGatherControl(m_workPath,false,ref tmpUrls);

            //�ɼ��������¼���,�󶨺�,ҳ�������Ӧ�ɼ����������¼�
            m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized += tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged += tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError += tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted += tManage_TaskAbort;
            m_GatherControl.TaskManage.Log += tManage_Log;
            m_GatherControl.TaskManage.GData += tManage_GData;
            m_GatherControl.TaskManage.GUrlCount += tManage_GUrlCount;
            m_GatherControl.Completed += m_Gather_Completed;

            //�������ļ���ȡ��ص�ʱ����
            cXmlSConfig cs = new cXmlSConfig(this.m_workPath);
            m_MonitorInterval = cs.MonitorInterval;
            cGlobalParas.DatabaseType dtype = (cGlobalParas.DatabaseType)cs.DataType;
            this.DatabaseType=dtype;
            this.dbCon = cs.DataConnection; 
            cs = null;

            //ת���ɺ��뵥λ
            m_MonitorInterval = m_MonitorInterval * 60000;
            
            //��ʼ����ʱ��
            timerInit();

        
        }

        ~cRadarManage()
        {
            m_GatherControl.TaskManage.TaskCompleted -= tManage_Completed;
            m_GatherControl.TaskManage.TaskStarted -= tManage_TaskStart;
            m_GatherControl.TaskManage.TaskInitialized -= tManage_TaskInitialized;
            m_GatherControl.TaskManage.TaskStateChanged -= tManage_TaskStateChanged;
            m_GatherControl.TaskManage.TaskStopped -= tManage_TaskStop;
            m_GatherControl.TaskManage.TaskError -= tManage_TaskError;
            m_GatherControl.TaskManage.TaskFailed -= tManage_TaskFailed;
            m_GatherControl.TaskManage.TaskAborted -= tManage_TaskAbort;
            m_GatherControl.TaskManage.Log -= tManage_Log;
            m_GatherControl.TaskManage.GData -= tManage_GData;
            m_GatherControl.TaskManage.GUrlCount -= tManage_GUrlCount;
            m_GatherControl = null;

        }

        #region ����
        private cGlobalParas.DatabaseType m_DatabaseType;
        public cGlobalParas.DatabaseType DatabaseType
        {
            get { return m_DatabaseType; }
            set { m_DatabaseType = value; }
        }

        private string m_dbCon;
        public string dbCon
        {
            get { return m_dbCon; }
            set { m_dbCon = value; }
        }

        //ֻ�����ԣ��жϵ�ǰ�״��Ƿ�����
        public bool IsStarted
        {
            get { return m_IsStarted; }
        }
        #endregion

        #region ��ʱ��
        /// ��ʼ��timer��ʱ��,��ǰ������Threading.timer��ʱ��
        /// �˼�ʱ������������ѯ��ǰ�״���м������ļ��
        /// 
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_RadarEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_MonitorEngine_CallBack), null, 0, m_MonitorInterval);
            m_IsInitialized = true;
            m_IsRunning = false;
        }

        /// ����ɼ����ݵ���ʾ����־��ʾ
        private void m_MonitorEngine_CallBack(object State)
        {
            if (m_IsStarted==true && m_IsRunning ==false)
            {
                m_Round++;

                m_IsRunning = true;

                //���¼����״�����
                IniRadar();

                //��ʼ�������ݼ�⣬��ǰΪ�˿��Խ��Ͷ�ϵͳ��Դ����ռ
                //�״�������Ĳɼ���˳����У�������һ�����ɼ�������ɺ�
                //�ڽ�����һ���״�ɼ�����
                for (int i = 0; i < this.m_RadarList.Count;i++ )
                {
                    //�״�ÿ��ִ��ǰ����Ҫ�жϴ��״��������Ƿ�ʧЧ
                    //�жϵ�������ʱ�䣬���ʧЧ���޸�״̬����������
                    //������

                    if (this.m_RadarList[i].InvalidType == cGlobalParas.MonitorInvalidType.InvalidByDate && DateTime.Compare(DateTime.Parse ( this.m_RadarList[i].InvalidDate), System.DateTime.Now) > 0)
                    {
                        oRadarIndex ci = new oRadarIndex(this.m_workPath);
                        ci.InvalidRule(this.m_RadarList[i].TaskName);
                        ci = null;

                        if (e_RadarState != null)
                            e_RadarState(this, new cRadarStateArgs(this.m_CurrentRadar.TaskName, cGlobalParas.MonitorState.Invalid));

                    }
                    else
                    {

                        m_CurrentRadar = this.m_RadarList[i];

                        if (e_RadarState != null)
                            e_RadarState(this, new cRadarStateArgs(this.m_CurrentRadar.TaskName, cGlobalParas.MonitorState.Running));

                        try
                        {
                            ExcuteGahterTask(this.m_RadarList[i]);
                        }
                        catch (System.Exception ex)
                        {
                            e_RadarState(this, new cRadarStateArgs(this.m_CurrentRadar.TaskName, cGlobalParas.MonitorState.Stop));
                            e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, ex.Message, this.m_IsErrorLog));
                        }

                        if (this.m_CurrentRadar.MonitorState == cGlobalParas.MonitorState.Invalid )
                        {
                            oRadarIndex ci = new oRadarIndex(m_workPath);
                            ci.InvalidRule(this.m_RadarList[i].TaskName);
                            ci = null;

                            if (e_RadarState != null)
                                e_RadarState(this, new cRadarStateArgs(this.m_CurrentRadar.TaskName, cGlobalParas.MonitorState.Invalid));
                        }

                        if (e_RadarState != null)
                            e_RadarState(this, new cRadarStateArgs(this.m_CurrentRadar.TaskName, cGlobalParas.MonitorState.Stop));
                    }
                }

                m_IsRunning = false;

            }
        }

        ///ִ��ĳ���״�Ĳɼ������п����Ƕ���ɼ�����
        ///����ɼ�����û��ִ����ɣ������������������
        private void ExcuteGahterTask(eRadar r)
        {
            
            //�����״��ع�������Ҫ���еĲɼ������������
            for (int j = 0; j < r.Source.Count; j++)
            {

                //��¼��ǰ�����ڼ���״����Ĳɼ�����
                m_CurrentRadarGatherIndex = j;

                //ÿ�������ɼ�����
                r.Source[j].GatheredCount = 0;
                r.Source[j].Count = 0;

                cGatherTask t = AddRunTask(r.Source[j].TaskClass, r.Source[j].TaskName);

                if (t == null)
                {
                    //��ʾ���زɼ�����ʧ��

                }
                else
                {
                    //����������
                    m_GatherControl.Start(t);

                    m_IsGathering = true;
                }

                while (true)
                {
                    if (m_IsGathering == false)
                        break;
                }

            }

        }

        #endregion

        //��ʼ���״���б���Ϣ
        private void IniRadar()
        {
            m_RadarList = null;
            m_RadarList = new List<eRadar>();

            oRadarIndex ci = new oRadarIndex(this.m_workPath);
            IEnumerable<NetMiner.Core.Radar.Entity.eRadarIndex> ers= ci.GetRules();
            
            foreach(NetMiner.Core.Radar.Entity.eRadarIndex er in ers)
            {

                if (er.State == cGlobalParas.MonitorState.Normal)
                {
                    oRadar or = new oRadar(this.m_workPath);
                    eRadar r= or.LoadRule(er.Name);

                    m_RadarList.Add(r);

                    or.Dispose();
                    or = null;

                }
               
            }
            ci = null;
            
        }

        public void StartRadar()
        {
            this.m_IsStarted = true;

            //���������¼�
            if (e_RadarStarted != null)
                e_RadarStarted(this, new cRadarStartedArgs());
        }

        public void StopRadar()
        {
            this.m_IsStarted = false;


            //ǿ���˳��ɼ�������Ϊ�п��ܴ��ڵ���������
            //����ǵ��������޷�ֱ��ֹͣ�ɼ�����ֻ�ܲ���ǿ��
            //�˳�
            this.m_GatherControl.Abort();

            if (e_RadarStop != null)
                e_RadarStop(this, new cRadarStopArgs());
        }

        /// �¼� �߳�ͬ����
        private readonly Object m_radarEventLock = new Object();

        #region �״�����ɼ�
        //����Ҫ�ɼ���������ص������б�
        private cGatherTask AddRunTask(string tClassName, string tName)
        {

            oTaskClass tc = new oTaskClass(this.m_workPath);
            cTaskData tData = new cTaskData();

            string tPath = "";

            if (tClassName == "")
            {
                tPath = this.m_workPath + "tasks";
            }
            else
            {
                tPath = this.m_workPath + tc.GetTaskClassPathByName(tClassName);
            }

            tc = null;

            string tFileName = tName + ".smt";
            Int64 NewID;

            //��ȡ����ִ��ID
            try
            {
                NewID = Int64.Parse(DateTime.Now.ToFileTime().ToString());
                oTask t = new oTask(this.m_workPath);
                t.LoadTask(tPath + "\\" + tFileName);

                tData = new cTaskData();
                tData.TaskID = NewID;
                tData.TaskName = t.TaskEntity.TaskName;
                tData.TaskClass = tClassName;
                tData.TaskType = t.TaskEntity.TaskType;
                //tData.RunType = tr.GetTaskRunType(0);

                //�ڽ����״����ʱ�����еĲɼ������ǽ��ɼ�����
                tData.RunType = cGlobalParas.TaskRunType.OnlyGather;

                tData.TempDataFile = t.TaskEntity.TempDataFile;
                //tData.TaskState =(cGlobalParas.TaskState) t.TaskState;
                
                //����������������δ��ʼ״̬
                tData.TaskState = cGlobalParas.TaskState.UnStart;

                tData.UrlCount = t.TaskEntity.UrlCount;
                tData.UrlNaviCount = 0;
                tData.ThreadCount = t.TaskEntity.ThreadCount;
                tData.GatheredUrlCount = 0;
                tData.GatherErrUrlCount = 0;
                tData.GatherDataCount = 0;
                tData.GatherTmpCount = 0;
                tData.StartTimer = System.DateTime.Now;


                //�������������
                m_GatherControl.AddMonitorTask(tData, tPath + "\\" + tFileName,true );

                tData = null;

            }
            catch (System.Exception ex)
            {
                return null;
            }

            return m_GatherControl.TaskManage.FindTask(NewID);

        }
        #endregion

        #region �ɼ��������¼�
        private void tManage_TaskStart(object sender, cTaskEventArgs e)
        {
           
        }

        private void tManage_TaskInitialized(object sender, TaskInitializedEventArgs e)
        {
            

        }

        private void tManage_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            if (e.NewState == cGlobalParas.TaskState.Stopped)
            {
                m_IsGathering = false;
            }
        }

        private void tManage_TaskStop(object sender, cTaskEventArgs e)
        {
            m_IsGathering = false;
        }

        private void tManage_Completed(object sender, cTaskEventArgs e)
        {
            m_IsGathering = false;
        }
       
        private void tManage_TaskError(object sender, TaskErrorEventArgs e)
        {
            m_IsGathering = false;
            if (e_Log != null)
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error,  e.Error.Message , this.m_IsErrorLog));
        }

        private void tManage_TaskFailed(object sender, cTaskEventArgs e)
        {
            m_IsGathering = false;
        }

        private void tManage_TaskAbort(object sender, cTaskEventArgs e)
        {
            m_IsGathering = false;
        }

        private void m_Gather_Completed(object sender, EventArgs e)
        {

            m_IsGathering = false;
        }

        private void tManage_Log(object sender, cGatherTaskLogArgs e)
        {
            //if (e_Log != null)
            //    e_Log(this, new cRadarLogArgs(e.LogType, e.strLog , this.m_IsErrorLog));

        }

        /// �¼� �߳�ͬ����
        private readonly Object m_IsUrlLock = new Object();

        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {

            //�ڴ˽������ݹ�����ж�
            bool IsMatch = false;
            
            DataTable d = e.gData;

            string Url=string.Empty ;
            if (d.Columns.Contains ("CollectionUrl"))
                Url = d.Rows[d.Rows.Count - 1]["CollectionUrl"].ToString();
            else
                Url = d.Rows[d.Rows.Count - 1][0].ToString();


                #region  ���ݲɼ������ݽ��й���ȶ�

                //����һ��Ĭ���У���ʾ��ǰ���м�¼�Ƿ���Ϲ���Ĭ���ǲ�����
                //DataColumn dc = new DataColumn("IsExport", System.Type.GetType("System.Boolean"));
                //dc.DefaultValue = false;

                //d.Columns.Add(dc);

                for (int j = 0; j < d.Rows.Count; j++)
                {
                    for (int i = 0; i < this.m_CurrentRadar.MRule.Count; i++)
                    {
                        bool Is = MatchRule(d.Rows[j][this.m_CurrentRadar.MRule[i].Field].ToString(), this.m_CurrentRadar.MRule[i]);
                        if (Is == true)
                        {
                            //d.Rows[j]["IsExport"] = true;
                            IsMatch = true;
                        }
                    }
                }

                //����ȶԹ�����д��ڵ���Ϣ�������ϵͳ����Ԥ��
                if (IsMatch == true)
                {
                    //�����ط��Ϲ�������ݣ��򽫴���ַѹ�����ؿ�
                    bool isExit = false;
                    lock (m_IsUrlLock)
                    {
                        //isExit = m_urlFilter.IsUrl(Url);
                        cRadarUrl rUrl = new cRadarUrl(this.m_workPath, this.m_CurrentRadar.TaskName);
                        isExit = !rUrl.AddUrl(Url);
                        rUrl = null;
                    }

                    if (isExit == true)
                    {
                        //�������ַ�Ѿ����ڣ�������Ԥ��������

                    }
                    else
                    {
                        if (m_CurrentRadar.InvalidType == cGlobalParas.MonitorInvalidType.Invalid)
                        {
                            m_CurrentRadar.MonitorState = cGlobalParas.MonitorState.Invalid;
                        }

                        //��ʾ�з��Ϲ��������
                        switch (this.m_CurrentRadar.DealType)
                        {
                            case cGlobalParas.MonitorDealType.ByTaskConfig:
                                ByTaskConfig(d);
                                break;
                            case cGlobalParas.MonitorDealType.SaveUrl:
                                BySaveUrl(d);
                                break;
                            case cGlobalParas.MonitorDealType.SaveUrlAndPage:
                                BySaveUrlAndPage(d);
                                break;
                            case cGlobalParas.MonitorDealType.SendEmail:
                                BySendEmail(d);
                                break;
                        }

                        switch (this.m_CurrentRadar.WaringType)
                        {
                            case cGlobalParas.WarningType.ByEmail:
                                BySendWarningEmail();
                                break;
                            case cGlobalParas.WarningType.ByTrayIcon:

                                //����Ԥ���¼�
                                if (e_RadarWarning != null)
                                {
                                    e_RadarWarning(this, new cRadarMonitorWaringArgs(this.m_CurrentRadar.WaringType, this.m_CurrentRadar.TaskName + "���ַ��Ϲ��������"));
                                }
                                break;
                            case cGlobalParas.WarningType.NoWaring:
                                break;
                        }

                        this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].Count++;
                        this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount++;

                        if (e_Log != null)
                            e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Info, "��⵽���Ϲ������ַ" + Url, this.m_IsErrorLog));
                    }

                }
                else
                {
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount++;
                }

                #endregion

            

            //ÿ�βɼ��Ա�һ�����ݺ󣬽��м�������ˢ��
            if (e_RadarCount !=null )
                e_RadarCount(this, new cRadarCountArgs(this.m_CurrentRadar.TaskName, this.m_Round,
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount,
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].Count));

        }

        private void tManage_GUrlCount(object sender,cGatherUrlCounterArgs e)
        {

        }

        //�ж������Ƿ����ƥ��Ĺ���
        private bool MatchRule(string strData,eRule rule)
        {
            bool IsMatch=false ;

            try
            {
                switch (rule.Rule)
                {
                    case cGlobalParas.MonitorRule.IncludeKeyword:
                        string str1 =strData;
                        string str2 =rule.Content;

                        if (str2.IndexOf("/") > 1)
                        {
                            IsMatch = MatchKeyword(str1, str2);
                        }
                        else
                        {
                            Regex RegexWords = new Regex(str2, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            int WordCount = RegexWords.Matches(str1).Count;

                            if (WordCount >= rule.Num)
                                IsMatch = true;
                        }

                        break;

                    case cGlobalParas.MonitorRule.LessThan:
                        if (float.Parse(strData) < float.Parse(rule.Content))
                            IsMatch = true;

                        break;

                    case cGlobalParas.MonitorRule.MoreThan:
                        if (float.Parse(strData) > float.Parse(rule.Content))
                            IsMatch = true;

                        break;

                    case cGlobalParas.MonitorRule.NumRange:
                        float n1 = float.Parse(rule.Content.Substring(0, rule.Content.IndexOf("-") - 1));
                        float n2 = float.Parse(rule.Content.Substring(rule.Content.IndexOf("-") + 1, rule.Content.Length - rule.Content.IndexOf("-") - 1));
                        if (float.Parse(strData) > n1 && float.Parse(strData) < n2)
                            IsMatch = true;

                        break;
                }
            }
            catch (System.Exception)
            {
                //������󲻴���
            }

            return IsMatch;
        }

        //ƥ����Ϲؼ���
        private bool MatchKeyword(string str1,string str2)
        {
            bool  isMatch=true  ;

            foreach(string ss in str2.Split ('/'))
            {
                if (!Regex.IsMatch(str1, ss, RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    isMatch = false;
                    break;
                }
            }

            return isMatch;
        }

        //

        #endregion

        #region �����״������������������ݴ���
        private void ByTaskConfig(DataTable tmpData)
        {
            //��ȡ��ǰ���ڼ���״�Ĺ�������ڲɼ�������
            oTask t = new oTask(this.m_workPath);

            oTaskClass tc = new oTaskClass(this.m_workPath);
            cTaskData tData = new cTaskData();

            string tPath = "";

            if (this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskClass== "")
            {
                tPath = this.m_workPath + "tasks";
            }
            else
            {
                tPath = this.m_workPath + tc.GetTaskClassPathByName(this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskClass);
            }

            tc = null;

            string tFileName = this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskName + ".smt";

            t.LoadTask(tPath + "\\" + tFileName);

            //�ж����ݷ���������
            if (t.TaskEntity.RunType == cGlobalParas.TaskRunType.OnlySave)
            {
                //ֱ����⣬���òɼ������������ݽ���������

                NewTable(t.TaskEntity.ExportType, tmpData.Columns,t.TaskEntity.DataSource ,t.TaskEntity.DataTableName );
                InsertData(tmpData,t.TaskEntity.ExportType, t.TaskEntity.DataSource,t.TaskEntity.InsertSql );
                tmpData = null;


            }
            else if (t.TaskEntity.RunType == cGlobalParas.TaskRunType.GatherExportData)
            {
                //�ɼ��������ݣ����òɼ��������ݷ�������
            }

        }

        //����Url
        private void BySaveUrl(DataTable d)
        {
            if (d==null)
                return ;

            string Url = d.Rows[d.Rows.Count - 1][d.Columns.Count - 1].ToString();
            string sql = this.m_CurrentRadar.Sql;

            //�滻sql�Ĳ���
            while (Regex.IsMatch(sql, "[{].*[}]"))
            {
                Match m = Regex.Match(sql, "[{].+?[}]");
                string s = m.Groups[0].Value.ToString();

                switch (s)
                {
                    case "{Url}":
                        sql = sql.Replace("{Url}", Url);
                        break;
                    case "{MonitorName}":
                        sql = sql.Replace("{MonitorName}", this.m_CurrentRadar.TaskName);
                        break;
                    case "{TaskName}":
                        sql = sql.Replace("{TaskName}", this.m_CurrentRadar .Source[this.m_CurrentRadarGatherIndex ].TaskName );
                        break ;
                    case "{MonitorDate}":
                        sql = sql.Replace("{MonitorDate}", System.DateTime.Now.ToString ());
                        break ;
                }

            }


            switch (this.DatabaseType)
            {
                case cGlobalParas.DatabaseType .MySql :

                    SaveUrlMysql(this.m_dbCon, sql, this.m_CurrentRadar.TableName);
                    break;
                case cGlobalParas.DatabaseType .MSSqlServer :

                    SaveUrlMSSqlserver(this.m_dbCon, sql, this.m_CurrentRadar.TableName);
                    break;
            }
        }

        private void BySaveUrlAndPage(DataTable d)
        {
            //�ȵ��÷���������ҳ��ַ
            BySaveUrl(d);

            //������ҳ������
            string tmpPath = this.m_CurrentRadar.SavePagePath;

            if (tmpPath == "")
                tmpPath = this.m_workPath;

            string Url = d.Rows[d.Rows.Count - 1][d.Columns.Count - 1].ToString();

            oTask t = new oTask(this.m_workPath);

            oTaskClass tc = new oTaskClass(this.m_workPath);
            cTaskData tData = new cTaskData();

            string tPath = "";

            if (this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskClass== "")
            {
                tPath = this.m_workPath + "tasks";
            }
            else
            {
                tPath = this.m_workPath + tc.GetTaskClassPathByName( this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskClass);
            }

            tc = null;

            string tFileName = this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].TaskName + ".smt";

            t.LoadTask(tPath + "\\" + tFileName);

            cGlobalParas.WebCode pageCode = t.TaskEntity.WebCode;

            //cGatherWeb g = new cGatherWeb();
            cProxyControl PControl = new cProxyControl(this.m_workPath , 0);


            //���и����⣬�����״﷢��ҳ�沢�������ʱ�����ֵĴ����������⣬��ǰ
            //�޷���ȡ��������߳�ִ�������Ϣ�������޷�̽�⵽��ʹ�õĴ���
            //ֻ����Ĭ�ϱ���ķ�ʽ�����У�Ӧ���޸ġ�
            string sCookie = t.TaskEntity.Cookie;
            eRequest request = NetMiner.Core.Url.UrlPack.GetRequest(Url, sCookie, t.TaskEntity.WebCode, t.TaskEntity.IsUrlEncode, 
                t.TaskEntity.IsTwoUrlCode, t.TaskEntity.UrlEncode, "",
                                    t.TaskEntity .Headers, "", t.TaskEntity.IsUrlAutoRedirect);

            eResponse response = NetMiner.Net.Unity.RequestUri(this.m_workPath, request, false);
            string webSource = response.Body;

            //cGatherWeb g = new cGatherWeb(this.m_workPath, ref PControl, t.TaskEntity.IsProxy, 
            //    t.TaskEntity.IsProxyFirst,cGlobalParas.ProxyType.TaskConfig,"",0);



            //string webSource = g.GetHtml(Url, t.TaskEntity.WebCode,t.TaskEntity.IsUrlEncode,t.TaskEntity.IsTwoUrlCode, 
            //    t.TaskEntity.UrlEncode, ref sCookie, "", "", true, false,"",
            //    t.TaskEntity.isGatherCoding, t.TaskEntity.GatherCodingFlag,t.TaskEntity.CodeUrl,t.TaskEntity.GatherCodingPlugin);

            Match m = Regex.Match(webSource, "(?<=<title>)[\\S\\s]*(?=</title>)", RegexOptions.IgnoreCase);
            string title = m.Groups[0].Value.ToString();

            t = null;

            //g = null;


            string fName =Url.Substring(Url.LastIndexOf("/")+1, Url.Length - Url.LastIndexOf("/")-1);

            string FileName = title + ".html";

            if (FileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                //���зǷ��ַ� \ / : * ? " < > | ��
                //if (e_Log != null)
                //    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "�ļ������ַǷ��ַ��޷�����", this.m_IsErrorLog));

                //return;

                //���title�к��зǷ��ַ�����ʹ��Url���б���
                FileName = Url.Substring (7,Url.Length -7) + ".html";
                FileName = FileName.Replace("/", "-");
                FileName = FileName.Replace("?", "-");
            }

            FileName = tmpPath + "\\" + FileName;
           

            try
            {
                FileStream myStream = File.Open(FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                StreamWriter sw;

                switch (pageCode)
                {
                   
                    case cGlobalParas.WebCode.gb2312 :
                        sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
                        break;
                    case cGlobalParas.WebCode.big5 :
                        sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("big5"));
                        break;
                    case cGlobalParas.WebCode.gbk :
                        sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gbk"));
                        break;
                    default :
                        sw = new StreamWriter(myStream, System.Text.Encoding.UTF8 );
                        break;
                }

                sw.Write(webSource);
                sw.Close();
                myStream.Close();
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, ex.Message, this.m_IsErrorLog));
            }

            PControl = null;
        }

        private void BySendEmail(DataTable d)
        {
            string Url = d.Rows[d.Rows.Count - 1][d.Columns.Count - 1].ToString();

            cXmlSConfig cs=new cXmlSConfig (this.m_workPath);

            NetMiner.Common.Tools.cEmail e = new NetMiner.Common.Tools.cEmail();
            e.Title = this.m_CurrentRadar.TaskName + "���ַ��Ϲ�������� ���������" + "";
            e.ReceiveEmail = this.m_CurrentRadar.ReceiveEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString ();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;

            string emailContent = "ʱ��" + System.DateTime.Now ;
            emailContent += "��ع������ƣ�" + this.m_CurrentRadar .TaskName ;
            emailContent += "�����ַ��" + Url;
            emailContent += "���ϼ�ع������飡";
            emailContent +="���ʼ�Ϊ�Զ����ͣ���������������ݲɼ����";

            e.Content = emailContent ;

            try
            {
                e.SendMail();
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, ex.Message , this.m_IsErrorLog));
            }

            cs = null;
            e = null;

        }

        #endregion

        #region ����ɼ���Url Access��MSSqlserver
        private void SaveUrlAccess( string strCon, string sql,string TableName)
        {
            DataTable dbtmp = new DataTable();
            DataColumnCollection dColumns = dbtmp.Columns;
            dColumns.Add("MonitorName");
            dColumns.Add("TaskName");
            dColumns.Add("Url");
            dColumns.Add("MonitorDate");

            NewAccessTable(dColumns,strCon, TableName);

            dbtmp = null;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            conn.Open();

            OleDbCommand com = new OleDbCommand();
            com.Connection = conn;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.ExecuteNonQuery();

            com.Dispose();
            conn.Close();
            conn.Dispose();

        }

        private void SaveUrlMysql(string strCon, string sql, string TableName)
        {
            DataTable dbtmp = new DataTable();
            DataColumnCollection dColumns = dbtmp.Columns;
            dColumns.Add("MonitorName");
            dColumns.Add("TaskName");
            dColumns.Add("Url");
            dColumns.Add("MonitorDate");

            NewMySqlTable(dColumns, strCon, TableName);

            dbtmp = null;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon(strCon);

            conn.Open();

            MySqlCommand com = new MySqlCommand();
            com.Connection = conn;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.ExecuteNonQuery();

            com.Dispose();
            conn.Close();
            conn.Dispose();
        }

        private void SaveUrlMSSqlserver( string strCon, string sql, string TableName)
        {
            DataTable dbtmp = new DataTable();
            DataColumnCollection dColumns = dbtmp.Columns;
            dColumns.Add("MonitorName");
            dColumns.Add("TaskName");
            dColumns.Add("Url");
            dColumns.Add("MonitorDate");

            NewMSSqlServerTable(dColumns,strCon, TableName);

            dbtmp = null;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (strCon);

            conn.Open();

            SqlCommand com = new SqlCommand();
            com.Connection = conn;
            com.CommandType = CommandType.Text;
            com.CommandText = sql;
            com.ExecuteNonQuery();

            com.Dispose();
            conn.Close();
            conn.Dispose();
        }

        #endregion

        #region �ж��Ƿ����±�
        private void NewTable(cGlobalParas.PublishType PublishType, DataColumnCollection dColumns, string DataSource,string TableName)
        {
            //�����жϱ��Ƿ���ڣ��������������н���
            if (IsNewTable == true)
            {
                bool isSucceed = false;

                switch (PublishType)
                {
                    case cGlobalParas.PublishType.PublishAccess:
                        isSucceed = NewAccessTable(dColumns, DataSource, TableName);
                        break;
                    case cGlobalParas.PublishType.PublishMSSql:
                        isSucceed = NewMSSqlServerTable(dColumns, DataSource, TableName);
                        break;
                    case cGlobalParas.PublishType.PublishMySql:
                        isSucceed = NewMySqlTable(dColumns, DataSource, TableName);
                        break;
                }

                if (isSucceed == true)
                    IsNewTable = false;
            }

        }

        private bool NewAccessTable(DataColumnCollection dColumns, string DataSource, string TableName)
        {
            bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();
            string connectionstring = DataSource;
            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message + "���޷�������������", this.m_IsErrorLog));
                return false;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    if (r[2].ToString() == TableName)
                    {
                        IsTable = true;
                        break;
                    }
                }

            }

            if (IsTable == false)
            {
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.Access, "", dColumns,TableName);

                OleDbCommand com = new OleDbCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.OleDb.OleDbException ex)
                {
                    //if (ex.ErrorCode != -2147217900)
                    //{

                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message , this.m_IsErrorLog));
                    conn.Close();
                    return false;
                    //}
                }

                IsTable = true;

            }

            conn.Close();

            return IsTable;
        }

        private bool NewMSSqlServerTable(DataColumnCollection dColumns, string DataSource, string TableName)
        {
            bool IsTable = false;

            SqlConnection conn = new SqlConnection();
            string connectionstring = DataSource;
            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message + "���޷�������������" , this.m_IsErrorLog));
                return false;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[2].ToString() == TableName)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MSSqlServer, "", dColumns,TableName);

                SqlCommand com = new SqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (System.Data.SqlClient.SqlException ex)
                {
                    //if (ex.ErrorCode != -2147217900)
                    //{
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message, this.m_IsErrorLog));
                    conn.Close();
                    return false;
                    //}
                }
                IsTable = true;
            }

            conn.Close();

            return IsTable;
        }

        private bool NewMySqlTable(DataColumnCollection dColumns, string DataSource, string TableName)
        {
            bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();
            string connectionstring = DataSource;

            conn.ConnectionString =ToolUtil.DecodingDBCon ( connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ���������Ϣ��" + ex.Message + "���޷�������������", this.m_IsErrorLog));
                return false;
            }

            System.Data.DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (string.Compare(r[2].ToString(), TableName, true) == 0)
                {
                    IsTable = true;
                    break;
                }
            }

            if (IsTable == false)
            {
                //ͨ�������ַ����ѱ����ȡ���������ݱ���������ݱ�Ľ���
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //��Ҫ�����±������±��ʱ�����ado.net�½��еķ�ʽ������������
                string CreateTablesql = getCreateTablesql(cGlobalParas.DatabaseType.MySql, Encoding, dColumns,TableName);

                MySqlCommand com = new MySqlCommand();
                com.Connection = conn;
                com.CommandText = CreateTablesql;
                com.CommandType = CommandType.Text;
                try
                {
                    int result = com.ExecuteNonQuery();
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    //if (ex.ErrorCode != -2147217900)
                    //{
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "���ݿ⽨�������󣬴�����Ϣ��" + ex.Message , this.m_IsErrorLog));
                    conn.Close();
                    return false;
                    //}
                }

                IsTable = true;
            }

            return IsTable;
        }

        private string getCreateTablesql(cGlobalParas.DatabaseType dType, string Encoding, DataColumnCollection dColumns,string TableName)
        {
            string strsql = "";

            switch (dType)
            {
                case cGlobalParas.DatabaseType.Access:
                    strsql = "create table " + TableName + "(";
                    break;
                case cGlobalParas.DatabaseType.MSSqlServer:
                    strsql = "create table [" + TableName + "](";
                    break;
                case cGlobalParas.DatabaseType.MySql:
                    strsql = "create table `" + TableName + "`(";
                    break;
                default:
                    strsql = "create table " + TableName + "(";
                    break;
            }

            for (int i = 0; i < dColumns.Count; i++)
            {
                switch (dType)
                {
                    case cGlobalParas.DatabaseType.Access:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MSSqlServer:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    case cGlobalParas.DatabaseType.MySql:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                    default:
                        strsql += dColumns[i].ColumnName + " " + "text" + ",";
                        break;
                }
            }
            strsql = strsql.Substring(0, strsql.Length - 1);
            strsql += ")";

            //�����mysql���ݿ⣬��Ҫ�������Ӵ����ַ����������ݱ�Ľ���
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }
        #endregion

        #region ֱ����� access mssqlserver mysql
        private void InsertData(DataTable tmpData, cGlobalParas.PublishType PublishType,string DataSource,string InsertSql)
        {
            if (IsNewTable == true)
            {
                e_Log(this,new cRadarLogArgs( cGlobalParas.LogType.Error, "���ݱ����ڣ��޷�����ɼ������ݣ��������־��", this.m_IsErrorLog));
            }

            //�жϴ洢���ݿ�����
            switch (PublishType)
            {
                case cGlobalParas.PublishType.PublishAccess:
                    ExportAccess(tmpData,DataSource ,InsertSql );
                    break;
                case cGlobalParas.PublishType.PublishMSSql:
                    ExportMSSql(tmpData, DataSource, InsertSql);
                    break;
                case cGlobalParas.PublishType.PublishMySql:
                    ExportMySql(tmpData, DataSource, InsertSql);
                    break;
            }
        }

        private void ExportAccess(DataTable tmpData, string DataSource, string InsertSql)
        {
            //bool IsTable = false;

            OleDbConnection conn = new OleDbConnection();

            string connectionstring = DataSource;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception)
            {

                //e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������" , this.IsErrorLog));
                return;
            }

            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
            System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //��ʼƴsql���
            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = InsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "����ʧ�ܣ�������Ϣ��" + ex.Message , this.m_IsErrorLog));
                }
            }

            conn.Close();

        }

        private void ExportMSSql(DataTable tmpData, string DataSource, string InsertSql)
        {
            //bool IsTable = false;

            SqlConnection conn = new SqlConnection();

            string connectionstring = DataSource;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);



            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����


            //��ʼƴsql���
            string strInsertSql = InsertSql;

            //��Ҫ��˫�����滻�ɵ�����
            //strInsertSql = strInsertSql.Replace("\"", "'");

            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    conn.Open();
                }
                catch (System.Exception)
                {

                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������", this.m_IsErrorLog));
                    return;
                }

                try
                {
                    SqlCommand cm = new SqlCommand();
                    cm.Connection = conn;
                    cm.CommandType = CommandType.Text;
                    cm.CommandTimeout = 10;

                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                    conn.Close();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "����ʧ�ܣ�������Ϣ��" + ex.Message , this.m_IsErrorLog));

                }
            }

        }

        private void ExportMySql(DataTable tmpData, string DataSource, string InsertSql)
        {
            //bool IsTable = false;

            MySqlConnection conn = new MySqlConnection();

            string connectionstring = DataSource;

            conn.ConnectionString = ToolUtil.DecodingDBCon (connectionstring);

            try
            {
                conn.Open();
            }
            catch (System.Exception)
            {
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "����ֱ����⣬���ݿ����ӳ����޷�������������" , this.m_IsErrorLog));
                return;
            }

            //���轨���±���Ҫ����sql���ķ�ʽ���У�����Ҫ�滻sql����е�����
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //��ʼƴsql���
            string strInsertSql = InsertSql;


            string sql = "";

            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                sql = strInsertSql;

                for (int j = 0; j < tmpData.Columns.Count; j++)
                {
                    string strPara = "{" + tmpData.Columns[j].ColumnName + "}";
                    string strParaValue = tmpData.Rows[i][j].ToString().Replace("\"", "\"\"");
                    sql = sql.Replace(strPara, strParaValue);
                }

                try
                {
                    cm.CommandText = sql;
                    cm.ExecuteNonQuery();
                }
                catch (System.Exception ex)
                {
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "����ʧ�ܣ�������Ϣ��" + ex.Message , this.m_IsErrorLog));
                }
            }

            conn.Close();
        }
        #endregion

        #region ������ҳ������Ϣ
        private void SaveWebPage(string Url)
        {
            //cGatherWeb gWeb = new cGatherWeb();
            //gWeb.GetHtml (this.url,this.m_CurrentRadar.Source[this.m_CurrentRadarIndex ].
        }
        #endregion

        #region ����Ԥ���ʼ�
        private void BySendWarningEmail()
        {

            cXmlSConfig cs = new cXmlSConfig(this.m_workPath);

            NetMiner.Common.Tools.cEmail e = new NetMiner.Common.Tools.cEmail();
            e.Title = this.m_CurrentRadar.TaskName + "���ַ��Ϲ�������� ���������" + "";
            e.ReceiveEmail = this.m_CurrentRadar.WaringEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;

            string emailContent = "ʱ��" + System.DateTime.Now ;
            emailContent += "��ع������ƣ�" + this.m_CurrentRadar.TaskName;
            emailContent += "���ϼ�ع������飡";
            emailContent += "���ʼ�Ϊ�Զ����ͣ���������������ݲɼ����";

            e.Content = emailContent;

            try
            {
                e.SendMail();
            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, ex.Message , this.m_IsErrorLog));
            }
            cs = null;
            e = null;

        }
        #endregion

        #region �����¼�
        private readonly Object m_eventLock = new Object();

        private event EventHandler<cRadarStartedArgs> e_RadarStarted;
        public event EventHandler<cRadarStartedArgs> RadarStarted
        {
            add { lock (m_eventLock) { e_RadarStarted += value; } }
            remove { lock (m_eventLock) { e_RadarStarted -= value; } }
        }

        private event EventHandler<cRadarStopArgs> e_RadarStop;
        public event EventHandler<cRadarStopArgs> RadarStop
        {
            add { lock (m_eventLock) { e_RadarStop += value; } }
            remove { lock (m_eventLock) { e_RadarStop -= value; } }
        }

        private event EventHandler<cRadarMonitorWaringArgs> e_RadarWarning;
        public event EventHandler<cRadarMonitorWaringArgs> RadarWarning
        {
            add { lock (m_eventLock) { e_RadarWarning += value; } }
            remove { lock (m_eventLock) { e_RadarWarning -= value; } }
        }

        private event EventHandler<cRadarLogArgs> e_Log;
        public event EventHandler<cRadarLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }

        private event EventHandler<cRadarStateArgs> e_RadarState;
        public event EventHandler<cRadarStateArgs> RadarState
        {
            add { lock (m_eventLock) { e_RadarState += value; } }
            remove { lock (m_eventLock) { e_RadarState -= value; } }
        }

        private event EventHandler<cRadarCountArgs> e_RadarCount;
        public event EventHandler<cRadarCountArgs> RadarCount
        {
            add { lock (m_eventLock) { e_RadarCount += value; } }
            remove { lock (m_eventLock) { e_RadarCount -= value; } }
        }
        #endregion

    }
}
