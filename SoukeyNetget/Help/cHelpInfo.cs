using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using NetMiner.Common;
using NetMiner.Base;

namespace MinerSpider.Help
{
    public class cHelpInfo
    {
        DataView dv;

        public cHelpInfo()
        {
            try
            {
                //加载帮助信息
                cXmlIO cx = new cXmlIO(Program.getPrjPath() + "HelpContent.xml");
                dv = cx.GetData("descendant::Help");
                cx = null;
            }
            catch (System.Exception)
            {
                return;
            }
        }

        ~cHelpInfo()
        {
            dv = null;
        }

        public cHelpContent GetByKey(string key)
        {
            if (dv != null)
            {

                for (int i = 0; i < dv.Count; i++)
                {
                    if (dv[i].Row["Title"].ToString().IndexOf(key) > -1)
                    {
                        cHelpContent hContent = new cHelpContent();
                        hContent.Title = dv[i].Row["Title"].ToString();
                        hContent.Content = dv[i].Row["Content"].ToString();
                        return hContent;
                    }
                }
            }

            return null;
        }




    }
}