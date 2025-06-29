using System;
using System.Collections.Generic;
using System.Text;
using NetMiner.Common;
using NetMiner.Resource;
using System.Collections ;
using System.IO;
using System.Data;
using NetMiner.Base;

///2013-9-14 模板升级到1.1
///增加了Web发布模板支持回链及验证码接口
namespace NetMiner.Publish.Rule
{
    public class cTemplate
    {
        //发布模板的版本
        private float m_SupportTaskVersion = float.Parse("1.1");
        private string m_workPath = string.Empty;

        public float SupportTaskVersion
        {
            get { return m_SupportTaskVersion; }
        }

        public cTemplate(string workPath)
        {
            m_workPath = workPath;

            this.m_LoginParas = new Dictionary<string, string>();
            this.m_PublishParas = new Dictionary<string, string>();
            this.m_UploadParas = new Dictionary<string, string>();
            this.m_pgPara = new System.Collections.Generic.List<cPublishGlobalPara>();

        }

        ~cTemplate()
        {
            this.m_LoginParas = null;
            this.m_PublishParas = null;
            this.m_UploadParas = null;
            m_pgPara = null;
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

        private cGlobalParas.WebCode m_uCode;
        public cGlobalParas.WebCode uCode
        {
            get { return m_uCode; }
            set { m_uCode = value; }
        }

        private string m_Domain;
        public string Domain
        {
            get { return m_Domain; }
            set { m_Domain = value; }
        }

        private string m_Remark;
        public string Remark
        {
            get { return m_Remark; }
            set { m_Remark = value; }
        }

        private cGlobalParas.PublishTemplateType m_TempType;
        public cGlobalParas.PublishTemplateType TempType
        {
            get { return m_TempType; }
            set { m_TempType = value; }
        }

        private string m_LoginUser;
        public string LoginUser
        {
            get { return m_LoginUser; }
            set { m_LoginUser = value; }
        }

        private string m_LoginPwd;
        public string LoginPwd
        {
            get { return m_LoginPwd; }
            set { m_LoginPwd = value; }
        }

        private string m_LoginUrl;
        public string LoginUrl
        {
            get { return m_LoginUrl; }
            set { m_LoginUrl = value; }
        }

        private string m_LoginRUrl;
        public string LoginRUrl
        {
            get { return m_LoginRUrl; }
            set { m_LoginRUrl = value; }
        }

        private string m_LoginVCodeUrl;
        public string LoginVCodeUrl
        {
            get { return m_LoginVCodeUrl; }
            set { m_LoginVCodeUrl = value; }
        }

        private string m_LoginSuccess;
        public string LoginSuccess
        {
            get { return m_LoginSuccess; }
            set { m_LoginSuccess = value; }
        }

        private string m_LoginFail;
        public string LoginFail
        {
            get { return m_LoginFail; }
            set { m_LoginFail = value; }
        }

        private Dictionary<string, string> m_LoginParas;
        public Dictionary<string, string> LoginParas
        {
            get { return m_LoginParas; }
            set { m_LoginParas = value; }
        }

        private string m_ClassUrl;
        public string ClassUrl
        {
            get { return m_ClassUrl; }
            set { m_ClassUrl = value; }
        }

        private string m_ClassRUrl;
        public string ClassRUrl
        {
            get { return m_ClassRUrl; }
            set { m_ClassRUrl = value; }
        }

        private string m_ClassStartCut;
        public string ClassStartCut
        {
            get { return m_ClassStartCut; }
            set { m_ClassStartCut = value; }
        }

        private string m_ClassEndCut;
        public string ClassEndCut
        {
            get { return m_ClassEndCut; }
            set { m_ClassEndCut = value; }
        }

        private string m_ClassRegex;
        public string ClassRegex
        {
            get { return m_ClassRegex; }
            set { m_ClassRegex = value; }
        }

        private string m_PublishUrl;
        public string PublishUrl
        {
            get { return m_PublishUrl; }
            set { m_PublishUrl = value; }
        }

        private string m_PublishRUrl;
        public string PublishRUrl
        {
            get { return m_PublishRUrl; }
            set { m_PublishRUrl = value; }
        }

        private string m_PublishSuccess;
        public string PublishSuccess
        {
            get { return m_PublishSuccess; }
            set { m_PublishSuccess = value; }
        }

        private string m_PublishFail;
        public string PublishFail
        {
            get { return m_PublishFail; }
            set { m_PublishFail = value; }
        }

        private Dictionary<string, string> m_PublishParas;
        public Dictionary<string, string> PublishParas
        {
            get { return m_PublishParas; }
            set { m_PublishParas = value; }
        }

        private string m_UploadUrl;
        public string UploadUrl
        {
            get { return m_UploadUrl; }
            set { m_UploadUrl = value; }
        }

        private string m_UploadRUrl;
        public string UploadRUrl
        {
            get { return m_UploadRUrl; }
            set { m_UploadRUrl = value; }
        }

        private Dictionary<string, string> m_UploadParas;
        public Dictionary<string, string> UploadParas
        {
            get { return m_UploadParas; }
            set { m_UploadParas = value; }
        }

        private string m_UploadReplace;
        public string UploadReplace
        {
            get { return m_UploadReplace; }
            set { m_UploadReplace = value; }
        }

        private bool m_IsHeader;
        public bool IsHeader
        {
            get { return m_IsHeader; }
            set { m_IsHeader = value; }
        }

        private string m_Headers;
        public string Headers
        {
            get { return m_Headers; }
            set { m_Headers = value; }
        }

        private int m_PublishInterval;
        public int PublishInterval
        {
            get { return m_PublishInterval; }
            set { m_PublishInterval = value; }
        }

        private List<cPublishGlobalPara> m_pgPara;
        public List<cPublishGlobalPara> pgPara
        {
            get { return m_pgPara; }
            set { m_pgPara = value; }
        }

        //以下为1.1支持
        private cGlobalParas.RUrlPageType m_RUrlPageType;
        public cGlobalParas.RUrlPageType RUrlPageType
        {
            get { return m_RUrlPageType; }
            set { m_RUrlPageType = value; }
        }

        private string m_RUrlPage;
        public string RUrlPage
        {
            get { return m_RUrlPage; }
            set { m_RUrlPage = value; }
        }

        private string m_RUrlRule;
        public string RUrlRule
        {
            get { return m_RUrlRule; }
            set { m_RUrlRule = value; }
        }

        private bool m_IsVCodePlugin;
        public bool IsVCodePlugin
        {
            get { return m_IsVCodePlugin; }
            set { m_IsVCodePlugin = value; }
        }

        private string m_VCodePlugin;
        public string VCodePlugin
        {
            get { return m_VCodePlugin; }
            set { m_VCodePlugin = value; }
        }

        private string m_TestDomain;
        public string TestDomain
        {
            get { return m_TestDomain; }
            set { m_TestDomain = value; }
        }

        private string m_TestUser;
        public string TestUser
        {
            get { return m_TestUser; }
            set { m_TestUser = value; }
        }

        private string m_TestPwd;
        public string TestPwd
        {
            get { return m_TestPwd; }
            set { m_TestPwd = value; }
        }

        private string m_TestData;
        public string TestData
        {
            get { return m_TestData; }
            set { m_TestData = value; }
        }

        #endregion

        public bool DeleTemplate(string tName)
        {
            //首先删除此任务所在分类下的index.xml中的索引内容然后再删除具体的任务文件


            //先删除索引文件中的任务索引内容
            cIndex tIndex = new cIndex(m_workPath,m_workPath + "publish\\index.xml");
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

            if (this.TempVersion < this.SupportTaskVersion)
            {
                tXml=null;
                throw new Exception("当前的版本低于系统指定版本，无法使用！");
            }

            this.uCode = (cGlobalParas.WebCode)int.Parse(tXml.GetNodeValue("Template/UCode"));
            this.TempType = (cGlobalParas.PublishTemplateType)int.Parse(tXml.GetNodeValue("Template/Type"));
            this.Domain = tXml.GetNodeValue("Template/Domain");
            this.Remark = tXml.GetNodeValue("Template/Remark");
            this.PublishInterval=int.Parse ( tXml.GetNodeValue("Template/PublishInterval"));

            this.m_TestDomain = tXml.GetNodeValue("Template/TestDomain");
            this.m_TestUser = tXml.GetNodeValue("Template/TestUser");
            this.m_TestPwd = tXml.GetNodeValue("Template/TestPwd");
            this.m_TestData = tXml.GetNodeValue("Template/TestData");

            this.LoginUser = tXml.GetNodeValue("Template/Login/User");
            this.LoginPwd = tXml.GetNodeValue("Template/Login/Password");
            this.LoginUrl = tXml.GetNodeValue("Template/Login/Url");
            this.LoginRUrl = tXml.GetNodeValue("Template/Login/Referer");
            this.LoginVCodeUrl = tXml.GetNodeValue("Template/Login/VCodeUrl");
            this.LoginSuccess = tXml.GetNodeValue("Template/Login/Success");
            this.LoginFail = tXml.GetNodeValue("Template/Login/Fail");

            dw = tXml.GetData("descendant::Login/Paras");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    this.LoginParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                }
            }

            this.ClassUrl = tXml.GetNodeValue("Template/Class/Url");
            this.ClassRUrl = tXml.GetNodeValue("Template/Class/Referer");
            this.ClassStartCut = tXml.GetNodeValue("Template/Class/StartCut");
            this.ClassEndCut = tXml.GetNodeValue("Template/Class/EndCut");
            this.ClassRegex = tXml.GetNodeValue("Template/Class/ClassRegex");
            

            this.PublishUrl = tXml.GetNodeValue("Template/Publish/Url");
            this.PublishRUrl = tXml.GetNodeValue("Template/Publish/Referer");
            this.PublishSuccess = tXml.GetNodeValue("Template/Publish/Success");
            this.PublishFail = tXml.GetNodeValue("Template/Publish/Fail");

            dw = tXml.GetData("descendant::Publish/Paras");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    this.PublishParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                }
            }

            this.UploadUrl = tXml.GetNodeValue("Template/Upload/Url");
            this.UploadRUrl = tXml.GetNodeValue("Template/Upload/Referer");

            dw = tXml.GetData("descendant::Upload/Paras");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    this.UploadParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                }
            }

            this.UploadReplace = tXml.GetNodeValue("Template/Upload/Replace");

            if (tXml.GetNodeValue("Template/HeaderSet/IsHeader") == "True")
                this.IsHeader = true;
            else
                this.IsHeader = false;
            this.Headers = tXml.GetNodeValue("Template/HeaderSet/Headers");

            dw = tXml.GetData("descendant::GlobalParas");
            if (dw != null)
            {
                for (i = 0; i < dw.Count; i++)
                {
                    cPublishGlobalPara gPara=new cPublishGlobalPara ();
                    gPara.Label =dw[i].Row["Label"].ToString();
                    gPara.pgPage= (cGlobalParas.PublishGlobalParaPage)int.Parse (dw[i].Row["Page"].ToString());
                    gPara.Value = dw[i].Row["Value"].ToString();
                    this.pgPara.Add(gPara);
                }
            }

            //加载高级信息
            this.RUrlPageType =(cGlobalParas.RUrlPageType)int.Parse (tXml.GetNodeValue("Template/Advance/RUrlPageType"));
            this.RUrlPage=tXml.GetNodeValue("Template/Advance/RUrlPage");
            this.RUrlRule = tXml.GetNodeValue("Template/Advance/RUrlRule");
            this.IsVCodePlugin = (tXml.GetNodeValue("Template/Advance/IsVCodePlugin") == "True" ? true : false);
            this.VCodePlugin = tXml.GetNodeValue("Template/Advance/VCodePlugin");

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
                "<UCode>" + (int)this.uCode  + "</UCode>" +
		        "<Type>" + (int)this.TempType + "</Type>" +
		        "<Domain>" + this.Domain + "</Domain>" +
		        "<Remark>" + this.Remark  + "</Remark>" +
                "<PublishInterval>" + this.PublishInterval + "</PublishInterval>" +
                "<TestDomain>" + this.TestDomain + "</TestDomain>" +
                "<TestUser>" + this.TestUser + "</TestUser>" +
                "<TestPwd>" + this.TestPwd + "</TestPwd>" +
                "<TestData><![CDATA[" + this.TestData + "]]></TestData>" +
		        "<Login>" +
			    "<User>" + this.LoginUser  + "</User>" +
			    "<Password>" + this.LoginPwd + "</Password>" +
                "<Url><![CDATA[" + this.LoginUrl + "]]></Url>" +
                "<Referer><![CDATA[" + this.LoginRUrl + "]]></Referer>" +
                "<VCodeUrl><![CDATA[" + this.LoginVCodeUrl + "]]></VCodeUrl>" +
			    "<Success>" + this.LoginSuccess + "</Success>" +
			    "<Fail>" + this.LoginFail + "</Fail>" +
			    "<Paras>";
            foreach (KeyValuePair<string, string> de in this.LoginParas)
            {
                tXml +="<Para>";
                tXml += "<Label><![CDATA[" + de.Key.ToString() + "]]></Label>";
                tXml += "<Value><![CDATA[" + de.Value.ToString() + "]]></Value>";
                tXml +="</Para>";
            }
			tXml +="</Paras>" +
		        "</Login>" +
		        "<Class>" +
                "<Url><![CDATA[" + this.ClassUrl + "]]></Url>" +
                "<Referer><![CDATA[" + this.ClassRUrl + "]]></Referer>" +
                "<StartCut><![CDATA[" + this.ClassStartCut + "]]></StartCut>" +
                "<EndCut><![CDATA[" + this.ClassEndCut + "]]></EndCut>" +
                "<ClassRegex><![CDATA[" + this.ClassRegex + "]]></ClassRegex>" +
                "</Class>" +
			    "<Publish>" +
                "<Url><![CDATA[" + this.PublishUrl + "]]></Url>" +
                "<Referer><![CDATA[" + this.PublishRUrl + "]]></Referer>" +
				"<Success>" + this.PublishSuccess + "</Success>" +
				"<Fail>" + this.PublishFail + "</Fail>" +
				"<Paras>";
            foreach (KeyValuePair<string, string> de in this.PublishParas)
            {
                tXml +="<Para>";
                tXml += "<Label><![CDATA[" + de.Key.ToString() + "]]></Label>";
                tXml += "<Value><![CDATA[" + de.Value.ToString() + "]]></Value>";
                tXml +="</Para>";
            }
             tXml += "</Paras>" +
                 "</Publish>";


             tXml += "<Upload>" +
                 "<Url><![CDATA[" + this.UploadUrl + "]]></Url>" +
                 "<Referer><![CDATA[" + this.UploadRUrl + "]]></Referer>" +
                 "<Paras>";

             foreach (KeyValuePair<string, string> de in this.UploadParas)
             {
                 tXml += "<Para>";
                 tXml += "<Label><![CDATA[" + de.Key.ToString() + "]]></Label>";
                 tXml += "<Value><![CDATA[" + de.Value.ToString() + "]]></Value>";
                 tXml += "</Para>";
             }

             tXml += "</Paras>" +
                 "<Replace><![CDATA[" + this.UploadReplace + "]]></Replace>" +
                 "</Upload>";

             tXml += "<HeaderSet>" +
                 "<IsHeader>" + this.IsHeader.ToString() + "</IsHeader>" +
                 "<Headers>" + this.Headers + "</Headers>" +
                 "</HeaderSet>";

             tXml += "<GlobalParas>";

             for (int i = 0; i < this.pgPara.Count; i++)
             {
                 tXml += "<GlobalPara>";
                 tXml += "<Label>" + this.pgPara[i].Label + "</Label>";
                 tXml += "<Page>" + (int)this.pgPara[i].pgPage + "</Page>";
                 tXml += "<Value><![CDATA[" + this.pgPara[i].Value + "]]></Value>";
                 tXml += "</GlobalPara>";
             }
             tXml += "</GlobalParas>";

             tXml += "<Advance>";
             tXml += "<RUrlPageType>" + (int)this.RUrlPageType + "</RUrlPageType>";
             tXml += "<RUrlPage><![CDATA[" + this.RUrlPage + "]]></RUrlPage>";
             tXml += "<RUrlRule><![CDATA[" + this.RUrlRule + "]]></RUrlRule>";
             tXml += "<IsVCodePlugin>" + this.IsVCodePlugin.ToString() + "</IsVCodePlugin>";
             tXml += "<VCodePlugin>" + this.VCodePlugin + "</VCodePlugin>";
             tXml += "</Advance>";
             tXml += "</Template>";

             cXmlIO tFile = new cXmlIO();
             tFile.NewXmlFile(fName, tXml);
             tFile = null;
        }

        private void  InsertIndex(string tName,string Type,string remrk)
        {
            string indexXml = "<Name>" + tName + "</Name>" +
                    "<Type>" + Type + "</Type>" +
                    "<Remark>" + remrk + "</Remark>";

            cIndex tIndex = new cIndex(this.m_workPath, m_workPath + "publish\\index.xml");
            tIndex.InsertTemplateIndex(indexXml);
            tIndex = null;
        }

        /// <summary>
        /// 升级模板，仅支持从上一个版本的升级操作，不支持跨版本升级
        /// </summary>
        /// <param name="tName"></param>
        /// <returns></returns>
        public bool Upgrade(string tName)
        {
            try
            {
                string fName = m_workPath + "publish\\" + tName + ".spt";
                DataView dw = new DataView();
                int i = 0;

                if (!System.IO.File.Exists(fName))
                {
                    return false ;
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
                this.TempVersion = this.SupportTaskVersion;

                this.uCode = (cGlobalParas.WebCode)int.Parse(tXml.GetNodeValue("Template/UCode"));
                this.TempType = (cGlobalParas.PublishTemplateType)int.Parse(tXml.GetNodeValue("Template/Type"));
                this.Domain = tXml.GetNodeValue("Template/Domain");
                this.Remark = tXml.GetNodeValue("Template/Remark");

                this.PublishInterval = int.Parse(tXml.GetNodeValue("Template/PublishInterval"));

                this.LoginUser = tXml.GetNodeValue("Template/Login/User");
                this.LoginPwd = tXml.GetNodeValue("Template/Login/Password");
                this.LoginUrl = tXml.GetNodeValue("Template/Login/Url");
                this.LoginRUrl = tXml.GetNodeValue("Template/Login/Referer");
                this.LoginVCodeUrl = tXml.GetNodeValue("Template/Login/VCodeUrl");
                this.LoginSuccess = tXml.GetNodeValue("Template/Login/Success");
                this.LoginFail = tXml.GetNodeValue("Template/Login/Fail");

                dw = tXml.GetData("descendant::Login/Paras");
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        this.LoginParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                    }
                }

                this.ClassUrl = tXml.GetNodeValue("Template/Class/Url");
                this.ClassRUrl = tXml.GetNodeValue("Template/Class/Referer");
                this.ClassStartCut = tXml.GetNodeValue("Template/Class/StartCut");
                this.ClassEndCut = tXml.GetNodeValue("Template/Class/EndCut");
                this.ClassRegex = tXml.GetNodeValue("Template/Class/ClassRegex");


                this.PublishUrl = tXml.GetNodeValue("Template/Publish/Url");
                this.PublishRUrl = tXml.GetNodeValue("Template/Publish/Referer");
                this.PublishSuccess = tXml.GetNodeValue("Template/Publish/Success");
                this.PublishFail = tXml.GetNodeValue("Template/Publish/Fail");

                dw = tXml.GetData("descendant::Publish/Paras");
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        this.PublishParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                    }
                }

                this.UploadUrl = tXml.GetNodeValue("Template/Upload/Url");
                this.UploadRUrl = tXml.GetNodeValue("Template/Upload/Referer");

                dw = tXml.GetData("descendant::Upload/Paras");
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        this.UploadParas.Add(dw[i].Row["Label"].ToString(), dw[i].Row["Value"].ToString());
                    }
                }

                this.UploadReplace = tXml.GetNodeValue("Template/Upload/Replace");

                if (tXml.GetNodeValue("Template/HeaderSet/IsHeader") == "True")
                    this.IsHeader = true;
                else
                    this.IsHeader = false;
                this.Headers = tXml.GetNodeValue("Template/HeaderSet/Headers");

                dw = tXml.GetData("descendant::GlobalParas");
                if (dw != null)
                {
                    for (i = 0; i < dw.Count; i++)
                    {
                        cPublishGlobalPara gPara = new cPublishGlobalPara();
                        gPara.Label = dw[i].Row["Label"].ToString();
                        gPara.pgPage = (cGlobalParas.PublishGlobalParaPage)int.Parse(dw[i].Row["Page"].ToString());
                        gPara.Value = dw[i].Row["Value"].ToString();
                        this.pgPara.Add(gPara);
                    }
                }

                tXml = null;

                //升级的部分
                this.RUrlPageType = cGlobalParas.RUrlPageType.CurrentPage;
                this.RUrlPage = "";
                this.RUrlRule = "";
                this.IsVCodePlugin = false;
                this.VCodePlugin = "";

                this.TestDomain = "";
                this.TestUser = "";
                this.TestPwd = "";

                DeleTemplate(tName);
                Save(tName);
            }
            catch (System.Exception)
            {
                return false;
            }

            return true;
        }
    }
}
