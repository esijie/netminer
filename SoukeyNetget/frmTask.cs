using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;
using System.Resources;
using System.Reflection;
using NetMiner.Gather.Control;
using SoukeyControl.WebBrowser;
using NetMiner.Core.Proxy;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Publish.Rule;
using System.Data.OracleClient;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using NetMiner.Core.gTask;
using NetMiner;
using NetMiner.Core.gTask.Entity;
using NetMiner.Gather.Url;
using NetMiner.Core.pTask;
using NetMiner.Core.pTask.Entity;
using NetMiner.Core.Event;
using NetMiner.Net.Common;
using System.Net;
using NetMiner.Core.Proxy.Entity;

///功能：采集任务信息处理  
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：任务格式版本已经修改至1.2
///
///修订：2010-04-15 任务格式版本修改至：1.8
///修订：2010-12-1 任务格式版本升级为：2.0
///修订：2012-1-30 优化用户体验，修改了网址导航的处理，增加了多页采集的功能、文件保存的独立设置功能
///采集任务版本升级为：3.0
///

namespace MinerSpider
{

    public partial class frmTask : Form
    {

        #region 属性

        private Single m_SupportTaskVersion = Single.Parse(Program.SominerTaskVersionCode);

        //public delegate void RIsShowWizard(bool IsShowWizard);
        //public RIsShowWizard RShowWizard;

        //定义一个访问资源文件的变量
        private ResourceManager rm;

        private string m_RegexNextPage;

        //此类别可处理的任务版本号，注意从1.3开始，任务处理不再向前兼容，需升级后方可操作
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public delegate void ReturnTaskClass(string tClass);
        public ReturnTaskClass rTClass;

        //是否已保存了任务，如果保存，即便在取消的时候，
        //也需要将任务所述分类进行返回，主要是用在“应用”
        //和“取消”按钮的判断上
        private bool IsSaveTask = false;

        //定义一个ToolTip
        ToolTip HelpTip = new ToolTip();
        //NetMiner.Gather.Task.cUrlAnalyze gUrl = new NetMiner.Gather.Task.cUrlAnalyze(Program.getPrjPath()); 

        //定义一个集合类用于存储Url地址对应的导航规则,因为当前界面无法显示所有的
        //导航规则，所以，需要通过一个结合类进行存储
        private List<eNavigRules> m_listNaviRules = new List<eNavigRules>();

        //定义一个集合，用于存储Url地址对应的多页采集规则，因为当前界面无法显示所有的
        //信息，需要通过一个集合类进行存储
        private List<eMultiPageRules> m_MultiPageRules = new List<eMultiPageRules>();

        //定义一个集合类用于存储采集字段的规则
        private List<cGatherRule> m_gRules = new List<cGatherRule>();
        //private List<cFieldRules> m_FieldRules = new List<cFieldRules>();

        //定义一个集合类，用于存储每个线程的代理信息
        private List<eThreadProxy> m_ThreadProxy = new List<eThreadProxy>();

        //定义一个值记录当前选择并编辑输出规则的字段
        private string SelectedFiled = "";

        //建立一个采集任务控制器，用于采集测试操作，采集测试
        //统一使用采集引擎来进行
        //private cGatherControl m_GatherControl;

        //定义一个值，判断当前是否正在进行采集测试
        private bool m_IsGathering = false;

        /// <summary>
        /// 0-未测试；1-测试网址规则；2-测试采集任务
        /// </summary>
        //private int m_TestType = 0;

        //定义一个值，表示当前是否正在进行导航测试
        private bool m_testNaving = false;

        //定义一个值，表示当前编辑的任务是否为远程任务
        private bool m_IsRemoteTask = false;

        private string m_iniTaskPath;
        public string m_iniTaskClassName;

        private int m_MainThreadID;

        #endregion

        public frmTask()
        {
            InitializeComponent();

            m_MainThreadID = Thread.CurrentThread.ManagedThreadId;

            this.listWebGetFlag.ImageClick += this.on_ImageClick;

            IniData();

            this.treeTestUrl.ExpandAll();

            //初始化采集控制器
            //NetMiner.Gather.Gather.cHashTree tmpUrls = null;
            //m_GatherControl = new cGatherControl(Program.getPrjPath(),false,ref tmpUrls);

            ////采集控制器事件绑定,绑定后,页面可以响应采集任务的相关事件
            //m_GatherControl.TaskManage.TaskCompleted += tManage_Completed;
            //m_GatherControl.TaskManage.TaskStarted += tManage_TaskStart;
            //m_GatherControl.TaskManage.TaskInitialized += tManage_TaskInitialized;
            //m_GatherControl.TaskManage.TaskStateChanged += tManage_TaskStateChanged;
            //m_GatherControl.TaskManage.TaskStopped += tManage_TaskStop;
            //m_GatherControl.TaskManage.TaskError += tManage_TaskError;
            //m_GatherControl.TaskManage.TaskFailed += tManage_TaskFailed;
            //m_GatherControl.TaskManage.TaskAborted += tManage_TaskAbort;
            //m_GatherControl.TaskManage.Log += tManage_Log;
            //m_GatherControl.TaskManage.GData += tManage_GData;

            //m_GatherControl.Completed += m_Gather_Completed;

            
        }

        #region 窗体进入的状态
        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState;}
            set { m_FormState = value; }
        }
        #endregion

        //设置ToolTip的信息
        private void SetTooltip()
        {
            HelpTip.SetToolTip(this.tTask, @"输入任务名称，任务名称可以是" + "\r\n" + @"中文或英文，但不允许出现.*\/!?|@$%^&,<>{}()[]#+,等" + "\r\n" + "此项为必填项");
            HelpTip.SetToolTip(this.comTaskClass, @"选择当前采集任务所属的分类");
            HelpTip.SetToolTip(this.IsUrlEncode, "如果网址中出现了中文字符，通常是会进行编码后再进行网页请求的\r\n在此指定编码格式，如果不知道编码类型，可使用我们提供的工具测试获取");
            HelpTip.SetToolTip(this.comUrlEncode, "如果网址中出现了中文字符，通常是会进行编码后再进行网页请求的\r\n在此指定编码格式，如果不知道编码类型，可使用我们提供的工具测试获取");
            HelpTip.SetToolTip(this.cmdUrlEncoding, @"点击此按钮可打开编/解码工具，进行相应的解码或编码操作，" + "\r\n" + "可用于验证实际的编码或解码类型");
            HelpTip.SetToolTip(this.button10, @"点击此按钮可打开获取Cookie的小工具，" + "\r\n" + "在工具中实际进行一次登录操作，系统会记录Cookie，确认即可");
            HelpTip.SetToolTip(this.comWebCode, "默认情况下，系统会自动获取编码类型，如果采集结果还出现了乱码\r\n则在此选择网页编码以纠正错误，如果不知道选择何种编码，可通过浏览器查看");
            HelpTip.SetToolTip(this.button2, @"测试网址解析，如果输入的网址较多，解析时间会较长，" + "\r\n" + "您也可随时中断网址测试工作");
            HelpTip.SetToolTip(this.cmdAddWeblink, @"点击增加采集网址" + "\r\n" + "每个采集任务都至少要包含一个有效的采集网址");
            HelpTip.SetToolTip(this.cmdEditWeblink, "点击修改当前选中的采集网址");
            HelpTip.SetToolTip(this.cmdDelWeblink, "点击删除选中的采集网址");
            HelpTip.SetToolTip(this.listWeblink, "显示当前已经配置好的采集网址");
            HelpTip.SetToolTip(this.IsExportGUrl, "选中此选项，可以自动输出当前采集数据的网址");
            HelpTip.SetToolTip(this.IsExportGDate, "选中此选项，可以自动输出采集数据的当前时间");
            HelpTip.SetToolTip(this.cmdRegex, "点击按钮可以打开正则测试器，系统将根据配置生成采集匹配规则，" + "\r\n" + "通过正则测试器来验证规则配置正确与否");
            HelpTip.SetToolTip(this.cmdUp, "采集规则配置需按照网页顺序由上至下，" + "\r\n" + "点击此按钮可以调整采集规则的顺序上移");
            HelpTip.SetToolTip(this.cmdDown, "采集规则配置需按照网页顺序由上至下，" + "\r\n" + "点击此按钮可以调整采集规则的顺序下移");
            HelpTip.SetToolTip(this.button3, "配置助手可以引导您完成采集规则的配置" + "\r\n" + "但对于Ajax、或者Json、或者XML结构的数据，配置助手不能使用");
            HelpTip.SetToolTip(this.cmdAddCutFlag, "增加采集数据的规则");
            HelpTip.SetToolTip(this.cmdEditWeblink, "编辑当前选中的采集数据规则");
            HelpTip.SetToolTip(this.cmdDelCutFlag, "删除当前选中的采集数据规则");
            HelpTip.SetToolTip(this.listWebGetFlag, "显示已经配置完成的采集数据规则");
            HelpTip.SetToolTip(this.IsPublishData, "启用数据发布操作，默认情况下采集数据会临时保存到本地磁盘，" + "\r\n" + "选择此选项，您可将采集的数据发布到指定的数据源或网站");
            HelpTip.SetToolTip(this.raInsertDB, "采集大数据量的时候一定要选择直接入库，直接入库将不再临时存储数据，" + "\r\n" + "采集的数据将直接存入数据库，适合大数据里的采集，譬如：几十万甚至上百万数据的采集操作");
            HelpTip.SetToolTip(this.udPublishThread, "设置发布的工作线程，如无特别的要求，请不要修改此项");
            HelpTip.SetToolTip(this.IsDelTempData, "选中此选项将在数据发布完成后，删除临时保存的采集数据，" + "\r\n" + "一般情况下不要选择此选项，否则将无法通过网络矿工浏览到已经采集到的数据");
            HelpTip.SetToolTip(this.comTemplate, "在此选择已经配置好的发布模板，用于当前数据的发布操作，" + "\r\n" + "发布模板可通过菜单 工具->发布模板 进行管理");
            HelpTip.SetToolTip(this.txtDomain, "在此输入需要发布到指定网站的域名地址");
            HelpTip.SetToolTip(this.txtUser, "在此输入发布到指定网站的登录用户名");
            HelpTip.SetToolTip(this.txtPwd, "在此输入发布到指定网站的密码");
            HelpTip.SetToolTip(this.txtTDbConn, "请通过下面的按钮设置发布到指定数据库的数据源信息");
            HelpTip.SetToolTip(this.button18, "点击此按钮设置发布到指定数据库的数据源信息");
            HelpTip.SetToolTip(this.dataParas, "在此根据所选模板的发布参数，逐一对应到采集数据规则，实现发布参数与采集数据关系的对应");
            HelpTip.SetToolTip(this.raExportTxt, "选中此选项，则将采集的数据发布到文本文件中");
            HelpTip.SetToolTip(this.raExportCSV, "选中此选项，则将采集的数据发布到CSV文件中");
            HelpTip.SetToolTip(this.raExportExcel, "选中此选项，则将采集的数据发布到Excel文件中");
            HelpTip.SetToolTip(this.raExportWord, "选中此选项，则将采集的数据发布到Word文件中");
            HelpTip.SetToolTip(this.IsIncludeHeader, "选中此选项则发布数据会自动包含采集数据规则的名称，否则只发布数据");
            HelpTip.SetToolTip(this.IsRowFile, "强制一条记录发布到一个文件中，指定文件后，系统会自动文件后面增加序号以进行存储");
            HelpTip.SetToolTip(this.cmdInsertFileDate, "如果此采集任务加入了采集计划，多次运行，插入时间参数，" + "\r\n" + "可以将每次采集数据导出的文档进行时间标记，方便文件管理");
            HelpTip.SetToolTip(this.cmdInsertFileDate, "点击此按钮，输入一个需要导出文件的文件名");
            HelpTip.SetToolTip(this.raExportAccess, "选中此选项，则将数据发布到Access数据库");
            HelpTip.SetToolTip(this.raExportMySql, "选中此选项，则将数据发布到Mysql数据库");
            HelpTip.SetToolTip(this.raExportMSSQL, "选中此选项，则将数据发布到MSSqlServer数据库");
            HelpTip.SetToolTip(this.raExportOracle, "选中此选项，则将数据发布到Oracle数据库");
            HelpTip.SetToolTip(this.txtDataSource , "在此输入数据库连接信息，也可点击右侧按钮进行设置");
            HelpTip.SetToolTip(this.button12, "点击此按钮，设置数据库连接信息");
            HelpTip.SetToolTip(this.comTableName, "再次选择数据表，也可以输出一个不存在的数据表，" + "\r\n" + "如果输入的数据表不存在，系统会自动建立此数据表");
            HelpTip.SetToolTip(this.txtInsertSql, "默认情况下系统会自动产生sql语句，如果您是非技术人员，" + "\r\n" + "请勿修改，如果您了解sql语法，请酌情修改");
            HelpTip.SetToolTip(this.IsSqlTrue, "默认情况下，系统通过自有方式发布数据，并会自动处理数据的合法性，" + "\r\n" + "您也可选中此项，强制提交sql语句发布数据，但请确保发布数据的合法性，" + "\r\n" + "此种方式更灵活，效率更高，但对技术要求也高");
            HelpTip.SetToolTip(this.button13, "如果您修改了Sql语句，可以点击此按钮，来插入需要发布的数据项");
            HelpTip.SetToolTip(this.button19, @"如果测试结果不正确，可以点击此按钮打开正则分析器，" + "\r\n" + "测试采集规则配置的情况，逐一调整修正错误");
            HelpTip.SetToolTip(this.button5, @"点击此按钮，系统将模拟正式采集来输出采集结果，以验证采集结果的正确性");
            HelpTip.SetToolTip(this.udThread, "设置采集的工作线程，默认情况下无需修改此项，" + "\r\n" + "同时也请注意：并非线程数越高采集效率就越高，采集效率是与当前的计算机性能、" + "\r\n" + "带宽及采集目标网站访问性能有关，但性能允许范围内，线程数越多，采集效率就越高");
            HelpTip.SetToolTip(this.udAgainNumber, "当偶尔出现网络问题或网站访问错误时，系统会重新对失败的网址进行采集，" + "\r\n" + "此处设置重新采集的次数，超过此次数，将继续采集下一条网址");
            HelpTip.SetToolTip(this.IsIgnore404, "当出现404错误时，可以忽略重试");
            HelpTip.SetToolTip(this.udGIntervalTime, "设置此选项可以控制采集的频率，即采集完成一条网址后停止一段时间继续采集下一条网址，" + "\r\n" + "设置此选项后，工作线程建议设置为1，此选项对防采网站特别有效");
            HelpTip.SetToolTip(this.udGIntervalTime1, "设置此选项可以将采集间隔在指定的范围内由系统随机选择进行采集。");
            HelpTip.SetToolTip(this.IsUrlAutoRedirect, "默认情况下允许网址重定向，符合网页浏览操作，但特殊情况，需禁止重定向操作，" + "\r\n" + "否则会无法采集数据或下载文件，至于如何设置需根据实际采集情况而定，一般情况下请不要修改此选项");
            HelpTip.SetToolTip(this.IsProxy, "默认情况是通过本机直接访问采集网站，但可设置此选项，通过代理访问网站，" + "\r\n" + "代理地址请通过系统参数设置，如果右侧选项为勾选，则轮流使用已经设置的代理地址");
            HelpTip.SetToolTip(this.IsProxyFirst, "默认情况选择使用代理后是一种代理轮询的机制，" + "\r\n" + "即轮流使用设置的所有代理地址，选中此选项后，可以强制永久使用第一个代理地址");
            HelpTip.SetToolTip(this.IsUrlNoneRepeat, "选中此选项后，无论此采集任务运行多少次，都不会对已经采集的地址再进行数据采集，" + "\r\n" + "此为排重机制，注意：系统排重只对最终的采集页面进行排重，适合定期运行的增量数据采集");
            HelpTip.SetToolTip(this.IsSucceedUrl, "选中此选项后，采集错误的网址将不再排重，您可再次运行采集任务，" + "\r\n" + "针对发生错误的网址进行重采，从而确保数据的完整性");
            HelpTip.SetToolTip(this.cmdSetHeader, "点击此按钮进行头部信息的设置操作");
            HelpTip.SetToolTip(this.cmdClearHeader, "点击此按钮清空已经设置的头部信息");
            HelpTip.SetToolTip(this.IsPluginsDealData, "选中此选项后，可以启用编辑采集数据的插件");
            HelpTip.SetToolTip(this.cmdBrowserPlugins2, "点击此按钮选择获取Cookie类插件文件");
            HelpTip.SetToolTip(this.cmdSetPlugins2, "点击此按钮设置选中插件的参数，如插件未提供参数设置，则点击此按钮无效");
            HelpTip.SetToolTip(this.cmdBrowserPlugins3, "点击此按钮选择数据编辑类插件文件");
            HelpTip.SetToolTip(this.cmdSetPlugins3, "点击此按钮设置选中插件的参数，如插件未提供参数设置，则点击此按钮无效");
            HelpTip.SetToolTip(this.IsNoDataStop, "选中此选项则启用无法采集到数据时的执行操作");
            HelpTip.SetToolTip(this.nNoDataStopCount, "设定一个值，超过此数值，则开始进行选中的执行操作");
            HelpTip.SetToolTip(this.comNoDataStopRule, "当采集符合无法采集到数据的条件时，进行的执行操作");
            HelpTip.SetToolTip(this.IsNoInsertStop, "选中此选项，则启用插入重复的执行操作，注意：此选项仅对“直接入库”有效");
            HelpTip.SetToolTip(this.nNoInsertStopCount, "设定一个值，超过此数值，则开始进行选中的执行操作");
            HelpTip.SetToolTip(this.IsRepeatStop, "选中此选项，则启用采集到重复记录的执行操作，" + "\r\n" + "注意：系统对重复记录的判断取决于您在采集规则中配置的不允许重复的数据项");
            HelpTip.SetToolTip(this.comRepeatStopRule, "当采集到重复记录时，执行的操作，" + "\r\n" + "注意：系统对重复记录的判断取决于您在采集规则中配置的不允许重复的数据项");
            HelpTip.SetToolTip(this.isNoneAllowSplit, "选中此选项，则此任务不再进行分解并由多台计算机协同采集完成，将由采集引擎独立采集完成");

            HelpTip.SetToolTip(this.txtSavePath, "采集数据会临时保存到您指定的目录下");
            HelpTip.SetToolTip(this.txtHeader, "HTTP Header属于高级配置，通常应用于采集验证较为严格的网站或进行在线数据发布");

            HelpTip.SetToolTip(this.txtStartPos, "如果需要限定采集页的采集范围，可在此输入开始采集的字符串，" + "\r\n" + @"输入后系统将在指定的范围内采集数据，仅对采集页有效");
            HelpTip.SetToolTip(this.txtEndPos, "如果需要限定采集页的采集范围，可在此输入结束采集的字符串，" + "\r\n" + @"输入后系统将在指定的范围内采集数据，仅对采集页有效");

        }

        public void NewTask(string TaskPath,string TaskClassName)
        {
            //if (TaskClassName == "")
            //    return;

            this.comTaskClass.Text = TaskClassName;
            this.comTaskClass.Tag = TaskPath;

            this.m_iniTaskPath = TaskPath;
            this.m_iniTaskClassName = TaskClassName;

            SetSelectedTClass(TaskPath);
            
        }

        public void EditTask(string TaskPath, string taskClassName, string TClassPath, string TaskName)
        {
            this.comTaskClass.Text = taskClassName;
            this.comTaskClass.Tag = TaskPath;

            this.m_iniTaskPath = TaskPath;
            this.m_iniTaskClassName = taskClassName;

            SetSelectedTClass(TaskPath);
            LoadTask(TClassPath, TaskName);
        }

        private void SetSelectedTClass(string tClassID)
        {
            for (int i=0;i<this.treeTClass.Nodes.Count ;i++)
            {
                if (this.treeTClass.Nodes[i].Name == "C" + tClassID)
                {
                    this.treeTClass.SelectedNode = this.treeTClass.Nodes[i];
                    break;
                }

                if (this.treeTClass.Nodes[i].Nodes != null && this.treeTClass.Nodes[i].Nodes.Count > 0)
                    SetSelectedTClass(this.treeTClass.Nodes[i], tClassID);
            }
        }

        private void SetSelectedTClass(TreeNode tNode, string tClassID)
        {
            for (int i=0;i<tNode.Nodes.Count ;i++)
            {
                if (tNode.Nodes[i].Name =="C" + tClassID)
                {
                    this.treeTClass.SelectedNode = tNode.Nodes[i];
                    break;
                }

                if (tNode.Nodes[i].Nodes != null && tNode.Nodes[i].Nodes.Count > 0)
                    SetSelectedTClass(tNode.Nodes[i], tClassID);
            }
        }

        

        public void Browser(string TClassPath, string TaskName)
        {
            LoadTask(TClassPath, TaskName);
        }

        /// <summary>
        /// 加载远程服务器任务
        /// </summary>
        /// <param name="tName">任务名称</param>
        public void LoadRemoteTask(string tName,string strXML)
        {
            int i = 0;
            m_listNaviRules = null;
            m_listNaviRules = new List<eNavigRules>();

            oTask t = new oTask(Program.getPrjPath());
            t.LoadTaskInfo(strXML);

            LoadTaskInfo(t);

            this.m_IsRemoteTask = true;
            this.Text= "远程采集任务编辑状态：" + this.Text;
        }

        private void LoadTask(string TClassPath, string TaskName)
        {
            //每次加载任务前，都必须将其重新置空
            
            m_listNaviRules = null;
            m_listNaviRules = new List<eNavigRules>();

            oTask t = new oTask(Program.getPrjPath());

            try
            {
                t.LoadTask(TClassPath + "\\" + TaskName);
            }
            catch (NetMinerException ex)
            {
                throw ex;
            }

            LoadTaskInfo(t);
        }

        private void LoadTaskInfo(oTask t)
        {
            int i = 0;

            #region 常规信息

            //开始判断任务版本号，如果任务版本号不匹配，则不进行任务信息的加载
            if (this.SupportTaskVersion !=t.TaskEntity.TaskVersion)
            {
                t = null;
                MessageBox.Show(rm.GetString("Info1"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            this.tTask.Text =t.TaskEntity.TaskName ;

            //if (t.TaskEntity.TaskClass == "")
            //{
            //    this.comTaskClass.Text = "";
            //}
            //else
            //{
            //    this.comTaskClass.Text = t.TaskEntity.TaskClass;
            //    oTaskClass tc = new oTaskClass(Program.getPrjPath ());
            //    this.comTaskClass.Tag = tc.GetTaskClassIDByName(t.TaskEntity.TaskClass);
            //    tc = null;

            //    SetSelectedTClass(this.comTaskClass.Tag.ToString ());
            //}

            #endregion

            #region 高级信息及其他

            //加载高级配置
            this.udAgainNumber.Value = t.TaskEntity.GatherAgainNumber;
            this.IsIgnore404.Checked = t.TaskEntity.IsIgnore404;
            this.IsDelTempData.Checked = t.TaskEntity.IsDelTempData;
            this.IsExportGUrl.Checked = t.TaskEntity.IsExportGUrl;
            this.IsExportGDate.Checked = t.TaskEntity.IsExportGDateTime;

            this.IsIncludeHeader.Checked = t.TaskEntity.IsExportHeader;
            this.IsRowFile.Checked = t.TaskEntity.IsRowFile;

            this.udGIntervalTime.Value = (decimal)t.TaskEntity.GIntervalTime;
            this.udGIntervalTime1.Value = (decimal)t.TaskEntity.GIntervalTime1;
            this.isMultiInterval.Checked = t.TaskEntity.IsMultiInterval;

            //V2.0提供
            this.IsProxy.Checked = t.TaskEntity.IsProxy;
            this.IsProxyFirst.Checked = t.TaskEntity.IsProxyFirst;

            //V2.1提供
            this.IsUrlNoneRepeat.Checked = t.TaskEntity.IsUrlNoneRepeat;
            this.IsSucceedUrl.Checked = t.TaskEntity.IsSucceedUrlRepeat;

            //V5提供
            this.IsUrlAutoRedirect.Checked = t.TaskEntity.IsUrlAutoRedirect;

            //V5.1增加
            this.IsNoDataStop.Checked = t.TaskEntity.IsGatherErrStop;
            this.nNoDataStopCount.Value =t.TaskEntity.GatherErrStopCount;
            this.comNoDataStopRule.Text = t.TaskEntity.GatherErrStopRule.GetDescription ();
            if (IsNoDataStop.Checked == true)
            {
                this.nNoDataStopCount.Enabled = true;
                this.comNoDataStopRule.Enabled = true;
            }
            else
            {
                this.nNoDataStopCount.Enabled = false;
                this.comNoDataStopRule.Enabled = false;
            }

            this.IsNoInsertStop.Checked = t.TaskEntity.IsInsertDataErrStop;
            this.nNoInsertStopCount.Value =t.TaskEntity.InsertDataErrStopConut ;
            if (this.IsNoInsertStop.Checked == true)
                this.nNoInsertStopCount.Enabled = true;
            else
                this.nNoInsertStopCount.Enabled = false;

            this.IsRepeatStop.Checked = t.TaskEntity.IsGatherRepeatStop;
            this.comRepeatStopRule.Text =t.TaskEntity.GatherRepeatStopRule.GetDescription();
            if (this.IsRepeatStop.Checked == true)
                this.comRepeatStopRule.Enabled = true;
            else
                this.comRepeatStopRule.Enabled = false;

            this.IsIgnoreErr.Checked = t.TaskEntity.IsIgnoreErr;
            this.IsAutoUpdateHeader.Checked = t.TaskEntity.IsAutoUpdateHeader;
            this.isNoneAllowSplit.Checked = t.TaskEntity.IsNoneAllowSplit;

            this.IsSplitDbUrls.Checked = t.TaskEntity.IsSplitDbUrls;

            //v5.31增加
            this.isCookieList.Checked = t.TaskEntity.isCookieList;
            if (this.isCookieList.Checked == true)
                this.txtCookie.WordWrap = false;
            else
                this.txtCookie.WordWrap = true;

            this.udGatherCount.Value = t.TaskEntity.GatherCount;
            this.udGatherCountPauseInterval.Value = (decimal)t.TaskEntity.GatherCountPauseInterval;
            this.txtRejectFlag.Text = t.TaskEntity.RejectFlag;
            switch(t.TaskEntity.RejectDeal)
            {
                case cGlobalParas.RejectDeal.None:
                    this.isNoneByPB.Checked = true;
                    break;
                case cGlobalParas.RejectDeal.StopGather:
                    this.cbStopGatherByPB.Checked = true;
                    break;
                case cGlobalParas.RejectDeal.UpdateCookie:
                    this.cbUpdateCookieByPB.Checked = true;
                    break;
                case cGlobalParas.RejectDeal.Error:
                    this.isThrowGatherErrByPB.Checked = true;
                    break;
                case cGlobalParas.RejectDeal.Coding:
                    this.isGatherCodingByPB.Checked = true;
                    break;
            }
           

            //V5.33增加
            this.m_ThreadProxy = t.TaskEntity.ThreadProxy;
            this.isSameSubTask.Checked = t.TaskEntity.isSameSubTask;

            this.isGatherCodingByPB.Checked =t.TaskEntity.isGatherCoding ;
            //this.txtCodingFlag.Text= t.GatherCodingFlag ;
            this.txtPluginsCodingByPB.Text= t.TaskEntity.GatherCodingPlugin  ;
            this.txtCodingUrlByPB.Text= t.TaskEntity.CodeUrl ;

            this.isCloseTab.Checked = t.TaskEntity.isCloseTab;

            #endregion

            #region HTTP Header
            //设置显示状态
            SetHeaderState(true);

            
            string strheader="";

            for (i = 0; i < t.TaskEntity.Headers.Count; i++)
            {
                strheader +=t.TaskEntity.Headers[i].Range + t.TaskEntity.Headers[i].Label + ":";
                    
                if (i==t.TaskEntity.Headers.Count -1)
                    strheader += t.TaskEntity.Headers[i].LabelValue ;
                else
                    strheader += t.TaskEntity.Headers[i].LabelValue + "\r\n";
            }

            this.txtHeader.Text = strheader;
            

            #endregion

            #region 触发器

            //this.IsStartTrigger.Checked = t.IsTrigger;
            //if(this.IsStartTrigger.Checked==true )
                //SetTriggerEnabled(t.IsTrigger);

            //if (t.TriggerType == ((int)cGlobalParas.TriggerType.GatheredRun).ToString ())
            //    this.raGatheredRun.Checked = true;
            //else if (t.TriggerType == ((int)cGlobalParas.TriggerType.PublishedRun).ToString ())
            //    this.raPublishedRun.Checked = true;

            //加载运行任务，触发器从V5.5去掉
            //if (this.IsStartTrigger.Checked == true)
            //{
            //    ListViewItem litem;

            //    for (i = 0; i <t.TriggerTask.Count ; i++)
            //    {
            //        litem = new ListViewItem();
            //        litem.Text = t.TriggerTask[i].RunTaskType.GetDescription();
            //        if (t.TriggerTask[i].RunTaskType == cGlobalParas.RunTaskType.DataTask)
            //            litem.SubItems.Add(((cGlobalParas.DatabaseType)(int.Parse(t.TriggerTask[i].RunTaskName))).GetDescription());
            //        else
            //            litem.SubItems.Add(t.TriggerTask[i].RunTaskName);
            //        litem.SubItems.Add(t.TriggerTask[i].RunTaskPara);

            //        this.listTask.Items.Add(litem);
            //    }
            //}

            #endregion

            #region  插件功能
            if (t.TaskEntity.IsPluginsCookie == true)
            {
                //this.IsPluginsCookie.Checked = true;
                //this.txtPluginsCookie.Text = t.PluginsCookie;
                //this.txtPluginsCookie.Enabled = true;
                //this.cmdBrowserPlugins1.Enabled = true;
                //this.cmdSetPlugins1.Enabled = true;
            }
            else
            {
                //this.IsPluginsCookie.Checked = false;
            }

            if (t.TaskEntity.IsPluginsDeal == true)
            {
                this.IsPluginsDealData.Checked = true;
                this.txtPluginsDeal.Text = t.TaskEntity.PluginsDeal;
                this.txtPluginsDeal.Enabled = true;
                this.cmdBrowserPlugins2.Enabled = true;
                this.cmdSetPlugins2.Enabled = true;
            }
            else
            {
                this.IsPluginsDealData.Checked = false;
            }

            if (t.TaskEntity.IsPluginsPublish == true)
            {
                this.txtPluginsPublish.Text = t.TaskEntity.PluginsPublish;
            }
            else
            {
            }
            #endregion

            #region 网址信息

            ListViewItem[] items=new ListViewItem[t.TaskEntity.WebpageLink.Count ];
           
            for (i = 0; i < t.TaskEntity.WebpageLink.Count;  i++)
            {
                items[i]=new ListViewItem();

                if (t.TaskEntity.WebpageLink[i].IsNavigation == true)
                {
                    eNavigRules cns = new eNavigRules();
                    cns.Url = t.TaskEntity.WebpageLink[i].Weblink.ToString();
                    cns.NavigRule = t.TaskEntity.WebpageLink[i].NavigRules;
                    
                    m_listNaviRules.Add(cns);
                }



                items[i].Name = t.TaskEntity.WebpageLink[i].id.ToString();
                items[i].Text = t.TaskEntity.WebpageLink[i].Weblink.ToString();
                if (t.TaskEntity.WebpageLink[i].IsNavigation == true)
                {
                    items[i].SubItems.Add("Y");
                    items[i].SubItems.Add(t.TaskEntity.WebpageLink[i].NavigRules.Count.ToString());
                }
                else
                {
                    items[i].SubItems.Add("N");
                    items[i].SubItems.Add("0");

                }
                items[i].SubItems.Add(t.TaskEntity.WebpageLink[i].NextPageRule);
                //if (t.WebpageLink[i].IsDoPostBack == true)
                //    items[i].SubItems.Add("Y");
                //else
                    items[i].SubItems.Add("N");

                items[i].SubItems.Add(t.TaskEntity.WebpageLink[i].NextMaxPage);

                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                items[i].SubItems.Add(cu.GetUrlCount(t.TaskEntity.WebpageLink[i].Weblink.ToString()).ToString());
                cu = null; 

                //处理多页采集规则
                if (t.TaskEntity.WebpageLink[i].IsMultiGather == true)
                {
                    eMultiPageRules cmps = new eMultiPageRules();
                    cmps.Url = t.TaskEntity.WebpageLink[i].Weblink.ToString();
                    cmps.MultiPageRule = t.TaskEntity.WebpageLink[i].MultiPageRules;

                    m_MultiPageRules.Add(cmps);
                    items[i].SubItems.Add("Y");
                }
                else
                {
                    items[i].SubItems.Add("N");
                }

                if (t.TaskEntity.WebpageLink[i].IsData121 == true)
                    items[i].SubItems.Add("Y");
                else
                    items[i].SubItems.Add("N");

                
            }
            this.listWeblink.Items.AddRange(items);
            items = null;

            #endregion

            #region 采集规则

            ListViewItem item;

            for (i = 0; i < t.TaskEntity.WebpageCutFlag.Count ; i++)
            {
                cGatherRule GRule = new cGatherRule();

                item=new ListViewItem() ;
                //item.Name =t.WebpageCutFlag[i].id.ToString ();
                item.Text =t.TaskEntity.WebpageCutFlag[i].Title.ToString ();
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].RuleByPage.GetDescription());
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].DataType.GetDescription() );

                item.SubItems.Add (t.TaskEntity.WebpageCutFlag[i].GatherRuleType.GetDescription( ));
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].XPath);
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].NodePrty);

                item.SubItems.Add (t.TaskEntity.WebpageCutFlag[i].StartPos.ToString ());
                item.SubItems.Add (t.TaskEntity.WebpageCutFlag[i].EndPos .ToString ());
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].LimitSign.GetDescription());

                item.SubItems.Add(t.TaskEntity.WebpageCutFlag [i].RegionExpression );
                if (t.TaskEntity.WebpageCutFlag[i].IsMergeData == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");

                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].NavLevel.ToString ());
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].MultiPageName.ToString());
                item.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].DownloadFileSavePath.ToString());
                //if (t.WebpageCutFlag[i].DownloadFileDealType.ToString() == "")
                //    item.SubItems.Add("");
                //else
                //    item.SubItems.Add(cGlobalParas.ConvertName(int.Parse (t.WebpageCutFlag[i].DownloadFileDealType.ToString())));

                if (t.TaskEntity.WebpageCutFlag[i].IsAutoDownloadFileImage == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");

                if (t.TaskEntity.WebpageCutFlag[i].IsAutoDownloadOnlyImage == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");

                //开始加载数据加工规则
                if (t.TaskEntity.WebpageCutFlag[i].ExportRules.Count > 0)
                {
                    //表示有数据加工规则
                    item.ImageIndex = 17;
                    this.listWebGetFlag.Items.Add(item);

                    //开始添加数据加工规则
                    for (int index = 0; index < t.TaskEntity.WebpageCutFlag[i].ExportRules.Count; index++)
                    {
                        ListViewItem dItem = new ListViewItem();
                        dItem.Name = t.TaskEntity.WebpageCutFlag[i].Title + "!Deal!-" + index.ToString();
                        dItem.Text = "  " + t.TaskEntity.WebpageCutFlag[i].ExportRules[index].FieldRuleType.GetDescription();
                        dItem.SubItems.Add(t.TaskEntity.WebpageCutFlag[i].ExportRules[index].FieldRule);
                        dItem.ForeColor = Color.Gray;
                        this.listWebGetFlag.Items.Add(dItem);
                    }
                }
                else
                {
                    this.listWebGetFlag.Items.Add(item);
                }

                item=null;

                //加载到集合中
                GRule.gName = t.TaskEntity.WebpageCutFlag[i].Title.ToString();
                GRule.gRuleByPage = (cGlobalParas.GatherRuleByPage)t.TaskEntity.WebpageCutFlag[i].RuleByPage;
                GRule.gType = t.TaskEntity.WebpageCutFlag[i].DataType.ToString ();
                GRule.gRuleType = (cGlobalParas.GatherRuleType)t.TaskEntity.WebpageCutFlag[i].GatherRuleType;
                GRule.xPath = t.TaskEntity.WebpageCutFlag[i].XPath;
                GRule.gNodePrty = t.TaskEntity.WebpageCutFlag[i].NodePrty;
                GRule.getStart = t.TaskEntity.WebpageCutFlag[i].StartPos.ToString();
                GRule.getEnd = t.TaskEntity.WebpageCutFlag[i].EndPos.ToString();
                GRule.limitType = t.TaskEntity.WebpageCutFlag[i].LimitSign.ToString ();

                GRule.strReg = t.TaskEntity.WebpageCutFlag[i].RegionExpression;
                GRule.IsMergeData = t.TaskEntity.WebpageCutFlag[i].IsMergeData;
                GRule.NaviLevel = t.TaskEntity.WebpageCutFlag[i].NavLevel.ToString();
                GRule.MultiPageName= t.TaskEntity.WebpageCutFlag[i].MultiPageName.ToString();
                GRule.sPath = t.TaskEntity.WebpageCutFlag[i].DownloadFileSavePath.ToString();
                //GRule.fileDealType = t.WebpageCutFlag[i].DownloadFileDealType;
                GRule.IsAutoDownloadFileImage = t.TaskEntity.WebpageCutFlag[i].IsAutoDownloadFileImage;
                GRule.IsAutoDownloadOnlyImage = t.TaskEntity.WebpageCutFlag[i].IsAutoDownloadOnlyImage;

                eFieldRules cfs = new eFieldRules();
                cfs.Field = t.TaskEntity.WebpageCutFlag[i].Title.ToString();
                cfs.FieldRule = t.TaskEntity.WebpageCutFlag[i].ExportRules;

                GRule.fieldDealRules = cfs;
                m_gRules.Add(GRule);

            }

            #endregion

            #region 发布规则 最后加载，因为如果调用了发布模版，系统会调用采集规则的数据
            //无论是否导出都记录导出的线程
            this.udPublishThread.Value = t.TaskEntity.PublishThread;

            if (t.TaskEntity.RunType==cGlobalParas.TaskRunType.OnlyGather)
            {
                //仅采集数据
                this.IsPublishData.Checked = false;
                this.groupBox4.Enabled = false;
                this.txtFileName.Text = "";
                this.comTableName.Text = "";
            }
            //else if(t.TaskEntity.RunType == cGlobalParas.TaskRunType.OnlySave)
            //{
            //    this.IsPublishData.Checked = true;
            //    this.raInsertDB.Enabled = true;

            //    switch (t.TaskEntity.ExportType)
            //    {
                    
            //        case cGlobalParas.PublishType.PublishAccess:
            //            this.raExportAccess.Checked = true;
            //            break;
            //        case cGlobalParas.PublishType.PublishMSSql:
            //            this.raExportMSSQL.Checked = true;
            //            break;
            //        case cGlobalParas.PublishType.PublishMySql:
            //            this.raExportMySql.Checked = true;
            //            break;
            //        case cGlobalParas.PublishType.publishOracle:
            //            this.raExportOracle.Checked = true;
            //            break;
            //        case cGlobalParas.PublishType.publishPlugin:
            //            this.raPublishByPlugin.Checked = true;
            //            break;
            //        default:
            //            break;
            //    }

            //    SetPublishTemplate();

             
            //    this.txtFileName.Text = t.TaskEntity.ExportFile;
            //    this.txtDataSource.Text = t.TaskEntity.DataSource;
            //    this.isSetServerDB.Checked = t.TaskEntity.isUserServerDB;

            //    this.comTableName.Text = t.TaskEntity.DataTableName;
            //    this.IsSqlTrue.Checked = t.TaskEntity.IsSqlTrue;
            //    this.txtInsertSql.Text = t.TaskEntity.InsertSql;

                

            //}
            else   //if (t.TaskEntity.RunType==cGlobalParas.TaskRunType.GatherExportData)
            {
               
                this.IsPublishData.Checked = true;
                
                    //存在导出任务
                switch (t.TaskEntity.ExportType)
                {
                    case cGlobalParas.PublishType.publishTemplate:
                        this.raPublishByTemplate.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishExcel:
                        this.raExportExcel.Checked = true;
                        this.raPublishFile.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishTxt:
                        this.raPublishFile.Checked = true;
                        this.raExportTxt.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishCSV:
                        this.raPublishFile.Checked = true;
                        this.raExportCSV.Checked = true;
                        break;
                    case cGlobalParas.PublishType.publishWord:
                        this.raPublishFile.Checked = true;
                        this.raExportWord.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishAccess:
                        this.raInsertDB.Checked = true;
                        this.raExportAccess.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishMSSql:
                        this.raInsertDB.Checked = true;
                        this.raExportMSSQL.Checked = true;
                        break;
                    case cGlobalParas.PublishType.PublishMySql:
                        this.raInsertDB.Checked = true;
                        this.raExportMySql.Checked = true;
                        break;
                    case cGlobalParas.PublishType.publishOracle:
                        this.raInsertDB.Checked = true;
                        this.raExportOracle.Checked = true;
                        break;
                    case cGlobalParas.PublishType.publishPlugin:
                        this.raPublishByPlugin.Checked = true;
                        break;
                    default:
                        break;
                }

                SetPublishTemplate();

                    if (this.raPublishByTemplate.Checked == true)
                    {
                        //表示采用发布模板进行数据发布操作
                        this.raPublishByTemplate.Enabled = true;
                        this.raPublishByTemplate.Checked = true;

                        this.comTemplate.Text = t.TaskEntity.TemplateName;
                        this.txtUser.Text = t.TaskEntity.User;
                        this.txtPwd.Text = t.TaskEntity.Password;
                        this.txtDomain.Text = t.TaskEntity.Domain;
                        this.txtTDbConn.Text = t.TaskEntity.TemplateDBConn;

                        this.dataParas.Rows.Clear();

                        //List<cPublishPara> paras = new List<cPublishPara>();
                        //paras = p.GetPublishParas(0);
                        for (int pi = 0; pi < t.TaskEntity.PublishParas.Count; pi++)
                        {
                            this.dataParas.Rows.Add();
                            this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = t.TaskEntity.PublishParas[pi].DataLabel;
                            cGlobalParas.PublishParaType pType = t.TaskEntity.PublishParas[pi].DataType;
                            if (pType == cGlobalParas.PublishParaType.NoData)
                            {
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                            }
                            if (pType == cGlobalParas.PublishParaType.Gather)
                            {
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = t.TaskEntity.PublishParas[pi].DataValue;
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                            }
                            else if (pType == cGlobalParas.PublishParaType.Custom)
                            {
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = "{手工输入}";
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = t.TaskEntity.PublishParas[pi].DataValue;
                            }
                        }
                    }
                    else
                    {
                        this.txtFileName.Text = t.TaskEntity.ExportFile;
                        this.txtDataSource.Text = t.TaskEntity.DataSource;
                        this.isSetServerDB.Checked = t.TaskEntity.isUserServerDB;
                        
                        this.comTableName.Text = t.TaskEntity.DataTableName;
                        this.IsSqlTrue.Checked = t.TaskEntity.IsSqlTrue;
                        this.txtInsertSql.Text = t.TaskEntity.InsertSql;
                        
                    }
                //}
                //else
                //{
                //    this.comRunType.SelectedIndex = 1;
                //}
            }


            this.txtSavePath.Text = t.TaskEntity.SavePath;
            this.udThread.Value = t.TaskEntity.ThreadCount;
            this.txtStartPos.Text  = t.TaskEntity.StartPos;
            this.txtEndPos.Text  = t.TaskEntity.EndPos;
            this.txtCookie.Text = t.TaskEntity.Cookie;
            this.comWebCode.SelectedItem =t.TaskEntity.WebCode.GetDescription();
            this.IsUrlEncode.Checked = t.TaskEntity.IsUrlEncode;
            if (t.TaskEntity.UrlEncode == cGlobalParas.WebCode.NoCoding)
            {
                this.comUrlEncode.SelectedIndex = -1;
            }
            else
            {
                this.comUrlEncode.SelectedItem = t.TaskEntity.UrlEncode.GetDescription();
            }

            this.isTwoUrlCode.Checked = t.TaskEntity.IsTwoUrlCode;

            #endregion

            t =null ;
            
        }

        #region 启动初始化数据 根据启动的状态进行加载：新建、修改、浏览等

        private void IniData()
        {
            //初始化页面加载数据

            //初始化资源文件的参数
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //根据当前的区域进行显示信息的加载
            ResourceManager rmPara = new ResourceManager("NetMiner.Resource.Resources.globalPara", Assembly.Load("NetMiner.Resource"));

            this.comWebCode.Items.Add(rmPara.GetString("WebCode2"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode3"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode4"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode5"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode6"));
            this.comWebCode.Items.Add(rmPara.GetString("WebCode7"));

            this.comWebCode.SelectedIndex = 0;

            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode3"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode4"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode5"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode6"));
            this.comUrlEncode.Items.Add(rmPara.GetString("WebCode8"));

            this.comUrlEncode.SelectedIndex = 0;

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.udGIntervalTime.Enabled = false;
                this.udGIntervalTime1.Enabled = false;
            }

            //初始化发布模版的信息
            cIndex tIndex = new cIndex(Program.getPrjPath());
            tIndex.GetData();
            int count = tIndex.GetCount();

            for (int i = 0; i < count; i++)
            {
                this.comTemplate.Items.Add(tIndex.GetTName(i) + "[" + tIndex.GetTType(i).GetDescription() + "]");
            }

            this.comTemplate.SelectedIndex = -1;



            this.comRepeatStopRule.Items.Add(rmPara.GetString("StopRule1"));
            this.comRepeatStopRule.Items.Add(rmPara.GetString("StopRule2"));

            this.comNoDataStopRule.Items.Add(rmPara.GetString("StopRule1"));
            this.comNoDataStopRule.Items.Add(rmPara.GetString("StopRule2"));

            rmPara = null;

            this.txtSavePath.Text = Program.getPrjPath() + "data\\" + this.tTask.Text + "_File";

            //this.labLogSavePath.Text = rm.GetString("Info2") + "：" + Program.getPrjPath() + "Log";

            //初始化页面加载时各个控件的状态

            //初始化任务分类
            //开始初始化树形结构,取xml中的数据,读取任务分类
            oTaskClass xmlTClass = new oTaskClass(Program.getPrjPath());
            TreeNode newNode = new TreeNode();

            for (int i = 0; i < xmlTClass.TaskClasses.Count; i++)
            {
                newNode = new TreeNode();
                newNode.Tag = xmlTClass.TaskClasses[i].tPath;
                newNode.Name = "C" + xmlTClass.TaskClasses[i].ID;
                newNode.Text = xmlTClass.TaskClasses[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (xmlTClass.TaskClasses[i].Children != null && xmlTClass.TaskClasses[i].Children.Count > 0)
                {
                    //加载子分类
                    LoadTreeClass(newNode, xmlTClass.TaskClasses[i].Children);
                }

                this.treeTClass.Nodes.Add(newNode);
                newNode = null;
            }
            xmlTClass = null;

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Program ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                this.raExportOracle.Enabled = true;
            else
                this.raExportOracle.Enabled = false ;

            if (Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                this.Tab5.Visible = true;
         
        }

        private void LoadTreeClass(TreeNode tNode, List<NetMiner.Core.gTask.Entity. eTaskClass> ets)
        {
            for (int i = 0; i < ets.Count; i++)
            {
                TreeNode newNode = new TreeNode();
                newNode.Tag = ets[i].tPath;
                newNode.Name = "C" + ets[i].ID;
                newNode.Text = ets[i].Name;
                newNode.ImageIndex = 15;
                newNode.SelectedImageIndex = 15;

                if (ets[i].Children != null && ets[i].Children.Count > 0)
                {
                    //加载子分类
                    LoadTreeClass(newNode, ets[i].Children);
                }

                tNode.Nodes.Add(newNode);
                newNode = null;
            }
        }

        #endregion

        #region 按钮及界面控制操作

        private void cmdCancel_Click(object sender, EventArgs e)
        {

            if (IsSaveTask == true && this.m_IsRemoteTask == false)
            {
                if (this.comTaskClass.Text =="")
                {
                    rTClass("");
                }
                else
                {
                    rTClass(this.comTaskClass.Text );
                }
            }

            //if (this.FormState == cGlobalParas.FormState.New)
            //{
            //    //RShowWizard(false);
            //}
            this.Close();
        }

        private void cmdAddWeblink_Click(object sender, EventArgs e)
        {
            AddUrl();
        }

        private void AddUrl()
        {
            frmAddGUrl f = new frmAddGUrl();
            f.fState = cGlobalParas.FormState.New;
            f.Cookie = this.txtCookie.Text;

            if (this.IsUrlEncode.Checked == true)
            {
                f.IsUrlEncoding = true;
                f.UrlCode = this.comUrlEncode.SelectedItem.ToString();
            }
            else
                f.IsUrlEncoding = false;
            f.wCode = this.comWebCode.SelectedItem.ToString();
            f.IsProxy = this.IsProxy.Checked;
            f.IsFirstProxy = this.IsProxyFirst.Checked;
            f.TaskType = cGlobalParas.TaskType.HtmlByUrl;

            f.rGUrl = new frmAddGUrl.ReturnGatherUrl(this.SaveGUrl);
            f.ShowDialog();

        }

        private void SaveGUrl(cGlobalParas.FormState fState, cGatherUrlFormConfig fgUrlRule)
        {
            this.errorProvider1.Clear();
            int UrlCount = 0;

            if (fState == cGlobalParas.FormState.New)
            {
                ListViewItem litem;
                litem = new ListViewItem();
                litem.Text = fgUrlRule.Url;

                if (fgUrlRule.IsNav  == true)
                {
                    //添加带有导航规则的网址

                    litem.SubItems.Add("Y");

                    eNavigRules m_listNaviRule = fgUrlRule.nRules ;

                    m_listNaviRules.Add(m_listNaviRule);

                    litem.SubItems.Add( fgUrlRule.nRules.NavigRule.Count.ToString ());

                    litem.SubItems.Add("");
                    litem.SubItems.Add("");
                    litem.SubItems.Add("");
                }
                else
                {
                    //添加普通网址

                    litem.SubItems.Add("N");
                    litem.SubItems.Add("0");

                    if (fgUrlRule.IsNext == true)
                    {
                        litem.SubItems.Add(fgUrlRule.NextRule);
                        litem.SubItems.Add("Aspx");
                        litem.SubItems.Add(fgUrlRule.MaxPageCount.ToString ());
                    }
                    else
                    {
                        litem.SubItems.Add("");
                        litem.SubItems.Add("");
                        litem.SubItems.Add("");
                    }
                }

                //计算网址数量

                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                UrlCount = cu.GetUrlCount(fgUrlRule.Url );
                cu = null;

                litem.SubItems.Add(UrlCount.ToString());

                //判断是否带有多页采集规则
                if (fgUrlRule.IsMulti == true)
                {
                    m_MultiPageRules.Add(fgUrlRule.MRules);
                    litem.SubItems.Add("Y");

                }
                else
                {
                    litem.SubItems.Add("N");
                }

                if(fgUrlRule.isMulti121==true )
                    litem.SubItems.Add("Y");
                else
                    litem.SubItems.Add("N");

                this.listWeblink.Items.Add(litem);
                litem = null;

            }
            else
            {

                this.listWeblink.SelectedItems[0].Text = fgUrlRule.Url;

                //删除多页规则
                for (int i = 0; i < m_MultiPageRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_MultiPageRules[i].Url)
                        m_MultiPageRules.Remove(m_MultiPageRules[i]);
                }

                if (fgUrlRule.IsNav == true)
                {
                    this.listWeblink.SelectedItems[0].SubItems[1].Text = "Y";

                    //删除存储的导航规则
                    for (int i = 0; i < m_listNaviRules.Count; i++)
                    {
                        if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                            m_listNaviRules.Remove(m_listNaviRules[i]);
                    }

             

                    m_listNaviRules.Add(fgUrlRule.nRules);

                    this.listWeblink.SelectedItems[0].SubItems[2].Text = fgUrlRule.nRules.NavigRule.Count.ToString ();

                    this.listWeblink.SelectedItems[0].SubItems[3].Text = "";
                    this.listWeblink.SelectedItems[0].SubItems[4].Text = "";
                    this.listWeblink.SelectedItems[0].SubItems[5].Text = "";
                }
                else
                {
                    this.listWeblink.SelectedItems[0].SubItems[1].Text = "N";
                    this.listWeblink.SelectedItems[0].SubItems[2].Text = "0";
                    if (fgUrlRule.IsNext == true)
                    {
                        this.listWeblink.SelectedItems[0].SubItems[3].Text = fgUrlRule.NextRule;
                        this.listWeblink.SelectedItems[0].SubItems[4].Text = "N";
                        this.listWeblink.SelectedItems[0].SubItems[5].Text = fgUrlRule.MaxPageCount.ToString();
                    }
                    else
                    {
                        this.listWeblink.SelectedItems[0].SubItems[3].Text = "";
                        this.listWeblink.SelectedItems[0].SubItems[4].Text = "";
                        this.listWeblink.SelectedItems[0].SubItems[5].Text = "";
                    }
                }


                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                UrlCount = cu.GetUrlCount(fgUrlRule.Url);
                cu = null;
                this.listWeblink.SelectedItems[0].SubItems[6].Text = UrlCount.ToString();

                //判断是否带有多页采集规则
                if (fgUrlRule.IsMulti == true)
                {
                   

                    m_MultiPageRules.Add(fgUrlRule.MRules );

                    this.listWeblink.SelectedItems[0].SubItems[7].Text = "Y";

                }
                else
                {
                    this.listWeblink.SelectedItems[0].SubItems[7].Text = "N";
                }

                if (fgUrlRule.isMulti121==true )
                    this.listWeblink.SelectedItems[0].SubItems[8].Text = "Y";
                else
                    this.listWeblink.SelectedItems[0].SubItems[8].Text = "N";
               
            }

            this.IsSave.Text = "true";
        }

        private List<string> AddDemoUrl(string SourceUrl,string Cookie, bool IsNavPage,List<eNavigRule> NavRule,
            cGlobalParas.WebCode urlCode, cGlobalParas.WebCode webCode, string referUrl, bool isAutoUpdateHeader,string strHeader, bool isUrlAutoRedirect)
        {
                string Url="";
                List<string> Urls=new List<string> ();
                List<string> rUrls=null;
                
                //Urls = gUrl.SplitWebUrl(SourceUrl);
                Urls.Add(SourceUrl);
               
                if (IsNavPage ==true )
                {
                    try
                    {
                        rUrls = GetTestUrl(Urls[0].ToString(), Cookie,NavRule, urlCode, webCode, referUrl, isAutoUpdateHeader, strHeader, isUrlAutoRedirect);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    Url = Urls[0].ToString();
                    rUrls.Add(Url);
                }

                Urls = null;
                return rUrls;

        }

        private void cmdGetCode_Click(object sender, EventArgs e)
        {

            //string Code = cTool.GetWebpageCode(this.txtWeblink.Text);
            //this.comCode.Text = Code;

            //wait.Dispose();

            //if (Code == "")
            //{
            //    MessageBox.Show("系统无法自动获取网页编码，可通过查看页面属性（Firefox）或查看页面编码（IE）来判断页面编码格式", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //}
        }

        private void cmdDelWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count == 0)
                return;

            //如果有导航需要删除导航的规则
            //删除存储的导航规则

            RemoveUrl();
            

        }

        private void RemoveUrl()
        {
            //删除导航
            if (this.listWeblink.SelectedItems[0].SubItems[1].Text.ToString() == "Y")
            {
                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                        m_listNaviRules.Remove(m_listNaviRules[i]);
                }
            }

            //删除多页
            if (this.listWeblink.SelectedItems[0].SubItems[7].Text.ToString() == "Y")
            {

                for (int i = 0; i < m_MultiPageRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_MultiPageRules[i].Url)
                        m_MultiPageRules.Remove(m_MultiPageRules[i]);
                }
            }

            if (this.listWeblink.SelectedItems.Count != 0)
            {
                this.listWeblink.Items.Remove(this.listWeblink.SelectedItems[0]);
            }

          

            this.IsSave.Text = "true";
        }

        private eTask FillTask(eTask t)
        {
            int i = 0;

            bool isVisual = false;

            #region 常规设置
            t.TaskName = this.tTask.Text;
            t.TaskDemo = "";
            t.TaskClass = cGlobalParas.TaskClass.Local;
            t.TaskType = cGlobalParas.TaskType.HtmlByUrl;

            if (this.IsPublishData.Checked == false)
                t.RunType = cGlobalParas.TaskRunType.OnlyGather;
            else if (this.IsPublishData.Checked == true)
            {
                if (this.raInsertDB.Checked == true)
                    t.RunType = cGlobalParas.TaskRunType.OnlySave;
                else
                    t.RunType = cGlobalParas.TaskRunType.GatherExportData;
            }
            if (this.txtSavePath.Text.Trim().ToString() == "")
                t.SavePath = Program.getPrjPath() + "data";
            else
                t.SavePath = this.txtSavePath.Text;

            t.ThreadCount = int.Parse(this.udThread.Value.ToString());
            t.StartPos = this.txtStartPos.Text;
            t.EndPos = this.txtEndPos.Text;
            t.Cookie = this.txtCookie.Text;
            t.WebCode = EnumUtil.GetEnumName<cGlobalParas.WebCode> (this.comWebCode.SelectedItem.ToString());
            t.IsUrlEncode = this.IsUrlEncode.Checked;
            if (this.IsUrlEncode.Checked == false)
            {
                t.UrlEncode =cGlobalParas.WebCode.utf8;
            }
            else
            {
                t.UrlEncode = EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString());
            }
            t.IsTwoUrlCode = this.isTwoUrlCode.Checked;

            #endregion

            #region 数据发布设置

            //判断是否导出文件，存储导出的配置信息
            //无论是否导出，都要记录导出的线程数
            t.PublishThread = int.Parse(this.udPublishThread.Value.ToString());
            if (t.RunType==cGlobalParas.TaskRunType.OnlySave || t.RunType == cGlobalParas.TaskRunType.GatherExportData)
            {
                if (this.raPublishByTemplate.Checked == true)
                {
                    #region 模板采集
                    t.ExportType = cGlobalParas.PublishType.publishTemplate;
                    t.TemplateName = this.comTemplate.Text;
                    t.User = this.txtUser.Text;
                    t.Password = this.txtPwd.Text;
                    t.Domain = this.txtDomain.Text;
                    t.TemplateDBConn = this.txtTDbConn.Text;

                    for (int pi = 0; pi < this.dataParas.Rows.Count; pi++)
                    {
                        ePublishData para = new ePublishData();
                        para.DataLabel = this.dataParas.Rows[pi].Cells[0].Value.ToString();
                        if (this.dataParas.Rows[pi].Cells[1].Value == null)
                        {
                            para.DataType = cGlobalParas.PublishParaType.NoData;
                            para.DataValue = "";
                        }
                        else
                        {
                            if ("{手工输入}" == this.dataParas.Rows[pi].Cells[1].Value.ToString())
                            {
                                para.DataType = cGlobalParas.PublishParaType.Custom;
                                if (this.dataParas.Rows[pi].Cells[2].Value == null)
                                    para.DataValue = "";
                                else
                                    para.DataValue = this.dataParas.Rows[pi].Cells[2].Value.ToString();
                            }
                            else
                            {
                                para.DataValue = this.dataParas.Rows[pi].Cells[1].Value.ToString();
                                para.DataType = cGlobalParas.PublishParaType.Gather;
                            }
                        }
                        t.PublishParas.Add(para);
                    }
                    #endregion
                }
                else
                {
                    #region 发布至文件或数据库
                    if (this.raExportTxt.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishTxt;
                    }
                    else if (this.raExportCSV.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishCSV;
                    }
                    else if (this.raExportExcel.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishExcel;
                    }
                    else if (this.raExportWord.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.publishWord;
                    }
                 
                    else if (this.raPublishByPlugin.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.publishPlugin;
                    }
                    else if (this.raExportAccess.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishAccess;
                    }
                    else if (this.raExportMSSQL.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishMSSql;
                    }
                    else if (this.raExportMySql.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.PublishMySql;
                    }
                    else if (this.raExportOracle.Checked == true)
                    {
                        t.ExportType = cGlobalParas.PublishType.publishOracle;
                    }
                    #endregion
                }

                t.ExportFile = this.txtFileName.Text.ToString();
                t.DataSource = this.txtDataSource.Text.ToString();
                t.isUserServerDB = this.isSetServerDB.Checked;
                t.DataTableName = this.comTableName.Text.ToString();
                t.IsSqlTrue = this.IsSqlTrue.Checked;
                t.InsertSql = this.txtInsertSql.Text;

            }
            else if (t.RunType == cGlobalParas.TaskRunType.OnlyGather)
            {
             
                t.ExportFile = "";
                t.DataSource = "";
                t.isUserServerDB = false;
                t.ExportType =cGlobalParas.PublishType.NoPublish;
                t.DataSource = "";
                t.DataTableName = "";

            }
            #endregion

            #region 高级及其他设置
            t.GatherAgainNumber = int.Parse(this.udAgainNumber.Value.ToString());
            t.IsIgnore404 = this.IsIgnore404.Checked;
            t.IsExportHeader = this.IsIncludeHeader.Checked;
            t.IsRowFile = this.IsRowFile.Checked;
            t.IsErrorLog = true;

            t.IsDelRepRow = false;
            t.IsDelTempData = this.IsDelTempData.Checked;

            t.IsSaveSingleFile = false;
            t.TempDataFile = Program.getPrjPath() + "data\\" + this.tTask.Text + ".xml";

            t.IsExportGUrl = this.IsExportGUrl.Checked;
            t.IsExportGDateTime = this.IsExportGDate.Checked;
            t.GIntervalTime = float.Parse(this.udGIntervalTime.Value.ToString());
            t.GIntervalTime1 = float.Parse(this.udGIntervalTime1.Value.ToString());
            t.IsMultiInterval = this.isMultiInterval.Checked;

            //V2.0提供
            t.IsProxy = this.IsProxy.Checked;
            t.IsProxyFirst = this.IsProxyFirst.Checked;

            //V2.1提供
            t.IsUrlNoneRepeat = this.IsUrlNoneRepeat.Checked;
            t.IsSucceedUrlRepeat = this.IsSucceedUrl.Checked;
            //V5提供
            t.IsUrlAutoRedirect = this.IsUrlAutoRedirect.Checked;

            //V5.1增加
            t.IsGatherErrStop = this.IsNoDataStop.Checked;
            t.GatherErrStopCount = int.Parse(this.nNoDataStopCount.Value.ToString());
            if (this.comNoDataStopRule.Text == "")
                t.GatherErrStopRule = cGlobalParas.StopRule.None;
            else
                t.GatherErrStopRule = EnumUtil.GetEnumName<cGlobalParas.StopRule> (this.comNoDataStopRule.Text);

            t.IsInsertDataErrStop = this.IsNoInsertStop.Checked;
            t.InsertDataErrStopConut = int.Parse(this.nNoInsertStopCount.Value.ToString());

            t.IsGatherRepeatStop = this.IsRepeatStop.Checked;
            if (this.comRepeatStopRule.Text == "")
                t.GatherRepeatStopRule = cGlobalParas.StopRule.None;
            else
                t.GatherRepeatStopRule = EnumUtil.GetEnumName<cGlobalParas.StopRule>(this.comRepeatStopRule.Text);

            //V5.2增加
            t.IsIgnoreErr = this.IsIgnoreErr.Checked;
            t.IsAutoUpdateHeader = this.IsAutoUpdateHeader.Checked;
            t.IsNoneAllowSplit = this.isNoneAllowSplit.Checked;
            t.IsSplitDbUrls = this.IsSplitDbUrls.Checked;

            //v5.31增加
            t.isCookieList = this.isCookieList.Checked;
            t.GatherCount = (int)this.udGatherCount.Value;
            t.GatherCountPauseInterval = (float)this.udGatherCountPauseInterval.Value;
            t.RejectFlag = this.txtRejectFlag.Text;


            if (this.isNoneByPB.Checked == true)
                t.RejectDeal = cGlobalParas.RejectDeal.None;
            if (this.cbStopGatherByPB.Checked == true)
                t.RejectDeal = cGlobalParas.RejectDeal.StopGather;
            if (this.cbUpdateCookieByPB.Checked == true)
                t.RejectDeal = cGlobalParas.RejectDeal.UpdateCookie;
            if (this.isThrowGatherErrByPB.Checked == true)
                t.RejectDeal = cGlobalParas.RejectDeal.Error;
            if (this.isGatherCodingByPB.Checked == true)
                t.RejectDeal = cGlobalParas.RejectDeal.Coding;
            

            //V5.33增加
            t.ThreadProxy = this.m_ThreadProxy;
            t.isSameSubTask = this.isSameSubTask.Checked;
            t.isGatherCoding = this.isGatherCodingByPB.Checked;
            //t.GatherCodingFlag = this.txtCodingFlag.Text;
            t.GatherCodingPlugin = this.txtPluginsCodingByPB.Text;
            t.CodeUrl = this.txtCodingUrlByPB.Text;

            t.isCloseTab = this.isCloseTab.Checked;

            #endregion

            #region HTTPHeader设置


            eHeader header;

            string strheader = this.txtHeader.Text;

            if (strheader != "")
            {
                foreach (string sc in strheader.Split('\r'))
                {
                    header = new eHeader();

                    string ss = sc.Trim();
                    if (ss.StartsWith("["))
                    {
                        string s1 = ss.Substring(0, ss.IndexOf("]") + 1);
                        header.Range = s1;
                        ss = ss.Replace(s1, "");
                    }
                    header.Label = ss.Substring(0, ss.IndexOf(":"));
                    header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                    t.Headers.Add(header);
                }
            }
            #endregion

            #region 触发器
            t.IsTrigger = false;

            //if (this.raGatheredRun.Checked == true)
            //    t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString();
            //else if (this.raPublishedRun.Checked == true)
            //    t.TriggerType = ((int)cGlobalParas.TriggerType.PublishedRun).ToString();

            //cTriggerTask tt;

            //开始添加触发器执行的任务
            //for (i = 0; i < this.listTask.Items.Count; i++)
            //{
            //    tt = new cTriggerTask();
            //    tt.RunTaskType = EnumUtil.GetEnumName<cGlobalParas.RunTaskType> (this.listTask.Items[i].Text);

            //    if (EnumUtil.GetEnumName<cGlobalParas.RunTaskType>(this.listTask.Items[i].Text) == cGlobalParas.RunTaskType.DataTask)
            //        tt.RunTaskName = EnumUtil.GetEnumName<cGlobalParas.DatabaseType>(this.listTask.Items[i].SubItems[1].Text.ToString()).ToString();
            //    else
            //        tt.RunTaskName = this.listTask.Items[i].SubItems[1].Text.ToString();

            //    tt.RunTaskPara = this.listTask.Items[i].SubItems[2].Text.ToString();

            //    t.TriggerTask.Add(tt);
            //}

            #endregion

            #region 插件信息
          
                t.IsPluginsCookie = false;
                t.PluginsCookie = "";
            

            if (this.IsPluginsDealData.Checked == true)
            {
                t.IsPluginsDeal = true;
                t.PluginsDeal = this.txtPluginsDeal.Text;
            }
            else
            {
                t.IsPluginsDeal = false;
                t.PluginsDeal = "";
            }

            if (this.raPublishByPlugin.Checked == true && this.IsPublishData.Checked == true)
            {
                t.IsPluginsPublish = true;
                t.PluginsPublish = this.txtPluginsPublish.Text;
            }
            else
            {
                t.IsPluginsPublish = false;
                t.PluginsPublish = "";
            }


            #endregion

            #region 水印信息
            //t.IsWatermark = this.IsWatermark.Checked;
            //t.WatermarkText = this.txtWatermark.Text;
            //t.FontFamily = this.comFontType.Text;
            //t.FontSize = int.Parse(this.upFontSize.Value.ToString());
            //t.IsFontWeight = this.IsFontWeight.Checked;
            //t.IsFontItalic = this.IsFontItalic.Checked;
            //t.FontColor = this.cmdFontColor.ForeColor.ToArgb().ToString();
            //t.WatermarkPOS = cGlobalParas.ConvertID(this.comWatermarkPos.Text);
            #endregion

            #region 网址信息
            List<eWebLink> wLinks = new List<eWebLink>();
            t.UrlCount = FillWebLinks(ref wLinks);
            t.WebpageLink = wLinks;
            #endregion

            #region 采集规则
            List<eWebpageCutFlag> wFlags = new List<eWebpageCutFlag>();
            FillGatherRule(ref wFlags);
            t.WebpageCutFlag = wFlags;
            #endregion

            #region 检测当前任务是否使用了可视化的操作
            //检测此任务中是否用到了XPATH可视化的配置
            //检测点为3个，导航规则、翻页规则及配置

            if (t.WebpageLink.Count > 0)
            {
                if (t.WebpageLink[0].NextPageRule.Contains("<XPath>"))
                    isVisual = true;

                if (isVisual == false)
                {
                    for (i = 0; i < t.WebpageLink.Count; i++)
                    {
                        for (int j = 0; j < t.WebpageLink[i].NavigRules.Count; j++)
                        {
                            if (t.WebpageLink[i].NavigRules[j].NavigRule.Contains("<XPath>")
                                || t.WebpageLink[i].NavigRules[j].NextRule.Contains("<XPath>"))
                            {
                                isVisual = true;
                                break;
                            }

                        }
                        if (isVisual == true)
                            break;
                    }
                }
            }

            if (isVisual==false)
            {
                for (i=0;i<t.WebpageCutFlag.Count;i++)
                {
                    if (t.WebpageCutFlag[i].XPath.Contains("<XPath>"))
                    {
                        isVisual = true;
                        break;
                    }

                }
            }

            t.IsVisual = isVisual;

            #endregion

            return t;
        }

        private int FillWebLinks(ref List<eWebLink> wLinks)
        {
            int UrlCount = 0;

            
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                UrlCount += int.Parse(this.listWeblink.Items[i].SubItems[6].Text);
            }
            //t.UrlCount = UrlCount;

            eWebLink w;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                w = new eWebLink();
                w.id = i;
                w.Weblink = this.listWeblink.Items[i].Text;
                if (this.listWeblink.Items[i].SubItems[1].Text == "N")
                {
                    w.IsNavigation = false;
                }
                else
                {
                    w.IsNavigation = true;

                    //添加导航规则
                    for (int m = 0; m < m_listNaviRules.Count; m++)
                    {
                        if (m_listNaviRules[m].Url == this.listWeblink.Items[i].Text)
                        {
                            w.NavigRules = m_listNaviRules[m].NavigRule;
                            break;
                        }
                    }

                }

                if (this.listWeblink.Items[i].SubItems[3].Text == "" || this.listWeblink.Items[i].SubItems[3].Text == null)
                {
                    w.IsNextpage = false;
                    w.NextPageRule = "";
                    w.NextMaxPage = "0";

                }
                else
                {
                    w.IsNextpage = true;
                    w.NextPageRule = this.listWeblink.Items[i].SubItems[3].Text;

                    w.NextMaxPage = this.listWeblink.Items[i].SubItems[5].Text;
                }

                //添加多页采集规则
                if (this.listWeblink.Items[i].SubItems[7].Text == "Y")
                {
                    w.IsMultiGather = true;
                    for (int m = 0; m < m_MultiPageRules.Count; m++)
                    {
                        if (m_MultiPageRules[m].Url == this.listWeblink.Items[i].Text)
                        {
                            w.MultiPageRules = m_MultiPageRules[m].MultiPageRule;
                            break;
                        }
                    }
                }
                else
                {
                    w.IsMultiGather = false;
                }

                if (this.listWeblink.Items[i].SubItems[8].Text == "Y")
                    w.IsData121 = true;
                else
                    w.IsData121 = false;



                wLinks.Add(w);
                w = null;
            }

            return UrlCount;
           
        }

        private void FillGatherRule(ref List<eWebpageCutFlag> wFlags)
        {
           

            eWebpageCutFlag c;
            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                if (this.listWebGetFlag.Items[i].Name == "")
                {
                    c = new eWebpageCutFlag();
                    c.id = i;
                    c.Title = this.listWebGetFlag.Items[i].Text;
                    c.RuleByPage = EnumUtil.GetEnumName<cGlobalParas.GatherRuleByPage>(this.listWebGetFlag.Items[i].SubItems[1].Text);
                    c.DataType = EnumUtil.GetEnumName<cGlobalParas.GDataType>(this.listWebGetFlag.Items[i].SubItems[2].Text);

                    c.GatherRuleType = EnumUtil.GetEnumName<cGlobalParas.GatherRuleType>(this.listWebGetFlag.Items[i].SubItems[3].Text);
                    c.XPath = this.listWebGetFlag.Items[i].SubItems[4].Text;
                    c.NodePrty = this.listWebGetFlag.Items[i].SubItems[5].Text;

                    c.StartPos = this.listWebGetFlag.Items[i].SubItems[6].Text;
                    c.EndPos = this.listWebGetFlag.Items[i].SubItems[7].Text;
                    c.LimitSign = EnumUtil.GetEnumName<cGlobalParas.LimitSign>(this.listWebGetFlag.Items[i].SubItems[8].Text);
                    c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[9].Text;
                    if (this.listWebGetFlag.Items[i].SubItems[10].Text == "Y")
                        c.IsMergeData = true;
                    else
                        c.IsMergeData = false;

                    c.NavLevel = int.Parse(this.listWebGetFlag.Items[i].SubItems[11].Text);

                    //多页名称
                    c.MultiPageName = this.listWebGetFlag.Items[i].SubItems[12].Text;

                    c.DownloadFileSavePath = this.listWebGetFlag.Items[i].SubItems[13].Text;


                    if (this.listWebGetFlag.Items[i].SubItems[14].Text == "Y")
                        c.IsAutoDownloadFileImage = true;
                    else
                        c.IsAutoDownloadFileImage = false;

                    if (this.listWebGetFlag.Items[i].SubItems[15].Text == "Y")
                        c.IsAutoDownloadOnlyImage = true;
                    else
                        c.IsAutoDownloadOnlyImage = false;

                    for (int n = 0; n < m_gRules.Count; n++)
                    {
                        if (this.listWebGetFlag.Items[i].Text == m_gRules[n].gName)
                        {
                            c.ExportRules = m_gRules[n].fieldDealRules.FieldRule;
                            break;
                        }
                    }



                    wFlags.Add(c);
                    c = null;
                }
            }
           
        }

        private void cmdOK_Click(object sender, EventArgs e)
        {

            if (this.listWeblink.Items.Count == 0 || this.listWebGetFlag.Items.Count == 0)
            {
                if (MessageBox.Show(rm.GetString("Quaere1"), rm.GetString ("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }

            try
            {
                if (this.IsSave.Text == "true")
                {
                    if (this.m_IsRemoteTask == false)
                    {
                        SaveLocalTask();
                        if (this.comTaskClass.Text =="")
                        {
                            rTClass("");
                        }
                        else
                        {
                            rTClass(this.comTaskClass.Text);
                        }


                        //RShowWizard(false);
                    }
                    else if (this.m_IsRemoteTask == true)
                        SaveRemoteTask();
                }
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error3") + ex.Message, rm.GetString ("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        #endregion

        #region 其他操作 输入检查
        //检查用户输入内容的有效性，只要任务有名称就可以保存，降低使用难度
        //但保存的任务不一定可以执行，需要在执行前做进一步修改
        private bool CheckInputvalidity()
        {
            this.errorProvider1.Clear();

            if (this.tTask.Text.ToString () == null || this.tTask.Text.Trim().ToString () == "")
            {
                this.errorProvider1.SetError(this.tTask, rm.GetString ("Error22"));
                return false ;
            }

            string[] ss={".","*",@"\",@"/","!","?","|","@","$","%","^","&",",","<",">","{","}","(",")","[","]","#","+"};
            bool isF = false;
            for (int i = 0; i < ss.Length; i++)
            {
                if (this.tTask.Text.IndexOf(ss[i]) > -1)
                {
                    isF = true;
                    break;
                }
            }

            if (isF==true )
            {
                this.errorProvider1.SetError(this.tTask, "采集任务名称中不能出现以下字符：.*\\/!?-|@$%^&,<>{}()[]#+");
                return false;
            }

            if (this.txtHeader.Text == "")
            {
                this.errorProvider1.SetError(this.txtHeader, rm.GetString("Error41"));
                return false;
            }

            //开始检测采集任务配置的正确性
            if (IsPublishData.Checked==true )
            {
                if (this.raInsertDB.Checked ==true )
                {
                    //使用数据库发布数据，验证其有效性
                    string sql = this.txtInsertSql.Text.Trim();
                    if (sql.IndexOf(";") > 0 && this.IsSqlTrue.Checked == false)
                    {
                        MessageBox.Show("发布规则错误：如果您使用多sql语句，请选中“直接提交sql语句发布数据”选项,如果不使用多sql语句，请去掉;", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (this.IsSqlTrue.Checked == false && !sql.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase))
                    {
                        MessageBox.Show("如果您要使用非insert语句，请选中“直接提交sql语句发布数据”选项", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    ////对insert语句进行分析
                    //if (sql.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    string pSql=sql.Substring (""
                    //}

                }
            }

            return true;
        }

        #endregion

        #region 传递给委托的方法
        private void GetDataSource(string strDataConn)
        {
            this.txtDataSource.Text = strDataConn;
        }

        private void GetCookie(string strCookie)
        {
            this.txtCookie.Text = strCookie;
        }

        private void GetExportCookie(string strCookie)
        {
           
        }

      
        #endregion

        private void listWeblink_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && this.listWeblink.SelectedItems.Count >0)
            {
                RemoveUrl();
            }

            if (e.KeyCode == Keys.Enter && this.listWeblink.SelectedItems.Count > 0)
            {
                EditUrlRule();
            }
        }

       
        private void listWebGetFlag_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.listWebGetFlag.Items.Count  == 0)
            {
                return;
            }

            if (e.KeyCode == Keys.Delete && this.listWebGetFlag.SelectedItems.Count >0)
            {
                DelGrule();
            }

            if (e.KeyCode == Keys.Enter && this.listWebGetFlag.SelectedItems.Count > 0)
            {
                EditGatherRule();
            }
        }

        private void DelGrule()
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
                return;

            if (this.listWebGetFlag.SelectedItems[0].Name == "")
            {
                //删除采集规则
                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (this.listWebGetFlag.SelectedItems[0].Text == m_gRules[i].gName)
                    {
                        m_gRules.Remove(m_gRules[i]);
                        break;
                    }
                }

                DelDealRule(this.listWebGetFlag.SelectedItems[0].Text, this.listWebGetFlag.SelectedItems[0].Index);

                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);
            }
            else
            {
                Match s = Regex.Match(this.listWebGetFlag.SelectedItems[0].Name, @"(?<=!Deal!-)\d*", RegexOptions.IgnoreCase);
                int index =int.Parse ( s.Groups[0].Value.ToString());
                string gName=string.Empty ;
                int ruleIndex=GetRuleIndex(this.listWebGetFlag.SelectedItems[0].Index);

                gName = this.listWebGetFlag.Items[ruleIndex].Text;

                //删除集合中的数据加工规则
                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (gName == m_gRules[i].gName)
                    {
                        //开始判断是第几个规则，然后从集合中删除第几个
                        m_gRules[i].fieldDealRules.FieldRule.RemoveAt(index);
                        break;
                    }
                }

                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                //开始调整显示的序号
                index = 0;
                for (int j = this.listWebGetFlag.Items[ruleIndex].Index + 1; j < this.listWebGetFlag.Items.Count; j++)
                {
                    string tmpDealName = this.listWebGetFlag.Items[j].Name;
                    if (tmpDealName.IndexOf ("!Deal!")>0)
                    {
                        tmpDealName = tmpDealName.Substring(0, tmpDealName.IndexOf("!Deal!"));
                        if (tmpDealName==gName)
                        {
                            this.listWebGetFlag.Items[j].Name = gName + "!Deal!-" + index.ToString();
                            index++;
                        }
                    }
                }

                if (index == 0)
                {
                    //表示此采集规则下的加工规则都已经删除完了
                    this.listWebGetFlag.Items[ruleIndex].ImageIndex = -1;
                }

            }
           
            this.IsSave.Text = "true";
        }

        private void frmTask_Load(object sender, EventArgs e)
        {
            this.Text+= Program.SominerTaskVersionCode;

            #region 识别当前的版本，并根据版本的不同加载不同的数据
            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.IsUrlNoneRepeat.Enabled = false;
                this.linkLabel1.Enabled = false;
                this.raInsertDB.Enabled = false;
            }

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.isNoneAllowSplit.Enabled = true;
                this.IsSplitDbUrls.Enabled = true;
            }
            else
            {
                this.isNoneAllowSplit.Enabled = false ;
                this.IsSplitDbUrls.Enabled = false ;
            }

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Program ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                this.txtRejectFlag.Enabled = true;
                this.isThrowGatherErrByPB.Enabled = true;
                this.button1.Enabled = true;
                this.isGatherCodingByPB.Enabled = true;
            }
            else
            {
                this.txtRejectFlag.Enabled = false ;
                this.isThrowGatherErrByPB.Enabled = false ;
                this.button1.Enabled = false;
                this.isGatherCodingByPB.Enabled = false;
                this.isGatherCodingByPB.Checked = false;
            }

            #endregion




            //对Tooltip进行初始化设置
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor =Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "小矿提示";

            SetTooltip();

            switch (this.FormState)
            {
                case cGlobalParas.FormState.New :
                    ModifyTitle("新建采集任务");
                    break;
                case cGlobalParas.FormState.Edit :
                    //编辑状态进来不能修改分类
                    //this.cmdWizard.Enabled = false;
                    //this.cmdAddByTask.Enabled = false;

                    this.tTask.ReadOnly = true;
                    this.comTaskClass.Enabled = false;

                    this.cmdWizard.Enabled = false;

                    break;
                case cGlobalParas.FormState.Browser :
                    SetFormBrowser();
                    break;
                default :
                    break ;
            }

            //ResourceManager rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Assembly.Load("NetMiner.Resource")());
            ////初始化导航规则的datagrid的表头
            //DataGridViewTextBoxColumn nRuleLevel = new DataGridViewTextBoxColumn();
            //nRuleLevel.HeaderText = rm.GetString("NaviLevel");
            //nRuleLevel.Width = 40;
            //this.dataNRule.Columns.Insert(0, nRuleLevel);

            //DataGridViewTextBoxColumn nRule = new DataGridViewTextBoxColumn();
            //nRule.HeaderText = rm.GetString("NaviRule");
            //nRule.Width = 240;
            //this.dataNRule.Columns.Insert(1, nRule);

            //rm = null;

            cXmlSConfig cCon = new cXmlSConfig(Program.getPrjPath());
            m_RegexNextPage = ToolUtil.ShowTrans(cCon.RegexNextPage);
            cCon = null;

            this.IsSave.Text = "false";
            this.tTask.Focus();

            this.ActiveControl = this.tTask;
        }

        private void SetFormBrowser()
        {
            //this.cmdWizard.Enabled = false;
            //this.cmdAddByTask.Enabled = false;

            this.button10.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.cmdAddWeblink.Enabled = false;
            this.cmdEditWeblink.Enabled = false;
            this.cmdDelWeblink.Enabled = false;


            this.cmdRegex.Enabled = false;

            this.cmdAddCutFlag.Enabled = false;
            this.cmdEditCutFlag.Enabled = false;
            this.cmdDelCutFlag.Enabled = false;


            this.cmdCancel.Text = "返 回";

            this.cmdOK.Enabled =false ;

            this.cmdApply.Enabled = false;
            this.cmdWizard.Enabled = false;
        }

        //定义一个代理用于修改任务的名称
        private delegate DataTable delegateGData(string Url,List<eWebpageCutFlag> gCutFlags, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax,bool IsExportGUrl,bool IsExportGDateTime);
        //private void GatherData()
        //{
        //    this.tabControl1.SelectedTab = this.tabControl1.TabPages[3];
        //    Application.DoEvents();

        //    if (this.listWeblink.Items.Count == 0)
        //    {
        //        MessageBox.Show(rm.GetString ("Info3"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        this.tabControl1.SelectedTab = this.tabControl1.TabPages[1];
        //        return ;
        //    }

        //    if (this.listWebGetFlag.Items.Count == 0)
        //    {
        //        MessageBox.Show(rm.GetString ("Info4"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        this.tabControl1.SelectedTab = this.tabControl1.TabPages[2];
        //        return ;
        //    }

        //    string tmpSavePath = this.txtSavePath.Text.ToString() + "\\" + this.tTask.Text.ToString() + "_file";

        //    bool IsAjax = false;

        //    if (cGlobalParas.ConvertID(this.TaskType.SelectedItem.ToString()) == (int)cGlobalParas.TaskType.AjaxHtmlByUrl)
        //        IsAjax = true;

        //    DataTable retValue=null ;

        //    //测试采集，根据用户定义的内容测试采集
        //    //验证数据是否正确
        //    //此部分内容有可能在下一版中单独一个页面来处理

        //    //首先判断是否要进行级联采集，测试是使用的是默认网址列表
        //    //中的第一位，首先获取网址，并添加相应的数据内容
        //    cWebLink w = new cWebLink();
        //    w.id = 0;
        //    w.Weblink = this.listWeblink.Items[0].Text;
        //    if (this.listWeblink.Items[0].SubItems[7].Text == "N")
        //        w.IsMultiGather = false;
        //    else
        //        w.IsMultiGather = true;

        //    if (this.listWeblink.Items[0].SubItems[1].Text == "N")
        //    {
        //        w.IsNavigation = false;
        //    }
        //    else
        //    {
        //        w.IsNavigation = true;

        //        //添加导航规则
        //        for (int m = 0; m < m_listNaviRules.Count; m++)
        //        {
        //            if (m_listNaviRules[m].Url == this.listWeblink.Items[0].Text)
        //            {
        //                w.NavigRules = m_listNaviRules[m].NavigRule;
        //                break;
        //            }
        //        }

        //    }
        //    if (this.listWeblink.Items[0].SubItems[3].Text == "" || this.listWeblink.Items[0].SubItems[3].Text == null)
        //    {
        //        w.IsNextpage = false;
        //    }
        //    else
        //    {
        //        w.IsNextpage = true;
        //        w.NextPageRule = this.listWeblink.Items[0].SubItems[3].Text;
        //    }

        //    if (w.IsNavigation == false)
        //    {
        //        //直接测试
        //        retValue=TestSingleData(w, tmpSavePath, IsAjax);
        //    }
        //    else
        //    {
        //        retValue=TestNaviData(w, tmpSavePath, IsAjax);
        //    }

        //    w = null;

        //    //绑定到显示的DataGrid中
        //    //this.dataTestGather.DataSource = retValue;

        //}

        //private DataTable TestSingleData(cWebLink w, string tmpSavePath, bool IsAjax)
        //{
        //    bool IsMerge = false;

        //    //判断是否已经提取了示例网址，如果没有，则进行提取
        //    if (this.txtWeblinkDemo.Text.ToString() == null || this.txtWeblinkDemo.Text.ToString() == "")
        //    {
        //        GetDemoUrl();
        //    }

        //    #region 添加采集标志

        //    cWebpageCutFlag c;
        //    List<cWebpageCutFlag> gFlag = new List<cWebpageCutFlag>();

        //    for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
        //    {
        //        c = new cWebpageCutFlag();
        //        c.id = i;
        //        c.Title = this.listWebGetFlag.Items[i].Text;
        //        c.RuleByPage = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[1].Text);

        //        c.DataType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[2].Text);
        //        c.GatherRuleType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[3].Text);
        //        c.XPath = this.listWebGetFlag.Items[i].SubItems[4].Text;
        //        c.NodePrty = this.listWebGetFlag.Items[i].SubItems[5].Text;
        //        c.StartPos = this.listWebGetFlag.Items[i].SubItems[6].Text;
        //        c.EndPos = this.listWebGetFlag.Items[i].SubItems[7].Text;
        //        c.LimitSign = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[8].Text);
        //        c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[9].Text;
        //        if (this.listWebGetFlag.Items[i].SubItems[10].Text == "Y")
        //        {
        //            c.IsMergeData = true;
        //            IsMerge = true;
        //        }
        //        else
        //            c.IsMergeData = false;

        //        c.NavLevel = int.Parse(this.listWebGetFlag.Items[i].SubItems[11].Text);
        //        c.MultiPageName = this.listWebGetFlag.Items[i].SubItems[12].Text;
        //        c.DownloadFileSavePath = this.listWebGetFlag.Items[i].SubItems[13].Text;
        //        c.DownloadFileDealType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[14].Text).ToString ();
        //        if (this.listWebGetFlag.Items[i].SubItems[15].Text == "Y")
        //            c.IsOcrText = true;
        //        else
        //            c.IsOcrText = false;

        //        if (this.listWebGetFlag.Items[i].SubItems[16].Text == "Y")
        //            c.IsAutoDownloadImage = true;
        //        else
        //            c.IsAutoDownloadImage = false;

        //        //添加采集数据的输出规则
        //        for (int n = 0; n < m_FieldRules.Count; n++)
        //        {
        //            if (this.listWebGetFlag.Items[i].Text == m_FieldRules[n].Field)
        //            {
        //                c.ExportRules = m_FieldRules[n].FieldRule;
        //                break;
        //            }
        //        }

        //        gFlag.Add(c);
        //        c = null;
        //    }

        //    #endregion

        //    DataTable retValue=null;

        //    if (IsMerge == true)
        //    {
        //        if (w.IsNavigation == true)
        //        {
        //            retValue = GatherTestSingleData(this.txtWeblinkDemo.Text, IsMerge, w.NavigRules[w.NavigRules.Count - 1].NaviNextPage, w.NavigRules[w.NavigRules.Count - 1].IsNaviNextDoPostBack, gFlag, tmpSavePath, IsAjax);
        //        }
        //        else
        //        {
        //            retValue = GatherTestSingleData(this.txtWeblinkDemo.Text, IsMerge, w.NextPageRule, w.IsDoPostBack ,gFlag, tmpSavePath, IsAjax);
        //        }
        //    }
        //    else
        //    {
        //        retValue = GatherTestSingleData(this.txtWeblinkDemo.Text, IsMerge, "", false,gFlag, tmpSavePath, IsAjax );
        //    }

        //    return retValue; 
        //}

        //private DataTable TestNaviData(cWebLink w,  string tmpSavePath, bool IsAjax)
        //{
        //    bool IsMerge = false;

        //    #region 导航测试

        //    DataTable retValue = null;

        //    if (w.NavigRules == null || w.NavigRules.Count ==0)
        //    {
        //        throw new NetMinerException("选中了导航选项却未配置导航规则，请检查采集网址配置信息");
        //    }

        //    if (w.NavigRules[0].IsGather == true)
        //    {

        //        #region 级联采集

        //        //增加采集的标志
        //        cWebpageCutFlag c;
        //        List<cWebpageCutFlag> gFlag = new List<cWebpageCutFlag>();

        //        for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
        //        {

        //            c = new cWebpageCutFlag();
        //            c.id = i;
        //            c.Title = this.listWebGetFlag.Items[i].Text;
        //            c.DataType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[1].Text);
        //            c.StartPos = this.listWebGetFlag.Items[i].SubItems[2].Text;
        //            c.EndPos = this.listWebGetFlag.Items[i].SubItems[3].Text;
        //            c.LimitSign = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[4].Text);
        //            c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[5].Text;
        //            if (this.listWebGetFlag.Items[i].SubItems[6].Text == "Y")
        //            {
        //                c.IsMergeData = true;
        //                IsMerge = true;
        //            }
        //            else
        //                c.IsMergeData = false;

        //            c.NavLevel = int.Parse(this.listWebGetFlag.Items[i].SubItems[7].Text);

        //            //添加采集数据的输出规则
        //            for (int n = 0; n < m_FieldRules.Count; n++)
        //            {
        //                if (this.listWebGetFlag.Items[i].Text == m_FieldRules[n].Field)
        //                {
        //                    c.ExportRules = m_FieldRules[n].FieldRule;
        //                    break;
        //                }
        //            }

        //            if (c.NavLevel == 1)
        //                gFlag.Add(c);

        //            c = null;
        //        }

        //        string Url = TransUrl(this.listWeblink.Items[0].Text.ToString());
        //        retValue = GatherTestSingleData(Url,false,"",false , gFlag, tmpSavePath, IsAjax);

        //        DataRow dr = retValue.Rows[retValue.Rows.Count - 1];

        //        //级联采集
        //        DataTable dt = null;

        //        for (int m = 0; m < w.NavigRules.Count; m++)
        //        {

        //            #region 增加采集标志
        //            gFlag = new List<cWebpageCutFlag>();
        //            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
        //            {

        //                c = new cWebpageCutFlag();
        //                c.id = i;
        //                c.Title = this.listWebGetFlag.Items[i].Text;
        //                c.DataType = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[1].Text);
        //                c.StartPos = this.listWebGetFlag.Items[i].SubItems[2].Text;
        //                c.EndPos = this.listWebGetFlag.Items[i].SubItems[3].Text;
        //                c.LimitSign = cGlobalParas.ConvertID(this.listWebGetFlag.Items[i].SubItems[4].Text);
        //                c.RegionExpression = this.listWebGetFlag.Items[i].SubItems[5].Text;
        //                if (this.listWebGetFlag.Items[i].SubItems[6].Text == "Y")
        //                {
        //                    c.IsMergeData = true;
        //                    IsMerge = true;
        //                }
        //                else
        //                    c.IsMergeData = false;

        //                c.NavLevel = int.Parse(this.listWebGetFlag.Items[i].SubItems[7].Text);

        //                //添加采集数据的输出规则
        //                for (int n = 0; n < m_FieldRules.Count; n++)
        //                {
        //                    if (this.listWebGetFlag.Items[i].Text == m_FieldRules[n].Field)
        //                    {
        //                        c.ExportRules = m_FieldRules[n].FieldRule;
        //                        break;
        //                    }
        //                }

        //                if (c.NavLevel == 0 && (m + 1) == w.NavigRules.Count)
        //                {
        //                    gFlag.Add(c);
        //                }
        //                else if (c.NavLevel == w.NavigRules[i].Level + 1)    //这个判断是多级导航的级联数据采集合并规则的对应关系，未测试，不知道正确与否
        //                {
        //                    gFlag.Add(c);
        //                }

        //                c = null;
        //            }

        //            List<string> rUrls = AddDemoUrl(Url, true, w.NavigRules);
        //            if (rUrls != null && rUrls.Count > 0)
        //                Url = rUrls[0].ToString();
        //            else

        //                return null;

        //            retValue = GatherTestSingleData(Url, false, "", false,gFlag, tmpSavePath, IsAjax);

        //            if (retValue != null && dr != null)
        //            {
        //                dt = MergeDataTable(dr, retValue);
        //            }

        //            #endregion
        //        }

        //        retValue = dt;

        //        #endregion

        //    }
        //    else
        //    {

        //        #region 导航采集测试
                
        //        retValue = TestSingleData(w,tmpSavePath, IsAjax);

        //        #endregion

        //    }

        //    #endregion

        //    return retValue;

        //}

        //private DataTable  GatherTestSingleData(string Url,bool IsMerger,string NextRule,bool IsDoPostBack, List<cWebpageCutFlag> gFlag,string tmpSavePath, bool IsAjax)
        //{
        //    DataTable retValue = new DataTable ();
        //    string OldUrl = "";
        //    string NewUrl = Url;

        //    int FirstNextIndex = 0;

        //    List<cHeader> headers = null;
        //    if (this.IsSetHeader.Checked == true && this.IsHeaderPublish.Checked == false)
        //    {
        //        headers = new List<cHeader>();
        //        cHeader header;

        //        string strheader = this.txtHeader.Text;

        //        if (strheader != "")
        //        {
        //            foreach (string sc in strheader.Split('\r'))
        //            {
        //                header = new cHeader();

        //                string ss = sc.Trim();
        //                header.Label = ss.Substring(0, ss.IndexOf(":"));
        //                header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

        //                headers.Add(header);

        //                header = null;
        //            }

        //        }

        //    }

        //    if (IsMerger == true)
        //    {
        //        while (OldUrl != NewUrl)
        //        {
        //            DataTable dt = GatherTestDataDelegation(NewUrl, gFlag, tmpSavePath, IsAjax);

        //            retValue.Merge(dt);

        //            //开始获取下一页的地址
        //            //cGatherWeb gWeb = new cGatherWeb();

        //            string sCookie = this.txtCookie.Text;

        //            cGatherWeb gWeb = new cGatherWeb();
        //            cGlobalParas.WebCode urlCode=cGlobalParas.WebCode.auto;
        //            if (this.IsUrlEncode.Checked == true)
        //                urlCode = (cGlobalParas.WebCode)int.Parse(this.comUrlEncode.Text);

        //            string webSource = gWeb.GetHtml(NewUrl, (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()),
        //                         this.IsUrlEncode.Checked , urlCode, ref sCookie, "", "", true, IsAjax);
        //            gWeb = null;

        //            //从配置文件中取出下一页的翻页正则规则
        //            cXmlSConfig cCon = new cXmlSConfig();
        //            string RegexNextPage = cTool.ShowTrans(cCon.RegexNextPage);
        //            cCon = null;

        //            //string NRule = RegexNextPage + "(?=" + NextRule + ")";
        //            //Match charSetMatch = Regex.Match(webSource, NRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        //            //string strNext = charSetMatch.Groups[1].Value;

        //            cUrlAnalyze cu = new cUrlAnalyze();
        //            string strNext = cu.GetNextUrl(NewUrl, webSource, NextRule, RegexNextPage, 
        //                (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()),this.IsUrlEncode.Checked ,urlCode, sCookie, IsDoPostBack, ref FirstNextIndex, headers);
        //            cu = null;
                    
        //            strNext = NewUrl;

        //            OldUrl = NewUrl;
        //            NewUrl = strNext;
        //        }

        //        retValue = MergeData(retValue, gFlag);

        //    }
        //    else
        //    {
        //        retValue = GatherTestDataDelegation(Url, gFlag, tmpSavePath, IsAjax);
        //    }

        //    return retValue;
        //}

        //private DataTable GatherTestDataDelegation(string Url,List<eWebpageCutFlag> gFlag,string tmpSavePath,bool IsAjax)
        //{
        //    //定义一个修改分类名称的委托
        //    delegateGData sd = new delegateGData(this.GatherTestData);

        //    //开始调用函数,可以带参数 
        //    IAsyncResult ir = sd.BeginInvoke(Url, gFlag,EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()), 
        //        this.txtCookie.Text.ToString(), this.txtStartPos.Text.Trim () , this.txtEndPos.Text.Trim () , 
        //        tmpSavePath, IsAjax,this.IsExportGUrl.Checked ,this.IsExportGDate .Checked, null, null);

        //    //显示等待的窗口 
        
        //    frmWaiting fWait = new frmWaiting(rm.GetString("Info5"));
        //    new Thread((ThreadStart)delegate
        //    {
        //        Application.Run(fWait);
        //    }).Start();

        
        //    //循环检测是否完成了异步的操作 
        //    while (true)
        //    {
        //        if (ir.IsCompleted)
        //        {
        //            //完成了操作则关闭窗口
                    
        //            break;
        //        }
        //    }

        //    //取函数的返回值 
        //    DataTable retValue = sd.EndInvoke(ir);

        //    if (fWait.IsHandleCreated)
        //        fWait.Invoke((EventHandler)delegate { fWait.Close(); });

        //    fWait = null;
        //    return retValue;
        //}
      
        //private DataTable GatherTestData(string Url,List<eWebpageCutFlag> gCutFlags, cGlobalParas.WebCode webCode, string cookie, string startPos, string endPos, string sPath, bool IsAjax,bool IsExportGUrl,bool IsExportGDateTime)
        //{
        //    cGatherWeb gData = new cGatherWeb(Program.getPrjPath());
        //    gData.CutFlag = gCutFlags;

           
        //        List<eHeader> headers = new List<eHeader>();
        //        eHeader header;

        //        string strheader = this.txtHeader.Text;

        //        if (strheader != "")
        //        {
        //            foreach (string sc in strheader.Split('\r'))
        //            {
        //                header = new eHeader();

        //                string ss = sc.Trim();
        //                if (ss.StartsWith("["))
        //                {
        //                    string s1 = ss.Substring(0, ss.IndexOf("]")+1);
        //                    header.Range = s1;
        //                    ss = ss.Replace(s1, "");
        //                }
        //                header.Label = ss.Substring(0, ss.IndexOf(":"));
        //                header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

        //                headers.Add(header);

        //                header = null;
        //            }

        //            gData.Headers = headers;
        //        }
            

        //    cGlobalParas.WebCode urlCode = cGlobalParas.WebCode.auto;

        //    DataTable dGather = new DataTable();
        //    try
        //    {
        //        if (this.IsUrlEncode.Checked == true)
        //            urlCode = (cGlobalParas.WebCode)int.Parse(this.comUrlEncode.Text);

        //        dGather = gData.GetGatherData(Url, startPos, endPos, sPath, IsExportGUrl, IsExportGDateTime,null,
        //            string.Empty, 0, string.Empty,cGlobalParas.RejectDeal.None);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        MessageBox.Show(rm.GetString ("Error4") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return null;
        //    }

        //    return dGather;

        //}


        private void button3_Click_1(object sender, EventArgs e)
        {
            frmDict dfrm = new frmDict();
            dfrm.ShowDialog();
            dfrm.Dispose();

        }

        private void menuOpenDict_Click(object sender, EventArgs e)
        {
            frmDict d = new frmDict();
            d.ShowDialog();
            d.Dispose();
        }


        //private void button6_Click(object sender, EventArgs e)
        //{
        //    if (this.dataNRule.Rows.Count ==0)
        //    {
        //        MessageBox.Show(rm.GetString ("Error5"), rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    List<cNavigRule> cns;
        //    cNavigRule cn;

        //    string nUrl = this.txtWebLink.Text;

        //    //先把字典表中的数据提取重来，这样做的目的是为了在编码和base64的时候
        //    //出现将字典分类也错误编码造成错误
        //    string strMatch = "(?<={Dict:)[^}]*(?=})";
        //    Match ss = Regex.Match(nUrl, strMatch, RegexOptions.IgnoreCase);
        //    string UrlPara = ss.Groups[0].Value;

        //    if (UrlPara != "")
        //    {
        //        cUrlAnalyze cua = new cUrlAnalyze();
        //        List<string> uDicts = cua.getListUrl( "Dict:" + UrlPara);
        //        cua = null;

        //        nUrl = Regex.Replace(nUrl, @"(?={Dict:)[^}]*(})", uDicts[0].ToString(), RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //    }

        //    //先行进行网址编码和Base64的问题
        //    //获取网址后，首先进行Url编码和Base64的处理
        //    if (this.IsUrlEncode.Checked == true)
        //        nUrl = cTool.UrlEncode(nUrl, (cGlobalParas.WebCode)(cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString())));

        //    //在此处理是否需要进行Base64编码的的问题
        //    if (Regex.IsMatch(nUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
        //    {

        //        Match s = Regex.Match(nUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
        //        string sBase64 = s.Groups[0].Value.ToString();
        //        sBase64 = cTool.Base64Encoding(sBase64);

        //        //将base64编码部分进行url替换
        //        nUrl=Regex.Replace(nUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //    }

        //    //需判断当前的网址是否含有参数，如果有参数则需要分解参数
        //    //当前的做法是是否含有参数都去分解一次，如果没有参数，则返回原址

        //    cProxyControl pControl = new cProxyControl(0);

        //    cUrlAnalyze cu = new cUrlAnalyze(ref pControl,this.IsProxy.Checked ,this.IsProxyFirst.Checked);

        //    List<string> nUrls= cu.SplitWebUrl(nUrl);
        //    cu = null;

        //    if (nUrls == null || nUrls.Count == 0)
        //    {
        //        MessageBox.Show(rm.GetString ("Error6"), "有可能导航规则出错，未能导航出网址！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    nUrl = nUrls[0].ToString();
        //    nUrls = null;

        //    if (this.IsUrlEncode.Checked == true)
        //        nUrl = cTool.UrlEncode(nUrl, (cGlobalParas.WebCode)(cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString())));
           
        //    //测试导航的时候，无需处理分页的一些问题
        //    for (int m = 0; m < this.dataNRule.Rows.Count; m++)
        //    {
        //        cns= new List<cNavigRule>();

        //        cn = new cNavigRule();
        //        cn.Url = nUrl;
        //        cn.Level = 1;
        //        //cn.NavigRule = this.dataNRule.Rows[m].Cells[5].Value.ToString();
        //        //cn.NaviStartPos = this.dataNRule.Rows[m].Cells[6].Value.ToString();
        //        //cn.NaviEndPos = this.dataNRule.Rows[m].Cells[7].Value.ToString();

        //        cn.NaviStartPos = this.dataNRule.Rows[m].Cells[6].Value.ToString();
        //        cn.NaviEndPos = this.dataNRule.Rows[m].Cells[7].Value.ToString();
        //        cn.NavigRule = this.dataNRule.Rows[m].Cells[5].Value.ToString();

               

        //        cns.Add(cn);

        //        try
        //        {
        //            nUrl = GetTestUrl(nUrl, cns);
        //        }
        //        catch (System.Exception ex)
        //        {
        //            MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }
        //    }

        //    string Url = nUrl;

        //    if (!Regex.IsMatch(Url, @"(http|https|ftp)+://[^\s]*"))
        //    {
        //        MessageBox.Show(rm.GetString ("Error6") + " 导航匹配结果为：" + Url, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return ;
        //    }

        //    try
        //    {

        //        System.Diagnostics.Process.Start(Url);
        //        this.txtWeblinkDemo.Text = Url;

        //    }
        //    catch (System.Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //    pControl = null;

        //}

        private delegate List<string> delegateGNavUrl(string webLink, List<eNavigRule> NavRule,cGlobalParas.WebCode webCode,
            bool isUrlCode,bool isTwoUrlCode, cGlobalParas.WebCode urlCode, ref string cookie, 
            string referUrl, bool isAutoUpdateHeader,string strHeader, bool isUrlAutoRedirect);
        private List<string> GetTestUrl(string webLink,string cookie, List<eNavigRule> NavRule,cGlobalParas.WebCode urlCode,
            cGlobalParas.WebCode webCode, string referUrl, bool isAutoUpdateHeader, string strHeader, bool isUrlAutoRedirect)
        {
            //定义一个获取导航网址的委托
            delegateGNavUrl sd = new delegateGNavUrl(this.GetNavUrl);

            ////开始调用函数,可以带参数 
            //string cookie = this.txtCookie.Text;

            object result = this.Invoke(sd, new object[] {
                webLink, NavRule,
                webCode ,
                this.IsUrlEncode.Checked ,this.isTwoUrlCode.Checked,urlCode, cookie,referUrl,isAutoUpdateHeader,this.txtHeader.Text , isUrlAutoRedirect
            });

            //IAsyncResult ir = sd.BeginInvoke(webLink, NavRule,
            //    webCode ,
            //    this.IsUrlEncode.Checked ,this.isTwoUrlCode.Checked,
            //    urlCode, 
            //    ref cookie,referUrl,isAutoUpdateHeader,this.txtHeader.Text , isUrlAutoRedirect ,null, null);



            ////循环检测是否完成了异步的操作 
            //while (true)
            //{
            //    if (ir.IsCompleted)
            //    {
            //        //完成了操作则关闭窗口
            //        break;
            //    }

            //}

            ////取函数的返回值 
            //List<string> rUrls = sd.EndInvoke(ref cookie,ir);

            //this.txtCookie.Text = cookie;

            List<string> rUrls = (List<string>)result;
            //SetValue((this.txtCookie), "Text", cookie);
            return rUrls;
        }

        //每次仅能获取一次导航，不能获取多次导航的网址，即当前只支持一层导航，多层导航的探测
        //需要重复调用此地址
        private List<string> GetNavUrl(string webLink, List<eNavigRule> NavRule,cGlobalParas.WebCode webCode,
            bool isUrlCode,bool isTwoUrlCode, cGlobalParas.WebCode urlCode, ref string cookie, string referUrl,
            bool isAutoUpdateHeader, string strHeader, bool isUrlAutoRedirect)
        {
            List<string> Urls;

            try
            {
                List<eHeader> headers = null;
               
                    headers = new List<eHeader>();
                    eHeader header;

                    string strheader = strHeader;

                    if (strheader != "")
                    {
                        foreach (string sc in strheader.Split('\r'))
                        {
                            header = new eHeader();

                            string ss = sc.Trim().Replace("\n","");
                            if (ss.StartsWith("["))
                            {
                                string s1 = ss.Substring(0, ss.IndexOf("]")+1);
                                header.Range = s1;
                                ss = ss.Replace(s1, "");
                            }
                            header.Label = ss.Substring(0, ss.IndexOf(":"));
                            header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                            headers.Add(header);

                            header = null;
                        }
                        
                    }

                //cGlobalParas.WebCode urlCode = cGlobalParas.WebCode.auto;
                //if (this.IsUrlEncode.Checked == true)
                //    urlCode = (cGlobalParas.WebCode)int.Parse(this.comUrlEncode.Text);

                cUrlAnalyze cu = new cUrlAnalyze(Program.getPrjPath());

                bool isVisual = false;
                if (NavRule[0].NavigRule.Contains("<XPath>"))
                    isVisual = true;

                string htmlSource = GetHtml(webLink, referUrl,"","", isVisual);
                Urls = cu.ParseUrlRule(webLink, htmlSource, NavRule,headers);
                cu = null;

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show ("获取导航网址出错，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw ex;
            }

            if (Urls == null || Urls.Count == 0)
                return null;

            return Urls ;

        }

        private List<string> GetMultiUrl(string webLink, string strHeader, string multiRule)
        {
            cUrlAnalyze cu = new cUrlAnalyze(Program.getPrjPath());

            bool isVisual = false;

            List<eHeader> headers = null;

            headers = new List<eHeader>();
            eHeader header;

            string strheader = strHeader;

            if (strheader != "")
            {
                foreach (string sc in strheader.Split('\r'))
                {
                    header = new eHeader();

                    string ss = sc.Trim().Replace("\n", "");
                    if (ss.StartsWith("["))
                    {
                        string s1 = ss.Substring(0, ss.IndexOf("]") + 1);
                        header.Range = s1;
                        ss = ss.Replace(s1, "");
                    }
                    header.Label = ss.Substring(0, ss.IndexOf(":"));
                    header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                    headers.Add(header);

                    header = null;
                }

            }

            string htmlSource = GetHtml(webLink, "", "", "", isVisual);
            List<string> Urls = cu.GetUrlsByRule(webLink, htmlSource, multiRule,1,cGlobalParas.NaviRunRule.Normal ,"","", headers);
            cu = null;

            return Urls;
        }

        private void cmdOKRun_Click(object sender, EventArgs e)
        {

        }

        private void GetDemoUrl(bool isAll, string Cookie, cGlobalParas.WebCode webCode, cGlobalParas.WebCode urlCode,List<eWebLink> wLinks)
        {
            ExportGatherLog(cGlobalParas.LogType.Warning, "网址导航规则测试仅对第一条网址进行测试，目的是为了检测规则的正确性，不做完整测试！");


            bool IsNav = false;

            #region 获取头部配置信息
            List<eHeader> headers = null;
           
                headers = new List<eHeader>();
                eHeader header;

                string strheader = this.txtHeader.Text;

                if (strheader != "")
                {
                    foreach (string sc in strheader.Split('\r'))
                    {
                        header = new eHeader();

                        string ss = sc.Trim();
                        if (ss.StartsWith("["))
                        {
                            string s1 = ss.Substring(0, ss.IndexOf("]")+1);
                            header.Range = s1;
                            ss = ss.Replace(s1, "");
                        }
                        header.Label = ss.Substring(0, ss.IndexOf(":"));
                        header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                        headers.Add(header);

                        header = null;
                    }

                }

            

            #endregion

            List<string> Urls=new List<string> ();
           
            List<eNavigRule> cns = new List<eNavigRule>();
            string DemoUrl="";
            DemoUrl = wLinks[0].Weblink ;

            cProxyControl PControl = new cProxyControl(Program.getPrjPath (), 0);

            //解析网址参数

            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath ());

            try
            {
                Urls = cu.SplitWebUrl(DemoUrl);
            }
            catch (System.Exception ex)
            {
                ExportGatherLog(cGlobalParas.LogType.Error,"网址参数解析失败，请检查字典的配置，如果是内存溢出，请试着将参数值减少！错误信息：" + ex.Message );
                return;
            }
            cu = null;

            int count = Urls.Count;
            if (count > 1)
                count = 1;

            //第一层网址
            for (int i = 0; i < count; i++)
            {
                string sUrl = Urls[i].ToString();

                try
                {
                    InvokeMethod(this, "AddNode", new object[] { this.treeTestUrl.Nodes, sUrl });
                }
                catch { }
            }

            cns = wLinks[0].NavigRules;

            TreeNodeCollection tNodeCol = this.treeTestUrl.Nodes;
            try
            {
                GetNaviUrl(tNodeCol, Cookie, cns, urlCode, webCode, headers, 1, isAll, "",
                                 this.IsAutoUpdateHeader.Checked, this.txtHeader.Text, this.IsUrlAutoRedirect.Checked);
            }
            catch(System.Exception ex)
            {
                InvokeMethod(this, "AddUrlLog", new object[] { "导航发生错误：" + ex.Message });
            }

            //处理入口至多页规则
            if(wLinks[0].IsMultiGather==true )
            {
                for (int m=0;m<wLinks[0].MultiPageRules.Count;m++)
                {
                    List<string> multiUrls= GetMultiUrl(wLinks[0].Weblink, this.txtHeader.Text, wLinks[0].MultiPageRules[m].Rule);

                    if (multiUrls!=null && multiUrls.Count >0)
                    InvokeMethod(this, "AddNode", new object[] { this.treeTestUrl.Nodes, multiUrls[0] + " [多页]" });
                }
            }

            PControl = null;
            
        }

        private void GetNaviUrl(TreeNodeCollection tNodeCol,string Cookie, List<eNavigRule> cns, 
            cGlobalParas.WebCode urlCode,cGlobalParas.WebCode webCode, List<eHeader> headers,
            int level, bool isAll, string referUrl, bool isAUpdateHeader,string strHeader, bool isUrlAutoRedirect)
        {
            foreach (TreeNode tNode in tNodeCol)
            {
                string nUrl = tNode.Text;

                List<eNavigRule> tempcns;
                eNavigRule tempcn;


                if (level <= cns.Count)
                {

                  

                    tempcns = new List<eNavigRule>();
                    tempcn = new eNavigRule();
                    tempcn.Url = nUrl;
                    tempcn.Level = 1;
                    tempcn.NaviStartPos = cns[level - 1].NaviStartPos;
                    tempcn.NaviEndPos = cns[level - 1].NaviEndPos;
                    tempcn.NavigRule = cns[level - 1].NavigRule;
                    tempcns.Add(tempcn);

                    List<string> us = AddDemoUrl(nUrl,Cookie , true, tempcns, urlCode, webCode, referUrl, isAUpdateHeader,strHeader, isUrlAutoRedirect);

                    if (us != null)
                    {
                        for (int index = 0; index < us.Count; index++)
                        {
                            //tNode.Nodes.Add(us[index], us[index]);
                            //Application.DoEvents();
                            try
                            {
                                InvokeMethod(this, "AddNode", new object[] { tNode.Nodes, us[index] });
                            }
                            catch { }

                            //先处理导航

                            #region 翻页操作
                            if (cns[level - 1].IsNaviNextPage == true)
                            {
                                int FirstNextIndex = 0;

                                string nextUrl = us[index];
                                string Old_Url = string.Empty;

                                //开始处理翻页
                                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "开始进行导航翻页处理" });


                                do
                                {
                                    Old_Url = nextUrl;

                                    try
                                    {
                                        string cookie = Cookie;
                                        cUrlAnalyze cu = new cUrlAnalyze(Program.getPrjPath());

                                        bool isVisual = false;
                                        if (cns[level - 1].NaviNextPage.Contains("<XPath>"))
                                            isVisual = true;

                                        string hSource = GetHtml(Old_Url, referUrl, tempcn.NaviStartPos, tempcn.NaviEndPos, isVisual);
                                        nextUrl = cu.GetNextUrl(Old_Url, hSource, cns[level - 1].NaviNextPage, m_RegexNextPage,ref FirstNextIndex, level);
                                        cu = null;
                                        referUrl = Old_Url;
                                        //this.txtCookie.Text = cookie;
                                        //SetValue((this.txtCookie), "Text", cookie);

                                        if (nextUrl != "" && Old_Url != nextUrl && nextUrl != "#")
                                        {
                                            //tNode.Nodes.Add(nextUrl, nextUrl);
                                            //Application.DoEvents();
                                            try
                                            {
                                                InvokeMethod(this, "AddNode", new object[] { tNode.Nodes, nextUrl });
                                            }
                                            catch { }
                                        }

                                        //tNode.Text = tNode.Text + " [" + tNode.Nodes.Count + "]";
                                    }
                                    catch (System.Exception ex)
                                    {
                                        throw ex;
                                    }
                                    
                                    if (m_testNaving == false)
                                        break;
                                    if (isAll == false)
                                        break;
                                }
                                while (nextUrl != "" && Old_Url != nextUrl && nextUrl != "#");
                                
                            }
                            #endregion

                            if (m_testNaving == false)
                                break;
                            if (isAll == false)
                                break;
                        }

                        //统一处理导航递归
                        //tNode.Text = tNode.Text + " [" + tNode.Nodes.Count + "]";
                        //Application.DoEvents();

                        try
                        {
                            InvokeMethod(this, "UpdateNodeText", new object[] { tNode, tNode.Text + " [" + tNode.Nodes.Count + "]" });
                        }
                        catch { }

                        InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "导航出了" + tNode.Nodes.Count + "个网址" });

                        if (tNode.Nodes != null && tNode.Nodes.Count > 0)
                            GetNaviUrl(tNode.Nodes,Cookie, cns, urlCode, webCode, headers, level + 1, isAll, nUrl, isAUpdateHeader, strHeader,isUrlAutoRedirect);

                        
                    }

                    #region 入口页面翻页操作
                    if (cns[level - 1].IsNext == true)
                    {
                        int FirstNextIndex = 0;

                        string nextUrl = nUrl;
                        string Old_Url = string.Empty;

                        do
                        {
                            Old_Url = nextUrl;

                            try
                            {
                                string cookie = Cookie;
                                cUrlAnalyze cu = new cUrlAnalyze(Program.getPrjPath());
                                bool isVisual = false;
                                if (cns[level - 1].NextRule.Contains("<XPath>"))
                                    isVisual = true;
                                string hSource = GetHtml(Old_Url, referUrl, tempcn.NaviStartPos, tempcn.NaviEndPos, isVisual);
                                nextUrl = cu.GetNextUrl(Old_Url, hSource, cns[level - 1].NextRule, m_RegexNextPage,
                                ref FirstNextIndex,level);

                                cu = null;

                                referUrl = Old_Url;

                                //this.txtCookie.Text = cookie;
                                //SetValue((this.txtCookie), "Text", cookie);
                                if (nextUrl != "" && Old_Url != nextUrl && nextUrl != "#")
                                {

                                    //tNodeCol.Add(nextUrl, nextUrl);
                                    //Application.DoEvents();

                                    InvokeMethod(this, "AddNode", new object[] { tNodeCol, nextUrl });

                                    //每翻一页导航一次
                                    List<string> us1 = AddDemoUrl(nextUrl, Cookie, true, tempcns, urlCode, webCode, nUrl, isAUpdateHeader, strHeader, isUrlAutoRedirect);
                                    if (us1 != null)
                                    {
                                        for (int index = 0; index < us1.Count; index++)
                                        {
                                            //tNodeCol[nextUrl].Nodes.Add(us1[index], us1[index]);
                                            //Application.DoEvents();
                                            InvokeMethod(this, "AddNode", new object[] { tNodeCol[nextUrl].Nodes, us1[index] });

                                            #region 翻页操作
                                            if (cns[level - 1].IsNaviNextPage == true)
                                            {
                                                FirstNextIndex = 0;

                                                string nextUrl1 = us1[index];
                                                Old_Url = string.Empty;

                                                //开始处理翻页
                                                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "开始进行导航翻页处理" });

                                                do
                                                {
                                                    Old_Url = nextUrl1;

                                                    try
                                                    {
                                                        cookie = Cookie;

                                                        cUrlAnalyze urlAnaly = new cUrlAnalyze(Program.getPrjPath());
                                                        isVisual = false;
                                                        if (cns[level - 1].NaviNextPage.Contains("<XPath>"))
                                                            isVisual = true;
                                                        string hSource1 = GetHtml(Old_Url, referUrl, tempcn.NaviStartPos, tempcn.NaviEndPos, isVisual);
                                                        nextUrl1 = urlAnaly.GetNextUrl(Old_Url, hSource1, cns[level - 1].NaviNextPage, m_RegexNextPage, 
                                                                ref FirstNextIndex,level);

                                                        urlAnaly = null;
                                                        referUrl = Old_Url;
                                                        //this.txtCookie.Text = cookie;
                                                        //SetValue((this.txtCookie), "Text", cookie);
                                                        if (nextUrl1 != "" && Old_Url != nextUrl1 && nextUrl1 != "#")
                                                        {
                                                            try
                                                            {
                                                                InvokeMethod(this, "AddNode", new object[] { tNodeCol[nextUrl].Nodes, nextUrl1 });
                                                            }
                                                            catch { }
                                                        }

                                                        //tNode.Text = tNode.Text + " [" + tNode.Nodes.Count + "]";
                                                    }
                                                    catch (System.Exception ex)
                                                    {
                                                        throw ex;
                                                    }

                                                    if (m_testNaving == false)
                                                        break;
                                                    if (isAll == false)
                                                        break;
                                                }
                                                while (nextUrl1 != "" && Old_Url != nextUrl1 && nextUrl1 != "#");

                                            }
                                            #endregion

                                        }
                                        if (tNodeCol[nextUrl].Nodes != null && tNodeCol[nextUrl].Nodes.Count > 0)
                                            GetNaviUrl(tNodeCol[nextUrl].Nodes,Cookie, cns, urlCode, webCode, headers, level + 1, isAll, nUrl, isAUpdateHeader,strHeader, isUrlAutoRedirect);

                                        //tNodeCol[nextUrl].Text = tNodeCol[nextUrl].Text + " [" + tNodeCol[nextUrl].Nodes.Count + "]";
                                        //Application.DoEvents();
                                        InvokeMethod(this, "UpdateNodeText", new object[] {tNodeCol[nextUrl], tNodeCol[nextUrl].Text + " [" + tNodeCol[nextUrl].Nodes.Count + "]" });

                                        InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "导航出了" + tNode.Nodes.Count + "个网址" });

                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                throw ex;
                            }

                            if (m_testNaving == false)
                                break;

                            if (isAll == false)
                                break;
                        }
                        while (nextUrl != "" && Old_Url != nextUrl && nextUrl != "#");

                    }

                    #endregion
                 
                }
                if (m_testNaving == false)
                    break;
                if (isAll == false)
                    break;

                break;
            }
        }


        //翻译网址内容，将网址中的编码、参数部分进行替换，返回一个真正可用的网址
        private string TransUrl(string DemoUrl)
        {
            //获取网址后，首先进行Url编码和Base64的处理
            if (this.IsUrlEncode.Checked == true)
                DemoUrl = ToolUtil.UrlEncode(DemoUrl,EnumUtil.GetEnumName< cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()));

            //在此处理是否需要进行Base64编码的的问题
            if (Regex.IsMatch(DemoUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(DemoUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //将base64编码部分进行url替换
                DemoUrl=Regex.Replace(DemoUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }


            //需判断当前的网址是否含有参数，如果有参数则需要分解参数
            //当前的做法是是否含有参数都去分解一次，如果没有参数，则返回原址
            cProxyControl PControl = new cProxyControl(Program.getPrjPath (), 0);
            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath ());
            List<string> nUrls = cu.SplitWebUrl(DemoUrl);
            cu = null;

            DemoUrl = nUrls[0].ToString();

            PControl = null;

            return DemoUrl;
        }

        private void ConnectAccess()
        {
            string connectionstring = string.Empty;

            if (this.txtFileName.Text.Trim().EndsWith("accdb", StringComparison.CurrentCultureIgnoreCase))
                connectionstring = "provider=Microsoft.ACE.OLEDB.12.0;data source=";
            else
                connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            connectionstring += this.txtFileName.Text + ";";

            //string connectionstring = "provider=microsoft.jet.oledb.4.0;data source=";
            //connectionstring += this.txtFileName.Text;
          
        }

        private void ConnectSqlServer()
        {
            //string strDataBase = "Server=.;DataBase=Library;Uid=" + this.txtDataUser.Text.Trim() + ";pwd=" + this.txtDataPwd.Text + ";";
            //SqlConnection conn = new SqlConnection(strDataBase);
            //conn.Open();
            //conn.Close();
            //MessageBox.Show("数据库连接成功！", "soukey信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            frmBrowser wftm = new frmBrowser();
            wftm.getFlag = 0;
            wftm.rCookie = new frmBrowser.ReturnCookie(GetCookie);
            wftm.ShowDialog();
            wftm.Dispose();
        }

        /// <summary>
        /// 2013-1-24 重新构建测试，将按照一个真实的任务进行操作
        /// 接管后台线程进行真正的操作。并返回第一条数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            TestTask();
        }

        private void TestTask()
       {
            if (this.IsSave.Text == "true")
            {
                if (this.m_IsRemoteTask == false)
                    SaveLocalTask();
                else if (this.m_IsRemoteTask == true)
                {
                    MessageBox.Show("远程任务无法进行测试，请先下载到本地后，再进行测试操作！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SaveRemoteTask();
                }

                if (this.IsSave.Text == "true")
                {
                    ExportGatherLog(cGlobalParas.LogType.Error,"测试采集任务需保存当前配置信息，系统在自动保存时失败了，请查看是否填写了采集任务名称！");
                    return;
                }
            }

            if (this.listWeblink.Items.Count == 0 && this.listWebGetFlag.Items.Count == 0)
            {
                ExportGatherLog(cGlobalParas.LogType.Warning, "当前采集任务未配置网址及采集数据规则，无法继续测试，请配置相关信息后重新启动测试工作。");
                return;
            }

            if (this.listWeblink.Items.Count > 0 && this.listWebGetFlag.Items.Count == 0)
            {
                ExportGatherLog(cGlobalParas.LogType.Info, "当前采集任务未配置采集数据规则，仅对网址配置规则进行测试。");
                TestUrlsRule();

                return;
            }


            m_IsGathering = true;
            this.button5.Enabled = false;
            this.button2.Enabled = false;
            Application.DoEvents();

            ExportGatherLog(cGlobalParas.LogType.Warning, "注意：采集测试仅根据规则获取到第一个采集页面进行数据采集测试，目的是为了检测规则的正确性，不做完整采集。");

            try
            {
                TestGather();
            }
            catch (System.Exception ex)
            {
                ExportGatherLog(cGlobalParas.LogType.Info, ex.Message);
            }

            m_IsGathering = false;
            this.button5.Enabled = true;
            this.button2.Enabled = true;
            Application.DoEvents();

        }

   

        private void TestGather()
        {
            this.dTestData.DataSource = null;
            this.treeTestUrl.Nodes.Clear();

            ExportGatherLog(cGlobalParas.LogType.Info, "");


            List<eWebLink> wLinks = new List<eWebLink>();
            FillWebLinks(ref wLinks);

            List<eWebpageCutFlag> wCutFlags = new List<eWebpageCutFlag>();
            FillGatherRule(ref wCutFlags);

            //开始获取第一条网址，注意，第一个入口地址有可能是带有参数的，因此，分解后获取第一条网址
            string firstUrl = wLinks[0].Weblink;

            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
            List<string> Urls = cu.SplitWebUrl(firstUrl);

            if (Urls.Count >1)
            {
                firstUrl = Urls[0];
            }

            eWebLink webLink = wLinks[0].DeepClone();
            webLink.Weblink = firstUrl;

            wLinks.Clear();
            wLinks.Add(webLink);

            delegateTestUrl sd = new delegateTestUrl(this.GetDemoUrl);

            sd.Invoke(false, this.txtCookie.Text,
                EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()),
                    EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()), wLinks);

            //开始根据后去的树形结构进行数据采集
            //先进行导航采集
            int naviMaxLeven = wLinks[0].NavigRules.Count;

            //在此判断导航级别与树形结构的级别是否一致，如果不一致，则出错
            int treeDepth = 1;
            GetTreeDepth(this.treeTestUrl.Nodes[0], ref treeDepth);
            if (naviMaxLeven+1!=treeDepth)
            {
                throw new NetMinerException("网址导航出错，采集规则测试已被迫终止，请先确认采集网址配置的正确性！");
            }

            List<eNavigRule> naviRules = wLinks[0].NavigRules;
            
            
            DataSet ds = new DataSet();
            //string Url = this.treeTestUrl.Nodes[0].Text;
            string referUrl = string.Empty;

            DataTable d= GetData(this.treeTestUrl.Nodes[0], referUrl,null ,1, naviRules, wLinks[0].MultiPageRules, wCutFlags);

            #region 在此处理数据加工插件的调用
            ///在此处理数据加工插件的调用
            if (this.IsPluginsDealData.Checked == true)
            {
                ExportGatherLog(cGlobalParas.LogType.Warning, "调用插件进行数据处理！");

                NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                if (this.txtPluginsDeal.Text == "")
                {
                    ExportGatherLog(cGlobalParas.LogType.Warning, "配置了数据加工规则插件的调用，但是没有提供插件文件地址！");
                }
                else
                {
                    d = rPlugin.CallDealData(d, this.txtPluginsDeal.Text);
                }
                rPlugin = null;
            }

            #endregion

            this.dTestData.DataSource = d;

            ExportGatherLog(cGlobalParas.LogType.Warning, "测试完成！");

        }

        /// <summary>
        /// 获取树形结构的深度
        /// </summary>
        /// <returns></returns>
        private void GetTreeDepth(TreeNode tNode,ref int Level)
        {
            if (tNode.Nodes.Count > 0)
            {
                Level++;
                GetTreeDepth(tNode.Nodes[0],ref Level);
            }

        }

        private DataTable GetData(TreeNode tNode,string referUrl, DataTable dRow, 
            int Level,  List<eNavigRule> naviRules, List<eMultiPageRule> multiPageRules, List<eWebpageCutFlag> cutFlag)
        {
            bool isMulti = false;
            string Url = tNode.Text;
            TreeNode mNode = null;

         

            int nLevel = Level;
            if (tNode.Nodes.Count ==0)
            {
                //表示进入了最低级
                nLevel = 0;
            }

            if (Url.Contains(" ["))
            {
                Url = Url.Substring(0, Url.IndexOf(" [") ).Trim ();
            }

            List<eWebpageCutFlag> cFlags = new List<eWebpageCutFlag>();
            //获取此级别的采集规则
            for (int j = 0; j < cutFlag.Count; j++)
            {
                if (cutFlag[j].NavLevel == nLevel)
                    cFlags.Add(cutFlag[j]);
            }

            DataTable d = null;
            DataRow dr1 = null;
            if (dRow != null)
            {
                dr1 = dRow.Rows[0];
            }

            if (cFlags.Count ==0)
            {
                //表示无采集规则，此级别不需要采集
                if (tNode.Nodes.Count > 0)
                {
                    return GetData(tNode.Nodes[0], Url, d, Level + 1, naviRules, multiPageRules, cutFlag);
                }
                else
                    return null;
            }
            else
            {
                #region 获取前后截取标记
                string startPos = string.Empty;
                string endPos = string.Empty;
                if (naviRules.Count ==0 && Level==1)
                {
                    startPos = this.txtStartPos.Text;
                    endPos = this.txtEndPos.Text;
                }
                else if (naviRules.Count ==Level && Level==1)
                {
                    //采集的也是采集页数据
                    startPos = naviRules[0].GatherStartPos;
                    endPos = naviRules[0].GatherEndPos;
                }
                else if (Level < 2 && Level <= naviRules.Count)
                {
                    startPos = naviRules[Level - 1].GatherStartPos;
                    endPos = naviRules[Level - 1].GatherEndPos;
                }
                else
                {
                    startPos = naviRules[Level - 2].GatherStartPos;
                    endPos = naviRules[Level - 2].GatherEndPos;
                }
                #endregion

                string strLoopFlag = string.Empty;

                cGatherWeb gWeb = new cGatherWeb(Program.getPrjPath());

                #region 是否存在自动编号，如果存在，则在此设置

                bool isVisual = false;
                for (int m = 0; m < cFlags.Count; m++)
                {
                    if (cFlags[m].GatherRuleType == cGlobalParas.GatherRuleType.NonePage)
                    {
                        for (int n = 0; n < cFlags[m].ExportRules.Count; n++)
                        {
                            if (cFlags[m].ExportRules[n].FieldRuleType == cGlobalParas.ExportLimit.ExportAutoCode)
                            {
                                try
                                {
                                    gWeb.AutoID = new string[] { cFlags[m].ExportRules[n].FieldRule };
                                }
                                catch (System.Exception)
                                {
                                    gWeb.AutoID = new string[] { "0" };
                                }
                                break;
                            }
                        }
                        break;
                    }
                    else
                        if (!string.IsNullOrEmpty(cFlags[m].XPath) && isVisual == false)
                            isVisual = true;

                }

                #endregion

                gWeb.CutFlag = cFlags;

                string htmlSource = GetHtml(Url, referUrl, startPos, endPos, isVisual);

                gWeb.WebpageSource = htmlSource;

                //判断是否为自我循环
                if (Level>1 &&  naviRules[Level - 2].NavigRule.StartsWith("<MySelf>"))
                {
                    Match strMatchLoop = Regex.Match(naviRules[Level - 2].NavigRule, "(?<=<MySelf>).*?(?=</MySelf>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    strLoopFlag = strMatchLoop.Groups[0].ToString();
                }


                //在此判断是否有多页地址
                if (tNode.NextNode != null)
                {
                    mNode = tNode.NextNode;
                    if (mNode.Text.Contains("[多页]"))
                    {
                        isMulti = true;
                    }
                }


                if (nLevel==0)
                {
                    //最终的采集页了
                    d = gWeb.GetGatherData(Url, startPos, endPos,this.txtSavePath.Text, this.IsExportGUrl.Checked, this.IsExportGDate.Checked,
                        dr1, strLoopFlag, 1, "", cGlobalParas.RejectDeal.None);

                    DataTable d1 = new DataTable();
                    //判断上层是否传入了数据，如果有，则进行合并
                    if (dRow!=null && dRow.Rows.Count > 0)
                    {
                        //表示有数据
                        d1 = MergeDataTable(dRow.Rows[0], d);
                    }
                    else
                        d1 = d;

                    d = d1;
                }
                else
                {
                    //导航页的采集，如果是导航页采集，则仅仅提取第一条数据
                    d = gWeb.GetGatherData(Url, startPos, endPos,this.txtSavePath.Text, this.IsExportGUrl.Checked, this.IsExportGDate.Checked,
                        dr1, strLoopFlag, 1, "", cGlobalParas.RejectDeal.None);

                    if (dRow != null &&  dRow.Rows.Count > 0)
                    {
                        //表示有数据
                        d = MergeDataTable(dRow.Rows[0], d);
                    }

                    DataTable d1 =d.Clone();
                    object dr = d.Rows[0].ItemArray.Clone();

                    //object dRow = tempData.Rows[index].ItemArray.Clone();
                    //DataRow rRow = tData.NewRow();
                    //rRow.ItemArray = ((object[])dRow);
                    //rRow[this.CutFlag[i].Title] = strWrap[n];
                    //tData.Rows.Add(rRow);

                    d.Clear();
                    DataRow dr2 = d.NewRow();
                    dr2.ItemArray = ((object[])dr);
                    d.Rows.Add(dr2);

                    //开始下一个级别的采集测试
                    if (tNode.Nodes.Count > 0)
                    {
                        d= GetData(tNode.Nodes[0], Url, d, Level + 1, naviRules, multiPageRules, cutFlag);
                    }

                    gWeb = null;

                }


                //在此判断是否有多页数据
                if(isMulti)
                {
                    //开始获取多页的数据
                    if (d==null || d.Rows.Count ==0)
                    {
                        ExportGatherLog(cGlobalParas.LogType.Warning, "多页数据获取失败！");
                    }

                    List<string> mName = new List<string>();

                    //判断当前级别是否存在多页规则
                    for (int n=0;n<multiPageRules.Count;n++)
                    {
                        if (multiPageRules[n].mLevel==nLevel)
                        {
                            mName.Add (multiPageRules[n].RuleName);
                        }
                    }

                    DataTable md = null;
                    if (mName!=null)
                    {
                        TreeNode nNode = tNode;

                        //开始循环采集多页数据
                        for(int index=0;index<mName.Count;index++)
                        {
                            List<eMultiPageRule> tmpMRule = new List<eMultiPageRule>();

                            nNode = nNode.NextNode;

                            for (int n = 0; n < multiPageRules.Count; n++)
                            {
                                if (multiPageRules[n].RuleName==mName[index])
                                {
                                    tmpMRule.Add(multiPageRules[n]);
                                }
                            }

                            List<eWebpageCutFlag> tmpFlag = new List<eWebpageCutFlag>();

                            for (int m=0;m<cutFlag.Count;m++)
                            {
                                if (cutFlag[m].MultiPageName == mName[index])
                                    tmpFlag.Add(cutFlag[m]);
                            }


                            cGatherWeb gWeb1 = new cGatherWeb(Program.getPrjPath());

            

                            gWeb1.CutFlag = tmpFlag;

                            string mUrl = nNode.Text;
                            if (mUrl.Contains(" ["))
                            {
                                mUrl = mUrl.Substring(0, mUrl.IndexOf(" [")).Trim();
                            }
                            string htmlSource1 = GetHtml(mUrl, Url, startPos, endPos, isVisual);

                            gWeb1.WebpageSource = htmlSource;
                            md = gWeb1.GetGatherData(Url, "", "", this.txtSavePath.Text, false, false,
                                null, "", 1, "", cGlobalParas.RejectDeal.None);

                            d = MergeDataTable(d.Rows[0], md);

                        }
                    }

                    //DataTable md= getMultiData(mNode, Url, naviRules, multiPageRules, cutFlag);

                   
                }



                return d;
            }

        }

        private DataTable getMultiData(TreeNode tNode,string referUrl, List<eNavigRule> naviRules, List<eMultiPageRule> multiPageRules, List<eWebpageCutFlag> cutFlag)
        {
            DataTable d = null;

            List<eWebpageCutFlag> cFlags = new List<eWebpageCutFlag>();

            

            //获取此级别的采集规则
            for (int j = 0; j < cutFlag.Count; j++)
            {
                //if (cutFlag[j].MultiPageName == multiName)
                //    cFlags.Add(cutFlag[j]);
            }

            d = GetData(tNode, referUrl, null, 0, null, multiPageRules, cutFlag);

            return d;
        }


        private void IsUrlEncode_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsUrlEncode.Checked == true)
            {
                this.comUrlEncode.Enabled = true;
                //this.comUrlEncode.SelectedIndex = 0;
                this.cmdUrlEncoding.Enabled = true;
                this.isTwoUrlCode.Enabled = true;
            }
            else
            {
                this.comUrlEncode.Enabled = false;
                //this.comUrlEncode.SelectedIndex= - 1;
                this.cmdUrlEncoding.Enabled = false;
                this.isTwoUrlCode.Enabled = false;
            }

            this.IsSave.Text = "true";
        }


        private void cmdEditWeblink_Click(object sender, EventArgs e)
        {
            EditUrlRule();
        }

        private void EditUrlRule()
        {
            if (this.listWeblink.SelectedItems.Count == 0)
                return;
            string url = this.listWeblink.SelectedItems[0].Text;
          
                frmAddGUrl f = new frmAddGUrl();
                f.fState = cGlobalParas.FormState.Edit;
                f.Cookie = this.txtCookie.Text;

                if (this.IsUrlEncode.Checked == true)
                {
                    f.IsUrlEncoding = true;
                    f.UrlCode = this.comUrlEncode.SelectedItem.ToString();
                }
                else
                    f.IsUrlEncoding = false;

                f.wCode = this.comWebCode.SelectedItem.ToString();
                f.IsProxy = this.IsProxy.Checked;
                f.IsFirstProxy = this.IsProxyFirst.Checked;
            f.TaskType = cGlobalParas.TaskType.HtmlByUrl;

                if (this.listWeblink.SelectedItems.Count == 0)
                    return;

                cGatherUrlFormConfig uConfig = new cGatherUrlFormConfig();

                uConfig.Url = this.listWeblink.SelectedItems[0].Text;

                if (this.listWeblink.SelectedItems[0].SubItems[1].Text == "N")
                {
                    uConfig.IsNav = false;
                }
                else
                {
                    uConfig.IsNav = true;

                    //this.txtNag.Text = "";

                    //添加导航规则

                    for (int i = 0; i < m_listNaviRules.Count; i++)
                    {
                        if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                        {
                            for (int j = 0; j < m_listNaviRules[i].NavigRule.Count; j++)
                            {
                                eNavigRule cn = new eNavigRule();

                                if (m_listNaviRules[i].NavigRule[j].IsGather == true)
                                    cn.IsGather = true;
                                else
                                    cn.IsGather = false;

                                if (m_listNaviRules[i].NavigRule[j].IsNaviNextPage == true)
                                    cn.IsNaviNextPage = true;
                                else
                                    cn.IsNaviNextPage = false;

                                if (m_listNaviRules[i].NavigRule[j].IsNext == true)
                                    cn.IsNext = true;
                                else
                                    cn.IsNext = false;

                                //if (m_listNaviRules[i].NavigRule[j].IsNextDoPostBack == true)
                                //    cn.IsNextDoPostBack = true;
                                //else
                                //    cn.IsNextDoPostBack = false;

                                //if (m_listNaviRules[i].NavigRule[j].IsNaviNextDoPostBack == true)
                                //    cn.IsNaviNextDoPostBack = true;
                                //else
                                //    cn.IsNaviNextDoPostBack = false;

                                cn.Level = m_listNaviRules[i].NavigRule[j].Level;
                                cn.NextRule = m_listNaviRules[i].NavigRule[j].NextRule;
                                cn.NextMaxPage = m_listNaviRules[i].NavigRule[j].NextMaxPage;
                                cn.NavigRule = m_listNaviRules[i].NavigRule[j].NavigRule;
                                cn.NaviStartPos = m_listNaviRules[i].NavigRule[j].NaviStartPos;
                                cn.NaviEndPos = m_listNaviRules[i].NavigRule[j].NaviEndPos;
                                cn.GatherStartPos = m_listNaviRules[i].NavigRule[j].GatherStartPos;
                                cn.GatherEndPos = m_listNaviRules[i].NavigRule[j].GatherEndPos;
                                cn.NaviNextPage = m_listNaviRules[i].NavigRule[j].NaviNextPage;
                                cn.NaviNextMaxPage = m_listNaviRules[i].NavigRule[j].NaviNextMaxPage;
                                cn.RunRule = m_listNaviRules[i].NavigRule[j].RunRule;
                                cn.OtherNaviRule = m_listNaviRules[i].NavigRule[j].OtherNaviRule;

                                uConfig.nRules.NavigRule.Add(cn);
                            }
                        }
                    }

                }

                //下一页的填充
                if (this.listWeblink.SelectedItems[0].SubItems[3].Text == "")
                {
                    uConfig.IsNext = false;
                    uConfig.NextRule = "";
                    uConfig.MaxPageCount = 0;
                   
                }
                else
                {
                    uConfig.IsNext = true;
                    uConfig.NextRule = this.listWeblink.SelectedItems[0].SubItems[3].Text;
                    
                   

                    uConfig.MaxPageCount = int.Parse(this.listWeblink.SelectedItems[0].SubItems[5].Text);
                }

                //填充多页采集的数据
                if (this.listWeblink.SelectedItems[0].SubItems[7].Text == "Y")
                {
                    uConfig.IsMulti = true;
                    if (this.listWeblink.SelectedItems[0].SubItems[8].Text == "Y")
                        uConfig.isMulti121 = true;
                    else
                        uConfig.isMulti121 = false;

                    for (int i = 0; i < this.m_MultiPageRules.Count; i++)
                    {
                        if (this.listWeblink.SelectedItems[0].Text == m_MultiPageRules[i].Url)
                        {
                            for (int j = 0; j < m_MultiPageRules[i].MultiPageRule.Count; j++)
                            {
                                eMultiPageRule cm = new eMultiPageRule();

                                cm.RuleName = m_MultiPageRules[i].MultiPageRule[j].RuleName;
                                cm.mLevel = m_MultiPageRules[i].MultiPageRule[j].mLevel;
                                cm.Rule = m_MultiPageRules[i].MultiPageRule[j].Rule;
                                uConfig.MRules.MultiPageRule.Add(cm);
                            }
                        }
                    }
                }
                else
                {
                    uConfig.IsMulti = false;
                    uConfig.isMulti121 = false;
                }

                f.IniData(uConfig);


                f.rGUrl = new frmAddGUrl.ReturnGatherUrl(this.SaveGUrl);
                f.ShowDialog();
            //}
        }

        private void cmdAddCutFlag_Click(object sender, EventArgs e)
        {
            frmAddGatherRule f;
            if (this.listWeblink.Items.Count ==0)
                f = new frmAddGatherRule(this.tTask.Text.Trim(), "");
            else
                f = new frmAddGatherRule(this.tTask.Text.Trim (),this.listWeblink.Items[0].Text);

            f.fState = cGlobalParas.FormState.New;
            f.Cookie = this.txtCookie.Text;
            //f.wCode = (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString());

            List<string> NaviLevels=new List<string> ();
            List<string> listMultiPages=new List<string> ();
            List<string> gNames = new List<string>();

            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                gNames.Add(this.listWebGetFlag.Items[i].Text);
            }

            //取最大导航级别的网址
            int maxLevel = 0;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (int.Parse(this.listWeblink.Items[i].SubItems[2].Text) > maxLevel)
                    maxLevel = int.Parse(this.listWeblink.Items[i].SubItems[2].Text);

            }

            //因为导航层级0默认为采集页，所以，必须要从1开始计算
            for (int i =1; i <= maxLevel; i++)
            {
                NaviLevels.Add(i.ToString ());
            }

            //取多页采集的页面
            Hashtable mPageName = new Hashtable();
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (this.listWeblink.Items[i].SubItems[7].Text == "Y")
                {
                    string url = this.listWeblink.Items[i].Text;
                    
                    for (int m = 0; m < this.m_MultiPageRules.Count; m++)
                    {
                        if (url == this.m_MultiPageRules[m].Url)
                        {
                            for (int j = 0; j < this.m_MultiPageRules[m].MultiPageRule.Count; j++)
                            {
                                try
                                {
                                    if (!mPageName.ContainsKey(this.m_MultiPageRules[m].MultiPageRule[j].RuleName))
                                    {
                                        listMultiPages.Add(this.m_MultiPageRules[m].MultiPageRule[j].RuleName);
                                        mPageName.Add(this.m_MultiPageRules[m].MultiPageRule[j].RuleName, this.m_MultiPageRules[m].MultiPageRule[j].RuleName);
                                    }
                                }
                                catch (System.Exception)
                                {
                                    //捕获错误，但不处理
                                }
                            }
                        }
                    }
                }

            }

            mPageName = null;

            f.IniPage(gNames,NaviLevels, listMultiPages);
            f.rGatherRule = new frmAddGatherRule.ReturnGatherRule(AddGatherRule);
            f.ShowDialog();
            f.Dispose();
        }

        private void AddGatherRules(List<cGatherRule> GRules)
        {
            for (int i = 0; i < GRules.Count; i++)
            {
                AddGatherRule(GRules[i]);
            }
        }

        private void AddGatherRule(cGatherRule GRule)
        {

            if (GRule.fState == cGlobalParas.FormState.New)
            {
                #region 增加采集规则
                ListViewItem item = new ListViewItem();
                item.Text = GRule.gName;
                item.SubItems.Add(GRule.gRuleByPage.GetDescription());
                item.SubItems.Add(GRule.gType);
                item.SubItems.Add(GRule.gRuleType.ToString ());
                item.SubItems.Add(GRule.xPath);
                item.SubItems.Add(GRule.gNodePrty);
                item.SubItems.Add(ToolUtil.ClearFlag(GRule.getStart));
                item.SubItems.Add(ToolUtil.ClearFlag(GRule.getEnd));
                item.SubItems.Add(GRule.limitType);
                item.SubItems.Add(GRule.strReg);
                if (GRule.IsMergeData == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");

                //增加导航层级
                if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.GatherPage)
                {
                    item.SubItems.Add("0");
                    item.SubItems.Add("");
                }
                else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.NaviPage)
                {
                    item.SubItems.Add(GRule.NaviLevel);
                    item.SubItems.Add("");
                }
                else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.MultiPage)
                {
                    //是否多页
                    item.SubItems.Add("-1");
                    item.SubItems.Add(GRule.MultiPageName);
                }
                //else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.NonePage)
                //{
                //    item.SubItems.Add("0");
                //    item.SubItems.Add("");
                //}

                item.SubItems.Add(GRule.sPath);
                //item.SubItems.Add(GRule.fileDealType);
          

                if (GRule.IsAutoDownloadFileImage == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");


                if (GRule.IsAutoDownloadOnlyImage == true)
                    item.SubItems.Add("Y");
                else
                    item.SubItems.Add("N");

                //判断是否有数据加工规则
                if (GRule.fieldDealRules != null && GRule.fieldDealRules.FieldRule.Count >0)
                {
                    //展开状态
                    item.ImageIndex = 17;
                    this.listWebGetFlag.Items.Add(item);

                    //开始添加数据加工规则
                    for (int i = 0; i < GRule.fieldDealRules.FieldRule.Count; i++)
                    {
                        ListViewItem dItem = new ListViewItem();
                        dItem.Name = GRule.gName + "!Deal!-" + i.ToString();
                        dItem.Text = "  " + GRule.fieldDealRules.FieldRule[i].FieldRuleType.GetDescription();
                        dItem.SubItems.Add(GRule.fieldDealRules.FieldRule[i].FieldRule);
                        dItem.ForeColor = Color.Gray;
                        this.listWebGetFlag.Items.Add(dItem);
                    }
                }
                else
                {
                    this.listWebGetFlag.Items.Add(item);
                }

                item = null;

                m_gRules.Add(GRule);

                #endregion

            }
            else if (GRule.fState == cGlobalParas.FormState.Edit)
            {
                string oldFileName = "";
                oldFileName = this.listWebGetFlag.SelectedItems[0].Text;

                this.listWebGetFlag.SelectedItems[0].Text = GRule.gName;
                this.listWebGetFlag.SelectedItems[0].SubItems[1].Text =GRule.gRuleByPage.GetDescription();
                this.listWebGetFlag.SelectedItems[0].SubItems[2].Text = GRule.gType;
                this.listWebGetFlag.SelectedItems[0].SubItems[3].Text = GRule.gRuleType.ToString () ;
                this.listWebGetFlag.SelectedItems[0].SubItems[4].Text = GRule.xPath ;
                this.listWebGetFlag.SelectedItems[0].SubItems[5].Text = GRule.gNodePrty;
                this.listWebGetFlag.SelectedItems[0].SubItems[6].Text = ToolUtil.ClearFlag(GRule.getStart);
                this.listWebGetFlag.SelectedItems[0].SubItems[7].Text = ToolUtil.ClearFlag(GRule.getEnd);
                this.listWebGetFlag.SelectedItems[0].SubItems[8].Text = GRule.limitType;


                this.listWebGetFlag.SelectedItems[0].SubItems[9].Text = GRule.strReg;
                if (GRule.IsMergeData == true)
                    this.listWebGetFlag.SelectedItems[0].SubItems[10].Text = "Y";
                else
                    this.listWebGetFlag.SelectedItems[0].SubItems[10].Text = "N";

                if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.GatherPage)
                {
                    this.listWebGetFlag.SelectedItems[0].SubItems[11].Text = "0";
                    this.listWebGetFlag.SelectedItems[0].SubItems[12].Text = "";
                }
                else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.NaviPage)
                {
                    this.listWebGetFlag.SelectedItems[0].SubItems[11].Text = GRule.NaviLevel;
                    this.listWebGetFlag.SelectedItems[0].SubItems[12].Text = "";
                }
                else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.MultiPage)
                {
                    //是否多页
                    this.listWebGetFlag.SelectedItems[0].SubItems[11].Text = "-1";
                    this.listWebGetFlag.SelectedItems[0].SubItems[12].Text = GRule.MultiPageName;
                }
                //else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.NonePage)
                //{
                //    //是否多页
                //    this.listWebGetFlag.SelectedItems[0].SubItems[11].Text = "0";
                //    this.listWebGetFlag.SelectedItems[0].SubItems[12].Text = ""; ;
                //}

                this.listWebGetFlag.SelectedItems[0].SubItems[13].Text = GRule.sPath;

                //this.listWebGetFlag.SelectedItems[0].SubItems[14].Text = GRule.fileDealType;

                if (GRule.IsAutoDownloadFileImage ==true )
                    this.listWebGetFlag.SelectedItems[0].SubItems[14].Text = "Y";
                else
                    this.listWebGetFlag.SelectedItems[0].SubItems[14].Text = "N";

                if (GRule.IsAutoDownloadOnlyImage == true)
                    this.listWebGetFlag.SelectedItems[0].SubItems[15].Text = "Y";
                else
                    this.listWebGetFlag.SelectedItems[0].SubItems[15].Text = "N";

                //先删除原有的数据加工规则显示
                int index = this.listWebGetFlag.SelectedItems[0].Index;

                DelDealRule(oldFileName, index);

                string gName = GRule.gName;

                AddDealRule(gName, index, GRule);

                //修改此字段的输出规则，如果名称发生了变化，则需要进行修改名称
                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (oldFileName == m_gRules[i].gName)
                    {
                        m_gRules[i] = GRule;
                        break;
                    }
                }

                oldFileName = gName;

                this.listWebGetFlag.SelectedItems[0].ImageIndex = 17;
            }

            this.IsSave.Text = "true";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            EditGatherRule();
        }

        private void EditGatherRule()
        {
          
            if (this.listWebGetFlag.SelectedItems.Count ==0)
            {
                MessageBox.Show ("请选择需要修改的采集规则！","网络矿工 信息",MessageBoxButtons.OK ,MessageBoxIcon.Information );
                return ;
            }

            if (this.listWebGetFlag.SelectedItems[0].Name != "")
                return;

            frmAddGatherRule f = new frmAddGatherRule(this.tTask.Text .Trim (), this.listWeblink.Items[0].Text);
            f.Cookie = this.txtCookie.Text;
            f.fState = cGlobalParas.FormState.Edit;
            List<string> NaviLevels = new List<string>();
            List<string> listMultiPages = new List<string>();
            List<string> gNames = new List<string>();

            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                gNames.Add(this.listWebGetFlag.Items[i].Text);
            }

            //取最大导航级别的网址
            int maxLevel = 0;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (int.Parse(this.listWeblink.Items[i].SubItems[2].Text) > maxLevel)
                    maxLevel = int.Parse(this.listWeblink.Items[i].SubItems[2].Text);

            }

            //因为导航层级0默认为采集页，所以，必须要从1开始计算
            for (int i = 1; i <= maxLevel; i++)
            {
                NaviLevels.Add(i.ToString());
            }

            //取多页采集的页面
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (this.listWeblink.Items[i].SubItems[7].Text == "Y")
                {
                    string url = this.listWeblink.Items[i].Text;
                    for (int m = 0; m < this.m_MultiPageRules.Count; m++)
                    {
                        if (url == this.m_MultiPageRules[m].Url)
                        {
                            for (int j = 0; j < this.m_MultiPageRules[m].MultiPageRule.Count; j++)
                            {
                                try
                                {
                                    listMultiPages.Add(this.m_MultiPageRules[m].MultiPageRule[j].RuleName);
                                }
                                catch (System.Exception)
                                {
                                    //捕获错误，但不处理
                                }
                            }
                        }
                    }
                }

            }

            cGlobalParas.GatherRuleByPage gRuleByPage = cGlobalParas.GatherRuleByPage.GatherPage;

            switch (EnumUtil.GetEnumName<cGlobalParas.GatherRuleByPage> (this.listWebGetFlag.SelectedItems[0].SubItems[1].Text))
            {
                //case (int)cGlobalParas.GatherRuleByPage.NonePage :
                //    gRuleByPage = cGlobalParas.GatherRuleByPage.NonePage;
                //    break ;
                case cGlobalParas.GatherRuleByPage.GatherPage :
                    gRuleByPage = cGlobalParas.GatherRuleByPage.GatherPage;
                    break ;
                case cGlobalParas.GatherRuleByPage.NaviPage :
                    gRuleByPage = cGlobalParas.GatherRuleByPage.NaviPage;
                    break ;
                case cGlobalParas.GatherRuleByPage.MultiPage :
                    gRuleByPage = cGlobalParas.GatherRuleByPage.MultiPage;
                    break ;
            }

            cGatherRule gRule=new cGatherRule ();

            gRule.gName=this.listWebGetFlag.SelectedItems[0].Text;
            gRule.gRuleByPage=gRuleByPage;
            gRule.gType=this.listWebGetFlag.SelectedItems[0].SubItems[2].Text;

            if (this.listWebGetFlag.SelectedItems[0].SubItems[3].Text == "Normal")
                gRule.gRuleType = cGlobalParas.GatherRuleType.Normal;
            else if (this.listWebGetFlag.SelectedItems[0].SubItems[3].Text == "XPath")
                gRule.gRuleType = cGlobalParas.GatherRuleType.XPath;
            else if (this.listWebGetFlag.SelectedItems[0].SubItems[3].Text == "Smart")
                gRule.gRuleType = cGlobalParas.GatherRuleType.Smart;
            else if (this.listWebGetFlag.SelectedItems[0].SubItems[3].Text == "NonePage")
                gRule.gRuleType = cGlobalParas.GatherRuleType.NonePage;

            gRule.xPath = this.listWebGetFlag.SelectedItems[0].SubItems[4].Text;
            gRule.gNodePrty = this.listWebGetFlag.SelectedItems[0].SubItems[5].Text;
            gRule.getStart=this.listWebGetFlag.SelectedItems[0].SubItems[6].Text;
            gRule.getEnd=this.listWebGetFlag.SelectedItems[0].SubItems[7].Text;
            gRule.limitType=this.listWebGetFlag.SelectedItems[0].SubItems[8].Text;
            gRule.strReg=this.listWebGetFlag.SelectedItems[0].SubItems[9].Text;
            if (this.listWebGetFlag.SelectedItems[0].SubItems[10].Text == "Y")
                gRule.IsMergeData = true;
            else
                gRule.IsMergeData = false;

            gRule.NaviLevel =this.listWebGetFlag.SelectedItems[0].SubItems[11].Text;
            gRule.MultiPageName=this.listWebGetFlag.SelectedItems[0].SubItems[12].Text;
            gRule.sPath=this.listWebGetFlag.SelectedItems[0].SubItems[13].Text;
            //gRule.fileDealType=this.listWebGetFlag.SelectedItems[0].SubItems[14].Text;

            if (this.listWebGetFlag.SelectedItems[0].SubItems[14].Text == "Y")
                gRule.IsAutoDownloadFileImage = true;
            else
                gRule.IsAutoDownloadFileImage = false;


            if (this.listWebGetFlag.SelectedItems[0].SubItems[15].Text == "Y")
                gRule.IsAutoDownloadOnlyImage = true;
            else
                gRule.IsAutoDownloadOnlyImage = false;

            for (int n = 0; n < m_gRules.Count; n++)
            {
                if (this.listWebGetFlag.SelectedItems[0].Text == m_gRules[n].gName)
                {
                    gRule.fieldDealRules = m_gRules[n].fieldDealRules;
                    break;
                }
            }

            f.IniPage(gNames, NaviLevels, listMultiPages, gRule);
            
            f.rGatherRule = new frmAddGatherRule.ReturnGatherRule(AddGatherRule);
            f.ShowDialog();
            f.Dispose();
         
        }

        private void cmdDelCutFlag_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count != 0)
            {

                //删除此采集规则下的输出规则
                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (this.listWebGetFlag.SelectedItems[0].Text == m_gRules[i].gName)
                    {
                        m_gRules.Remove(m_gRules[i]);
                        break;
                    }
                }

                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

               this.IsSave.Text = "true";
            }
        }

        

        private void cmdOpenFolder_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.Description = rm.GetString ("Info7");
            this.folderBrowserDialog1.SelectedPath = Program.getPrjPath();
            if (this.folderBrowserDialog1.ShowDialog()==DialogResult.OK)
            {
                this.txtSavePath.Text = this.folderBrowserDialog1.SelectedPath;
            }

        }

        private void frmTask_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (m_FormState == cGlobalParas.FormState.Browser)
                return;

            //退出前需要检查是否正在测试网址，是否正在测试采集数据
            if (m_testNaving == true)
            {
                MessageBox.Show("请先停止测试网址操作，再退出采集任务编辑窗口！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (m_IsGathering ==true)
            {
                MessageBox.Show("请先停止采集测试操作，再退出采集任务编辑窗口！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (this.IsSave.Text == "true")
            {
                if (MessageBox.Show(rm.GetString ("Quaere2"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    e.Cancel = true;
                    return;
            }
            
        }

        #region 设置修改保存标记
        private void tTask_TextChanged(object sender, EventArgs e)
        {
            ModifyTitle("新建采集任务：" + this.tTask.Text);
            this.IsSave.Text = "true";
        }

        private void txtTaskDemo_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comTaskClass_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void TaskType_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comRunType_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udThread_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtSavePath_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comWebCode_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtCookie_TextChanged(object sender, EventArgs e)
        {
            
            this.IsSave.Text = "true";
        }

        private void txtLoginUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void DataSource_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtDataPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtTableName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comUrlEncode_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtStartPos_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEndPos_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        #endregion

        private void raExportTxt_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportTxt.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "" && this.txtFileName.Text.Length > 5 && this.txtFileName.Text.LastIndexOf(".")>0)
                {
                    this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.LastIndexOf(".")) + ".txt";
                }

                this.IsSave.Text = "true";
            }
            
        }

        private void raExportExcel_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportExcel.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "" && this.txtFileName.Text.Length > 6 && this.txtFileName.Text.LastIndexOf(".")>0)
                {
                    this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.LastIndexOf(".")) + ".xlsx";
                }

                this.IsSave.Text = "true";
            }
        }

        private void raExportAccess_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportAccess.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportMSSQL_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportMSSQL.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportMySql_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportMySql.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void raExportOracle_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportOracle.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportWord.Checked = false;

                SetExportDB();

                this.txtDataSource.Text = "";
                this.comTableName.Items.Clear();

                this.IsSave.Text = "true";
            }
        }

        private void SetExportFile()
        {
            this.txtFileName.Enabled = true;
            this.cmdBrowser.Enabled = true;
            this.IsIncludeHeader.Enabled = true;
            this.IsRowFile.Enabled = true;

            this.txtDataSource.Enabled = false;
            this.comTableName.Enabled = false;
            this.button12.Enabled = false;
            this.txtInsertSql.Enabled = false;

            this.button13.Enabled = false ;
        }

        private void SetExportDB()
        {
            this.txtFileName.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.IsIncludeHeader.Enabled = false;
            this.IsRowFile.Enabled = false ;

            this.txtDataSource.Enabled = true;
            this.comTableName.Enabled = true;
            this.button12.Enabled = true;
            this.txtInsertSql.Enabled = true;

            this.button13.Enabled = true;
        }

        private void SetExportWeb()
        {
            this.txtFileName.Enabled = false;
            this.cmdBrowser.Enabled = false;
            this.IsIncludeHeader.Enabled = false;
            this.IsRowFile.Enabled = false ;

            this.txtDataSource.Enabled = false;
            this.comTableName.Enabled = false;
            this.button12.Enabled = false;
            this.txtInsertSql.Enabled = false;

            this.button13.Enabled = false ;
        }

        private void cmdBrowser_Click(object sender, EventArgs e)
        {
            if (this.raExportTxt.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString ("Info12");

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "Text Files(*.txt)|*.txt|All Files(*.*)|*.*";
            }
            else if (this.raExportCSV.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString("Info234");

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "CSV Files(*.csv)|*.csv|All Files(*.*)|*.*";
            }
            else if (this.raExportExcel.Checked == true)
            {
                this.saveFileDialog1.Title = rm.GetString("Info13");

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "Excel Files(*.xlsx)|*.xlsx|All Files(*.*)|*.*";
            }
            else if (this.raExportWord.Checked == true)
            {
                this.saveFileDialog1.Title = "请输入导出Word的文件名称";

                saveFileDialog1.InitialDirectory = Program.getPrjPath();
                saveFileDialog1.Filter = "Word Files(*.docx)|*.docx|All Files(*.*)|*.*";
            }
            

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtFileName.Text = this.saveFileDialog1.FileName;
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.raExportAccess.Checked == true)
                fSD.FormState = 0;
            else if (this.raExportMSSQL .Checked ==true )
                fSD.FormState=1;
            else if (this.raExportMySql.Checked ==true )
                fSD.FormState =2;
            else if (this.raExportOracle.Checked==true )
                fSD.FormState = 3;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetDataSource);
            fSD.ShowDialog();
            fSD.Dispose();
           
        }

        private void comTableName_DropDown(object sender, EventArgs e)
        {
            if (this.comTableName.Items.Count == 0)
            {
                if (this.raExportAccess.Checked == true)
                {
                    FillAccessTable();
                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    FillMSSqlTable();
                }
                else if (this.raExportMySql.Checked == true)
                {
                    FillMySql();
                }
                else if (this.raExportOracle.Checked == true)
                {
                    FillOracle();
                }

            }
        }

        private void FillAccessTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString =ToolUtil.DecodingDBCon ( this.txtDataSource.Text); 

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString ("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
                if (r[3].ToString() == "TABLE")
                {
                    this.comTableName.Items.Add(r[2].ToString());
                }
                
            }
           
        }

        private void FillMSSqlTable()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            SqlConnection conn = new SqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {
               
                this.comTableName.Items.Add(r[2].ToString());
                
            }
        }

        private void FillMySql()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            MySqlConnection conn = new MySqlConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[2].ToString());

            }
        }

        private void FillOracle()
        {
            if (this.comTableName.Items.Count != 0)
                return;

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = ToolUtil.DecodingDBCon(this.txtDataSource.Text);

            try
            {
                conn.Open();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error12") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataTable tb = conn.GetSchema("Tables");

            foreach (DataRow r in tb.Rows)
            {

                this.comTableName.Items.Add(r[1].ToString());

            }
        }

        private void comTableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillInsertSql(this.comTableName.SelectedItem.ToString (),this.raExportOracle.Checked);

            this.IsSave.Text = "true";
        }

        private void txtDataSource_TextChanged(object sender, EventArgs e)
        {
            if (this.comTableName.Items.Count != 0)
                this.comTableName.Items.Clear();

            this.IsSave.Text = "true";
        }

        private DataTable GetTableColumns(string tName)
        {
            DataTable tc=new DataTable ();

            try
            {

                if (this.raExportAccess.Checked == true)
                {
                    OleDbConnection conn = new OleDbConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns",Restrictions);
                    conn.Close();

                    return tc;

                }
                else if (this.raExportMSSQL.Checked == true)
                {
                    SqlConnection conn = new SqlConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);
                    conn.Close();
                    return tc;
                }
                else if (this.raExportMySql.Checked == true)
                {
                    MySqlConnection conn = new MySqlConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon (this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[4];
                    Restrictions[2] = tName;
                    tc = conn.GetSchema("Columns", Restrictions);
                    conn.Close();
                    return tc;
                }
                else if (this.raExportOracle.Checked == true)
                {
                    OracleConnection conn = new OracleConnection();
                    conn.ConnectionString = ToolUtil.DecodingDBCon(this.txtDataSource.Text);

                    conn.Open();

                    string[] Restrictions = new string[3];
                    Restrictions[1] = tName;

                    tc = conn.GetSchema("Columns", Restrictions);
                    conn.Close();

                    return tc;
                }

                return tc;

            }
            catch (System.Exception )
            {
                return null;
            }


        }

        private void FillInsertSql(string TableName,bool isOracle)
        {
            string iSql = "";
            string strColumns = "";

            if (this.raExportAccess.Checked == true)
            {
                iSql = "insert into " + TableName + " (";
            }
            else if (this.raExportMSSQL.Checked == true)
            {
                iSql = "insert into [" + TableName + "] (";
            }
            else if (this.raExportMySql.Checked == true)
            {
                iSql = "insert into `" + TableName + "` (";
            }
            else if (this.raExportOracle.Checked == true)
            {
                iSql = "insert into " + TableName + " (";
            }
            

            DataTable tc = null;
            try
            {
                tc = GetTableColumns(TableName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("自动生成Sql语句出错，请手工输入Sql语句！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (tc == null)
            {
                MessageBox.Show("自动生成Sql语句出错，请手工输入Sql语句！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < tc.Rows.Count; i++)
            {
                if (isOracle==true )
                    strColumns += tc.Rows[i][2].ToString() + ",";
                else
                    strColumns += tc.Rows[i][3].ToString() + ",";
            }

            strColumns = strColumns.Substring(0, strColumns.Length - 1);

            iSql = iSql + strColumns + ") values ( ";

            string strColumnsValue = "";

            for (int j = 0; j < this.listWebGetFlag.Items.Count; j++)
            {
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "文本" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumnsValue += "'{" + this.listWebGetFlag.Items[j].Text + "}',";

            }

            //判断是否输出采集的Url 和 时间
            if (this.IsExportGUrl.Checked == true)
            {
                strColumnsValue += "'{CollectionUrl}',";
            }

            if (this.IsExportGDate.Checked == true)
            {
                strColumnsValue += "'{CollectionDateTime}',";
            }


            if (strColumnsValue!="")
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);

            iSql = iSql + strColumnsValue + ")";

            this.txtInsertSql .Text = iSql;

        }

        private void comTableName_TextChanged(object sender, EventArgs e)
        {
            string iSql = string.Empty;

            if (this.raExportAccess.Checked == true)
            {
                iSql = "insert into " + this.comTableName.Text + " (";
            }
            else if (this.raExportMSSQL.Checked == true)
            {
                iSql = "insert into [" + this.comTableName.Text + "] (";
            }
            else if (this.raExportMySql.Checked == true)
            {
                iSql = "insert into `" + this.comTableName.Text + "` (";
            }
            else if (this.raExportOracle.Checked == true)
            {
                iSql = "insert into " + this.comTableName.Text + " (";
            }
                
                //"insert into " + this.comTableName.Text + "(";
            string strColumns = "";
            string strColumnsValue = "";

            for (int j = 0; j < this.listWebGetFlag.Items.Count; j++)
            {
                if (this.listWebGetFlag.Items[j].Name == "")
                {
                    if (this.listWebGetFlag.Items[j].SubItems[2].Text == "文本" || this.listWebGetFlag.Items[j].SubItems[2].Text == "Text")
                    {
                        strColumns += this.listWebGetFlag.Items[j].Text + ",";
                        strColumnsValue += "'{" + this.listWebGetFlag.Items[j].Text + "}',";
                    }
                }
            }

            //判断是否输出采集的Url 和 时间
            if (this.IsExportGUrl.Checked == true)
            {
                strColumns += "CollectionUrl" + ",";
                strColumnsValue += "'{CollectionUrl}',";
            }

            if (this.IsExportGDate.Checked == true)
            {
                strColumns += "CollectionDateTime" + ",";
                strColumnsValue += "'{CollectionDateTime}',";
            }

            if (strColumns != "")
            {
                strColumns = strColumns.Substring(0, strColumns.Length - 1);
                strColumnsValue = strColumnsValue.Substring(0, strColumnsValue.Length - 1);
            }

            
            iSql = iSql + strColumns + ") values (" + strColumnsValue + ")";
            this.txtInsertSql.Text = iSql;

        }

        private void IsSave_TextChanged(object sender, EventArgs e)
        {
            if (this.IsSave.Text == "true" && this.FormState !=cGlobalParas.FormState .Browser )
            {
                this.cmdApply.Enabled = true;
            }
            else if (this.IsSave.Text == "false")
            {
                this.cmdApply.Enabled = false;
            }
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            if (this.m_IsRemoteTask == false)
                SaveLocalTask();
            else if (this.m_IsRemoteTask == true)
            {
                SaveRemoteTask();
            }
        }

        /// <summary>
        /// 保存远程的采集任务信息
        /// </summary>
        private void SaveRemoteTask()
        {
             if (!CheckInputvalidity())
            {
                return;
            }

            //localhost.NetMinerWebService sWeb = new localhost.NetMinerWebService();
            //sWeb.Url = Program.ConnectServer + "/NetMiner.WebService.asmx";
            //if (Program.g_IsAuthen == true)
            //    sWeb.Credentials = new System.Net.NetworkCredential(Program.g_WindowsUser, Program.g_WindowsPwd);

            oTask t = new oTask(Program.getPrjPath());
            t.New();

            eTask et = new eTask();
            t.TaskEntity = FillTask(et);

            string strXML = t.GetTaskXML();

            MemoryStream mStream=new MemoryStream ();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(mStream, strXML);

            //sWeb.SaveTask(t.TaskEntity.TaskName + ".smt", mStream.ToArray ());

            //sWeb = null;

            mStream.Close();

            this.IsSave.Text = "false";

            IsSaveTask = true;

        }

        /// <summary>
        /// 保存本地的采集任务信息
        /// </summary>
        private void SaveLocalTask()
        {
            if (!CheckInputvalidity())
            {
                return;
            }

            string tClassPath = string.Empty;

            oTask t = new oTask(Program.getPrjPath());
            
            try
            {
                //新建一个任务
                t.New();

                eTask et = new eTask();
                t.TaskEntity = FillTask(et);

                if (this.comTaskClass.Tag == null)
                    tClassPath = NetMiner.Constant.g_TaskPath;
                else
                    tClassPath = this.comTaskClass.Tag.ToString();

                //检测分类是否发生了变化
                if (this.FormState == cGlobalParas.FormState.New)
                {
                    t.Save(tClassPath, cGlobalParas.opType.Add, true);
                    this.FormState = cGlobalParas.FormState.Edit;
                    this.tTask.Enabled = false;
                }
                else if (this.FormState == cGlobalParas.FormState.Edit)
                {
                    if (this.m_iniTaskPath != this.comTaskClass.Tag.ToString())
                    {
                        //删除原有的index文件信息
                        oTaskIndex ti = new oTaskIndex(Program.getPrjPath(), Program.getPrjPath() + m_iniTaskPath + "\\index.xml");
                        try
                        {
                            ti.DeleTaskIndex(t.TaskEntity.TaskName);
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
                        t.Save(tClassPath, cGlobalParas.opType.Add, true);
                    }
                    else
                    {
                        t.Save(tClassPath, cGlobalParas.opType.Edit, false);
                    }
                }

                

                //else
                //{
                //    //仅保存文件即可
                //    t.SaveTask(Program.getPrjPath() + tClassPath + "\\" + t.TaskEntity.TaskName + ".smt");
                //}

                this.IsSave.Text = "false";

                IsSaveTask = true;
            }

            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error13") + ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
              
                t.Dispose();
                t = null;
            }
        }

        private void txtWeblinkDemo_TextChanged(object sender, EventArgs e)
        {
            //this.IsSave.Text = "true";
        }

        private void txtFileName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtInsertSql_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtExportUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtExportCookie_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comExportUrlCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
        private void txtNag_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        //private void cmdAddNRule_Click(object sender, EventArgs e)
        //{
        //    frmAddNavPageRule f = new frmAddNavPageRule();
        //    f.fState = cGlobalParas.FormState.New;
        //    f.TestUrl = this.txtWebLink.Text;
        //    f.rNavRule = new frmAddNavPageRule.ReturnNavRule(GetNavRule);
        //    f.ShowDialog();
        //    f.Dispose();
        //}

        //private void GetNavRule(cGlobalParas.FormState fState,cNaviRuleFormConfig fNaviRule)
        //{
            
        //    for (int i = 0; i<this.dataNRule.Rows.Count; i++)
        //    {
        //        this.dataNRule.Rows[i].Cells[0].Value = i + 1;
        //    }

        //    int MaxLevel = 0;
        //    if (this.dataNRule.Rows.Count == 0)
        //        MaxLevel = 1;
        //    else
        //        MaxLevel = this.dataNRule.Rows.Count + 1;

        //    string strIsNaviNext = "";
        //    string strIsGather = "";
            
        //    //导航翻页参数
        //    string strIsNext = "";

        //    string strIsNextDoPostBack = "";
        //    string strIsNaviNextDoPostBack = "";

        //    if (fNaviRule.IsGather == true)
        //        strIsGather = "Y";
        //    else
        //        strIsGather = "N";

        //    if (fNaviRule.IsNaviNext == true)
        //        strIsNaviNext = "Y";
        //    else
        //        strIsNaviNext = "N";

        //    if (fNaviRule.IsNext == true)
        //        strIsNext = "Y";
        //    else
        //        strIsNext = "N";

        //    if (fNaviRule.IsNextDoPostBack == true)
        //        strIsNextDoPostBack = "Y";
        //    else
        //        strIsNextDoPostBack = "N";

        //    if (fNaviRule.IsNaviNextDoPostBack == true)
        //        strIsNaviNextDoPostBack = "Y";
        //    else
        //        strIsNaviNextDoPostBack = "N";

        //    if (fState == cGlobalParas.FormState.New)
        //    {
        //        this.dataNRule.Rows.Add(MaxLevel, strIsNext, fNaviRule.NextRule,strIsNextDoPostBack, fNaviRule.NextMaxPage,
        //            fNaviRule.NavRule, fNaviRule.SPos, fNaviRule.EPos, strIsGather, fNaviRule.GSPos, fNaviRule.GEPos,
        //            strIsNaviNext, fNaviRule.NaviNextRule, strIsNaviNextDoPostBack,fNaviRule.NaviNextMaxPage);
        //    }
        //    else if (fState == cGlobalParas.FormState.Edit)
        //    {
        //        int curr = this.dataNRule.CurrentCell.RowIndex;
        //        this.dataNRule.Rows[curr].Cells[1].Value = strIsNext;
        //        this.dataNRule.Rows[curr].Cells[2].Value = fNaviRule.NextRule;
        //        this.dataNRule.Rows[curr].Cells[3].Value = strIsNextDoPostBack;
        //        this.dataNRule.Rows[curr].Cells[4].Value = fNaviRule.NextMaxPage;
        //        this.dataNRule.Rows[curr].Cells[5].Value = fNaviRule.NavRule;
        //        this.dataNRule.Rows[curr].Cells[6].Value = fNaviRule.SPos;
        //        this.dataNRule.Rows[curr].Cells[7].Value = fNaviRule.EPos;
        //        this.dataNRule.Rows[curr].Cells[8].Value = strIsGather;
        //        this.dataNRule.Rows[curr].Cells[9].Value = fNaviRule.GSPos;
        //        this.dataNRule.Rows[curr].Cells[10].Value = fNaviRule.GEPos;
        //        this.dataNRule.Rows[curr].Cells[11].Value = strIsNaviNext;
        //        this.dataNRule.Rows[curr].Cells[12].Value = fNaviRule.NaviNextRule;
        //        this.dataNRule.Rows[curr].Cells[13].Value = strIsNaviNextDoPostBack;
        //        this.dataNRule.Rows[curr].Cells[14].Value = fNaviRule.NaviNextMaxPage;
        //    }

        //    this.IsSave.Text = "true";
        //}

        //private void cmdDelNRule_Click(object sender, EventArgs e)
        //{
        //    this.dataNRule.Focus();
        //    SendKeys.Send("{Del}");
        //    this.IsSave.Text = "true";
        //}

        private void IsStartTrigger_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        //private void cmdAddTask_Click(object sender, EventArgs e)
        //{
        //    frmAddPlanTask f = new frmAddPlanTask();
        //    f.RTask = GetTaskInfo;
        //    f.ShowDialog();
        //    f.Dispose();

        //    this.IsSave.Text = "true";
        //}

        //private void GetTaskInfo(cGlobalParas.RunTaskType rType, string TaskName, string TaskPara)
        //{
        //    ListViewItem Litem = new ListViewItem();

        //    Litem.Text = rType.GetDescription();
        //    Litem.SubItems.Add(TaskName);
        //    Litem.SubItems.Add(TaskPara);

        //    this.listTask.Items.Add(Litem);

        //}

        //private void cmdDelTask_Click(object sender, EventArgs e)
        //{
        //    this.listTask.Focus();
        //    SendKeys.Send("{Del}");

        //    this.IsSave.Text = "true";
        //}

        //private void listTask_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Delete)
        //    {
        //        while (this.listTask.SelectedItems.Count > 0)
        //        {
        //            this.listTask.Items.Remove(this.listTask.SelectedItems[0]);
        //        }

        //        this.IsSave.Text = "true";
        //    }
        //}

        private void txtWebLink_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udAgainNumber_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsIgnore404_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsIncludeHeader_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsDelRepRow_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSaveErrorLog_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdWizard_Click(object sender, EventArgs e)
        {
            frmTaskGuide f = new frmTaskGuide();
            f.ShowDialog();
            f.Dispose();
        }

        private void listWebGetFlag_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            SetDataState();
        }

        /// <summary>
        /// 根据选择的数据规则，设置按钮显示
        /// </summary>
        private void SetDataState()
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
                return;

            if (this.listWebGetFlag.SelectedItems[0].Name != "")
            {
                this.cmdUp.Enabled = false;
                this.cmdDown.Enabled = false;

                this.cmdEditCutFlag.Enabled = false ;
                this.cmdDelCutFlag.Enabled = false ;
                return;
            }

            if (this.listWebGetFlag.SelectedItems.Count == 1 && this.listWebGetFlag.Items.Count == 1)
            {
                 this.cmdUp.Enabled = false;
                this.cmdDown.Enabled = false;

                this.cmdEditCutFlag.Enabled = true ;
                this.cmdDelCutFlag.Enabled = true ;
            }
            else if (this.listWebGetFlag.SelectedItems.Count == 0 || this.listWebGetFlag.Items.Count == 1)
            {
                this.cmdUp.Enabled = false;
                this.cmdDown.Enabled = false;

                this.cmdEditCutFlag.Enabled = false;
                this.cmdDelCutFlag.Enabled = false;
            }
            else
            {
                this.cmdEditCutFlag.Enabled = true;
                this.cmdDelCutFlag.Enabled = true;

                if (this.listWebGetFlag.SelectedItems[0].Index == 0)
                {
                    this.cmdUp.Enabled = false;
                    this.cmdDown.Enabled = true;
                }
                else if (this.listWebGetFlag.SelectedItems[0].Index == this.listWebGetFlag.Items.Count - 1)
                {
                    this.cmdUp.Enabled = true;
                    this.cmdDown.Enabled = false;
                }
                else
                {
                    if (isLastItem(this.listWebGetFlag.SelectedItems[0].Index))
                    {
                        this.cmdUp.Enabled = true  ;
                        this.cmdDown.Enabled = false ;
                    }
                    else
                    {
                        this.cmdUp.Enabled = true;
                        this.cmdDown.Enabled = true;
                    }
                }
            }
        }

        private bool isLastItem(int index)
        {
            for (int i = index+1; i < this.listWebGetFlag.Items.Count; i++)
            {
                if (this.listWebGetFlag.Items[i].Name=="")
                    return false;
            
            }
            return true;
        }

        private void cmdUp_Click(object sender, EventArgs e)
        {
            ListViewItem Litem = this.listWebGetFlag.SelectedItems[0];

            int index = this.listWebGetFlag.SelectedItems[0].Index;
            List<int> rangeIndex = new List<int>();
            bool isShowDeal = false;

            //开始判断是否包含了加工规则，如果不包含或者是不显示，都不做处理
            string delName = this.listWebGetFlag.SelectedItems[0].Text;
            delName += "!Deal!-";
            List<string> delKey = new List<string>();
            for (int i = index; i < this.listWebGetFlag.Items.Count; i++)
            {
                string dName = this.listWebGetFlag.Items[i].Name;
                if (dName.StartsWith(delName))
                {
                    isShowDeal = true;
                    break;
                }
            }

            int pIndex = 0;
            //开始判断向上移的位置
            for (int i = index-1; i >= 0; i--)
            {
                if (this.listWebGetFlag.Items[i].Name =="")
                {
                    pIndex = this.listWebGetFlag.Items[i].Index;
                    break;
                }
            }

            //开始删除显示
            if (isShowDeal == true)
            {
                //删除加工规则
                DelDealRule(this.listWebGetFlag.SelectedItems[0].Text, index);
            }
            this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

            int insertIndex = 0;

            insertIndex = pIndex;

            insertIndex= this.listWebGetFlag.Items.Insert(insertIndex, Litem).Index;

            if (isShowDeal == true)
            {
                cGatherRule gRule=new cGatherRule ();

                for (int i=0;i<m_gRules.Count ;i++)
                {
                    if (Litem.Text ==m_gRules[i].gName )
                    {
                        gRule=m_gRules[i];
                        break ;
                    }
                }
                AddDealRule(Litem.Text, insertIndex, gRule);
            }

            this.listWebGetFlag.SelectedItems[0].EnsureVisible();
            this.IsSave.Text = "true";
        }

        private void cmdDown_Click(object sender, EventArgs e)
        {
            int index = this.listWebGetFlag.SelectedItems[0].Index;
            ListViewItem Litem = this.listWebGetFlag.SelectedItems[0];

            List<int> rangeIndex = new List<int>();
            bool isShowDeal = false;
            int dealCount = 0;

            //开始判断是否包含了加工规则，如果不包含或者是不显示，都不做处理
            string delName = this.listWebGetFlag.SelectedItems[0].Text;
            delName += "!Deal!-";
            List<string> delKey = new List<string>();
            for (int i = index; i < this.listWebGetFlag.Items.Count; i++)
            {
                string dName = this.listWebGetFlag.Items[i].Name;
                if (dName.StartsWith(delName))
                {
                    isShowDeal = true;
                    dealCount++;
                }
            }
            dealCount++;

            int pIndex = 0;
            int pDealCount = 0;
            string pName = string.Empty;

            //开始判断向下移的位置
            for (int i = index+1;i<this.listWebGetFlag.Items.Count ; i++)
            {
                if (this.listWebGetFlag.Items[i].Name =="")
                {
                    pIndex = this.listWebGetFlag.Items[i].Index;
                    pName = this.listWebGetFlag.Items[i].Text;
                    break ;
                }
            }

            //开始计算下一个采集规则中是否有加工规则
            for(int i=pIndex+1 ;i<this.listWebGetFlag.Items.Count ; i++)
            {
                if (this.listWebGetFlag.Items[i].Name.StartsWith(pName ))
                {
                    pDealCount++;
                }
            }

            pIndex = pIndex + pDealCount;

            //开始删除显示
            if (isShowDeal == true)
            {
                //删除加工规则
                DelDealRule(this.listWebGetFlag.SelectedItems[0].Text, index);
            }
            this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

            int insertIndex = 0;

            insertIndex = pIndex-dealCount+1;

            this.listWebGetFlag.Items.Insert(insertIndex, Litem);
            if (isShowDeal == true)
            {
                cGatherRule gRule = new cGatherRule();

                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (Litem.Text == m_gRules[i].gName)
                    {
                        gRule = m_gRules[i];
                        break;
                    }
                }
                AddDealRule(Litem.Text, insertIndex, gRule);
            }

            this.listWebGetFlag.SelectedItems[0].EnsureVisible();

            this.IsSave.Text = "true";

        }

        private void cmdUrlEncoding_Click(object sender, EventArgs e)
        {
            frmUrlEncoding f = new frmUrlEncoding();
            f.Show();
        }

        private void frmTask_FormClosed(object sender, FormClosedEventArgs e)
        {

            //m_GatherControl.TaskManage.TaskCompleted -= tManage_Completed;
            //m_GatherControl.TaskManage.TaskStarted -= tManage_TaskStart;
            //m_GatherControl.TaskManage.TaskInitialized -= tManage_TaskInitialized;
            //m_GatherControl.TaskManage.TaskStateChanged -= tManage_TaskStateChanged;
            //m_GatherControl.TaskManage.TaskStopped -= tManage_TaskStop;
            //m_GatherControl.TaskManage.TaskError -= tManage_TaskError;
            //m_GatherControl.TaskManage.TaskFailed -= tManage_TaskFailed;
            //m_GatherControl.TaskManage.TaskAborted -= tManage_TaskAbort;
            //m_GatherControl.TaskManage.Log -= tManage_Log;
            //m_GatherControl.TaskManage.GData -= tManage_GData;

            //m_GatherControl.Dispose();
            //m_GatherControl = null;


            rm = null;
        }
       
        //private void SaveExportRule()
        //{
        //    //添加输出规则类
        //    for (int i = 0; i < m_FieldRules.Count; i++)
        //    {
        //        if (m_FieldRules[i].Field == SelectedFiled)
        //            m_FieldRules.Remove(m_FieldRules[i]);
        //    }

        //cFieldRules cfs = new cFieldRules();
        //    cFieldRule cf;
        //    List<cFieldRule> listcf = new List<cFieldRule>();
        //    for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
        //    {
        //        if (this.dataDataRules.Rows[j].Cells[1].Value!= null)
        //        {
        //            cf = new cFieldRule();
        //            cf.Field = SelectedFiled;
        //            cf.Level = int.Parse(this.dataDataRules.Rows[j].Cells[0].Value.ToString());
        //            cf.FieldRuleType = cGlobalParas.ConvertID(this.dataDataRules.Rows[j].Cells[1].Value.ToString());
        //            string ss=string.Empty;
        //            if (this.dataDataRules.Rows[j].Cells[2].Value != null)
        //            {
        //                ss = this.dataDataRules.Rows[j].Cells[2].Value.ToString();
        //                ss = ss.Replace("\r", "");
        //                ss = ss.Replace("\n", "");
        //            }

        //            cf.FieldRule = (ss == null ? "" : ss);

        //            listcf.Add(cf);
        //        }
        //    }
        //    cfs.Field = SelectedFiled;
        //    cfs.FieldRule = listcf;
        //    m_FieldRules.Add(cfs);
            
        //    this.IsSave.Text = "true";
        //}

        //private void DelExportRule()
        //{
        //    //添加输出规则类
        //    for (int i = 0; i < m_FieldRules.Count; i++)
        //    {
        //        if (m_FieldRules[i].Field == SelectedFiled)
        //            m_FieldRules.Remove(m_FieldRules[i]);
        //    }

        //    cFieldRules cfs = new cFieldRules();
        //    cFieldRule cf;
        //    List<cFieldRule> listcf = new List<cFieldRule>();
        //    for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
        //    {
        //        if (this.dataDataRules.Rows[j].Cells[1].Value != null &&
        //            this.dataDataRules.Rows[j].Cells[1].Value!=this.dataDataRules.SelectedCells [1].Value)
        //        {
        //            cf = new cFieldRule();
        //            cf.Field = SelectedFiled;
        //            cf.Level = int.Parse(this.dataDataRules.Rows[j].Cells[0].Value.ToString());
        //            cf.FieldRuleType = cGlobalParas.ConvertID(this.dataDataRules.Rows[j].Cells[1].Value.ToString());
        //            cf.FieldRule = (this.dataDataRules.Rows[j].Cells[2].Value == null ? "" : this.dataDataRules.Rows[j].Cells[2].Value.ToString());

        //            listcf.Add(cf);
        //        }
        //    }
        //    cfs.Field = SelectedFiled;
        //    cfs.FieldRule = listcf;
        //    m_FieldRules.Add(cfs);

        //    this.IsSave.Text = "true";
        //}

        //private void IsSaveSingleFile_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.IsSaveSingleFile.Checked == true)
        //    {
        //        this.txtTempFileName.Enabled = true;
        //        this.txtTempFileName.Text = this.tTask.Text + ".xml";
        //    }
        //    else
        //    {
        //        this.txtTempFileName.Enabled = false;
        //    }

        //    this.IsSave.Text = "true";
        //}

        private void txtTempFileName_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsDataProcess_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsExportGUrl_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsDelTempData_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

       
        //private void dataDataRules_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (this.listWebGetFlag.Items.Count == 0)
        //    {
        //        return;
        //    }

        //    if (e.ColumnIndex == 2 && this.dataDataRules.Rows[e.RowIndex].Cells[1].Value == null)
        //        return;

        //    SaveExportRule();
            
        //}


     

        private void dataDataRules_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Delete)
            //{
            //    DelExportRule();
            //}
        }

        private void cmdRegex_Click(object sender, EventArgs e)
        {
            try
            {
                TestGatherRule();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestGatherRule()
        {
            //if (this.txtWeblinkDemo.Text.Trim() == "")
            //{
            //    MessageBox.Show("请先输入需要测试的导航网址，再进行规则测试！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    this.txtWeblinkDemo.Focus();
            //    return;
            //}

            if (this.listWebGetFlag.Items.Count == 0)
            {
                MessageBox.Show("请先增加采集数据的规则，再进行规则的测试！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            cGlobalParas.WebCode urlCode = cGlobalParas.WebCode.auto;
            if (this.IsUrlEncode.Checked == true)
                urlCode =EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString());


            ////判断是否已经提取了示例网址，如果没有，则进行提取
            //if (this.txtWeblinkDemo.Text.ToString() == null || this.txtWeblinkDemo.Text.ToString() == "")
            //{
            //    if (this.treeTestUrl.Nodes == null && this.listWeblink.Items.Count >0)
            //        GetDemoUrl(false,this.listWeblink.Items[0] , this.txtCookie.Text , EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()),
            //        EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()));
            //    else if (this.treeTestUrl.Nodes.Count == 0 && this.listWeblink.Items.Count > 0)
            //        GetDemoUrl(false, this.listWeblink.Items[0], this.txtCookie.Text, EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()),
            //        EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()));
            //    else
            //        this.txtWeblinkDemo.Text = FillDemoUrl(this.treeTestUrl.Nodes[0]);
            //}

            if (this.listWebGetFlag.Items.Count == 0)
            {
                frmRegex f1 = new frmRegex();
                f1.Show();
                return;
            }

             //增加采集的标志
            eWebpageCutFlag c;
            List<eWebpageCutFlag> gFlag = new List<eWebpageCutFlag>();

            for (int i = 0; i < m_gRules.Count; i++)
            {
                c = new eWebpageCutFlag();
                c.id = i;
                c.Title = m_gRules[i].gName;
                c.RuleByPage = (cGlobalParas.GatherRuleByPage) (int)m_gRules[i].gRuleByPage;
                c.DataType = EnumUtil.GetEnumName<cGlobalParas.GDataType>( m_gRules[i].gType);
                c.GatherRuleType = (cGlobalParas.GatherRuleType) (int)m_gRules[i].gRuleType;
                c.XPath = m_gRules[i].xPath;
                c.NodePrty = m_gRules[i].gNodePrty;
                c.StartPos = m_gRules[i].getStart;
                c.EndPos = m_gRules[i].getEnd;
                c.LimitSign = EnumUtil.GetEnumName<cGlobalParas.LimitSign>(m_gRules[i].limitType);

                c.RegionExpression = m_gRules[i].strReg;
                c.IsMergeData = m_gRules[i].IsMergeData;
                c.NavLevel = int.Parse(m_gRules[i].NaviLevel);
                c.MultiPageName = m_gRules[i].MultiPageName;
                c.DownloadFileSavePath = m_gRules[i].sPath;
                //c.DownloadFileDealType = m_gRules[i].fileDealType;
                c.IsAutoDownloadFileImage = m_gRules[i].IsAutoDownloadFileImage;
                c.IsAutoDownloadOnlyImage = m_gRules[i].IsAutoDownloadOnlyImage;

                c.ExportRules = m_gRules[i].fieldDealRules.FieldRule;

                gFlag.Add(c);
                c = null;
            }


            string strCut = "";

            //根据用户指定的页面截取位置构造正则表达式
            for (int i = 0; i < gFlag.Count; i++)
            {
                strCut += "(?<" + gFlag[i].Title + ">" + ToolUtil.RegexReplaceTrans(gFlag[i].StartPos,true ) + ")";

                //strCut += "(?<=" + cTool.RegexReplaceTrans(gFlag[i].StartPos) + ")";

                switch (gFlag[i].LimitSign)
                {
                    case cGlobalParas.LimitSign.NoLimit:
                        strCut += ".*?";
                        break;
                    case cGlobalParas.LimitSign.NoWebSign:
                        strCut += "[^<>]*?";
                        break;
                    case cGlobalParas.LimitSign.OnlyCN:
                        strCut += "[\\u4e00-\\u9fa5]*?";
                        break;
                    case cGlobalParas.LimitSign.OnlyDoubleByte:
                        strCut += "[^\\x00-\\xff]*?";
                        break;
                    case cGlobalParas.LimitSign.OnlyNumber:
                        strCut += "[\\d]*?";
                        break;
                    case cGlobalParas.LimitSign.OnlyChar:
                        strCut += "[\\x00-\\xff]*?";
                        break;
                    case cGlobalParas.LimitSign.Custom:
                        //strCut += cTool.RegexReplaceTrans(gFlag[i].RegionExpression.ToString());
                        strCut += gFlag[i].RegionExpression.ToString();
                        break;
                    default:
                        strCut += "[\\S\\s]*?";
                        break;
                }
                strCut += "(?=" + ToolUtil.RegexReplaceTrans(gFlag[i].EndPos,true ) + ")|";
            }

            strCut = strCut.Substring(0, strCut.Length - 1);

            bool IsStarted = false;

            foreach (Form f in Application.OpenForms )
            {
                if (f is frmRegex )
                {
                    IsStarted =true ;
                    f.Activate();
                    break ;
                }
            }

            if (IsStarted==false )
            {
                frmRegex f1 = new frmRegex();

                f1 = new frmRegex();
                try
                {
                    cGatherWeb g = new cGatherWeb(Program.getPrjPath());
                    //if (this.txtWeblinkDemo.Text.ToString() != null && this.txtWeblinkDemo.Text.ToString() != "")
                    //{
                    //    try
                    //    {
                    //        string sCookie = this.txtCookie.Text;
                    //        f1.rtbSource.Text = g.GetHtml(this.txtWeblinkDemo.Text, EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()), 
                    //            this.IsUrlEncode.Checked,this.isTwoUrlCode.Checked,  urlCode , ref sCookie, this.txtStartPos.Text.Trim (), this.txtEndPos.Text.Trim () ,
                    //            true, this.IsAutoUpdateHeader.Checked,"",this.isGatherCoding.Checked, this.txtCodingFlag.Text ,
                    //            this.txtCodingUrl.Text ,this.txtPluginsCoding.Text  );
                    //    }
                    //    catch (System.Exception)
                    //    {
                    //        //捕获错误，但不处理，需要让程序继续执行下去
                    //    }
                    //}
                    f1.rtbRegex.Text = strCut;

                    //标识已经去除了回车换行
                    f1.IsWordwrap.Checked = true;

                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (f1 != null)
                        f1 = null;
                    return;
                }
                f1.Show();
            
            }

        }

        private void groupBox8_Enter(object sender, EventArgs e)
        {

        }
      
        private void IsExportGDate_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IncrBy_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void TaskType_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void udGIntervalTime_ValueChanged(object sender, EventArgs e)
        {
            if (udGIntervalTime.Value >= this.udGIntervalTime1.Value)
            {
                this.udGIntervalTime1.Value = this.udGIntervalTime.Value;
            }
            this.udGIntervalTime1.Minimum = this.udGIntervalTime.Value;

            this.IsSave.Text = "true";
        }

        private void raExportCSV_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportCSV.Checked == true)
            {
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportWord.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "" && this.txtFileName.Text.Length > 5 && this.txtFileName.Text.LastIndexOf(".")>0)
                {
                    this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.LastIndexOf(".")) + ".csv";
                }

                this.IsSave.Text = "true";
            }
        }

        //private void butOpenUrl_Click(object sender, EventArgs e)
        //{
        //    //判断是否已经提取了示例网址，如果没有，则进行提取
          

          
        //    else
        //    {
        //        System.Diagnostics.Process.Start(this.txtWeblinkDemo.Text.ToString());
        //    }
        //}


        private void IsHeaderPublish_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtHeader_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdClearHeader_Click(object sender, EventArgs e)
        {
            this.txtHeader.Text = "";
        }

        private void cmdSetHeader_Click(object sender, EventArgs e)
        {
            //取最大导航级别的网址
            int maxLevel = 0;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (int.Parse(this.listWeblink.Items[i].SubItems[2].Text) > maxLevel)
                    maxLevel = int.Parse(this.listWeblink.Items[i].SubItems[2].Text);

            }

            frmSetHeader f = new frmSetHeader();
            f.strHeader = this.txtHeader.Text;
            f.MaxLevel = maxLevel;
            f.rHeader = GetHttpHeader;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetHttpHeader(string strHeader)
        {
            this.txtHeader.Text = strHeader;
        }

        #region 将两个datatable的数据合并在一起
        //将datarow与datatable合并在一起 这是一个一对N的关系
        //private DataTable MergeDataTable(DataRow dtr1, DataTable dt2)
        //{
        //    DataTable dt1 = dtr1.Table.Clone();
        //    dt1.ImportRow(dtr1);

        //    DataTable dt3 = dt1.Clone();
        //    for (int i = 0; i < dt2.Columns.Count; i++)
        //    {
        //        dt3.Columns.Add(dt2.Columns[i].ColumnName);
        //    }
        //    object[] obj = new object[dt3.Columns.Count];

        //    for (int i = 0; i < dt1.Rows.Count; i++)
        //    {
        //        dt1.Rows[i].ItemArray.CopyTo(obj, 0);
        //        dt3.Rows.Add(obj);
        //    }


        //    if (dt1.Rows.Count >= dt2.Rows.Count)
        //    {
        //        for (int i = 0; i < dt2.Rows.Count; i++)
        //        {
        //            for (int j = 0; j < dt2.Columns.Count; j++)
        //            {
        //                dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        DataRow dr3;
        //        for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
        //        {
        //            dr3 = dt3.NewRow();
        //            dt3.Rows.Add(dr3);
        //        }
        //        for (int i = 0; i < dt2.Rows.Count; i++)
        //        {
        //            for (int j = 0; j < dt2.Columns.Count; j++)
        //            {
        //                dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
        //            }
        //        }
        //    }

        //    return dt3;
        //}

        private DataTable MergeDataTable(DataRow dtr1, DataTable dt2)
        {
            //将dtr1 datarow 转换成Table
            DataTable dt1 = dtr1.Table.Clone();
            dt1.ImportRow(dtr1);

            //判断dt2中是否有与dtr1重复的列，如果有，则删除，删除目标为dt1

            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                bool isExit = false;
                string cName = "";

                for (int j = 0; j < dt1.Columns.Count; j++)
                {
                    if (dt2.Columns[i].ColumnName == dt1.Columns[j].ColumnName)
                    {
                        isExit = true;
                        cName = dt1.Columns[j].ColumnName;
                        break;
                    }
                }

                if (isExit == true)
                {
                    dt1.Columns.Remove(cName);
                }
            }

            //建立一个整合的数据表
            DataTable dt3 = dt1.Clone();

            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                dt3.Columns.Add(dt2.Columns[i].ColumnName);
            }



            object[] obj = new object[dt3.Columns.Count];

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                dt1.Rows[i].ItemArray.CopyTo(obj, 0);
                dt3.Rows.Add(obj);
            }

            if (dt1.Rows.Count >= dt2.Rows.Count)
            {
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }
            else
            {
                DataRow dr3;
                for (int i = 0; i < dt2.Rows.Count - dt1.Rows.Count; i++)
                {
                    dr3 = dt3.NewRow();
                    for (int m = 0; m < dt1.Columns.Count; m++)
                    {
                        dr3[m] = dt1.Rows[0][m].ToString();
                    }
                    dt3.Rows.Add(dr3);
                }
                for (int i = 0; i < dt2.Rows.Count; i++)
                {
                    for (int j = 0; j < dt2.Columns.Count; j++)
                    {
                        dt3.Rows[i][j + dt1.Columns.Count] = dt2.Rows[i][j].ToString();
                    }
                }
            }

            return dt3;
        }
        #endregion

        #region 合并数据操作
        private DataTable MergeData(DataTable tmpData, List<eWebpageCutFlag> CutFlag)
        {
            if (tmpData == null || tmpData.Rows.Count == 0)
                return null;

            object oldRow;
            object newRow;
            DataTable d1 = tmpData.Clone();
            object[] mRow;

            d1 = tmpData.Clone();

            oldRow = tmpData.Rows[0].ItemArray.Clone();



            for (int i = 0; i < tmpData.Rows.Count; i++)
            {
                newRow = tmpData.Rows[i].ItemArray.Clone();
                mRow = MergeRow(oldRow, newRow, CutFlag);

                DataRow r = d1.NewRow();

                if (mRow != null)
                {
                    oldRow = mRow.Clone();
                    r.ItemArray = ((object[])oldRow);
                    if (d1.Rows.Count > 0)
                    {
                        d1.Rows.Remove(d1.Rows[d1.Rows.Count - 1]);
                        d1.Rows.Add(r);
                    }
                    else
                    {
                        d1.Rows.Add(r);
                    }
                }
                else
                {
                    r.ItemArray = ((object[])oldRow);
                    d1.Rows.Add(r);
                    oldRow = newRow;
                }
            }

            return d1;
        }

        private object[] MergeRow(object row1, object row2, List<eWebpageCutFlag> CutFlag)
        {
            object[] oldRow = ((object[])row1);
            object[] newRow = ((object[])row2);
            object[] mergeRow = new object[CutFlag.Count];

            for (int i = 0; i < CutFlag.Count; i++)
            {
                if (CutFlag[i].IsMergeData == false)
                {
                    if (oldRow[i].ToString() == newRow[i].ToString())
                    {
                        mergeRow[i] = oldRow[i].ToString();
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    mergeRow[i] = oldRow[i].ToString() + newRow[i].ToString();
                }
            }
            return mergeRow;
        }
        #endregion


        private void dataTestGather_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.ToString(), rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void IsProxy_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsProxy.Checked == true)
            {
                if (Program.SominerVersion == cGlobalParas.VersionType.Free)
                {
                    this.IsProxyFirst.Checked = true;
                    this.IsProxyFirst.Enabled = false ;
                }
                else
                {
                    this.IsProxyFirst.Enabled = true;
                }
            }
            else
                this.IsProxyFirst.Enabled = false;

            this.IsSave.Text = "true";
        }

        private void IsProxyFirst_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSilent_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsUrlNoneRepeat_CheckedChanged(object sender, EventArgs e)
        {
            if (this.IsUrlNoneRepeat.Checked == true)
                this.IsSucceedUrl.Enabled = true;
            else
                this.IsSucceedUrl.Enabled = false;

             this.IsSave.Text = "true";
        }

        private void raSingleGather_CheckedChanged(object sender, EventArgs e)
        {

        }

        //private void IsNavigPage_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (this.IsNavigPage.Checked == true)
        //    {
        //        this.groupBox14.Visible = true;
        //        this.groupBox19.Visible = false;
        //    }
        //    else
        //    {
        //        this.groupBox14.Visible = false;
        //        this.groupBox19.Visible = true;
        //    }
        //    this.IsSave.Text = "true";
        //}

        //private void cmdAddPage_Click(object sender, EventArgs e)
        //{
        //    frmAddMultiPage fn = new frmAddMultiPage("", "");
        //    fn.fState = cGlobalParas.FormState.New;
        //    fn.rNavRule = new frmAddMultiPage.ReturnNavRule(GetNavRule);
        //    fn.ShowDialog();
        //    fn.Dispose();
        //}

        //private void GetNavRule(cGlobalParas.FormState fState, string[] strs)
        //{
        //    if (fState ==cGlobalParas.FormState.New )
        //        this.dataMultiPage.Rows.Add(strs[0].ToString(), strs[1].ToString ());
        //    else if (fState == cGlobalParas.FormState.Edit)
        //    {
        //        int curr = this.dataMultiPage.CurrentCell.RowIndex;
        //        this.dataMultiPage.Rows[curr].Cells[0].Value = strs[0].ToString();
        //        this.dataMultiPage.Rows[curr].Cells[1].Value = strs[1].ToString();
        //    }

        //    this.IsSave.Text = "true";
        //}

        //private void cmdDelPage_Click(object sender, EventArgs e)
        //{
        //    this.dataMultiPage.Focus();
        //    SendKeys.Send("{Del}");
        //    this.IsSave.Text = "true";
        //}

        private void groupBox14_Enter(object sender, EventArgs e)
        {

        }

      

        private void button13_Click(object sender, EventArgs e)
        {
            this.rmenuInsertDBField.Show(this.button13, 0, 21);
        }

        private void rmenuInsertDBField_Opening(object sender, CancelEventArgs e)
        {
            this.rmenuInsertDBField.Items.Clear();
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("主从表主键参数", null, null, "rmenuDBMasterPK"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("主从表外键键参数", null, null, "rmenuDBDetailFK"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("自动编号 {AutoCode:自动编号字段名称}", null, null, "rmenuDBAutoID"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("当前时间", null, null, "rmenuDBCurTime"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripSeparator());

            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                if (!this.listWebGetFlag.Items[i].Text.StartsWith(" "))
                    this.rmenuInsertDBField.Items.Add("{" + this.listWebGetFlag.Items[i].Text + "}");
            }
        }

        private void IsPublishData_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// 设置发布数据界面显示
        /// </summary>
        private void SetGatherPublishDB()
        {
            this.groupBox4.Enabled = true;
            this.groupBox9.Enabled = true;

            if (Program.SominerVersion !=cGlobalParas.VersionType.Free)
                this.raInsertDB.Enabled = true;

            this.IsDelTempData.Enabled = true;

            this.IsDelTempData.Enabled = true;
            this.button6.Enabled = true ;

            this.label16.Enabled = true;
            this.udPublishThread.Enabled = true;
        }

        /// <summary>
        /// 设置不发布数据界面显示
        /// </summary>
        private void SetGatherDB()
        {
            this.groupBox4.Enabled = false;
            this.groupBox9.Enabled = false;

            this.raInsertDB.Enabled = false ;
            this.IsDelTempData.Enabled = false ;


            this.IsDelTempData.Enabled = false;

            this.label16.Enabled = false;
            this.udPublishThread.Enabled = false;

            this.button6.Enabled = false;
        }

        /// <summary>
        /// 设置直接入库
        /// </summary>
        private void SetInsertDB()
        {
            this.groupBox4.Enabled = false;
            this.groupBox9.Enabled = true;

            if (Program.SominerVersion != cGlobalParas.VersionType.Free)
                this.raInsertDB.Enabled = true ;
            this.IsDelTempData.Enabled = false;



            this.label16.Enabled = true;
            this.udPublishThread.Enabled = true;

            this.button6.Enabled = true ;
        }

        private void rmenuInsertDBField_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string str = "";

            if (e.ClickedItem.Name == "rmenuDBMasterPK")
            {
                str = "{PK}";
            }
            else if (e.ClickedItem.Name == "rmenuDBDetailFK")
            {
                str = "{FK}";
            }
            else if (e.ClickedItem.Name == "rmenuDBAutoID")
            {
                str = "{AutoCode:}";
            }
            else if (e.ClickedItem.Name == "rmenuDBCurTime")
            {
                str = "{CurrentTimer}";
            }
            else
            {

                Match s;

                if (Regex.IsMatch(e.ClickedItem.ToString(), "[{].*[}]"))
                {
                    s = Regex.Match(e.ClickedItem.ToString(), "[{].*[}]");
                }
                else
                {
                    s = Regex.Match(e.ClickedItem.ToString(), "[<].*[>]");
                }

                str = s.Groups[0].Value;
            }

            int startPos = this.txtInsertSql.SelectionStart;
            int l = this.txtInsertSql.SelectionLength;

            this.txtInsertSql.Text = this.txtInsertSql.Text.Substring(0, startPos) + str + this.txtInsertSql.Text.Substring(startPos + l, this.txtInsertSql.Text.Length - startPos - l);

            this.txtInsertSql.SelectionStart = startPos + str.Length;
            this.txtInsertSql.ScrollToCaret();

            this.txtInsertSql.Focus();
        }

        private void txtNextPage_TextChanged(object sender, EventArgs e)
        {

        }

        private void listWebGetFlag_DoubleClick(object sender, EventArgs e)
        {
            EditGatherRule();
        }

        private void listWebGetFlag_Click(object sender, EventArgs e)
        {
            SetDataState();
        }

      

        /// <summary>
        /// 清空排重库
        /// </summary>
        private void ClearUrlsDB()
        {
            if (MessageBox.Show("是否删除此采集任务的排重库，删除后此任务可以重新进行数据采集。", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.No)
                return;

            //GetTaskFullClassName(this.comTaskClass.Tag)

            string tClassPath = this.comTaskClass.Tag.ToString();

            tClassPath = tClassPath.Replace("tasks", "");
            if (tClassPath.StartsWith("\\"))
                tClassPath = tClassPath.Substring(1, tClassPath.Length - 1);

            string FileName = Program.getPrjPath() + "urls\\" + tClassPath.Replace("\\", "-") + "-" + this.tTask.Text  + ".db";

            try
            {
                System.IO.File.Delete(FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("删除排重库发生错误，错误信息：" + ex.Message, "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("删除排重库成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        //private void button15_Click(object sender, EventArgs e)
        //{
        //    frmAddNextRule f = new frmAddNextRule(this.txtNextPage.Text);
        //    f.TestUrl = this.txtWebLink.Text;
        //    f.rNavRule = new frmAddNextRule.ReturnNavRule(GetNavRule);
        //    f.ShowDialog();
        //    f.Dispose();
        //}

        //private void GetNavRule(string strNavRule)
        //{
        //    this.txtNextPage.Text = strNavRule;
        //}

        //private void dataMultiPage_DoubleClick(object sender, EventArgs e)
        //{
        //    if (this.dataMultiPage.Rows.Count == 0)
        //    {
        //        return;
        //    }

        //    int curr = this.dataMultiPage.CurrentCell.RowIndex;

        //    frmAddMultiPage f = new frmAddMultiPage(this.dataMultiPage.Rows[curr].Cells[0].Value.ToString(), this.dataMultiPage.Rows[curr].Cells[1].Value.ToString());

        //    f.fState = cGlobalParas.FormState.Edit;
        //    f.rNavRule = new frmAddMultiPage.ReturnNavRule(this.GetNavRule);
        //    f.ShowDialog();
        //    f.Dispose();

        //}

        //private void button16_Click(object sender, EventArgs e)
        //{
        //    //先获取正常的网址
        //    List<string> urls = GetDemoUrl();

        //    if (urls==null || urls.Count ==0)
        //        return ;

        //    string url = urls[0].ToString();

        //    //增加导航规则，尽管是多页，但还是按照导航的逻辑进行处理

        //    string DemoUrl="";

        //    List<cNavigRule> tempcns;
        //    cNavigRule tempcn;
        //    for (int i = 0; i < this.dataMultiPage.Rows.Count; i++)
        //    {
                
        //            tempcns = new List<cNavigRule>();
        //            tempcn = new cNavigRule();

        //            tempcn.Url = url;
        //            tempcn.Level = 1;


        //            tempcn.NaviStartPos ="";
        //            tempcn.NaviEndPos = "";
        //            tempcn.NavigRule = this.dataMultiPage.Rows[i].Cells[1].Value.ToString();


        //            tempcns.Add(tempcn);

        //            DemoUrl = AddDemoUrl(url, true, tempcns);

                
        //    }

        //    if (!Regex.IsMatch(DemoUrl, @"(http|https|ftp)+://[^\s]*"))
        //    {
        //        MessageBox.Show(rm.GetString("Error6") + " 多页规则匹配结果：" + DemoUrl, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    try
        //    {
        //        System.Diagnostics.Process.Start(DemoUrl);
        //    }
        //    catch (System.Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }


        //}

        private void ModifyTitle(string strTitle)
        {
            this.Text = strTitle ;
        }

        private void frmTask_Shown(object sender, EventArgs e)
        {
            switch (this.FormState)
            {
                case cGlobalParas.FormState.New:
                    ModifyTitle("新建采集任务");
                    break;
                case cGlobalParas.FormState.Edit:
                    //编辑状态进来不能修改分类
                    ModifyTitle("修改采集任务：" + this.tTask.Text);

                    break;
                case cGlobalParas.FormState.Browser:
                    ModifyTitle("浏览采集任务：" + this.tTask.Text);
                    break;
                default:
                    break;
            }
        }


        public void ExportGatherLog(cGlobalParas.LogType lType, string lText)
        {
            lText = System.DateTime.Now.ToString() + " " + lText;

            this.txtTestLog.AppendText(lType, lText);

        }

        public void ExportData(DataTable d)
        {
            //this.dataTestGather.gData = d;
            this.dTestData.DataSource = d;
            //this.dTestData.databin
        }

       

        #region 委托代理 用于后台线程调用 配置UI线程的方法、属性

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// 委托设置对象属性
        /// </summary>
        /// <param name="Instance">对象</param>
        /// <param name="Property">属性名</param>
        /// <param name="value">属性值</param>
        private void SetValue(object Instance, string Property, object value)
        {
            Type iType = Instance.GetType();
            object inst = null;

            if (iType.Name.ToString() == "ToolStripButton")
            {
                //inst = this.toolStrip1;
            }
            else
            {
                inst = Instance;
            }

            bool a = (bool)GetPropertyValue(inst, "InvokeRequired");

            if (a)
            {
                bindvalue d = new bindvalue(SetValue);
                this.Invoke(d, new object[] { Instance, Property, value });
            }
            else
            {
                SetPropertyValue(Instance, Property, value);
            }
        }
        /// <summary>
        /// 委托执行实例的方法，方法必须都是Public 否则会出错
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeMethod(object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokemethod d = new invokemethod(InvokeMethod);
                return this.Invoke(d, new object[] { Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }

        /// <summary>
        /// 委托执行实例的方法
        /// </summary>
        /// <param name="InstanceInvokeRequired">窗体控件对象</param>
        /// <param name="Instance">需要执行方法的对象</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokeChailMethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(InstanceInvokeRequired, "InvokeRequired"))
            {
                invokechailmethod d = new invokechailmethod(InvokeChailMethod);
                return this.Invoke(d, new object[] { InstanceInvokeRequired, Instance, Method, parameters });
            }
            else
            {
                return MethodInvoke(Instance, Method, parameters);
            }
        }
        /// <summary>
        /// 委托执行实例的属性的方法
        /// </summary>
        /// <param name="Instance">类实例</param>
        /// <param name="Property">属性名</param>
        /// <param name="Method">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private object InvokePMethod(object Instance, string Property, string Method, object[] parameters)
        {
            if ((bool)GetPropertyValue(Instance, "InvokeRequired"))
            {
                invokepmethod d = new invokepmethod(InvokePMethod);
                return this.Invoke(d, new object[] { Instance, Property, Method, parameters });
            }
            else
            {
                return MethodInvoke(GetPropertyValue(Instance, Property), Method, parameters);
            }
        }
        /// <summary>
        /// 获取实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        /// <returns>属性值</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// 设置实例的属性值
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="PropertyName">属性名</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// 执行实例的方法
        /// </summary>
        /// <param name="ClassInstance">类实例</param>
        /// <param name="MethodName">方法名</param>
        /// <param name="parameters">参数列表</param>
        /// <returns>返回值</returns>
        private static object MethodInvoke(object ClassInstance, string MethodName, object[] parameters)
        {
            if (parameters == null)
            {
                parameters = new object[0];
            }
            Type myType = ClassInstance.GetType();
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i)
            {
                types[i] = parameters[i].GetType();
            }
            MethodInfo myMethodInfo = myType.GetMethod(MethodName, types);
            return myMethodInfo.Invoke(ClassInstance, parameters);
        }

        #endregion

        private void IsDoPostBack_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void listWeblink_DoubleClick(object sender, EventArgs e)
        {
            EditUrlRule();
        }

        private delegate void delegateTestUrl(bool isAll,string Cookie, cGlobalParas.WebCode webCode,
            cGlobalParas.WebCode urlCode,List<eWebLink> wLinks);
        private void button2_Click(object sender, EventArgs e)
        {
            TestUrlsRule();
        }

        private void TestUrlsRule()
        {
            if (this.listWeblink.Items.Count == 0)
            {
                MessageBox.Show("请先添加一个网址，再进行网址解析测试工作！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            ListViewItem cItem = null;
            if (this.listWeblink.SelectedItems.Count == 0)
                cItem = this.listWeblink.Items[0];
            else
                cItem = this.listWeblink.SelectedItems[0];

            bool isAll=true;
            if (int.Parse(cItem.SubItems[6].Text) > 1000)
            {
                if (MessageBox.Show("当前测试网址存在参数且根据参数解析数量较大，建议只对第一个参数值的网址进行测试，确定？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    isAll = false ;
            }

            this.treeTestUrl.Nodes.Clear();

            //填充网址规则
            List<eWebLink> wLinks = new List<eWebLink>();
            FillWebLinks(ref wLinks);

            ExportGatherLog(cGlobalParas.LogType.Info, "获取指定测试解析的网址：" + cItem.Text );

            this.button2.Enabled = false;
            this.button5.Enabled = false;
            m_testNaving = true;

            this.TabTest.Checked = true;
            this.Tab1.Checked = false;
            this.panel1.Visible = false;
            this.panel2.Visible = false;
            this.panel3.Visible = false;
            this.panel4.Visible = false;
            this.panelTest.Visible = true;

            Application.DoEvents();

            delegateTestUrl sd = new delegateTestUrl(this.GetDemoUrl);

            sd.Invoke(isAll, this.txtCookie.Text ,
                EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()),
                    EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()), wLinks);


            m_testNaving = false;
            this.button2.Enabled = true;
            this.button5.Enabled = true;

        }

        private void CallbackTestUrl(IAsyncResult ar)
        {
            try
            {
                //((delegateTestUrl)ar).EndInvoke(ar);
                SetValue(this.button2, "Text", "测试网址解析");
                SetValue(this.button2, "Enabled", true);

                SetValue(this.button5, "Text", "启动测试");

                m_testNaving = false;

                string url = FillDemoUrl(this.treeTestUrl.Nodes[0]);

                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "网址解析测试结束" });

                
            }
            catch { return; }
        }

        private string FillDemoUrl(TreeNode tNode)
        {
            string url = string.Empty;
            if (tNode.Nodes.Count > 0)
            {
                if (tNode.Nodes[0].Nodes.Count > 0)
                    url=FillDemoUrl(tNode.Nodes[0]);
                else
                    return tNode.Nodes[0].Text;
            }
            return url;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            frmSmartGRule f = new frmSmartGRule();
            f.Cookie = this.txtCookie.Text;

            f.rGatherRule = new frmSmartGRule.ReturnGatherRule(this.AddGatherRules);
            f.ShowDialog();
            f.Dispose();
        }

        private void txtTestGUrl_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void txtTestLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        public void CopyUrl()
        {
            if (this.listWeblink.SelectedItems.Count  == 0)
            {
                return ;
            }

            Clipboard.SetDataObject(this.listWeblink.SelectedItems [0]);
            
        }

        private void PasterUrl()
        {
            if (Clipboard.ContainsData(DataFormats.Serializable))
            {
                try
                {
                    IDataObject cdata;
                    cdata = Clipboard.GetDataObject();
                    ListViewItem item = (ListViewItem)cdata.GetData(DataFormats.Serializable);
                    if (this.listWeblink.Columns.Count != item.SubItems.Count )
                    {
                        MessageBox.Show("无法粘贴采集网址以外的数据", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    this.listWeblink.Items.Add(item);
                    this.IsSave.Text = "true";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("粘贴出错，错误信息:" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }

        private void rmenuCopyUrl_Click(object sender, EventArgs e)
        {
            CopyUrl();
        }

        private void rmenuPasterUrl_Click(object sender, EventArgs e)
        {
            PasterUrl();
        }

        private void contextMenuStrip2_Opening(object sender, CancelEventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count == 0)
            {
                this.rmenuCopyUrl.Enabled = false;
                this.rmenuEditUrl.Enabled = false;
                this.rmenuDelUrl.Enabled = false;
            }
            else
            {
                this.rmenuCopyUrl.Enabled = true;
                this.rmenuEditUrl.Enabled = true;
                this.rmenuDelUrl.Enabled = true;
            }

            if (Clipboard.ContainsData(DataFormats.Serializable))
            {
                this.rmenuPasterUrl.Enabled = true;
            }
            else
            {
                this.rmenuPasterUrl.Enabled = false;
            }
        }

        private void rmenuCopyRule_Click(object sender, EventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems[0].Name == "")
                CopyRule();
            else
                CopyDataRule();
        }

        public void CopyRule()
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
            {
                return;
            }

            Clipboard.SetDataObject(this.listWebGetFlag.SelectedItems[0]);

        }

        public void CopyDataRule()
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
            {
                return;
            }

            Clipboard.SetDataObject(this.listWebGetFlag.SelectedItems[0]);

        }

        private void PasterRule()
        {
            if (Clipboard.ContainsData(DataFormats.Serializable))
            {
                try
                {
                    IDataObject cdata;
                    cdata = Clipboard.GetDataObject();
                    ListViewItem item = (ListViewItem)cdata.GetData(DataFormats.Serializable);
                    if (item.SubItems.Count != 16 && item.SubItems.Count != 2)
                    {
                       
                            MessageBox.Show("无法粘贴采集规则以外的数据", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        
                    }

                    if (item.SubItems.Count!=2)
                    {
                        string gName = item.Text;
                        string gNewName=item.Text + "复制";
                        item.Text = gNewName;

                        //for (int i = 0; i < m_gRules.Count; i++)
                        //{
                        //    if (gName == m_gRules[i].gName)
                        //        gRule = m_gRules[i].Clone();
                        //}

                        //gRule.gName =gNewName;

                        this.listWebGetFlag.Items.Add(item);


                        cGatherRule gRule = new cGatherRule();

                        gRule.gName = item.Text;
                        gRule.gRuleByPage = EnumUtil.GetEnumName<cGlobalParas.GatherRuleByPage>(item.SubItems[1].Text);
                        gRule.gType = item.SubItems[2].Text;

                        if (item.SubItems[3].Text == "Normal")
                            gRule.gRuleType = cGlobalParas.GatherRuleType.Normal;
                        else if (item.SubItems[3].Text == "XPath")
                            gRule.gRuleType = cGlobalParas.GatherRuleType.XPath;
                        else if (item.SubItems[3].Text == "Smart")
                            gRule.gRuleType = cGlobalParas.GatherRuleType.Smart;

                        gRule.xPath = item.SubItems[4].Text;
                        gRule.gNodePrty = item.SubItems[5].Text;
                        gRule.getStart = item.SubItems[6].Text;
                        gRule.getEnd = item.SubItems[7].Text;
                        gRule.limitType = item.SubItems[8].Text;
                        gRule.strReg = item.SubItems[9].Text;
                        if (item.SubItems[10].Text == "Y")
                            gRule.IsMergeData = true;
                        else
                            gRule.IsMergeData = false;

                        gRule.NaviLevel = item.SubItems[11].Text;
                        gRule.MultiPageName = item.SubItems[12].Text;
                        gRule.sPath = item.SubItems[13].Text;
                        //gRule.fileDealType=this.listWebGetFlag.SelectedItems[0].SubItems[14].Text;

                        if (item.SubItems[14].Text == "Y")
                            gRule.IsAutoDownloadFileImage = true;
                        else
                            gRule.IsAutoDownloadFileImage = false;


                        if (item.SubItems[15].Text == "Y")
                            gRule.IsAutoDownloadOnlyImage = true;
                        else
                            gRule.IsAutoDownloadOnlyImage = false;

                        eFieldRules fRules = new eFieldRules();
                        fRules.Field = item.Text;
                        List<eFieldRule> listcf = new List<eFieldRule>();
                    
                        fRules.Field = item.Text;
                        fRules.FieldRule = listcf;

                        gRule.fieldDealRules = fRules;

                        //AddDealRule(gNewName, this.listWebGetFlag.Items.Count - 1, gRule);

                        m_gRules.Add(gRule);
                    }
                    else
                    {
                       //粘贴的是数据加工规则
                       //先计算当前是属于那个采集规则
                        int index = 0;
                        int startindex = 0;
                        if (this.listWebGetFlag.SelectedItems.Count == 0)
                        {
                            //当前没有选中则粘贴到最后一个
                            startindex = this.listWebGetFlag.Items.Count - 1;
                        }
                        else
                        {
                            startindex = this.listWebGetFlag.SelectedItems[0].Index;
                        }

                        for (int i = startindex; i > 0; i--)
                        {
                            if (this.listWebGetFlag.Items[i].Name == "")
                            {
                                index = this.listWebGetFlag.Items[i].Index;
                                break;
                            }
                        }

                        string gName = this.listWebGetFlag.Items[index].Text;
                        int gCount = 0;

                        //开始计算此采集规则下有多少个加工规则
                        for (int i = index + 1; i < this.listWebGetFlag.Items.Count; i++)
                        {
                            string dName = this.listWebGetFlag.Items[i].Name;
                            if (dName.StartsWith(gName))
                            {
                                gCount++;
                            }
                            else
                                break;
                        }

                        item.Name = gName + "!Deal!-" + gCount.ToString();
                        
                        //规则应该是插入到最后一个
                        if (gCount == 0)
                        {
                            //表示当前的采集规则下没有加工规则
                            this.listWebGetFlag.Items[index].ImageIndex=17;
                        }
                        this.listWebGetFlag.Items.Insert(index + gCount+1, item);

                        //增加成功后，开始添加集合数据
                        for (int n = 0; n < m_gRules.Count; n++)
                        {
                            if (gName == m_gRules[n].gName)
                            {
                                eFieldRule fRule=new eFieldRule ();
                                fRule .FieldRuleType=EnumUtil.GetEnumName<cGlobalParas.ExportLimit>( item.Text.Trim ());
                                fRule.FieldRule=item.SubItems[1].Text ;
                                m_gRules[n].fieldDealRules.FieldRule.Add(fRule);
                                break;
                            }
                        }

                    }
                    
                    this.IsSave.Text = "true";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("粘贴出错，错误信息:" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }

        private void rmenuPasteRule_Click(object sender, EventArgs e)
        {
            PasterRule();
        }

        private void contextMenuStrip3_Opening(object sender, CancelEventArgs e)
        {
            if (this.listWebGetFlag.SelectedItems.Count == 0)
            {
                this.rmenuCopyRule.Enabled = false;
                this.rmenuEditRule.Enabled = false;
                this.rmenuDelRule.Enabled = false;
            }
            else
            {
                this.rmenuCopyRule.Enabled = true;

                if (this.listWebGetFlag.SelectedItems[0].Name == "")
                {
                    this.rmenuEditRule.Enabled = true;
                }
                else
                    this.rmenuEditRule.Enabled = false ;

            }

            if (Clipboard.ContainsData(DataFormats.Serializable))
            {
                this.rmenuPasteRule.Enabled = true;
            }
            else
            {
                this.rmenuPasteRule.Enabled = false;
            }

            
        }

        private void button6_Click(object sender, EventArgs e)
        {
            frmSelectPublishRule f = new frmSelectPublishRule();
            f.rPName = this.GetPRule;
            f.ShowDialog();
            f.Dispose();

        }

        private void GetPRule(string pName)
        {
            oPublishTask p = new oPublishTask(Program.getPrjPath());
            ePublishTask ep = new ePublishTask();
            ep= p.LoadSingleTask(pName);

            if (ep.PublishType == cGlobalParas.PublishType.publishTemplate)
            {
                this.raPublishByTemplate.Checked = true;

                //if (p.GetIsDelRepeat(0) == true)
                //    this.IsDelRepRow.Checked = true;
                //else
                //    this.IsDelRepRow.Checked = false;


                this.comTemplate.Text = ep.TemplateName;
                this.txtUser.Text = ep.User;
                this.txtPwd.Text = ep.Password;
                this.txtDomain.Text = ep.Domain;
                this.txtTDbConn.Text = ep.TemplateDBConn;

                this.dataParas.Rows.Clear();

                for (int pi = 0; pi < p.GetPublishParas(0).Count; pi++)
                {
                    this.dataParas.Rows.Add();
                    this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = p.GetPublishParas(0)[pi].DataLabel;
                    cGlobalParas.PublishParaType pType = p.GetPublishParas(0)[pi].DataType;
                    if (pType == cGlobalParas.PublishParaType.NoData)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                    }
                    if (pType == cGlobalParas.PublishParaType.Gather)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = p.GetPublishParas(0)[pi].DataValue;
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = "";
                    }
                    else if (pType == cGlobalParas.PublishParaType.Custom)
                    {
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = "{手工输入}";
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[2].Value = p.GetPublishParas(0)[pi].DataValue;
                    }
                }
            }
            else if (ep.PublishType == cGlobalParas.PublishType.PublishData)
            {
                this.raPublishByTemplate.Checked = false;

                //if (p.GetIsDelRepeat(0) == true)
                //    this.IsDelRepRow.Checked = true;
                //else
                //    this.IsDelRepRow.Checked = false;

                cGlobalParas.DatabaseType dType = ep.DataType;
                if (dType == cGlobalParas.DatabaseType.Access)
                    this.raExportAccess.Checked = true;
                else if (dType == cGlobalParas.DatabaseType.MSSqlServer)
                    this.raExportMSSQL.Checked = true;
                else if (dType == cGlobalParas.DatabaseType.MySql)
                    this.raExportMySql.Checked = true;

                this.txtDataSource.Text = ep.DataSource;
                this.comTableName.Text = ep.DataTable;
                this.txtInsertSql.Text = ep.InsertSql;


            }
            else
            {
                MessageBox.Show("未能正确识别此发布规则或发布规则不符合导入的要求，导入失败！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.udPublishThread.Value = ep.ThreadCount;

            p = null;
        }

        private void udPublishThread_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udPIntervalTime_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPublishSucceedFlag_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdBrowserPlugins1_Click(object sender, EventArgs e)
        {

            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";
          

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //this.txtPluginsCookie.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdBrowserPlugins2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPluginsDeal.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdBrowserPlugins3_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPluginsPublish.Text = this.openFileDialog1.FileName;
            }
        }

      

        private void txtPluginsCookie_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPluginsDeal_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPluginsPublish_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsUrlAutoRedirect_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtWatermark_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comFontType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void upFontSize_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsFontWeight_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsFontItalic_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comWatermarkPos_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

       

        //测试插件的使用
        //private void button15_Click(object sender, EventArgs e)
        //{
        //    this.txtTestPluginsLog.Text = "启动测试插件，请等待插件的调用结果......" + "\r\n";

        //    #region 测试登录插件
        //    if (this.IsPluginsCookie.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();
        //            if (this.txtPluginsCookie.Text.Trim() == "")
        //            {
        //                MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsCookie.Focus();
        //                return;
        //            }

        //            if (this.listWeblink.Items.Count ==0)
        //            {
        //                MessageBox.Show("请输入采集网址后，再进行登录插件的测试工作！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //            cUrlAnalyze u = new cUrlAnalyze(Program.getPrjPath());
        //            List<string> urls= u.SplitWebUrl(this.listWeblink.Items[0].Text);
        //            if (urls.Count  == 0)
        //            {
        //                MessageBox.Show("配置的采集网址有错误，无法测试，请先进行修改！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //            string loginUrl = urls[0];
        //            string cookie = rPlugin.CallGetCookie(loginUrl , this.txtPluginsCookie.Text);
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "通过插件返回的Cookie：" + cookie + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "通过插件获取Cookie失败，错误信息：" + ex.Message  + "\r\n";
        //        }
        //    }
        //    #endregion

        //    #region 数据处理插件测试
        //    if (this.IsPluginsDealData.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();

        //            if (this.txtPluginsDeal.Text.Trim() == "")
        //            {
        //                MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsDeal.Focus();
        //                return;
        //            }

        //            DataTable tmp = (DataTable)this.dTestData.DataSource;
        //            DataTable d = rPlugin.CallDealData(tmp, this.txtPluginsDeal.Text);
        //            this.dTestData.DataSource = d;
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "请通过“测试数据”页查看数据加工的结果" + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "通过插件对采集数据进行编辑失败，错误信息：" + ex.Message + "\r\n";
        //        }
        //    }
        //    #endregion

        //    #region 数据发布插件测试
        //    if (this.raPublishByPlugin.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();

        //            if (this.txtPluginsPublish.Text.Trim() == "")
        //            {
        //                MessageBox.Show("请输入插件文件地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsPublish.Focus();
        //                return;
        //            }

        //            DataTable tmp = (DataTable)this.dTestData.DataSource;
        //            rPlugin.CallPublishData(tmp, this.txtPluginsPublish.Text);
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "通过插件发布数据成功，请根据插件发布规则对发布的数据进行验证！" + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "通过插件发布数据失败，错误信息：" + ex.Message + "\r\n";
        //        }
        //    }
        //    #endregion

        //    this.txtTestPluginsLog.Text += "测试插件完成！";
        //}
        private void IsPluginsDealData_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (IsPluginsDealData.Checked == true)
                {
                    this.txtPluginsDeal.Enabled = true;
                    this.cmdBrowserPlugins2.Enabled = true;
                    this.cmdSetPlugins2.Enabled = true;
                }
                else
                {
                    this.txtPluginsDeal.Enabled = false;
                    this.cmdBrowserPlugins2.Enabled = false;
                    this.cmdSetPlugins2.Enabled = false ;
                }
                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持插件功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsPluginsDealData.Checked = false;
                return;
            }
        }

        private void IsPluginsPublishData_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (this.raPublishByPlugin .Checked == true)
                {
                    this.groupBox10.Enabled = true;
                }
                else
                {
                    this.groupBox10.Enabled = false;
                }
                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持插件功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.raPublishByPlugin.Checked = false;
                return;
            }
        }

        private void SetHeaderState(bool isEnabled)
        {
            this.txtHeader.Enabled = isEnabled;
            this.cmdSetHeader.Enabled = isEnabled;
            this.cmdClearHeader.Enabled = isEnabled;
        }

        private void cmdSetPlugins1_Click(object sender, EventArgs e)
        {
            //if (this.txtPluginsCookie.Text.Trim() == "")
            //{
            //    MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    cmdBrowserPlugins1.Focus();
            //    return;
            //}

            //NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();
            //rPlugin.CallSetConfig(this.txtPluginsCookie.Text);
            //rPlugin = null;
        }

        private void cmdSetPlugins2_Click(object sender, EventArgs e)
        {
            if (this.txtPluginsDeal.Text.Trim() == "")
            {
                MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmdBrowserPlugins2.Focus();
                return;
            }


            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtPluginsDeal.Text);
            rPlugin = null;
        }

        private void cmdSetPlugins3_Click(object sender, EventArgs e)
        {
            if (this.txtPluginsPublish.Text.Trim() == "")
            {
                MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmdBrowserPlugins3.Focus();
                return;
            }

            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtPluginsPublish.Text);
            rPlugin = null;
        }

        private void rmenuEditUrl_Click(object sender, EventArgs e)
        {
            EditUrlRule();
        }

        private void rmenuAddUrl_Click(object sender, EventArgs e)
        {
            AddUrl();
        }

        private void rmenuDelUrl_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count == 0)
                return;

            //如果有导航需要删除导航的规则
            //删除存储的导航规则

            RemoveUrl();
        }

        private void cmdInsertFileDate_Click(object sender, EventArgs e)
        {
            int startPos = this.txtFileName.SelectionStart;
            int l = this.txtFileName.SelectionLength;

            string paraDate = "{FileCurrentDateTime}";
            this.txtFileName.Text = this.txtFileName.Text.Substring(0, startPos) + paraDate
                + this.txtFileName.Text.Substring(startPos + l, this.txtFileName.Text.Length - startPos - l);

            this.txtFileName.SelectionStart = startPos + paraDate.Length;
            this.txtFileName.ScrollToCaret();
        }

        /// <summary>
        /// 设置发布数据界面显示样式
        /// </summary>
        private void SetPublishTemplate()
        {
            
            if (this.IsPublishData.Checked == true)
            {
                this.groupTemplate.Enabled = true;

                if (this.raPublishFile.Checked == true)
                {
                    this.groupTemplate.Visible = false ;
                    this.groupBox9.Visible = true ;
                    this.groupBox4.Visible = true ;
                    this.groupBox4.Enabled = true;
                    this.groupBox10.Visible = true;

                    this.groupBox9.Enabled = false;
                    

                    this.groupBox10.Enabled = false;
                }
                //else if (this.raPublishDB.Checked == true)
                //{
                //    this.groupTemplate.Visible = false ;
                //    this.groupBox9.Visible = true ;
                //    this.groupBox4.Visible = true ;
                //    this.groupBox4.Enabled = false ;
                //    this.groupBox9.Enabled = true ;
                //    this.groupBox10.Visible = true;

                //    this.groupBox10.Enabled = false;
                //}
                else if (this.raPublishByTemplate.Checked == true)
                {
                    this.groupTemplate.Visible = true;
                    this.groupBox9.Visible = false;
                    this.groupBox4.Visible = false;
                    this.groupBox10.Visible = false ;
                   
                }
                else if (this.raInsertDB.Checked == true)
                {
                    this.groupTemplate.Visible = false ;
                    this.groupBox9.Visible = true ;
                    this.groupBox4.Visible = true ;
                    this.groupBox10.Visible = true;

                    this.groupBox10.Enabled = false ;
                    this.groupBox4.Enabled = false ;
                    this.groupBox9.Enabled = true ;
                }
                else if (this.raPublishByPlugin.Checked == true)
                {
                    this.groupTemplate.Visible = false;
                    this.groupBox9.Visible = true;
                    this.groupBox4.Visible = true;
                    this.groupBox10.Visible = true;

                    this.groupBox10.Enabled = true;
                    this.groupBox4.Enabled = false;
                    this.groupBox9.Enabled = false ;
                }

                this.raInsertDB.Enabled = true ;
                this.raPublishFile.Enabled = true;
                this.raPublishByTemplate.Enabled = true;
                this.label16.Enabled = true;
                this.udPublishThread.Enabled = true ;
                this.IsDelTempData.Enabled = true;
                this.button6.Enabled = true;
                this.raPublishByPlugin.Enabled = true;
            }
            else
            {
                this.raInsertDB.Enabled = false ;
                this.raPublishFile.Enabled = false ;
                this.raPublishByTemplate.Enabled = false ;
                this.label16.Enabled = false ;
                this.udPublishThread.Enabled = false ;
                this.IsDelTempData.Enabled = false ;
                this.groupTemplate.Enabled = false;
                this.button6.Enabled = false;
                this.raPublishByPlugin.Enabled = false;
                this.groupBox4.Enabled = false;
                this.groupBox9.Enabled = false;
                this.raPublishByPlugin.Enabled = false;
                this.groupBox10.Enabled = false;
            }

            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                this.raInsertDB.Enabled = false;
            }

        }

        private void comTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strType = this.comTemplate.Text;
            if (strType == "")
            {
                this.PDataColumn.Items.Clear();
                this.dataParas.Rows.Clear();
                return;
            }

            strType = strType.Substring(strType.IndexOf("[") + 1, strType.IndexOf("]") - strType.IndexOf("[") - 1);
            string tName = this.comTemplate.Text.Substring(0, this.comTemplate.Text.IndexOf("["));

            this.PDataColumn.Items.Clear();
            for (int i = 0; i < this.listWebGetFlag.Items.Count; i++)
            {
                if (this.listWebGetFlag.Items[i].Name == "")
                    this.PDataColumn.Items.Add(this.listWebGetFlag.Items[i].Text );
            }

            this.PDataColumn.Items.Add("CollectionUrl");
            this.PDataColumn.Items.Add("CollectionDateTime");
            this.PDataColumn.Items.Add("DownloadFile");
            this.PDataColumn.Items.Add("{手工输入}");


            this.dataParas.Rows.Clear();

            if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(strType) == cGlobalParas.PublishTemplateType.Web)
            {
                this.groupBox14.Enabled = true;
                this.groupBox13.Enabled = false;

                //加载参数
                cTemplate t = new cTemplate(Program.getPrjPath ());
                t.LoadTemplate(tName);

                this.dataParas.Rows.Add();
                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = "系统分类";

                foreach (KeyValuePair<string, string> de in t.PublishParas)
                {
                    string ss = de.Value.ToString();
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={发布参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        this.dataParas.Rows.Add();
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                    }
                }

                foreach (KeyValuePair<string, string> de in t.UploadParas)
                {
                    string ss = de.Value.ToString();
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={上传参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        if (ma.ToString() != "当前上传的文件名")
                        {
                            this.dataParas.Rows.Add();
                            this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                        }
                    }
                }
            }
            else if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(strType) == cGlobalParas.PublishTemplateType.DB)
            {
                this.groupBox14.Enabled = false;
                this.groupBox13.Enabled = true;
                cDbTemplate t = new cDbTemplate(Program.getPrjPath());
                t.LoadTemplate(tName);

                this.txtTDbConn.Tag = ((int)t.DbType).ToString();

                //分解sql语句中的参数
                for (int i = 0; i < t.sqlParas.Count; i++)
                {
                    string ss = t.sqlParas[i].Sql;
                    ss = ss.Replace("：", ":");
                    Regex re = new Regex("(?<={发布参数:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        this.dataParas.Rows.Add();
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = ma.Value.ToString();
                    }
                }
                t = null;

            }

            this.IsSave.Text = "true";
        }

        private void button18_Click(object sender, EventArgs e)
        {
            frmSetData fSD = new frmSetData();

            if (this.txtTDbConn.Tag == null)
            {
                MessageBox.Show("请先选择需要使用的发布模版 !", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            cGlobalParas.DatabaseType dType = (cGlobalParas.DatabaseType)int.Parse(this.txtTDbConn.Tag.ToString());


            if (dType == cGlobalParas.DatabaseType.Access)
                fSD.FormState = 0;
            else if (dType == cGlobalParas.DatabaseType.MSSqlServer)
                fSD.FormState = 1;
            else if (dType == cGlobalParas.DatabaseType.MySql)
                fSD.FormState = 2;

            fSD.rDataSource = new frmSetData.ReturnDataSource(GetTemplateConn);
            fSD.ShowDialog();
            fSD.Dispose();
        }

        private void GetTemplateConn(string strDataConn)
        {
            this.txtTDbConn.Text = strDataConn;
        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                TestGatherRule();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataParas_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("您选择的发布规则与当前配置的采集任务不匹配，请重新选择发布规则，此错误消息有可能还会出现！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void txtUser_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPwd_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtDomain_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtTDbConn_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void dataParas_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void listWeblink_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void listWeblink_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count > 0)
            {
                this.cmdEditWeblink.Enabled = true;
                this.cmdDelWeblink.Enabled = true;
            }
            else
            {
                this.cmdEditWeblink.Enabled = false;
                this.cmdDelWeblink.Enabled = false;
            }
        }

        private void IsSqlTrue_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

    

        //private void SetTriggerEnabled(bool isEnabled)
        //{
        //    if (Program.SominerVersion == cGlobalParas.VersionType.Union ||
        //        Program.SominerVersion == cGlobalParas.VersionType.Professional ||
        //        Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
        //        Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
        //    {
        //        //if (isEnabled == true)
        //        //    this.tabPage1.Parent = this.tabControl1;
        //        //else
        //        //    this.tabPage1.Parent = null;

        //            this.raGatheredRun.Enabled = isEnabled;
        //            this.raPublishedRun.Enabled = isEnabled;
        //            this.cmdAddTask.Enabled = isEnabled;
        //            this.cmdDelTask.Enabled = isEnabled;
        //            this.listTask.Enabled = isEnabled;
        //            this.IsSave.Text = "true";
        //    }
        //    else
        //    {
        //        MessageBox.Show("当前版本不支持触发器，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        this.IsStartTrigger.Checked = false;
        //        return;
        //    }
        //}

       

        private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
        {
            
        }

        private int ImportUrl(string fName)
        {
            StreamReader fileReader = null;

            try
            {
                fileReader = new StreamReader(fName, System.Text.Encoding.GetEncoding("gb2312"));
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("文件打开失败，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return 0;
            }
            string str = fileReader.ReadToEnd();
            string[] Urls = Regex.Split(str, "\r\n");

            ListViewItem[] items = new ListViewItem[Urls.Length];
            int i = 0;

            for (i = 0; i < Urls.Length; i++)
            {
                items[i] = new ListViewItem();
                items[i].Text = Urls[i];
                items[i].SubItems.Add("N");
                items[i].SubItems.Add("0");
                items[i].SubItems.Add("");
                items[i].SubItems.Add("");
                items[i].SubItems.Add("");

                //计算网址数量
                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                int UrlCount = cu.GetUrlCount(Urls[i]);
                cu = null;

                items[i].SubItems.Add(UrlCount.ToString());
                items[i].SubItems.Add("N");
                items[i].SubItems.Add("N");
            }

            this.listWeblink.Items.AddRange(items);

            fileReader.Close();
            fileReader = null;
            
            Urls = null;

            return i+1;
        }

        private void label44_Click(object sender, EventArgs e)
        {

        }

        private void IsNoDataStop_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void IsNoInsertStop_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void IsRepeatStop_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void nNoDataStopCount_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void nNoInsertStopCount_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comNoDataStopRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void comRepeatStopRule_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void treeTestUrl_DoubleClick(object sender, EventArgs e)
        {
            if (this.treeTestUrl.SelectedNode != null)
            {
                string url = this.treeTestUrl.SelectedNode.Text;
                if (url.IndexOf ("[")>0)
                    url = url.Substring(0, url.IndexOf("["));
                if (url.ToLower().StartsWith("http://") || url.ToLower().StartsWith ("https://"))
                    System.Diagnostics.Process.Start(url);
                else
                    MessageBox.Show("不是标准的Url地址！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

        private void raExportWord_CheckedChanged(object sender, EventArgs e)
        {
            if (raExportWord.Checked == true)
            {
                this.raExportCSV.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportAccess.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;

                SetExportFile();

                if (this.txtFileName.Text.Trim() != "" && this.txtFileName.Text.Length > 6 && this.txtFileName.Text.LastIndexOf(".")>0)
                {
                    this.txtFileName.Text = this.txtFileName.Text.Substring(0, this.txtFileName.Text.LastIndexOf(".")) + ".docx";
                }

                this.IsSave.Text = "true";
            }
        }

        private void IsRowFile_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSucceedUrl_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        #region 处理网址测试的树形结构和日志的界面显示
        public void AddNode(TreeNodeCollection tNodeCol, string sUrl)
        {
            TreeNode tNode= tNodeCol.Add(sUrl, sUrl);
            if (tNode.Parent!=null)
                tNode.Parent.ExpandAll();
            Application.DoEvents();
            
            //if (tNode == null)

            //    this.treeTestUrl.Nodes.Add(sUrl, sUrl);
            //else
            //    tNode.Nodes.Add(sUrl, sUrl);
        }

        public void AddUrlLog(string sLog)
        {
            this.txtTestLog.AppendText(sLog + "\r\n");
        }

        public void UpdateNodeText(TreeNode tNode,string str)
        {
            tNode.Text = str;
            Application.DoEvents();
        }
        #endregion

        private void txtStartPos_TextChanged_1(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtEndPos_TextChanged_1(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsPublishData_Click(object sender, EventArgs e)
        {

            if (this.IsPublishData.Checked == true)
            {
               
            }
            else
            {
                this.raInsertDB.Enabled = false;
            }

            SetPublishTemplate();

            this.IsSave.Text = "true";
        }

        private void IsNoDataStop_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {

                if (IsNoDataStop.Checked == true)
                {
                    this.nNoDataStopCount.Enabled = true;
                    this.comNoDataStopRule.Enabled = true;
                    this.comNoDataStopRule.SelectedIndex = 0;
                }
                else if (IsNoDataStop.Checked == false)
                {
                    this.nNoDataStopCount.Enabled = false;
                    this.comNoDataStopRule.Enabled = false;
                    this.comNoDataStopRule.SelectedIndex = -1;
                }
                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持采集任务执行设置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsNoDataStop.Checked = false;
                return;
            }
        }

        private void IsNoInsertStop_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise )
            {
                if (IsNoInsertStop.Checked == true)
                    this.nNoInsertStopCount.Enabled = true;
                else if (IsNoInsertStop.Checked == false)
                    this.nNoInsertStopCount.Enabled = false;

                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持采集任务执行设置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsNoInsertStop.Checked = false;
                return;
            }
        }

        private void IsRepeatStop_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (IsRepeatStop.Checked == true)
                {
                    this.comRepeatStopRule.Enabled = true;
                    this.comRepeatStopRule.SelectedIndex = 0;
                }
                else if (IsRepeatStop.Checked == false)
                {
                    this.comRepeatStopRule.Enabled = false;
                    this.comRepeatStopRule.SelectedIndex = -1;
                }

                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持采集任务执行设置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.IsRepeatStop.Checked = false;
                return;
            }
        }

        private void IsIgnoreErr_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsAutoUpdateHeader_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        //private void cmdUpDealRule_Click(object sender, EventArgs e)
        //{
        //    if (this.dataDataRules.SelectedCells.Count == 0 || this.dataDataRules.Rows.Count == 1)
        //        return;

        //    int i = this.dataDataRules.SelectedCells[0].RowIndex;
        //    if (i == 0)
        //        return;

        //    if (this.dataDataRules.Rows[i].Cells[1].Value == null)
        //        return;

        //    string[] r = new string[3];
        //    r[0] = (i + 1).ToString();
        //    r[1] = this.dataDataRules.Rows[i].Cells[1].Value.ToString();
        //    r[2] = this.dataDataRules.Rows[i].Cells[2].Value.ToString();
        //    this.dataDataRules.Rows.Remove(this.dataDataRules.Rows[i]);

        //    this.dataDataRules.Rows.Insert(i - 1);
        //    this.dataDataRules.Rows[i - 1].Cells[1].Value = r[1];
        //    this.dataDataRules.Rows[i - 1].Cells[2].Value = r[2];

        //    for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
        //    {
        //        this.dataDataRules.Rows[j].Selected = false;
        //    }
        //    this.dataDataRules.Rows[i - 1].Selected = true;
            
        //    this.dataDataRules.FirstDisplayedScrollingRowIndex = i - 1;

        //    SaveExportRule();
        //    this.IsSave.Text = "true";
        //}

        //private void cmdDownDealRule_Click(object sender, EventArgs e)
        //{
        //    if (this.dataDataRules.SelectedCells.Count == 0 || this.dataDataRules.Rows.Count ==1)
        //        return;
           

        //    int i = this.dataDataRules.SelectedCells[0].RowIndex;
        //    if (this.dataDataRules.Rows[i].Cells[1].Value == null)
        //        return;


        //    if ((i+2) == this.dataDataRules.Rows.Count)
        //        return;
        //    string[] r = new string[3];
        //    r[0] = (i + 1).ToString();
        //    r[1] = this.dataDataRules.Rows[i].Cells[1].Value.ToString ();
        //    r[2] = this.dataDataRules.Rows[i].Cells[2].Value.ToString ();
        //    this.dataDataRules.Rows.Remove(this.dataDataRules.Rows[i]);

        //    this.dataDataRules.Rows.Insert(i + 1);
        //    this.dataDataRules.Rows[i + 1].Cells[1].Value  = r[1];
        //    this.dataDataRules.Rows[i + 1].Cells[2].Value  = r[2];

        //    for (int j = 0; j < this.dataDataRules.Rows.Count; j++)
        //    {
        //        this.dataDataRules.Rows[j].Selected = false;
        //    }
        //    this.dataDataRules.Rows[i + 1].Selected = true;

        //    this.dataDataRules.FirstDisplayedScrollingRowIndex = i + 1;

        //    SaveExportRule();
        //    this.IsSave.Text = "true";
        //}

        private void listWebGetFlag_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void isAllowSplit_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud || 
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (this.isNoneAllowSplit.Checked == true)
                    this.IsSplitDbUrls.Enabled = false;
                else
                    this.IsSplitDbUrls.Enabled = true;

                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持采集任务执行设置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.isNoneAllowSplit.Checked = false  ;
                return;
            }
        }

        private void isNoneAllowSplit_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSplitDbUrls_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isTwoUrlCode_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.DistriServer ||
                Program.SominerVersion == cGlobalParas.VersionType.Server )
            {
                this.IsSave.Text = "true";
            }
            else
            {
                MessageBox.Show("当前版本不支持采集任务执行设置，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.isTwoUrlCode.Checked = true;
                return;
            }
            
        }

        private void raPublishByTemplate_CheckedChanged(object sender, EventArgs e)
        {
            SetPublishTemplate();
            this.IsSave.Text = "true";
        }

        private void raPublishFile_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void raPublishFile_Click(object sender, EventArgs e)
        {
            if (raPublishFile.Checked == true)
            {
                //this.comRunType.SelectedIndex = 0;

                this.groupTemplate.Visible = false;
                this.groupBox9.Visible = true;
                this.groupBox4.Visible = true;
                this.groupBox4.Enabled = true;
                this.groupBox9.Enabled = false;
                this.groupBox10.Visible = true;
                this.groupBox10.Enabled = false;

                this.raExportTxt.Checked = true;

                
            }
            this.IsSave.Text = "true";
        }

        //private void raPublishDB_Click(object sender, EventArgs e)
        //{
        //     if (this.raPublishDB.Checked == true)
        //        {
        //            this.comRunType.SelectedIndex = 0;

        //            this.groupTemplate.Visible = false ;
        //            this.groupBox9.Visible = true ;
        //            this.groupBox4.Visible = true ;
        //            this.groupBox4.Enabled = false ;
        //            this.groupBox9.Enabled = true ;
        //            this.groupBox10.Visible = true;
        //            this.groupBox10.Enabled = false;

        //            this.raExportAccess.Checked = true;

                    
        //        }
        //     this.IsSave.Text = "true";
              
        //}

        private void raPublishByTemplate_Click(object sender, EventArgs e)
        {
              if (this.raPublishByTemplate.Checked == true)
                {
                    //this.comRunType.SelectedIndex = 0;
                    this.groupTemplate.Visible = true;
                    this.groupBox9.Visible = false;
                    this.groupBox4.Visible = false;
                    this.groupBox10.Visible = false;
                    
                }
              this.IsSave.Text = "true";
              
        }

        private void raInsertDB_Click(object sender, EventArgs e)
        {
            if (this.raInsertDB.Checked == true)
                {
                    //this.comRunType.SelectedIndex = 2;

                    this.groupTemplate.Visible = false ;
                    this.groupBox9.Visible = true ;
                    this.groupBox4.Visible = true ;
                    this.groupBox4.Enabled = false ;
                    this.groupBox9.Enabled = true ;
                    this.groupBox10.Visible = true;
                    this.groupBox10.Enabled = false;

                    this.raExportMySql.Checked = true;

                    
                }
              this.IsSave.Text = "true";
        }

        private void raPublishByPlugin_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                MessageBox.Show("当前版本不支持直接入库，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.raInsertDB.Checked = false;
                return;
            }

            if (this.raPublishByPlugin.Checked == true)
            {
                //this.comRunType.SelectedIndex = 0;

                this.groupTemplate.Visible = false;
                this.groupBox9.Visible = true;
                this.groupBox4.Visible = true;
                this.groupBox10.Visible = true;
                this.groupBox4.Enabled = false;

                this.groupBox9.Enabled = false ;
                this.groupBox10.Enabled = true;

                this.raExportAccess.Checked = false;
                this.raExportCSV.Checked = false;
                this.raExportExcel.Checked = false;
                this.raExportMSSQL.Checked = false;
                this.raExportMySql.Checked = false;
                this.raExportOracle.Checked = false;
                this.raExportTxt.Checked = false;
                this.raExportWord.Checked = false;


                
            }
            this.IsSave.Text = "true";
        }

        private void on_ImageClick(object sender, SoukeyControl.CustomControl.cImageClickEvent e)
        {
            int index = e.Index;
            if (this.listWebGetFlag.Items[index].ImageIndex == 17)
            {
                //已经展开，需要折叠，折叠就是删除以下的所有内容
                DelDealRule(this.listWebGetFlag.Items[index].Text, index);
               
                this.listWebGetFlag.Items[index].ImageIndex = 18;
            }
            else if (this.listWebGetFlag.Items[index].ImageIndex == 18)
            {
                string gName = this.listWebGetFlag.Items[index].Text;

                //已经折叠，需要展开

                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (gName == m_gRules[i].gName)
                    {
                        AddDealRule(gName, index, m_gRules[i]);
                        break;
                    }
                }

                this.listWebGetFlag.Items[index].ImageIndex = 17;
            }

        }

        private void DelDealRule(string gName,int index)
        {
            string delName = gName;
            delName += "!Deal!-";
            List<string> delKey = new List<string>();
            for (int i = index; i < this.listWebGetFlag.Items.Count; i++)
            {
                string dName = this.listWebGetFlag.Items[i].Name;
                if (dName.StartsWith(delName))
                {
                    delKey.Add(dName);
                }
            }

            //开始删除
            for (int i = 0; i < delKey.Count; i++)
            {
                this.listWebGetFlag.Items.RemoveByKey(delKey[i]);
            }
        }

        private void AddDealRule(string gName, int index,cGatherRule gRule)
        {
            for (int j = 0; j < gRule.fieldDealRules.FieldRule.Count; j++)
            {
                ListViewItem item = new ListViewItem();
                item.Name = gName + "!Deal!-" + j.ToString();
                item.Text = "  " + gRule.fieldDealRules.FieldRule[j].FieldRuleType.GetDescription();
                item.SubItems.Add(gRule.fieldDealRules.FieldRule[j].FieldRule);
                item.ImageIndex = -1;
                item.ForeColor = Color.Gray;
                this.listWebGetFlag.Items.Insert(index + j + 1, item);
            }
        }

        private int GetRuleIndex(int index)
        {
            //向上寻找所归属的采集数据规则的序号
            if (this.listWebGetFlag.Items[index].Name == "")
                return index;
            else
            {
                for (int i=index;i>0;i--)
                {
                    if(this.listWebGetFlag.Items[i].Name=="")
                        return this.listWebGetFlag.Items[i].Index;
                }
            }

            return 0;
        }

        private void rmenuDelRule_Click(object sender, EventArgs e)
        {
            DelGrule();
        }

      

        private void Tab1_Click(object sender, EventArgs e)
        {
            if (Tab1.Checked == true)
            {
                this.Tab2.Checked = false;
                this.Tab3.Checked = false;
                this.Tab4.Checked = false;
                this.Tab5.Checked = false;
                this.TabTest.Checked = false;

                this.panel1.Visible = true;
                this.panel2.Visible = false ;
                this.panel3.Visible = false;
                this.panel4.Visible = false;
                this.panelTest.Visible = false;
                this.panel5.Visible = false;
            }
        }

        private void Tab2_Click(object sender, EventArgs e)
        {
            if (Tab2.Checked == true)
            {
                this.Tab1.Checked = false;
                this.Tab3.Checked = false;
                this.Tab4.Checked = false;
                this.Tab5.Checked = false;
                this.TabTest.Checked = false;

                this.panel1.Visible = false ;
                this.panel2.Visible = true ;
                this.panel3.Visible = false;
                this.panel4.Visible = false;
                this.panelTest.Visible = false;
                this.panel5.Visible = false;
            }
        }

        private void Tab3_Click(object sender, EventArgs e)
        {
            if (Tab3.Checked == true)
            {
                this.Tab1.Checked = false;
                this.Tab2.Checked = false;
                this.Tab4.Checked = false;
                this.Tab5.Checked = false;
                this.TabTest.Checked = false;

                this.panel1.Visible = false;
                this.panel2.Visible = false ;
                this.panel3.Visible = true;
                this.panel4.Visible = false;
                this.panelTest.Visible = false;
                this.panel5.Visible = false;
            }
        }

        private void Tab4_Click(object sender, EventArgs e)
        {
            if (Tab4.Checked == true)
            {
                this.Tab1.Checked = false;
                this.Tab2.Checked = false;
                this.Tab3.Checked = false;
                this.Tab5.Checked = false;
                this.TabTest.Checked = false;

                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = false ;
                this.panel4.Visible = true;
                this.panelTest.Visible = false;
                this.panel5.Visible = false;
            }
        }

        private void TabTest_Click(object sender, EventArgs e)
        {
            if (TabTest.Checked == true)
            {
                this.Tab1.Checked = false;
                this.Tab2.Checked = false;
                this.Tab3.Checked = false;
                this.Tab4.Checked = false;
                this.Tab5.Checked = false;

                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = false;
                this.panel4.Visible = false ;
                this.panelTest.Visible = true;
                this.panel5.Visible = false;
            }
        }

        private void udGIntervalTime1_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ClearUrlsDB();
        }

        private void isCookieList_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void udGatherCount_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void udGatherCountPauseInterval_ValueChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtStopFlag_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsPluginsCookie_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void isCookieList_Click(object sender, EventArgs e)
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                if (this.isCookieList.Checked == true)
                {
                 

                    this.txtCookie.WordWrap = false;
                }
                else
                {
                    if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                        Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
                    Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
                    {
                   
                        this.txtCookie.WordWrap = true;
                    }
                }
                this.IsSave.Text = "true";
             }
            else
            {
                MessageBox.Show("当前版本不支持插件功能，请获取正确的版本！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.isCookieList.Checked = false;
                return;
            }
        }

        private void isThrowGatherErr_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmAddThreadProxy f = new frmAddThreadProxy((int)this.udThread.Value,this.m_ThreadProxy);
            f.rThread = this.GetThreadProxy;
            f.ShowDialog();
            f.Dispose();

            this.IsSave.Text = "true";
        }

        private void GetThreadProxy(List<eThreadProxy> threadProxy)
        {
            this.m_ThreadProxy = threadProxy;
        }

        private void isMultiInterval_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void IsSplitDbUrls_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isSameSubTask_Click(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isSetServerDB_CheckedChanged(object sender, EventArgs e)
        {
            if (isSetServerDB.Checked == true)
            {
                this.txtDataSource.Enabled = false;
                this.button12.Enabled = false;
            }
            else
            {
                this.txtDataSource.Enabled = true;
                this.button12.Enabled = true;
            }
            this.IsSave.Text = "true";
        }

        private void isGatherCoding_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isGatherCodingByPB.Checked == true)
            {
                //this.txtCodingFlag.Enabled = true;
                this.txtPluginsCodingByPB.Enabled = true;
                this.cmdBrowserPlugins4.Enabled = true;
                this.cmdSetPlugins4.Enabled = true;
                this.txtCodingUrlByPB.Enabled = true;
            }
            else
            {
                //this.txtCodingFlag.Enabled = false;
                this.txtPluginsCodingByPB.Enabled = false;
                this.cmdBrowserPlugins4.Enabled = false;
                this.cmdSetPlugins4.Enabled = false;
                this.txtCodingUrlByPB.Enabled = false;
            }
            this.IsSave.Text = "true";
        }

        private void txtCodingFlag_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtCodingUrl_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void txtPluginsCoding_TextChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cmdBrowserPlugins4_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择插件文件";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "插件(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPluginsCodingByPB.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdSetPlugins4_Click(object sender, EventArgs e)
        {
            if (this.txtPluginsCodingByPB.Text.Trim() == "")
            {
                MessageBox.Show("请先选择插件，然后再进行插件设置，选择插件请点击浏览按钮，选择插件dll文件！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmdSetPlugins4.Focus();
                return;
            }


            NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
            rPlugin.CallSetConfig(this.txtPluginsCodingByPB.Text);
            rPlugin = null;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (this.treeTClass.Visible == true)
                this.treeTClass.Visible = false;
            else
                this.treeTClass.Visible = true;
        }

        private void treeTClass_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.comTaskClass.Text = this.treeTClass.SelectedNode.Text;
            this.comTaskClass.Tag = this.treeTClass.SelectedNode.Tag.ToString();
            this.treeTClass.Visible = false;
            this.IsSave.Text = "true";
        }

        private string GetTaskFullClassName(TreeNode eNode)
        {
            string cName = string.Empty;

            if (eNode.Parent==null)
            {
                return eNode.Text;
            }
            else
            {
                cName = GetTaskFullClassName(eNode.Parent) + "/" + eNode.Text;
            }
            return cName;
        }

        private void groupBox9_Enter(object sender, EventArgs e)
        {

        }

        private void raInputCookie_Click(object sender, EventArgs e)
        {
            if (raInputCookie.Checked==true)
            {
                this.raPageCookie.Checked = false;
                this.raLoginCookie.Checked = false;

                this.panelInputCookie.Visible = true;
                this.panelLoginCookie.Visible = false;
                this.panelPageCookie.Visible = false;
            }
        }

        private void raLoginCookie_Click(object sender, EventArgs e)
        {
            if (raLoginCookie.Checked == true)
            {
                this.raPageCookie.Checked = false;
                this.raInputCookie.Checked = false;

                this.panelInputCookie.Visible = false ;
                this.panelLoginCookie.Visible = true ;
                this.panelPageCookie.Visible = false;
            }
        }

        private void raPageCookie_Click(object sender, EventArgs e)
        {
            if (raPageCookie.Checked == true)
            {
                this.raLoginCookie.Checked = false;
                this.raInputCookie.Checked = false;

                this.panelInputCookie.Visible = false;
                this.panelLoginCookie.Visible = false ;
                this.panelPageCookie.Visible = true ;
            }
        }

        private void Tab5_Click(object sender, EventArgs e)
        {
            if (Tab5.Checked == true)
            {
                this.Tab1.Checked = false;
                this.Tab2.Checked = false;
                this.Tab3.Checked = false;
                this.Tab4.Checked = false;
                this.TabTest.Checked = false;

                this.panel1.Visible = false;
                this.panel2.Visible = false;
                this.panel3.Visible = false;
                this.panel4.Visible = false;
                this.panelTest.Visible = false;
                this.panel5.Visible = true ;
            }
        }

        private void isGatherCodingByPB_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isGatherCodingByPB.Checked==true )
            {
                this.txtCodingUrlByPB.Enabled = true;
                this.txtPluginsCodingByPB.Enabled = true;
                this.cmdBrowserPlugins4.Enabled = true;
                this.cmdSetPlugins4.Enabled = true;
            }
            else
            {
                this.txtCodingUrlByPB.Enabled = false;
                this.txtPluginsCodingByPB.Enabled = false;
                this.cmdBrowserPlugins4.Enabled = false;
                this.cmdSetPlugins4.Enabled = false;
            }

            this.IsSave.Text = "true";
        }

        private void isNoneByPB_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cbStopGatherByPB_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void cbUpdateCookieByPB_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void isThrowGatherErrByPB_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raInputCookie_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raLoginCookie_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void raPageCookie_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }

        private void groupBox11_Enter(object sender, EventArgs e)
        {

        }

        private delegate object delegateGetValue(object ClassInstance, string PropertyName);
        private object GetValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="referUrl"></param>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="isVisual">是否可视化提取</param>
        /// <returns></returns>
        private string GetHtml(string Url, string referUrl,string startPos,string endPos,bool isVisual)
        {
            //需要判断是否为可视化提取的网页



            #region HTTPHeader设置


            List<eHeader> headers = new List<eHeader>();
            eHeader header;

            string strheader = this.txtHeader.Text;

            if (strheader != "")
            {
                foreach (string sc in strheader.Split('\r'))
                {
                    header = new eHeader();

                    string ss = sc.Trim();
                    if (ss.StartsWith("["))
                    {
                        string s1 = ss.Substring(0, ss.IndexOf("]") + 1);
                        header.Range = s1;
                        ss = ss.Replace(s1, "");
                    }
                    header.Label = ss.Substring(0, ss.IndexOf(":"));
                    header.LabelValue = ss.Substring(ss.IndexOf(":") + 1, ss.Length - ss.IndexOf(":") - 1);

                    headers.Add(header);
                }
            }
            #endregion

            object[] paras = new object[] { (this.txtCookie), "Text" };
            string oldCookie = this.Invoke(new delegateGetValue(this.GetValue), paras).ToString();
            object[] paras1 = new object[] { this.comWebCode, "SelectedItem" };
            string webCode= this.Invoke(new delegateGetValue(this.GetValue), paras1).ToString();
            object[] paras2 = new object[] { this.comUrlEncode, "SelectedItem" };
            string urlCode = this.Invoke(new delegateGetValue(this.GetValue), paras2).ToString();

            //在此控制代理的问题
            eRequest request = NetMiner.Core.Url.UrlPack.GetRequest(Url, oldCookie, 
                EnumUtil.GetEnumName<cGlobalParas.WebCode>(webCode),
                this.IsUrlEncode.Checked,this.isTwoUrlCode.Checked,
                EnumUtil.GetEnumName<cGlobalParas.WebCode>(urlCode),
                "",headers, referUrl, this.IsUrlAutoRedirect.Checked);

           
            
            WebProxy wProxy = null;
           
            if (this.m_ThreadProxy.Count >0)
            {
                if (this.m_ThreadProxy[0].pType==cGlobalParas.ProxyType.HttpProxy)
                {
                    request.ProxyType = cGlobalParas.ProxyType.HttpProxy;
                    wProxy = new WebProxy(this.m_ThreadProxy[0].Address, this.m_ThreadProxy[0].Port);
                    wProxy.BypassProxyOnLocal = true;
                    request.webProxy = wProxy;
                }
                else if (this.m_ThreadProxy[0].pType == cGlobalParas.ProxyType.Socket5)
                {
                    request.ProxyType = cGlobalParas.ProxyType.Socket5;
                    request.socket5Server = this.m_ThreadProxy[0].Address;
                    request.socket5Port = this.m_ThreadProxy[0].Port;
                }
            }
            else
            {
                if (this.IsProxy.Checked)
                {
                    
                    cProxyControl PControl = new cProxyControl(Program.getPrjPath());
                    eProxy proxy = PControl.GetFirstProxy();
                    wProxy = new WebProxy(proxy.ProxyServer, int.Parse(proxy.ProxyPort));
                    wProxy.BypassProxyOnLocal = true;
                    if (proxy.User != "")
                    {
                        wProxy.Credentials = new NetworkCredential(proxy.User, proxy.Password);
                    }
                    request.ProxyType = cGlobalParas.ProxyType.TaskConfig;
                    request.webProxy = wProxy;
                }
                else
                {
                    request.ProxyType = cGlobalParas.ProxyType.SystemProxy;
                }
            }

          

            eResponse response = NetMiner.Net.Unity.RequestUri(Program.getPrjPath(), request, isVisual);

            string newCookie = ToolUtil.MergerCookie(oldCookie, response.cookie);
          
            SetValue((this.txtCookie), "Text", newCookie);
            
            string htmlSource = response.Body;

            //去除\r\n
            htmlSource = Regex.Replace(htmlSource, "([\\r\\n])[\\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            htmlSource = Regex.Replace(htmlSource, "\\n", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            htmlSource.Replace("\\r\\n", "");

            if (!string.IsNullOrEmpty(startPos) && !string.IsNullOrEmpty (endPos))
            {
                string Splitstr = "(" + startPos + ").*?(" + endPos + ")";

                Match aa = Regex.Match(htmlSource, Splitstr);
                htmlSource = aa.Groups[0].ToString();

            }

            return htmlSource;
        }

        private void isCloseTab_CheckedChanged(object sender, EventArgs e)
        {
            this.IsSave.Text = "true";
        }
    }

}