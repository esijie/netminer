using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DataGatherControl.xml;
using System.IO;
using DataGatherControl;

namespace DataGatherControl.Task
{
    public class cTaskTemplate
    {
        public cTaskTemplate()
        {
            m_GAction = new  List<cGAction> ();
            m_GRule = new List<cGRule> ();
        }

        ~cTaskTemplate()
        {
            m_GAction = null;
            m_GRule = null;
        }

        #region 模版属性
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private string m_WebSite;
        public string WebSite
        {
            get { return m_WebSite; }
            set { m_WebSite = value; }
        }

        private int m_NextMaxPage;
        public int NextMaxPage
        {
            get { return m_NextMaxPage; }
            set { m_NextMaxPage = value; }
        }

        private string m_SearchUrl;
        public string SearchUrl
        {
            get { return m_SearchUrl; }
            set { m_SearchUrl = value; }
        }

        private string m_NextXPath;
        public string NextXPath
        {
            get { return m_NextXPath; }
            set { m_NextXPath = value; }
        }

        private bool m_IsLogin;
        public bool IsLogin
        {
            get { return m_IsLogin; }
            set { m_IsLogin = value; }
        }

        private string m_LoginUrl;
        public string LoginUrl
        {
            get { return m_LoginUrl; }
            set { m_LoginUrl = value; }
        }

        private string m_User;
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Pwd;
        public string Pwd
        {
            get { return m_Pwd; }
            set { m_Pwd = value; }
        }

        private string m_UserXpath;
        public string UserXpath
        {
            get { return m_UserXpath; }
            set { m_UserXpath = value; }
        }

        private string m_PwdXpath;
        public string PwdXpath
        {
            get { return m_PwdXpath; }
            set { m_PwdXpath = value; }
        }

        private string m_LoginButtonXPath;
        public string LoginButtonXPath
        {
            get { return m_LoginButtonXPath; }
            set { m_LoginButtonXPath = value; }
        }

        private List<cGAction> m_GAction;
        public List<cGAction> GAction
        {
            get { return m_GAction; }
            set { m_GAction = value; }
        }

        private List<cGRule> m_GRule;
        public List<cGRule> GRule
        {
            get { return m_GRule; }
            set { m_GRule = value; }
        }

        #endregion

        public void LoadTaskTemplate(string tName)
        {
            DataView dw;

            cXmlIO xmlTask;

            tName = cTool.getPrjPath() + "TaskTemplate\\" + tName;

            try
            {
                xmlTask = new cXmlIO(tName);

            }
            catch (System.Exception ex)
            {
                if (!File.Exists(tName))
                {
                    throw new System.IO.IOException("您指定的任务文件不存在！");
                }
                else
                {
                    throw ex;
                }
            }

            //加载模版信息
            this.Name = xmlTask.GetNodeValue("Task/BasicSettings/Name");
            this.WebSite = xmlTask.GetNodeValue("Task/BasicSettings/WebSite");
            this.NextMaxPage = int.Parse(xmlTask.GetNodeValue("Task/BasicSettings/MaxNextPage"));
            this.SearchUrl = xmlTask.GetNodeValue("Task/BasicSettings/SearchUrl");
            this.NextXPath = xmlTask.GetNodeValue("Task/BasicSettings/NextPageXPath");

            string isL = xmlTask.GetNodeValue("Task/BasicSettings/IsLogin");
            if (isL == "True")
                this.IsLogin = true;
            else
                this.IsLogin = false;

            this.LoginUrl = xmlTask.GetNodeValue("Task/BasicSettings/LoginUrl");
            this.User = xmlTask.GetNodeValue("Task/BasicSettings/User");
            this.Pwd = xmlTask.GetNodeValue("Task/BasicSettings/Pwd");
            this.UserXpath = xmlTask.GetNodeValue("Task/BasicSettings/UserXPath");
            this.PwdXpath = xmlTask.GetNodeValue("Task/BasicSettings/PwdXPath");
            this.LoginButtonXPath = xmlTask.GetNodeValue("Task/BasicSettings/LoginButtonXPath");

            //加载参数信息
            dw = new DataView();
            
            dw = xmlTask.GetData("descendant::Paras");
            cGAction ca;

            if (dw != null)
            {
                for (int i = 0; i < dw.Count; i++)
                {
                    ca = new cGAction();
                    ca.Para = dw[i].Row["Name"].ToString();
                    ca.xPath = dw[i].Row["XPath"].ToString();
                    ca.ElemType = dw[i].Row["ElemType"].ToString();
                    ca.ActionType = dw[i].Row["ActionType"].ToString();
                    ca.ParaValueType = (cGlobal.ParaValueType) int.Parse ( dw[i].Row["ParaValueType"].ToString());

                    this.m_GAction.Add(ca);
                }
            }

            //加载采集规则
            dw = new DataView();
            dw = xmlTask.GetData("descendant::Rules");
            cGRule cr;

            if (dw != null)
            {
                for (int i = 0; i < dw.Count; i++)
                {
                    cr = new cGRule();
                    cr.Name = dw[i].Row["Name"].ToString();
                    cr.xPath = dw[i].Row["XPath"].ToString();
                    cr.gType = (cGlobal.GatherDataType)int.Parse(dw[i].Row["gType"].ToString());
                    this.m_GRule.Add(cr);
                }
            }

            xmlTask = null;

        }

        public void SaveTemplate()
        {
            string tXml;
            tXml = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>" + "\r\n" +
                "<Task>" + "\r\n" +
                "<BasicSettings>" + "\r\n" +
                "<Name>" + this.Name + "</Name>" + "\r\n" +
                "<WebSite>" + this.WebSite + "</WebSite>" + "\r\n" +
                "<MaxNextPage>" + this.NextMaxPage.ToString() + "</MaxNextPage>" + "\r\n" +
                "<SearchUrl><![CDATA[" + this.SearchUrl + "]]></SearchUrl>" + "\r\n" +
                "<NextPageXPath>" + this.NextXPath + "</NextPageXPath>" + "\r\n" +
                "<IsLogin>" + this.IsLogin.ToString() + "</IsLogin>" + "\r\n" +
                "<LoginUrl>" + this.LoginUrl + "</LoginUrl>" + "\r\n" +
                "<User>" + this.User + "</User>" + "\r\n" +
                "<UserXPath>" + this.UserXpath + "</UserXPath>" + "\r\n" +
                "<PwdXPath>" + this.PwdXpath + "</PwdXPath>" + "\r\n" +
                "<LoginButtonXPath>" + this.LoginButtonXPath + "</LoginButtonXPath>" + "\r\n" +
                "<Pwd>" + this.Pwd + "</Pwd>" + "\r\n" +
                "</BasicSettings>" + "\r\n" +
                "<Paras>" + "\r\n" ;
            if (this.m_GAction !=null)
            {
                for (int i = 0; i < this.m_GAction.Count; i++)
                {
                    tXml += "<Para>" + "\r\n";
                    tXml += "<Name>" + this.m_GAction[i].Para + "</Name>" + "\r\n";
                    tXml += "<XPath>" + this.m_GAction[i].xPath + "</XPath>" + "\r\n";
                    tXml += "<ElemType>" + this.m_GAction[i].ElemType + "</ElemType>" + "\r\n";
                    tXml += "<ActionType>" + this.m_GAction[i].ActionType + "</ActionType>" + "\r\n";
                    tXml += "<ParaValueType>" + ((int)this.m_GAction[i].ParaValueType).ToString () + "</ParaValueType>" + "\r\n" ;
                    tXml += "</Para>" + "\r\n" ;
                }

            }
            tXml += "</Paras>" + "\r\n" ;
            tXml += "<Rules>" + "\r\n" ;
            if (this.m_GRule != null)
            {
                for (int i = 0; i < this.m_GRule.Count; i++)
                {
                    tXml += "<Rule>" + "\r\n" ;
                    tXml += "<Name>" + this.m_GRule[i].Name + "</Name>" + "\r\n" ;
                    tXml += "<XPath>" + this.m_GRule[i].xPath + "</XPath>" + "\r\n" ;
                    tXml += "<gType>" + (int)this.m_GRule[i].gType + "</gType>" + "\r\n";
                    tXml += "</Rule>" + "\r\n" ;
                }
            }
            tXml += "</Rules>" + "\r\n" ;
            tXml += "</Task>" + "\r\n" ;

            string fName = cTool.getPrjPath() + "TaskTemplate\\" + this.Name + ".xml";
            cXmlIO xTask = new cXmlIO();
            xTask.NewXmlFile(fName, tXml);
            xTask = null;
        }
    }
}
