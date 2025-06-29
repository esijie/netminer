using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Data ;
using NetMiner.Resource;

///���ܣ��Զ���datagridview�ؼ�
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������Ҫ���ڲɼ����ݵ��Զ���ʾ 
///�汾��01.10.00
///�޶�����
namespace SoukeyControl.CustomControl
{
    public partial class cMyDataGridView : DataGridView 
    {
        private string m_FileName;
        private cGlobalParas.VersionType m_SVersion;

      
        public cMyDataGridView(cGlobalParas.VersionType SVersion)
        {
            InitializeComponent();
            m_gData = new DataTable();
            base.VirtualMode = true;
            base.ReadOnly = true;
            //base.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            base.MultiSelect = true;
            base.DataSource = m_gData;
            ////m_gData.AcceptChanges();


            base.AllowUserToAddRows = false;
            base.AllowUserToDeleteRows = true ;

            m_SVersion = SVersion;
        }

        public cMyDataGridView(string FileName, cGlobalParas.VersionType SVersion)
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

            m_SVersion = SVersion;
        }

        protected override void  OnKeyDown(KeyEventArgs e)
        {
 	            base.OnKeyDown(e);
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

        public DataGridViewRow FindRow(object[] row)
        {
            for (int i = 0; i < this.Rows.Count; i++)
            {
                bool isExist = true;
                for (int j = 0; j < this.Columns.Count-1; j++)
                {
                    if (this.Rows[i].Cells[j].Value.ToString() != row[j].ToString())
                    {
                        isExist = false;
                        break;
                    }
                }
                if (isExist == true && this.Rows[i].Cells[this.Columns.Count -1].Value.ToString ()==cGlobalParas.PublishResult.UnPublished.ToString ())
                    return this.Rows[i];
            }
            return null;
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
                    //��������󣬲����κδ���ֻҪ���ϲ��������򼴿�
                }
            }
        }

        private string FileName
        {
            get
            {
                string FileName = "data\\" + this.TaskName + "-" + this.TaskRunID + ".xml";
                return FileName;
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
