using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Management;
using Microsoft.Win32;
using System.Web;
using System.Net;
using System.Text.RegularExpressions;
using NetMiner.Resource;
using System.Reflection;
using System.Resources ;
using NetMiner.Common;

namespace NetMiner.Common.Tool
{
    public class cVersion
    {
        public struct StructRegisterInfo
        {
            public string User;
            public string Keys;
            public string CpuID;
            public cGlobalParas.VersionType SominerVersion;
            public string SominerVersonCode;
        }

        private string m_workPath = string.Empty;
        public cVersion(string workPath)
        {
            this.m_workPath = workPath;
        }

        ~cVersion()
        {
        }

        private StructRegisterInfo m_RegisterInfo;
        public StructRegisterInfo RegisterInfo
        {
            get { return m_RegisterInfo; }
            set { m_RegisterInfo = value; }
        }

        private cGlobalParas.VersionType m_SominerVersion;
        public cGlobalParas.VersionType SominerVersion
        {
            get { return m_SominerVersion; }
            set { m_SominerVersion = value; }
        }

        private string m_SominerVersionCode;
        public string SominerVersionCode
        {
            get { return m_SominerVersionCode; }
            set { m_SominerVersionCode = value; }
        }


        //根据算法校验序列号的合法性
        public bool VerifyLicence(string User, string LicenceID)
        {
            try
            {
                byte[] bStr = (new UnicodeEncoding()).GetBytes(User);
                byte[] bKey = (new UnicodeEncoding()).GetBytes("yijie");

                for (int i = 0; i < bStr.Length; i += 2)
                {
                    for (int j = 0; j < bKey.Length; j += 2)
                    {
                        bStr[i] = Convert.ToByte(bStr[i] ^ bKey[j]);
                    }
                }

                string sss = "";

                for (int i = 0; i < bStr.Length; i += 2)
                {
                    sss += bStr[i].ToString("X");
                }

                LicenceID = LicenceID.Substring(0, 3) + LicenceID.Substring(5, 4) + LicenceID.Substring(10, 4);

                //if (LicenceID.StartsWith(sss))
                if (sss.StartsWith(LicenceID))
                    return true;
                else
                    return false;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        //在线进行校验，序列号的合法性，如果合法，则在本地写注册文件，表示在线激活
        public cGlobalParas.RegisterResult VerfyOnline(string User, string LicenceID)
        {

            string Url = "http://www.netminer.cn/RegiserSominer.aspx?user=" + User + "&licenceid=" + LicenceID ;
            int Again = 0;
            string r;

        AgainL:
            try
            {
                r = GetHtmlSource(Url, false);
            }
            catch (System.Exception ex)
            {
                if (Again > 1)
                    throw ex;
                else
                {
                    Url = "http://www.netminer.cn/RegiserSominer.aspx?user=" + User + "&licenceid=" + LicenceID;
                    Again++;
                    goto AgainL;
                }
            }

            switch (r)
            {
                case "UnRegister":
                    return cGlobalParas.RegisterResult.Failed;
                case "Disable":
                    return cGlobalParas.RegisterResult.Failed;
                case "Failed":
                    return cGlobalParas.RegisterResult.Failed;
                default:
                    try
                    {
                        Registe(User, LicenceID, r);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }

                    return cGlobalParas.RegisterResult.Succeed;
            }

        }

        //在线进行校验，序列号的合法性，如果合法，则在本地写注册文件，表示在线激活
        /// <summary>
        /// 验证时，把版本传进来进行匹配，如果低于当前的版本，可以让用户选择升级
        /// </summary>
        /// <param name="User"></param>
        /// <param name="LicenceID"></param>
        /// <param name="curVersion"></param>
        /// <returns></returns>
        public cGlobalParas.RegisterResult VerfyOnline(string User, string LicenceID, string curVersion)
        {

            string Url = "http://www.netminer.cn/RegiserSominer.aspx?user=" + User + "&licenceid=" + LicenceID;

            int Again = 0;
            string r;

        AgainL:
            try
            {
                r = GetHtmlSource(Url, false);
            }
            catch (System.Exception ex)
            {
                if (Again > 1)
                    return cGlobalParas.RegisterResult.Succeed;
                else
                {
                    Url = "http://www.netminer.cn/RegiserSominer.aspx?user=" + User + "&licenceid=" + LicenceID;
                    Again++;
                    goto AgainL;
                }
            }

            switch (r)
            {
                case "UnRegister":
                    return cGlobalParas.RegisterResult.Failed;
                case "Disable":
                    return cGlobalParas.RegisterResult.Failed;
                case "Failed":
                    return cGlobalParas.RegisterResult.Failed;
                default:
                    try
                    {
                        //开始匹配版本
                        string[] cVer = curVersion.Split('.');
                        string[] rVer = r.Split('.');

                        bool isMatch = false;

                        if (int.Parse(rVer[0]) == int.Parse(cVer[0]) &&
                            int.Parse(rVer[1]) == int.Parse(cVer[1]) &&
                            int.Parse(rVer[2]) == int.Parse(cVer[2]))
                        {
                            isMatch = true;
                        }
                        else
                        {
                            if (int.Parse(rVer[0]) < int.Parse(cVer[0]))
                            {
                                return cGlobalParas.RegisterResult.Upgrade;
                            }
                            else if (int.Parse(rVer[0]) == int.Parse(cVer[0]))
                            {
                                if (int.Parse(rVer[1]) < int.Parse(cVer[1]))
                                {
                                    return cGlobalParas.RegisterResult.Upgrade;
                                }
                                else if (int.Parse(rVer[1]) == int.Parse(cVer[1]))
                                {
                                    if (int.Parse(rVer[2]) < int.Parse(cVer[2]))
                                    {
                                        return cGlobalParas.RegisterResult.Upgrade;
                                    }
                                    else
                                    {
                                        return cGlobalParas.RegisterResult.NoMatch;
                                    }
                                }
                                else if (int.Parse(rVer[1]) > int.Parse(cVer[1]))
                                {
                                    return cGlobalParas.RegisterResult.NoMatch;
                                }
                            }
                            else if (int.Parse(rVer[0]) > int.Parse(cVer[0]))
                            {
                                return cGlobalParas.RegisterResult.NoMatch;
                            }
                        }

                        if (isMatch == true)
                            Registe(User, LicenceID, r);
                        else
                            return cGlobalParas.RegisterResult.NoMatch;
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }

                    return cGlobalParas.RegisterResult.Succeed;
            }

        }

        //写注册文件,此方法只有在在线验证成功后，才会调用写入，其他情况均不允许调用
        public void Registe(string User, string Keys, string Version)
        {
            string strKeys = User + ";" + Keys + ";" + GetCPUID() + ";" + Version;
            //strKeys = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(strKeys));
            strKeys = ToolUtil.Encrypt(strKeys);

            string KeyFile = this.m_workPath + "components\\SoMiner.key";

            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }

            FileStream myStream = File.Open(KeyFile, FileMode.Create, FileAccess.Write, FileShare.Write);
            //StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
            BinaryWriter sw = new BinaryWriter(myStream, System.Text.Encoding.UTF8);
            byte[] filebyte=System.Text.Encoding.UTF8.GetBytes (strKeys);
            sw.Write(filebyte);
            //sw.WriteLine(strKeys);
            sw.Close();
            myStream.Close();

        }

        public bool ReadRegisterInfo()
        {
            try
            {
                string KeyFile = this.m_workPath + "components\\SoMiner.key";
                FileStream myStream = File.Open(KeyFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //StreamReader sr = new StreamReader(KeyFile, System.Text.Encoding.UTF8);

                BinaryReader sr = new BinaryReader(myStream, System.Text.Encoding.UTF8);
                int fLength = (int)myStream.Length;
                byte[] filebyte = new byte[fLength];
                filebyte = sr.ReadBytes(fLength);
                string str = System.Text.Encoding.UTF8.GetString(filebyte);

                sr.Close();
                myStream.Close();

                //byte[] brInfo = (Convert.FromBase64String(str));
                string rInfo = ToolUtil.Decrypt(str);

                int i = 0;
                foreach (string sc in rInfo.Split(';'))
                {
                    switch (i)
                    {
                        case 0:
                            m_RegisterInfo.User = sc.Trim();
                            break;
                        case 1:
                            m_RegisterInfo.Keys = sc.Trim();
                            break;
                        case 2:
                            m_RegisterInfo.CpuID = sc.Trim();
                            break;
                        case 3:
                            //版本判断，版本格式为：00.00.00.00

                            m_RegisterInfo.SominerVersonCode = sc.ToString();
                            this.SominerVersionCode = sc.ToString();

                            string SVersion = sc.Substring(sc.Length - 2, 2);

                            switch (SVersion)
                            {
                         
                                case "01":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Program ;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Program;
                                    break;
                                case "02":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free ;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Free;
                                    break;
                                case "03":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Cloud ;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Cloud;
                                    break;
                                case "04":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Ultimate   ;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Ultimate;
                                    break;
                                case "05":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Enterprise  ;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Enterprise;
                                    break;
                                case "06":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Server;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Server;
                                    break;
                                default:
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Free;
                                    break;
                            }
                            break;
                    }
                    i++;
                }

                //如果是个人版、专业版或商业版，则需要验证CPU是否匹配
                if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                    m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Enterprise || 
                    m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Program || 
                    m_RegisterInfo.SominerVersion ==cGlobalParas.VersionType.Ultimate)
                {
                    if (!VerifyCPU())
                    {
                        m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                        this.m_SominerVersion = cGlobalParas.VersionType.Free;
                    }
                }
            }
            catch (System.Exception )
            {
                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                this.m_RegisterInfo.User = "sominer";
                this.m_RegisterInfo.Keys = "519F-1B1F-1813";
                this.m_RegisterInfo.CpuID = "thisisfreeversion";
                string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString(); 

                //将最后一位补齐
                string lastVer = ver.Substring(ver.LastIndexOf(".") + 1, ver.Length - ver.LastIndexOf(".") - 1);

                if (lastVer.Length == 1)
                    lastVer = "0" + lastVer;

                ver = ver.Substring(0, ver.LastIndexOf(".")) + "." + lastVer;

                m_RegisterInfo.SominerVersonCode = ver;
                this.SominerVersionCode = ver;
                this.m_SominerVersion = cGlobalParas.VersionType.Free;
                return true ;
            }

            return true;
        }

        /// <summary>
        /// 读取网络矿工注册信息，当前仅用于网络矿工企业版
        /// </summary>
        /// <returns></returns>
        public bool ReadRegisterInfo(string vPath)
        {
            try
            {
                string KeyFile = vPath + "\\SoMiner.key";
                FileStream myStream = File.Open(KeyFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //StreamReader sr = new StreamReader(KeyFile, System.Text.Encoding.UTF8);

                BinaryReader sr = new BinaryReader(myStream, System.Text.Encoding.UTF8);
                int fLength = (int)myStream.Length;
                byte[] filebyte = new byte[fLength];
                filebyte = sr.ReadBytes(fLength);
                string str = System.Text.Encoding.UTF8.GetString(filebyte);

                sr.Close();
                myStream.Close();

                //byte[] brInfo = (Convert.FromBase64String(str));
                string rInfo = ToolUtil.Decrypt(str);

                int i = 0;
                foreach (string sc in rInfo.Split(';'))
                {
                    switch (i)
                    {
                        case 0:
                            m_RegisterInfo.User = sc.Trim();
                            break;
                        case 1:
                            m_RegisterInfo.Keys = sc.Trim();
                            break;
                        case 2:
                            m_RegisterInfo.CpuID = sc.Trim();
                            break;
                        case 3:
                            //版本判断，版本格式为：00.00.00.00

                            m_RegisterInfo.SominerVersonCode = sc.ToString();
                            this.SominerVersionCode = sc.ToString();

                            string SVersion = sc.Substring(sc.Length - 2, 2);

                            switch (SVersion)
                            {

                                case "01":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Program;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Program;
                                    break;
                                case "02":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Free;
                                    break;
                                case "03":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Cloud;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Cloud;
                                    break;
                                case "04":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Ultimate;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Ultimate;
                                    break;
                                case "05":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Enterprise;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Enterprise;
                                    break;
                                case "06":
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Server;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Server;
                                    break;
                                case "07":
                                     m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.DistriServer;
                                     this.m_SominerVersion = cGlobalParas.VersionType.DistriServer;
                                    break;
                                default:
                                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                                    this.m_SominerVersion = cGlobalParas.VersionType.Free;
                                    break;
                            }
                            break;
                    }
                    i++;
                }

                //读取出来信息后，到服务器验证一下
                cGlobalParas.VerifyUserd vUser = VerifyUerd(m_RegisterInfo.User, m_RegisterInfo.Keys, m_RegisterInfo.CpuID, m_RegisterInfo.SominerVersonCode);

                if (vUser!=cGlobalParas.VerifyUserd.Available)
                {
                    return false;
                }


                //如果是个人版、专业版或商业版，则需要验证CPU是否匹配
                if (ToolUtil.GetCurrentVersion() == cGlobalParas.VersionType.Cloud ||
                    m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Enterprise ||
                    m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Program ||
                    m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Ultimate)
                {
                    if (!VerifyCPU())
                    {
                        m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                        this.m_SominerVersion = cGlobalParas.VersionType.Free;
                    }
                }
            }
            catch (System.Exception)
            {
                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Free;
                this.m_RegisterInfo.User = "sominer";
                this.m_RegisterInfo.Keys = "519F-1B1F-1813";
                this.m_RegisterInfo.CpuID = "thisisfreeversion";
                string ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                //将最后一位补齐
                string lastVer = ver.Substring(ver.LastIndexOf(".") + 1, ver.Length - ver.LastIndexOf(".") - 1);

                if (lastVer.Length == 1)
                    lastVer = "0" + lastVer;

                ver = ver.Substring(0, ver.LastIndexOf(".")) + "." + lastVer;

                m_RegisterInfo.SominerVersonCode = ver;
                this.SominerVersionCode = ver;
                this.m_SominerVersion = cGlobalParas.VersionType.Free;
                return true;
            }

            return true;
        }


        private string GetCPUID()
        {
            try
            {
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();

                String strCpuID = null;
                foreach (ManagementObject mo in moc)
                {
                    strCpuID = mo.Properties["ProcessorId"].Value.ToString();
                    break;
                }
                return strCpuID;
            }
            catch
            {
                return "";
            }
        }

        //写第一次运行的时间,采用GUID来标识系统身份
        //private void WriteFirstRunTime(string sVersion)
        //{
        //    RegistryKey SoGUID;

        //    if (sVersion.EndsWith("03"))
        //    {
        //        SoGUID = Registry.ClassesRoot.CreateSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
        //    }
        //    else if (sVersion.EndsWith("02"))
        //    {
        //        SoGUID = Registry.ClassesRoot.CreateSubKey("AppID\\dc160e76-0648-44fe-88d8-d7771cdfeb07");
        //    }
        //    else
        //    {
        //        SoGUID = Registry.ClassesRoot.CreateSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
        //    }

        //    SoGUID.SetValue("AccessPermission", DateTime.Now.ToBinary(), RegistryValueKind.String);
        //    SoGUID.Close();

        //}

        //public DateTime GetFirstRunTime(string sVersion)
        //{
        //    DateTime FirstDate;

        //    try
        //    {
        //        RegistryKey SoGUID;

        //        if (sVersion.EndsWith("03"))
        //        {
        //            SoGUID = Registry.ClassesRoot.OpenSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
        //        }
        //        else if (sVersion.EndsWith("02"))
        //        {
        //            SoGUID = Registry.ClassesRoot.OpenSubKey("AppID\\dc160e76-0648-44fe-88d8-d7771cdfeb07");
        //        }
        //        else
        //        {
        //            SoGUID = Registry.ClassesRoot.OpenSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
        //        }

        //        if (SoGUID == null)
        //        {
        //            WriteFirstRunTime(sVersion);
        //            FirstDate = DateTime.Now;
        //        }
        //        else
        //        {
        //            FirstDate = DateTime.FromBinary(long.Parse(SoGUID.GetValue("AccessPermission").ToString()));
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        return System.DateTime.Now;
        //    }

        //    return FirstDate;
        //}

        //验证注册文件中的CPU与实际的CPU是否一致，如果
        //一致则表示正确，如果不一致则表示此注册文件时拷贝
        //而来，还是试用版，true为正式版，false为拷贝的文件
        private bool VerifyCPU()
        {
            if (m_RegisterInfo.CpuID == GetCPUID())
                return true;
            else
                return false;
        }

        //定期验证用户的合法授权数及版本的合法授权
        public cGlobalParas.VerifyUserd VerifyUerd(string User, string LicenceID, string CPUID,string Version)
        {
            string Url="http://www.netminer.cn/verifyUserLicence.aspx?user=" + User + "&licenceid=" + LicenceID + "&cpuid=" + CPUID + "&Version=" + Version;
            string r = "";
            int Again=0;

            AgainL:

            try
            {
                r = GetHtmlSource(Url, false);
            }
            catch (System.Exception)
            {
                if (Again>1)
                //表示网站出错，如果网站出错，则应该返回激活信息，不能影响终端使用
                return cGlobalParas.VerifyUserd.Available;
                else
                {
                    Url="http://www.soukey.com/verifyUserLicence.aspx?user=" + User + "&licenceid=" + LicenceID + "&cpuid=" + CPUID + "&Version=" + Version;
                    Again++;
                    goto AgainL;

                }
            }

            switch (r)
            {
                case "UnRegister":
                    return cGlobalParas.VerifyUserd.UnRegister;
                case "Available":
                    return cGlobalParas.VerifyUserd.Available;
                case "Disable":
                    return cGlobalParas.VerifyUserd.Disable;
                default:
                    //其他情况也返回激活，也是避免由于错误造成的客户端无法
                    //使用
                    return cGlobalParas.VerifyUserd.Available;
            }

            //return cGlobalParas.VerifyUserd.Failed;
        }

        private string GetHtmlSource(string url, bool Isbool)
        {
            if (url == "")
                return "";

            string charSet = "";
            WebClient myWebClient = new WebClient();
            myWebClient.Proxy = null;

            //获取或设置用于对向 Internet 资源的请求进行身份验证的网络凭据。   
            myWebClient.Credentials = CredentialCache.DefaultCredentials;


            byte[] myDataBuffer;
            string strWebData;

            try
            {
                //从资源下载数据并返回字节数组。（加@是因为网址中间有"/"符号）   
                myDataBuffer = myWebClient.DownloadData(@url);
                strWebData = Encoding.Default.GetString(myDataBuffer);
            }
            catch (System.Net.WebException ex)
            {
                throw ex;
            }

            //获取此页面的编码格式
            Match charSetMatch = Regex.Match(strWebData, "<meta([^<]*)charset=([^<]*)\"", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string webCharSet = charSetMatch.Groups[2].Value;
            if (charSet == null || charSet == "")
                charSet = webCharSet;

            if (charSet != null && charSet != "" && Encoding.GetEncoding(charSet) != Encoding.Default)
                strWebData = Encoding.GetEncoding(charSet).GetString(myDataBuffer);

            if (Isbool == true)
            {
                strWebData = Regex.Replace(strWebData, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                strWebData.Replace(@"\r\n", "");
            }

            return strWebData;

        }
    }
} 
