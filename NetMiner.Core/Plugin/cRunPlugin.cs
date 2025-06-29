using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using NetMiner.Plugins;
using System.Data;

namespace NetMiner.Core.Plugin
{
    public class cRunPlugin
    {
        public string CallGetCookie(string Url,string TaskName, string strDll)
        {
            Type ObjType = null;
            IPlugins ipi;

            string strcookie = "";
            try
            {
                // load it
                Assembly ass = null;
                ass = Assembly.LoadFrom(strDll);
                string dllass = System.IO.Path.GetFileNameWithoutExtension(strDll);
                if (ass != null)
                {
                    ObjType = ass.GetType(dllass + ".RunPlugin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (ObjType != null)
                {
                    ipi = (IPlugins)Activator.CreateInstance(ObjType);
                    strcookie = ipi.GetCookie(Url, TaskName);

                }
            }
            catch (Exception ex)
            {
                return "";
            }

            return strcookie;
        }

        public DataTable CallDealData(DataTable d,string strDll)
        {
            Type ObjType = null;
            IPlugins ipi;

            DataTable tmpData=null;
            try
            {
                // load it
                Assembly ass = null;
                ass = Assembly.LoadFrom(strDll);
                string dllass = System.IO.Path.GetFileNameWithoutExtension(strDll);
                if (ass != null)
                {
                    ObjType = ass.GetType(dllass + ".RunPlugin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (ObjType != null)
                {
                    ipi = (IPlugins)Activator.CreateInstance(ObjType);
                    tmpData = ipi.TransData(d);

                }
            }
            catch (Exception ex)
            {
                return d;
            }

            return tmpData;
        }

        public void CallPublishData(DataTable d, string strDll)
        {
            Type ObjType = null;
            IPlugins ipi;

            try
            {
                // load it
                Assembly ass = null;
                ass = Assembly.LoadFrom(strDll);
                string dllass = System.IO.Path.GetFileNameWithoutExtension(strDll);
                if (ass != null)
                {
                    ObjType = ass.GetType(dllass + ".RunPlugin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
            try
            {
                if (ObjType != null)
                {
                    ipi = (IPlugins)Activator.CreateInstance(ObjType);
                    ipi.PublishData(d);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void CallSetConfig(string strDll)
        {
            Type ObjType = null;
            IPlugins ipi;

            try
            {
                // load it
                Assembly ass = null;
                ass = Assembly.LoadFrom(strDll);
                string dllass = System.IO.Path.GetFileNameWithoutExtension(strDll);
                if (ass != null)
                {
                    ObjType = ass.GetType(dllass + ".RunPlugin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (ObjType != null)
                {
                    ipi = (IPlugins)Activator.CreateInstance(ObjType);
                    ipi.Config();

                }
            }
            catch (Exception ex)
            {
                return;
            }

        }

        public string CallVerifyCode(string imageName,string imageUrl,string imgCookie,out string outCookie, string strDll)
        {
            outCookie = "";

            Type ObjType = null;
            IPlugins ipi;

            string strCode = string.Empty;

            try
            {
                // load it
                Assembly ass = null;
                ass = Assembly.LoadFrom(strDll);
                string dllass = System.IO.Path.GetFileNameWithoutExtension(strDll);
                if (ass != null)
                {
                    ObjType = ass.GetType(dllass + ".RunPlugin");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            try
            {
                if (ObjType != null)
                {
                    ipi = (IPlugins)Activator.CreateInstance(ObjType);
                    strCode = ipi.GetVerifyCode(imageName, imageUrl);

                }
            }
            catch (Exception ex)
            {
                return strCode;
            }

            return strCode;
        }
    }
}
