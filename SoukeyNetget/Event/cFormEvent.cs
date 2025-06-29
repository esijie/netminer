using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using NetMiner.Gather;
using NetMiner.Resource;

///窗体响应事件，主要是由于主窗体采用了weifengluo.winformUI控件，将窗体部分改为多窗体
///内容，窗体彼此之间的操作响应则通过事件来完成，所以，定义了此内容
///功能：窗体响应时间
///完成时间：2010-6-21
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：
///版本：02.10.00
///修订：无
namespace MinerSpider
{
    public class cFormEvent : EventArgs
    {

        public cFormEvent()
        {

        }

        /// <param name="cancel">是否取消事件</param>
        public cFormEvent(bool cancel)
        {
            m_Cancel = cancel;
        }

        private bool m_Cancel;
        /// <summary>
        /// 是否取消事件
        /// </summary>
        public bool Cancel
        {
            get { return m_Cancel; }
            set { m_Cancel = value; }
        }
    }

    //通过树形菜单加载数据时使用
    public class TreeNodeMouseClickEvent : cFormEvent
    {
        public TreeNodeMouseClickEvent(TreeNode node)
        {
            m_node = node;
        }

        private TreeNode m_node;
        public TreeNode node
        {
            get { return m_node; }
            set { m_node = value; }
        }

    }

    //通过树形菜单和task grid 来触发设置工具栏的状态
    public class SetToolbarStateEvent : cFormEvent
    {
        public SetToolbarStateEvent(int gridrowCount,cGlobalParas.TaskState taskState)
        {
            m_gridrowCount = gridrowCount;
            m_TaskState = taskState;
        }

        private int m_gridrowCount;
        public int gridrowCount
        {
            get { return m_gridrowCount; }
            set { m_gridrowCount = value; }
        }

        private cGlobalParas.TaskState m_TaskState;
        public cGlobalParas.TaskState TaskState
        {
            get { return m_TaskState; }
            set { m_TaskState = value; }
        }
    }

    public class SetToolbarRemote : cFormEvent
    {
        public SetToolbarRemote(int gridrowCount,string selectNode)
        {
            m_gridrowCount = gridrowCount;
            m_selectNode = selectNode;
        }

        private int m_gridrowCount;
        public int gridrowCount
        {
            get { return m_gridrowCount; }
            set { m_gridrowCount = value; }
        }

        private string m_selectNode;
        public string selectNode
        {
            get { return m_selectNode; }
            set { m_selectNode = value; }
        }
    }

    //通过树形菜单和task grid 来触发设置工具栏的状态
    public class ResetToolbarStateEvent : cFormEvent
    {
        public ResetToolbarStateEvent()
        {
           
        }
    }

    //通过事件触发设置主窗体某个控件的属性值
    public class SetControlPropertyEvent : cFormEvent
    {
        public SetControlPropertyEvent(string ControlName, string Property, string Value)
        {
            m_cName = ControlName;
            m_Pro = Property;
            m_Value = Value;
        }

        private  string m_cName;
        public string ControlName
        {
            get { return m_cName; }
            set { m_cName = value; }
        }

        private string m_Pro;
        public string Property
        {
            get { return m_Pro; }
            set { m_Pro = value; }
        }

        private string m_Value;
        public string Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }

    public class ExcuteFunctionEvent : cFormEvent
    {
        public ExcuteFunctionEvent(string FunctionName, object[] parameters)
        {
            m_fName = FunctionName;
            m_Paras = parameters;
        }

        private string m_fName;
        public string FunctionName
        {
            get { return m_fName; }
            set { m_fName = value; }
        }

        private object[] m_Paras;
        public object[] Parameters
        {
            get { return m_Paras; }
            set { m_Paras = value; }
        }
    }

    //提醒图标，用于显示托盘图标的信息
    public class ShowInfoEvent : cFormEvent
    {
        public ShowInfoEvent(string Title, string strInfo)
        {
            m_Title = Title;
            m_strInfo = strInfo;
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_strInfo;
        public string strInfo
        {
            get { return m_strInfo; }
            set { m_strInfo = value; }
        }
    }

    public class ExportLogEvent : cFormEvent
    {
        public ExportLogEvent(cGlobalParas.LogType lType,  cGlobalParas.LogClass lClass,string Dtime,  string strLog)
        {
            m_lType = lType;
            m_lClass = lClass;
            m_Dtime = Dtime;
            m_strLog = strLog;
        }

        private cGlobalParas.LogType m_lType;
        public cGlobalParas.LogType lType
        {
            get { return m_lType; }
            set { m_lType = value; }
        }

        private cGlobalParas.LogClass m_lClass;
        public cGlobalParas.LogClass lClass
        {
            get { return m_lClass; }
            set { m_lClass = value; }
        }

        private string m_Dtime;
        public string Dtime
        {
            get { return m_Dtime; }
            set { m_Dtime = value; }
        }

        private string m_strLog;
        public string strLog
        {
            get { return m_strLog; }
            set { m_strLog = value; }
        }
    }

    //更新当前删除信息，确保主窗体删除按钮可以删除到想要的内容
    public class DelInfoEvent : cFormEvent
    {
        public DelInfoEvent(string DelName)
        {
            m_DelName = DelName;
        }

        private string m_DelName;
        public string DelName
        {
            get { return m_DelName; }
            set { m_DelName = value; }
        }
    }

    //删除datagrid行
    public class DelDatagridRowEvent : cFormEvent
    {
        public DelDatagridRowEvent(DataGridViewRow DelRow)
        {
            m_DelRow = DelRow;
        }

        private DataGridViewRow m_DelRow;
        public DataGridViewRow DelRow
        {
            get { return m_DelRow; }
            set { m_DelRow = value; }
        }
    }

    //更新主界面的下载速率
    public class DownloadSpeedEvent : cFormEvent
    {
        public DownloadSpeedEvent(Single dSpeed)
        {
            m_dSpeed = dSpeed;
        }

        private Single m_dSpeed;
        public Single dSpeed
        {
            get { return m_dSpeed; }
            set { m_dSpeed = value; }
        }
    }

    public class OpenDataEvent : cFormEvent
    {
        public OpenDataEvent (cGlobalParas.DatabaseType dType,string strCon,string sql,string TaskName)
        {
            m_dType = dType;
            m_strCon = strCon;
            m_sql = sql;
            m_TaskName = TaskName;
        }

        private cGlobalParas.DatabaseType m_dType;
        public cGlobalParas.DatabaseType dType
        {
            get { return m_dType; }
            set { m_dType = value; }
        }

        private string  m_strCon;
        public string strCon
        {
            get { return m_strCon; }
            set { m_strCon = value; }
        }

        private string m_sql;
        public string sql
        {
            get { return m_sql; }
            set { m_sql = value; }
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }
    }

}
