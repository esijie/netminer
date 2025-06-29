using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SominerMonitor
{
    public partial class frmMain : Form
    {
        private bool m_isExit=false ;
        private cSubscriber sub = null;
        private bool m_isConnected = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

            //加载托盘图标
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ShowBalloonTip(2, "网络矿工引擎日志跟踪器", "用于监控网络矿工采集引擎运行状态", ToolTipIcon.Info);

            cXmlSConfig sCon = new cXmlSConfig();
            string serverIP = sCon.BindAddress;
            int serverPort = sCon.BindPort;
            bool isAutoConnect = sCon.AutoConnect;
            string gatherIP = sCon.GatherAddress;
            int gatherPort = sCon.GatherPort;
            sCon = null;

            this.labMathine.Text = serverIP;

            if (isAutoConnect == true)
            {
                ConnectServer(serverIP, serverPort);
               
            }
          
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="serverPort"></param>
        private void ConnectServer(string serverIP, int serverPort)
        {
            ExportServerLog("开始连接服务器......"); 
            string serviceUri = "net.tcp://" + serverIP + ":" + serverPort.ToString();

            try
            {
                sub = new cSubscriber(serviceUri);
                sub.SendMessage += this.on_SendMessage;

                sub.Subscribe();

                ExportServerLog("服务器连接成功！"); 
            }
            catch (Exception ex)
            {
                ExportServerLog("服务器连接失败，有可能目标服务不存在或服务未启动，\r\n错误信息：" + ex.Message );
                return;
            }

            m_isConnected = true;
            this.sta2.Text = "断开";
        }

        private void on_SendMessage(object sender, eMessageEvent e)
        {
            InvokeMethod(this, "ExportServerLog", new object[] { e.strInfo });
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_isExit == false)
            {
                this.Hide();
                e.Cancel = true;
            }
            else
            {
                if (sub!=null)
                    sub.Dispose();
            }
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            m_isExit = true;
            this.Close();
        }

        private void toolOpen_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        public void ExportServerLog(string log)
        {
            this.txtServerLog.Text = System.DateTime.Now.ToString() + ":" + log + "\r\n" + this.txtServerLog.Text;

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

        private void toolStripStatusLabel4_Click(object sender, EventArgs e)
        {
            frmConfig f = new frmConfig();
            f.ShowDialog();
            f.Dispose();
        }

        private void sta2_Click(object sender, EventArgs e)
        {
            if (m_isConnected == true)
            {
                sub.Dispose();
                m_isConnected = false;
                this.sta2.Text = "连接";
            }
            else
            {
                cXmlSConfig sCon = new cXmlSConfig();
                string serverIP = sCon.BindAddress;
                int serverPort = sCon.BindPort;
                bool isAutoConnect = sCon.AutoConnect;
                string gatherIP = sCon.GatherAddress;
                int gatherPort = sCon.GatherPort;
                sCon = null;
                ConnectServer(serverIP, serverPort);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }
    }
}
