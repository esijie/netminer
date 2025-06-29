using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.Resources;
using System.Reflection;
using NetMiner.Gather;
using NetMiner.Resource;
using System.Threading;

namespace SoukeyDataPublish
{
    public partial class frmMain : Form
    {
        //����һ��������Դ�ļ��ı���
        private ResourceManager rm;

        private frmToolbox m_Toolbox = new frmToolbox();

        //ϵͳ��ʼ������Ĳ���ֵ��Ĭ��Ϊ��
        private cGlobalParas.DatabaseType m_dbType ;
        private string m_dbCon = "";
        private string m_strSql = "";

        //����һ������ļ��ϣ��������ӻ���ɾ��һ�����ݴ���
        List<frmDataOp> m_fDatas = new List<frmDataOp>();

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(cGlobalParas.DatabaseType dType, string strCon,string sql)
        {
            InitializeComponent();

            m_dbType = dType;
            m_dbCon = strCon;
            m_strSql = sql;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //��ʼ����Դ�ļ��Ĳ���
            rm = new ResourceManager("NetMiner.Resource.Resources.globalUI", Assembly.Load("NetMiner.Resource"));

            //if (Program.SominerVersion == cGlobalParas.VersionType.Free && DateTime.Compare(DateTime.Now, Program.Deadline) > 0)
            //{
            //    //��ʾ�������ѹ�
            //    MessageBox.Show(rm.GetString("Error27"), rm.GetString("MessageboxQuaere"), MessageBoxButtons.OK, MessageBoxIcon.Error);

            //    this.Dispose();
            //    return;

            //}

            if (Program.SominerVersion == cGlobalParas.VersionType.Cloud)
            {
                //��ʾ�汾�Ѿ��������汾�Ѿ����������ٷ���վ������Ϣ���Ӷ���֤�汾�ĺϷ���
                //������֤�Ķ�ʱ�����ɴ˶�ʱ�����к�̨����������Ϊ�Ƿ�����ǿ��ϵͳ�˳�

                this.Text += " [" + rm.GetString("Info2250") + "]";
            }
            else if (Program.SominerVersion == cGlobalParas.VersionType.Free)
            {
                //TimeSpan ts = Program.Deadline - DateTime.Now;
                //int d = ts.Days;

                //this.Text += " [" + rm.GetString("Info2251") + " " + d.ToString() + rm.GetString("Day") + "]";
            }

            //ע���¼� �����ݹ�����
            m_Toolbox.LoadDataEvent += this.ToolboxLoadData;
            m_Toolbox.Show(dockPanel);

            //�жϳ��������Ƿ�����˲����������������������
            if (m_dbCon != "")
                OpenData(this.m_dbType, this.m_dbCon, this.m_strSql);
        }

        private void ToolboxLoadData(object sender, LoadDataEventArgs e)
        {
            InvokeMethod(this, "LoadData", new object[] { e.node}); 
        }

        private void menuAbout_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
            f.Dispose();
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            rm = null;
            this.Dispose();
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
            Environment.Exit(0);
        }

        private void toolOpenData_Click(object sender, EventArgs e)
        {
            LoadData(m_Toolbox.SelectNode);
        }

        public void LoadData(TreeNode node)
        {

            //��ʾ�ȴ��Ĵ��� 
            frmWaiting fWait = new frmWaiting("���ڼ���������ȴ�...");
            new Thread((ThreadStart)delegate
            {
                Application.Run(fWait);
            }).Start();

            try
            {
                switch (node.Name)
                {
                    case "AccessData":
                        GetOtherData(cGlobalParas.DatabaseType.Access);

                        break;
                    case "SqlServerData":
                        GetOtherData(cGlobalParas.DatabaseType.MSSqlServer);

                        break;
                    case "MySqlData":
                        GetOtherData(cGlobalParas.DatabaseType.MySql);

                        break;
                    default:
                        if (node.Name.Substring(0, 1) == "C")
                        {
                            OpenData(cGlobalParas.DatabaseType.SoukeyData, node.Tag.ToString(), "");

                        }
                        else if (node.Name.StartsWith("OtherData"))
                        {
                            string strCon = node.Parent.Name;
                            string strSql = node.Name.Substring(10, node.Name.Length - 10);

                            switch (node.Parent.Parent.Name)
                            {
                                case "AccessData":
                                    OpenData(cGlobalParas.DatabaseType.Access, strCon, strSql);
                                    break;
                                case "SqlServerData":
                                    OpenData(cGlobalParas.DatabaseType.MSSqlServer, strCon, strSql);
                                    break;
                                case "MySqlData":
                                    OpenData(cGlobalParas.DatabaseType.MySql, strCon, strSql);
                                    break;

                            }
                        }
                        else if (node.Name.StartsWith("RTable-"))
                        {
                            string strCon = node.Parent.Tag.ToString();
                            string strSql = "select * from " + node.Text;

                            if (node.Parent.Name.StartsWith("MSSql-") || node.Parent.Name.StartsWith("MSSqlDB-"))
                            {
                                OpenData(cGlobalParas.DatabaseType.MSSqlServer, strCon, strSql);
                            }
                            else if (node.Parent.Name.StartsWith("Access-"))
                            {
                                OpenData(cGlobalParas.DatabaseType.Access, strCon, strSql);
                            }

                        }


                        break;
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                if (fWait.IsHandleCreated)
                    fWait.Invoke((EventHandler)delegate { fWait.Close(); });
                fWait = null;
            }
        }

        private void GetOtherData(cGlobalParas.DatabaseType dType)
        {
            frmGetData fs = new frmGetData();
            fs.DataType = dType;
            fs.rDataSource = new frmGetData.ReturnDataSource(this.getDataSource);
            fs.ShowDialog();
            fs.Dispose();
        }

        private void getDataSource(string strCon, string sql, cGlobalParas.DatabaseType dType)
        {
            if (strCon == "" || sql == "")
                return;

            TreeNode newNode = new TreeNode();
            newNode.Tag = sql;
            newNode.Text = strCon;
            newNode.Name = strCon;

            TreeNode newChildNode = new TreeNode();

            switch (dType)
            {
                case cGlobalParas.DatabaseType.Access:
                    newNode.ImageIndex = 5;
                    newNode.SelectedImageIndex = 5;

                    newChildNode.Text = sql;
                    newChildNode.Name = "OtherData:" + sql;
                    newChildNode.ImageIndex = 5;
                    newChildNode.SelectedImageIndex = 5;
                    newNode.Nodes.Add(newChildNode);

                    m_Toolbox.AddNode("Access", newNode);
                    newNode = null;

                    break;
                case cGlobalParas.DatabaseType.MSSqlServer:
                    newNode.ImageIndex = 5;
                    newNode.SelectedImageIndex = 5;

                    newChildNode.Text = sql;
                    newChildNode.Name = "OtherData:" + sql;
                    newChildNode.ImageIndex = 5;
                    newChildNode.SelectedImageIndex = 5;
                    newNode.Nodes.Add(newChildNode);
                    m_Toolbox.AddNode("SqlServer", newNode);
                    break;
                case cGlobalParas.DatabaseType.MySql:
                    newNode.ImageIndex = 5;
                    newNode.SelectedImageIndex = 5;

                    newChildNode.Text = sql;
                    newChildNode.Name = "OtherData:" + sql;
                    newChildNode.ImageIndex = 5;
                    newChildNode.SelectedImageIndex = 5;
                    newNode.Nodes.Add(newChildNode);
                    m_Toolbox.AddNode("MySql", newNode);
                    break;
            }

            newNode = null;

            OpenData(dType, strCon, sql);
        }

        private void OpenData(cGlobalParas.DatabaseType dType,string strCon,string  sql)
        {
            frmDataOp f = new frmDataOp();
            f.Show(dockPanel);
            f.LoadData(dType, strCon,sql);
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
            object inst;

            if (iType.Name.ToString() == "ToolStripButton" || iType.Name.ToString() == "ToolStripStatusLabel" || iType.Name.ToString() == "DataGridView" || iType.Name.ToString() == "ToolStripProgressBar")
            {
                inst = this.toolStrip1;
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

        private void toolCloseData_Click(object sender, EventArgs e)
        {
            Form f = this.ActiveMdiChild;
            if (f!=null )
                f.Dispose();
        }

        private void menuExit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void menuOpenData_Click(object sender, EventArgs e)
        {
            LoadData(m_Toolbox.SelectNode);
        }

        private void menuCloseData_Click(object sender, EventArgs e)
        {
            Form f = this.ActiveMdiChild;
            if (f != null)
                f.Dispose();
        }

        private void menuCloseAll_Click(object sender, EventArgs e)
        {
            foreach (Form f in this.MdiChildren)
            {
                f.Dispose();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmMergeData f = new frmMergeData();
            f.ShowDialog();
            f.Dispose();

        }
    }
}