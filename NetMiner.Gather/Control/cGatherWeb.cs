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

///功能：采集数据处理
///完成时间：2009-3-2
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：无 
///版本：01.10.00
///修订：2011年5月19日进行修订，如果下载文件的数据内容进行了数据加工，则无法正确下载文件内容，原因是因为
///数据加工操作位于下载之前，如果数据下载地址进行了编辑，则下载模块无法正确识别下载的地址
///修改，将下载提前至数据编辑之前
///版本：01.20.00
///2013-7-7 修改，每次请求url后，都根据返回的头部信息进行cookie的更新
///2013-7-10 增加了对https的支持，但仅限于get方式，证书查找还有点问题
///在此不进行源码获取，由外部直接传入，此类就是一个单纯的数据采集匹配类

namespace NetMiner.Gather.Control
{

    public class cGatherWeb
    {
        DataTable tempData ;
        private static readonly int DEF_PACKET_LENGTH = 2048;
        private X509Certificate2 x509c = null;

        //定义一个代理信息控制类，并传引用给采集类，根据采集任务的
        //代理配置信息，去获取代理
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
       
        #region 属性
        private List<eWebpageCutFlag> m_CutFlag; 
        public List<eWebpageCutFlag> CutFlag
        {
            get { return m_CutFlag; }
            set { m_CutFlag = value; }
        }

        private string m_WebpageSource;
        /// <summary>
        /// 根据前后标记截取的网页源码
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
        /// 获取根据排重库判断是否重复
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
        /// 这个是用于http构造器使用的方法
        /// </summary>
        /// <param name="url"></param>
        /// <param name="webCode"></param>
        /// <param name="websource"></param>
        /// <param name="cookie"></param>
        /// <param name="referUrl"></param>
        /// <returns></returns>

        private readonly Object m_autoLock = new Object();

        //新的采集匹配核心
        /// <summary>
        /// 采集数据核心
        /// </summary>
        /// <param name="Url">采集的地址</param>
        /// <param name="webCode">网页编码</param>
        /// <param name="isUrlCode">网址是否需要编码</param>
        /// <param name="isTwoUrlCode">是否需要二次编码</param>
        /// <param name="UrlCode">网址编码</param>
        /// <param name="cookie">Cookie</param>
        /// <param name="startPos">网页截断符号：起始</param>
        /// <param name="endPos">网页截断符号：终止</param>
        /// <param name="sPath">下载文件存储路径</param>
        /// <param name="IsAutoUpdateHeader">是否自动更新头信息</param>
        /// <param name="IsExportGUrl">是否导出采集地址</param>
        /// <param name="IsExportGDateTime">是否导出采集事件</param>
        /// <param name="referUrl">来路地址</param>
        /// <param name="ndRow">导航页采集的数据</param>
        /// <param name="loopFlag">循环采集标记</param>
        /// <param name="loopIndex">循环了第几次</param>
        /// <param name="stopFlag">屏蔽标记</param>
        /// <returns></returns>
        public DataTable GetGatherData(string Url,string startPos, string endPos, 
            string sPath,  bool IsExportGUrl, bool IsExportGDateTime,DataRow ndRow,
            string loopFlag, int loopIndex, string rejectFlag, cGlobalParas.RejectDeal rejectDeal)
        {
            if (this.CutFlag == null || this.CutFlag.Count == 0)
                return null;

            if (string.IsNullOrEmpty(this.WebpageSource))
            {
                throw new NetMinerException("网页数据不存在，或设置了网页的前后获取标记，导致网页数据不存在！");
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
            
            //定义一个值，指示是否需要用到xpath
            bool IsXPath = false;
            List<cXPathExpression> xPaths = new List<cXPathExpression>();


            //定义一个值是否自动下载正文中包含的图片
            bool IsAutoDownloadImage = false;
            bool IsAutoFirstImage = false;

            //定义一直是否判断数据重复
            bool isRepeat = false;

            //此标记为自动维护，由系统监测到文章智能匹配标签为准
            bool IsArticleMatch = false;
            List<cSmartObj> SmartObjs = new List<cSmartObj>();

            #region 构建表结构，并构建截取正则

            //根据页面截取的标志构建表结构
            for (i = 0; i < this.CutFlag.Count; i++)
            {
                //如果是获取
                if (this.CutFlag[i].DataType == cGlobalParas.GDataType.HtmlCached)
                    isGetHtmlCached = true;
                
                tempData.Columns.Add(new DataColumn(this.CutFlag[i].Title, typeof(string)));

                if ((this.CutFlag[i].DataType == cGlobalParas.GDataType.Picture && IsDownloadFile == false) ||
                    (this.CutFlag[i].DataType == cGlobalParas.GDataType.File && IsDownloadFile == false))
                {
                    IsDownloadFile = true;
                }

                //判定是否有自动编号，并记录自动编号的顺序ID
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

            
                #region 正则表达式部分
                //根据用户指定的页面截取位置构造正则表达式
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
                            #region 根据采集规则生成正则表达式
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

                        //判断智能匹配的标签
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

            #region 获取网页源码

            int rowCount = this.CutFlag.Count;

            //try
            //{
            //    //GetHtml(Url, webCode, isUrlCode,isTwoUrlCode, UrlCode, ref cookie, startPos, endPos, true, 
            //    //    IsAutoUpdateHeader, referUrl,isGatherCoding, CodingFlag, CodingUrl, Plugin);

            //}
            //catch (System.Web.HttpException ex)
            //{
            //    throw new NetMinerException("获取网页源码出错：" + ex.Message);
            //}

            #endregion
          
            #region 判断是否为循环采集，如果是，则根据编号取出循环的内容，替换源码
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


            #region  在此判断是否被屏蔽了，如果被屏蔽则强制停止采集任务
            if (!string.IsNullOrEmpty(rejectFlag))  // && !(this.m_IsProxy==true && this.m_IsProxyFirst==false ))
            {
                if (this.WebpageSource.Contains(rejectFlag))
                {
                    if (e_Log != null)
                    {
                        e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Warning, "系统检测到存在指定停止采集的符号信息，采集任务将自动停止。", false));
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
                            throw new NetMinerException("检测到网站屏蔽了采集信息，引发采集错误！");
                        case cGlobalParas.RejectDeal.UpdateCookie:

                            break;
                        case cGlobalParas.RejectDeal.Coding:
                            //开始打码

                            break;
                    }
                    
                }
            }
            #endregion

            int rows = 0; //统计共采集了多少行

            #region 正则匹配并输出数据
            try
            {
                //开始获取截取内容
                
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
                        //先注释掉了，但不知道是否有问题，观察一段时间
                        //注释的目的是：当如果存在规则采集和职能采集，如果规则采集不到数据，则有可能导致
                        //智能采集也会采集到数据，但规则的无数据则直接返回为空了

                        //tempData = null;
                        //return tempData;
                    //}

                    //开始根据采集的数据构造数据表进行输出
                    //在此需要处理采集数据有可能错行的问题

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
                throw new NetMinerException("正则匹配数据时发生错误，请检查采集规则，如果使用了通配符，请重点检查此处内容。错误信息：" + ex.Message);
            }
            #endregion

            #region xPath数据获取处理
            //根据xPath获取数据

            try
            {
                if (IsXPath == true)
                {
                    DataTable xPathData = null;

                    xPathData = GetXPathData(this.WebpageSource, xPaths);

                    //开始合并xpath采集获取的数据
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
                throw new NetMinerException("可视化获取数据时发生错误，请检查采集数据规则，错误信息：" + ex.Message);
            }
            #endregion

            i = 0;

            #region 如果配置了智能化的提取，则在此处理
            try
            {
                if (IsArticleMatch == true)
                {
                    DataTable smartGather = null;
                    smartGather = MatchArticle(this.WebpageSource);
                    if (smartGather != null && smartGather.Rows.Count != 0)
                    {
                        //开始合并智能采集获取的数据
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
                throw new NetMinerException("智能化提取数据发生错误，请根据错误排错，如任然出错，请使用其他方法配置规则，并采集数据，错误信息：" + ex.Message);
            }
            #endregion


            #region 在处理数据加工规则前，先处理自动编号的问题
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

            #region 开始进行输出控制，进行获取数据加工

            bool isExportDownloadFilePath = false;

            //是否采用插件加工，插件加工的是一个表，所以加工一次即可，无需
            //多次加工，所以，加入标记，如果已经加工即不再进行加工处理
            

            //在此判断是否需要在输出时进行数据的限制,根据任务版本1.2增加了数据输出的限制
            

            for (i = 0; i < this.CutFlag.Count; i++)
            {
                //if (this.CutFlag[i].RuleByPage != (int)cGlobalParas.GatherRuleByPage.NonePage)
                //{
                
                    //每一个采集数据只能用于加工一次
                    //bool isEditByPlugin = false;

                    for (j = 0; j < this.CutFlag[i].ExportRules.Count; j++)
                    {
                        string exportRule = "";

                        switch (this.CutFlag[i].ExportRules[j].FieldRuleType)
                        {
                       
                            //先进行删除操作
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportDelData.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message );
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
                                            //表示是参数,获取参数值
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportInclude.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportNoWebSign.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportPrefix.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportReplace:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        //替换的格式是：<OldValue:><NewValue:>，所以首先判断必须字符串长度大于22
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportReplace.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSuffix.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" +  cGlobalParas.ExportLimit.ExportTrimLeft.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportTrimRight.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportTrim.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportRegexReplace:
                                try
                                {
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        //替换的格式是：<OldValue:><NewValue:>，所以首先判断必须字符串长度大于22
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportRegexReplace.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSetEmpty.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                            string strValue = mUni.Result("${code}");   //代码
                                            int CharNum = Int32.Parse(strValue.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                                            string ch = string.Format("{0}", (char)CharNum);
                                            strUnicode = strUnicode.Replace(strValue, ch);
                                        }

                                        tempData.Rows[index][i] = strUnicode;
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportConvertUnicode.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportConvertHtml.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportHaveCRLF.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportHavePImgNoneCSS.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportReplaceCRLF.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportWrap :
                                try
                                {
                                    //拆分行
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
                                                    //第一行直接修改原来数据表的值，其他的值进行新表添加
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

                                        //合并两个表的数据
                                        tempData.Merge(tData);
                                        tData = null;

                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportWrap.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportGatherdata :
                                try
                                {
                                    //通过正则二次提取数据
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportGatherdata.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                                }
                                break;
                            case cGlobalParas.ExportLimit.ExportFormatString :
                                try
                                {
                                    //格式化字符串
                                    string fRule1 = this.CutFlag[i].ExportRules[j].FieldRule;
                                    for (int index = 0; index < tempData.Rows.Count; index++)
                                    {
                                        try
                                        {
                                            string strFormat = tempData.Rows[index][i].ToString();
                                            if (fRule1.IndexOf("yy") > 0)
                                            {
                                                //表示是日期
                                                tempData.Rows[index][i] = string.Format(fRule1, DateTime.Parse(strFormat));
                                            }
                                            else
                                            {
                                                //数字
                                                tempData.Rows[index][i] = string.Format(fRule1, double.Parse(strFormat));
                                            }

                                        }
                                        catch { }
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportFormatString.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportGatherUrl.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportGather.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportMakeGather.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportEncodingString.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportEncoding.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportValue.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSynonymsReplace.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportMergeParagraphs.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportPY.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportByPlugin.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportDict.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportBase64.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                            //去第一个符号 及 数字
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportNumber.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportHaveIMG.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportToAbsoluteUrl.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportToAbsoluteUrl.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportBase64Encoding.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportConvertDateTime.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportSubstring.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportToMD5.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                    throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription () + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportDelInvalidChar.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
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
                                throw new NetMinerException("数据加工规则：" + cGlobalParas.ExportLimit.ExportUUID.GetDescription() + "，请对采集的数据进行检查！错误信息：" + ex.Message);
                            }
                            break;

                        default:

                                break;
                        }
                    //}
                }
              

            }

            #endregion

            #region 在此进行数据排重的处理
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

                    //开始排重
                    if (!this.m_DataRepeat.Add(ref dCode, false))
                    {
                        //重复了，开始处理
                        m_IsDataRepeat = true ;
                        tempData.Rows.Remove(tempData.Rows[rIndex]);
                        rIndex--;
                    }
                }
            }
            #endregion

            #region 针对采集需要下载文件的字段进行文件下载处理
            //判断是否存在有下载文件的任务，如果有，则开始下载，因为此功能设计最初是下载图片使用
            //并非是专用的下载工具，所以对下载处理并没有单独进行线程处理

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

                    //2013-7-16修改，因为有些行有可能会删除，rows是最初的正则
                    //匹配的行数
                    for (i = 0; i < tempData.Rows.Count; i++)
                    {
                        for (j = 0; j < this.CutFlag.Count; j++)
                        {
                            FileUrl = tempData.Rows[i][j].ToString();

                            if ((this.CutFlag[j].DataType == cGlobalParas.GDataType.File && FileUrl !="")
                                || (this.CutFlag[j].DataType == cGlobalParas.GDataType.Picture && FileUrl != ""))
                            {
                                
                                //开始获取下载文件名称
                                Regex s = new Regex(@"(?<=/)[^/]*", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                                MatchCollection urlstr = s.Matches(FileUrl);
                                if (urlstr.Count == 0)
                                    DownloadFileName = FileUrl;
                                else
                                    DownloadFileName = urlstr[urlstr.Count - 1].ToString();

                                //判断文件名的合法性
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
                                
                                //替换掉所有非法字符
                                DownloadFileName =Regex.Replace (DownloadFileName ,"[:|&|\\|/|!|?|*|<|>|\\|]","",RegexOptions.IgnoreCase );

                                if (this.CutFlag[j].DownloadFileSavePath == "")
                                {
                                    //sPath = cTool.getPrjPath() + "data\\tem_file";
                                }
                                else
                                {
                                    sPath = this.CutFlag[j].DownloadFileSavePath;
                                }

                                //在此处理下载文件重命名的问题
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

                                //将图片路径补充完整
                                DownloadFileName = sPath + "\\" + DownloadFileName;

                                //开始现在图片
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

                                //增加下载图片的地址,判断是否需要处理网址
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
                                    

                                //文件下载下来之后，需要判断是否是一张图片，并且需要进行OCR识别
                                if (isOCR==true)
                                {
                                    //if (e_Log != null)
                                    //    e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Info, "系统检测到有图片需要进行文字识别，请等待...", false));

                                    //cOcr ocr = new cOcr(this.m_workPath);
                                    //string strText = ocr.OcrText(DownloadFileName, ocrSCore,isNumberOcr);
                                    //ocr = null;

                                    //if (e_Log != null)
                                    //    e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Info, "图片识别工作已经完成", false));

                                    ////替换文本
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

            #region 开始处理是否自动下载文本图片 注意下载图片是一个同步的处理方式，并非异步
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

            #region 处理保存网页快照的问题
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

                                //替换掉所有非法字符
                                htmlFileName = Regex.Replace(htmlFileName, "[:|&|\\|/|!|?|*|<|>|\\|]", "", RegexOptions.IgnoreCase);

                                if (this.CutFlag[j].DownloadFileSavePath != "")
                                {
                                    htmlPath = this.CutFlag[j].DownloadFileSavePath;
                                }

                                //在此处理下载文件重命名的问题
                                for (int e = 0; e < this.CutFlag[j].ExportRules.Count; e++)
                                {
                                    if (this.CutFlag[j].ExportRules[e].FieldRuleType == cGlobalParas.ExportLimit.ExportRenameDownloadFile)
                                    {
                                        htmlFileName = RenameDownloadFile(htmlFileName, Url, sPath, this.CutFlag[j].ExportRules[e].FieldRule, tempData.Rows[0]);
                                    }
                                }

                                htmlFileName = sPath + "\\" + htmlFileName;

                                //写文件
                                FileStream fs = new FileStream(htmlFileName, FileMode.Create, FileAccess.Write, FileShare.Write);
                                StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);
                                sw.Write(this.WebpageSource);
                                sw.Close();
                                sw.Dispose();
                                fs.Close();
                                fs.Dispose();
                             
                                //替换文本
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
        /// 检测指定内容中是否含有可下载的文件
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
                    //获取数据，分析url，并下载
                    for (int j = 0; j < tempData.Rows.Count; j++)
                    {
                        string strContent = tempData.Rows[j][i].ToString();

                        //获取str中的所有链接地址
                        //string strRef = "(?<=href=[\'|\"])\\S[^#+$<>\\s]*(?=[\'|\"])";
                        string strRef = "(?<=<img.*src=[\'|\"]?)\\S[^#+$<>\\s'\"]*(?=[\'|\"]?)";
                        MatchCollection matches = new Regex(strRef,RegexOptions.IgnoreCase).Matches(strContent);

                        foreach (Match match in matches)
                        {
                            string strUrl = match.Value.ToString();
                            strUrl = Regex.Replace(strUrl, "[\'|\"]", "");

                            //去除链接地址，并判断这个链接地址是否是一个图片
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
                                //下载文件

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

                                //在此处理下载文件重命名的问题
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
        /// 根据xPath采集页面数据
        /// 2013-1-28进行修改，增加支持xpath参数，可以进行多行数据的匹配
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
                //开始检索数据

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

        //下载文件，是一个单线程的方法，适用于小文件下载，仅支持http方式
        //在现在过程中处理水印的问题，同时在此根据文件重名规则进行重名处理，但文件重命名规则不在此处理
        public string DownloadFile(string url, string pageUrl, string fName,
            cGlobalParas.DownloadFileDealType fType,string cookie,string rule,bool isWatermark,string Watermark,DataRow dRow)
        {
           
            if (e_Log !=null)
                e_Log(this, new cGatherTaskLogArgs(0,"", cGlobalParas.LogType.Info, "系统检测到需要下载文件，并已启动下载操作...", false));

            System.Net.HttpWebRequest wReq = null;
            System.Net.HttpWebResponse wRep = null;
            FileStream SaveFileStream = null;

            int startingPoint = 0;

            //判断目录是否存在

            string tmpPath = fName.Replace(System.IO.Path.GetFileName(fName), "");

            string fPath = System.IO.Path.GetDirectoryName(tmpPath);
            string fName1 = Path.GetFileName(fName);
            fName1 = Regex.Replace(fName1, "[/|\\|:|*|\\?|\"|<|>|\\|]", "");
            fName =fPath  + "\\" + fName1;
            if (Directory.Exists(fPath) == false)
            {
                Directory.CreateDirectory(fPath);
            }

            //判断文件是否存在，并根据重名规则进行处理
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

                #region 通讯header

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

                #region POST数据

                //判断是否含有POST参数
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
                    //根据返回的 Content-Disposition 来获取文件名
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

                        //先用utf-8转一下文档的名称
                        string ConDisposition= wCode.GetString(wRep.Headers.ToByteArray());
                        Match a1 = Regex.Match(ConDisposition, @"Content-Disposition.+?\x0A", RegexOptions.IgnoreCase);

                        ConDisposition = a1.Groups[0].Value.ToString();


                        string tName = System.Web.HttpUtility.UrlDecode(ConDisposition.ToLower(), wCode);
                        tName = tName.Substring(tName.IndexOf("filename=") + 9, tName.Length - tName.IndexOf("filename=") - 9);

                        //先判断是否有乱码
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

                #region 请求数据的编码、压缩格式转换
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
                    e_Log(this, new cGatherTaskLogArgs(0,"", cGlobalParas.LogType.Info, fName + "　下载成功！", false));

                

            }
            catch (System.Exception ex)
            {
                if (e_Log != null)
                    e_Log(this, new cGatherTaskLogArgs(0, "",cGlobalParas.LogType.Info, fName + "　下载失败，错误原因：" + ex.Message , false));

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

            //在此处理水印的问题
            if (isWatermark == true && Watermark != "")
            {
                SetWatermark(fName, Watermark);
            }

            return fName;

        }

        private void SetWatermark(string fName, string strWatermark)
        {
            //解析水印数据
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
                    //先判断是否是一个图片
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
                            //捕获错误，不做处理
                        }
                    }
                }
                else
                {
                    if (e_Log != null)
                        e_Log(this, new cGatherTaskLogArgs(0, "", cGlobalParas.LogType.Warning, "当前版本不支持水印功能，请获取正确的版本！", false));

                }

        }

        /// <summary>
        /// 根据文件的处理类别来处理文件名称
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

                                    //查看结尾是不是符合规范的数字部分,规范为： -数字
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

        //只能匹配只能匹配文章，返回的数据只能是一行
        public DataTable MatchArticle(string htmlSource)
        {
            //文章匹配肯定是一行，如果大于一行，则删除
            //未能匹配数据 采用智能化的方法，提取标题和正文
            string sourceTag = HtmlHelper.HtmlCleanTag(htmlSource);
            ContentHandle cHandel = null;

            DataTable aData = new DataTable();
            aData.Columns.Add("Title");
            aData.Columns.Add("PublishDate");
            aData.Columns.Add("Content");
            aData.Columns.Add("Author");
            aData.Columns.Add("Source");

            DataRow drNew = null;

            //内容提取处理
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

        #region 自动获取内容

        //获取网页titile
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

        //获取网页纯文本
        public string GetFullText(string HtmlSource)
        {
            if (HtmlSource == "")
                return "";

            //去除文本头部不完整的标签
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

            //替换转码后的空格
            m_outstr = System.Web.HttpUtility.HtmlDecode(m_outstr);

            //去除首尾空格
            m_outstr = m_outstr.Trim();

            return m_outstr;
        }

        //去除网页符号，替换<p>为 \r\n
        public string GetText(string HtmlSource)
        {
            HtmlSource = Regex.Replace(HtmlSource, @"<[^/pbr][^>]*.|</[^pbr][^>]*.", @"\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            HtmlSource = GetFullText(HtmlSource);
            return HtmlSource;

        }

        //获取网页中的电子邮件
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

        //获取纯数字 整数
        private int GetNumber(string strNumber)
        {
            if (strNumber == "")
                return 0;

            //去除数字之间的符号
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
        /// 获取一个网页源码中的标签列表，支持嵌套，一般或去div，td等容器
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
        /// 获取指定网页的源码，支持编码自动识别
        ///</summary>
        ///<param name="url"></param>
        ///<returns></returns>
        private string getDataFromUrl(string url)
        {
            string str = string.Empty;
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(url);

            //设置http头
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
                    //根据http应答的http头来判断编码
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

                    //声明一个内存流来保存http应答流
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

                    //从内存流里读取字符串
                    StreamReader reader = new StreamReader(mStream, encode);
                    char[] buffer = new char[1024];
                    count = reader.Read(buffer, 0, 1024);
                    while (count > 0)
                    {
                        str += new String(buffer, 0, count);
                        count = reader.Read(buffer, 0, 1024);
                    }

                    //从解析出的字符串里判断charset，如果和http应答的编码不一直
                    //那么以页面声明的为准，再次从内存流里重新读取文本
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

                    //1、获取网页的所有div标签
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

                    //5、把p和br替换成特殊的占位符[p][br]
                    input = new Regex(reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "[$1]");

                    //6、去掉HTML标签，保留汉字
                    //input = new Regex(reg2, RegexOptions.Multiline | RegexOptions.IgnoreCase).Replace(input, "");
                    input = ToolUtil.GetTextImage(input);

                    //7、把特殊占维护替换成回车和换行
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
            string mDate = @"\d\d\d\d[-|年]\d\d[-|月]\d\d(日)?[\s]+?\d\d[:]\d\d([:]\d\d)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strDate = charSetMatch.Groups[0].ToString();

            return strDate;
        }

        public string GetArticleSource(string input)
        {
            string mDate = @"来源[^<>]+?((<a).+?(>))?(?<Source>[^<>\s]+)(</a>)?";

            Match charSetMatch = Regex.Match(input, mDate, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string strSource = charSetMatch.Groups["Source"].ToString();

            return strSource;
        }

        public string GetArticleSourceWeb(string input)
        {
            string mDate = @"来源[^<>]+?((<a).+?(>))?(?<Source>[^<>\s]+)(</a>)?";

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
        /// 根据Url转换成本地目录地址
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
            //开始处理规则，规则有可能是多个，且不处理扩展名
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
                        throw new NetMinerException("下载文件/图片重命名失败，错误信息：" + ex.Message );
                    }

                }

                string strRule = rule.Replace("{Rename:AutoID}", "");
                if (!Regex.IsMatch(strRule, "{.*}"))
                {
                    break;
                }
            }

            //最后替换自动编号，防止出问题
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

            //判断是否有后缀名，如果没有后缀名，则补充，默认为jpg
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

        #region 正则匹配 设置了超时的时间为10秒，然后强制退出，正则匹配如果由于正则表达式有误，有可能造成未响应
        private delegate MatchCollection delegateMatchText(string regExpress, string source,out string err);
        private MatchCollection RegexMatch(string regExpress, string source)
        {
            delegateMatchText dMatchText = new delegateMatchText(this.dRegexMatch);

            //开始调用函数,可以带参数 
            string errmessage = "";
            IAsyncResult ir = dMatchText.BeginInvoke(regExpress, source,out errmessage ,  null, null);
            MatchCollection mc = null;


            //ir.AsyncWaitHandle.WaitOne(30000, false);

            //每次停留0.1秒，设置超时时间为30秒，强制退出
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
                        //强制退出
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
                //表示出错了
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

        #region SSL的处理
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
            //如果没有错就表示验证成功
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            else
            {
                if ((SslPolicyErrors.RemoteCertificateNameMismatch & sslPolicyErrors) == SslPolicyErrors.RemoteCertificateNameMismatch)
                {
                    string errMsg = "证书名称不匹配{0}" + sslPolicyErrors;
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
                    string errMsg = "证书链错误{0}" + msg;
                    Console.WriteLine(errMsg);
                    throw new AuthenticationException(errMsg);
                }
                string errorMsg = "证书验证失败{0}" + sslPolicyErrors;
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
                    //获取错误编码
                    string errCode = doc.DocumentElement.FirstChild.Attributes[0].Value;

                    //获取错误信息
                    string errMsg = doc.DocumentElement.FirstChild.Attributes[1].Value;

                    //抛出异常信息
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


        #region 事件
        private readonly Object m_eventLock = new Object();
        /// <summary>
        /// 写日志事件
        /// </summary>
        private event EventHandler<cGatherTaskLogArgs> e_Log;
        internal event EventHandler<cGatherTaskLogArgs> Log
        {
            add { lock (m_eventLock) { e_Log += value; } }
            remove { lock (m_eventLock) { e_Log -= value; } }
        }

        /// <summary>
        /// 强制停止采集任务
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
