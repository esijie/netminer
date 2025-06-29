using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;
using NetMiner.Resource;

namespace NetMiner.Net.Socket
{
    internal class cUtilities
    {
        /// <summary>
        /// 查找头部信息的结尾标记
        /// </summary>
        /// <param name="arrData"></param>
        /// <param name="iBodySeekProgress"></param>
        /// <param name="lngDataLen"></param>
        /// <param name="oWarnings"></param>
        /// <returns></returns>
        internal static bool FindEndOfHeaders(byte[] arrData, ref int iBodySeekProgress, long lngDataLen, out HTTPHeaderParseWarnings oWarnings)
        {
            bool flag;
            oWarnings = HTTPHeaderParseWarnings.None;
        Label_0003:
            flag = false;
            while (((long)iBodySeekProgress) < (lngDataLen - 1L))
            {
                iBodySeekProgress++;
                if (10 == arrData[iBodySeekProgress - 1])
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                if ((13 != arrData[iBodySeekProgress]) && (10 != arrData[iBodySeekProgress]))
                {
                    iBodySeekProgress++;
                    goto Label_0003;
                }
                if (10 == arrData[iBodySeekProgress])
                {
                    oWarnings = HTTPHeaderParseWarnings.EndedWithLFLF;
                    return true;
                }
                iBodySeekProgress++;
                if ((((long)iBodySeekProgress) < lngDataLen) && (10 == arrData[iBodySeekProgress]))
                {
                    if (13 != arrData[iBodySeekProgress - 3])
                    {
                        oWarnings = HTTPHeaderParseWarnings.EndedWithLFCRLF;
                    }
                    return true;
                }
                if (iBodySeekProgress > 3)
                {
                    iBodySeekProgress -= 4;
                }
                else
                {
                    iBodySeekProgress = 0;
                }
            }
            return false;
        }

        //判断Chunked分块数据是否接收完毕
        internal static bool IsChunkedBodyComplete(MemoryStream oData, long iStartAtOffset, out long outStartOfLatestChunk, out long outEndOfEntity)
        {
            long num = iStartAtOffset;
            outStartOfLatestChunk = num;
            outEndOfEntity = -1L;
            while (num < oData.Length)
            {
                outStartOfLatestChunk = num;
                oData.Position = num;
                byte[] buffer = new byte[0x20];
                oData.Read(buffer, 0, buffer.Length);
                string sInput = Encoding.ASCII.GetString(buffer);
                int index = sInput.IndexOf("\r\n", StringComparison.Ordinal);
                if (index > -1)
                {
                    num += index + 2;
                    sInput = sInput.Substring(0, index);
                }
                else
                {
                    return false;
                }
                index = sInput.IndexOf(';');
                if (index > -1)
                {
                    sInput = sInput.Substring(0, index);
                }
                int iOutput = 0;
                if (!TryHexParse(sInput, out iOutput))
                {
                    return true;
                }
                if (iOutput == 0)
                {
                    oData.Position = num;
                    bool flag = true;
                    bool flag2 = false;
                    for (int i = oData.ReadByte(); i != -1; i = oData.ReadByte())
                    {
                        int num5 = i;
                        if (num5 != 10)
                        {
                            if (num5 != 13)
                            {
                                goto Label_010C;
                            }
                            flag2 = true;
                        }
                        else if (flag2)
                        {
                            if (flag)
                            {
                                outEndOfEntity = oData.Position;
                                oData.Position = oData.Length;
                                return true;
                            }
                            flag = true;
                            flag2 = false;
                        }
                        else
                        {
                            flag2 = false;
                            flag = false;
                        }
                        continue;
                    Label_010C:
                        flag2 = false;
                        flag = false;
                    }
                    return false;
                }
                num += iOutput + 2;
            }
            oData.Position = oData.Length;
            return false;
        }

        internal static cGlobalParas.WebCode ConvertCode(string strCode)
        {
            switch (strCode.ToLower())
            {
                case "":
                    return cGlobalParas.WebCode.auto;
                case "gb2312":
                    return cGlobalParas.WebCode.gb2312;
                case "utf-8":
                    return cGlobalParas.WebCode.utf8;
                case "big5":
                    return cGlobalParas.WebCode.big5;
                case "gbk":
                    return cGlobalParas.WebCode.gbk;
                default:
                    return cGlobalParas.WebCode.auto ;
            }
        }

        internal static bool TryHexParse(string sInput, out int iOutput)
        {
            return int.TryParse(sInput, NumberStyles.HexNumber, NumberFormatInfo.InvariantInfo, out iOutput);
        }

        internal static MemoryStream ClearChunkedFlag(MemoryStream mStream,long bodyOffset)
        {
            mStream.Seek(bodyOffset, SeekOrigin.Begin);
            byte[] bytes = new byte[mStream.Length - bodyOffset];

            MemoryStream tmpStream = new MemoryStream();

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                byte[] b1=new byte[1];
                b1[0]=bytes[i];
                sb.Append(Encoding.ASCII.GetString(b1));
                if (sb.ToString().EndsWith("\r\n") && Regex.IsMatch (sb.ToString (),"[a-f0-9]+?\r\n"))
                {
                    string ss=sb.ToString ();
                    ss=ss.Substring(0, ss.Length - 2);
                    int len = 0;
                    if (TryHexParse(ss, out len))
                    {
                        tmpStream.Write(bytes, i + 1, len);
                        i = i + len;
                        sb.Remove(0, sb.ToString().Length);
                    }
                }

            }

            tmpStream.Seek(0, SeekOrigin.Begin);
            return tmpStream;
        }

    }


}
