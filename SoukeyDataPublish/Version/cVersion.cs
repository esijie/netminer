using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Management;
using Microsoft.Win32;

namespace SoukeyDataPublish.Version
{
    class cVersion
    {
        public struct StructRegisterInfo
        {
            public string User;
            public string Keys;
            public string CpuID;
            public cGlobalParas.VersionType SominerVersion;
        }

        public cVersion()
        {
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

        //在线进行校验，序列号的合法性
        public cGlobalParas.RegisterResult VerfyOnline(string User, string LicenceID)
        {

            string Url = "http://www.yijie.net/verify.aspx?user=" + User + "&licenceid=" + LicenceID;
            string r;

            try
            {
                r = cTool.GetHtmlSource(Url, false);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            switch (r)
            {
                case "Available":
                    try
                    {
                        Registe(User, LicenceID);
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                    return cGlobalParas.RegisterResult.Succeed;
                case "UnRegister":
                    return cGlobalParas.RegisterResult.Failed ;
                case "Disable":
                    return cGlobalParas.RegisterResult.Failed;
                case "Failed":
                    return cGlobalParas.RegisterResult.Failed;
                default :
                    return cGlobalParas.RegisterResult.Failed;
            }

        }

        //写注册文件,此方法只有在在线验证成功后，才会调用写入，其他情况均不允许调用
        private void Registe(string User, string Keys)
        {
            string strKeys = User + ";" + Keys + ";" + GetCPUID() + ";" + "Professional V1.8";
            strKeys = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(strKeys));

            string KeyFile = "SomMiner.key";

            if (File.Exists(KeyFile))
            {
                File.Delete(KeyFile);
            }

            FileStream myStream = File.Open(KeyFile, FileMode.Create, FileAccess.Write, FileShare.Write);
            StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
            sw.WriteLine(strKeys);
            sw.Close();
            myStream.Close();

        }

        /// <summary>
        /// 注册测试版本的key文件
        /// </summary>
        /// <param name="LastDateTime">最后的使用时间</param>
        public void RegisterBeta(DateTime LastDateTime)
        {

        }

        public bool ReadRegisterInfo()
        {
            try
            {
                string KeyFile = "SomMiner.key";
                FileStream myStream = File.Open(KeyFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(KeyFile, System.Text.Encoding.UTF8);
                string str = sr.ReadToEnd();

                sr.Close();
                myStream.Close();

                byte[] brInfo = (Convert.FromBase64String(str));
                string rInfo = System.Text.Encoding.Default.GetString(brInfo);

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
                            if (sc.Trim().StartsWith("Business"))
                            {
                                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Business;
                                this.m_SominerVersion = cGlobalParas.VersionType.Business;
                            }
                            else if (sc.Trim().StartsWith("Beta"))
                            {
                                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Beta;
                                this.m_SominerVersion = cGlobalParas.VersionType.Beta;
                            }
                            else
                            {
                                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Trial;
                                this.m_SominerVersion = cGlobalParas.VersionType.Trial;
                            }
                            break;
                    }
                    i++;
                }

                if (m_RegisterInfo.SominerVersion == cGlobalParas.VersionType.Business)
                {
                    //如果是商业版，则需要验证CPU是否匹配
                    if (!VerifyCPU())
                    {
                        m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Trial;
                        this.m_SominerVersion = cGlobalParas.VersionType.Trial;
                    }
                }
            }
            catch (System.Exception ex)
            {
                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Trial;
                this.m_SominerVersion = cGlobalParas.VersionType.Trial;
                return false;
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
        public void WriteFirstRunTime()
        {
            RegistryKey SoGUID = Registry.ClassesRoot.CreateSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
            SoGUID.SetValue("AccessPermission", DateTime.Now.ToBinary(), RegistryValueKind.String);
            SoGUID.Close();

        }

        public DateTime GetFirstRunTime()
        {
            DateTime FirstDate;

            try
            {
                RegistryKey SoGUID = Registry.ClassesRoot.OpenSubKey("AppID\\5451caf5-59e8-4671-9abc-5921ff53f90b");
                if (SoGUID == null)
                {
                    WriteFirstRunTime();
                    FirstDate = DateTime.Now;
                }
                else
                {
                    FirstDate = DateTime.FromBinary(long.Parse(SoGUID.GetValue("AccessPermission").ToString()));
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            return FirstDate;
        }

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
        public cGlobalParas.VerifyUserd VerifyUerd(string User,string LicenceID,string CPUID)
        {
            string r = "";

            try
            {
                r = cTool.GetHtmlSource("http://www.yijie.net/verifyUsers.aspx?user=" + User + "&licenceid=" + LicenceID + "&cpuid=" + CPUID, false);
            }
            catch (System.Exception)
            {
                //表示网站出错，如果网站出错，则应该返回激活信息，不能影响终端使用
                return cGlobalParas.VerifyUserd.Available  ;
            }

            switch (r)
            {
                case "UnRegister":
                    return cGlobalParas.VerifyUserd.UnRegister;
                case "Available":
                    return cGlobalParas.VerifyUserd.Available;
                case "Disable":
                    return cGlobalParas.VerifyUserd.Disable;
                default :
                    //其他情况也返回激活，也是避免由于错误造成的客户端无法
                    //使用
                    return cGlobalParas.VerifyUserd.Available;
            }

            return cGlobalParas.VerifyUserd.Failed;
        }

        
    }
}
