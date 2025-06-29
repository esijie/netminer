using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.IO;

namespace SoukeyVersion
{
    public class cRegister
    {
        public struct StructRegisterInfo
        {
            public string User;
            public string Keys;
            public string CpuID;
        }

        private StructRegisterInfo m_RegisterInfo ;

        public cRegister()
        {
        }

        ~cRegister()
        {
        }

        public StructRegisterInfo RegisterInfo
        {
            get { return m_RegisterInfo; }
            set { m_RegisterInfo = value; }
        }
        

        public bool Registe(string User, string Keys)
        {
            string strKeys = User + ";" + Keys + ";" + GetCPUID();
            strKeys = Convert.ToBase64String(System.Text.Encoding.Default.GetBytes(strKeys));

            string KeyFile = "SomMiner.key";

            if (File.Exists(KeyFile))
            {

            }
            else
            {
                FileStream myStream = File.Open(KeyFile, FileMode.Create, FileAccess.Write, FileShare.Write);
                StreamWriter sw = new StreamWriter(myStream, System.Text.Encoding.UTF8);
                sw.WriteLine(strKeys);
                sw.Close();
                myStream.Close();
            }

            return true;
        }

        //public bool ReadRegisterInfo()
        //{
        //    string KeyFile = "SomMiner.key";
        //    FileStream myStream = File.Open(KeyFile, FileMode.Open, FileAccess.Read , FileShare.Read);
        //    StreamReader sr = new StreamReader(KeyFile, System.Text.Encoding.UTF8);
        //    string str = sr.ReadToEnd();

        //    sr.Close();
        //    myStream.Close();

        //    byte[] brInfo = (Convert.FromBase64String(str));
        //    string rInfo = System.Text.Encoding.Default.GetString(brInfo);

        //    int i = 0;
        //    foreach (string sc in rInfo.Split(';'))
        //    {
        //        switch (i)
        //        {
        //            case 0:
        //                m_RegisterInfo.User = sc.Trim();
        //                break;
        //            case 1:
        //                m_RegisterInfo.Keys = sc.Trim();
        //                break;
        //            case 2:
        //                m_RegisterInfo.CpuID = sc.Trim();
        //                break;
        //         }
        //            i++;
        //    }

        //    return true;
        //}

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
    }
}
