using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using NetMiner.Core.Proxy;
using HtmlExtract.Utility;
using HtmlExtract.Content;
using System.Threading;
using System.Drawing;
using NetMiner.Resource;
using NetMiner.Common;
using NetMiner.Common.Tool;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;
using System.Xml;
using Newtonsoft.Json;
using NetMiner.Core.gTask.Entity;
using NetMiner.Core.Event;
using NetMiner.Core.Proxy.Entity;
using NetMiner.Gather.Entity;
using NetMiner.Net.Socket;
using NetMiner.Net.Common;
using NetMiner.Net;
using NetMiner.Core.Url;
using HtmlAgilityPack;

///���ܣ��ɼ����ݴ���
///���ʱ�䣺2009-3-2
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵������ 
///�汾��01.10.00
///�޶���2011��5��19�ս����޶�����������ļ����������ݽ��������ݼӹ������޷���ȷ�����ļ����ݣ�ԭ������Ϊ
///���ݼӹ�����λ������֮ǰ������������ص�ַ�����˱༭��������ģ���޷���ȷʶ�����صĵ�ַ
///�޸ģ���������ǰ�����ݱ༭֮ǰ
///�汾��01.20.00
///2013-7-7 �޸ģ�ÿ������url�󣬶����ݷ��ص�ͷ����Ϣ����cookie�ĸ���
///2013-7-10 �����˶�https��֧�֣���������get��ʽ��֤����һ��е�����
///�ڴ˲�����Դ���ȡ�����ⲿֱ�Ӵ��룬�������һ�����������ݲɼ�ƥ����

namespace NetMiner.Gather.Control
{

    public class cGatherWeb
    {
        DataTable tempData ;
        private static readonly int DEF_PACKET_LENGTH = 2048;
        private X509Certificate2 x509c = null;

        //����һ��������Ϣ�����࣬�������ø��ɼ��࣬���ݲɼ������
        //����������Ϣ��ȥ��ȡ����
        private string m_workPath = string.Empty;

        private string m_vCode = string.Empty;
        private string m_ImgCookie = string.Empty;

        internal static string sMakeCertParamsEE = "-pe -ss my -n \"CN={0}{1}\" -sky exchange -in {2} -is my -eku 1.3.6.1.5.5.7.3.1 -cy end -a sha1 -m 120";
        internal static string sMakeCertParamsRoot = "-r -ss my -n \"CN={0}{1}\" -sky signature -eku 1.3.6.1.5.5.7.3.1 -h 1 -cy authority -a sha1 -m 120";
        internal static string sMakeCertSubjectO = ", O=DO_NOT_TRUST, OU=Created by http://www.netminer.cn";
        internal static string sMakeCertRootCN = "DO_NOT_TRUST_NetMinerRoot";
        private static ICertificateProvider oCertProvider = null;

        public cGatherWeb(string workPath)
        {
            m_workPath = workPath;
            this.CutFlag = new List<eWebpageCutFlag>();
            m_IsDataRepeat = false;

        }

        ~cGatherWeb()
        {
            this.CutFlag = null;
        }
       
        #region ����
        private List<eWebpageCutFlag> m_CutFlag; 
        public List<eWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }

        private string m_WebpageSource;
        /// <summary>
        /// ����ǰ���ǽ�ȡ����ҳԴ��
        /// </summary>
        public string WebpageSource
        {
            get { return this.m_WebpageSource; }
            set { this.m_WebpageSource = value; }
        }

        public string[] AutoID { get; set; }

        private NetMiner.Base.cHashTree m_DataRepeat;
        public NetMiner.Base.cHashTree DataRepeat
        {
            get { return m_DataRepeat; }
            set { m_DataRepeat = value; }
        }

        /// <summary>
        /// ��ȡ�������ؿ��ж��Ƿ��ظ�
        /// </summary>
        private bool m_IsDataRepeat;
        public bool IsDataRepeat
        {
            get { return m_IsDataRepeat; }
        }

      

        #endregion 

        private void GetVCode(string code, string cookie)
        {
            m_vCode = code;
            m_ImgCookie = cookie;
        }

        /// <summary>
        /// ���������http������ʹ�õķ���
        /// </summary>
        /// <param name="url"></param>
        /// <param name="webCode"></param>
        /// <param name="websource"></param>
        /// <param name="cookie"></param>
        /// <param name="referUrl"></param>
        /// <returns></returns>

        private readonly Object m_autoLock = new Object();

        //�µĲɼ�ƥ�����
        /// <summary>
        /// �ɼ����ݺ���
        /// </summary>
        /// <param name="Url">�ɼ��ĵ�ַ</param>
        /// <param name="webCode">��ҳ����</param>
        /// <param name="isUrlCode">��ַ�Ƿ���Ҫ����</param>
        /// <param name="isTwoUrlCode">�Ƿ���Ҫ���α���</param>
        /// <param name="UrlCode">��ַ����</param>
        /// <param name="cookie">Cookie</param>
        /// <param name="startPos">��ҳ�ضϷ��ţ���ʼ</param>
        /// <param name="endPos">��ҳ�ضϷ��ţ���ֹ</param>
        /// <param name="sPath">�����ļ��洢·��</param>
        /// <param name="IsAutoUpdateHeader">�Ƿ��Զ�����ͷ��Ϣ</param>
        /// <param name="IsExportGUrl">�Ƿ񵼳��ɼ���ַ</param>
        /// <param name="IsExportGDateTime">�Ƿ񵼳��ɼ��¼�</param>
        /// <param name="referUrl">��·��ַ</param>
        /// <param name="ndRow">����ҳ�ɼ�������</param>
        /// <param name="loopFlag">ѭ���ɼ����</param>
        /// <param name="loopIndex">ѭ���˵ڼ���</param>
        /// <param name="stopFlag">���α��</param>
        /// <returns></returns>
        public DataTable GetGatherData(string Url,string startPos, string endPos, 
            string sPath,  bool IsExportGUrl, bool IsExportGDateTime,DataRow ndRow,
            string loopFlag, int loopIndex, string rejectFlag, cGlobalParas.RejectDeal rejectDeal)
        {
            if (this.CutFlag == null || this.CutFlag.Count == 0)
                return null;

            if (string.IsNullOrEmpty(this.WebpageSource))
            {
                throw new NetMinerException("��ҳ���ݲ����ڣ�����������ҳ��ǰ���ȡ��ǣ�������ҳ���ݲ����ڣ�");
            }

            DataTable JsonTable=null;

            tempData = new DataTable("tempData");
            int i;
            int j;
            string strCut = "";
            bool IsDownloadFile = false;
            bool isGetHtmlCached = false;
            bool isAutoID = false;
            int AutoIDColumn = 0;
            
            //����һ��ֵ��ָʾ�Ƿ���Ҫ�õ�xpath
            bool IsXPath = false;
            List<cXPathExpression> xPaths = new List<cXPathExpression>();


            //����һ��ֵ�Ƿ��Զ����������а�����ͼƬ
            bool IsAutoDownloadImage = false;
            bool IsAutoFirstImage = false;

            //����һֱ�Ƿ��ж������ظ�
            bool isRepeat = false;

            //�˱��Ϊ�Զ�ά������ϵͳ��⵽��������ƥ���ǩΪ׼
            bool IsArticleMatch = false;
            List<cSmartObj> SmartObjs = new List<cSmartObj>();

            #region ������ṹ����������ȡ����

            //����ҳ���ȡ�ı�־������ṹ
            for (i = 0; i < this.CutFlag.Count; i++)
            {
                //����ǻ�ȡ
                if (this.CutFlag[i].DataType == cGlobalParas.GDataType.HtmlCached)
                    isGetHtmlCached = true;
                
                tempData.Columns.Add(new DataColumn(this.CutFlag[i].Title, typeof(string)));

                if ((this.CutFlag[i].DataType == cGlobalParas.GDataType.Picture && IsDownloadFile == false) ||
                    (this.CutFlag[i].DataType == cGlobalParas.GDataType.File && IsDownloadFile == false))
                {
                    IsDownloadFile = true;
                }

                //�ж��Ƿ����Զ���ţ�����¼�Զ���ŵ�˳��ID
                if (isAutoID == false && this.CutFlag[i].GatherRuleType == cGlobalParas.GatherRuleType.NonePage)
                {
                    for (j = 0; j < this.CutFlag[i].ExportRules.Count; j++)
                    {
                        if (this.CutFlag[i].ExportRules[j].FieldRuleType == cGlobalParas.ExportLimit.ExportAutoCode)
                        {
                            isAutoID = true;
                            AutoIDColumn = i;
                            break;
                        }
                    }
                }
            }



            if (IsExportGUrl == true)
            {
                tempData.Columns.Add(new DataColumn("CollectionUrl", typeof(string)));
            }
            if (IsExportGDateTime == true)
                tempData.Columns.Add(new DataColumn("CollectionDateTime", typeof(string)));

            
                #region ������ʽ����
                //�����û�ָ����ҳ���ȡλ�ù���������ʽ
                //if (this.CutFlag[0].StartPos == "" && this.CutFlag[0].EndPos == "" && this.CutFlag[0].LimitSign != (int)cGlobalParas.LimitSign.Custom
                //    && this.CutFlag[0].RuleByPage != (int)cGlobalParas.GatherRuleByPage.NonePage)
                //{
                   
                //        strCut += "(?<" + this.CutFlag[0].Title + ">)" + @"^.*$|";
                //}
                //else
                //{
                    for (i = 0; i < this.CutFlag.Count; i++)
                    {
                        if (this.CutFlag[i].GatherRuleType == cGlobalParas.GatherRuleType.Normal
                            && this.CutFlag[i].DataType!=cGlobalParas.GDataType.HtmlCached)
                        {
                            #region ���ݲɼ���������������ʽ
                            switch (this.CutFlag[i].LimitSign)
                            {
                                case cGlobalParas.LimitSign.NoLimit:
                                    //if (i == 0)
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += ".*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += ".*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }
                                    break;
                                case cGlobalParas.LimitSign.NoWebSign:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[^<>]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[^<>]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }
                                    break;
                                case cGlobalParas.LimitSign.OnlyCN:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\u4e00-\\u9fa5]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\u4e00-\\u9fa5]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }
                                    break;
                                case cGlobalParas.LimitSign.OnlyDoubleByte:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[^\\x00-\\xff]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";

                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[^\\x00-\\xff]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }

                                    break;
                                case cGlobalParas.LimitSign.OnlyNumber:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\d]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\d]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }
                                    break;
                                case cGlobalParas.LimitSign.OnlyChar:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\x00-\\xff]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\x00-\\xff]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }

                                    break;
                                case cGlobalParas.LimitSign.CustomMatchChar:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut +=  this.CutFlag[i].RegionExpression.ToString() + "*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += this.CutFlag[i].RegionExpression.ToString() + "*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }

                                    break;
                                case cGlobalParas.LimitSign.Custom:
                                    if (strCut == "")
                                    {
                                        strCut += "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += this.CutFlag[i].RegionExpression.ToString();
                                        strCut += ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += this.CutFlag[i].RegionExpression.ToString();
                                        strCut += "))";
                                    }
                                    break;

                                default:
                                    if (strCut == "")
                                    {
                                        strCut += ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\S\\s]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + ")";
                                    }
                                    else
                                    {
                                        //strCut += "(?=(?:(?!" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + ").)*" + cTool.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += @"(([\s\S]+?)" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].StartPos) + "(?<" + this.CutFlag[i].Title + ">";
                                        strCut += "[\\S\\s]*?";
                                        strCut += ")";
                                        strCut += "(?=" + ToolUtil.RegexReplaceTrans(this.CutFlag[i].EndPos) + "))?";
                                    }

                                    break;
                            }
                            #endregion

                        }



                        if (this.CutFlag[i].GatherRuleType == cGlobalParas.GatherRuleType.XPath
                            && this.CutFlag[i].DataType != cGlobalParas.GDataType.HtmlCached)
                        {
                            IsXPath = true;

                            cXPathExpression xpe = new cXPathExpression();
                            xpe.XPath = this.CutFlag[i].XPath;
                            xpe.ColIndex = i;
                            xpe.NodePrty = EnumUtil.GetEnumName<cGlobalParas.HtmlNodeTextType>(this.CutFlag[i].NodePrty);
                            xPaths.Add(xpe);
                        }

                        if (this.CutFlag[i].IsAutoDownloadFileImage == true && IsAutoDownloadImage ==false )
                        {
                            IsAutoDownloadImage = true;
                            if (this.CutFlag[i].IsAutoDownloadOnlyImage == true)
                                IsAutoFirstImage=true ;
                        }

                        //�ж�����ƥ��ı�ǩ
                        if (this.CutFlag[i].GatherRuleType == cGlobalParas.GatherRuleType.Smart)
                        {
                            
                            IsArticleMatch = true;

                            cSmartObj sObj = new cSmartObj();
                            sObj.ColIndex = i;
                            sObj.GType = (cGlobalParas.GDataType)this.CutFlag[i].DataType;
                            SmartObjs.Add(sObj);
                        }
                    }
                //}
                #endregion
                
            #endregion

            #region ��ȡ��ҳԴ��

            int rowCount = this.CutFlag.Count;

            //try
            //{
            //    //GetHtml(Url, webCode, isUrlCode,isTwoUrlCode, UrlCode, ref cookie, startPos, endPos, true, 
            //    //    IsAutoUpdateHeader, referUrl,isGatherCoding, CodingFlag, CodingUrl, Plugin);

            //}
            //catch (System.Web.HttpException ex)
            //{
            //    throw new NetMinerException("��ȡ��ҳԴ�����" + ex.Message);
            //}

            #endregion
          
            #region �ж��Ƿ�Ϊѭ���ɼ�������ǣ�����ݱ��ȡ��ѭ�������ݣ��滻Դ��
            if (!string.IsNullOrEmpty(loopFlag))
            {
                //MatchCollection mcLoop = null;
                //try
                //{
                //    mcLoop = RegexMatch(".+?(?=" + cTool.RegexReplaceTrans(loopFlag) + ")", this.WebpageSource);
                //}
                //catch (System.Exception ex)
                //{
                //    tempData = null;
                //    throw ex;
                //    return tempData;
                //}

                string[] loops = Regex.Split(this.WebpageSource, ToolUtil.RegexReplaceTrans(loopFlag));
                if (loopIndex+1 > loops.Length)
                    this.WebpageSource = loops[loops.Length  - 1].ToString();
                else
                    this.WebpageSource = loops[loopIndex+1].ToString();
            }
            #endregion

            if (this.WebpageSource == null)
                return null;


            #region  �ڴ��ж��Ƿ������ˣ������������ǿ��ֹͣ�ɼ�����
            if (!string.IsNullOrEmpty(rejectFlag))  // && !(this.m_IsProxy==true && this.m_IsProxyFirst==false ))
            {
                if (this.WebpageSource.Contains(rejectFlag))
                {
                    if (e_Log != null)
                    {
                        e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Warning, "ϵͳ��⵽����ָ��ֹͣ�ɼ��ķ�����Ϣ���ɼ������Զ�ֹͣ��", false));
                    }
                    
                    switch(rejectDeal)
                    {
                        case cGlobalParas.RejectDeal.None:
                            break;
                        case cGlobalParas.RejectDeal.StopGather:
                            if (e_ForcedStopped != null)
                                e_ForcedStopped(this, new cTaskEventArgs());
                            return null;
                        case cGlobalParas.RejectDeal.Error:
                            throw new NetMinerException("��⵽��վ�����˲ɼ���Ϣ�������ɼ�����");
                        case cGlobalParas.RejectDeal.UpdateCookie:

                            break;
                        case cGlobalParas.RejectDeal.Coding:
                            //��ʼ����

                            break;
                    }
                    
                }
            }
            #endregion

            int rows = 0; //ͳ�ƹ��ɼ��˶�����

            #region ����ƥ�䲢�������
            try
            {
                //��ʼ��ȡ��ȡ����
                
                if (!string.IsNullOrEmpty(strCut))
                {
                    DataRow drNew = null;

                    MatchCollection mc = null;
                    try
                    {
                        //Regex re = new Regex(@strCut, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        //mc = re.Matches(this.WebpageSource);
                        mc = RegexMatch(strCut, this.WebpageSource);
                        
                    }
                    catch (System.Exception ex)
                    {
                        tempData = null;
                        throw ex;
                        return tempData;
                    }

                    //if (mc.Count == 0)
                    //{
                        //��ע�͵��ˣ�����֪���Ƿ������⣬�۲�һ��ʱ��
                        //ע�͵�Ŀ���ǣ���������ڹ���ɼ���ְ�ܲɼ����������ɼ��������ݣ����п��ܵ���
                        //���ܲɼ�Ҳ��ɼ������ݣ����������������ֱ�ӷ���Ϊ����

                        //tempData = null;
                        //return tempData;
                    //}

                    //��ʼ���ݲɼ������ݹ������ݱ�������
                    //�ڴ���Ҫ����ɼ������п��ܴ��е�����

                    foreach (Match ma in mc)
                    {

                        drNew = tempData.NewRow();
                        rows++;

                        for (i = 0; i < this.CutFlag.Count; i++)
                        {
                            if (this.CutFlag[i].GatherRuleType == cGlobalParas.GatherRuleType.Normal)
                                drNew[i] = ma.Groups[this.CutFlag[i].Title].ToString();
                        }

                        if (IsExportGUrl == true && IsExportGDateTime == true)
                            drNew[drNew.ItemArray.Length - 2] = Url;
                        else if (IsExportGUrl == true && IsExportGDateTime == false)
                            drNew[drNew.ItemArray.Length - 1] = Url;

                        if (IsExportGDateTime == true)
                            drNew[drNew.ItemArray.Length - 1] = DateTime.Now.ToString();

                        tempData.Rows.Add(drNew);
                        drNew = null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new NetMinerException("����ƥ������ʱ������������ɼ��������ʹ����ͨ��������ص���˴����ݡ�������Ϣ��" + ex.Message);
            }
            #endregion

            #region xPath���ݻ�ȡ����
            //����xPath��ȡ����

            try
            {
                if (IsXPath == true)
                {
                    DataTable xPathData = null;

                    xPathData = GetXPathData(this.WebpageSource, xPaths);

                    //��ʼ�ϲ�xpath�ɼ���ȡ������
                    for (int l = 0; l < xPathData.Rows.Count; l++)
                    {
                        if (l < tempData.Rows.Count)
                        {
                            for (int k = 0; k < xPathData.Columns.Count; k++)
                            {
                                tempData.Rows[l][int.Parse(xPathData.Columns[k].ToString())] = xPathData.Rows[l][k];
                            }
                        }
                        else
                        {
                            DataRow row1 = tempData.NewRow();
                            for (int k = 0; k < xPathData.Columns.Count; k++)
                            {
                                row1[int.Parse(xPathData.Columns[k].ToString())] = xPathData.Rows[l][k];
                            }
                            if (IsExportGUrl == true && IsExportGDateTime == true)
                                row1[row1.ItemArray.Length - 2] = Url;
                            else if (IsExportGUrl == true && IsExportGDateTime == false)
                                row1[row1.ItemArray.Length - 1] = Url;

                            if (IsExportGDateTime == true)
                                row1[row1.ItemArray.Length - 1] = DateTime.Now.ToString();
                            rows++;
                            tempData.Rows.Add(row1);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                throw new NetMinerException("���ӻ���ȡ����ʱ������������ɼ����ݹ��򣬴�����Ϣ��" + ex.Message);
            }
            #endregion

            i = 0;

            #region ������������ܻ�����ȡ�����ڴ˴���
            try
            {
                if (IsArticleMatch == true)
                {
                    DataTable smartGather = null;
                    smartGather = MatchArticle(this.WebpageSource);
                    if (smartGather != null && smartGather.Rows.Count != 0)
                    {
                        //��ʼ�ϲ����ܲɼ���ȡ������
                        if (tempData.Rows.Count > 0)
                        {
                            for (int l = 0; l < SmartObjs.Count; l++)
                            {
                                switch (SmartObjs[l].GType)
                                {
                                    case cGlobalParas.GDataType.AutoTitle:
                                        tempData.Rows[0][SmartObjs[l].ColIndex] = smartGather.Rows[0]["Title"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoPublishDate:
                                        tempData.Rows[0][SmartObjs[l].ColIndex] = smartGather.Rows[0]["PublishDate"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoContent:
                                        tempData.Rows[0][SmartObjs[l].ColIndex] = smartGather.Rows[0]["Content"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoAuthor:
                                        tempData.Rows[0][SmartObjs[l].ColIndex] = smartGather.Rows[0]["Author"].ToString();
                                        break ;
                                    case cGlobalParas.GDataType.AutoSource:
                                        tempData.Rows[0][SmartObjs[l].ColIndex] = smartGather.Rows[0]["Source"].ToString();
                                        break;

                                }
                            }
                        }
                        else
                        {
                            DataRow d = tempData.NewRow();
                            for (int l = 0; l < SmartObjs.Count; l++)
                            {
                                switch (SmartObjs[l].GType)
                                {
                                    case cGlobalParas.GDataType.AutoTitle:
                                        d[SmartObjs[l].ColIndex] = smartGather.Rows[0]["Title"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoPublishDate:
                                        d[SmartObjs[l].ColIndex] = smartGather.Rows[0]["PublishDate"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoContent:
                                        d[SmartObjs[l].ColIndex] = smartGather.Rows[0]["Content"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoAuthor:
                                        d[SmartObjs[l].ColIndex] = smartGather.Rows[0]["Author"].ToString();
                                        break;
                                    case cGlobalParas.GDataType.AutoSource:
                                        d[SmartObjs[l].ColIndex] = smartGather.Rows[0]["Source"].ToString();
                                        break;

                                }
                            }
                            if (IsExportGUrl == true && IsExportGDateTime == true)
                                d[d.ItemArray.Length - 2] = Url;
                            else if (IsExportGUrl == true && IsExportGDateTime == false)
                                d[d.ItemArray.Length - 1] = Url;

                            if (IsExportGDateTime == true)
                                d[d.ItemArray.Length - 1] = DateTime.Now.ToString();
                            tempData.Rows.Add(d);
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                throw new NetMinerException("���ܻ���ȡ���ݷ�����������ݴ����Ŵ�����Ȼ������ʹ�������������ù��򣬲��ɼ����ݣ�������Ϣ��" + ex.Message);
            }
            #endregion


            #region �ڴ������ݼӹ�����ǰ���ȴ����Զ���ŵ�����
            if (isAutoID == true)
            {
                for (i = 0; i < tempData.Rows.Count; i++)
                {
                    int aID = int.Parse(this.AutoID[0]);
                    System.Threading.Interlocked.Increment(ref aID);
                    this.AutoID[0] = aID.ToString();
                    tempData.Rows[i][AutoIDColumn] = aID.ToString();
                }
            }
            #endregion

            #region ��ʼ����������ƣ����л�ȡ���ݼӹ�

            bool isExportDownloadFilePath = false;

            //�Ƿ���ò���ӹ�������ӹ�����һ�������Լӹ�һ�μ��ɣ�����
            //��μӹ������ԣ������ǣ�����Ѿ��ӹ������ٽ��мӹ�����
            

            //�ڴ��ж��Ƿ���Ҫ�����ʱ�������ݵ�����,��������汾1.2�������������������
            

            for (i = 0; i < this.CutFlag.Count; i++)
            {
                //if (this.CutFlag[i].RuleByPage != (int)cGlobalParas.GatherRuleByPage.NonePage)
                //{
                
                    //ÿһ���ɼ�����ֻ�����ڼӹ�һ��
                    //bool isEditByPlugin = false;

                    for (j = 0; j < this.CutFlag[i].ExportRules.Count; j++)
                    {
                        string exportRule = "";

                        switch (this.CutFlag[i].ExportRules[j].FieldRuleType)
                        {
                       
                            //�Ƚ���ɾ������
                            case cGlobalParas.ExportLimit.ExportDelData:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        if (this.CutFlag[i].ExportRules[j].FieldRule.ToString() == "")
                                        {
                                            if (tempData.Rows[index][i].ToString() == "")
                                            {
                                                tempData.Rows.Remove(tempData.Rows[index]);
                                                index--;
                                            }
                                        }
                                        else
                                        {
                                            if (Regex.IsMatch(tempData.Rows[index][i].ToString(), this.CutFlag[i].ExportRules[j].FieldRule.ToString()))
                                            {
                                                tempData.Rows.Remove(tempData.Rows[index]);
                                                index--;
                                            }
                                        }

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportDelData.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message );
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportInclude:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string eWord = this.CutFlag[i].ExportRules[j].FieldRule.ToString().Trim();

                                        if (eWord.IndexOf("{") == 0 && eWord.IndexOf("}") > 1)
                                        {
                                            //��ʾ�ǲ���,��ȡ����ֵ
                                            string pvalue = eWord.Substring(1, eWord.IndexOf("}") - 1);

                                            if (tempData.Columns.Contains(pvalue))
                                            {
                                                eWord = tempData.Rows[index][pvalue].ToString();
                                            }
                                            else if (ndRow.Table.Columns.Contains(pvalue))
                                            {
                                                eWord = ndRow[pvalue].ToString();
                                            }
                                        }

                                        string[] eWords = eWord.Split('|');

                                        bool isExist = false;

                                        for (int eIndex = 0; eIndex < eWords.Length; eIndex++)
                                        {
                                            if (Regex.IsMatch(tempData.Rows[index][i].ToString(), eWords[eIndex]))
                                            {
                                                isExist = true;
                                                break;
                                            }

                                        }

                                        if (isExist == false)
                                        {
                                            tempData.Rows.Remove(tempData.Rows[index]);
                                            index--;
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportInclude.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportNoWebSign:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = ToolUtil.getTxt(tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportNoWebSign.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportPrefix:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = this.CutFlag[i].ExportRules[j].FieldRule + tempData.Rows[index][i].ToString();
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportPrefix.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportReplace:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        //�滻�ĸ�ʽ�ǣ�<OldValue:><NewValue:>�����������жϱ����ַ������ȴ���22
                                        if (this.CutFlag[i].ExportRules[j].FieldRule.Length > 22)
                                        {
                                            string sRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                            string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                            string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:") - 11);
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Replace(oStr, nStr);
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportReplace.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportSuffix:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = tempData.Rows[index][i].ToString() + this.CutFlag[i].ExportRules[j].FieldRule;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSuffix.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportTrimLeft:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        int len = tempData.Rows[index][i].ToString().Length;
                                        int lefti = int.Parse(this.CutFlag[i].ExportRules[j].FieldRule.ToString());
                                        if (tempData.Rows[index][i].ToString().Length > lefti)
                                        {
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Substring(lefti, len - lefti);
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" +  cGlobalParas.ExportLimit.ExportTrimLeft.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportTrimRight:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        int len = tempData.Rows[index][i].ToString().Length;
                                        int righti = int.Parse(this.CutFlag[i].ExportRules[j].FieldRule.ToString());
                                        if (tempData.Rows[index][i].ToString().Length > righti)
                                        {
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Substring(0, len - righti);
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportTrimRight.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportTrim:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Trim();
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportTrim.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportRegexReplace:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        //�滻�ĸ�ʽ�ǣ�<OldValue:><NewValue:>�����������жϱ����ַ������ȴ���22
                                        if (this.CutFlag[i].ExportRules[j].FieldRule.Length > 22)
                                        {
                                            string sRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                            string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                            string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:") - 11);
                                            if (nStr == @"\r\n")
                                                tempData.Rows[index][i] = Regex.Replace(tempData.Rows[index][i].ToString(), oStr, "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                            else if (nStr == @"\t")
                                                tempData.Rows[index][i] = Regex.Replace(tempData.Rows[index][i].ToString(), oStr, "\t", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                            else
                                                tempData.Rows[index][i] = Regex.Replace(tempData.Rows[index][i].ToString(), oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        }

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportRegexReplace.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;

                            case cGlobalParas.ExportLimit.ExportSetEmpty:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {

                                        if (Regex.IsMatch(tempData.Rows[index][i].ToString(), this.CutFlag[i].ExportRules[j].FieldRule.ToString()))
                                        {
                                            tempData.Rows[index][i] = "";
                                        }

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSetEmpty.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportConvertUnicode:
                                try
                                {
                                    Match mUni;
                                    Regex r = new Regex("(?<code>\\\\u[a-z0-9]{4})", RegexOptions.IgnoreCase);

                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string strUnicode = tempData.Rows[index][i].ToString().Trim();
                                        for (mUni = r.Match(strUnicode); mUni.Success; mUni = mUni.NextMatch())
                                        {
                                            string strValue = mUni.Result("${code}");   //����
                                            int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                                            string ch = string.Format("{0}", (char)CharNum);
                                            strUnicode = strUnicode.Replace(strValue, ch);
                                        }

                                        tempData.Rows[index][i] = strUnicode;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportConvertUnicode.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;

                            case cGlobalParas.ExportLimit.ExportConvertHtml:

                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = System.Web.HttpUtility.HtmlDecode(tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportConvertHtml.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }

                                break;

                            case cGlobalParas.ExportLimit.ExportHaveCRLF:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();

                                        tempData.Rows[index][i] = Regex.Replace(str, @"<[^/pbr][^>]*.|</[^pbr][^>]*.", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportHaveCRLF.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportHavePImgNoneCSS:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();

                                        tempData.Rows[index][i] =ToolUtil. RemoveCSS(ToolUtil. GetTextImage(str)); 
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportHavePImgNoneCSS.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportReplaceCRLF:
                                try
                                {

                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();

                                        byte[] bytes = new byte[2];
                                        bytes[0] = 13;
                                        bytes[1] = 10;

                                        str = Regex.Replace(str, @"<[^/pbr][^>]*.|</[^pbr][^>]*.", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);

                                        tempData.Rows[index][i] = Regex.Replace(str, @"<[pbr][^>]*.|</[pbr][^>]*.", System.Text.Encoding.ASCII.GetString(bytes), RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportReplaceCRLF.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportWrap :
                                try
                                {
                                    //�����
                                    string fRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    if (fRule.Length > 7)
                                    {
                                        DataTable tData = new DataTable();
                                        for (int m = 0; m < tempData.Columns.Count; m++)
                                        {
                                            tData.Columns.Add(new DataColumn(tempData.Columns[m].ColumnName, typeof(string)));
                                        }

                                        for (int index = 0; index < tempData.Rows.Count; index++)
                                        {
                                            string oStr = fRule.Substring(fRule.IndexOf("<Wrap:") + 6, fRule.LastIndexOf(">") - 6);
                                            string str = tempData.Rows[index][i].ToString().Trim();

                                            //char[] sp = oStr.ToCharArray();
                                            //string[] strWrap = str.Split(sp,StringSplitOptions.RemoveEmptyEntries);
                                            string[] strWrap = Regex.Split(str, oStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                                            for (int n = 0; n < strWrap.Length; n++)
                                            {
                                                if (n == 0)
                                                {
                                                    //��һ��ֱ���޸�ԭ�����ݱ��ֵ��������ֵ�����±����
                                                    tempData.Rows[index][i] = strWrap[n];
                                                }
                                                else
                                                {
                                                    object dRow = tempData.Rows[index].ItemArray.Clone();
                                                    DataRow rRow = tData.NewRow();
                                                    rRow.ItemArray = ((object[])dRow);
                                                    rRow[this.CutFlag[i].Title] = strWrap[n];
                                                    tData.Rows.Add(rRow);
                                                }
                                            }

                                        }

                                        //�ϲ������������
                                        tempData.Merge(tData);
                                        tData = null;

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportWrap.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportGatherdata :
                                try
                                {
                                    //ͨ�����������ȡ����
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = this.CutFlag[i].ExportRules[j].FieldRule;
                                        string ss = tempData.Rows[index][i].ToString();
                                        if (str != "")
                                        {
                                            Regex re = new Regex(str, RegexOptions.None);
                                            MatchCollection mc = re.Matches(ss);
                                            tempData.Rows[index][i] = "";
                                            foreach (Match ma in mc)
                                            {
                                                tempData.Rows[index][i] += ma.Value.ToString() + " ";
                                            }
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Trim();
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportGatherdata.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportFormatString :
                                try
                                {
                                    //��ʽ���ַ���
                                    string fRule1 = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        try
                                        {
                                            string strFormat = tempData.Rows[index][i].ToString();
                                            if (fRule1.IndexOf("yy") > 0)
                                            {
                                                //��ʾ������
                                                tempData.Rows[index][i] = string.Format(fRule1, DateTime.Parse(strFormat));
                                            }
                                            else
                                            {
                                                //����
                                                tempData.Rows[index][i] = string.Format(fRule1, double.Parse(strFormat));
                                            }

                                        }
                                        catch { }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportFormatString.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;

                            case cGlobalParas.ExportLimit.ExportDownloadFilePath:
                                isExportDownloadFilePath = true;
                                break;

                            case cGlobalParas.ExportLimit.ExportGatherUrl:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    string strUrl = string.Empty;

                                    if (exportRule != "")
                                    {
                                        Match charSetMatch = Regex.Match(Url, exportRule, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        strUrl = charSetMatch.Groups[0].ToString();
                                    }
                                    else
                                    {
                                        strUrl = Url;
                                    }

                                    if (tempData == null || tempData.Rows.Count == 0)
                                    {
                                        DataRow dr = tempData.NewRow();
                                        dr[i] = strUrl;
                                        tempData.Rows.Add(dr);
                                    }
                                    else
                                    {
                                        for (int index = 0; index < tempData.Rows.Count; index++)
                                        {
                                            tempData.Rows[index][i] = strUrl;
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportGatherUrl.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }

                                break;
                            case cGlobalParas.ExportLimit.ExportGather:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;


                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        if (tempData.Columns.Contains(exportRule))
                                        {
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString() + tempData.Rows[index][exportRule];
                                        }
                                        else if (ndRow.Table.Columns.Contains(exportRule))
                                        {
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString() + ndRow[exportRule];
                                        }

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportGather.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportMakeGather:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;

                                    Regex r1 = new Regex("(?<={).+?(?=})", RegexOptions.None);
                                    MatchCollection mc1 = r1.Matches(exportRule);
                                    string gname1, gname2;
                                    try
                                    {
                                        gname1 = mc1[0].ToString();
                                        gname2 = mc1[1].ToString();
                                    }
                                    catch
                                    {
                                        break;
                                    }

                                    string Symbol = exportRule.Substring(exportRule.IndexOf("}") + 1, 1);
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {

                                        string v1 = string.Empty, v2 = string.Empty;
                                        if (tempData.Columns.Contains(gname1))
                                            v1 = tempData.Rows[index][gname1].ToString();
                                        else if (ndRow.Table.Columns.Contains(gname1))
                                            v1 = ndRow[gname1].ToString ();

                                        if (tempData.Columns.Contains(gname2))
                                            v2 = tempData.Rows[index][gname2].ToString();
                                        else if (ndRow.Table.Columns.Contains(gname2))
                                            v2 = ndRow[gname2].ToString ();

                                        try
                                        {
                                            switch (Symbol)
                                            {
                                                case "+":
                                                    try
                                                    {
                                                        tempData.Rows[index][i] = float.Parse(v1) + float.Parse(v2);
                                                    }
                                                    catch
                                                    {
                                                        tempData.Rows[index][i] =v1 + v2;
                                                    }
                                                    break;
                                                case "-":
                                                    try
                                                    {
                                                        tempData.Rows[index][i] = float.Parse(v1) - float.Parse(v2);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    break;
                                                case "*":
                                                    try
                                                    {
                                                        tempData.Rows[index][i] = float.Parse(v1) * float.Parse(v2);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    break;
                                                case "/":
                                                    try
                                                    {
                                                        tempData.Rows[index][i] = float.Parse(v1) / float.Parse(v2);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    break;
                                            }
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportMakeGather.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }

                                break;
                            case cGlobalParas.ExportLimit.ExportEncodingString:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = System.Web.HttpUtility.UrlDecode(tempData.Rows[index][i].ToString(), Encoding.GetEncoding(exportRule));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportEncodingString.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportEncoding:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = System.Web.HttpUtility.UrlEncode(tempData.Rows[index][i].ToString(), Encoding.GetEncoding(exportRule));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportEncoding.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportValue:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = exportRule;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportValue.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportSynonymsReplace:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    NetMiner.Common.Tool.cArticleDeal aDeal = new NetMiner.Common.Tool.cArticleDeal(this.m_workPath);
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        try
                                        {
                                            tempData.Rows[index][i] = aDeal.SynonymsReplace(tempData.Rows[index][i].ToString(), exportRule);
                                        }
                                        catch { }
                                    }
                                    aDeal = null;
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSynonymsReplace.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportMergeParagraphs:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    NetMiner.Common.Tool.cArticleDeal aDeal1 = new NetMiner.Common.Tool.cArticleDeal(this.m_workPath);
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        try
                                        {
                                            tempData.Rows[index][i] = aDeal1.MergeParagraphs(tempData.Rows[index][i].ToString(), int.Parse(exportRule));
                                        }
                                        catch { }
                                    }
                                    aDeal1 = null;
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportMergeParagraphs.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;

                            case cGlobalParas.ExportLimit.ExportPY:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = ToolUtil.ToPinYin(tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportPY.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                         
                            case cGlobalParas.ExportLimit.ExportByPlugin:
                                try
                                {
                                    NetMiner.Core.Plugin.cRunPlugin rPlugin = new NetMiner.Core.Plugin.cRunPlugin();
                                    if (this.CutFlag[i].ExportRules[j].FieldRule != "")
                                        tempData = rPlugin.CallDealData(tempData, this.CutFlag[i].ExportRules[j].FieldRule);
                                    rPlugin = null;
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportByPlugin.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportDict :
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] =NetMiner.Common.ToolUtil. ReplaceDict(exportRule, tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportDict.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportBase64:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = Encoding.GetEncoding(exportRule).GetString(Convert.FromBase64String(tempData.Rows[index][i].ToString()));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportBase64.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;

                            case cGlobalParas.ExportLimit.ExportNumber:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;

                                    string sFlag = exportRule.Substring(0, 1);
                                    Regex r2 = new Regex(@"[\d|\.|\d]+", RegexOptions.None);
                                    MatchCollection mc2 = r2.Matches(exportRule);
                                    float sNum = 0.0F;
                                    try
                                    {
                                        sNum = float.Parse(mc2[0].Value.ToString());
                                    }
                                    catch { break; }

                                    if (mc2.Count > 0)
                                    {


                                        for (int index = 0; index < tempData.Rows.Count; index++)
                                        {
                                            //ȥ��һ������ �� ����
                                            try
                                            {


                                                switch (sFlag)
                                                {
                                                    case ">":
                                                        if (float.Parse(tempData.Rows[index][i].ToString()) <= sNum)
                                                        {
                                                            tempData.Rows.Remove(tempData.Rows[index]);
                                                            index--;
                                                        }
                                                        break;
                                                    case "<":
                                                        if (float.Parse(tempData.Rows[index][i].ToString()) >= sNum)
                                                        {
                                                            tempData.Rows.Remove(tempData.Rows[index]);
                                                            index--;
                                                        }
                                                        break;
                                                    case "=":
                                                        if (float.Parse(tempData.Rows[index][i].ToString()) != sNum)
                                                        {
                                                            tempData.Rows.Remove(tempData.Rows[index]);
                                                            index--;
                                                        }
                                                        break;
                                                }



                                            }
                                            catch { }
                                        }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportNumber.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportHaveIMG:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();

                                        //tempData.Rows[index][i] = Regex.Replace(str, @"<(?!img)[^>]*.|</[^>]*.", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        tempData.Rows[index][i] = ToolUtil.getTxtAndImage(str);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportHaveIMG.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportToAbsoluteUrl :
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();

                                        //str = NetMiner.Common.ToolUtil.ConvertToAbsoluteUrls(str, new Uri(Url));
                                    str = NetMiner.Common.ToolUtil.RelativeToAbsoluteUrl(Url,str );
                                        tempData.Rows[index][i] = str;

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportToAbsoluteUrl.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportFormatXML:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        string str = tempData.Rows[index][i].ToString().Trim();
                                        str = HtmlExtract.Utility.HtmlHelper.HtmlFormat(str);
                                        tempData.Rows[index][i] = str;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportToAbsoluteUrl.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportNoRepeat:
                                isRepeat = true;
                                break;
                            case cGlobalParas.ExportLimit.ExportBase64Encoding:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = 
                                         Convert.ToBase64String (Encoding.GetEncoding(exportRule).GetBytes(tempData.Rows[index][i].ToString()));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportBase64Encoding.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportConvertDateTime:
                                try
                                {
                                    exportRule = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i]=ToolUtil. ConvertDateTime (tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportConvertDateTime.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportSubstring:
                                int sLen=0;
                                try
                                {
                                    sLen =int.Parse ( this.CutFlag[i].ExportRules[j].FieldRule);
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        if (sLen<tempData.Rows[index][i].ToString().Length )
                                            tempData.Rows[index][i] = tempData.Rows[index][i].ToString().Substring(0, sLen);
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportToLower:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = tempData.Rows[index][i].ToString().ToLower();
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportToUpper:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = tempData.Rows[index][i].ToString().ToUpper();
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportToMD5:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(tempData.Rows[index][i].ToString(), "MD5");
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportToMD5.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportDelInvalidChar:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        tempData.Rows[index][i] = ToolUtil.CleanInvalidXmlChars(tempData.Rows[index][i].ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportReplaceFileName:


                                break;
                            case cGlobalParas.ExportLimit.ExportJsonToObject:

                                //JsonTable=JsonConvert.DeserializeObject<DataTable>()
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        JsonTable = MergeTable121(JsonTable, JsonConvert.DeserializeObject<DataTable>(tempData.Rows[index][i].ToString()));
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription () + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                                }
                                break;
                        case cGlobalParas.ExportLimit.ExportAesDecrypt:

                            string sKey = this.CutFlag[i].ExportRules[j].FieldRule;
                            try
                            {
                                for (int index = 0; index < tempData.Rows.Count; index++)
                                {
                                    tempData.Rows[index][i] = ToolUtil.AesDecrypt(tempData.Rows[index][i].ToString(), sKey);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                            }
                            break;
                        case cGlobalParas.ExportLimit.ExportUUID:

                            try
                            {
                                for (int index = 0; index < tempData.Rows.Count; index++)
                                {
                                    tempData.Rows[index][i] = System.Guid.NewGuid().ToString("N");
                                }
                            }
                            catch (System.Exception ex)
                            {
                                throw new NetMinerException("���ݼӹ�����" + cGlobalParas.ExportLimit.ExportUUID.GetDescription() + "����Բɼ������ݽ��м�飡������Ϣ��" + ex.Message);
                            }
                            break;

                        default:

                                break;
                        }
                    //}
                }
              

            }

            #endregion

            #region �ڴ˽����������صĴ���
            if (isRepeat == true)
            {
                for (int rIndex = 0; rIndex < tempData.Rows.Count; rIndex++)
                {
                    string dCode = string.Empty;
                    for (int cIndex = 0; cIndex < tempData.Columns.Count; cIndex++)
                    {
                        if (cIndex < this.CutFlag.Count)
                        {
                            for (int m = 0; m < this.CutFlag[cIndex].ExportRules.Count; m++)
                            {
                                if (this.CutFlag[cIndex].ExportRules[m].FieldRuleType == cGlobalParas.ExportLimit.ExportNoRepeat)
                                {
                                    dCode += tempData.Rows[rIndex][cIndex].ToString();
                                }
                            }
                        }
                    }

                    //��ʼ����
                    if (!this.m_DataRepeat.Add(ref dCode, false))
                    {
                        //�ظ��ˣ���ʼ����
                        m_IsDataRepeat = true ;
                        tempData.Rows.Remove(tempData.Rows[rIndex]);
                        rIndex--;
                    }
                }
            }
            #endregion

            #region ��Բɼ���Ҫ�����ļ����ֶν����ļ����ش���
            //�ж��Ƿ�����������ļ�����������У���ʼ���أ���Ϊ�˹���������������ͼƬʹ��
            //������ר�õ����ع��ߣ����Զ����ش���û�е��������̴߳���

            try
            {
                if (IsDownloadFile == true)
                {

                    if (isExportDownloadFilePath == true)
                    {
                        tempData.Columns.Add("DownloadFile");
                    }

                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                    string FileUrl = "";
                    string DownloadFileName = "";

                    //2013-7-16�޸ģ���Ϊ��Щ���п��ܻ�ɾ����rows�����������
                    //ƥ�������
                    for (i = 0; i < tempData.Rows.Count; i++)
                    {
                        for (j = 0; j < this.CutFlag.Count; j++)
                        {
                            FileUrl = tempData.Rows[i][j].ToString();

                            if ((this.CutFlag[j].DataType == cGlobalParas.GDataType.File && FileUrl !="")
                                || (this.CutFlag[j].DataType == cGlobalParas.GDataType.Picture && FileUrl != ""))
                            {
                                
                                //��ʼ��ȡ�����ļ�����
                                Regex s = new Regex(@"(?<=/)[^/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                MatchCollection urlstr = s.Matches(FileUrl);
                                if (urlstr.Count == 0)
                                    DownloadFileName = FileUrl;
                                else
                                    DownloadFileName = urlstr[urlstr.Count - 1].ToString();

                                //�ж��ļ����ĺϷ���
                                if (DownloadFileName.IndexOf(".") < 0)
                                {
                                    if (this.CutFlag[j].DataType == cGlobalParas.GDataType.Picture)
                                    {

                                        DownloadFileName = DownloadFileName + ".jpg";
                                    }
                                }

                                if (DownloadFileName.EndsWith(".jpg", StringComparison.CurrentCultureIgnoreCase)
                                        || DownloadFileName.EndsWith(".png", StringComparison.CurrentCultureIgnoreCase)
                                        || DownloadFileName.EndsWith(".gif", StringComparison.CurrentCultureIgnoreCase)
                                        || DownloadFileName.EndsWith(".bmp", StringComparison.CurrentCultureIgnoreCase)
                                        || DownloadFileName.EndsWith(".jpeg", StringComparison.CurrentCultureIgnoreCase))
                                {
                                }
                                else
                                {
                                    if (this.CutFlag[j].DataType == cGlobalParas.GDataType.Picture)
                                    {

                                        DownloadFileName = DownloadFileName + ".jpg";
                                    }
                                }
                                
                                //�滻�����зǷ��ַ�
                                DownloadFileName =Regex.Replace (DownloadFileName ,"[:|&|\\|/|!|?|*|<|>|\\|]","",RegexOptions.IgnoreCase );

                                if (this.CutFlag[j].DownloadFileSavePath == "")
                                {
                                    //sPath = cTool.getPrjPath() + "data\\tem_file";
                                }
                                else
                                {
                                    sPath = this.CutFlag[j].DownloadFileSavePath;
                                }

                                //�ڴ˴��������ļ�������������
                                string FileRenameRule = string.Empty;
                                bool isOCR = false;
                                decimal ocrSCore = 0;
                                bool isNumberOcr = false;
                                bool isWatermark = false;
                                string strWatermark = string.Empty;
                                cGlobalParas.DownloadFileDealType repeatFileDeal = cGlobalParas.DownloadFileDealType.SaveCover;

                                for (int e = 0; e < this.CutFlag[j].ExportRules.Count; e++)
                                {
                                    if (this.CutFlag[j].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportRenameDownloadFile)
                                    {
                                        DownloadFileName = RenameDownloadFile(DownloadFileName, FileUrl, sPath, this.CutFlag[j].ExportRules[e].FieldRule, tempData.Rows[i]);
                                        FileRenameRule = this.CutFlag[j].ExportRules[e].FieldRule;
                                    }
                                  
                                    if (this.CutFlag[j].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportRepeatNameDeal)
                                    {
                                        repeatFileDeal =EnumUtil.GetEnumName<cGlobalParas.DownloadFileDealType>(this.CutFlag[j].ExportRules[e].FieldRule);
                                    }
                                    if (this.CutFlag[j].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportWatermark)
                                    {
                                        isWatermark = true;
                                        strWatermark = this.CutFlag[j].ExportRules[e].FieldRule;
                                    }
                                }

                                //��ͼƬ·����������
                                DownloadFileName = sPath + "\\" + DownloadFileName;

                                //��ʼ����ͼƬ
                                if (FileUrl.StartsWith ("http",StringComparison.CurrentCultureIgnoreCase ))
                                {
                                    DownloadFileName = DownloadFile(FileUrl, Url, DownloadFileName,
                                        repeatFileDeal, "", FileRenameRule, isWatermark, strWatermark, tempData.Rows[i]);
                                }
                                else
                                {
                                

                                    Uri absoluteUri = new Uri(new Uri(Url), FileUrl);
                                    FileUrl = absoluteUri.ToString();

                                    absoluteUri = null;

                                    DownloadFileName = DownloadFile(FileUrl, Url, DownloadFileName,
                                       repeatFileDeal, "", FileRenameRule, isWatermark, strWatermark, tempData.Rows[i]);
                                }

                                //��������ͼƬ�ĵ�ַ,�ж��Ƿ���Ҫ������ַ
                                for (int m = 0; m < this.CutFlag[j].ExportRules.Count; m++)
                                {
                                    if (this.CutFlag[j].ExportRules[m].FieldRuleType == cGlobalParas.ExportLimit.ExportReplaceFileName)
                                    {
                                        tempData.Rows[i][j] = DownloadFileName;
                                    }

                                    if (this.CutFlag[j].ExportRules[m].FieldRuleType == cGlobalParas.ExportLimit.ExportImgReplace)
                                    {
                                        string sRule = this.CutFlag[j].ExportRules[m].FieldRule;
                                        string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                        string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:") - 11);
                                        if (nStr == @"\r\n")
                                            tempData.Rows[i][j] = Regex.Replace(tempData.Rows[i][j].ToString(), oStr, "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        else if (nStr == @"\t")
                                            tempData.Rows[i][j] = Regex.Replace(tempData.Rows[i][j].ToString(), oStr, "\t", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                        else
                                            tempData.Rows[i][j] = Regex.Replace(tempData.Rows[i][j].ToString (),oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                                        //tempData.Rows[i][j] = FileUrl;
                                    }
                               
                                }

                                if (isExportDownloadFilePath == true)
                                {
                                    tempData.Rows[i][tempData.Columns.Count - 1] += DownloadFileName + " ";
                                }
                                    

                                //�ļ���������֮����Ҫ�ж��Ƿ���һ��ͼƬ��������Ҫ����OCRʶ��
                                if (isOCR==true)
                                {
                                    //if (e_Log != null)
                                    //    e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Info, "ϵͳ��⵽��ͼƬ��Ҫ��������ʶ����ȴ�...", false));

                                    //cOcr ocr = new cOcr(this.m_workPath);
                                    //string strText = ocr.OcrText(DownloadFileName, ocrSCore,isNumberOcr);
                                    //ocr = null;

                                    //if (e_Log != null)
                                    //    e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Info, "ͼƬʶ�����Ѿ����", false));

                                    ////�滻�ı�
                                    //tempData.Rows[i][j] = strText;
                                }


                            }
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            #endregion

            #region ��ʼ�����Ƿ��Զ������ı�ͼƬ ע������ͼƬ��һ��ͬ���Ĵ���ʽ�������첽
            if (IsAutoDownloadImage == true && tempData.Rows.Count>0)
            {
                AutoDownloadImage(Url ,ref sPath ,ref tempData,"",IsAutoFirstImage,tempData.Rows[0]);

                if (isExportDownloadFilePath == true)
                {
                    if (!tempData.Columns.Contains("DownloadFile"))
                        tempData.Columns.Add("DownloadFile");
                    for (i = 0; i < rows; i++)
                    {
                        tempData.Rows[i][tempData.Columns.Count - 1] += sPath;
                    }
                }

                for (int mi = 0; mi < this.CutFlag.Count; mi++)
                {
                    if (this.CutFlag[mi].IsAutoDownloadFileImage == true)
                    {
                        for (int m = 0; m < this.CutFlag[mi].ExportRules.Count; m++)
                        {
                            if (this.CutFlag[mi].ExportRules[m].FieldRuleType == cGlobalParas.ExportLimit.ExportImgReplace)
                            {
                                for (int mj = 0; mj < tempData.Rows.Count; mj++)
                                {
                                    string strData = tempData.Rows[mj][mi].ToString();
                                    string sRule = this.CutFlag[mi].ExportRules[m].FieldRule;
                                    string oStr = sRule.Substring(sRule.IndexOf("<OldValue:") + 10, sRule.IndexOf("><NewValue:") - 10);
                                    string nStr = sRule.Substring(sRule.IndexOf("<NewValue:") + 10, sRule.Length - sRule.IndexOf("<NewValue:") - 11);
                                    if (nStr == @"\r\n")
                                        strData = Regex.Replace(strData, oStr, "\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    else if (nStr == @"\t")
                                        strData = Regex.Replace(strData, oStr, "\t", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    else
                                        strData = Regex.Replace(strData, oStr, nStr, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                                    tempData.Rows[mj][mi] = strData;
                                }

                            }
                            else if (this.CutFlag[mi].ExportRules[m].FieldRuleType ==cGlobalParas.ExportLimit.ExportReplaceFileName)
                            {

                            }
                        }
                    }
                }

            }
            #endregion

            xPaths = null;

            #region ��������ҳ���յ�����
            try
            {
                if (isGetHtmlCached == true)
                {
                    if (!Directory.Exists(sPath))
                    {
                        Directory.CreateDirectory(sPath);
                    }

                        for (j = 0; j < this.CutFlag.Count; j++)
                        {
                            if (this.CutFlag[j].DataType ==cGlobalParas.GDataType.HtmlCached)
                            {
                                string htmlFileName = string.Empty;
                                string htmlPath = sPath;

                                Regex s = new Regex(@"(?<=/)[^/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                MatchCollection urlstr = s.Matches(Url);
                                if (urlstr.Count == 0)
                                    htmlFileName = Url;
                                else
                                {
                                    int urlstrIndex = urlstr.Count - 1;
                                    while (htmlFileName == "" && urlstrIndex > 0)
                                    {
                                        htmlFileName = urlstr[urlstrIndex].ToString();
                                        urlstrIndex--;
                                    }
                                    if (htmlFileName.IndexOf(".") ==-1)
                                    {
                                        htmlFileName = htmlFileName + ".html";
                                    }
                                }

                                //�滻�����зǷ��ַ�
                                htmlFileName = Regex.Replace(htmlFileName, "[:|&|\\|/|!|?|*|<|>|\\|]", "", RegexOptions.IgnoreCase);

                                if (this.CutFlag[j].DownloadFileSavePath != "")
                                {
                                    htmlPath = this.CutFlag[j].DownloadFileSavePath;
                                }

                                //�ڴ˴��������ļ�������������
                                for (int e = 0; e < this.CutFlag[j].ExportRules.Count; e++)
                                {
                                    if (this.CutFlag[j].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportRenameDownloadFile)
                                    {
                                        htmlFileName = RenameDownloadFile(htmlFileName, Url, sPath, this.CutFlag[j].ExportRules[e].FieldRule, tempData.Rows[0]);
                                    }
                                }

                                htmlFileName = sPath + "\\" + htmlFileName;

                                //д�ļ�
                                FileStream fs = new FileStream(htmlFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                                sw.Write(this.WebpageSource);
                                sw.Close();
                                sw.Dispose();
                                fs.Close();
                                fs.Dispose();
                             
                                //�滻�ı�
                                for (int f=0;f<tempData.Rows.Count ;f++)
                                    tempData.Rows[f][j] = htmlFileName;

                                break;
                            }
                        }
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            #endregion

            if (JsonTable != null)
                tempData = MergeTable121(tempData, JsonTable);

            return tempData;
        }

        private DataTable MergeTable121(DataTable d1, DataTable d2)
        {
            if (d1 == null)
                return d2;
            if (d2 == null)
                return d1;

            DataTable d = d1.Clone();
            DataTable dd1 = d2.Clone();

            d.Merge(dd1);

            int count = 0;
            if (d1.Rows.Count > d2.Rows.Count)
                count = d1.Rows.Count;
            else
                count = d2.Rows.Count;

            for (int i = 0; i < count; i++)
            {
                DataRow dr = d.NewRow();
                for (int j = 0; j < d1.Columns.Count; j++)
                {
                    if (i < d1.Rows.Count)
                    {
                        dr[j] = d1.Rows[i][j].ToString();
                    }

                }

                for (int m = 0; m < d2.Columns.Count; m++)
                {
                    if (i < d2.Rows.Count)
                    {
                        dr[m + d1.Columns.Count] = d2.Rows[i][m].ToString();
                    }
                }

                d.Rows.Add(dr);
            }

            return d;

        }

        /// <summary>
        /// ���ָ���������Ƿ��п����ص��ļ�
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="sPath"></param>
        /// <param name="tempData"></param>
        /// <param name="cookie"></param>
        /// <param name="isFirstImage"></param>
        /// <param name="dRow"></param>
        private void AutoDownloadImage(string Url,ref string sPath,ref DataTable tempData,string cookie,bool isOnlyImage,DataRow dRow)
        {
            sPath = sPath + "\\" + GetPathByUrl(Url);

            if (sPath.Length >150)
            {
                sPath = sPath.Substring(0, 150);
            }


            for (int i = 0; i < this.CutFlag.Count; i++)
            {
                if (this.CutFlag[i].IsAutoDownloadFileImage == true)
                {
                    //��ȡ���ݣ�����url��������
                    for (int j = 0; j < tempData.Rows.Count; j++)
                    {
                        string strContent = tempData.Rows[j][i].ToString();

                        //��ȡstr�е��������ӵ�ַ
                        //string strRef = "(?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"])";
                        string strRef = "(?<=<img.*src=[\'|\"]?)\\S[^#+$<>\\s'\"]*(?=[\'|\"]?)";
                        MatchCollection matches = new Regex(strRef,RegexOptions.IgnoreCase).Matches(strContent);

                        foreach (Match match in matches)
                        {
                            string strUrl = match.Value.ToString();
                            strUrl = Regex.Replace(strUrl, "[\'|\"]", "");

                            //ȥ�����ӵ�ַ�����ж�������ӵ�ַ�Ƿ���һ��ͼƬ
                            string[] ExtArray = null;
                            if (isOnlyImage==false)
                                ExtArray = new string[] { ".pdf",".doc",".docx",".xls",".xlsx",".ppt",".pptx",".gif", ".jpg", ".png",".jpeg" };
                            else
                                ExtArray = new string[] {  ".gif", ".jpg", ".png", ".jpeg" };

                            bool bDownload = true ;
                            //string strUrlLow = strUrl.ToLower();
                            bool isReplaceSource = false;

                            if (bDownload == true)
                            {
                                //�����ļ�

                                if (this.CutFlag[i].DownloadFileSavePath == "")
                                {
                                    //sPath = cTool.getPrjPath() + "data\\tem_file";
                                }
                                else
                                {
                                    sPath = this.CutFlag[i].DownloadFileSavePath;
                                }

                                if (!strUrl.StartsWith("http",StringComparison.CurrentCultureIgnoreCase))
                                {
                                    strUrl = ToolUtil.RelativeToAbsoluteUrl(Url, strUrl);
                                }

                                string picLocalUrl = new Uri(strUrl).LocalPath;
                                string DownloadFileName = picLocalUrl.Substring(picLocalUrl.LastIndexOf("/") + 1, picLocalUrl.Length - picLocalUrl.LastIndexOf("/") - 1);

                                //string FileRenameRule = string.Empty;
                                bool isWatermark = false;
                                string strWatermark = string.Empty;
                                cGlobalParas.DownloadFileDealType repeatFileDeal = cGlobalParas.DownloadFileDealType.SaveCover;

                                //�ڴ˴��������ļ�������������
                                for (int e = 0; e < this.CutFlag[i].ExportRules.Count; e++)
                                {
                                    if (this.CutFlag[i].ExportRules[e].FieldRuleType ==cGlobalParas.ExportLimit.ExportRenameDownloadFile)
                                    {
                                        DownloadFileName = RenameDownloadFile(DownloadFileName,strUrl, sPath, this.CutFlag[i].ExportRules[e].FieldRule,dRow);
                                    }
                                    if (this.CutFlag[i].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportRepeatNameDeal)
                                    {
                                        //repeatFileDeal = (cGlobalParas.DownloadFileDealType)int.Parse(this.CutFlag[j].ExportRules[e].FieldRule);
                                        repeatFileDeal = EnumUtil.GetEnumName<cGlobalParas.DownloadFileDealType>(this.CutFlag[j].ExportRules[e].FieldRule);
                                    }
                                    if (this.CutFlag[i].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportWatermark)
                                    {
                                        isWatermark = true;
                                        strWatermark = this.CutFlag[j].ExportRules[e].FieldRule;
                                    }

                                    if (this.CutFlag[i].ExportRules[e].FieldRuleType ==cGlobalParas.ExportLimit.ExportReplaceFileName)
                                    {
                                        isReplaceSource = true;
                                        
                                    }
                                }
                                
                                DownloadFileName = sPath + "\\" + DownloadFileName;

                                if (string.Compare(strUrl.Substring(0, 4), "http", true) == 0)
                                {
                                        DownloadFileName = DownloadFile(strUrl, Url, DownloadFileName,
                                            repeatFileDeal, cookie, "",isWatermark,strWatermark,dRow);
                                }
                                else
                                {
                                    if (strUrl.Substring(0, 1) == "/")
                                    {
                                        Url = Url.Substring(7, Url.Length - 7);
                                        Url = Url.Substring(0, Url.IndexOf("/"));
                                        Url = "http://" + Url;

                                        if (!Url.EndsWith("/"))
                                            Url += "/";

                                        strUrl = Url + strUrl.Substring(1, strUrl.Length - 1);
                                    }
                                    else if (strUrl.IndexOf("/") <= 0)
                                    {
                                        Url = Url.Substring(0, Url.LastIndexOf("/") + 1);
                                        strUrl = Url + strUrl;
                                    }
                                    else
                                    {
                                        Url = Url.Substring(0, Url.LastIndexOf("/") + 1);
                                        strUrl = Url + strUrl;
                                    }

                                    DownloadFileName=DownloadFile(strUrl, Url, DownloadFileName,
                                        repeatFileDeal, cookie, "",isWatermark ,strWatermark,dRow);
                                }

                                if (isReplaceSource==true)
                                {
                                    tempData.Rows[j][i] = tempData.Rows[j][i].ToString().Replace(strUrl, DownloadFileName);
                                }

                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����xPath�ɼ�ҳ������
        /// 2013-1-28�����޸ģ�����֧��xpath���������Խ��ж������ݵ�ƥ��
        /// </summary>
        /// <param name="HtmlSource"></param>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        private DataTable GetXPathData(string HtmlSouce, List<cXPathExpression> xPaths)
        {
            DataTable xpathData = new DataTable();

            for (int l = 0; l < xPaths.Count; l++)
            {
                xpathData.Columns.Add(new DataColumn(xPaths[l].ColIndex.ToString(), typeof(string)));
            }

            int maxRows = 0;

            HtmlAgilityPack.HtmlDocument hDoc = new HtmlAgilityPack.HtmlDocument();
            hDoc.LoadHtml("<Html>" + HtmlSouce + "</Html>");

            for (int l = 0; l < xPaths.Count; l++)
            {
                //��ʼ��������

                try
                {
                    cUrlParse u = new cUrlParse(this.m_workPath);
                    List<string> xs = u.SplitWebUrl(xPaths[l].XPath);

                    if (xs.Count > maxRows)
                        maxRows = xs.Count;

                    for (int i = 0; i < xs.Count; i++)
                    {
                        HtmlNodeCollection ss = hDoc.DocumentNode.SelectNodes(xs[i]);

                        if (ss != null )
                        {

                            string strxPahtvalue = "";
                            if (xPaths[l].NodePrty == cGlobalParas.HtmlNodeTextType.InnerHtml)
                                strxPahtvalue = ss[0].InnerHtml;
                            else if (xPaths[l].NodePrty == cGlobalParas.HtmlNodeTextType.InnerText)
                                strxPahtvalue = ss[0].InnerText;
                            else if (xPaths[l].NodePrty == cGlobalParas.HtmlNodeTextType.OuterHtml)
                                strxPahtvalue = ss[0].OuterHtml; 

                            if (xpathData.Rows.Count >= maxRows)
                            {
                                xpathData.Rows[i][l] = strxPahtvalue;
                            }
                            else
                            {
                                DataRow xRow = xpathData.NewRow();
                                xRow[l] = strxPahtvalue;
                                xpathData.Rows.Add(xRow);
                            }

                        }
                    }

                }
                catch { }

            }

            return xpathData;

        }

        //�����ļ�����һ�����̵߳ķ�����������С�ļ����أ���֧��http��ʽ
        //�����ڹ����д���ˮӡ�����⣬ͬʱ�ڴ˸����ļ�����������������������ļ������������ڴ˴���
        public string DownloadFile(string url, string pageUrl, string fName,
            cGlobalParas.DownloadFileDealType fType,string cookie,string rule,bool isWatermark,string Watermark,DataRow dRow)
        {
           
            if (e_Log !=null)
                e_Log(this, new cGatherTaskLogArgs(0,"", cGlobalParas.LogType.Info, "ϵͳ��⵽��Ҫ�����ļ��������������ز���...", false));

            System.Net.HttpWebRequest wReq = null;
            System.Net.HttpWebResponse wRep = null;
            FileStream SaveFileStream = null;

            int startingPoint = 0;

            //�ж�Ŀ¼�Ƿ����

            string tmpPath = fName.Replace(System.IO.Path.GetFileName(fName), "");

            string fPath = System.IO.Path.GetDirectoryName(tmpPath);
            string fName1 = Path.GetFileName(fName);
            fName1 = Regex.Replace(fName1, "[/|\\|:|*|\\?|\"|<|>|\\|]", "");
            fName =fPath  + "\\" + fName1;
            if (Directory.Exists(fPath) == false)
            {
                Directory.CreateDirectory(fPath);
            }

            //�ж��ļ��Ƿ���ڣ�����������������д���
            fName = RepeatFile(fPath,  fName, fType,url);

            try
            {
                //For using untrusted SSL Certificates
                string strReferer = pageUrl;

                if (url.IndexOf("<POST") > 0)
                {
                    string url2 = url.Substring(0, url.IndexOf("<POST"));
                    wReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url2);
                }
                else
                    wReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

                //wReq.AddRange(startingPoint);xdwsdcfbgzx  

                #region ͨѶheader

                wReq.UserAgent = "Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.0; .NET CLR 1.1.4322; .NET CLR 2.0.50215;)";
                wReq.Headers.Add("Accept-Language", "zh-cn,en-us;");
                wReq.KeepAlive = true;
                wReq.Headers.Add("Accept-Encoding", "gzip, deflate");

                Match a = Regex.Match(url, @"(http://).[^/]*[?=/]", RegexOptions.IgnoreCase);

                string url1 = a.Groups[0].Value.ToString();
                wReq.Referer = url1;
                

                #endregion

                wReq.Referer = pageUrl;
                wReq.AllowAutoRedirect = true  ;

                CookieContainer CookieCon = new CookieContainer();
                if (cookie != "")
                {
                    CookieCollection cl = new CookieCollection();

                    foreach (string sc in cookie.Split(';'))
                    {
                        string ss = sc.Trim();


                        string s1 = ss.Substring(0, ss.IndexOf("="));
                        string s2 = ss.Substring(ss.IndexOf("=") + 1, ss.Length - ss.IndexOf("=") - 1);

                        if (s2.IndexOf(",") > 0 || s2.IndexOf(";") > 0)
                        {
                            s2 = s2.Replace(",", "%2c");
                            s2 = s2.Replace(";", "%3b");
                        }

                        cl.Add(new Cookie(s1, s2, "/"));
                    }


                    CookieCon.Add(new Uri(url), cl);
                    wReq.CookieContainer = CookieCon;
                }

                #region POST����

                //�ж��Ƿ���POST����
                if (Regex.IsMatch(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase))
                {

                    Match s = Regex.Match(url, @"(?<=<POST[^>]*>)[\S\s]*(?=</POST>)", RegexOptions.IgnoreCase);
                    string PostPara = s.Groups[0].Value.ToString();
                    byte[] pPara;

                    s = Regex.Match(url, @"(?<=<POST)[^>]*(?=>)", RegexOptions.IgnoreCase);
                    string postCode = s.Groups[0].Value.ToString();

                    if (postCode != "")
                        postCode = postCode.Substring(1, postCode.Length - 1).ToLower();

                    if (postCode == "" || postCode == "ascii")
                        pPara = Encoding.ASCII.GetBytes(PostPara);
                    else if (postCode == "utf8")
                        pPara = Encoding.UTF8.GetBytes(PostPara);
                    else
                        pPara = Encoding.GetEncoding(postCode).GetBytes(PostPara);


                    if (wReq.ContentType == "")
                        wReq.ContentType = "application/x-www-form-urlencoded";

                    wReq.ContentLength = pPara.Length;

                    wReq.Method = "POST";

                    System.IO.Stream reqStream = wReq.GetRequestStream();
                    reqStream.Write(pPara, 0, pPara.Length);
                    reqStream.Close();

                }
                else
                {
                    wReq.Method = "GET";
                }

                #endregion

                wRep = (System.Net.HttpWebResponse)wReq.GetResponse();


                if (Path.GetExtension(fName) == "" || Path.GetExtension(fName).Length > 5)
                {
                    //���ݷ��ص� Content-Disposition ����ȡ�ļ���
                    bool isContentDis = false;
                    for (int m = 0; m < wRep.Headers.Count; m++)
                    {
                        if (wRep.Headers.Keys[m].ToLower().ToString() == "Content-Disposition".ToLower())
                        {
                            isContentDis = true;
                        }
                    }

                    if (isContentDis == true)
                    {
                        string autoCode = wRep.CharacterSet == null ? "" : wRep.CharacterSet.ToString();
                        Encoding wCode = Encoding.UTF8;
                        if (autoCode != "")
                        {
                            wCode = System.Text.Encoding.GetEncoding(autoCode);
                            if (autoCode.ToLower() == "iso-8859-1" || autoCode == "")
                            {
                                wCode = Encoding.UTF8;
                            }
                        }

                        //����utf-8תһ���ĵ�������
                        string ConDisposition= wCode.GetString(wRep.Headers.ToByteArray());
                        Match a1 = Regex.Match(ConDisposition, @"Content-Disposition.+?\x0A", RegexOptions.IgnoreCase);

                        ConDisposition = a1.Groups[0].Value.ToString();


                        string tName = System.Web.HttpUtility.UrlDecode(ConDisposition.ToLower(), wCode);
                        tName = tName.Substring(tName.IndexOf("filename=") + 9, tName.Length - tName.IndexOf("filename=") - 9);

                        //���ж��Ƿ�������
                        //Regex objReg1 = new Regex("[^a-z|^A-Z|^0-1|^\\.|^/|^\\|^=|^'|^\"|^\\-|^:|^;]+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        //tName = objReg1.Replace(tName, "1");

                        tName = Regex.Replace(tName, "['|\"|\x0A|\x0D]", "");

                        if (rule != null && rule != "")
                            tName = RenameDownloadFile(tName, url, fPath, rule,dRow);

                        fName = fPath + "\\" + tName;
                        fName = RepeatFile(fPath, fName, fType, url);
                       
                    }

                }

                Stream responseSteam = wRep.GetResponseStream();

                if (startingPoint == 0)
                {
                    SaveFileStream = new FileStream(fName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    SaveFileStream = new FileStream(fName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }

                #region �������ݵı��롢ѹ����ʽת��
                System.IO.StreamReader reader;
                if (wRep.ContentEncoding == "gzip")
                {
                    GZipStream myGZip = new GZipStream(responseSteam, CompressionMode.Decompress);
                    reader = new System.IO.StreamReader(myGZip,Encoding.ASCII);
                    //myGZip.Close();
                }
                else if (wRep.ContentEncoding.StartsWith("deflate"))
                {
                    DeflateStream myDeflate = new DeflateStream(responseSteam, CompressionMode.Decompress);
                    reader = new System.IO.StreamReader(myDeflate, Encoding.ASCII);
                    //myDeflate.Close();

                 }
                else
                {

                    reader = new System.IO.StreamReader(responseSteam, Encoding.ASCII);
                    
                }

           

                #endregion


                int bytesSize;
                long fileSize = wRep.ContentLength;
                byte[] downloadBuffer = new byte[DEF_PACKET_LENGTH];

                try
                {
                    while ((bytesSize = reader.BaseStream.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
                    {

                        SaveFileStream.Write(downloadBuffer, 0, bytesSize);
                    }
                }
                catch { }

                reader.Close();
                reader.Dispose();


                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(0,"", cGlobalParas.LogType.Info, fName + "�����سɹ���", false));

                

            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(0, "",cGlobalParas.LogType.Info, fName + "������ʧ�ܣ�����ԭ��" + ex.Message , false));

                return "";
            }
            finally
            {
                if (SaveFileStream != null)
                {
                    SaveFileStream.Close();
                    SaveFileStream.Dispose();
                }

                if (wRep !=null)
                    wRep.Close();
            }

            //�ڴ˴���ˮӡ������
            if (isWatermark == true && Watermark != "")
            {
                SetWatermark(fName, Watermark);
            }

            return fName;

        }

        private void SetWatermark(string fName, string strWatermark)
        {
            //����ˮӡ����
            string wText = string.Empty;
            string fontFamily = string.Empty;
            double fontSize = 12;
            bool isFontBold = false;
            bool isFontItalic = false;
            string FontColor = string.Empty;
            cGlobalParas.WatermarkPOS pos = cGlobalParas.WatermarkPOS.RightTop;

            Match charSetMatch = Regex.Match(strWatermark, "(?<=<Text>).*?(?=</Text>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            wText = charSetMatch.Groups[0].ToString();

            charSetMatch = Regex.Match(strWatermark, "(?<=<FontFamily>).*?(?=</FontFamily>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            fontFamily = charSetMatch.Groups[0].ToString();

            charSetMatch = Regex.Match(strWatermark, "(?<=<FontSize>).*?(?=</FontSize>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            try
            {
                fontSize = double.Parse(charSetMatch.Groups[0].ToString());
            }
            catch { }

            charSetMatch = Regex.Match(strWatermark, "(?<=<FontBold>).*?(?=</FontBold>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            isFontBold = charSetMatch.Groups[0].ToString ().ToLower ()=="true"?true:false;

            charSetMatch = Regex.Match(strWatermark, "(?<=<FontItalic>).*?(?=</FontItalic>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            isFontItalic = charSetMatch.Groups[0].ToString().ToLower() == "true" ? true : false;

            charSetMatch = Regex.Match(strWatermark, "(?<=<FontColor>).*?(?=</FontColor>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            FontColor = charSetMatch.Groups[0].ToString();

            charSetMatch = Regex.Match(strWatermark, "(?<=<POS>).*?(?=</POS>)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            pos = EnumUtil.GetEnumName<cGlobalParas.WatermarkPOS>(charSetMatch.Groups[0].ToString());

            if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Program ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Enterprise ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Ultimate ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Server ||
                    ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.DistriServer)
                {
                    //���ж��Ƿ���һ��ͼƬ
                    string ename = System.IO.Path.GetExtension(fName).ToLower();
                    if (fName.EndsWith("gif") || fName.EndsWith("png") || fName.EndsWith("jpg")
                        || fName.EndsWith("jpeg") || fName.EndsWith("bmp"))
                    {
                        try
                        {
                            Watermark wmark = new Watermark(fName);
                            wmark.FontColor = System.Drawing.Color.FromArgb(int.Parse(FontColor));
                            FontStyle fstyle = new FontStyle();
                            if (isFontBold == true && isFontItalic == true)
                                fstyle = FontStyle.Bold | FontStyle.Italic;
                            else if (isFontItalic == true)
                                fstyle = FontStyle.Italic;
                            else if (isFontBold == true)
                                fstyle = FontStyle.Bold;


                            Font f = new System.Drawing.Font(fontFamily, (float)fontSize, fstyle);
                            wmark.Font = f;
                            wmark.Position = pos;
                            wmark.DrawText(wText);
                            wmark.ClearImage();

                            //System.IO.File.Delete(fName);
                            if (fName.EndsWith("gif"))
                                wmark.Image.Save(fName, System.Drawing.Imaging.ImageFormat.Gif);
                            else if (fName.EndsWith("png"))
                                wmark.Image.Save(fName, System.Drawing.Imaging.ImageFormat.Png);
                            else if (fName.EndsWith("jpg") || fName.EndsWith("jpeg"))
                                wmark.Image.Save(fName, System.Drawing.Imaging.ImageFormat.Jpeg);
                            else if (fName.EndsWith("bmp"))
                                wmark.Image.Save(fName, System.Drawing.Imaging.ImageFormat.Bmp);
                            else
                                wmark.Image.Save(fName);

                            wmark = null;
                        }
                        catch
                        {
                            //������󣬲�������
                        }
                    }
                }
                else
                {
                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Warning, "��ǰ�汾��֧��ˮӡ���ܣ����ȡ��ȷ�İ汾��", false));

                }

        }

        /// <summary>
        /// �����ļ��Ĵ�������������ļ�����
        /// </summary>
        /// <param name="fName"></param>
        /// <param name="fType"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string RepeatFile(string fPath, string fName, cGlobalParas.DownloadFileDealType fType,string url)
        {
           
              
            

                if (File.Exists(fName) == true)
                {
                    switch (fType)
                    {
                        case cGlobalParas.DownloadFileDealType.SaveCover:

                            break;
                        case cGlobalParas.DownloadFileDealType.SaveBySort:
                            for (int i = 0; i < 10000; i++)
                            {
                                if (File.Exists(fName) == true)
                                {
                                    string filename1 = Path.GetFileNameWithoutExtension(fName);

                                    //�鿴��β�ǲ��Ƿ��Ϲ淶�����ֲ���,�淶Ϊ�� -����
                                    int fNum=0;
                                    string sNum = "";
                                    Match charSetMatch = Regex.Match(filename1, "(?<=-)[\\d]+?(?=$)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                    if (charSetMatch==null ||charSetMatch.Groups.Count ==0)
                                    {
                                    }
                                    else
                                    {
                                        sNum=charSetMatch.Groups[0].ToString();
                                        if (string.IsNullOrEmpty(sNum.Trim()))
                                        {
                                        }
                                        else
                                        {
                                            fNum = int.Parse(charSetMatch.Groups[0].ToString());
                                            fNum = fNum + 1;
                                        }
                                    }

                                    if (string.IsNullOrEmpty(sNum))
                                        filename1 = filename1 + "-" + fNum;
                                    else
                                        filename1 = Regex.Replace(filename1 ,"(?<=-)[\\d]+?(?=$)","") + fNum;

                                    fName = fPath + "\\" + filename1 + Path.GetExtension(fName);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            break;
                        case cGlobalParas.DownloadFileDealType.SaveByDir:
                            string file = "";
                            if (url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
                            {
                                file = url.Substring(7, url.Length - 7);
                            }
                            else
                            {
                                file = url;
                            }
                            file = file.Replace("/", "-");

                            //file = file.Replace(".", "");

                            fName = fPath + "\\" + file; // +Path.GetExtension(path);
                            break;
                        default:
                            break;
                    }
                }
          

            return fName;
        }

        //ֻ��ƥ��ֻ��ƥ�����£����ص�����ֻ����һ��
        public DataTable MatchArticle(string htmlSource)
        {
            //����ƥ��϶���һ�У��������һ�У���ɾ��
            //δ��ƥ������ �������ܻ��ķ�������ȡ���������
            string sourceTag = HtmlHelper.HtmlCleanTag(htmlSource);
            ContentHandle cHandel = null;

            DataTable aData = new DataTable();
            aData.Columns.Add("Title");
            aData.Columns.Add("PublishDate");
            aData.Columns.Add("Content");
            aData.Columns.Add("Author");
            aData.Columns.Add("Source");

            DataRow drNew = null;

            //������ȡ����
            cHandel = new ContentHandle(sourceTag);
            cHandel.Process();

            drNew = aData.NewRow();
            string title = cHandel.GetTitle();
            if (title == "" || title == null)
            {
                Regex reg = new Regex(@"(?m)<title[^>]*>(?<title>(?:\w|\W)*?)</title[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Match mc = reg.Match(sourceTag);
                if (mc.Success)
                    title = mc.Groups["title"].Value.Trim();

            }

            drNew[0] = GetFullText(title);
            drNew[1] = cHandel.GetPublishTime();
            drNew[3] = cHandel.GetAuthor();
            drNew[4] = cHandel.GetSource();

            string content = ToolUtil.GetTextImage(cHandel.GetClearSubject());

            drNew[2] = content;

            aData.Rows.Add(drNew);

            cHandel = null;

            return aData;


        }

        #region �Զ���ȡ����

        //��ȡ��ҳtitile
        public string GetHtmlTitle(string HtmlSource)
        {
            string Splitstr = "(?<=<title>)[^<>]*(?=</title>)";

            if (HtmlSource == "")
                return "";

            Match aa = Regex.Match(HtmlSource, Splitstr, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string Title = aa.Groups[0].ToString();
            aa = null;
            return Title;
        }

        //��ȡ��ҳ���ı�
        public string GetFullText(string HtmlSource)
        {
            if (HtmlSource == "")
                return "";

            //ȥ���ı�ͷ���������ı�ǩ
            int index = HtmlSource.IndexOf(">");
            if (index < 120)
            {
                string str1 = HtmlSource.Substring(0, index + 1);

                Regex r = new Regex("[\u4e00-\u9fa5]");
                int f1 = r.Matches(str1).Count;
                if (f1 <= 0)
                {
                    HtmlSource = HtmlSource.Substring(index + 1, HtmlSource.Length - index - 1);
                }
            }

            string instr = HtmlHelper.HtmlFormat(HtmlSource);

            string m_outstr;

            m_outstr = instr.Clone() as string;
            m_outstr = new Regex(@"(?m)<script[^>]*>(\w|\W)*?</script[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<style[^>]*>(\w|\W)*?</style[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");
            m_outstr = new Regex(@"(?m)<select[^>]*>(\w|\W)*?</select[^>]*>", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(m_outstr, "");

            Regex objReg = new System.Text.RegularExpressions.Regex("(<[^>]+?>)|&nbsp;", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg.Replace(m_outstr, "");

            Regex objReg2 = new System.Text.RegularExpressions.Regex("(\\s)+", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            m_outstr = objReg2.Replace(m_outstr, " ");

            //�滻ת���Ŀո�
            m_outstr = System.Web.HttpUtility.HtmlDecode(m_outstr);

            //ȥ����β�ո�
            m_outstr = m_outstr.Trim();

            return m_outstr;
        }

        //ȥ����ҳ���ţ��滻<p>Ϊ \r\n
        public string GetText(string HtmlSource)
        {
            HtmlSource = Regex.Replace(HtmlSource, @"<[^/pbr][^>]*.|</[^pbr][^>]*.", @"\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            HtmlSource = GetFullText(HtmlSource);
            return HtmlSource;

        }

        //��ȡ��ҳ�еĵ����ʼ�
        public DataTable GetEmail(string HtmlSource)
        {

            if (HtmlSource == "")
                return null;

            Regex re = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            MatchCollection mc = re.Matches(HtmlSource);

            DataTable t = new DataTable();
            t.Columns.Add("Email");

            foreach (Match ma in mc)
            {
                t.Rows.Add(ma.ToString());
            }

            return t;

        }

        //��ȡ������ ����
        private int GetNumber(string strNumber)
        {
            if (strNumber == "")
                return 0;

            //ȥ������֮��ķ���
            strNumber = Regex.Replace(strNumber, @"[^\d]", "");

            return int.Parse(strNumber);

        }

        public int CompareDinosByChineseLength(string x, string y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    Regex r = new Regex("[\u4e00-\u9fa5]");
                    float xCount = (float)(r.Matches(x).Count) / (float)x.Length;
                    float yCount = (float)(r.Matches(y).Count) / (float)y.Length;

                    int retval = xCount.CompareTo(yCount);

                    if (retval != 0)
                    {
                        return retval;
                    }
                    else
                    {
                        return x.CompareTo(y);
                    }
                }
            }
        }

        ///<summary>
        /// ��ȡһ����ҳԴ���еı�ǩ�б�֧��Ƕ�ף�һ���ȥdiv��td������
        ///</summary>
        ///<param name="input"></param>
        ///<param name="tag"></param>
        ///<returns></returns>
        ///

        public List<string> GetTags1(string input, string tag)
        {
            string strReg = @"(<div).+?(</div>)";
            List<string> tags = null;

            try
            {
                Regex re = new Regex(strReg, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mc = re.Matches(input);

                tags = new List<string>();

                //foreach (Match ma in mc)
                //{
                //    tags.Add(ma.Value.ToString());
                //}


            }
            catch (System.OutOfMemoryException)
            {
                if (tags != null)
                    return tags;
                else
                    return null;
            }

            return tags;
        }

        public string[] GetTags(string input, string tag)
        {
            StringReader strReader = new StringReader(input);
            int lowerThanCharCounter = 0;
            int lowerThanCharPos = 0;
            Stack<int> tagPos = new Stack<int>();
            //List<string> taglist = new List<string>();
            string[] tags = new string[200];
            int i = 0;
            int rows = 0;

            try
            {
                while (true)
                {
                    try
                    {
                        int intCharacter = strReader.Read();
                        if (intCharacter == -1) break;

                        char convertedCharacter = Convert.ToChar(intCharacter);

                        if (lowerThanCharCounter > 0)
                        {
                            if (convertedCharacter == '>')
                            {
                                lowerThanCharCounter--;

                                string biaoqian = input.Substring(lowerThanCharPos, i - lowerThanCharPos + 1);
                                if (biaoqian.StartsWith(string.Format("<{0}", tag), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    tagPos.Push(lowerThanCharPos);
                                }
                                if (biaoqian.StartsWith(string.Format("</{0}", tag), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    if (tagPos.Count < 1)
                                        continue;
                                    int tempTagPos = tagPos.Pop();
                                    string strdiv = input.Substring(tempTagPos, i - tempTagPos + 1);
                                    //taglist.Add(strdiv);

                                    if (rows < 200)
                                    {
                                        tags[rows] = strdiv;
                                        rows++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        if (convertedCharacter == '<')
                        {
                            lowerThanCharCounter++;
                            lowerThanCharPos = i;
                        }
                    }
                    finally
                    {
                        i++;
                    }
                }
            }
            catch (System.Exception)
            {
                return null;
            }

            strReader.Dispose();
            tagPos = null;

            return tags;

            //return taglist;
        }

        ///<summary>
        /// ��ȡָ����ҳ��Դ�룬֧�ֱ����Զ�ʶ��
        ///</summary>
        ///<param name="url"></param>
        ///<returns></returns>
        private string getDataFromUrl(string url)
        {
            string str = string.Empty;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

            //����httpͷ
            request.AllowWriteStreamBuffering = true;
            request.Referer = "";
            request.Timeout = 10 * 1000;
            request.UserAgent = "";

            System.Net.HttpWebResponse response = null;
            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //����httpӦ���httpͷ���жϱ���
                    string characterSet = response.CharacterSet;
                    Encoding encode;
                    if (characterSet != "")
                    {
                        if (characterSet == "ISO-8859-1")
                        {
                            characterSet = "gb2312";
                        }
                        encode = Encoding.GetEncoding(characterSet);
                    }
                    else
                    {
                        encode = Encoding.Default;
                    }

                    //����һ���ڴ���������httpӦ����
                    Stream receiveStream = response.GetResponseStream();
                    MemoryStream mStream = new MemoryStream();

                    byte[] bf = new byte[255];
                    int count = receiveStream.Read(bf, 0, 255);
                    while (count > 0)
                    {
                        mStream.Write(bf, 0, count);
                        count = receiveStream.Read(bf, 0, 255);
                    }
                    receiveStream.Close();

                    mStream.Seek(0, SeekOrigin.Begin);

                    //���ڴ������ȡ�ַ���
                    StreamReader reader = new StreamReader(mStream, encode);
                    char[] buffer = new char[1024];
                    count = reader.Read(buffer, 0, 1024);
                    while (count > 0)
                    {
                        str += new String(buffer, 0, count);
                        count = reader.Read(buffer, 0, 1024);
                    }

                    //�ӽ��������ַ������ж�charset�������httpӦ��ı��벻һֱ
                    //��ô��ҳ��������Ϊ׼���ٴδ��ڴ��������¶�ȡ�ı�
                    Regex reg =
                        new Regex(@"<meta[\s\S]+?charset=(.*)""[\s\S]+?>",
                                  RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    MatchCollection mc = reg.Matches(str);
                    if (mc.Count > 0)
                    {
                        string tempCharSet = mc[0].Result("$1");
                        if (string.Compare(tempCharSet, characterSet, true) != 0)
                        {
                            encode = Encoding.GetEncoding(tempCharSet);
                            str = string.Empty;
                            mStream.Seek(0, SeekOrigin.Begin);
                            reader = new StreamReader(mStream, encode);
                            buffer = new char[255];
                            count = reader.Read(buffer, 0, 255);
                            while (count > 0)
                            {
                                str += new String(buffer, 0, count);
                                count = reader.Read(buffer, 0, 255);
                            }
                        }
                    }
                    reader.Close();
                    mStream.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (response != null)
                    response.Close();
            }
            return str;
        }


        private string GetMainContent(string input, int Type)
        {

            if (Type == 0)
            {
                try
                {
                    string reg1 = @"<(p|br)[^<]*>";
                    //string reg2 =
                    //    @"(\[([^=]*)(=[^\]]*)?\][\s\S]*?\[/\1\])|(?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";


                    //string reg2 =
                    //    @"(?<lj>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");])<a\s+[^>]*>[^<]{2,}</a>(?=[^\u4E00-\u9FA5\uFE30-\uFFA0,."");]))|(?<Style><style[\s\S]+?/style>)|(?<select><select[\s\S]+?/select>)|(?<Script><script[\s\S]*?/script>)|(?<Explein><\!\-\-[\s\S]*?\-\->)|(?<li><li(\s+[^>]+)?>[\s\S]*?/li>)|(?<Html></?\s*[^> ]+(\s*[^=>]+?=['""]?[^""']+?['""]?)*?[^\[<]*>)|(?<Other>&[a-zA-Z]+;)|(?<Other2>\#[a-z0-9]{6})|(?<Space>\s+)|(\&\#\d+\;)";

                    //1����ȡ��ҳ������div��ǩ
                    //List<string> list = GetTags(input, "div");

                    string[] list = GetTags(input, "div");

                    if (list == null)
                        return "";

                    float maxCount = 0;
                    int maxID = 0;
                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i] == null)
                            break;

                        string s = list[i].ToString();

                        Regex r = new Regex("[\u4e00-\u9fa5]");
                        float f1 = (float)r.Matches(s).Count / (float)s.Length;
                        if (f1 > maxCount)
                        {
                            maxCount = f1;
                            maxID = i;
                        }
                    }

                    if (maxCount == 0)
                        return "";

                    input = list[maxID];

                    list = null;

                    Regex r1 = new Regex("[\u4e00-\u9fa5]");
                    if (r1.Matches(input).Count < 150)
                    {
                        r1 = null;
                        return "";
                    }

                    //5����p��br�滻�������ռλ��[p][br]
                    input = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "[$1]");

                    //6��ȥ��HTML��ǩ����������
                    //input = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
                    input = ToolUtil.GetTextImage(input);

                    //7��������ռά���滻�ɻس��ͻ���
                    input = new Regex("\\[p]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n ");
                    input = new Regex("\\[br]", RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "\r\n");
                }
                catch (System.Exception)
                {
                    input = "";
                }
            }
            else if (Type == 1)
            {
                input = ToolUtil.GetTextImage(input);
            }

            return input;

        }

        public string GetArticlePublishDate(string input)
        {
            string mDate = @"\d\d\d\d[-|��]\d\d[-|��]\d\d(��)?[\s]+?\d\d[:]\d\d([:]\d\d)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strDate = charSetMatch.Groups[0].ToString();

            return strDate;
        }

        public string GetArticleSource(string input)
        {
            string mDate = @"��Դ[^<>]+?((<a).+?(>))?(?<Source>[^<>\s]+)(</a>)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strSource = charSetMatch.Groups["Source"].ToString();

            return strSource;
        }

        public string GetArticleSourceWeb(string input)
        {
            string mDate = @"��Դ[^<>]+?((<a).+?(>))?(?<Source>[^<>\s]+)(</a>)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strSource = charSetMatch.Groups[0].ToString();

            if (strSource != "")
            {
                mDate = "(?<=href=[\'|\"]?)\\S[^#+$<>\\s]*(?=[\'|\"]?)";
                charSetMatch = Regex.Match(strSource, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strSource = charSetMatch.Groups[0].ToString();
            }

            return strSource;
        }

        /// <summary>
        /// ����Urlת���ɱ���Ŀ¼��ַ
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetPathByUrl(string url)
        {
            string path="";
            try
            {
                Uri u = new Uri(url);
                path = u.PathAndQuery;
                path = Regex.Replace(path, "[\\|/|?|&|.|#|!|@|:|=|>|<|\\||*]", "");
                if (path.StartsWith("-"))
                    path = path.Substring(1, path.Length - 1);
            }
            catch { }
            return path;
        }

        private string RenameDownloadFile(string dFile, string downUrl, string strPath, string rule,DataRow dRow)
        {
            //��ʼ������򣬹����п����Ƕ�����Ҳ�������չ��
            string eName = Path.GetExtension(dFile);
            string fName = Path.GetFileNameWithoutExtension(dFile);

            while (Regex.IsMatch(rule, "{.*}"))
            {
                if (rule.IndexOf ("{Rename:OldName}")>-1)
                {
                    if(rule.IndexOf ("{RenameRegex")>-1)
                        rule = rule.Replace("{Rename:OldName}", ToolUtil.RegexReplaceTrans(downUrl));
                    else
                        rule = rule.Replace("{Rename:OldName}", fName);
                }
                else if (rule.IndexOf ("{Rename:Title}")>-1)
                {
                    string strMatch1 = "(?<=<title>)[^>]*?(?=</title>)";
                    Match s1 = Regex.Match(this.WebpageSource , strMatch1, RegexOptions.IgnoreCase);
                    string str = s1.Groups[0].Value;
                    rule = rule.Replace("{Rename:Title}", str);
                }
                else if (rule.IndexOf ("{Rename:CurrentDate}")>-1)
                {
                    string paraDate = System.DateTime.Now.Year.ToString ();
                    if (System.DateTime.Now.Month.ToString().Length == 1)
                        paraDate += "0" + System.DateTime.Now.Month.ToString();
                    else
                        paraDate += System.DateTime.Now.Month.ToString();

                    if (System.DateTime.Now.Day.ToString ().Length ==1)
                        paraDate += "0" + System.DateTime.Now.Day.ToString();
                    else
                        paraDate += System.DateTime.Now.Day.ToString();

                    //if (System.DateTime.Now.Hour .ToString ().Length ==1)
                    //    paraDate += "0" + System.DateTime.Now.Hour.ToString();
                    //else
                    //    paraDate += System.DateTime.Now.Hour.ToString();

                    //if (System.DateTime.Now.Minute.ToString().Length == 1)
                    //    paraDate += "0" + System.DateTime.Now.Minute.ToString();
                    //else
                    //    paraDate += System.DateTime.Now.Minute.ToString();

                    rule = rule.Replace("{Rename:CurrentDate}", paraDate);
                }
                else if (rule.IndexOf("{RenameRegex:") > -1)
                {
                    string Para = rule.Substring(rule.IndexOf("{RenameRegex:") + 13, 
                        rule.IndexOf ("}",rule.IndexOf("{RenameRegex:") + 13) - rule.IndexOf("{RenameRegex:") - 13);

                    string strMatch1 = Para;
                    Match s1 = Regex.Match(this.WebpageSource, strMatch1, RegexOptions.IgnoreCase);
                    string str = s1.Groups[0].Value;
                    rule = rule.Replace("{RenameRegex:" + Para + "}", str);
                }
                else if (rule.IndexOf("{RenameDataRule:") > -1)
                {
                    string dName = rule.Substring(rule.IndexOf("{RenameDataRule:") + 16,
                        rule.IndexOf("}", rule.IndexOf("{RenameDataRule:") + 16) - rule.IndexOf("{RenameDataRule:") - 16);

                    try
                    {
                        rule = dRow[dName].ToString();
                    }
                    catch (System.Exception ex)
                    {
                        throw new NetMinerException("�����ļ�/ͼƬ������ʧ�ܣ�������Ϣ��" + ex.Message );
                    }

                }

                string strRule = rule.Replace("{Rename:AutoID}", "");
                if (!Regex.IsMatch(strRule, "{.*}"))
                {
                    break;
                }
            }

            //����滻�Զ���ţ���ֹ������
            if (rule.IndexOf("{Rename:AutoID}") > -1)
            {
                string strMatch = "(?<={)[^}]*(?=})";
                Match s = Regex.Match(rule, strMatch, RegexOptions.IgnoreCase);
                string rulePara = s.Groups[0].Value;
               
                //string rule1 = rule.Replace("{" + rulePara + "}", "");
                string fdName = "";

                for (int i = 0; i < 10000; i++)
                {
                    fdName = strPath + "\\" + rule.Replace("{" + rulePara + "}", i.ToString()) + eName;
                    if (System.IO.Path.GetExtension(fdName) == "")
                        fdName = fdName + ".jpg";

                    if (File.Exists(fdName) == true)
                    {

                    }
                    else
                    {

                        break;
                    }
                }
                rule = Path.GetFileNameWithoutExtension(fdName);
                
            }

            fName = rule + eName;

            //�ж��Ƿ��к�׺�������û�к�׺�����򲹳䣬Ĭ��Ϊjpg
            if (System.IO.Path.GetExtension(fName) == "")
                fName = fName + ".jpg";
            return fName;
        }
        #endregion

        private string UrlCharCode(string value, cGlobalParas.WebCode uCode)
        {
            value = value.Replace("/", "%2f");
            value = value.Replace("?", "%3f");
            value = value.Replace("\\", "%5c");
            value = value.Replace("+", "%2b");
            value = value.Replace("=", "%3d");
            value = value.Replace("|", "%7c");
            value = value.Replace("&", "%26");
            value = value.Replace("\"", "%22");
            return value;
        }

        #region ����ƥ�� �����˳�ʱ��ʱ��Ϊ10�룬Ȼ��ǿ���˳�������ƥ���������������ʽ�����п������δ��Ӧ
        private delegate MatchCollection delegateMatchText(string regExpress, string source,out string err);
        private MatchCollection RegexMatch(string regExpress, string source)
        {
            delegateMatchText dMatchText = new delegateMatchText(this.dRegexMatch);

            //��ʼ���ú���,���Դ����� 
            string errmessage = "";
            IAsyncResult ir = dMatchText.BeginInvoke(regExpress, source,out errmessage ,  null, null);
            MatchCollection mc = null;


            //ir.AsyncWaitHandle.WaitOne(30000, false);

            //ÿ��ͣ��0.1�룬���ó�ʱʱ��Ϊ30�룬ǿ���˳�
            int i = 0;

            while (true)
            {
                Thread.Sleep(100);

                if (!ir.IsCompleted)
                {
                    //break;
                    i++;
                    if (((i * 100) / 1000) > 30)
                    {
                        //ǿ���˳�
                        break;
                    }
                }
                else
                {

                    mc = dMatchText.EndInvoke(out errmessage,ir);

                    break;
                }
            }

            //mc = dMatchText.EndInvoke( out errmessage  , ir);
            if (errmessage != "")
            {
                //��ʾ������
                throw new NetMinerException(errmessage);
            }
            return mc;
        }

        private MatchCollection dRegexMatch(string regExpress, string source,out string err)
        {
            try
            {
                Regex re = new Regex(regExpress, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                MatchCollection mc = re.Matches(source);
                err = "";
                //int count = mc.Count;
                return mc;
            }
            catch (System.Exception ex)
            {
                err = ex.Message;
                return null;
            }
        }
        #endregion

        #region SSL�Ĵ���
        private void GetCert(X509Certificate2 x)
        {
            x509c = x;
        }

        public bool RemoteCertificateValidationCallback(Object sender,
         X509Certificate certificate,
         X509Chain chain,
         SslPolicyErrors sslPolicyErrors)
        {
            return true;

            #region Validated Message
            //���û�д�ͱ�ʾ��֤�ɹ�
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {
                if ((SslPolicyErrors.RemoteCertificateNameMismatch & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    string errMsg = "֤�����Ʋ�ƥ��{0}" + sslPolicyErrors;
                    Console.WriteLine(errMsg);
                    throw new AuthenticationException(errMsg);
                }

                if ((SslPolicyErrors.RemoteCertificateChainErrors & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    string msg = "";
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        msg += "status code ={0} " + status.Status;
                        msg += "Status info = " + status.StatusInformation + " ";
                    }
                    string errMsg = "֤��������{0}" + msg;
                    Console.WriteLine(errMsg);
                    throw new AuthenticationException(errMsg);
                }
                string errorMsg = "֤����֤ʧ��{0}" + sslPolicyErrors;
                Console.WriteLine(errorMsg);
                throw new AuthenticationException(errorMsg);
            }
            #endregion
        }

        public static bool CheckStatusOfResponse(string strResponseXml)
        {
            XmlDocument doc = new XmlDocument();
            bool flag = false;
            string id = string.Empty;
            try
            {
                doc.LoadXml(strResponseXml);
                if (doc.DocumentElement.Attributes[0].Value == "fail")
                {
                    //��ȡ�������
                    string errCode = doc.DocumentElement.FirstChild.Attributes[0].Value;

                    //��ȡ������Ϣ
                    string errMsg = doc.DocumentElement.FirstChild.Attributes[1].Value;

                    //�׳��쳣��Ϣ
                    string[] errMsgParameters = new string[] { errCode, errMsg };
                    string exceptionMsg = string.Format("Error Code: {0}, Error Message: {1}", errMsgParameters);
                    throw new Exception(exceptionMsg);
                }
                else
                {
                    flag = true;
                }

                return flag;
            }
            catch (XmlException ex)
            {
                throw new XmlException(ex.Message, ex.InnerException);
            }
        }
        #endregion


        #region �¼�
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// д��־�¼�
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }

        /// <summary>
        /// ǿ��ֹͣ�ɼ�����
        /// </summary>
        private event EventHandler<cTaskEventArgs> e_ForcedStopped;
        internal event EventHandler<cTaskEventArgs> ForcedStopped
        {
            add { lock (m_eventLock) { e_ForcedStopped += value; } }
            remove { lock (m_eventLock) { e_ForcedStopped -= value; } }
        }
        #endregion

    }




    public interface ICertificateProvider
    {
        bool ClearCertificateCache();
        bool CreateRootCertificate();
        X509Certificate2 GetCertificateForHost(string sHostname);
        X509Certificate2 GetRootCertificate();
        bool rootCertIsTrusted(out bool bUserTrusted, out bool bMachineTrusted);
        bool TrustRootCertificate();
    }
    

}
