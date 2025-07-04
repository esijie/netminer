﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Seg
{
    static  class cTool
    {
        public static string getPrjPath()
        {
            return "";
        }

        static public bool GetMatchStrings(String text, String regx,
            bool ignoreCase, out List<string> output)
        {
            output = new List<string>();

            output.Clear();

            System.Text.RegularExpressions.Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (GetMatch(text, endText, ignoreCase) == "")
                {
                    return false;
                }
            }

            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            System.Text.RegularExpressions.MatchCollection m = reg.Matches(text);

            if (m.Count == 0)
                return false;

            for (int j = 0; j < m.Count; j++)
            {
                int count = m[j].Groups.Count;

                for (int i = 1; i < count; i++)
                {
                    output.Add(m[j].Groups[i].Value.Trim());
                }
            }

            return true;

        }

        static public bool GetSingleMatchStrings(String text, String regx,
            bool ignoreCase, out List<string> output)
        {
            output = new List<string>();

            System.Text.RegularExpressions.Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (GetMatch(text, endText, ignoreCase) == "")
                {
                    return false;
                }
            }

            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            System.Text.RegularExpressions.MatchCollection m = reg.Matches(text);

            if (m.Count == 0)
                return false;

            for (int j = 0; j < m.Count; j++)
            {
                int count = m[j].Groups.Count;

                if (count > 0)
                {
                    output.Add(m[j].Groups[count - 1].Value.Trim());
                }
            }

            return true;

        }


        static public bool GetSplitWithoutFirstStrings(String text, String regx,
            bool ignoreCase, ref ArrayList output)
        {

            if (output == null)
            {
                return false;
            }

            output.Clear();

            System.Text.RegularExpressions.Regex reg;

            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            String[] strs = reg.Split(text);
            if (strs == null)
            {
                return false;
            }

            if (strs.Length <= 1)
            {
                return false;
            }

            for (int j = 1; j < strs.Length; j++)
            {
                output.Add(strs[j]);
            }

            return true;

        }


        static public String GetMatch(String text, String regx, bool ignoreCase)
        {
            System.Text.RegularExpressions.Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (endText != "")
                {
                    if (GetMatch(text, endText, ignoreCase) == "")
                    {
                        return "";
                    }
                }
            }

            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            String ret = "";
            System.Text.RegularExpressions.Match m = reg.Match(text);

            if (m.Groups.Count > 0)
            {
                ret = m.Groups[m.Groups.Count - 1].Value;
            }

            return ret;
        }

        static public String GetMatchSum(String text, String regx, bool ignoreCase)
        {
            System.Text.RegularExpressions.Regex reg;

            int index = 0;
            int begin = 0;
            index = regx.IndexOf("(.+)");
            if (index < 0)
            {
                index = regx.IndexOf("(.+?)");
                if (index >= 0)
                {
                    begin = index + 5;
                }
            }
            else
            {
                begin = index + 4;
            }

            if (index >= 0)
            {
                String endText = regx.Substring(begin);

                if (GetMatch(text, endText, ignoreCase) == "")
                {
                    return "";
                }
            }


            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            String ret = "";
            System.Text.RegularExpressions.Match m = reg.Match(text);

            for (int i = 1; i < m.Groups.Count; i++)
            {
                ret += m.Groups[i].Value;
            }
            return ret;
        }

        public static String[] Split(String Src, String SplitStr)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(SplitStr);
            return reg.Split(Src);
        }

        public static String[] Split(String Src, String SplitStr, System.Text.RegularExpressions.RegexOptions option)
        {
            System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(SplitStr, option);
            return reg.Split(Src);
        }

        public static String Replace(String text, String regx, String newText, bool ignoreCase)
        {
            System.Text.RegularExpressions.Regex reg;

            if (ignoreCase)
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Singleline);
            }
            else
            {
                reg = new System.Text.RegularExpressions.Regex(regx, System.Text.RegularExpressions.RegexOptions.Singleline);
            }

            return reg.Replace(text, newText);

        }
    }
}
