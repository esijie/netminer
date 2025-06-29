using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NetMiner.Publish;
using NetMiner.Publish.Rule;
using NetMiner.Resource;

namespace SoukeyPublishManage
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        public void IniData()
        {
            cIndex tIndex = new cIndex(Program.getPrjPath ());
            tIndex.GetData();
            int count = tIndex.GetCount();

            for (int i = 0; i < count; i++)
            {
                ListViewItem cItem = new ListViewItem();
                cItem.ImageIndex = 0;
                cItem.Name = tIndex.GetTName(i);
                cItem.Text  = tIndex.GetTName(i);
                cItem.SubItems.Add( tIndex.GetTType(i).GetDescription());
                cItem.SubItems.Add(tIndex.GetTRemark(i));

                this.listTemplate.Items.Add(cItem);

            }

        }

        private void toolMenuAddWeb_Click(object sender, EventArgs e)
        {
            frmWebRule f = new frmWebRule();
            f.IniData(NetMiner.Resource.cGlobalParas.FormState.New, "");
            f.RTemplate = GetAddTemplate;
            f.ShowDialog();
            f.Dispose();
        }

        private void GetAddTemplate( string tName, cGlobalParas.PublishTemplateType tType, string remark)
        {
            ListViewItem cItem = new ListViewItem();
            cItem.ImageIndex = 0;
            cItem.Name = tName;
            cItem.Text = tName;
            cItem.SubItems.Add(tType.GetDescription());
            cItem.SubItems.Add(remark);

            this.listTemplate.Items.Add(cItem);
        }

        private void toolMenuAddDb_Click(object sender, EventArgs e)
        {
            frmDBRule f = new frmDBRule();
            f.IniData(NetMiner.Resource.cGlobalParas.FormState.New, "");
            f.RTemplate = GetAddTemplate;
            f.ShowDialog();
            f.Dispose();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void toolEdit_Click(object sender, EventArgs e)
        {
            Edit();
        }

        private void Edit()
        {
            if (this.listTemplate.SelectedItems.Count == 0)
                return;

            try
            {
                if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(this.listTemplate.SelectedItems[0].SubItems[1].Text) ==
                    cGlobalParas.PublishTemplateType.Web)
                {
                    string tName = this.listTemplate.SelectedItems[0].Name;
                    frmWebRule f = new frmWebRule();
                    f.IniData(NetMiner.Resource.cGlobalParas.FormState.Edit, tName);
                    f.RTemplate = GetEditTemplate;
                    f.ShowDialog();
                    f.Dispose();
                }
                else
                {
                    string tName = this.listTemplate.SelectedItems[0].Name;
                    frmDBRule f = new frmDBRule();
                    f.IniData(NetMiner.Resource.cGlobalParas.FormState.Edit, tName);
                    f.RTemplate = GetEditTemplate;
                    f.ShowDialog();
                    f.Dispose();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("加载发布模板发生错误，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void GetEditTemplate(string tName, cGlobalParas.PublishTemplateType tType, string remark)
        {
            string oldName = this.listTemplate.SelectedItems[0].Name;
            this.listTemplate.SelectedItems[0].Name = tName;
            this.listTemplate.SelectedItems[0].Text = tName;
            this.listTemplate.SelectedItems[0].SubItems[1].Text =tType.GetDescription();
            this.listTemplate.SelectedItems[0].SubItems[2].Text =remark;

            //修改index中的名称和备注
            cIndex tindex = new cIndex(Program.getPrjPath(), Program.getPrjPath() + "publish\\index.xml");
            tindex.EditName(oldName,tName, remark);
            tindex = null;

        }

       

        private void listTemplate_DoubleClick(object sender, EventArgs e)
        {
            Edit();
        }

        private void toolExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listTemplate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DelTemplate();
            }
        }

        private void toolDel_Click(object sender, EventArgs e)
        {
            DelTemplate();
        }

        private void DelTemplate()
        {
            if (this.listTemplate.SelectedItems.Count == 0)
            {
                return;
            }

            if (MessageBox.Show("确实删除发布模版：" + this.listTemplate.SelectedItems[0].Text + "么？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.No)
                return;
           
            string tName = this.listTemplate.SelectedItems[0].Text;
            cIndex tindex = new cIndex(Program.getPrjPath() ,Program.getPrjPath() + "publish\\index.xml");
            tindex.DeleTemplateIndex(tName);
            tindex = null;

            string fName = Program.getPrjPath() + "publish\\" + tName + ".spt";
            System.IO.File.Delete(fName);
            
            this.listTemplate.Items.Remove(this.listTemplate.SelectedItems[0]);

        }

        private void toolExport_Click(object sender, EventArgs e)
        {
            if (this.listTemplate.SelectedItems.Count == 0)
            {
                return;
            }

            this.saveFileDialog1.OverwritePrompt = true;
            this.saveFileDialog1.Title = "导出采集模版";
            saveFileDialog1.InitialDirectory = Program.getPrjPath();
            saveFileDialog1.Filter = "采集模版文件(*.spt)|*.spt";

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = this.saveFileDialog1.FileName;

                string OldName = Program.getPrjPath () + "publish\\" + this.listTemplate.SelectedItems[0].Text + ".spt";

                try
                {
                    System.IO.File.Copy(OldName, FileName);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("导出发布模版出错，错误信息：" + ex.Message, "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void toolImport_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Title = "请选择导入的采集模版";

            openFileDialog1.Filter = "采集模版文件(*.xpt)|*.spt";


            if (this.openFileDialog1.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }

            string FileName = this.openFileDialog1.FileName;
            string NewFileName= Program.getPrjPath () + "publish\\" + System.IO.Path.GetFileName (FileName );
            string tName=System.IO.Path.GetFileNameWithoutExtension (NewFileName) ;

            //验证任务格式是否正确
            try
            {
                bool isExistTask = false;

                if (System.IO.File.Exists(NewFileName))
                {
                    isExistTask = true;
                }

                System.IO.File.Copy(FileName, NewFileName, true);
                cGlobalParas.PublishTemplateType pType = cGlobalParas.PublishTemplateType.Web;

                //判断导入的文件是什么类别
                try
                {
                    cTemplate t = new cTemplate(Program.getPrjPath());
                    t.LoadTemplate(tName);
                    pType = t.TempType;
                    t = null;
                }
                catch
                {
                    pType = cGlobalParas.PublishTemplateType.DB;
                }



                //如果导入一个已经存在的采集任务，则不插入索引文件
                if (isExistTask == false)
                {
                    cIndex tIndex = new cIndex(Program.getPrjPath() ,Program.getPrjPath () + "publish\\index.xml");
                    string tXML = "<Name>" + tName + "</Name>" +
                            "<Type>" + (int)pType + "</Type>" +
                            "<Remark></Remark>";
                    tIndex.InsertTemplateIndex(tXML);
                    tIndex = null;
                }

                ListViewItem cItem = new ListViewItem();
                cItem.ImageIndex = 0;
                cItem.Name = tName;
                cItem.Text = tName;
                cItem.SubItems.Add(pType.GetDescription());
                cItem.SubItems.Add("");

                this.listTemplate.Items.Add(cItem);

                MessageBox.Show("发布模版导入成功！", "网络矿工 信息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("发布模版导入失败，错误信息：" + ex.Message , "网络矿工 错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolUpgrade_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("是否升级当前的采集模板？", "网络矿工 询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            toolUpgrade.Enabled = false;

            for (int i = 0; i < this.listTemplate.Items.Count; i++)
            {
                if (EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType> (this.listTemplate.Items[i].SubItems[1].Text) ==
                    cGlobalParas.PublishTemplateType.Web)
                {
                    string demo = this.listTemplate.Items[i].SubItems[2].Text;
                    try
                    {

                        this.listTemplate.Items[i].SubItems[2].Text = "正在升级...";
                        Application.DoEvents();

                        cTemplate t = new cTemplate(Program.getPrjPath());
                        bool isS = t.Upgrade(this.listTemplate.Items[i].Name);
                        t = null;

                        if (isS == true)
                            this.listTemplate.Items[i].SubItems[2].Text = "升级成功  " + demo;
                        else
                            this.listTemplate.Items[i].SubItems[2].Text = "升级失败  " + demo;
                    }
                    catch (System.Exception ex)
                    {
                        this.listTemplate.Items[i].SubItems[2].Text = "升级失败，失败信息：" + ex.Message + "  " + demo;
                    }
                }
                else
                {
                    //数据库模板升级操作，一般不升级

                }
            }

            toolUpgrade.Enabled = true;

        }
       
    }
}
