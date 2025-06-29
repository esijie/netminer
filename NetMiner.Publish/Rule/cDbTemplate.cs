using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Resource;
using NetMiner.Common;
using System.IO;
using System.Data;
using NetMiner.Base;

namespace NetMiner.Publish.Rule
{
    public class cDbTemplate
    {
        private string m_workPath = string.Empty;

        public cDbTemplate(string workPath)
        {
            m_workPath = workPath;
            m_sqlParas = new List<cSqlPara>();
        }

        ~cDbTemplate()
        {
            m_sqlParas = null;
        }

        private Single m_SupportTaskVersion = Single.Parse("1.0");
        //此类别可处理的任务版本号，注意从1.3开始，任务处理类不再向前兼容
        public Single SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        #region 属性
        private Single m_TempVersion;
        public Single TempVersion
        {
            get { return m_TempVersion; }
            set { m_TempVersion = value; }
        }

        private string m_TempName;
        public string TempName
        {
            get { return m_TempName; }
            set { m_TempName = value; }
        }

        private cGlobalParas.PublishTemplateType m_TempType;
        public cGlobalParas.PublishTemplateType TempType
        {
            get { return m_TempType; }
            set { m_TempType = value; }
        }

        private string m_Remark;
        public string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        private cGlobalParas.DatabaseType m_DbType;
        public cGlobalParas.DatabaseType DbType
        {
            get { return m_DbType; }
            set { m_DbType = value; }
        }

        public List<cSqlPara> m_sqlParas;
        public List<cSqlPara> sqlParas
        {
            get { return m_sqlParas; }
            set { m_sqlParas = value; }
        }
        #endregion

        public bool DeleTemplate(string tName)
        {
            //首先删除此任务所在分类下的index.xml中的索引内容然后再删除具体的任务文件


            //先删除索引文件中的任务索引内容
            cIndex tIndex = new cIndex(m_workPath, m_workPath + "publish\\index.xml");
            tIndex.DeleTemplateIndex(tName);
            tIndex = null;

            //如果是编辑状态，为了防止删除了文件后，任务保存失败，则
            //任务文件将丢失的问题，首先先不删除此文件，只是将其改名

            //删除任务的物理文件
            string FileName = m_workPath + "publish\\" + tName + ".spt";
            string tmpFileName = m_workPath + "publish\\" + "~" + tName + ".spt";

            try
            {
                //删除物理临时文件
                if (File.Exists(tmpFileName))
                {
                    //File.SetAttributes(tmpFileName, System.IO.FileAttributes.Normal );
                    System.IO.File.Delete(tmpFileName);
                }

                System.IO.File.Move(FileName, tmpFileName);
                File.SetAttributes(tmpFileName, System.IO.FileAttributes.Hidden);

            }
            catch (System.Exception)
            {
                //如果出现临时文件备份操作失败，则继续进行，不能影响到最终的文件保存
                //但如果文件保存也失败，那只能报错了
            }

            //删除物理任务文件
            if (File.Exists(FileName))
            {
                File.SetAttributes(FileName, System.IO.FileAttributes.Normal);
                System.IO.File.Delete(FileName);
            }

            //将文件设置为隐藏
            return true;
        }

        private void InsertIndex(string tName, string Type, string remrk)
        {
            string indexXml = "<Name>" + tName + "</Name>" +
                    "<Type>" + Type + "</Type>" +
                    "<Remark>" + remrk + "</Remark>";

            cIndex tIndex = new cIndex(m_workPath, m_workPath + "publish\\index.xml");
            tIndex.InsertTemplateIndex(indexXml);
            tIndex = null;
        }

        /// <summary>
        /// 完整的文件
        /// </summary>
        /// <param name="fName"></param>
        public void LoadTemplate(string tName)
        {
            string fName = m_workPath + "publish\\" + tName + ".spt";
            DataView dw = new DataView();
            int i = 0;

            if (!System.IO.File.Exists(fName))
            {
                return;
            }
            cXmlIO tXml;

            try
            {
                tXml = new cXmlIO(fName);
            }
            catch (System.Exception ex)
            {
                if (!File.Exists(fName))
                {
                    throw new System.IO.IOException("您指定的任务文件不存在！");
                }
                else
                {
                    throw ex;
                }
            }

            //加载模版信息
            this.TempName = tXml.GetNodeValue("Template/Name");
            this.TempVersion = float.Parse(tXml.GetNodeValue("Template/Version"));
            this.TempType = (cGlobalParas.PublishTemplateType)int.Parse(tXml.GetNodeValue("Template/Type"));
            this.DbType = (cGlobalParas.DatabaseType)int.Parse(tXml.GetNodeValue("Template/DbType"));
            this.Remark = tXml.GetNodeValue("Template/Remark");

            dw = tXml.GetData("descendant::SqlParas");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    cSqlPara sPara = new cSqlPara();
                    sPara.Index = int.Parse (dw[i].Row["Index"].ToString());
                    if (dw[i].Row["IsRepeat"].ToString() == "True")
                        sPara.IsRepeat = true;
                    else
                        sPara.IsRepeat = false;

                    sPara.PK = dw[i].Row["PK"].ToString();
                    sPara.SqlType =(cGlobalParas.PublishSqlType) int.Parse ( dw[i].Row["SqlType"].ToString());
                    sPara.Sql = dw[i].Row["Sql"].ToString();
                    this.sqlParas.Add(sPara);
                }
            }


            tXml = null;
        }

        public void Save(string tName)
        {
            string fName = m_workPath + "publish\\" + tName + ".spt";

            if (System.IO.File.Exists(fName))
            {
                throw new Exception("任务已经存在，不能建立");
            }

            InsertIndex(this.TempName, ((int)this.TempType).ToString(), this.Remark);

            //开始构建文件
            string tXml;
            tXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" +
                "<Template>" +
                "<Name>" + this.TempName + "</Name>	" +
                "<Version>" + this.SupportTaskVersion + "</Version>" +
                "<Remark>" + this.Remark + "</Remark>" +
                "<Type>" + (int)this.TempType + "</Type>" +
                "<DbType>" + (int)this.DbType + "</DbType>" +
                "<SqlParas>";
            for(int i=0;i<this.sqlParas.Count ;i++)
            {
                tXml += "<Para>";
                tXml += "<Index><![CDATA[" + this.sqlParas[i].Index  + "]]></Index>";
                tXml += "<IsRepeat>" + this.sqlParas[i].IsRepeat.ToString() + "</IsRepeat>";
                tXml += "<PK>" + this.sqlParas[i].PK + "</PK>";
                tXml += "<SqlType><![CDATA[" + this.sqlParas[i].SqlType + "]]></SqlType>";
                tXml += "<Sql><![CDATA[" + this.sqlParas[i].Sql + "]]></Sql>";
                tXml += "</Para>";
            }
            tXml += "</SqlParas>" +
                "</Template>";

            cXmlIO tFile = new cXmlIO();
            tFile.NewXmlFile(fName, tXml);
            tFile = null;
        }
    }
}
