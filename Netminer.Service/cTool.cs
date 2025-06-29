using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SoukeyService
{
    static class cTool
    {
  

        /// <summary>
        /// 将加密后的数据库连接字符串的密码解码
        /// </summary>
        /// <param name="con"></param>
        /// <returns></returns>
        static public string DecodingDBCon(string con)
        {
            if (Regex.IsMatch(con, "password="))
            {
                Match charSetMatch = Regex.Match(con, "(?<=password=).*?(?=;)", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                string pwd = charSetMatch.Groups[0].ToString();

                if (pwd != "")
                {
                    string strPre = con.Substring(0, con.IndexOf("password="));
                    int startI = con.IndexOf("password=");
                    startI += pwd.Length + 9;
                    string strSuffix = con.Substring(startI, con.Length - startI);

                    pwd = cTool.Base64Decoding(pwd);
                    con = strPre + "password=" + pwd + strSuffix;
                }
            }

            return con;
        }

      

        static public string Base64Decoding(string str)
        {
            byte[] outputb = Convert.FromBase64String(str);
            string orgStr = Encoding.Default.GetString(outputb);
            return orgStr;
        }
    }
}
