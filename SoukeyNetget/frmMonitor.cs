using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Reflection;
using System.Resources;
using NetMiner.Gather;
using NetMiner.Gather.Radar ;
using NetMiner.Core.Log;
using NetMiner.Resource;
using NetMiner.Core.Radar.Entity;
using NetMiner.Core.Radar;
using NetMiner.Core.Event;

namespace MinerSpider
{
    public partial class frmMonitor : DockContent 
    {
        private ResourceManager rm;

        private cRadarControl m_RadarControl;
        private DataGridViewCellStyle m_RowStyleErr;

        //����һ��ֵ���ж��Ƿ񱣴��״���־
        private bool m_IsAutoSaveRadarLog = false;
        private int m_MaxLogNumber;

       
        public int MaxLogNumber
        {
            get { return m_MaxLogNumber; }
            set { m_MaxLogNumber = value; }
        }

        public bool IsAutoSaveRadarLog
        {
            get { return m_IsAutoSaveRadarLog; }
            set { m_IsAutoSaveRadarLog = value; }
        }

        public frmMonitor()
        {
            InitializeComponent();

            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load ("NetMiner.Resource"));

            m_RadarControl = new cRadarControl(Program.getPrjPath());

            m_RadarControl.RadarManage.RadarStarted += this.on_RadarStarted;
            m_RadarControl.RadarManage.RadarStop += this.on_RadarStop;
            m_RadarControl.RadarManage.Log += this.on_RadarLog;
            m_RadarControl.RadarManage.RadarWarning += this.on_RadarWarning;
            m_RadarControl.RadarManage.RadarCount += this.on_RadarCount;
            m_RadarControl.RadarManage.RadarState += this.on_RadarState;
        }

        //��ʼ���״����ݼ���Ӧ���¼���Ϣ
        public void IniData()
        {
            LoadRadarRule();
        }

        //���ü��ش����gridrows����ʵ��ʽ
        private void SetRowErrStyle()
        {
            this.m_RowStyleErr = new DataGridViewCellStyle();
            this.m_RowStyleErr.Font = new Font(DefaultFont, FontStyle.Italic);
            this.m_RowStyleErr.ForeColor = Color.Red;
        }

        public void StartRadar()
        {
            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud ||
                Program.SominerVersion == cGlobalParas.VersionType.Ultimate ||
             Program.SominerVersion == cGlobalParas.VersionType.Enterprise)
            {
                //���¼���һ���״�������Ϣ

                LoadRadarRule();

                m_RadarControl.StartRadar();
            }
            else
            {
                MessageBox.Show("��ǰ�汾��֧�������״���ݼ�أ����ܣ����ȡ��ȷ�İ汾��", "����� ��Ϣ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //this.toolmenuSilentRun.Checked = false;
                return;
            }
        }

        public void StopRadar()
        {
            m_RadarControl.StopRadar();
        }

        private void ShowRuleInfo()
        {

            try
            {
                this.dataTask.Columns.Clear();
                this.dataTask.Rows.Clear();
            }
            catch (System.Exception)
            {

            }

            #region �Ȳ���Ϊ�̶���ʾ
            DataGridViewImageColumn tStateImg = new DataGridViewImageColumn();
            tStateImg.HeaderText = ""; ;
            tStateImg.Width = 26;
            tStateImg.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            tStateImg.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(0, tStateImg);

            DataGridViewTextBoxColumn tID = new DataGridViewTextBoxColumn();
            tID.Name = rm.GetString("GridTaskID");
            tID.Width = 0;
            tID.Visible = false;
            this.dataTask.Columns.Insert(1, tID);

            //����״̬,����ʾ����
            DataGridViewTextBoxColumn tState = new DataGridViewTextBoxColumn();
            tState.Name = "״̬";
            tState.Width = 60;
            tState.Visible = false;
            this.dataTask.Columns.Insert(2, tState);

            //����ͨ���ж�Datagridview�����ݾͿ�֪����ǰ���νṹѡ��Ľڵ�
            //���ڿ���(����)������ʾ״̬
            DataGridViewTextBoxColumn tTreeNode = new DataGridViewTextBoxColumn();
            tTreeNode.HeaderText = "treeMenuName";
            tTreeNode.Visible = false;
            this.dataTask.Columns.Insert(3, tTreeNode);

            #endregion

            DataGridViewTextBoxColumn tName = new DataGridViewTextBoxColumn();
            tName.HeaderText = "��������";
            tName.Width = 80;
            this.dataTask.Columns.Insert(4, tName);

            DataGridViewTextBoxColumn tShowState = new DataGridViewTextBoxColumn();
            tShowState.HeaderText = "��ǰ״̬";
            tShowState.Width = 80;
            this.dataTask.Columns.Insert(5, tShowState);

            DataGridViewTextBoxColumn tRound = new DataGridViewTextBoxColumn();
            tRound.HeaderText = "�Ѽ�ش���";
            tRound.Width = 80;
            tRound.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(6, tRound);

            DataGridViewTextBoxColumn tUrls = new DataGridViewTextBoxColumn();
            tUrls.HeaderText = "�Ѵ�����ַ";
            tUrls.Width = 80;
            tUrls.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(7, tUrls);

            DataGridViewTextBoxColumn tGetUrls = new DataGridViewTextBoxColumn();
            tGetUrls.HeaderText = "���Ϲ�����ַ";
            tGetUrls.Width = 80;
            tGetUrls.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this.dataTask.Columns.Insert(8, tGetUrls);

            DataGridViewTextBoxColumn tInvaidType = new DataGridViewTextBoxColumn();
            tInvaidType.HeaderText = "ʧЧ����";
            tInvaidType.Width = 180;
            this.dataTask.Columns.Insert(9, tInvaidType);

            DataGridViewTextBoxColumn tDealType = new DataGridViewTextBoxColumn();
            tDealType.HeaderText = "����ʽ";
            tDealType.Width = 180;
            this.dataTask.Columns.Insert(10, tDealType);

            DataGridViewTextBoxColumn tWarningType = new DataGridViewTextBoxColumn();
            tWarningType.HeaderText = "Ԥ������";
            tWarningType.Width = 1100;

            this.dataTask.Columns.Insert(11, tWarningType);


        }

        private void frmMonitor_Load(object sender, EventArgs e)
        {
            //ShowRuleInfo();
        }

        private void frmMonitor_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
        }

        #region �¼�����
        private void on_RadarStarted(object sender, cRadarStartedArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("SetRadar", new object[] {true}));

            InvokeMethod(this, "ExportGatherLog", new object[] {cGlobalParas.LogType.Info, "�������Ϣ����״���" + System.DateTime.Now.ToString () + "�ɹ�������"  });

            if (e_ExportLog != null)
                e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Info,cGlobalParas.LogClass.Radar , System.DateTime.Now.ToString () , "����󹤼���״�����"));

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("�����", "����״�������"));
        }

        private void on_RadarStop(object sender, cRadarStopArgs e)
        {
            InvokeMethod(this, "ExportGatherLog", new object[] { cGlobalParas.LogType.Info, "�������Ϣ����״���" + System.DateTime.Now.ToString() + "ֹͣ��"  });
            
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("SetRadar",new object[] {false }));

            if (e_ExportLog != null)
                e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Info ,cGlobalParas.LogClass.Radar, System.DateTime.Now.ToString() , "����󹤼���״���ֹͣ"));

            if (e_ShowLogInfo != null)
                e_ShowLogInfo(this, new ShowInfoEvent("�����", "����״���ֹͣ"));
        }

        private void on_RadarLog(object sender, cRadarLogArgs e)
        {
            try
            {
                InvokeMethod(this, "ExportGatherLog", new object[] {  e.LogType, e.strLog });
            }
            catch (System.Exception ex)
            {
                if (e_ExportLog != null)
                    e_ExportLog(this, new ExportLogEvent(cGlobalParas.LogType.Error ,cGlobalParas.LogClass.Radar,System.DateTime.Now.ToString (), ex.Message));
            }
        }

        private void on_RadarWarning(object sender, cRadarMonitorWaringArgs e)
        {
            if (e_ExcuteFunction != null)
                e_ExcuteFunction(this, new ExcuteFunctionEvent("MonitorWarningByTrayIcon", new object[] { e.strWarning }));
        }

        private void on_RadarCount(object sender, cRadarCountArgs e)
        {
            for (int i = 0; i < this.dataTask.Rows.Count; i++)
            {
                if (this.dataTask.Rows[i].Cells[4].Value.ToString() == e.RadarName)
                {
                    this.dataTask.Rows[i].Cells[6].Value=e.RCount ;
                    this.dataTask.Rows[i].Cells[7].Value = e.GatheredCount;
                    this.dataTask.Rows[i].Cells[8].Value=e.Count ;
                    break;
                }
            }
        }

        private void on_RadarState(object sender, cRadarStateArgs e)
        {
            for (int i = 0; i < this.dataTask.Rows.Count; i++)
            {
                if (this.dataTask.Rows[i].Cells[4].Value.ToString() == e.RName)
                {
                    this.dataTask.Rows[i].Cells[2].Value =(int) e.RState;
                    this.dataTask.Rows[i].Cells[5].Value = e.RState.GetDescription();
                    break;
                }
            }
        }
        #endregion

        #region  ������÷���
        public void ExportGatherLog(cGlobalParas.LogType lType, string lText)
        {
            

            //������־

            if (this.m_IsAutoSaveRadarLog == true  )
            {
                try
                {
                    cSystemLog sl = new cSystemLog(Program.getPrjPath());
                    sl.WriteLog(lType, cGlobalParas.LogClass.Radar, System.DateTime.Now.ToString(), lText);
                    sl = null;
                }
                catch (System.Exception ex)
                {
                    this.myLog.AppendText(cGlobalParas.LogType.Error, rm.GetString("Info51") + ex.Message);
                }
               
            }

            int i = this.myLog.Lines.Length;

            if (i > this.m_MaxLogNumber)
            {
                this.myLog.Clear();
            }

            this.myLog.AppendText(lType, lText);
        }
        #endregion

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

        #region �����¼� ���ڸ���������������Ϣ
        private readonly Object m_eventLock = new Object();

        private event EventHandler<SetToolbarStateEvent> e_SetToolbarState;
        public event EventHandler<SetToolbarStateEvent> SetToolbarState
        {
            add { lock (m_eventLock) { e_SetToolbarState += value; } }
            remove { lock (m_eventLock) { e_SetToolbarState -= value; } }
        }

        private event EventHandler<ResetToolbarStateEvent> e_ResetToolbarState;
        public event EventHandler<ResetToolbarStateEvent> ResetToolbarState
        {
            add { lock (m_eventLock) { e_ResetToolbarState += value; } }
            remove { lock (m_eventLock) { e_ResetToolbarState -= value; } }
        }

        private event EventHandler<SetControlPropertyEvent> e_SetControlProperty;
        public event EventHandler<SetControlPropertyEvent> SetControlProperty
        {
            add { lock (m_eventLock) { e_SetControlProperty += value; } }
            remove { lock (m_eventLock) { e_SetControlProperty -= value; } }
        }

        private event EventHandler<ShowInfoEvent> e_ShowLogInfo;
        public event EventHandler<ShowInfoEvent> ShowLogInfo
        {
            add { lock (m_eventLock) { e_ShowLogInfo += value; } }
            remove { lock (m_eventLock) { e_ShowLogInfo -= value; } }
        }

        private event EventHandler<ExportLogEvent> e_ExportLog;
        public event EventHandler<ExportLogEvent> ExportLog
        {
            add { lock (m_eventLock) { e_ExportLog += value; } }
            remove { lock (m_eventLock) { e_ExportLog -= value; } }
        }

        private event EventHandler<ExcuteFunctionEvent> e_ExcuteFunction;
        public event EventHandler<ExcuteFunctionEvent> ExcuteFunction
        {
            add { lock (m_eventLock) { e_ExcuteFunction += value; } }
            remove { lock (m_eventLock) { e_ExcuteFunction -= value; } }
        }

        private event EventHandler<DelInfoEvent> e_DelInfo;
        public event EventHandler<DelInfoEvent> DelInfo
        {
            add { lock (m_eventLock) { e_DelInfo += value; } }
            remove { lock (m_eventLock) { e_DelInfo -= value; } }
        } 

        #endregion

        private void toolmenuStartRadar_Click(object sender, EventArgs e)
        {
            StartRadar();
        }

        private void toolmenuStopRadar_Click(object sender, EventArgs e)
        {
            StopRadar();
        }

        private void toolmenuNew_Click(object sender, EventArgs e)
        {
            if (e_ExcuteFunction !=null)
                e_ExcuteFunction (this,new ExcuteFunctionEvent ("NewRadar",null));
        }

        private void EditRadar()
        {
            if (this.dataTask.SelectedRows.Count == 0)
                return;

            if (this.dataTask.SelectedCells[2].Value.ToString() == ((int)cGlobalParas.MonitorState.Running).ToString ())
            {
                MessageBox.Show("�����״����������У��޷����б༭������", rm.GetString("MessageboxInfo"),MessageBoxButtons.OK ,MessageBoxIcon.Information );
                return;
            }

            try
            {
                frmRader f = new frmRader();
                f.FormState = cGlobalParas.FormState.Edit;
                f.LoadMonitorRule(this.dataTask.SelectedCells[4].Value.ToString());
                f.ShowDialog();
                f.Dispose();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(rm.GetString("Error43") + ex.Message , rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoadRadarRule();
        }

        private void toolmenuEdit_Click(object sender, EventArgs e)
        {
            EditRadar();
        }

        public void LoadRadarRule()
        {
            ShowRuleInfo();


            this.dataTask.DataSource = null;

            oRadarIndex ci = new oRadarIndex(Program.getPrjPath());
            IEnumerable<eRadarIndex> ers= ci.GetRules();

            foreach(eRadarIndex er in ers)
            {

                try
                {
                    dataTask.Rows.Add(imageList1.Images["radar"], er.ID,
                        (int)er.State,
                        "nodRadarRule", er.Name, er.State.GetDescription(), "0", "0", "0",
                        er.InvalidType.GetDescription(),
                        er.DealType.GetDescription(),
                        er.WarningType.GetDescription());
                }
                catch (System.Exception ex)
                {
                    dataTask.Rows.Add(imageList1.Images["error"], er.ID, er.State.GetDescription(), "nodRadarRule", er.Name, "0", "0", "0",
                      "",
                     ex.Message);
                    dataTask.Rows[dataTask.Rows.Count - 1].DefaultCellStyle = this.m_RowStyleErr;
                }
            }

            ci = null;


            this.dataTask.Sort(this.dataTask.Columns[4], ListSortDirection.Ascending);

            this.dataTask.ClearSelection();

        }

        private void dataTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (this.dataTask.SelectedCells.Count  == 0)
                return;

            switch (e.KeyCode)
            {
                case Keys.Delete:

                    if (DelRadarRule() == false)
                    {
                        e.SuppressKeyPress = true;
                        return;
                    }
                    break;

                default:
                    break;
            }
        }

        public void Del()
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

        private void toolmenuDel_Click(object sender, EventArgs e)
        {
            this.dataTask.Focus();
            SendKeys.Send("{Del}");
        }

        public bool DelRadarRule()
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return false;
            }

            if (this.dataTask.SelectedRows.Count == 1)
            {
                if (MessageBox.Show(rm.GetString("Quaere27") + this.dataTask.SelectedCells[4].Value + "\r\n",
                     rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }
            else
            {
                if (MessageBox.Show(rm.GetString("Quaere26"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return false;
                }
            }

            try
            {
                for (int index = 0; index < this.dataTask.SelectedRows.Count; index++)
                {
                    oRadar r = new oRadar(Program.getPrjPath());
                    r.DelRule(this.dataTask.SelectedRows[index].Cells[4].Value.ToString());
                    r = null;

                }
                return true;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, rm.GetString("MessageboxError"), MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return false;
            }


            
        }

        private void dataTask_DoubleClick(object sender, EventArgs e)
        {

            EditRadar();

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            //�ж��״�״̬

            if (this.m_RadarControl.RadarManage.IsStarted == true)
            {
                this.toolmenuStartRadar.Enabled = false;
                this.toolmenuStopRadar.Enabled = true;
                this.rMenuDelUrlDB.Enabled =false ;
            }
            else
            {
                this.toolmenuStartRadar.Enabled = true ;
                this.toolmenuStopRadar.Enabled = false;
                this.rMenuDelUrlDB.Enabled =true ;
            }
        }

        private void frmMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_RadarControl.RadarManage.RadarStarted -= this.on_RadarStarted;
            m_RadarControl.RadarManage.RadarStop -= this.on_RadarStop;
            m_RadarControl.RadarManage.Log -= this.on_RadarLog;
            m_RadarControl.RadarManage.RadarWarning -= this.on_RadarWarning;
            m_RadarControl.RadarManage.RadarCount -= this.on_RadarCount;
            m_RadarControl.RadarManage.RadarState -= this.on_RadarState;

            m_RadarControl.StopRadar();

            m_RadarControl = null;
        }

        private void dataTask_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            ExportGatherLog(cGlobalParas.LogType.Error, e.ThrowException.ToString());
        }

        private void myLog_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.LinkText);
        }

        private void dataTask_Enter(object sender, EventArgs e)
        {
            if (e_DelInfo != null)
                e_DelInfo(this, new DelInfoEvent("MonitorTask"));
        }

        private void rMenuDelUrlDB_Click(object sender, EventArgs e)
        {
            if (this.dataTask.SelectedRows.Count == 0)
            {
                return ;
            }

            if (MessageBox.Show("ɾ���˹����ؼ�¼���Ѽ�ص������ݽ��ᱻ�ٴμ�⵽���Ƿ������", "����� ѯ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            oRadar r = new oRadar(Program.getPrjPath());
            r.DelRuleDB(this.dataTask.SelectedRows[0].Cells[4].Value.ToString());
            r = null;

                
        }
    }
}