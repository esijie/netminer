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

///���ܣ��ɼ�������Ϣ����  
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶��������ʽ�汾�Ѿ��޸���1.2
///
///�޶���2010-04-15 �����ʽ�汾�޸�����1.8
///�޶���2010-12-1 �����ʽ�汾����Ϊ��2.0
///�޶���2012-1-30 �Ż��û����飬�޸�����ַ�����Ĵ��������˶�ҳ�ɼ��Ĺ��ܡ��ļ�����Ķ������ù���
///�ɼ�����汾����Ϊ��3.0
///

namespace MinerSpider
{

    public partial class frmTask : Form
    {

        #region ����

        private Single m_SupportTaskVersion = Single.Parse(Program.SominerTaskVersionCode);

        //public delegate void RIsShowWizard(bool IsShowWizard);
        //public RIsShowWizard RShowWizard;

        //����һ��������Դ�ļ��ı���
        private ResourceManager rm;

        private string m_RegexNextPage;

        //�����ɴ��������汾�ţ�ע���1.3��ʼ������������ǰ���ݣ��������󷽿ɲ���
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public delegate void ReturnTaskClass(string tClass);
        public ReturnTaskClass rTClass;

        //�Ƿ��ѱ���������������棬������ȡ����ʱ��
        //Ҳ��Ҫ����������������з��أ���Ҫ�����ڡ�Ӧ�á�
        //�͡�ȡ������ť���ж���
        private bool IsSaveTask = false;

        //����һ��ToolTip
        ToolTip HelpTip = new ToolTip();
        //NetMiner.Gather.Task.cUrlAnalyze gUrl = new NetMiner.Gather.Task.cUrlAnalyze(Program.getPrjPath()); 

        //����һ�����������ڴ洢Url��ַ��Ӧ�ĵ�������,��Ϊ��ǰ�����޷���ʾ���е�
        //�����������ԣ���Ҫͨ��һ���������д洢
        private List<eNavigRules> m_listNaviRules = new List<eNavigRules>();

        //����һ�����ϣ����ڴ洢Url��ַ��Ӧ�Ķ�ҳ�ɼ�������Ϊ��ǰ�����޷���ʾ���е�
        //��Ϣ����Ҫͨ��һ����������д洢
        private List<eMultiPageRules> m_MultiPageRules = new List<eMultiPageRules>();

        //����һ�����������ڴ洢�ɼ��ֶεĹ���
        private List<cGatherRule> m_gRules = new List<cGatherRule>();
        //private List<cFieldRules> m_FieldRules = new List<cFieldRules>();

        //����һ�������࣬���ڴ洢ÿ���̵߳Ĵ�����Ϣ
        private List<eThreadProxy> m_ThreadProxy = new List<eThreadProxy>();

        //����һ��ֵ��¼��ǰѡ�񲢱༭���������ֶ�
        private string SelectedFiled = "";

        //����һ���ɼ���������������ڲɼ����Բ������ɼ�����
        //ͳһʹ�òɼ�����������
        //private cGatherControl m_GatherControl;

        //����һ��ֵ���жϵ�ǰ�Ƿ����ڽ��вɼ�����
        private bool m_IsGathering = false;

        /// <summary>
        /// 0-δ���ԣ�1-������ַ����2-���Բɼ�����
        /// </summary>
        //private int m_TestType = 0;

        //����һ��ֵ����ʾ��ǰ�Ƿ����ڽ��е�������
        private bool m_testNaving = false;

        //����һ��ֵ����ʾ��ǰ�༭�������Ƿ�ΪԶ������
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

            //��ʼ���ɼ�������
            //NetMiner.Gather.Gather.cHashTree tmpUrls = null;
            //m_GatherControl = new cGatherControl(Program.getPrjPath(),false,ref tmpUrls);

            ////�ɼ��������¼���,�󶨺�,ҳ�������Ӧ�ɼ����������¼�
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

        #region ��������״̬
        private cGlobalParas.FormState m_FormState;
        public cGlobalParas.FormState FormState
        {
            get { return m_FormState;}
            set { m_FormState = value; }
        }
        #endregion

        //����ToolTip����Ϣ
        private void SetTooltip()
        {
            HelpTip.SetToolTip(this.tTask, @"�����������ƣ��������ƿ�����" + "\r\n" + @"���Ļ�Ӣ�ģ������������.*\/!?|@$%^&,<>{}()[]#+,��" + "\r\n" + "����Ϊ������");
            HelpTip.SetToolTip(this.comTaskClass, @"ѡ��ǰ�ɼ����������ķ���");
            HelpTip.SetToolTip(this.IsUrlEncode, "�����ַ�г����������ַ���ͨ���ǻ���б�����ٽ�����ҳ�����\r\n�ڴ�ָ�������ʽ�������֪���������ͣ���ʹ�������ṩ�Ĺ��߲��Ի�ȡ");
            HelpTip.SetToolTip(this.comUrlEncode, "�����ַ�г����������ַ���ͨ���ǻ���б�����ٽ�����ҳ�����\r\n�ڴ�ָ�������ʽ�������֪���������ͣ���ʹ�������ṩ�Ĺ��߲��Ի�ȡ");
            HelpTip.SetToolTip(this.cmdUrlEncoding, @"����˰�ť�ɴ򿪱�/���빤�ߣ�������Ӧ�Ľ������������" + "\r\n" + "��������֤ʵ�ʵı�����������");
            HelpTip.SetToolTip(this.button10, @"����˰�ť�ɴ򿪻�ȡCookie��С���ߣ�" + "\r\n" + "�ڹ�����ʵ�ʽ���һ�ε�¼������ϵͳ���¼Cookie��ȷ�ϼ���");
            HelpTip.SetToolTip(this.comWebCode, "Ĭ������£�ϵͳ���Զ���ȡ�������ͣ�����ɼ����������������\r\n���ڴ�ѡ����ҳ�����Ծ������������֪��ѡ����ֱ��룬��ͨ��������鿴");
            HelpTip.SetToolTip(this.button2, @"������ַ����������������ַ�϶࣬����ʱ���ϳ���" + "\r\n" + "��Ҳ����ʱ�ж���ַ���Թ���");
            HelpTip.SetToolTip(this.cmdAddWeblink, @"������Ӳɼ���ַ" + "\r\n" + "ÿ���ɼ���������Ҫ����һ����Ч�Ĳɼ���ַ");
            HelpTip.SetToolTip(this.cmdEditWeblink, "����޸ĵ�ǰѡ�еĲɼ���ַ");
            HelpTip.SetToolTip(this.cmdDelWeblink, "���ɾ��ѡ�еĲɼ���ַ");
            HelpTip.SetToolTip(this.listWeblink, "��ʾ��ǰ�Ѿ����úõĲɼ���ַ");
            HelpTip.SetToolTip(this.IsExportGUrl, "ѡ�д�ѡ������Զ������ǰ�ɼ����ݵ���ַ");
            HelpTip.SetToolTip(this.IsExportGDate, "ѡ�д�ѡ������Զ�����ɼ����ݵĵ�ǰʱ��");
            HelpTip.SetToolTip(this.cmdRegex, "�����ť���Դ������������ϵͳ�������������ɲɼ�ƥ�����" + "\r\n" + "ͨ���������������֤����������ȷ���");
            HelpTip.SetToolTip(this.cmdUp, "�ɼ����������谴����ҳ˳���������£�" + "\r\n" + "����˰�ť���Ե����ɼ������˳������");
            HelpTip.SetToolTip(this.cmdDown, "�ɼ����������谴����ҳ˳���������£�" + "\r\n" + "����˰�ť���Ե����ɼ������˳������");
            HelpTip.SetToolTip(this.button3, "�������ֿ�����������ɲɼ����������" + "\r\n" + "������Ajax������Json������XML�ṹ�����ݣ��������ֲ���ʹ��");
            HelpTip.SetToolTip(this.cmdAddCutFlag, "���Ӳɼ����ݵĹ���");
            HelpTip.SetToolTip(this.cmdEditWeblink, "�༭��ǰѡ�еĲɼ����ݹ���");
            HelpTip.SetToolTip(this.cmdDelCutFlag, "ɾ����ǰѡ�еĲɼ����ݹ���");
            HelpTip.SetToolTip(this.listWebGetFlag, "��ʾ�Ѿ�������ɵĲɼ����ݹ���");
            HelpTip.SetToolTip(this.IsPublishData, "�������ݷ���������Ĭ������²ɼ����ݻ���ʱ���浽���ش��̣�" + "\r\n" + "ѡ���ѡ����ɽ��ɼ������ݷ�����ָ��������Դ����վ");
            HelpTip.SetToolTip(this.raInsertDB, "�ɼ�����������ʱ��һ��Ҫѡ��ֱ����⣬ֱ����⽫������ʱ�洢���ݣ�" + "\r\n" + "�ɼ������ݽ�ֱ�Ӵ������ݿ⣬�ʺϴ�������Ĳɼ���Ʃ�磺��ʮ�������ϰ������ݵĲɼ�����");
            HelpTip.SetToolTip(this.udPublishThread, "���÷����Ĺ����̣߳������ر��Ҫ���벻Ҫ�޸Ĵ���");
            HelpTip.SetToolTip(this.IsDelTempData, "ѡ�д�ѡ������ݷ�����ɺ�ɾ����ʱ����Ĳɼ����ݣ�" + "\r\n" + "һ������²�Ҫѡ���ѡ������޷�ͨ�������������Ѿ��ɼ���������");
            HelpTip.SetToolTip(this.comTemplate, "�ڴ�ѡ���Ѿ����úõķ���ģ�壬���ڵ�ǰ���ݵķ���������" + "\r\n" + "����ģ���ͨ���˵� ����->����ģ�� ���й���");
            HelpTip.SetToolTip(this.txtDomain, "�ڴ�������Ҫ������ָ����վ��������ַ");
            HelpTip.SetToolTip(this.txtUser, "�ڴ����뷢����ָ����վ�ĵ�¼�û���");
            HelpTip.SetToolTip(this.txtPwd, "�ڴ����뷢����ָ����վ������");
            HelpTip.SetToolTip(this.txtTDbConn, "��ͨ������İ�ť���÷�����ָ�����ݿ������Դ��Ϣ");
            HelpTip.SetToolTip(this.button18, "����˰�ť���÷�����ָ�����ݿ������Դ��Ϣ");
            HelpTip.SetToolTip(this.dataParas, "�ڴ˸�����ѡģ��ķ�����������һ��Ӧ���ɼ����ݹ���ʵ�ַ���������ɼ����ݹ�ϵ�Ķ�Ӧ");
            HelpTip.SetToolTip(this.raExportTxt, "ѡ�д�ѡ��򽫲ɼ������ݷ������ı��ļ���");
            HelpTip.SetToolTip(this.raExportCSV, "ѡ�д�ѡ��򽫲ɼ������ݷ�����CSV�ļ���");
            HelpTip.SetToolTip(this.raExportExcel, "ѡ�д�ѡ��򽫲ɼ������ݷ�����Excel�ļ���");
            HelpTip.SetToolTip(this.raExportWord, "ѡ�д�ѡ��򽫲ɼ������ݷ�����Word�ļ���");
            HelpTip.SetToolTip(this.IsIncludeHeader, "ѡ�д�ѡ���򷢲����ݻ��Զ������ɼ����ݹ�������ƣ�����ֻ��������");
            HelpTip.SetToolTip(this.IsRowFile, "ǿ��һ����¼������һ���ļ��У�ָ���ļ���ϵͳ���Զ��ļ�������������Խ��д洢");
            HelpTip.SetToolTip(this.cmdInsertFileDate, "����˲ɼ���������˲ɼ��ƻ���������У�����ʱ�������" + "\r\n" + "���Խ�ÿ�βɼ����ݵ������ĵ�����ʱ���ǣ������ļ�����");
            HelpTip.SetToolTip(this.cmdInsertFileDate, "����˰�ť������һ����Ҫ�����ļ����ļ���");
            HelpTip.SetToolTip(this.raExportAccess, "ѡ�д�ѡ������ݷ�����Access���ݿ�");
            HelpTip.SetToolTip(this.raExportMySql, "ѡ�д�ѡ������ݷ�����Mysql���ݿ�");
            HelpTip.SetToolTip(this.raExportMSSQL, "ѡ�д�ѡ������ݷ�����MSSqlServer���ݿ�");
            HelpTip.SetToolTip(this.raExportOracle, "ѡ�д�ѡ������ݷ�����Oracle���ݿ�");
            HelpTip.SetToolTip(this.txtDataSource , "�ڴ��������ݿ�������Ϣ��Ҳ�ɵ���Ҳఴť��������");
            HelpTip.SetToolTip(this.button12, "����˰�ť���������ݿ�������Ϣ");
            HelpTip.SetToolTip(this.comTableName, "�ٴ�ѡ�����ݱ�Ҳ�������һ�������ڵ����ݱ�" + "\r\n" + "�����������ݱ����ڣ�ϵͳ���Զ����������ݱ�");
            HelpTip.SetToolTip(this.txtInsertSql, "Ĭ�������ϵͳ���Զ�����sql��䣬������ǷǼ�����Ա��" + "\r\n" + "�����޸ģ�������˽�sql�﷨���������޸�");
            HelpTip.SetToolTip(this.IsSqlTrue, "Ĭ������£�ϵͳͨ�����з�ʽ�������ݣ������Զ��������ݵĺϷ��ԣ�" + "\r\n" + "��Ҳ��ѡ�д��ǿ���ύsql��䷢�����ݣ�����ȷ���������ݵĺϷ��ԣ�" + "\r\n" + "���ַ�ʽ����Ч�ʸ��ߣ����Լ���Ҫ��Ҳ��");
            HelpTip.SetToolTip(this.button13, "������޸���Sql��䣬���Ե���˰�ť����������Ҫ������������");
            HelpTip.SetToolTip(this.button19, @"������Խ������ȷ�����Ե���˰�ť�������������" + "\r\n" + "���Բɼ��������õ��������һ������������");
            HelpTip.SetToolTip(this.button5, @"����˰�ť��ϵͳ��ģ����ʽ�ɼ�������ɼ����������֤�ɼ��������ȷ��");
            HelpTip.SetToolTip(this.udThread, "���òɼ��Ĺ����̣߳�Ĭ������������޸Ĵ��" + "\r\n" + "ͬʱҲ��ע�⣺�����߳���Խ�߲ɼ�Ч�ʾ�Խ�ߣ��ɼ�Ч�����뵱ǰ�ļ�������ܡ�" + "\r\n" + "�����ɼ�Ŀ����վ���������йأ�����������Χ�ڣ��߳���Խ�࣬�ɼ�Ч�ʾ�Խ��");
            HelpTip.SetToolTip(this.udAgainNumber, "��ż�����������������վ���ʴ���ʱ��ϵͳ�����¶�ʧ�ܵ���ַ���вɼ���" + "\r\n" + "�˴��������²ɼ��Ĵ����������˴������������ɼ���һ����ַ");
            HelpTip.SetToolTip(this.IsIgnore404, "������404����ʱ�����Ժ�������");
            HelpTip.SetToolTip(this.udGIntervalTime, "���ô�ѡ����Կ��Ʋɼ���Ƶ�ʣ����ɼ����һ����ַ��ֹͣһ��ʱ������ɼ���һ����ַ��" + "\r\n" + "���ô�ѡ��󣬹����߳̽�������Ϊ1����ѡ��Է�����վ�ر���Ч");
            HelpTip.SetToolTip(this.udGIntervalTime1, "���ô�ѡ����Խ��ɼ������ָ���ķ�Χ����ϵͳ���ѡ����вɼ���");
            HelpTip.SetToolTip(this.IsUrlAutoRedirect, "Ĭ�������������ַ�ض��򣬷�����ҳ�����������������������ֹ�ض��������" + "\r\n" + "������޷��ɼ����ݻ������ļ�������������������ʵ�ʲɼ����������һ��������벻Ҫ�޸Ĵ�ѡ��");
            HelpTip.SetToolTip(this.IsProxy, "Ĭ�������ͨ������ֱ�ӷ��ʲɼ���վ���������ô�ѡ�ͨ�����������վ��" + "\r\n" + "�����ַ��ͨ��ϵͳ�������ã�����Ҳ�ѡ��Ϊ��ѡ��������ʹ���Ѿ����õĴ����ַ");
            HelpTip.SetToolTip(this.IsProxyFirst, "Ĭ�����ѡ��ʹ�ô������һ�ִ�����ѯ�Ļ��ƣ�" + "\r\n" + "������ʹ�����õ����д����ַ��ѡ�д�ѡ��󣬿���ǿ������ʹ�õ�һ�������ַ");
            HelpTip.SetToolTip(this.IsUrlNoneRepeat, "ѡ�д�ѡ������۴˲ɼ��������ж��ٴΣ���������Ѿ��ɼ��ĵ�ַ�ٽ������ݲɼ���" + "\r\n" + "��Ϊ���ػ��ƣ�ע�⣺ϵͳ����ֻ�����յĲɼ�ҳ��������أ��ʺ϶������е��������ݲɼ�");
            HelpTip.SetToolTip(this.IsSucceedUrl, "ѡ�д�ѡ��󣬲ɼ��������ַ���������أ������ٴ����вɼ�����" + "\r\n" + "��Է����������ַ�����زɣ��Ӷ�ȷ�����ݵ�������");
            HelpTip.SetToolTip(this.cmdSetHeader, "����˰�ť����ͷ����Ϣ�����ò���");
            HelpTip.SetToolTip(this.cmdClearHeader, "����˰�ť����Ѿ����õ�ͷ����Ϣ");
            HelpTip.SetToolTip(this.IsPluginsDealData, "ѡ�д�ѡ��󣬿������ñ༭�ɼ����ݵĲ��");
            HelpTip.SetToolTip(this.cmdBrowserPlugins2, "����˰�ťѡ���ȡCookie�����ļ�");
            HelpTip.SetToolTip(this.cmdSetPlugins2, "����˰�ť����ѡ�в���Ĳ���������δ�ṩ�������ã������˰�ť��Ч");
            HelpTip.SetToolTip(this.cmdBrowserPlugins3, "����˰�ťѡ�����ݱ༭�����ļ�");
            HelpTip.SetToolTip(this.cmdSetPlugins3, "����˰�ť����ѡ�в���Ĳ���������δ�ṩ�������ã������˰�ť��Ч");
            HelpTip.SetToolTip(this.IsNoDataStop, "ѡ�д�ѡ���������޷��ɼ�������ʱ��ִ�в���");
            HelpTip.SetToolTip(this.nNoDataStopCount, "�趨һ��ֵ����������ֵ����ʼ����ѡ�е�ִ�в���");
            HelpTip.SetToolTip(this.comNoDataStopRule, "���ɼ������޷��ɼ������ݵ�����ʱ�����е�ִ�в���");
            HelpTip.SetToolTip(this.IsNoInsertStop, "ѡ�д�ѡ������ò����ظ���ִ�в�����ע�⣺��ѡ����ԡ�ֱ����⡱��Ч");
            HelpTip.SetToolTip(this.nNoInsertStopCount, "�趨һ��ֵ����������ֵ����ʼ����ѡ�е�ִ�в���");
            HelpTip.SetToolTip(this.IsRepeatStop, "ѡ�д�ѡ������òɼ����ظ���¼��ִ�в�����" + "\r\n" + "ע�⣺ϵͳ���ظ���¼���ж�ȡ�������ڲɼ����������õĲ������ظ���������");
            HelpTip.SetToolTip(this.comRepeatStopRule, "���ɼ����ظ���¼ʱ��ִ�еĲ�����" + "\r\n" + "ע�⣺ϵͳ���ظ���¼���ж�ȡ�������ڲɼ����������õĲ������ظ���������");
            HelpTip.SetToolTip(this.isNoneAllowSplit, "ѡ�д�ѡ���������ٽ��зֽⲢ�ɶ�̨�����Эͬ�ɼ���ɣ����ɲɼ���������ɼ����");

            HelpTip.SetToolTip(this.txtSavePath, "�ɼ����ݻ���ʱ���浽��ָ����Ŀ¼��");
            HelpTip.SetToolTip(this.txtHeader, "HTTP Header���ڸ߼����ã�ͨ��Ӧ���ڲɼ���֤��Ϊ�ϸ����վ������������ݷ���");

            HelpTip.SetToolTip(this.txtStartPos, "�����Ҫ�޶��ɼ�ҳ�Ĳɼ���Χ�����ڴ����뿪ʼ�ɼ����ַ�����" + "\r\n" + @"�����ϵͳ����ָ���ķ�Χ�ڲɼ����ݣ����Բɼ�ҳ��Ч");
            HelpTip.SetToolTip(this.txtEndPos, "�����Ҫ�޶��ɼ�ҳ�Ĳɼ���Χ�����ڴ���������ɼ����ַ�����" + "\r\n" + @"�����ϵͳ����ָ���ķ�Χ�ڲɼ����ݣ����Բɼ�ҳ��Ч");

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
        /// ����Զ�̷���������
        /// </summary>
        /// <param name="tName">��������</param>
        public void LoadRemoteTask(string tName,string strXML)
        {
            int i = 0;
            m_listNaviRules = null;
            m_listNaviRules = new List<eNavigRules>();

            oTask t = new oTask(Program.getPrjPath());
            t.LoadTaskInfo(strXML);

            LoadTaskInfo(t);

            this.m_IsRemoteTask = true;
            this.Text= "Զ�̲ɼ�����༭״̬��" + this.Text;
        }

        private void LoadTask(string TClassPath, string TaskName)
        {
            //ÿ�μ�������ǰ�������뽫�������ÿ�
            
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

            #region ������Ϣ

            //��ʼ�ж�����汾�ţ��������汾�Ų�ƥ�䣬�򲻽���������Ϣ�ļ���
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

            #region �߼���Ϣ������

            //���ظ߼�����
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

            //V2.0�ṩ
            this.IsProxy.Checked = t.TaskEntity.IsProxy;
            this.IsProxyFirst.Checked = t.TaskEntity.IsProxyFirst;

            //V2.1�ṩ
            this.IsUrlNoneRepeat.Checked = t.TaskEntity.IsUrlNoneRepeat;
            this.IsSucceedUrl.Checked = t.TaskEntity.IsSucceedUrlRepeat;

            //V5�ṩ
            this.IsUrlAutoRedirect.Checked = t.TaskEntity.IsUrlAutoRedirect;

            //V5.1����
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

            //v5.31����
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
           

            //V5.33����
            this.m_ThreadProxy = t.TaskEntity.ThreadProxy;
            this.isSameSubTask.Checked = t.TaskEntity.isSameSubTask;

            this.isGatherCodingByPB.Checked =t.TaskEntity.isGatherCoding ;
            //this.txtCodingFlag.Text= t.GatherCodingFlag ;
            this.txtPluginsCodingByPB.Text= t.TaskEntity.GatherCodingPlugin  ;
            this.txtCodingUrlByPB.Text= t.TaskEntity.CodeUrl ;

            this.isCloseTab.Checked = t.TaskEntity.isCloseTab;

            #endregion

            #region HTTP Header
            //������ʾ״̬
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

            #region ������

            //this.IsStartTrigger.Checked = t.IsTrigger;
            //if(this.IsStartTrigger.Checked==true )
                //SetTriggerEnabled(t.IsTrigger);

            //if (t.TriggerType == ((int)cGlobalParas.TriggerType.GatheredRun).ToString ())
            //    this.raGatheredRun.Checked = true;
            //else if (t.TriggerType == ((int)cGlobalParas.TriggerType.PublishedRun).ToString ())
            //    this.raPublishedRun.Checked = true;

            //�����������񣬴�������V5.5ȥ��
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

            #region  �������
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

            #region ��ַ��Ϣ

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

                //�����ҳ�ɼ�����
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

            #region �ɼ�����

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

                //��ʼ�������ݼӹ�����
                if (t.TaskEntity.WebpageCutFlag[i].ExportRules.Count > 0)
                {
                    //��ʾ�����ݼӹ�����
                    item.ImageIndex = 17;
                    this.listWebGetFlag.Items.Add(item);

                    //��ʼ������ݼӹ�����
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

                //���ص�������
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

            #region �������� �����أ���Ϊ��������˷���ģ�棬ϵͳ����òɼ����������
            //�����Ƿ񵼳�����¼�������߳�
            this.udPublishThread.Value = t.TaskEntity.PublishThread;

            if (t.TaskEntity.RunType==cGlobalParas.TaskRunType.OnlyGather)
            {
                //���ɼ�����
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
                
                    //���ڵ�������
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
                        //��ʾ���÷���ģ��������ݷ�������
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
                                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = "{�ֹ�����}";
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

        #region ������ʼ������ ����������״̬���м��أ��½����޸ġ������

        private void IniData()
        {
            //��ʼ��ҳ���������

            //��ʼ����Դ�ļ��Ĳ���
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //���ݵ�ǰ�����������ʾ��Ϣ�ļ���
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

            //��ʼ������ģ�����Ϣ
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

            //this.labLogSavePath.Text = rm.GetString("Info2") + "��" + Program.getPrjPath() + "Log";

            //��ʼ��ҳ�����ʱ�����ؼ���״̬

            //��ʼ���������
            //��ʼ��ʼ�����νṹ,ȡxml�е�����,��ȡ�������
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
                    //�����ӷ���
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
                    //�����ӷ���
                    LoadTreeClass(newNode, ets[i].Children);
                }

                tNode.Nodes.Add(newNode);
                newNode = null;
            }
        }

        #endregion

        #region ��ť��������Ʋ���

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
                    //��Ӵ��е����������ַ

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
                    //�����ͨ��ַ

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

                //������ַ����

                NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath());
                UrlCount = cu.GetUrlCount(fgUrlRule.Url );
                cu = null;

                litem.SubItems.Add(UrlCount.ToString());

                //�ж��Ƿ���ж�ҳ�ɼ�����
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

                //ɾ����ҳ����
                for (int i = 0; i < m_MultiPageRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_MultiPageRules[i].Url)
                        m_MultiPageRules.Remove(m_MultiPageRules[i]);
                }

                if (fgUrlRule.IsNav == true)
                {
                    this.listWeblink.SelectedItems[0].SubItems[1].Text = "Y";

                    //ɾ���洢�ĵ�������
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

                //�ж��Ƿ���ж�ҳ�ɼ�����
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
            //    MessageBox.Show("ϵͳ�޷��Զ���ȡ��ҳ���룬��ͨ���鿴ҳ�����ԣ�Firefox����鿴ҳ����루IE�����ж�ҳ������ʽ", "��Ϣ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            //}
        }

        private void cmdDelWeblink_Click(object sender, EventArgs e)
        {
            if (this.listWeblink.SelectedItems.Count == 0)
                return;

            //����е�����Ҫɾ�������Ĺ���
            //ɾ���洢�ĵ�������

            RemoveUrl();
            

        }

        private void RemoveUrl()
        {
            //ɾ������
            if (this.listWeblink.SelectedItems[0].SubItems[1].Text.ToString() == "Y")
            {
                for (int i = 0; i < m_listNaviRules.Count; i++)
                {
                    if (this.listWeblink.SelectedItems[0].Text == m_listNaviRules[i].Url)
                        m_listNaviRules.Remove(m_listNaviRules[i]);
                }
            }

            //ɾ����ҳ
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

            #region ��������
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

            #region ���ݷ�������

            //�ж��Ƿ񵼳��ļ����洢������������Ϣ
            //�����Ƿ񵼳�����Ҫ��¼�������߳���
            t.PublishThread = int.Parse(this.udPublishThread.Value.ToString());
            if (t.RunType==cGlobalParas.TaskRunType.OnlySave || t.RunType == cGlobalParas.TaskRunType.GatherExportData)
            {
                if (this.raPublishByTemplate.Checked == true)
                {
                    #region ģ��ɼ�
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
                            if ("{�ֹ�����}" == this.dataParas.Rows[pi].Cells[1].Value.ToString())
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
                    #region �������ļ������ݿ�
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

            #region �߼�����������
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

            //V2.0�ṩ
            t.IsProxy = this.IsProxy.Checked;
            t.IsProxyFirst = this.IsProxyFirst.Checked;

            //V2.1�ṩ
            t.IsUrlNoneRepeat = this.IsUrlNoneRepeat.Checked;
            t.IsSucceedUrlRepeat = this.IsSucceedUrl.Checked;
            //V5�ṩ
            t.IsUrlAutoRedirect = this.IsUrlAutoRedirect.Checked;

            //V5.1����
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

            //V5.2����
            t.IsIgnoreErr = this.IsIgnoreErr.Checked;
            t.IsAutoUpdateHeader = this.IsAutoUpdateHeader.Checked;
            t.IsNoneAllowSplit = this.isNoneAllowSplit.Checked;
            t.IsSplitDbUrls = this.IsSplitDbUrls.Checked;

            //v5.31����
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
            

            //V5.33����
            t.ThreadProxy = this.m_ThreadProxy;
            t.isSameSubTask = this.isSameSubTask.Checked;
            t.isGatherCoding = this.isGatherCodingByPB.Checked;
            //t.GatherCodingFlag = this.txtCodingFlag.Text;
            t.GatherCodingPlugin = this.txtPluginsCodingByPB.Text;
            t.CodeUrl = this.txtCodingUrlByPB.Text;

            t.isCloseTab = this.isCloseTab.Checked;

            #endregion

            #region HTTPHeader����


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

            #region ������
            t.IsTrigger = false;

            //if (this.raGatheredRun.Checked == true)
            //    t.TriggerType = ((int)cGlobalParas.TriggerType.GatheredRun).ToString();
            //else if (this.raPublishedRun.Checked == true)
            //    t.TriggerType = ((int)cGlobalParas.TriggerType.PublishedRun).ToString();

            //cTriggerTask tt;

            //��ʼ��Ӵ�����ִ�е�����
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

            #region �����Ϣ
          
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

            #region ˮӡ��Ϣ
            //t.IsWatermark = this.IsWatermark.Checked;
            //t.WatermarkText = this.txtWatermark.Text;
            //t.FontFamily = this.comFontType.Text;
            //t.FontSize = int.Parse(this.upFontSize.Value.ToString());
            //t.IsFontWeight = this.IsFontWeight.Checked;
            //t.IsFontItalic = this.IsFontItalic.Checked;
            //t.FontColor = this.cmdFontColor.ForeColor.ToArgb().ToString();
            //t.WatermarkPOS = cGlobalParas.ConvertID(this.comWatermarkPos.Text);
            #endregion

            #region ��ַ��Ϣ
            List<eWebLink> wLinks = new List<eWebLink>();
            t.UrlCount = FillWebLinks(ref wLinks);
            t.WebpageLink = wLinks;
            #endregion

            #region �ɼ�����
            List<eWebpageCutFlag> wFlags = new List<eWebpageCutFlag>();
            FillGatherRule(ref wFlags);
            t.WebpageCutFlag = wFlags;
            #endregion

            #region ��⵱ǰ�����Ƿ�ʹ���˿��ӻ��Ĳ���
            //�����������Ƿ��õ���XPATH���ӻ�������
            //����Ϊ3�����������򡢷�ҳ��������

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

                    //��ӵ�������
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

                //��Ӷ�ҳ�ɼ�����
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

                    //��ҳ����
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

        #region �������� ������
        //����û��������ݵ���Ч�ԣ�ֻҪ���������ƾͿ��Ա��棬����ʹ���Ѷ�
        //�����������һ������ִ�У���Ҫ��ִ��ǰ����һ���޸�
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
                this.errorProvider1.SetError(this.tTask, "�ɼ����������в��ܳ��������ַ���.*\\/!?-|@$%^&,<>{}()[]#+");
                return false;
            }

            if (this.txtHeader.Text == "")
            {
                this.errorProvider1.SetError(this.txtHeader, rm.GetString("Error41"));
                return false;
            }

            //��ʼ���ɼ��������õ���ȷ��
            if (IsPublishData.Checked==true )
            {
                if (this.raInsertDB.Checked ==true )
                {
                    //ʹ�����ݿⷢ�����ݣ���֤����Ч��
                    string sql = this.txtInsertSql.Text.Trim();
                    if (sql.IndexOf(";") > 0 && this.IsSqlTrue.Checked == false)
                    {
                        MessageBox.Show("����������������ʹ�ö�sql��䣬��ѡ�С�ֱ���ύsql��䷢�����ݡ�ѡ��,�����ʹ�ö�sql��䣬��ȥ��;", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    if (this.IsSqlTrue.Checked == false && !sql.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase))
                    {
                        MessageBox.Show("�����Ҫʹ�÷�insert��䣬��ѡ�С�ֱ���ύsql��䷢�����ݡ�ѡ��", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    ////��insert�����з���
                    //if (sql.StartsWith("insert", StringComparison.CurrentCultureIgnoreCase))
                    //{
                    //    string pSql=sql.Substring (""
                    //}

                }
            }

            return true;
        }

        #endregion

        #region ���ݸ�ί�еķ���
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
                //ɾ���ɼ�����
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

                //ɾ�������е����ݼӹ�����
                for (int i = 0; i < m_gRules.Count; i++)
                {
                    if (gName == m_gRules[i].gName)
                    {
                        //��ʼ�ж��ǵڼ�������Ȼ��Ӽ�����ɾ���ڼ���
                        m_gRules[i].fieldDealRules.FieldRule.RemoveAt(index);
                        break;
                    }
                }

                this.listWebGetFlag.Items.Remove(this.listWebGetFlag.SelectedItems[0]);

                //��ʼ������ʾ�����
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
                    //��ʾ�˲ɼ������µļӹ������Ѿ�ɾ������
                    this.listWebGetFlag.Items[ruleIndex].ImageIndex = -1;
                }

            }
           
            this.IsSave.Text = "true";
        }

        private void frmTask_Load(object sender, EventArgs e)
        {
            this.Text+= Program.SominerTaskVersionCode;

            #region ʶ��ǰ�İ汾�������ݰ汾�Ĳ�ͬ���ز�ͬ������
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




            //��Tooltip���г�ʼ������
            HelpTip.ToolTipIcon = ToolTipIcon.Info;
            HelpTip.ForeColor =Color.YellowGreen;
            HelpTip.BackColor = Color.LightGray;
            HelpTip.AutoPopDelay = 20000;
            HelpTip.ShowAlways = true;
            HelpTip.ToolTipTitle = "С����ʾ";

            SetTooltip();

            switch (this.FormState)
            {
                case cGlobalParas.FormState.New :
                    ModifyTitle("�½��ɼ�����");
                    break;
                case cGlobalParas.FormState.Edit :
                    //�༭״̬���������޸ķ���
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
            ////��ʼ�����������datagrid�ı�ͷ
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


            this.cmdCancel.Text = "�� ��";

            this.cmdOK.Enabled =false ;

            this.cmdApply.Enabled = false;
            this.cmdWizard.Enabled = false;
        }

        //����һ�����������޸����������
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

        //    //���Բɼ��������û���������ݲ��Բɼ�
        //    //��֤�����Ƿ���ȷ
        //    //�˲��������п�������һ���е���һ��ҳ��������

        //    //�����ж��Ƿ�Ҫ���м����ɼ���������ʹ�õ���Ĭ����ַ�б�
        //    //�еĵ�һλ�����Ȼ�ȡ��ַ���������Ӧ����������
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

        //        //��ӵ�������
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
        //        //ֱ�Ӳ���
        //        retValue=TestSingleData(w, tmpSavePath, IsAjax);
        //    }
        //    else
        //    {
        //        retValue=TestNaviData(w, tmpSavePath, IsAjax);
        //    }

        //    w = null;

        //    //�󶨵���ʾ��DataGrid��
        //    //this.dataTestGather.DataSource = retValue;

        //}

        //private DataTable TestSingleData(cWebLink w, string tmpSavePath, bool IsAjax)
        //{
        //    bool IsMerge = false;

        //    //�ж��Ƿ��Ѿ���ȡ��ʾ����ַ�����û�У��������ȡ
        //    if (this.txtWeblinkDemo.Text.ToString() == null || this.txtWeblinkDemo.Text.ToString() == "")
        //    {
        //        GetDemoUrl();
        //    }

        //    #region ��Ӳɼ���־

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

        //        //��Ӳɼ����ݵ��������
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

        //    #region ��������

        //    DataTable retValue = null;

        //    if (w.NavigRules == null || w.NavigRules.Count ==0)
        //    {
        //        throw new NetMinerException("ѡ���˵���ѡ��ȴδ���õ�����������ɼ���ַ������Ϣ");
        //    }

        //    if (w.NavigRules[0].IsGather == true)
        //    {

        //        #region �����ɼ�

        //        //���Ӳɼ��ı�־
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

        //            //��Ӳɼ����ݵ��������
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

        //        //�����ɼ�
        //        DataTable dt = null;

        //        for (int m = 0; m < w.NavigRules.Count; m++)
        //        {

        //            #region ���Ӳɼ���־
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

        //                //��Ӳɼ����ݵ��������
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
        //                else if (c.NavLevel == w.NavigRules[i].Level + 1)    //����ж��Ƕ༶�����ļ������ݲɼ��ϲ�����Ķ�Ӧ��ϵ��δ���ԣ���֪����ȷ���
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

        //        #region �����ɼ�����
                
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

        //            //��ʼ��ȡ��һҳ�ĵ�ַ
        //            //cGatherWeb gWeb = new cGatherWeb();

        //            string sCookie = this.txtCookie.Text;

        //            cGatherWeb gWeb = new cGatherWeb();
        //            cGlobalParas.WebCode urlCode=cGlobalParas.WebCode.auto;
        //            if (this.IsUrlEncode.Checked == true)
        //                urlCode = (cGlobalParas.WebCode)int.Parse(this.comUrlEncode.Text);

        //            string webSource = gWeb.GetHtml(NewUrl, (cGlobalParas.WebCode)cGlobalParas.ConvertID(this.comWebCode.SelectedItem.ToString()),
        //                         this.IsUrlEncode.Checked , urlCode, ref sCookie, "", "", true, IsAjax);
        //            gWeb = null;

        //            //�������ļ���ȡ����һҳ�ķ�ҳ�������
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
        //    //����һ���޸ķ������Ƶ�ί��
        //    delegateGData sd = new delegateGData(this.GatherTestData);

        //    //��ʼ���ú���,���Դ����� 
        //    IAsyncResult ir = sd.BeginInvoke(Url, gFlag,EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comWebCode.SelectedItem.ToString()), 
        //        this.txtCookie.Text.ToString(), this.txtStartPos.Text.Trim () , this.txtEndPos.Text.Trim () , 
        //        tmpSavePath, IsAjax,this.IsExportGUrl.Checked ,this.IsExportGDate .Checked, null, null);

        //    //��ʾ�ȴ��Ĵ��� 
        
        //    frmWaiting fWait = new frmWaiting(rm.GetString("Info5"));
        //    new Thread((ThreadStart)delegate
        //    {
        //        Application.Run(fWait);
        //    }).Start();

        
        //    //ѭ������Ƿ�������첽�Ĳ��� 
        //    while (true)
        //    {
        //        if (ir.IsCompleted)
        //        {
        //            //����˲�����رմ���
                    
        //            break;
        //        }
        //    }

        //    //ȡ�����ķ���ֵ 
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

        //    //�Ȱ��ֵ���е�������ȡ��������������Ŀ����Ϊ���ڱ����base64��ʱ��
        //    //���ֽ��ֵ����Ҳ���������ɴ���
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

        //    //���н�����ַ�����Base64������
        //    //��ȡ��ַ�����Ƚ���Url�����Base64�Ĵ���
        //    if (this.IsUrlEncode.Checked == true)
        //        nUrl = cTool.UrlEncode(nUrl, (cGlobalParas.WebCode)(cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString())));

        //    //�ڴ˴����Ƿ���Ҫ����Base64����ĵ�����
        //    if (Regex.IsMatch(nUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
        //    {

        //        Match s = Regex.Match(nUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
        //        string sBase64 = s.Groups[0].Value.ToString();
        //        sBase64 = cTool.Base64Encoding(sBase64);

        //        //��base64���벿�ֽ���url�滻
        //        nUrl=Regex.Replace(nUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //    }

        //    //���жϵ�ǰ����ַ�Ƿ��в���������в�������Ҫ�ֽ����
        //    //��ǰ���������Ƿ��в�����ȥ�ֽ�һ�Σ����û�в������򷵻�ԭַ

        //    cProxyControl pControl = new cProxyControl(0);

        //    cUrlAnalyze cu = new cUrlAnalyze(ref pControl,this.IsProxy.Checked ,this.IsProxyFirst.Checked);

        //    List<string> nUrls= cu.SplitWebUrl(nUrl);
        //    cu = null;

        //    if (nUrls == null || nUrls.Count == 0)
        //    {
        //        MessageBox.Show(rm.GetString ("Error6"), "�п��ܵ����������δ�ܵ�������ַ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //        return;
        //    }

        //    nUrl = nUrls[0].ToString();
        //    nUrls = null;

        //    if (this.IsUrlEncode.Checked == true)
        //        nUrl = cTool.UrlEncode(nUrl, (cGlobalParas.WebCode)(cGlobalParas.ConvertID(this.comUrlEncode.SelectedItem.ToString())));
           
        //    //���Ե�����ʱ�����账���ҳ��һЩ����
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
        //        MessageBox.Show(rm.GetString ("Error6") + " ����ƥ����Ϊ��" + Url, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            //����һ����ȡ������ַ��ί��
            delegateGNavUrl sd = new delegateGNavUrl(this.GetNavUrl);

            ////��ʼ���ú���,���Դ����� 
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



            ////ѭ������Ƿ�������첽�Ĳ��� 
            //while (true)
            //{
            //    if (ir.IsCompleted)
            //    {
            //        //����˲�����رմ���
            //        break;
            //    }

            //}

            ////ȡ�����ķ���ֵ 
            //List<string> rUrls = sd.EndInvoke(ref cookie,ir);

            //this.txtCookie.Text = cookie;

            List<string> rUrls = (List<string>)result;
            //SetValue((this.txtCookie), "Text", cookie);
            return rUrls;
        }

        //ÿ�ν��ܻ�ȡһ�ε��������ܻ�ȡ��ε�������ַ������ǰֻ֧��һ�㵼������㵼����̽��
        //��Ҫ�ظ����ô˵�ַ
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
                //MessageBox.Show ("��ȡ������ַ����������Ϣ��" + ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            ExportGatherLog(cGlobalParas.LogType.Warning, "��ַ����������Խ��Ե�һ����ַ���в��ԣ�Ŀ����Ϊ�˼��������ȷ�ԣ������������ԣ�");


            bool IsNav = false;

            #region ��ȡͷ��������Ϣ
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

            //������ַ����

            NetMiner.Core.Url.cUrlParse cu = new NetMiner.Core.Url.cUrlParse(Program.getPrjPath ());

            try
            {
                Urls = cu.SplitWebUrl(DemoUrl);
            }
            catch (System.Exception ex)
            {
                ExportGatherLog(cGlobalParas.LogType.Error,"��ַ��������ʧ�ܣ������ֵ�����ã�������ڴ�����������Ž�����ֵ���٣�������Ϣ��" + ex.Message );
                return;
            }
            cu = null;

            int count = Urls.Count;
            if (count > 1)
                count = 1;

            //��һ����ַ
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
                InvokeMethod(this, "AddUrlLog", new object[] { "������������" + ex.Message });
            }

            //�����������ҳ����
            if(wLinks[0].IsMultiGather==true )
            {
                for (int m=0;m<wLinks[0].MultiPageRules.Count;m++)
                {
                    List<string> multiUrls= GetMultiUrl(wLinks[0].Weblink, this.txtHeader.Text, wLinks[0].MultiPageRules[m].Rule);

                    if (multiUrls!=null && multiUrls.Count >0)
                    InvokeMethod(this, "AddNode", new object[] { this.treeTestUrl.Nodes, multiUrls[0] + " [��ҳ]" });
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

                            //�ȴ�����

                            #region ��ҳ����
                            if (cns[level - 1].IsNaviNextPage == true)
                            {
                                int FirstNextIndex = 0;

                                string nextUrl = us[index];
                                string Old_Url = string.Empty;

                                //��ʼ����ҳ
                                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "��ʼ���е�����ҳ����" });


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

                        //ͳһ�������ݹ�
                        //tNode.Text = tNode.Text + " [" + tNode.Nodes.Count + "]";
                        //Application.DoEvents();

                        try
                        {
                            InvokeMethod(this, "UpdateNodeText", new object[] { tNode, tNode.Text + " [" + tNode.Nodes.Count + "]" });
                        }
                        catch { }

                        InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "��������" + tNode.Nodes.Count + "����ַ" });

                        if (tNode.Nodes != null && tNode.Nodes.Count > 0)
                            GetNaviUrl(tNode.Nodes,Cookie, cns, urlCode, webCode, headers, level + 1, isAll, nUrl, isAUpdateHeader, strHeader,isUrlAutoRedirect);

                        
                    }

                    #region ���ҳ�淭ҳ����
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

                                    //ÿ��һҳ����һ��
                                    List<string> us1 = AddDemoUrl(nextUrl, Cookie, true, tempcns, urlCode, webCode, nUrl, isAUpdateHeader, strHeader, isUrlAutoRedirect);
                                    if (us1 != null)
                                    {
                                        for (int index = 0; index < us1.Count; index++)
                                        {
                                            //tNodeCol[nextUrl].Nodes.Add(us1[index], us1[index]);
                                            //Application.DoEvents();
                                            InvokeMethod(this, "AddNode", new object[] { tNodeCol[nextUrl].Nodes, us1[index] });

                                            #region ��ҳ����
                                            if (cns[level - 1].IsNaviNextPage == true)
                                            {
                                                FirstNextIndex = 0;

                                                string nextUrl1 = us1[index];
                                                Old_Url = string.Empty;

                                                //��ʼ����ҳ
                                                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "��ʼ���е�����ҳ����" });

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

                                        InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "��������" + tNode.Nodes.Count + "����ַ" });

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


        //������ַ���ݣ�����ַ�еı��롢�������ֽ����滻������һ���������õ���ַ
        private string TransUrl(string DemoUrl)
        {
            //��ȡ��ַ�����Ƚ���Url�����Base64�Ĵ���
            if (this.IsUrlEncode.Checked == true)
                DemoUrl = ToolUtil.UrlEncode(DemoUrl,EnumUtil.GetEnumName< cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString()));

            //�ڴ˴����Ƿ���Ҫ����Base64����ĵ�����
            if (Regex.IsMatch(DemoUrl, @"(?<=<BASE64>)[\S\s]*(?=</BASE64>)", RegexOptions.IgnoreCase))
            {

                Match s = Regex.Match(DemoUrl, @"(?<=<BASE64>).*(?=</BASE64>)", RegexOptions.IgnoreCase);
                string sBase64 = s.Groups[0].Value.ToString();
                sBase64 = ToolUtil.Base64Encoding(sBase64);

                //��base64���벿�ֽ���url�滻
                DemoUrl=Regex.Replace(DemoUrl, @"(<BASE64>).*(</BASE64>)", sBase64, RegexOptions.IgnoreCase | RegexOptions.Multiline);

            }


            //���жϵ�ǰ����ַ�Ƿ��в���������в�������Ҫ�ֽ����
            //��ǰ���������Ƿ��в�����ȥ�ֽ�һ�Σ����û�в������򷵻�ԭַ
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
            //MessageBox.Show("���ݿ����ӳɹ���", "soukey��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 2013-1-24 ���¹������ԣ�������һ����ʵ��������в���
        /// �ӹܺ�̨�߳̽��������Ĳ����������ص�һ�����ݡ�
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
                    MessageBox.Show("Զ�������޷����в��ԣ��������ص����غ��ٽ��в��Բ�����", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SaveRemoteTask();
                }

                if (this.IsSave.Text == "true")
                {
                    ExportGatherLog(cGlobalParas.LogType.Error,"���Բɼ������豣�浱ǰ������Ϣ��ϵͳ���Զ�����ʱʧ���ˣ���鿴�Ƿ���д�˲ɼ��������ƣ�");
                    return;
                }
            }

            if (this.listWeblink.Items.Count == 0 && this.listWebGetFlag.Items.Count == 0)
            {
                ExportGatherLog(cGlobalParas.LogType.Warning, "��ǰ�ɼ�����δ������ַ���ɼ����ݹ����޷��������ԣ������������Ϣ�������������Թ�����");
                return;
            }

            if (this.listWeblink.Items.Count > 0 && this.listWebGetFlag.Items.Count == 0)
            {
                ExportGatherLog(cGlobalParas.LogType.Info, "��ǰ�ɼ�����δ���òɼ����ݹ��򣬽�����ַ���ù�����в��ԡ�");
                TestUrlsRule();

                return;
            }


            m_IsGathering = true;
            this.button5.Enabled = false;
            this.button2.Enabled = false;
            Application.DoEvents();

            ExportGatherLog(cGlobalParas.LogType.Warning, "ע�⣺�ɼ����Խ����ݹ����ȡ����һ���ɼ�ҳ��������ݲɼ����ԣ�Ŀ����Ϊ�˼��������ȷ�ԣ����������ɼ���");

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

            //��ʼ��ȡ��һ����ַ��ע�⣬��һ����ڵ�ַ�п����Ǵ��в����ģ���ˣ��ֽ���ȡ��һ����ַ
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

            //��ʼ���ݺ�ȥ�����νṹ�������ݲɼ�
            //�Ƚ��е����ɼ�
            int naviMaxLeven = wLinks[0].NavigRules.Count;

            //�ڴ��жϵ������������νṹ�ļ����Ƿ�һ�£������һ�£������
            int treeDepth = 1;
            GetTreeDepth(this.treeTestUrl.Nodes[0], ref treeDepth);
            if (naviMaxLeven+1!=treeDepth)
            {
                throw new NetMinerException("��ַ���������ɼ���������ѱ�����ֹ������ȷ�ϲɼ���ַ���õ���ȷ�ԣ�");
            }

            List<eNavigRule> naviRules = wLinks[0].NavigRules;
            
            
            DataSet ds = new DataSet();
            //string Url = this.treeTestUrl.Nodes[0].Text;
            string referUrl = string.Empty;

            DataTable d= GetData(this.treeTestUrl.Nodes[0], referUrl,null ,1, naviRules, wLinks[0].MultiPageRules, wCutFlags);

            #region �ڴ˴������ݼӹ�����ĵ���
            ///�ڴ˴������ݼӹ�����ĵ���
            if (this.IsPluginsDealData.Checked == true)
            {
                ExportGatherLog(cGlobalParas.LogType.Warning, "���ò���������ݴ���");

                NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                if (this.txtPluginsDeal.Text == "")
                {
                    ExportGatherLog(cGlobalParas.LogType.Warning, "���������ݼӹ��������ĵ��ã�����û���ṩ����ļ���ַ��");
                }
                else
                {
                    d = rPlugin.CallDealData(d, this.txtPluginsDeal.Text);
                }
                rPlugin = null;
            }

            #endregion

            this.dTestData.DataSource = d;

            ExportGatherLog(cGlobalParas.LogType.Warning, "������ɣ�");

        }

        /// <summary>
        /// ��ȡ���νṹ�����
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
                //��ʾ��������ͼ�
                nLevel = 0;
            }

            if (Url.Contains(" ["))
            {
                Url = Url.Substring(0, Url.IndexOf(" [") ).Trim ();
            }

            List<eWebpageCutFlag> cFlags = new List<eWebpageCutFlag>();
            //��ȡ�˼���Ĳɼ�����
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
                //��ʾ�޲ɼ����򣬴˼�����Ҫ�ɼ�
                if (tNode.Nodes.Count > 0)
                {
                    return GetData(tNode.Nodes[0], Url, d, Level + 1, naviRules, multiPageRules, cutFlag);
                }
                else
                    return null;
            }
            else
            {
                #region ��ȡǰ���ȡ���
                string startPos = string.Empty;
                string endPos = string.Empty;
                if (naviRules.Count ==0 && Level==1)
                {
                    startPos = this.txtStartPos.Text;
                    endPos = this.txtEndPos.Text;
                }
                else if (naviRules.Count ==Level && Level==1)
                {
                    //�ɼ���Ҳ�ǲɼ�ҳ����
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

                #region �Ƿ�����Զ���ţ�������ڣ����ڴ�����

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

                //�ж��Ƿ�Ϊ����ѭ��
                if (Level>1 &&  naviRules[Level - 2].NavigRule.StartsWith("<MySelf>"))
                {
                    Match strMatchLoop = Regex.Match(naviRules[Level - 2].NavigRule, "(?<=<MySelf>).*?(?=</MySelf>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    strLoopFlag = strMatchLoop.Groups[0].ToString();
                }


                //�ڴ��ж��Ƿ��ж�ҳ��ַ
                if (tNode.NextNode != null)
                {
                    mNode = tNode.NextNode;
                    if (mNode.Text.Contains("[��ҳ]"))
                    {
                        isMulti = true;
                    }
                }


                if (nLevel==0)
                {
                    //���յĲɼ�ҳ��
                    d = gWeb.GetGatherData(Url, startPos, endPos,this.txtSavePath.Text, this.IsExportGUrl.Checked, this.IsExportGDate.Checked,
                        dr1, strLoopFlag, 1, "", cGlobalParas.RejectDeal.None);

                    DataTable d1 = new DataTable();
                    //�ж��ϲ��Ƿ��������ݣ�����У�����кϲ�
                    if (dRow!=null && dRow.Rows.Count > 0)
                    {
                        //��ʾ������
                        d1 = MergeDataTable(dRow.Rows[0], d);
                    }
                    else
                        d1 = d;

                    d = d1;
                }
                else
                {
                    //����ҳ�Ĳɼ�������ǵ���ҳ�ɼ����������ȡ��һ������
                    d = gWeb.GetGatherData(Url, startPos, endPos,this.txtSavePath.Text, this.IsExportGUrl.Checked, this.IsExportGDate.Checked,
                        dr1, strLoopFlag, 1, "", cGlobalParas.RejectDeal.None);

                    if (dRow != null &&  dRow.Rows.Count > 0)
                    {
                        //��ʾ������
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

                    //��ʼ��һ������Ĳɼ�����
                    if (tNode.Nodes.Count > 0)
                    {
                        d= GetData(tNode.Nodes[0], Url, d, Level + 1, naviRules, multiPageRules, cutFlag);
                    }

                    gWeb = null;

                }


                //�ڴ��ж��Ƿ��ж�ҳ����
                if(isMulti)
                {
                    //��ʼ��ȡ��ҳ������
                    if (d==null || d.Rows.Count ==0)
                    {
                        ExportGatherLog(cGlobalParas.LogType.Warning, "��ҳ���ݻ�ȡʧ�ܣ�");
                    }

                    List<string> mName = new List<string>();

                    //�жϵ�ǰ�����Ƿ���ڶ�ҳ����
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

                        //��ʼѭ���ɼ���ҳ����
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

            

            //��ȡ�˼���Ĳɼ�����
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

                    //��ӵ�������

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

                //��һҳ�����
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

                //����ҳ�ɼ�������
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

            //ȡ��󵼺��������ַ
            int maxLevel = 0;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (int.Parse(this.listWeblink.Items[i].SubItems[2].Text) > maxLevel)
                    maxLevel = int.Parse(this.listWeblink.Items[i].SubItems[2].Text);

            }

            //��Ϊ�����㼶0Ĭ��Ϊ�ɼ�ҳ�����ԣ�����Ҫ��1��ʼ����
            for (int i =1; i <= maxLevel; i++)
            {
                NaviLevels.Add(i.ToString ());
            }

            //ȡ��ҳ�ɼ���ҳ��
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
                                    //������󣬵�������
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
                #region ���Ӳɼ�����
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

                //���ӵ����㼶
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
                    //�Ƿ��ҳ
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

                //�ж��Ƿ������ݼӹ�����
                if (GRule.fieldDealRules != null && GRule.fieldDealRules.FieldRule.Count >0)
                {
                    //չ��״̬
                    item.ImageIndex = 17;
                    this.listWebGetFlag.Items.Add(item);

                    //��ʼ������ݼӹ�����
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
                    //�Ƿ��ҳ
                    this.listWebGetFlag.SelectedItems[0].SubItems[11].Text = "-1";
                    this.listWebGetFlag.SelectedItems[0].SubItems[12].Text = GRule.MultiPageName;
                }
                //else if (GRule.gRuleByPage == cGlobalParas.GatherRuleByPage.NonePage)
                //{
                //    //�Ƿ��ҳ
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

                //��ɾ��ԭ�е����ݼӹ�������ʾ
                int index = this.listWebGetFlag.SelectedItems[0].Index;

                DelDealRule(oldFileName, index);

                string gName = GRule.gName;

                AddDealRule(gName, index, GRule);

                //�޸Ĵ��ֶε��������������Ʒ����˱仯������Ҫ�����޸�����
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
                MessageBox.Show ("��ѡ����Ҫ�޸ĵĲɼ�����","����� ��Ϣ",MessageBoxButtons.OK ,MessageBoxIcon.Information );
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

            //ȡ��󵼺��������ַ
            int maxLevel = 0;
            for (int i = 0; i < this.listWeblink.Items.Count; i++)
            {
                if (int.Parse(this.listWeblink.Items[i].SubItems[2].Text) > maxLevel)
                    maxLevel = int.Parse(this.listWeblink.Items[i].SubItems[2].Text);

            }

            //��Ϊ�����㼶0Ĭ��Ϊ�ɼ�ҳ�����ԣ�����Ҫ��1��ʼ����
            for (int i = 1; i <= maxLevel; i++)
            {
                NaviLevels.Add(i.ToString());
            }

            //ȡ��ҳ�ɼ���ҳ��
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
                                    //������󣬵�������
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

                //ɾ���˲ɼ������µ��������
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

            //�˳�ǰ��Ҫ����Ƿ����ڲ�����ַ���Ƿ����ڲ��Բɼ�����
            if (m_testNaving == true)
            {
                MessageBox.Show("����ֹͣ������ַ���������˳��ɼ�����༭���ڣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancel = true;
                return;
            }

            if (m_IsGathering ==true)
            {
                MessageBox.Show("����ֹͣ�ɼ����Բ��������˳��ɼ�����༭���ڣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        #region �����޸ı�����
        private void tTask_TextChanged(object sender, EventArgs e)
        {
            ModifyTitle("�½��ɼ�����" + this.tTask.Text);
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
                this.saveFileDialog1.Title = "�����뵼��Word���ļ�����";

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
                MessageBox.Show("�Զ�����Sql���������ֹ�����Sql��䣡", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (tc == null)
            {
                MessageBox.Show("�Զ�����Sql���������ֹ�����Sql��䣡", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (this.listWebGetFlag.Items[j].SubItems[1].Text == "�ı�" || this.listWebGetFlag.Items[j].SubItems[1].Text == "Text")
                    strColumnsValue += "'{" + this.listWebGetFlag.Items[j].Text + "}',";

            }

            //�ж��Ƿ�����ɼ���Url �� ʱ��
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
                    if (this.listWebGetFlag.Items[j].SubItems[2].Text == "�ı�" || this.listWebGetFlag.Items[j].SubItems[2].Text == "Text")
                    {
                        strColumns += this.listWebGetFlag.Items[j].Text + ",";
                        strColumnsValue += "'{" + this.listWebGetFlag.Items[j].Text + "}',";
                    }
                }
            }

            //�ж��Ƿ�����ɼ���Url �� ʱ��
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
        /// ����Զ�̵Ĳɼ�������Ϣ
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
        /// ���汾�صĲɼ�������Ϣ
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
                //�½�һ������
                t.New();

                eTask et = new eTask();
                t.TaskEntity = FillTask(et);

                if (this.comTaskClass.Tag == null)
                    tClassPath = NetMiner.Constant.g_TaskPath;
                else
                    tClassPath = this.comTaskClass.Tag.ToString();

                //�������Ƿ����˱仯
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
                        //ɾ��ԭ�е�index�ļ���Ϣ
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
                //    //�������ļ�����
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
            
        //    //������ҳ����
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
        /// ����ѡ������ݹ������ð�ť��ʾ
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

            //��ʼ�ж��Ƿ�����˼ӹ�������������������ǲ���ʾ������������
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
            //��ʼ�ж������Ƶ�λ��
            for (int i = index-1; i >= 0; i--)
            {
                if (this.listWebGetFlag.Items[i].Name =="")
                {
                    pIndex = this.listWebGetFlag.Items[i].Index;
                    break;
                }
            }

            //��ʼɾ����ʾ
            if (isShowDeal == true)
            {
                //ɾ���ӹ�����
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

            //��ʼ�ж��Ƿ�����˼ӹ�������������������ǲ���ʾ������������
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

            //��ʼ�ж������Ƶ�λ��
            for (int i = index+1;i<this.listWebGetFlag.Items.Count ; i++)
            {
                if (this.listWebGetFlag.Items[i].Name =="")
                {
                    pIndex = this.listWebGetFlag.Items[i].Index;
                    pName = this.listWebGetFlag.Items[i].Text;
                    break ;
                }
            }

            //��ʼ������һ���ɼ��������Ƿ��мӹ�����
            for(int i=pIndex+1 ;i<this.listWebGetFlag.Items.Count ; i++)
            {
                if (this.listWebGetFlag.Items[i].Name.StartsWith(pName ))
                {
                    pDealCount++;
                }
            }

            pIndex = pIndex + pDealCount;

            //��ʼɾ����ʾ
            if (isShowDeal == true)
            {
                //ɾ���ӹ�����
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
        //    //������������
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
        //    //������������
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
                MessageBox.Show(ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TestGatherRule()
        {
            //if (this.txtWeblinkDemo.Text.Trim() == "")
            //{
            //    MessageBox.Show("����������Ҫ���Եĵ�����ַ���ٽ��й�����ԣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    this.txtWeblinkDemo.Focus();
            //    return;
            //}

            if (this.listWebGetFlag.Items.Count == 0)
            {
                MessageBox.Show("�������Ӳɼ����ݵĹ����ٽ��й���Ĳ��ԣ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            cGlobalParas.WebCode urlCode = cGlobalParas.WebCode.auto;
            if (this.IsUrlEncode.Checked == true)
                urlCode =EnumUtil.GetEnumName<cGlobalParas.WebCode>(this.comUrlEncode.SelectedItem.ToString());


            ////�ж��Ƿ��Ѿ���ȡ��ʾ����ַ�����û�У��������ȡ
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

             //���Ӳɼ��ı�־
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

            //�����û�ָ����ҳ���ȡλ�ù���������ʽ
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
                    //        //������󣬵���������Ҫ�ó������ִ����ȥ
                    //    }
                    //}
                    f1.rtbRegex.Text = strCut;

                    //��ʶ�Ѿ�ȥ���˻س�����
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
        //    //�ж��Ƿ��Ѿ���ȡ��ʾ����ַ�����û�У��������ȡ
          

          
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
            //ȡ��󵼺��������ַ
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

        #region ������datatable�����ݺϲ���һ��
        //��datarow��datatable�ϲ���һ�� ����һ��һ��N�Ĺ�ϵ
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
            //��dtr1 datarow ת����Table
            DataTable dt1 = dtr1.Table.Clone();
            dt1.ImportRow(dtr1);

            //�ж�dt2���Ƿ�����dtr1�ظ����У�����У���ɾ����ɾ��Ŀ��Ϊdt1

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

            //����һ�����ϵ����ݱ�
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

        #region �ϲ����ݲ���
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
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("���ӱ���������", null, null, "rmenuDBMasterPK"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("���ӱ����������", null, null, "rmenuDBDetailFK"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("�Զ���� {AutoCode:�Զ�����ֶ�����}", null, null, "rmenuDBAutoID"));
            //this.rmenuInsertDBField.Items.Add(new ToolStripMenuItem("��ǰʱ��", null, null, "rmenuDBCurTime"));
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
        /// ���÷������ݽ�����ʾ
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
        /// ���ò��������ݽ�����ʾ
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
        /// ����ֱ�����
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
        /// ������ؿ�
        /// </summary>
        private void ClearUrlsDB()
        {
            if (MessageBox.Show("�Ƿ�ɾ���˲ɼ���������ؿ⣬ɾ���������������½������ݲɼ���", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
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
                MessageBox.Show("ɾ�����ؿⷢ�����󣬴�����Ϣ��" + ex.Message, "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            MessageBox.Show("ɾ�����ؿ�ɹ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);


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
        //    //�Ȼ�ȡ��������ַ
        //    List<string> urls = GetDemoUrl();

        //    if (urls==null || urls.Count ==0)
        //        return ;

        //    string url = urls[0].ToString();

        //    //���ӵ������򣬾����Ƕ�ҳ�������ǰ��յ������߼����д���

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
        //        MessageBox.Show(rm.GetString("Error6") + " ��ҳ����ƥ������" + DemoUrl, rm.GetString("MessageboxInfo"), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    ModifyTitle("�½��ɼ�����");
                    break;
                case cGlobalParas.FormState.Edit:
                    //�༭״̬���������޸ķ���
                    ModifyTitle("�޸Ĳɼ�����" + this.tTask.Text);

                    break;
                case cGlobalParas.FormState.Browser:
                    ModifyTitle("����ɼ�����" + this.tTask.Text);
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

       

        #region ί�д��� ���ں�̨�̵߳��� ����UI�̵߳ķ���������

        delegate void bindvalue(object Instance, string Property, object value);
        delegate object invokemethod(object Instance, string Method, object[] parameters);
        delegate object invokepmethod(object Instance, string Property, string Method, object[] parameters);
        delegate object invokechailmethod(object InstanceInvokeRequired, object Instance, string Method, object[] parameters);

        /// <summary>
        /// ί�����ö�������
        /// </summary>
        /// <param name="Instance">����</param>
        /// <param name="Property">������</param>
        /// <param name="value">����ֵ</param>
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
        /// ί��ִ��ʵ���ķ������������붼��Public ��������
        /// </summary>
        /// <param name="Instance">��ʵ��</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ί��ִ��ʵ���ķ���
        /// </summary>
        /// <param name="InstanceInvokeRequired">����ؼ�����</param>
        /// <param name="Instance">��Ҫִ�з����Ķ���</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ί��ִ��ʵ�������Եķ���
        /// </summary>
        /// <param name="Instance">��ʵ��</param>
        /// <param name="Property">������</param>
        /// <param name="Method">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
        /// ��ȡʵ��������ֵ
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="PropertyName">������</param>
        /// <returns>����ֵ</returns>
        private static object GetPropertyValue(object ClassInstance, string PropertyName)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            return myPropertyInfo.GetValue(ClassInstance, null);
        }
        /// <summary>
        /// ����ʵ��������ֵ
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="PropertyName">������</param>
        private static void SetPropertyValue(object ClassInstance, string PropertyName, object PropertyValue)
        {
            Type myType = ClassInstance.GetType();
            PropertyInfo myPropertyInfo = myType.GetProperty(PropertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            myPropertyInfo.SetValue(ClassInstance, PropertyValue, null);
        }

        /// <summary>
        /// ִ��ʵ���ķ���
        /// </summary>
        /// <param name="ClassInstance">��ʵ��</param>
        /// <param name="MethodName">������</param>
        /// <param name="parameters">�����б�</param>
        /// <returns>����ֵ</returns>
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
                MessageBox.Show("�������һ����ַ���ٽ�����ַ�������Թ�����", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                if (MessageBox.Show("��ǰ������ַ���ڲ����Ҹ��ݲ������������ϴ󣬽���ֻ�Ե�һ������ֵ����ַ���в��ԣ�ȷ����", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    isAll = false ;
            }

            this.treeTestUrl.Nodes.Clear();

            //�����ַ����
            List<eWebLink> wLinks = new List<eWebLink>();
            FillWebLinks(ref wLinks);

            ExportGatherLog(cGlobalParas.LogType.Info, "��ȡָ�����Խ�������ַ��" + cItem.Text );

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
                SetValue(this.button2, "Text", "������ַ����");
                SetValue(this.button2, "Enabled", true);

                SetValue(this.button5, "Text", "��������");

                m_testNaving = false;

                string url = FillDemoUrl(this.treeTestUrl.Nodes[0]);

                InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "��ַ�������Խ���" });

                
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
                        MessageBox.Show("�޷�ճ���ɼ���ַ���������", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    this.listWeblink.Items.Add(item);
                    this.IsSave.Text = "true";
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("ճ������������Ϣ:" + ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                       
                            MessageBox.Show("�޷�ճ���ɼ��������������", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        
                    }

                    if (item.SubItems.Count!=2)
                    {
                        string gName = item.Text;
                        string gNewName=item.Text + "����";
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
                       //ճ���������ݼӹ�����
                       //�ȼ��㵱ǰ�������Ǹ��ɼ�����
                        int index = 0;
                        int startindex = 0;
                        if (this.listWebGetFlag.SelectedItems.Count == 0)
                        {
                            //��ǰû��ѡ����ճ�������һ��
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

                        //��ʼ����˲ɼ��������ж��ٸ��ӹ�����
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
                        
                        //����Ӧ���ǲ��뵽���һ��
                        if (gCount == 0)
                        {
                            //��ʾ��ǰ�Ĳɼ�������û�мӹ�����
                            this.listWebGetFlag.Items[index].ImageIndex=17;
                        }
                        this.listWebGetFlag.Items.Insert(index + gCount+1, item);

                        //���ӳɹ��󣬿�ʼ��Ӽ�������
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
                    MessageBox.Show("ճ������������Ϣ:" + ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                        this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[1].Value = "{�ֹ�����}";
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
                MessageBox.Show("δ����ȷʶ��˷�������򷢲����򲻷��ϵ����Ҫ�󣬵���ʧ�ܣ�", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            this.openFileDialog1.Title = "��ѡ�����ļ�";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "���(*.dll)|*.dll";
          

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //this.txtPluginsCookie.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdBrowserPlugins2_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "��ѡ�����ļ�";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "���(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPluginsDeal.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdBrowserPlugins3_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "��ѡ�����ļ�";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "���(*.dll)|*.dll";


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

       

        //���Բ����ʹ��
        //private void button15_Click(object sender, EventArgs e)
        //{
        //    this.txtTestPluginsLog.Text = "�������Բ������ȴ�����ĵ��ý��......" + "\r\n";

        //    #region ���Ե�¼���
        //    if (this.IsPluginsCookie.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();
        //            if (this.txtPluginsCookie.Text.Trim() == "")
        //            {
        //                MessageBox.Show("���������ļ���ַ��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsCookie.Focus();
        //                return;
        //            }

        //            if (this.listWeblink.Items.Count ==0)
        //            {
        //                MessageBox.Show("������ɼ���ַ���ٽ��е�¼����Ĳ��Թ�����", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //            cUrlAnalyze u = new cUrlAnalyze(Program.getPrjPath());
        //            List<string> urls= u.SplitWebUrl(this.listWeblink.Items[0].Text);
        //            if (urls.Count  == 0)
        //            {
        //                MessageBox.Show("���õĲɼ���ַ�д����޷����ԣ����Ƚ����޸ģ�", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                return;
        //            }
        //            string loginUrl = urls[0];
        //            string cookie = rPlugin.CallGetCookie(loginUrl , this.txtPluginsCookie.Text);
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "ͨ��������ص�Cookie��" + cookie + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "ͨ�������ȡCookieʧ�ܣ�������Ϣ��" + ex.Message  + "\r\n";
        //        }
        //    }
        //    #endregion

        //    #region ���ݴ���������
        //    if (this.IsPluginsDealData.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();

        //            if (this.txtPluginsDeal.Text.Trim() == "")
        //            {
        //                MessageBox.Show("���������ļ���ַ��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsDeal.Focus();
        //                return;
        //            }

        //            DataTable tmp = (DataTable)this.dTestData.DataSource;
        //            DataTable d = rPlugin.CallDealData(tmp, this.txtPluginsDeal.Text);
        //            this.dTestData.DataSource = d;
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "��ͨ�����������ݡ�ҳ�鿴���ݼӹ��Ľ��" + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "ͨ������Բɼ����ݽ��б༭ʧ�ܣ�������Ϣ��" + ex.Message + "\r\n";
        //        }
        //    }
        //    #endregion

        //    #region ���ݷ����������
        //    if (this.raPublishByPlugin.Checked == true)
        //    {
        //        try
        //        {
        //            NetMiner.Gather.Plugin.cRunPlugin rPlugin = new NetMiner.Gather.Plugin.cRunPlugin();

        //            if (this.txtPluginsPublish.Text.Trim() == "")
        //            {
        //                MessageBox.Show("���������ļ���ַ��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        //                this.txtPluginsPublish.Focus();
        //                return;
        //            }

        //            DataTable tmp = (DataTable)this.dTestData.DataSource;
        //            rPlugin.CallPublishData(tmp, this.txtPluginsPublish.Text);
        //            rPlugin = null;

        //            this.txtTestPluginsLog.Text += "ͨ������������ݳɹ�������ݲ����������Է��������ݽ�����֤��" + "\r\n";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            this.txtTestPluginsLog.Text += "ͨ�������������ʧ�ܣ�������Ϣ��" + ex.Message + "\r\n";
        //        }
        //    }
        //    #endregion

        //    this.txtTestPluginsLog.Text += "���Բ����ɣ�";
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
                MessageBox.Show("��ǰ�汾��֧�ֲ�����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧�ֲ�����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            //    MessageBox.Show("����ѡ������Ȼ���ٽ��в�����ã�ѡ�������������ť��ѡ����dll�ļ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("����ѡ������Ȼ���ٽ��в�����ã�ѡ�������������ť��ѡ����dll�ļ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("����ѡ������Ȼ���ٽ��в�����ã�ѡ�������������ť��ѡ����dll�ļ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

            //����е�����Ҫɾ�������Ĺ���
            //ɾ���洢�ĵ�������

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
        /// ���÷������ݽ�����ʾ��ʽ
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
            this.PDataColumn.Items.Add("{�ֹ�����}");


            this.dataParas.Rows.Clear();

            if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(strType) == cGlobalParas.PublishTemplateType.Web)
            {
                this.groupBox14.Enabled = true;
                this.groupBox13.Enabled = false;

                //���ز���
                cTemplate t = new cTemplate(Program.getPrjPath ());
                t.LoadTemplate(tName);

                this.dataParas.Rows.Add();
                this.dataParas.Rows[this.dataParas.Rows.Count - 1].Cells[0].Value = "ϵͳ����";

                foreach (KeyValuePair<string, string> de in t.PublishParas)
                {
                    string ss = de.Value.ToString();
                    ss = ss.Replace("��", ":");
                    Regex re = new Regex("(?<={��������:).+?(?=})", RegexOptions.None);
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
                    ss = ss.Replace("��", ":");
                    Regex re = new Regex("(?<={�ϴ�����:).+?(?=})", RegexOptions.None);
                    MatchCollection mc = re.Matches(ss);
                    foreach (Match ma in mc)
                    {
                        if (ma.ToString() != "��ǰ�ϴ����ļ���")
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

                //�ֽ�sql����еĲ���
                for (int i = 0; i < t.sqlParas.Count; i++)
                {
                    string ss = t.sqlParas[i].Sql;
                    ss = ss.Replace("��", ":");
                    Regex re = new Regex("(?<={��������:).+?(?=})", RegexOptions.None);
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
                MessageBox.Show("����ѡ����Ҫʹ�õķ���ģ�� !", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show(ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataParas_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("��ѡ��ķ��������뵱ǰ���õĲɼ�����ƥ�䣬������ѡ�񷢲����򣬴˴�����Ϣ�п��ܻ�����֣�", "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
        //        MessageBox.Show("��ǰ�汾��֧�ִ����������ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("�ļ���ʧ�ܣ�������Ϣ��" + ex.Message, "����� ����", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                //������ַ����
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
                    MessageBox.Show("���Ǳ�׼��Url��ַ��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Error);

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

        #region ������ַ���Ե����νṹ����־�Ľ�����ʾ
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
                MessageBox.Show("��ǰ�汾��֧�ֲɼ�����ִ�����ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧�ֲɼ�����ִ�����ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧�ֲɼ�����ִ�����ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧�ֲɼ�����ִ�����ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧�ֲɼ�����ִ�����ã����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("��ǰ�汾��֧��ֱ����⣬���ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                //�Ѿ�չ������Ҫ�۵����۵�����ɾ�����µ���������
                DelDealRule(this.listWebGetFlag.Items[index].Text, index);
               
                this.listWebGetFlag.Items[index].ImageIndex = 18;
            }
            else if (this.listWebGetFlag.Items[index].ImageIndex == 18)
            {
                string gName = this.listWebGetFlag.Items[index].Text;

                //�Ѿ��۵�����Ҫչ��

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

            //��ʼɾ��
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
            //����Ѱ���������Ĳɼ����ݹ�������
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
                MessageBox.Show("��ǰ�汾��֧�ֲ�����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            this.openFileDialog1.Title = "��ѡ�����ļ�";

            openFileDialog1.InitialDirectory = Program.getPrjPath();
            openFileDialog1.Filter = "���(*.dll)|*.dll";


            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.txtPluginsCodingByPB.Text = this.openFileDialog1.FileName;
            }
        }

        private void cmdSetPlugins4_Click(object sender, EventArgs e)
        {
            if (this.txtPluginsCodingByPB.Text.Trim() == "")
            {
                MessageBox.Show("����ѡ������Ȼ���ٽ��в�����ã�ѡ�������������ť��ѡ����dll�ļ���", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// <param name="isVisual">�Ƿ���ӻ���ȡ</param>
        /// <returns></returns>
        private string GetHtml(string Url, string referUrl,string startPos,string endPos,bool isVisual)
        {
            //��Ҫ�ж��Ƿ�Ϊ���ӻ���ȡ����ҳ



            #region HTTPHeader����


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

            //�ڴ˿��ƴ��������
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

            //ȥ��\r\n
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