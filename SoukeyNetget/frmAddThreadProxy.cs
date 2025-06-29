using System;
using System.Collections.Generic;
using System.Windows.Forms;
using NetMiner.Resource;
using NetMiner.Core.gTask.Entity;

namespace MinerSpider
{
    public partial class frmAddThreadProxy : Form
    {
        private int m_threadCount;
        public delegate void ReturnFieldName(List<eThreadProxy> tProxy);
        public ReturnFieldName rThread;


        public frmAddThreadProxy(int threadCount,List<eThreadProxy> threadProxy)
        {
            InitializeComponent();
            m_threadCount = threadCount;

            this.ProxyType.Items.Add(cGlobalParas.ProxyType.TaskConfig.GetDescription ());
            this.ProxyType.Items.Add(cGlobalParas.ProxyType.None.GetDescription ());
            this.ProxyType.Items.Add(cGlobalParas.ProxyType.SystemProxy.GetDescription());
            this.ProxyType.Items.Add(cGlobalParas.ProxyType.HttpProxy.GetDescription());
            this.ProxyType.Items.Add(cGlobalParas.ProxyType.Socket5.GetDescription());

            for (int i = 0; i < this.m_threadCount; i++)
            {
                this.threadDataGrid.Rows.Add(i.ToString(), cGlobalParas.ProxyType.TaskConfig.GetDescription(), "", "");
            }

            for (int i = 0; i < threadProxy.Count; i++)
            {
                for (int j = 0; j < this.threadDataGrid.Rows.Count; j++)
                {
                    if (threadProxy[i].Index == int.Parse(this.threadDataGrid.Rows[j].Cells[0].Value.ToString()))
                    {
                        this.threadDataGrid.Rows[i].Cells[1].Value = threadProxy[i].pType.GetDescription();
                        this.threadDataGrid.Rows[i].Cells[2].Value = threadProxy[i].Address;
                        this.threadDataGrid.Rows[i].Cells[3].Value = threadProxy[i].Port;
                    }
                }
            }

        }

        private void frmAddThreadProxy_Load(object sender, EventArgs e)
        {
            
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdApply_Click(object sender, EventArgs e)
        {
            bool isDefault = false;
            bool isCustom = false;

            List<eThreadProxy> threadProxy = new List<eThreadProxy>();
            for (int i = 0; i < this.threadDataGrid.Rows.Count; i++)
            {
                if (EnumUtil.GetEnumName< cGlobalParas.ProxyType >(this.threadDataGrid.Rows[i].Cells[1].Value.ToString()) ==
                    cGlobalParas.ProxyType.TaskConfig)
                {
                    isDefault = true;
                }

                if (EnumUtil.GetEnumName<cGlobalParas.ProxyType>(this.threadDataGrid.Rows[i].Cells[1].Value.ToString()) !=
                    cGlobalParas.ProxyType.TaskConfig)
                {
                    isCustom = true;
                }

                eThreadProxy tProxy = new eThreadProxy();
                tProxy.Index = int.Parse(this.threadDataGrid.Rows[i].Cells[0].Value.ToString());
                tProxy.pType = EnumUtil.GetEnumName<cGlobalParas.ProxyType>(this.threadDataGrid.Rows[i].Cells[1].Value.ToString());
                if (this.threadDataGrid.Rows[i].Cells[2].Value == null)
                    tProxy.Address = "";
                else
                    tProxy.Address = this.threadDataGrid.Rows[i].Cells[2].Value.ToString().Trim ();
                if (this.threadDataGrid.Rows[i].Cells[3].Value==null || this.threadDataGrid.Rows[i].Cells[3].Value.ToString().Trim() == "")
                    tProxy.Port = 80;
                else
                {
                    try
                    {
                        tProxy.Port = int.Parse(this.threadDataGrid.Rows[i].Cells[3].Value.ToString().Trim());
                    }
                    catch { tProxy.Port = 80; }
                }
                threadProxy.Add(tProxy);
            }

            //if (isDefault == true && isCustom == true)
            //{
            //    MessageBox.Show("如果指定每个线程都使用独立代理，则不允许设置线程为“任务默认设置”选项，必须为独立设置！", "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

            rThread(threadProxy);
            this.Close();
        }
    }
}
