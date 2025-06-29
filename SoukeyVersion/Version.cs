using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Management;
using System.Management;
using System.IO;
using Microsoft.Win32;

///用于版本的验证，确认当前版本
///判断是否可用商业版功能，注册等操作
namespace SoukeyVersion
{
    public class Version
    {
        public struct StructRegisterInfo
        {
            public string User;
            public string Keys;
            public string CpuID;
            public cGlobalParas.VersionType SominerVersion;
        }

        public Version()
        {

        }

        ~Version()
        {

        }

        private StructRegisterInfo m_RegisterInfo;
        public StructRegisterInfo RegisterInfo
        {
            get { return m_RegisterInfo; }
            set { m_RegisterInfo = value; }
        }

        public void Registe(string User, string Keys)
        {
            string strKeys = User + ";" + Keys + ";" + GetCPUID();
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
                    }
                    i++;
                }
                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Business;
            }
            catch (System.Exception ex)
            {
                return false;
            }

            return true;
        }

        public cGlobalParas.VersionType GetVersion()
        {
            bool IsRegister = ReadRegisterInfo();
            if (IsRegister == false)
            {
                m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Trial;
                return cGlobalParas.VersionType.Trial;
            }
            else
            {
                if (this.RegisterInfo.Keys == "")
                {
                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Trial;
                    return cGlobalParas.VersionType.Trial;
                }
                else
                {
                    m_RegisterInfo.SominerVersion = cGlobalParas.VersionType.Business ;
                    return cGlobalParas.VersionType.Business;
                }
            }
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

    }
}
