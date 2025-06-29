using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace NetMiner.Net.Native
{
    /// <summary>
    /// 传入多个cookie，以\r\n分割，然后逐一取出每个cookie
    /// </summary>
    public class cCookieManage
    {
        public cCookieManage(string cookie)
        {
            index = 0;
            string[] ss = Regex.Split(cookie, "\r\n");

            ArrayList al = new ArrayList();
            for (int i = 0; i < ss.Length; i++)
            {
                if (!string.IsNullOrEmpty(ss[i]))
                {
                    al.Add(ss[i]);
                }
            }

            CookieList = (string[])al.ToArray(typeof(string));
           
        }

        public KeyValuePair<int,string> getCookie()
        {
            if (CookieList.Length == 0)
            {
                return new KeyValuePair<int, string>(0, "");
            }
            else
            {
                if (this.index >= CookieList.Length)
                {
                    this.index = 0;
                }

                KeyValuePair<int,string> kv=new KeyValuePair<int, string>(index, CookieList[index]);
                index++;
                return kv; 
            }
        }

        public int index;
        public string[] CookieList { get; set; }

       
    }
}
