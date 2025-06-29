using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using NetMiner.Resource;
using NetMiner.Publish.Rule;
using NetMiner.Common.HttpSocket;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.pTask.Entity;

///2013-5-7�������޸ģ������˷����������ݿ⼰web�ķ���������
///���ܣ����ݵ��� �ı� excel
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶�����

///�����Ӧ����UI��������ʱ���õ��� 
namespace NetMiner.Publish
{
    /// <summary>
    /// UI������ר���࣬��ΪUI����ʱ��Ҫ��ص����Ľ��ȣ���ˣ������д����˿ؼ���Ϣ�����ڷ���������ʵʱ����
    /// </summary>
    public class cExport
    {
        ContainerControl m_sender = null;
        int m_total = 0;
        Delegate m_senderDelegate = null;
        cGlobalParas.PublishType m_pType=cGlobalParas.PublishType.NoPublish ;
        string m_FileName = "";
        System.Data.DataTable m_pData;

        private string m_dSource;
        private string m_InsertSql;
        private string m_TableName;
        private bool m_IsSqlTrue;

        private int m_PIntervalTime;
        private string m_headers;
        private string m_ExportUrl;
        private string m_Cookie;
        private cGlobalParas.WebCode m_UrlCode;
        private string m_PluginName;


        private string m_TemplateName = "";
        private string m_User = "";
        private string m_Pwd = "";
        private string m_Domain = "";
        private string m_DbConn = "";
        private List<ePublishData> m_pParas = null;

        private Hashtable m_globalPara;
        private Hashtable m_WebClass;
        private bool m_IsGetClass = false;

        private string m_workPath = string.Empty;

        public cExport(string workPath)
        {
            m_workPath = workPath;
        }

        ~cExport()
        {
        }

        //�������ļ��Ĺ��캯��
        public cExport(ContainerControl sender, Delegate senderDelegate, cGlobalParas.PublishType pType,string FileName,System.Data.DataTable pData )
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_pType =pType ;
            m_FileName = FileName;
            m_pData = pData;
        }

        //���������ݿ�Ĺ��캯��
        public cExport(ContainerControl sender, Delegate senderDelegate, cGlobalParas.DatabaseType pType,
            string dSource,string strSql,string TableName, System.Data.DataTable pData,bool IsSqlTrue)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            if (pType == cGlobalParas.DatabaseType.Access)
                m_pType = cGlobalParas.PublishType.PublishAccess;
            else if (pType == cGlobalParas.DatabaseType.MSSqlServer)
                m_pType = cGlobalParas.PublishType.PublishMSSql;
            else if (pType == cGlobalParas.DatabaseType.MySql)
                m_pType = cGlobalParas.PublishType.PublishMySql;
                ;
            m_pData = pData;
            this.m_dSource = dSource;
            this.m_InsertSql = strSql;
            this.m_TableName = TableName;
            this.m_IsSqlTrue = IsSqlTrue;
        }

        //��������վ�Ĺ��캯��
        public cExport(ContainerControl sender, Delegate senderDelegate, cGlobalParas.PublishType pType, int PIntervalTime,
            string headers, string ExportUrl, string Cookie, cGlobalParas.WebCode  UrlCode, System.Data.DataTable pData)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_pType = pType;
            m_pData = pData;
            m_PIntervalTime = PIntervalTime;
            m_headers = headers;
            m_ExportUrl = ExportUrl;
            m_Cookie = Cookie;
            m_UrlCode = UrlCode;
        }

        //��ģ�淢�������ݿ�Ĺ��캯��
        public cExport(ContainerControl sender, Delegate senderDelegate, string templateName,cGlobalParas.PublishType pType,string user,string pwd,string domain,
            string dbConn, List<ePublishData> pParas, System.Data.DataTable pData)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_pType = pType;
            m_TemplateName = templateName;
            m_User = user;
            m_Pwd = pwd;
            m_Domain = domain;
            m_DbConn = dbConn;
            m_pParas = pParas;
            m_pData = pData;
        }
        
        public cExport(ContainerControl sender, Delegate senderDelegate, string pName, System.Data.DataTable pData)
        {
            m_sender = sender;
            m_senderDelegate = senderDelegate;
            m_pType = cGlobalParas.PublishType.publishPlugin;
            m_PluginName = pName;
            m_pData = pData;
        }

        //public void RunProcess(object obj)
        //{
        //    Thread.CurrentThread.IsBackground = true; //make them a daemon
        //    object[] objArray = (object[])obj;
        //    m_sender = (System.Windows.Forms.Form)objArray[0];
        //    m_senderDelegate = (System.Delegate)objArray[1];
        //    m_total = (int)objArray[2];

        //    LocalRunProcess();
        //}

        /// <summary>
        /// Method for ThreadStart delegate
        /// </summary>
        public void RunProcess(object obj)
        {
            object[] objArray = (object[])obj;
            long TaskID = long.Parse (objArray[0].ToString ());

            try
            {
                Thread.CurrentThread.IsBackground = true; //make them a daemon
                LocalRunProcess(TaskID);
            }
            catch (System.Exception ex)
            {
                //throw ex;
                return;
            }
        }

        private void LocalRunProcess(long TaskID)
        {
            //�����жϵ�ǰ��Ҫ����������
            switch (m_pType)
            {
                case cGlobalParas.PublishType.PublishExcel :
                    ExportExcel(TaskID);
                    break;
                case cGlobalParas.PublishType.PublishTxt :
                    ExportTxt(TaskID);
                    break;
                case cGlobalParas.PublishType.PublishCSV :
                    ExportCSV(TaskID);
                    break;
                case cGlobalParas.PublishType.publishWord:
                    ExportWord(TaskID);
                    break;
                case cGlobalParas.PublishType.PublishAccess:
                    ExportData(m_pType,TaskID);
                    break;
                case cGlobalParas.PublishType.PublishMSSql :
                    ExportData(m_pType, TaskID);
                    break;
                case cGlobalParas.PublishType.PublishMySql :
                    ExportData(m_pType, TaskID);
                    break;
                case cGlobalParas.PublishType.publishPlugin :
                    ExportPlugin(this.m_PluginName);
                    break;
                case cGlobalParas.PublishType.publishTemplate:
                    ExportTemplate(m_TemplateName);
                    break;
                default :
                    break;
            }
        }

        #region �������ļ�

        //�����ı��ļ�
        private  void ExportTxt(long TaskID)
        {
            FileStream  myStream = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.GetEncoding("gb2312"));
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                //д���� 
                for ( i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += m_pData.Columns[i].ColumnName;
                    str += "\t";
                }

                //ȥ�����һ���Ʊ��
                str = str.Substring(0, str.Length - 1);
              
                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //д���� 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        tempStr += m_pData.Rows[i][j];
                        tempStr += "\t";
                    }

                    //ȥ�����һ���Ʊ��
                    tempStr = tempStr.Substring(0, tempStr.Length -1);
                    
                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count , i, TaskID });
                }
               

                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                //���½�������Ϣ
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
            finally
            {
                sw.Close();
                myStream.Close();

                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });

            }

            return ;
        }

        public void ExportExcel(long TaskID)
        {
            int Count = 0;
            int i = 0;

            try
            {
                IWorkbook workbook = new XSSFWorkbook();

                ISheet sheet1 = workbook.CreateSheet("����󹤲ɼ�����");
                int step = 0;

                Count = m_pData.Rows.Count;
               
                //�����ͷ
                IRow row = sheet1.CreateRow(0);
                for (int j = 0; j < m_pData.Columns.Count; j++)
                {
                    ICell cell = row.CreateCell(j);
                    cell.SetCellValue(m_pData.Columns[j].ColumnName);
                }
                step = 1;

                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    IRow row1 = sheet1.CreateRow(i + step);
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        cell.SetCellValue(m_pData.Rows[i][j].ToString());
                    }
                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count,i, TaskID });
                }

                FileStream myStream = new FileStream(m_FileName, FileMode.Create);
                workbook.Write(myStream);

                myStream.Close();
                myStream.Dispose();

            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
            finally
            {

                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, Count, TaskID });
            }
        }

        public void ExportWord(long TaskID)
        {
            XWPFDocument doc = new XWPFDocument();
            int Count = 0;
            int i = 0;

            try
            {
                Count = m_pData.Rows.Count;

                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        XWPFParagraph p0 = doc.CreateParagraph();
                        XWPFRun r0 = p0.CreateRun();
                        r0.SetText(m_pData.Columns[j].ColumnName + ":" + m_pData.Rows[i][j].ToString());
                        r0.SetBold(false);
                        r0.FontFamily="����";
                        r0.FontSize=12;
                    }

                    XWPFParagraph p1 = doc.CreateParagraph();
                    XWPFRun r1 = p1.CreateRun();
                    r1.SetText("\r\n");

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
                }

                FileStream sw = new FileStream(m_FileName, FileMode.Create);
                doc.Write(sw);
                sw.Close();
            }
            catch(System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
            finally
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, Count, TaskID });
            }
        }

        public void ExportCSV(long TaskID)
        {
            FileStream myStream = File.Open(m_FileName, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
            string str = "";
            string tempStr = "";
            int i = 0;
            int Count = 0;

            try
            {
                //д���� 
                for (i = 0; i < m_pData.Columns.Count; i++)
                {
                    str += m_pData.Columns[i].ColumnName;
                    str += ",";
                }

                //ȥ�����һ���ָ���
                str = str.Substring(0, str.Length - 1);

                sw.WriteLine(str);

                Count = m_pData.Rows.Count;
                //д���� 
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    for (int j = 0; j < m_pData.Columns.Count; j++)
                    {
                        tempStr += m_pData.Rows[i][j];
                        tempStr += ",";
                    }

                    //ȥ�����һ���ָ���
                    tempStr = tempStr.Substring(0, tempStr.Length - 1);

                    sw.WriteLine(tempStr);
                    tempStr = "";

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
                }


                sw.Close();
                myStream.Close();

            }
            catch (Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
            finally
            {
                sw.Close();
                myStream.Close();

                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });

            }

            return;
        }
        #endregion

        private void ExportData(cGlobalParas.PublishType pType,long TaskID)
        {

            int Count = m_pData.Rows.Count;
            int i = 0;

            try
            {
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    NetMiner.Core.DB.oDBDeal dd = new NetMiner.Core.DB.oDBDeal(this.m_workPath);
                    object dRow = m_pData.Rows[i].ItemArray.Clone();
                    switch (pType)
                    {
                        case cGlobalParas.PublishType.PublishAccess:
                            //dd.ExportAccess(m_pData.Columns, dRow, this.m_dSource, this.m_InsertSql, this.m_TableName, this.m_IsSqlTrue);
                            break;
                        case cGlobalParas.PublishType.PublishMSSql:
                            dd.ExportMSSql(m_pData.Columns, dRow, this.m_dSource, this.m_InsertSql, this.m_TableName, this.m_IsSqlTrue);
                            break;
                        case cGlobalParas.PublishType.PublishMySql:
                            dd.ExportMySql(m_pData.Columns, dRow, this.m_dSource, this.m_InsertSql, this.m_TableName, this.m_IsSqlTrue);
                            break;
                    }

                    dd = null;

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i + 1, TaskID });
                }
            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
            finally
            {
                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, TaskID });
            }
        }

        public void ExportWeb()
        {
            int Count = m_pData.Rows.Count;
            int i = 0;

            try
            {
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    object dRow = m_pData.Rows[i].ItemArray.Clone();

                    cPublishData cp = new cPublishData(this.m_workPath);

                    if (this.m_PIntervalTime != 0)
                    {
                        //���òɼ���ʱ
                        Thread.Sleep(this.m_PIntervalTime);
                    }

                    string strHead = this.m_headers;
                    bool isHead = false;

                    if (strHead != "")
                        isHead = true;

                    string webSource = cp.ExportWeb(m_pData.Columns, dRow, this.m_ExportUrl,
                       this.m_UrlCode, ref this.m_Cookie, isHead, strHead);

                    cp = null;

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, m_pData.Rows[i][0] + "... �����ɹ�", false });
                }
            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, "�����������󣬵�������ֹͣ��������Ϣ��" + ex.Message, false });

            }

            //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
            m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, "�������", true });

        }

        public void ExportPlugin(string pName)
        {
            int Count=m_pData.Rows.Count ;

            try
            {
                NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                if (pName != "")
                    rPlugin.CallPublishData(m_pData, pName);
                rPlugin = null;
            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, Count, "�����������󣬵�������ֹͣ��������Ϣ��" + ex.Message, false });
            }
            finally
            {
                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, Count, "�������", true });
            }
        }

        public void ExportTemplate(string tempName)
        {
            string templateName = tempName.Substring(0, tempName.IndexOf("["));

            cGlobalParas.PublishTemplateType pType = EnumUtil.GetEnumName<cGlobalParas.PublishTemplateType>(tempName.Substring(tempName.IndexOf("[") + 1, tempName.IndexOf("]") - tempName.IndexOf("[") - 1));

            if (pType == cGlobalParas.PublishTemplateType.Web)
            {
                //web����ģ��
                ExportTemplateWeb(templateName);
            }
            else if (pType == cGlobalParas.PublishTemplateType.DB)
            {
                cDbTemplate dbT = new cDbTemplate(m_workPath);
                dbT.LoadTemplate (templateName);

                //���ݿⷢ��ģ��
                ExportTemplateDB(dbT.DbType, templateName, m_DbConn, m_pParas);

            }
        }

        public void ExportTemplateWeb(string templateName)
        {
             //����ģ����Ϣ
            cTemplate t = new cTemplate(m_workPath);
            t.LoadTemplate(templateName);

            //���¼��ط�������ʱ����
            m_PIntervalTime   = t.PublishInterval;

            string webSource = "";
 
            //��ʼ���е�¼����
            bool isLogin = cHttpSocket.Login(this.m_User, this.m_Pwd, t.uCode,
                this.m_Domain, t.Domain, t.LoginUrl, t.LoginRUrl, t.LoginVCodeUrl, t.LoginSuccess, t.LoginFail,
                t.LoginParas, t.IsVCodePlugin,t.VCodePlugin, out this.m_Cookie, out webSource);

            if (t.pgPara.Count > 0)
            {
                m_globalPara = new Hashtable();

                for (int j = 0; j < t.pgPara.Count; j++)
                {
                    if (t.pgPara[j].pgPage == cGlobalParas.PublishGlobalParaPage.LoginPage)
                    {
                        string label = t.pgPara[j].Label;
                        string strReg = t.pgPara[j].Value;

                        Match a = Regex.Match(webSource, strReg, RegexOptions.IgnoreCase);
                        string value = a.Groups[0].Value.ToString();
                        m_globalPara.Add(label, value);
                    }
                }
            }

            cPublishData cp = new cPublishData(this.m_workPath);

            int Count = m_pData.Rows.Count;
            int i = 0;

            try
            {
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    object dRow = m_pData.Rows[i].ItemArray.Clone();

                    //�����Ƿ���ڷ������Ϣ����Ҫ���л�ȡ
                    if (m_IsGetClass == false)
                    {
                        this.m_WebClass = cp.GetWebClass(templateName, this.m_Domain, this.m_Cookie, this.m_globalPara);
                        m_IsGetClass = true;
                    }

                    string rSource = string.Empty;
                    cp.PublishByWebTemplate(m_pData.Columns, dRow, templateName, this.m_Cookie,
                        this.m_Domain, m_WebClass, this.m_pParas, this.m_globalPara,t.VCodePlugin, out rSource);

                    if (m_PIntervalTime != 0)
                    {
                        //���òɼ���ʱ
                        Thread.Sleep(m_PIntervalTime);

                    }
                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i + 1, m_pData.Rows[i][0] + "... �����ɹ�", false });
                }
            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, "�����������󣬵�������ֹͣ��������Ϣ��" + ex.Message, false });

            }
            finally
            {
                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i,"�������", true });
            }
        }

        public void ExportTemplateDB(cGlobalParas.DatabaseType dType, string templateName,string dbConn,List<ePublishData> pParas)
        {
            cPublishData cp = new cPublishData(this.m_workPath);

            int Count = m_pData.Rows.Count;
            int i = 0;

            try
            {
                for (i = 0; i < m_pData.Rows.Count; i++)
                {
                    object dRow = m_pData.Rows[i].ItemArray.Clone();


                    switch (dType)
                    {
                        case cGlobalParas.DatabaseType.Access:
                            cp.PublishByDbTemplate(m_pData.Columns, dRow, templateName, cGlobalParas.PublishType.PublishAccess,
                                                dbConn, pParas);
                            break;
                        case cGlobalParas.DatabaseType.MSSqlServer:
                            cp.PublishByDbTemplate(m_pData.Columns, dRow, templateName, cGlobalParas.PublishType.PublishMSSql,
                                                dbConn, pParas);
                            break;
                        case cGlobalParas.DatabaseType.MySql:
                            cp.PublishByDbTemplate(m_pData.Columns, dRow, templateName, cGlobalParas.PublishType.PublishMySql,
                                                dbConn, pParas);
                            break;
                    }

                    //���½�������Ϣ
                    m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i + 1, m_pData.Rows[i][0] + "... �����ɹ�", false });
                }
            }
            catch (System.Exception ex)
            {
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i, "�����������󣬵�������ֹͣ��������Ϣ��" + ex.Message, false });

            }
            finally
            {
                //���½�����Ϊ��ɣ������ݲ�����ʾ�����������
                m_sender.BeginInvoke(m_senderDelegate, new object[] { Count, i,"�������", true });
            }
        }
    }
}
