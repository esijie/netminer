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
        //初始化默认轮次
        private int m_Round = 0;

        //定义一个值，判断当前是否启动雷达
        private bool m_IsStarted = false;

        //定义一个值，判断当前是否正在运行监测任务
        private bool m_IsRunning = false;
        //是否进行采集
        private bool m_IsGathering = false;

        //定义一个计时器，进行雷达的轮询操作
        private System.Threading.Timer m_RadarEngine;

        //初始化成功标志
        private bool m_IsInitialized = false;

        //定义一个雷达信息的集合
        private List<eRadar> m_RadarList;

        //建立一个采集任务控制器，用于雷达任务的采集操作
        private cGatherControl m_GatherControl;

        //记录当前需要监控数据的规则
        private eRadar m_CurrentRadar;

        //记录当前正在运行的雷达监控
        private int m_CurrentRadarGatherIndex = 0;

        //定义一个值表示是否存储日志事件
        private bool m_IsErrorLog = false;

        //定义一个值判断是否建立新表
        private bool IsNewTable = true;

        //定义一个值，进行监控间隔的设置，系统默认为10分钟
        private int m_MonitorInterval=10;

        private int m_LastTime;

        //定义一个值，表示记录当前的工作路径
        private string m_workPath = string.Empty;

        public cRadarManage(string workPath)
        {

            this.m_workPath = workPath;

            //初始化采集控制器，并绑定相应的事件
            //初始化一个采集任务的控制器,采集任务由此控制器来负责采集任务
            //管理
            NetMiner.Base.cHashTree tmpUrls = null;
            m_GatherControl = new cGatherControl(m_workPath,false,ref tmpUrls);

            //采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
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

            //从配置文件获取监控的时间间隔
            cXmlSConfig cs = new cXmlSConfig(this.m_workPath);
            m_MonitorInterval = cs.MonitorInterval;
            cGlobalParas.DatabaseType dtype = (cGlobalParas.DatabaseType)cs.DataType;
            this.DatabaseType=dtype;
            this.dbCon = cs.DataConnection; 
            cs = null;

            //转换成毫秒单位
            m_MonitorInterval = m_MonitorInterval * 60000;
            
            //初始化计时器
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

        #region 属性
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

        //只读属性，判断当前雷达是否运行
        public bool IsStarted
        {
            get { return m_IsStarted; }
        }
        #endregion

        #region 计时器
        /// 初始化timer计时器,当前采用了Threading.timer计时器
        /// 此计时器的作用是轮询当前雷达，进行监测任务的监控
        /// 
        private void timerInit()
        {
            m_LastTime = System.Environment.TickCount;
            m_RadarEngine = new System.Threading.Timer(new System.Threading.TimerCallback(m_MonitorEngine_CallBack), null, 0, m_MonitorInterval);
            m_IsInitialized = true;
            m_IsRunning = false;
        }

        /// 处理采集数据的显示和日志显示
        private void m_MonitorEngine_CallBack(object State)
        {
            if (m_IsStarted==true && m_IsRunning ==false)
            {
                m_Round++;

                m_IsRunning = true;

                //重新加载雷达数据
                IniRadar();

                //开始进行数据监测，当前为了可以降低对系统资源的侵占
                //雷达监测任务的采集是顺序进行，即加载一个监测采集任务，完成后
                //在进行下一个雷达采集任务
                for (int i = 0; i < this.m_RadarList.Count;i++ )
                {
                    //雷达每次执行前，都要判断此雷达监测任务是否失效
                    //判断的依据是时间，如果失效则修改状态，并跳过此
                    //监测规则

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

        ///执行某个雷达的采集任务，有可能是多个采集任务
        ///如果采集任务没有执行完成，则不允许跳出这个方法
        private void ExcuteGahterTask(eRadar r)
        {
            
            //将此雷达监控规则下需要运行的采集任务加载运行
            for (int j = 0; j < r.Source.Count; j++)
            {

                //记录当前所正在监控雷达规则的采集任务
                m_CurrentRadarGatherIndex = j;

                //每次启动采集任务
                r.Source[j].GatheredCount = 0;
                r.Source[j].Count = 0;

                cGatherTask t = AddRunTask(r.Source[j].TaskClass, r.Source[j].TaskName);

                if (t == null)
                {
                    //表示加载采集任务失败

                }
                else
                {
                    //启动此任务
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

        //初始化雷达的列表信息
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

            //触发启动事件
            if (e_RadarStarted != null)
                e_RadarStarted(this, new cRadarStartedArgs());
        }

        public void StopRadar()
        {
            this.m_IsStarted = false;


            //强制退出采集任务，因为有可能存在导航操作，
            //如果是导航，则无法直接停止采集任务，只能采用强制
            //退出
            this.m_GatherControl.Abort();

            if (e_RadarStop != null)
                e_RadarStop(this, new cRadarStopArgs());
        }

        /// 事件 线程同步锁
        private readonly Object m_radarEventLock = new Object();

        #region 雷达任务采集
        //将需要采集的任务加载到运行列表
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

            //获取最大的执行ID
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

                //在进行雷达监测的时候，所有的采集任务都是仅采集数据
                tData.RunType = cGlobalParas.TaskRunType.OnlyGather;

                tData.TempDataFile = t.TaskEntity.TempDataFile;
                //tData.TaskState =(cGlobalParas.TaskState) t.TaskState;
                
                //所有新增的任务都是未开始状态
                tData.TaskState = cGlobalParas.TaskState.UnStart;

                tData.UrlCount = t.TaskEntity.UrlCount;
                tData.UrlNaviCount = 0;
                tData.ThreadCount = t.TaskEntity.ThreadCount;
                tData.GatheredUrlCount = 0;
                tData.GatherErrUrlCount = 0;
                tData.GatherDataCount = 0;
                tData.GatherTmpCount = 0;
                tData.StartTimer = System.DateTime.Now;


                //添加任务到运行区
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

        #region 采集任务处理事件
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

        /// 事件 线程同步锁
        private readonly Object m_IsUrlLock = new Object();

        private void tManage_GData(object sender, cGatherDataEventArgs e)
        {

            //在此进行数据规则的判断
            bool IsMatch = false;
            
            DataTable d = e.gData;

            string Url=string.Empty ;
            if (d.Columns.Contains ("CollectionUrl"))
                Url = d.Rows[d.Rows.Count - 1]["CollectionUrl"].ToString();
            else
                Url = d.Rows[d.Rows.Count - 1][0].ToString();


                #region  根据采集的数据进行规则比对

                //增加一个默认列，表示当前的行记录是否符合规则，默认是不符合
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

                //如果比对规则后，有存在的信息，则根据系统进行预警
                if (IsMatch == true)
                {
                    //如果监控符合规则的数据，则将此网址压入排重库
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
                        //如果此网址已经存在，则无需预警操作了

                    }
                    else
                    {
                        if (m_CurrentRadar.InvalidType == cGlobalParas.MonitorInvalidType.Invalid)
                        {
                            m_CurrentRadar.MonitorState = cGlobalParas.MonitorState.Invalid;
                        }

                        //表示有符合规则的数据
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

                                //触发预警事件
                                if (e_RadarWarning != null)
                                {
                                    e_RadarWarning(this, new cRadarMonitorWaringArgs(this.m_CurrentRadar.WaringType, this.m_CurrentRadar.TaskName + "发现符合规则的数据"));
                                }
                                break;
                            case cGlobalParas.WarningType.NoWaring:
                                break;
                        }

                        this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].Count++;
                        this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount++;

                        if (e_Log != null)
                            e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Info, "检测到符合规则的网址" + Url, this.m_IsErrorLog));
                    }

                }
                else
                {
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount++;
                }

                #endregion

            

            //每次采集对比一次数据后，进行计数器的刷新
            if (e_RadarCount !=null )
                e_RadarCount(this, new cRadarCountArgs(this.m_CurrentRadar.TaskName, this.m_Round,
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].GatheredCount,
                    this.m_CurrentRadar.Source[this.m_CurrentRadarGatherIndex].Count));

        }

        private void tManage_GUrlCount(object sender,cGatherUrlCounterArgs e)
        {

        }

        //判断数据是否符合匹配的规则
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
                //捕获错误不处理
            }

            return IsMatch;
        }

        //匹配组合关键词
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

        #region 根据雷达的配置情况，进行数据处理
        private void ByTaskConfig(DataTable tmpData)
        {
            //获取当前正在监控雷达的规则和正在采集的任务
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

            //判断数据发布的类型
            if (t.TaskEntity.RunType == cGlobalParas.TaskRunType.OnlySave)
            {
                //直接入库，调用采集任务配置数据进行入库操作

                NewTable(t.TaskEntity.ExportType, tmpData.Columns,t.TaskEntity.DataSource ,t.TaskEntity.DataTableName );
                InsertData(tmpData,t.TaskEntity.ExportType, t.TaskEntity.DataSource,t.TaskEntity.InsertSql );
                tmpData = null;


            }
            else if (t.TaskEntity.RunType == cGlobalParas.TaskRunType.GatherExportData)
            {
                //采集发布数据，调用采集任务数据发布配置
            }

        }

        //保存Url
        private void BySaveUrl(DataTable d)
        {
            if (d==null)
                return ;

            string Url = d.Rows[d.Rows.Count - 1][d.Columns.Count - 1].ToString();
            string sql = this.m_CurrentRadar.Sql;

            //替换sql的参数
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
            //先调用方法保存网页地址
            BySaveUrl(d);

            //保存网页的内容
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


            //这有个问题，网络雷达发现页面并保存快照时，出现的代理设置问题，当前
            //无法获取到任务的线程执行情况信息，所以无法探测到所使用的代理
            //只能用默认保存的方式来进行，应该修改。
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
                //含有非法字符 \ / : * ? " < > | 等
                //if (e_Log != null)
                //    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "文件名出现非法字符无法保存", this.m_IsErrorLog));

                //return;

                //如果title中含有非法字符，则使用Url进行保存
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
            e.Title = this.m_CurrentRadar.TaskName + "发现符合规则的内容 来自网络矿工" + "";
            e.ReceiveEmail = this.m_CurrentRadar.ReceiveEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString ();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;

            string emailContent = "时间" + System.DateTime.Now ;
            emailContent += "监控规则名称：" + this.m_CurrentRadar .TaskName ;
            emailContent += "监控网址：" + Url;
            emailContent += "符合监控规则，请检查！";
            emailContent +="此邮件为自动发送，来自于网络矿工数据采集软件";

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

        #region 保存采集的Url Access及MSSqlserver
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

        #region 判断是否建立新表
        private void NewTable(cGlobalParas.PublishType PublishType, DataColumnCollection dColumns, string DataSource,string TableName)
        {
            //首先判断表是否存在，如果不存在则进行建立
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
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message + "。无法进行入库操作！", this.m_IsErrorLog));
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
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
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

                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message , this.m_IsErrorLog));
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
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message + "。无法进行入库操作！" , this.m_IsErrorLog));
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
                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
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
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message, this.m_IsErrorLog));
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
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，错误信息：" + ex.Message + "。无法进行入库操作！", this.m_IsErrorLog));
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
                //通过连接字符串把编码获取出来，根据编码进行数据表的建立
                string strMatch = "(?<=character set=)[^\\s]*(?=[\\s;])";
                Match s = Regex.Match(connectionstring, strMatch, RegexOptions.IgnoreCase);
                string Encoding = s.Groups[0].Value;

                //需要建立新表，建立新表的时候采用ado.net新建行的方式进行数据增加
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
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据库建表发生错误，错误信息：" + ex.Message , this.m_IsErrorLog));
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

            //如果是mysql数据库，需要根据连接串的字符集进行数据表的建立
            if (dType == cGlobalParas.DatabaseType.MySql)
            {
                if (Encoding == "" || Encoding == null)
                    Encoding = "utf8";

                strsql += " CHARACTER SET " + Encoding + " ";
            }

            return strsql;
        }
        #endregion

        #region 直接入库 access mssqlserver mysql
        private void InsertData(DataTable tmpData, cGlobalParas.PublishType PublishType,string DataSource,string InsertSql)
        {
            if (IsNewTable == true)
            {
                e_Log(this,new cRadarLogArgs( cGlobalParas.LogType.Error, "数据表不存在，无法保存采集的数据，请查阅日志！", this.m_IsErrorLog));
            }

            //判断存储数据库的类别
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

                //e_Log(this, new cGatherTaskLogArgs(m_TaskID, cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！" , this.IsErrorLog));
                return;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            System.Data.OleDb.OleDbCommand cm = new System.Data.OleDb.OleDbCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
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
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "插入失败，错误信息：" + ex.Message , this.m_IsErrorLog));
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



            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容


            //开始拼sql语句
            string strInsertSql = InsertSql;

            //需要将双引号替换成单引号
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

                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！", this.m_IsErrorLog));
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
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "插入失败，错误信息：" + ex.Message , this.m_IsErrorLog));

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
                e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, "数据直接入库，数据库连接出错，无法进行入库操作！" , this.m_IsErrorLog));
                return;
            }

            //无需建立新表，需要采用sql语句的方式进行，但需要替换sql语句中的内容
            MySqlCommand cm = new MySqlCommand();
            cm.Connection = conn;
            cm.CommandType = CommandType.Text;

            //开始拼sql语句
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
                    e_Log(this, new cRadarLogArgs(cGlobalParas.LogType.Error, tmpData.Rows.ToString() + "插入失败，错误信息：" + ex.Message , this.m_IsErrorLog));
                }
            }

            conn.Close();
        }
        #endregion

        #region 保存网页数据信息
        private void SaveWebPage(string Url)
        {
            //cGatherWeb gWeb = new cGatherWeb();
            //gWeb.GetHtml (this.url,this.m_CurrentRadar.Source[this.m_CurrentRadarIndex ].
        }
        #endregion

        #region 发送预警邮件
        private void BySendWarningEmail()
        {

            cXmlSConfig cs = new cXmlSConfig(this.m_workPath);

            NetMiner.Common.Tools.cEmail e = new NetMiner.Common.Tools.cEmail();
            e.Title = this.m_CurrentRadar.TaskName + "发现符合规则的内容 来自网络矿工" + "";
            e.ReceiveEmail = this.m_CurrentRadar.WaringEmail;
            e.Email = cs.Email;
            e.PopServer = cs.EmailPopServer;
            e.Port = cs.EmailPopPort.ToString();
            e.User = cs.EmailUser;
            e.Pwd = cs.EmailPwd;

            e.IsPopVerfy = cs.IsPopVerfy;

            string emailContent = "时间" + System.DateTime.Now ;
            emailContent += "监控规则名称：" + this.m_CurrentRadar.TaskName;
            emailContent += "符合监控规则，请检查！";
            emailContent += "此邮件为自动发送，来自于网络矿工数据采集软件";

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

        #region 定义事件
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
