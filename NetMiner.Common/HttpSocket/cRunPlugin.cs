using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data;
using NetMiner.Plugins;

namespace NetMiner.Common.HttpSocket
{
    public class cRunPlugin
    {
        public string CallVerifyCode(string imageName,string imageUrl,string strDll)
        {
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
