using System;
using System.Collections.Generic;
using System.Text;
using System.Resources;
using System.Reflection;
using System.ComponentModel;

///���ܣ�ȫ�� ���� ���� ��
///���ʱ�䣺2009-9-19
///���ߣ�һ��
///�������⣺��
///�����ƻ�����
///˵�������Ӷ�������Դ�ļ��Ĵ���
///�汾��01.80.00
///�޶�����
namespace NetMiner.Resource
{
    public static class cGlobalParas
    {

        #region ö�ٳ���
        public enum TaskState
        {
            [Description("δ����")]
            UnStart = 1010,
            [Description("����")]
            Started =1011,
            [Description("��ֹ")]
            Aborted =1012,
            [Description("�ȴ�")]
            Waiting =1013,
            [Description("��������")]
            Running = 1014,
            [Description("��ͣ")]
            Pause = 1015,
            [Description("ֹͣ")]
            Stopped = 1016,
            [Description("������...")]
            Exporting = 1017,
            [Description("���")]
            Completed =1018,
            [Description("ʧ��")]
            Failed = 1019,
            [Description("������...")]
            Publishing =1020,
            [Description("����ֹͣ")]
            PublishStop =1021,
            [Description("����ʧ��")]
            PublishFailed =1022,
            [Description("�ͻ���������...")]
            Request =1023,                      //��ʾ�����񱻿ͻ�������������ȡ����ռ��
            [Description("�ȴ�����")]
            WaitingPublish =1024,
            [Description("Զ��δ����")]
            RemoteUnstart =1025,                //Զ����������ɹ�����û�п�ʼ����
            [Description("Զ��������")]
            RemoteRunning =1026,                //��ʾ���������ɱ������У�������Զ�����е�
            [Description("�쳣ֹͣ")]
            ErrStop=1027,
        }

        public enum GatherResult
        {
            [Description("�ɼ�δ����")]
            UnGather=1077,
            [Description("�ɼ��ɹ�")]
            GatherSucceed =1071,
            [Description("�ɼ�ʧ��")]
            GatherFailed =1072,
            [Description("�ɼ�ֹͣ")]
            GatherStoppedByUser =1076,
            [Description("�����ɹ�")]
            PublishSuccees =1073,
            [Description("����ʧ��")]
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
            [Description("�ı�")]
            Txt=1091,              //�ı�
            [Description("ͼƬ")]
            Picture =1092,          //ͼƬ   
            [Description("Flash")]
            Flash =1093,            //Flash
            [Description("�ļ�")]
            File =1094,             //�ļ�
            [Description("������ȡҳ�����")]
            AutoTitle =1095,        //�Զ�����
            [Description("������ȡ��������")]
            AutoContent =1096,      //�Զ�����
            [Description("������ȡ����")]
            AutoAuthor =1100,       //�Զ�����
            [Description("������ȡ��Դ")]
            AutoSource =1101,       //�Զ���Դ
            [Description("������ȡ����ʱ��")]
            AutoPublishDate =1097,        //�Զ����
            [Description("��ҳ����")]
            HtmlCached =1099,       //��ҳ����
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
            [Description("���ɼ�")]
            OnlyGather=1041,                  //���ɼ�
            [Description("�ɼ�������")]
            GatherExportData =1042,            //�ɼ�������
            [Description("ֱ�����")]
            OnlySave =1043,                    //ֱ�����
            [Description("�״�����")]
            Radar=1044,
        }

        public enum TaskType
        {
            [Description("������ַ�ɼ���ҳ����")]
            HtmlByUrl=1051,
            [Description("��������")]
            HtmlByWeb =1053,
            [Description("�ⲿ��������")]
            ExternalPara=1054,
        }

        public enum PublishType
        {
            [Description("����������")]
            NoPublish=1060,
            [Description("������Access")]
            PublishAccess = 1061,
            [Description("������MS SqlServer")]
            PublishMSSql = 1062,
            [Description("�������ı��ļ�")]
            PublishTxt =1063,
            [Description("������Excel��2007��ʽ��")]
            PublishExcel =1064,
            [Description("������MySql")]
            PublishMySql =1065,
            [Description("������CSV�ļ�")]
            PublishCSV =1067,
            [Description("��������")]
            PublishData = 1068,
            [Description("ͨ�������������")]
            publishPlugin =1069,
            [Description("ͨ������ģ�巢������")]
            publishTemplate =1070,
            [Description("������Oracle")]
            publishOracle =1071,
            [Description("������Word��2007��ʽ��")]
            publishWord =1072,

        }

        public enum LimitSign
        {
            [Description("���������ʽ������")]
            NoLimit = 2001,          //���������ʽ������
            [Description("ֻƥ�����ҳ����")]
            NoWebSign = 2002,        //ֻƥ�����ҳ����
            [Description("ֻƥ������")]
            OnlyCN = 2003,           //ֻƥ������
            [Description("ֻƥ��˫�ֽ��ַ�")]
            OnlyDoubleByte =2004,     //ֻƥ��˫�ֽ��ַ�
            [Description("ֻƥ������")]
            OnlyNumber =2005,         //ֻƥ������
            [Description("ֻƥ����ĸ���ּ������ַ�")]
            OnlyChar =2006,           //ֻƥ����ĸ���ּ������ַ�
            [Description("�Զ���ƥ���ַ�")]
            CustomMatchChar =2007,    //�Զ���ƥ���ַ�
            [Description("�Զ�������ƥ����ʽ")]
            Custom = 2008,           //�Զ�������ƥ����ʽ 
            [Description("�Զ��������±���")]
            AutoTitle =2009,          //�Զ��������±���
            [Description("�Զ�������������")]
            AutoContent =2010,        //�Զ�������������
            [Description("�Զ����񷢲�ʱ��")]
            AutoPublish =2011,        //�Զ����񷢲�ʱ��
            [Description("�Զ�����������Դ")]
            AutoSource =2012,         //�Զ�����������Դ
        }

        public enum ExportLimit
        {
            [Description("�����������")]
            ExportNoLimit =2040,        //�����������
            [Description("���ʱȥ����ҳ����")]
            ExportNoWebSign = 2041,    //���ʱȥ����ҳ����
            [Description("���ʱ����ǰ׺")]
            ExportPrefix = 2042,       //���ʱ����ǰ׺
            [Description("���ʱ���Ӻ�׺")]
            ExportSuffix = 2043,       //���ʱ���Ӻ�׺
            [Description("����ȥ���ַ�")]
            ExportTrimLeft = 2044,     //����ȥ���ַ�
            [Description("����ȥ���ַ�")]
            ExportTrimRight = 2045,    //����ȥ���ַ�
            [Description("�滻���з����������ַ�")]
            ExportReplace = 2046,      //�滻���з����������ַ�
            [Description("ȥ���ַ�����β�ո�")]
            ExportTrim = 2047,         //ȥ���ַ�����β�ո�
            [Description("���ʱ����������ʽ�����滻")]
            ExportRegexReplace = 2048, //���ʱ����������ʽ�����滻
            [Description("����ָ������ɾ������")]
            ExportDelData =2049,        //����ָ������ɾ������
            [Description("�������ָ������")]
            ExportInclude = 2140,      //�������ָ������
            [Description("������������������Ϊ��")]
            ExportSetEmpty =2141,       //������������������Ϊ��
            [Description("��Unicodeת�ɺ���")]
            ExportConvertUnicode =2142, //��Unicodeת�ɺ���
            [Description("�����HTML����")]
            ExportConvertHtml =2143,    //��html����ת���ɿ�ʶ�������
            [Description("ȥ����ҳ���룬���������任�з���")]
            ExportHaveCRLF =2144,       //ȥ����ҳ���룬�������س����з���
            [Description("ȥ����ҳ���룬���ҽ��س����з����滻��\\r\\n")]
            ExportReplaceCRLF =2145,    //ȥ����ҳ���룬���ҽ��س����з����滻��\r\n
            [Description("�Զ���ţ���ʼֵ")]
            ExportAutoCode = 2146,     //�Զ���ţ���ʼֵ
            [Description("�滻���з��ţ����л��в���")]
            ExportWrap = 2147,         //�滻���з��ţ����л��в���
            [Description("ͨ�����������ȡ�ɼ�������")]
            ExportGatherdata =2148,     //ͨ�����������ȡ�ɼ�������
            [Description("��ʽ���ַ���")]
            ExportFormatString =2149,   //��ʽ���ַ���
            [Description("�Զ���������ļ���ַ")]
            ExportDownloadFilePath =2150,  //�Զ���������ļ���ַ
            [Description("�Ӳɼ���ַ�л�ȡ����")]
            ExportGatherUrl =2151,	  //�Ӳɼ���ַ�л�ȡ����	
            [Description("�Ӳɼ������л�ȡ")]
            ExportGather =2152,	  //�Ӳɼ������л�ȡ	
            [Description("�ɼ��������")]
            ExportMakeGather =2153,	 //�ɼ��������	
            [Description("�ַ�������")]
            ExportEncodingString =2154,	//�ַ�������	
            [Description("����һ���̶�ֵ")]
            ExportValue =2155,	//����һ���̶�ֵ	
            [Description("ͬ����滻")]
            ExportSynonymsReplace =2156,  //ͬ����滻
            [Description("����ϲ�")]
            ExportMergeParagraphs =2157,  //����ϲ�
            [Description("�����ص��ļ�����������")]
            ExportRenameDownloadFile =2158, //�����ص��ļ�����������
            [Description("תƴ��")]
            ExportPY =2159,                 //תƴ��
            [Description("���ò���ӹ�")]
            ExportByPlugin =2160,           //���ò���ӹ�
            [Description("ʹ���ⲿ�ֵ��滻����")]
            ExportDict =2161,               //ʹ���ⲿ�ֵ��滻���
            [Description("Base64����")]
            ExportBase64 =2162,             //base64����
            [Description("�������������ֵ�ж�����")]
            ExportNumber =2163,             //�������������ֵ�ж�����
            [Description("ȥ����ҳ���룬������ͼƬ��Ϣ")]
            ExportHaveIMG = 2164,          //ȥ����ҳ���룬������ͼƬ��Ϣ
            [Description("�������ļ���Դ��ַ���������滻")]
            ExportImgReplace =2165,         //�������ļ���Դ��ַ���������滻
            [Description("����Ե�ַת���ɾ��Ե�ַ")]
            ExportToAbsoluteUrl = 2166,    //����Ե�ַת���ɾ��Ե�ַ
            [Description("ָ�������ݲɼ�������ظ�")]
            ExportNoRepeat = 2167,         //ָ�������ݲɼ�������ظ�
            [Description("�ַ�������")]
            ExportEncoding = 2168,         //�ַ�������
            [Description("��xml�ĵ��淶����׼��")]
            ExportFormatXML =2169,          //��xml�ĵ��淶����׼��
            [Description("�����ص�ͼƬ����ˮӡ")]
            ExportWatermark =2170,          //�����ص�ͼƬ����ˮӡ 
            //[Description("OCRʶ��")]
            //ExportOCR =2171,                //OCRʶ��
            [Description("�ļ������������")]
            ExportRepeatNameDeal =2172,     //�ļ������������
            [Description("ȥ����ҳ���룬�������䡢ͼƬ����ȥ��css��ʽ")]
            ExportHavePImgNoneCSS =2173,    //ȥ����ҳ���룬�������䡢ͼƬ����ȥ��css��ʽ
            [Description("Base64����")]
            ExportBase64Encoding = 2174,   //base64����
            [Description("ת���ɵ�ǰ����")]
            ExportConvertDateTime =2075,    //����ת��
            [Description("��ȡ�ַ�")]
            ExportSubstring =2076,          //��ȡ�ַ�
            [Description("ȫ��Сд")]
            ExportToLower =2077,            //ȫ��Сд
            [Description("ȫ����д")]
            ExportToUpper=2078,            //ȫ����д
            [Description("md5����")]
            ExportToMD5 =2079,              //md5����
            [Description("ȥ����Ч�ַ�����һЩ����ı�����ţ������ǲ����Ϲ淶�ĸߴ������")]
            ExportDelInvalidChar=2080,     //ȥ����Ч�ַ�����һЩ����ı�����ţ������ǲ����Ϲ淶�ĸߴ������
            [Description("ʹ�����ش洢���ļ����滻Դ�����е��ļ���")]
            ExportReplaceFileName=2081,    //ʹ�����ش洢���ļ����滻Դ�����е��ļ���
            [Description("jsonת����")]
            ExportJsonToObject=2082,       //jsonת����
            [Description("AES����")]
            ExportAesDecrypt = 2083,
            [Description("UUID")]
            ExportUUID=2084,
            [Description("�������ļ������ַ����滻")]
            ExportDownloadFileReplace=2085,
        }

        public enum WebCode
        {
            [Description("�Զ�")]
            auto = 1000,
            [Description("GB2312")]
            gb2312 = 1001,
            [Description("UTF-8")]
            utf8 = 1002,
            [Description("GBK")]
            gbk = 1003,
            [Description("BIG5")]
            big5 =1004,
            [Description("������")]
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
            NaviGathered=2020,   //�����ɼ���һ����ַ��������
            Gathered=2021,       //�ɼ���һ����ַ��������  �п����ǵ�����ַ��һ��������ַ��Ӧ���Ƕ����ַ
            Err=2022,            //�����˴����������
            NaviErr=2023,        //���������˴���
            ReIni = 2024,        //������������
            //�ɼ�δ��ɣ���ָ�����ɼ���ʱ�򣬵���������ַ���ɼ���һ���ʱ��ϵͳ��ֹ�˲ɼ����������²ɼ�
            //��������ַδ��ȫ���ɼ����
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
            [Description("���������")]
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
            [Description("��Ϣ")]
            Info =2061,
            [Description("����")]
            Error =2062,
            [Description("����")]
            Warning =2063,
            [Description("�ƻ�����")]
            RunPlanTask =2064,
            [Description("�ɼ�����")]
            GatherError =2065,
            [Description("��������")]
            PublishError =2066,
            [Description("����")]
            Start =2067,
            [Description("ֹͣ")]
            Stop =2068,
            
        }

        public enum RunTaskType
        {
            [Description("���������")]
            SoukeyTask =2071,           //���������
            [Description("���ݿ�洢����")]
            DataTask =2072,               //���ݿ�洢����
            [Description("��ӿ�ִ������")]
            OtherTask =2073,            //��ӿ�ִ������
        }

        public enum RunTaskPlanType
        {
            [Description("������һ��")]
            Ones=2081,                //������һ��
            [Description("ÿ������һ��")]
            DayOnes =2082,             //ÿ������һ��
            [Description("ÿ���������������һ��")]
            DayTwice =2083,            //ÿ���������������һ��
            [Description("ÿ������")]
            Weekly =2084,              //ÿ������
            [Description("�Զ������м��")]
            Custom =2085,              //�Զ������м��
        }

        public enum PlanDisabledType
        {
            [Description("����ָ�����д����ƻ�ʧЧ")]
            RunTime=2091,            //�������д���ʧЧ
            [Description("����ָ��ʱ��ƻ�ʧЧ")]
            RunDateTime =2092,        //����ʱ��ʧЧ 
        }

        public enum PlanState
        {
            [Description("��Ч")]
            Enabled=3001,           //��Ч
            [Description("��Ч")]
            Disabled =3002,          //��Ч
            [Description("����")]
            Expired =3003,          //����
        }

        //��������
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

        //����������
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
        /// ��2018�汾��ʼ��ȡ��רҵ��
        /// </summary>
        public enum VersionType
        {
            /// <summary>
            /// ��ҵ��ͻ���
            /// </summary>
            [Description("��ҵ��ͻ���")]
            Enterprise = 3051,    //��ҵ�汾�ͻ���  05

            /// <summary>
            /// �콢��
            /// </summary> 
            [Description("�콢��")]
            Ultimate = 3053,      //�콢��    04

            /// <summary>
            /// �Ʋɰ�
            /// </summary>
            [Description("�Ʋɰ�")]
            Cloud =3055,    //�Ʋɰ�    03
            /// <summary>
            /// ��Ѱ�
            /// </summary>
            [Description("��Ѱ�")]
            Free =3056,            //��Ѱ�   02
            /// <summary>
            /// �������汾
            /// </summary>
            [Description("��������")]
            Server =3057,          //��ҵ�汾    06
            /// <summary>
            /// �ֲ�ʽ��������
            /// </summary>
            [Description("�ֲ�ʽ��������")]
            DistriServer =3058,    //��ҵ�汾+�����зֲ�ʽ���ܣ�   07
            ///// <summary>
            ///// ��վ�㷢���汾
            ///// </summary>
            //[Description("��վ�㷢���汾")]
            //Publish =3059,         //��վ�㷢��ϵͳ   09
            /// <summary>
            /// ������
            /// </summary>
            [Description("������")]
            Program = 3060,         //������  01
        }

        public enum RegisterResult
        {
            Succeed=3060,            //ע��ɹ�
            Failed=3061,              //ע��ʧ��
            Error=3062,              //ע�ᷢ������ֻҪ�ڵõ���ȷ��ʧ����Ϣ�󣬷����ж�Ϊ����
            Registed=3063,
            Upgrade=3064,
            NoMatch=3065,            //ע��汾�뵱ǰ�汾��ƥ��
        }

        public enum VerifyUserd
        {
            UnRegister=3071,         //δע��
            Available=3072,          //���� 
            Disable=3073,            //���� ǿ���˳�
            Failed=3074,             //��֤ʧ�ܣ������κβ������ȴ��´���֤
        }

        //public enum IncrByCondition
        //{
        //    FormTop = 3081,                //�����ж����� ��ǰ֮��
        //    FromEnd = 3082,                 //�����ж����� �Ӻ���ǰ
        //}

        //��ع���
        public enum MonitorType
        {
            [Description("�Զ���")]
            CustomRule =3091,                 //�Զ������
            [Description("ϵͳ����")]
            SystemRule =3092,                 //����ϵͳ�Ĺ���
        }

        //����ֶεĹ���
        public enum MonitorRule
        {
            [Description("�����ؼ���")]
            IncludeKeyword =4001,             //�����ؼ���
            [Description("���ִ���")]
            MoreThan =4002,                   //������ִ���
            [Description("����С��")]
            LessThan =4003,                   //�������С��
            [Description("���ַ�Χ")]
            NumRange =4004,                   //�����ֵ��Χ
        }

        //��ؽ������ʽ
        public enum MonitorDealType
        {
            [Description("��������ִ��")]
            ByTaskConfig =4011,               //���ղɼ��������ý��д���
            [Description("���͵����ʼ�")]
            SendEmail =4012,                  //���͵����ʼ�
            [Description("������ҳ����")]
            SaveUrlAndPage =4013,             //������ҳ��ַ����ҳ����
            [Description("��������ҳ��ַ")]
            SaveUrl =4014,                    //��������ҳ��ַ
        }

        //���Ԥ������
        public enum WarningType
        {
            [Description("��Ԥ��")]
            NoWaring =4021,                   //��Ԥ��
            [Description("����ͼ������")]
            ByTrayIcon =4022,                 //ͨ����˸����ͼ��Ԥ��
            [Description("���͵����ʼ�")]
            ByEmail =4023,                    //ͨ�����͵����ʼ�Ԥ��
        }

        public enum MonitorState
        {
            [Description("����")]
            Normal=4031,
            [Description("��Ч")]
            Invalid =4032,
            [Description("��ʧ")]
            Lose =4033,
            [Description("����")]
            Running =4034,
            [Description("ֹͣ")]
            Stop =4035,
        }

        public enum MonitorInvalidType
        {
            [Description("�ò�ʧЧ")]
            NoInvalid =4041,                //����ʧЧ
            [Description("����ָ������ʧЧ")]
            InvalidByDate =4042,            //�������ڼ��ʧЧ
            [Description("��⵽ָ������ʧЧ")]
            InvalidByChecked =4043,         //�����⵽���ϵĹ���ʧЧ
            [Description("ʧЧ")]
            Invalid =4044,                  //ʧЧ
        }

        public enum NewTaskType
        {
            Wizard=4051,         //��ʽ�����ɼ�����
            Normal=4052,         //��ݽ����ɼ�����
            Cancel=4053,         //ȡ������
        }

        public enum SoMinerRunningModel
        {
            Normail=4061,
            Silent=4062,
        }

        //��־����
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

        //��־���UI���ز���
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
            [Description("��������и���")]
            SaveCover=5001,
            [Description("����������Ž������򱣴�")]
            SaveBySort=5002,
            [Description("����������վ·�����б���")]
            SaveByDir=5003,
        }

        public enum GatherRuleByPage
        {
            [Description("�ɼ�ҳ")]
            GatherPage=5011,
            [Description("����ҳ")]
            NaviPage =5012,
            [Description("��ҳ")]
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
            /// ���ֻ����С���������ʹ��
            /// </summary>
            [Description("Link")]
            Link =5024,
            [Description("ImgUrl")]
            ImgUrl =5025,
            
        }

        public enum RegResult
        {
            Succeed = 5031,          //ע��ɹ�
            UnReg = 5032,            //�������ӣ���δע��
            VersionNoMatch = 5033,   //ע�ᵫ�汾��ƥ��
            Faild=5034,              //����ʧ��
        }

        public enum ServerState
        {
            Running=5041,
            Stopped=5042,
            UnSetup=5043,
        }

        public enum PluginsType
        {
            [Description("��ȡCookie��")]
            GetCookie=5051,
            [Description("���ݴ�����")]
            DealData=5052,
            [Description("���ݷ�����")]
            PublishData=5053,
        }

        //ˮӡ��ͼƬ��λ��
        public enum WatermarkPOS
        {
            [Description("������")]
            LeftTop=5061,         //����
            [Description("�������¾���")]
            LeftMiddle=5062,      //����
            [Description("������")]
            LeftBottom =5063,      //����
            [Description("���п���")]
            CenterTop = 5064,     //����
            [Description("���Ҿ������¾���")]
            CenterMiddle = 5065,  //����
            [Description("���п���")]
            CenterBottom = 5066,  //����
            [Description("���ҿ���")]
            RightTop = 5067,      //����
            [Description("�������¾���")]
            RightMiddle = 5068,   //����
            [Description("���ҿ���")]
            RightBottom = 5069,   //����
        }

        //����ģ�����
        public enum PublishTemplateType
        {
            [Description("��վ������")]
            Web=5101,
            [Description("���ݿⷢ����")]
            DB=5102,
            [Description("����")]
            All=5103,
        }
        
        //sql�������������ڷ���ģ������
        public enum PublishSqlType
        {
            [Description("��ִ��sql")]
            Common=5201,
            [Description("�����������и���")]
            UpdateByPK=5202,
            [Description("��ȡ����ֵ")]
            GetValues=5203,
            [Description("��ȡ�����¼��ID")]
            GetLastID=5204,
        }

        //��������ʱ�Ĳ�����Ӧ���
        public enum PublishParaType
        {
            Gather=5301,         //�������ݴӲɼ��������л�ȡ
            Custom=5302,         //�������ݴ��ֹ������л�ȡ
            NoData=5303,         //�����ݣ�����Ҫ�����κβ���
        }

        //����ȫ�ֲ���������ҳ��
        public enum PublishGlobalParaPage
        {
            [Description("��¼ҳ")]
            LoginPage=5401,
            [Description("����ҳ")]
            PublishPage=5402,
        }

        //ͬ��ʿ�����
        public enum SynonymType
        {
            System=5501,
            Custom=5502,
        }

        //������ȡ��ҳ�����
        public enum RUrlPageType
        {
            CurrentPage=5601,
            CustomPage=5602,
        }

        //�ɼ��ظ����Ĵ������
        public enum StopRule
        {
            [Description("��������")]
            None=5703,
            [Description("ֹͣ�ɼ�")]
            StopTaskGather=5701,
            [Description("��������ַ������һ����ַ�ɼ�")]
            StopUrlGather=5702,
        }

        public enum SplitTaskState
        {
            [Description("�ȴ��ֽ�")]
            UnSplit=5801,            //δ�ֽ�
            [Description("�ֽ����")]
            Splited=5802,            //�ѷֽ�������
            [Description("����ֽ�")]
            WithoutSplit=5803,       //����ֽ�
            [Description("���ڷֽ�")]
            Spliting=5804,           //�ֽ���
            [Description("�ֽ�ʧ��")]
            SplitFaild=5805,         //�ֽ�ʧ��
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
            UnHave=6001,          //������
            HaveUnrunning=6002,   //����δ����
            HaveRunning=6003,     //��������
            HaveCompleted=6004,   //�����Ѿ��������
        }

        public enum VerifyProxyState
        {
            Succeed=6011,
            Faild=6012,
        }

        public enum GatherTaskType
        {
            [Description("��������")]
            Normal =6021,
            /// <summary>
            /// ��ָ�ֲ�ʽ�ɼ�����
            /// </summary>
            [Description("��ָ�ֲ�ʽ�ɼ�����")]
            DistriGather =6022,
            /// <summary>
            /// �ֲ�ʽִ�еķֽ�����
            /// </summary>
            [Description("�ֲ�ʽִ�еķֽ�����")]
            DistriSplitTask = 6023,
            /// <summary>
            /// �ֲ�ʽִ�еĶ�������
            /// </summary>
            [Description("�ֲ�ʽִ�еĶ�������")]
            DistriTask =6024,
        }

        public enum FileType
        {
            [Description("�ļ�")]
            File=6031,
            [Description("Ŀ¼")]
            Directory =6032,
        }

        public enum NaviRunRule
        {
            [Description("��������")]
            Normal=6041,
            [Description("����")]
            Skip=6042,
            [Description("ʹ����������")]
            Other=6043,
        }

        public enum ProxyType
        {
            [Description("��ʹ�ô���")]
            None = 6051,
            /// <summary>
            /// http����
            /// </summary>
            [Description("Web����")]
            HttpProxy = 6052,
            /// <summary>
            /// Socket5����
            /// </summary>
            [Description("Socket5����")]
            Socket5 = 6053,
            /// <summary>
            /// ϵͳ����
            /// </summary>
            [Description("ϵͳ����")]
            SystemProxy = 6054,
            /// <summary>
            /// ʹ�òɼ���������
            /// </summary>
            [Description("�ɼ�����Ĭ������")]
            TaskConfig=6055,
        }

        /// <summary>
        /// �������ͣ���ʾ�ͻ���ÿ���������������Ϣʱ������Ķ���
        /// </summary>
        public enum activeType
        {
            Normal=6061,
        }

        public enum WebEngineRuleType
        {
            /// <summary>
            /// ��ȡ��ַ
            /// </summary>
            [Description("��ȡ��ַ")]
            GetUrl =6071,
            /// <summary>
            /// �ɼ�����
            /// </summary>
            [Description("�ɼ�����")]
            GetData = 6072,
        }

        public enum CopyType
        {
            [Description("�ƶ�")]
            Move=6081,
            [Description("����")]
            Copy=6082,
        }


        public enum TaskClass
        {
            [Description("��������")]
            Local =6091,
            [Description("Զ������")]
            Remote=6092,
        }

        public enum GetCookieType
        {
            [Description("�ֶ������ȡCookie")]
            Input=6101,
            [Description("��¼��ȡCookie")]
            Login=6102,
            [Description("ͨ���ƶ���ַ��ȡCookie")]
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
        /// ��ȡ�ֶ�Description  
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
            //throw new ArgumentException(string.Format("{0} δ���ҵ���Ӧ��ö��.", description), "Description");
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
                throw new ArgumentException("����Ĳ���������ö�����ͣ�", "enumType");
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
