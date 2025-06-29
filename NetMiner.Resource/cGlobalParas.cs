using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.ComponentModel;

///功能：全局 变量 常量 等
///完成时间：2009-9-19
///作者：一孑
///遗留问题：无
///开发计划：无
///说明：增加对语言资源文件的处理。
///版本：01.80.00
///修订：无
namespace NetMiner.Resource
{
    public static class cGlobalParas
    {

        #region 枚举常量
        public enum TaskState
        {
            [Description("未启动")]
            UnStart = 1010,
            [Description("启动")]
            Started =1011,
            [Description("中止")]
            Aborted =1012,
            [Description("等待")]
            Waiting =1013,
            [Description("正在运行")]
            Running = 1014,
            [Description("暂停")]
            Pause = 1015,
            [Description("停止")]
            Stopped = 1016,
            [Description("导出中...")]
            Exporting = 1017,
            [Description("完成")]
            Completed =1018,
            [Description("失败")]
            Failed = 1019,
            [Description("发布中...")]
            Publishing =1020,
            [Description("发布停止")]
            PublishStop =1021,
            [Description("发布失败")]
            PublishFailed =1022,
            [Description("客户端请求中...")]
            Request =1023,                      //表示此任务被客户端正在请求索取，被占用
            [Description("等待发布")]
            WaitingPublish =1024,
            [Description("远程未启动")]
            RemoteUnstart =1025,                //远程请求任务成功，但没有开始运行
            [Description("远程运行中")]
            RemoteRunning =1026,                //表示此任务不是由本机运行，而是由远程运行的
            [Description("异常停止")]
            ErrStop=1027,
        }

        public enum GatherResult
        {
            [Description("采集未启动")]
            UnGather=1077,
            [Description("采集成功")]
            GatherSucceed =1071,
            [Description("采集失败")]
            GatherFailed =1072,
            [Description("采集停止")]
            GatherStoppedByUser =1076,
            [Description("发布成功")]
            PublishSuccees =1073,
            [Description("发布失败")]
            PublishFailed =1074,
            [Description("")]
            ModifySave =1075,
        }

        public enum DownloadResult
        {
            Succeed=1081,
            Failed=1082,
            Err=1083,
        }


        public enum GDataType
        {
            [Description("文本")]
            Txt=1091,              //文本
            [Description("图片")]
            Picture =1092,          //图片   
            [Description("Flash")]
            Flash =1093,            //Flash
            [Description("文件")]
            File =1094,             //文件
            [Description("智能提取页面标题")]
            AutoTitle =1095,        //自动正文
            [Description("智能提取文章正文")]
            AutoContent =1096,      //自动标题
            [Description("智能提取作者")]
            AutoAuthor =1100,       //自动作者
            [Description("智能提取来源")]
            AutoSource =1101,       //自动来源
            [Description("智能提取发布时间")]
            AutoPublishDate =1097,        //自动表格
            [Description("网页快照")]
            HtmlCached =1099,       //网页快照
        }

        public enum GatherThreadState
        {
            UnStart = 1030,
            Started = 1031,
            Running = 1032,
            Stopped = 1033,
            Completed = 1034,
            Failed = 1035,
            Aborted=1036,
            Waiting=1037,
        }

        public enum PublishThreadState
        {
            UnStart=5051,
            Running=5052,
            Complete=5053,
            Stopped=5055,
            Failed=5054,
        }

        public enum PublishState
        {
            Ini=5061,
            Succeed=5062,
            UnGetChainback=5064,
            Failed=5063,
            Publishing=5065,
        }

        public enum PublishResult
        {
            UnPublished=5071,
            Succeed=5072,
            Fail=5073,
        }

        public enum FormState
        {
            New=1021,
            Edit=1022,
            Browser=1023,
        }

        public enum TaskRunType
        {
            [Description("仅采集")]
            OnlyGather=1041,                  //仅采集
            [Description("采集并发布")]
            GatherExportData =1042,            //采集并发布
            [Description("直接入库")]
            OnlySave =1043,                    //直接入库
            [Description("雷达任务")]
            Radar=1044,
        }

        public enum TaskType
        {
            [Description("根据网址采集网页数据")]
            HtmlByUrl=1051,
            [Description("爬虫任务")]
            HtmlByWeb =1053,
            [Description("外部参数任务")]
            ExternalPara=1054,
        }

        public enum PublishType
        {
            [Description("不发布数据")]
            NoPublish=1060,
            [Description("发布至Access")]
            PublishAccess = 1061,
            [Description("发布至MS SqlServer")]
            PublishMSSql = 1062,
            [Description("发布至文本文件")]
            PublishTxt =1063,
            [Description("发布至Excel（2007格式）")]
            PublishExcel =1064,
            [Description("发布至MySql")]
            PublishMySql =1065,
            [Description("发布至CSV文件")]
            PublishCSV =1067,
            [Description("发布数据")]
            PublishData = 1068,
            [Description("通过插件发布数据")]
            publishPlugin =1069,
            [Description("通过发布模板发布数据")]
            publishTemplate =1070,
            [Description("发布至Oracle")]
            publishOracle =1071,
            [Description("发布至Word（2007格式）")]
            publishWord =1072,

        }

        public enum LimitSign
        {
            [Description("不做任意格式的限制")]
            NoLimit = 2001,          //不做任意格式的限制
            [Description("只匹配非网页数据")]
            NoWebSign = 2002,        //只匹配非网页数据
            [Description("只匹配中文")]
            OnlyCN = 2003,           //只匹配中文
            [Description("只匹配双字节字符")]
            OnlyDoubleByte =2004,     //只匹配双字节字符
            [Description("只匹配数字")]
            OnlyNumber =2005,         //只匹配数字
            [Description("只匹配字母数字及常用字符")]
            OnlyChar =2006,           //只匹配字母数字及常用字符
            [Description("自定义匹配字符")]
            CustomMatchChar =2007,    //自定义匹配字符
            [Description("自定义正则匹配表达式")]
            Custom = 2008,           //自定义正则匹配表达式 
            [Description("自动捕获文章标题")]
            AutoTitle =2009,          //自动捕获文章标题
            [Description("自动捕获文章正文")]
            AutoContent =2010,        //自动捕获文章正文
            [Description("自动捕获发布时间")]
            AutoPublish =2011,        //自动捕获发布时间
            [Description("自动捕获文章来源")]
            AutoSource =2012,         //自动捕获文章来源
        }

        public enum ExportLimit
        {
            [Description("不做输出控制")]
            ExportNoLimit =2040,        //不做输出控制
            [Description("输出时去掉网页符号")]
            ExportNoWebSign = 2041,    //输出时去掉网页符号
            [Description("输出时附加前缀")]
            ExportPrefix = 2042,       //输出时附加前缀
            [Description("输出时附加后缀")]
            ExportSuffix = 2043,       //输出时附加后缀
            [Description("左起去掉字符")]
            ExportTrimLeft = 2044,     //左起去掉字符
            [Description("右起去掉字符")]
            ExportTrimRight = 2045,    //右起去掉字符
            [Description("替换其中符合条件的字符")]
            ExportReplace = 2046,      //替换其中符合条件的字符
            [Description("去掉字符串首尾空格")]
            ExportTrim = 2047,         //去掉字符串首尾空格
            [Description("输出时采用正则表达式进行替换")]
            ExportRegexReplace = 2048, //输出时采用正则表达式进行替换
            [Description("根据指定条件删除整行")]
            ExportDelData =2049,        //根据指定条件删除整行
            [Description("必须包含指定条件")]
            ExportInclude = 2140,      //必须包含指定条件
            [Description("将符合条件的数据置为空")]
            ExportSetEmpty =2141,       //将符合条件的数据置为空
            [Description("将Unicode转成汉字")]
            ExportConvertUnicode =2142, //将Unicode转成汉字
            [Description("需进行HTML解码")]
            ExportConvertHtml =2143,    //将html代码转换成可识别的内容
            [Description("去除网页代码，但保留段落换行符号")]
            ExportHaveCRLF =2144,       //去除网页代码，但保留回车换行符号
            [Description("去除网页代码，并且将回车换行符号替换成\\r\\n")]
            ExportReplaceCRLF =2145,    //去除网页代码，并且将回车换行符号替换成\r\n
            [Description("自动编号，起始值")]
            ExportAutoCode = 2146,     //自动编号，起始值
            [Description("替换换行符号，进行换行操作")]
            ExportWrap = 2147,         //替换换行符号，进行换行操作
            [Description("通过正则二次提取采集的数据")]
            ExportGatherdata =2148,     //通过正则二次提取采集的数据
            [Description("格式化字符串")]
            ExportFormatString =2149,   //格式化字符串
            [Description("自动输出下载文件地址")]
            ExportDownloadFilePath =2150,  //自动输出下载文件地址
            [Description("从采集网址中获取数据")]
            ExportGatherUrl =2151,	  //从采集网址中获取数据	
            [Description("从采集数据中获取")]
            ExportGather =2152,	  //从采集数据中获取	
            [Description("采集数据组合")]
            ExportMakeGather =2153,	 //采集数据组合	
            [Description("字符串解码")]
            ExportEncodingString =2154,	//字符串解码	
            [Description("输入一个固定值")]
            ExportValue =2155,	//输入一个固定值	
            [Description("同义词替换")]
            ExportSynonymsReplace =2156,  //同义词替换
            [Description("段落合并")]
            ExportMergeParagraphs =2157,  //段落合并
            [Description("对下载的文件进行重命名")]
            ExportRenameDownloadFile =2158, //对下载的文件进行重命名
            [Description("转拼音")]
            ExportPY =2159,                 //转拼音
            [Description("调用插件加工")]
            ExportByPlugin =2160,           //调用插件加工
            [Description("使用外部字典替换数据")]
            ExportDict =2161,               //使用外部字典替换编号
            [Description("Base64解码")]
            ExportBase64 =2162,             //base64解码
            [Description("必须包含符合数值判断条件")]
            ExportNumber =2163,             //必须包含符合数值判断条件
            [Description("去除网页代码，但保留图片信息")]
            ExportHaveIMG = 2164,          //去除网页代码，但保留图片信息
            [Description("对下载文件的源地址进行正则替换")]
            ExportImgReplace =2165,         //对下载文件的源地址进行正则替换
            [Description("将相对地址转换成绝对地址")]
            ExportToAbsoluteUrl = 2166,    //将相对地址转换成绝对地址
            [Description("指定此数据采集项不允许重复")]
            ExportNoRepeat = 2167,         //指定此数据采集项不允许重复
            [Description("字符串编码")]
            ExportEncoding = 2168,         //字符串编码
            [Description("将xml文档规范化标准化")]
            ExportFormatXML =2169,          //将xml文档规范化标准化
            [Description("给下载的图片增加水印")]
            ExportWatermark =2170,          //给下载的图片增加水印 
            //[Description("OCR识别")]
            //ExportOCR =2171,                //OCR识别
            [Description("文件重名处理规则")]
            ExportRepeatNameDeal =2172,     //文件重名处理规则
            [Description("去除网页代码，保留段落、图片，并去除css样式")]
            ExportHavePImgNoneCSS =2173,    //去除网页代码，保留段落、图片，并去除css样式
            [Description("Base64编码")]
            ExportBase64Encoding = 2174,   //base64编码
            [Description("转换成当前日期")]
            ExportConvertDateTime =2075,    //日期转换
            [Description("截取字符")]
            ExportSubstring =2076,          //截取字符
            [Description("全部小写")]
            ExportToLower =2077,            //全部小写
            [Description("全部大写")]
            ExportToUpper=2078,            //全部大写
            [Description("md5操作")]
            ExportToMD5 =2079,              //md5操作
            [Description("去除无效字符，即一些特殊的表情符号，或者是不符合规范的高代理符号")]
            ExportDelInvalidChar=2080,     //去除无效字符，即一些特殊的表情符号，或者是不符合规范的高代理符号
            [Description("使用下载存储的文件名替换源内容中的文件名")]
            ExportReplaceFileName=2081,    //使用下载存储的文件名替换源内容中的文件名
            [Description("json转对象")]
            ExportJsonToObject=2082,       //json转对象
            [Description("AES解密")]
            ExportAesDecrypt = 2083,
            [Description("UUID")]
            ExportUUID=2084,
            [Description("对下载文件进行字符串替换")]
            ExportDownloadFileReplace=2085,
        }

        public enum WebCode
        {
            [Description("自动")]
            auto = 1000,
            [Description("GB2312")]
            gb2312 = 1001,
            [Description("UTF-8")]
            utf8 = 1002,
            [Description("GBK")]
            gbk = 1003,
            [Description("BIG5")]
            big5 =1004,
            [Description("不编码")]
            NoCoding =1005,
            [Description("Shift_JIS")]
            Shift_JIS =1006,
            [Description("Unicode")]
            Unicode =1007,
        }

        public enum ExitPara
        {
            Exit=2010,
            MinForm=2012,
        }

        public enum UpdateUrlCountType
        {
            NaviGathered=2020,   //导航采集了一个地址进行增加
            Gathered=2021,       //采集了一个地址进行增加  有可能是导航网址，一个导航网址对应的是多个地址
            Err=2022,            //发生了错误的网孩子
            NaviErr=2023,        //导航发生了错误
            ReIni = 2024,        //总数进行增加
            //采集未完成，特指导航采集的时候，导航出来网址，采集到一半的时候，系统终止了采集操作，导致采集
            //导航的网址未能全部采集完成
            GatherUnCompleted=2025,
            //UrlCountAdd=2025,    
            //ErrUrlCountAdd=2026,
        }

        public enum UrlGatherResult
        {
            UnGather=2031,
            Succeed=2032,
            Error=2033,
            Gathered=2034,
        }

        public enum DatabaseType
        {
            [Description("网络矿工数据")]
            SoukeyData = 2050,
            [Description("Access")]
            Access =2051,
            [Description("MSSqlServer")]
            MSSqlServer =2052,
            [Description("MySql")]
            MySql =2053,
            [Description("Oracle")]
            Oracle =2054,
            [Description("SqlLite")]
            SqLite =2055,
        }

        public enum LogType
        {
            [Description("信息")]
            Info =2061,
            [Description("错误")]
            Error =2062,
            [Description("警告")]
            Warning =2063,
            [Description("计划任务")]
            RunPlanTask =2064,
            [Description("采集错误")]
            GatherError =2065,
            [Description("发布错误")]
            PublishError =2066,
            [Description("启动")]
            Start =2067,
            [Description("停止")]
            Stop =2068,
            
        }

        public enum RunTaskType
        {
            [Description("网络矿工任务")]
            SoukeyTask =2071,           //网络矿工任务
            [Description("数据库存储过程")]
            DataTask =2072,               //数据库存储过程
            [Description("外接可执行命令")]
            OtherTask =2073,            //外接可执行命令
        }

        public enum RunTaskPlanType
        {
            [Description("仅运行一次")]
            Ones=2081,                //仅运行一次
            [Description("每天运行一次")]
            DayOnes =2082,             //每天运行一次
            [Description("每天上午下午各运行一次")]
            DayTwice =2083,            //每天上午下午各运行一次
            [Description("每周运行")]
            Weekly =2084,              //每周运行
            [Description("自定义运行间隔")]
            Custom =2085,              //自定义运行间隔
        }

        public enum PlanDisabledType
        {
            [Description("超过指定运行次数计划失效")]
            RunTime=2091,            //按照运行次数失效
            [Description("超过指定时间计划失效")]
            RunDateTime =2092,        //按照时间失效 
        }

        public enum PlanState
        {
            [Description("有效")]
            Enabled=3001,           //有效
            [Description("无效")]
            Disabled =3002,          //无效
            [Description("过期")]
            Expired =3003,          //过期
        }

        //监听类型
        public enum MessageType
        {
            [Description("")]
            RunSoukeyTask=3010,
            [Description("")]
            RunFileTask =3011,
            [Description("")]
            RunData =3013,
            [Description("")]
            ReloadPlan =3013,
            [Description("")]
            MonitorFileFaild =3014,
        }

        //触发器类型
        public enum TriggerType
        {
            GatheredRun=3020,
            PublishedRun=3021,
        }

        public enum CurLanguage
        {
            Auto=3031,
            zhCN=3032,
            enUS=3033,
        }

        public enum FormatLimit
        {
            OnlyNumber=3041,
            OnlyText=3042,
        }

        /// <summary>
        /// 从2018版本开始，取消专业版
        /// </summary>
        public enum VersionType
        {
            /// <summary>
            /// 企业版客户端
            /// </summary>
            [Description("企业版客户端")]
            Enterprise = 3051,    //企业版本客户端  05

            /// <summary>
            /// 旗舰版
            /// </summary> 
            [Description("旗舰版")]
            Ultimate = 3053,      //旗舰版    04

            /// <summary>
            /// 云采版
            /// </summary>
            [Description("云采版")]
            Cloud =3055,    //云采版    03
            /// <summary>
            /// 免费版
            /// </summary>
            [Description("免费版")]
            Free =3056,            //免费版   02
            /// <summary>
            /// 服务器版本
            /// </summary>
            [Description("服务器版")]
            Server =3057,          //企业版本    06
            /// <summary>
            /// 分布式服务器版
            /// </summary>
            [Description("分布式服务器版")]
            DistriServer =3058,    //企业版本+（带有分布式功能）   07
            ///// <summary>
            ///// 多站点发布版本
            ///// </summary>
            //[Description("多站点发布版本")]
            //Publish =3059,         //多站点发布系统   09
            /// <summary>
            /// 开发板
            /// </summary>
            [Description("开发版")]
            Program = 3060,         //开发版  01
        }

        public enum RegisterResult
        {
            Succeed=3060,            //注册成功
            Failed=3061,              //注册失败
            Error=3062,              //注册发生错误，只要在得到明确的失败信息后，方可判断为盗版
            Registed=3063,
            Upgrade=3064,
            NoMatch=3065,            //注册版本与当前版本不匹配
        }

        public enum VerifyUserd
        {
            UnRegister=3071,         //未注册
            Available=3072,          //可用 
            Disable=3073,            //禁用 强制退出
            Failed=3074,             //验证失败，不做任何操作，等待下次验证
        }

        //public enum IncrByCondition
        //{
        //    FormTop = 3081,                //增量判断依据 从前之后
        //    FromEnd = 3082,                 //增量判断依据 从后至前
        //}

        //监控规则
        public enum MonitorType
        {
            [Description("自定义")]
            CustomRule =3091,                 //自定义规则
            [Description("系统规则")]
            SystemRule =3092,                 //内置系统的规则
        }

        //监控字段的规则
        public enum MonitorRule
        {
            [Description("包含关键词")]
            IncludeKeyword =4001,             //包含关键字
            [Description("数字大于")]
            MoreThan =4002,                   //监控数字大于
            [Description("数字小雨")]
            LessThan =4003,                   //监控数字小于
            [Description("数字范围")]
            NumRange =4004,                   //监控数值范围
        }

        //监控结果处理方式
        public enum MonitorDealType
        {
            [Description("按照配置执行")]
            ByTaskConfig =4011,               //按照采集任务配置进行处理
            [Description("发送电子邮件")]
            SendEmail =4012,                  //发送电子邮件
            [Description("保存网页快照")]
            SaveUrlAndPage =4013,             //保存网页地址及网页内容
            [Description("仅保存网页地址")]
            SaveUrl =4014,                    //仅保存网页地址
        }

        //监控预警规则
        public enum WarningType
        {
            [Description("不预警")]
            NoWaring =4021,                   //不预警
            [Description("托盘图标闪动")]
            ByTrayIcon =4022,                 //通过闪烁托盘图标预警
            [Description("发送电子邮件")]
            ByEmail =4023,                    //通过发送电子邮件预警
        }

        public enum MonitorState
        {
            [Description("正常")]
            Normal=4031,
            [Description("无效")]
            Invalid =4032,
            [Description("丢失")]
            Lose =4033,
            [Description("运行")]
            Running =4034,
            [Description("停止")]
            Stop =4035,
        }

        public enum MonitorInvalidType
        {
            [Description("用不失效")]
            NoInvalid =4041,                //永不失效
            [Description("超过指定日期失效")]
            InvalidByDate =4042,            //按照日期检测失效
            [Description("检测到指定规则失效")]
            InvalidByChecked =4043,         //如果检测到符合的规则即失效
            [Description("失效")]
            Invalid =4044,                  //失效
        }

        public enum NewTaskType
        {
            Wizard=4051,         //向导式建立采集任务
            Normal=4052,         //便捷建立采集任务
            Cancel=4053,         //取消建立
        }

        public enum SoMinerRunningModel
        {
            Normail=4061,
            Silent=4062,
        }

        //日志分类
        public enum LogClass
        {
            GatherLog=4071,
            PublishLog=4072,
            ListenPlan=4073,
            Trigger=4074,
            Task=4075,
            Radar=4076,
            System=4077,
            Data=4078,
            RunTask=4079,
        }

        //日志清除UI返回参数
        public enum LogExitPara
        {
            Cancel=4081,
            BackupAndClear=4082,
            Clear=4083,
        }

        public enum RuleState
        {
            Normal =4091,
            Lose=4092,
        }

        public enum DownloadFileDealType
        {
            [Description("重名则进行覆盖")]
            SaveCover=5001,
            [Description("重名则按照序号进行排序保存")]
            SaveBySort=5002,
            [Description("重名则按照网站路径进行保存")]
            SaveByDir=5003,
        }

        public enum GatherRuleByPage
        {
            [Description("采集页")]
            GatherPage=5011,
            [Description("导航页")]
            NaviPage =5012,
            [Description("多页")]
            MultiPage =5013,
            //NonePage=5014,
        }

        public enum GatherRuleType
        {
            [Description("Normal")]
            Normal=5031,
            [Description("XPath")]
            XPath =5032,
            [Description("Smart")]
            Smart =5033,
            [Description("NonePage")]
            NonePage = 5034,
        }

        public enum HtmlNodeTextType
        {
            [Description("InnerHtml")]
            InnerHtml=5021,
            [Description("InnerText")]
            InnerText =5022,
            [Description("OuterHtml")]
            OuterHtml =5023,
            /// <summary>
            /// 这个只有在小矿机器人中使用
            /// </summary>
            [Description("Link")]
            Link =5024,
            [Description("ImgUrl")]
            ImgUrl =5025,
            
        }

        public enum RegResult
        {
            Succeed = 5031,          //注册成功
            UnReg = 5032,            //可以连接，但未注册
            VersionNoMatch = 5033,   //注册但版本不匹配
            Faild=5034,              //连接失败
        }

        public enum ServerState
        {
            Running=5041,
            Stopped=5042,
            UnSetup=5043,
        }

        public enum PluginsType
        {
            [Description("获取Cookie类")]
            GetCookie=5051,
            [Description("数据处理类")]
            DealData=5052,
            [Description("数据发布类")]
            PublishData=5053,
        }

        //水印在图片的位置
        public enum WatermarkPOS
        {
            [Description("靠左靠上")]
            LeftTop=5061,         //左上
            [Description("靠左上下居中")]
            LeftMiddle=5062,      //左中
            [Description("靠左靠下")]
            LeftBottom =5063,      //左下
            [Description("居中靠上")]
            CenterTop = 5064,     //中上
            [Description("左右居中上下居中")]
            CenterMiddle = 5065,  //中中
            [Description("居中靠下")]
            CenterBottom = 5066,  //中下
            [Description("靠右靠上")]
            RightTop = 5067,      //右上
            [Description("靠右上下居中")]
            RightMiddle = 5068,   //右中
            [Description("靠右靠下")]
            RightBottom = 5069,   //右下
        }

        //发布模版类别
        public enum PublishTemplateType
        {
            [Description("网站发布类")]
            Web=5101,
            [Description("数据库发布类")]
            DB=5102,
            [Description("所有")]
            All=5103,
        }
        
        //sql语句插入的类别，用于发布模版制作
        public enum PublishSqlType
        {
            [Description("仅执行sql")]
            Common=5201,
            [Description("根据主键进行更新")]
            UpdateByPK=5202,
            [Description("获取返回值")]
            GetValues=5203,
            [Description("获取插入记录的ID")]
            GetLastID=5204,
        }

        //发布数据时的参数对应类别
        public enum PublishParaType
        {
            Gather=5301,         //发布数据从采集的数据中获取
            Custom=5302,         //发布数据从手工输入中获取
            NoData=5303,         //无数据，不需要进行任何操作
        }

        //发布全局参数的所属页面
        public enum PublishGlobalParaPage
        {
            [Description("登录页")]
            LoginPage=5401,
            [Description("发布页")]
            PublishPage=5402,
        }

        //同义词库类型
        public enum SynonymType
        {
            System=5501,
            Custom=5502,
        }

        //回链获取的页面分类
        public enum RUrlPageType
        {
            CurrentPage=5601,
            CustomPage=5602,
        }

        //采集重复或错的处理规则
        public enum StopRule
        {
            [Description("不做处理")]
            None=5703,
            [Description("停止采集")]
            StopTaskGather=5701,
            [Description("跳过此网址进行下一条网址采集")]
            StopUrlGather=5702,
        }

        public enum SplitTaskState
        {
            [Description("等待分解")]
            UnSplit=5801,            //未分解
            [Description("分解结束")]
            Splited=5802,            //已分解子任务
            [Description("无需分解")]
            WithoutSplit=5803,       //无需分解
            [Description("正在分解")]
            Spliting=5804,           //分解中
            [Description("分解失败")]
            SplitFaild=5805,         //分解失败
        }

        public enum ClientState
        {
            Online=5901,
            OffLine=5902,
            Unknow=5903,
            Reset=5904,
        }

        public enum UploadTaskState
        {
            UnHave=6001,          //不存在
            HaveUnrunning=6002,   //存在未运行
            HaveRunning=6003,     //存在运行
            HaveCompleted=6004,   //存在已经运行完毕
        }

        public enum VerifyProxyState
        {
            Succeed=6011,
            Faild=6012,
        }

        public enum GatherTaskType
        {
            [Description("常规任务")]
            Normal =6021,
            /// <summary>
            /// 特指分布式采集操作
            /// </summary>
            [Description("特指分布式采集操作")]
            DistriGather =6022,
            /// <summary>
            /// 分布式执行的分解任务
            /// </summary>
            [Description("分布式执行的分解任务")]
            DistriSplitTask = 6023,
            /// <summary>
            /// 分布式执行的独立任务
            /// </summary>
            [Description("分布式执行的独立任务")]
            DistriTask =6024,
        }

        public enum FileType
        {
            [Description("文件")]
            File=6031,
            [Description("目录")]
            Directory =6032,
        }

        public enum NaviRunRule
        {
            [Description("正常导航")]
            Normal=6041,
            [Description("跳过")]
            Skip=6042,
            [Description("使用其它规则")]
            Other=6043,
        }

        public enum ProxyType
        {
            [Description("不使用代理")]
            None = 6051,
            /// <summary>
            /// http代理
            /// </summary>
            [Description("Web代理")]
            HttpProxy = 6052,
            /// <summary>
            /// Socket5代理
            /// </summary>
            [Description("Socket5代理")]
            Socket5 = 6053,
            /// <summary>
            /// 系统代理
            /// </summary>
            [Description("系统代理")]
            SystemProxy = 6054,
            /// <summary>
            /// 使用采集任务设置
            /// </summary>
            [Description("采集任务默认设置")]
            TaskConfig=6055,
        }

        /// <summary>
        /// 动作类型，表示客户端每次向服务器发送消息时，代表的动作
        /// </summary>
        public enum activeType
        {
            Normal=6061,
        }

        public enum WebEngineRuleType
        {
            /// <summary>
            /// 获取网址
            /// </summary>
            [Description("获取网址")]
            GetUrl =6071,
            /// <summary>
            /// 采集数据
            /// </summary>
            [Description("采集数据")]
            GetData = 6072,
        }

        public enum CopyType
        {
            [Description("移动")]
            Move=6081,
            [Description("拷贝")]
            Copy=6082,
        }


        public enum TaskClass
        {
            [Description("本地任务")]
            Local =6091,
            [Description("远程任务")]
            Remote=6092,
        }

        public enum GetCookieType
        {
            [Description("手动输入获取Cookie")]
            Input=6101,
            [Description("登录获取Cookie")]
            Login=6102,
            [Description("通过制定网址获取Cookie")]
            Url=6103,
        }

        public enum RejectDeal
        {
            None=6201,
            StopGather=6202,
            UpdateCookie=6203,
            Error=6204,
            Coding=6205,
        }

        public enum opType
        {
            Add=6301,
            Edit=6302,
            Del=6303,
        }

        public enum RequestMethod
        {
            Get=6401,
            Post=6402,
        }

        public enum TaskProcess
        {
            Gather=6501,
            ETL=6502,
            Publish=6503,
            UnKnow=6504,
        }

        #endregion

       
    }

    public static class EnumUtil
    {
        public static int GetEnum(this Enum value, string Description)
        {
            return -1;
        }

        /// <summary>  
        /// 获取字段Description  
        /// </summary>  
        /// <param name="fieldInfo">FieldInfo</param>  
        /// <returns>DescriptionAttribute[] </returns>  
        public static DescriptionAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
        {
            if (fieldInfo != null)
            {
                return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            }
            return null;
        }

        public static T GetEnumName<T>(string description)
        {
            Type _type = typeof(T);
            foreach (FieldInfo field in _type.GetFields())
            {
                DescriptionAttribute[] _curDesc = field.GetDescriptAttr();
                if (_curDesc != null && _curDesc.Length > 0)
                {
                    if (_curDesc[0].Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }

            return default(T);
            //throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
        }

        public static string GetDescription(this Enum value, Boolean nameInstead = true)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name == null)
            {
                return null;
            }

            FieldInfo field = type.GetField(name);
            DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            if (attribute == null && nameInstead == true)
            {
                return name;
            }
            return attribute == null ? null : attribute.Description;
        }

        public static Dictionary<Int32, String> EnumToDictionary(Type enumType, Func<Enum, String> getText)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("传入的参数必须是枚举类型！", "enumType");
            }
            Dictionary<Int32, String> enumDic = new Dictionary<int, string>();
            Array enumValues = Enum.GetValues(enumType);
            foreach (Enum enumValue in enumValues)
            {
                Int32 key = Convert.ToInt32(enumValue);
                String value = getText(enumValue);
                enumDic.Add(key, value);
            }
            return enumDic;
        }


    }
}
