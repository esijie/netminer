using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Data ;

///功能：自定义datagridview控件
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：主要用于采集数据的自动显示 
///版本：01.10.00
///修订：无
namespace SoukeyNetget.CustomControl
{
    public partial class cMyDataGridView : DataGridView 
    {
        private string m_FileName;

        public cMyDataGridView()
        {
            InitializeComponent();
            m_gData = new DataTable();
            base.VirtualMode = true;
            base.ReadOnly = true;
            base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            base.MultiSelect = true;
            base.DataSource = m_gData;
            ////m_gData.AcceptChanges();


            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = true ;
        }

        public cMyDataGridView(string FileName)
        {
            InitializeComponent();
            m_gData = new DataTable();
            base.VirtualMode = true;
            base.ReadOnly = true;
            base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            base.DataSource = m_gData;
            m_gData.AcceptChanges();

            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = false;

            m_FileName = FileName;

        }

        public void Clear()
        {
            m_gData = null;
            m_gData = new DataTable();
            base.DataSource = m_gData;
        }

        public cMyDataGridView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        private string m_TaskName;
        public string TaskName
        {
            get { return m_TaskName; }
            set { m_TaskName = value; }
        }

        private Int64 m_TaskRunID;
        public Int64 TaskRunID
        {
            get { return m_TaskRunID; }
            set { m_TaskRunID = value; }
        }

        private DataTable m_gData;
        public DataTable gData
        {
            get { return this.m_gData; }
            set 
            {
                DataTable tmp = new DataTable();
                tmp=value;
                try
                {
                    if (tmp != null)
                    {
                        m_gData.Merge(tmp);
                        //m_gData.AcceptChanges();

                        //for (int i = 0; i < tmp.Rows.Count; i++)
                        //{
                        //    .ImportRow(tmp.Rows[i]);
                        //}
                        //m_gData.AcceptChanges();
                        
                        base.FirstDisplayedScrollingRowIndex = base.Rows.Count - 1;

                        Application.DoEvents();
                    }
                }
                catch (System.Exception)
                {
                    //仅捕获错误，不做任何处理，只要保障不跳出程序即可
                }
            }
        }

        private string FileName
        {
            get
            {
                string FileName = Program.getPrjPath() + "data\\" + this.TaskName + "-" + this.TaskRunID + ".xml";
                return FileName;
            }

        }
    }
}
