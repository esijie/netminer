using System;
using System.Collections.Generic;
using System.Text;

namespace NetMinerHttpHelp.Net
{
    public enum ProxyType
    {
        None = 1000,
        /// <summary>
        /// http代理
        /// </summary>
        HttpProxy = 1001,
        /// <summary>
        /// Socket5代理
        /// </summary>
        Socket5 = 1002,
        /// <summary>
        /// 系统代理
        /// </summary>
        SystemProxy = 1003,
    }

    public enum Method
    {
        GET = 1010,
        POST = 1012,
        
    }

    public enum enCodingCode
    {
        auto = 1000,
        gb2312 = 1001,
        utf8 = 1002,
        gbk = 1003,
        big5 = 1004,
        NoCoding=1005,
    }

    internal enum HTTPHeaderParseWarnings
    {
        EndedWithLFCRLF = 2,
        EndedWithLFLF = 1,
        Malformed = 4,
        None = 0
    }

    #region 定义代理处理异步回调
    /// <summary>
    /// Socket连接回调
    /// </summary>
    /// <param name="connected"></param>
    internal delegate void ConnectionEventHandler(bool connected);

    /// <summary>
    /// 接收数据完成回调
    /// </summary>
    internal delegate void ReceiveCompleteHandler();

    /// <summary>
    /// 接收完头部信息回调
    /// </summary>
    /// <param name="response"></param>
    internal delegate void ReceiveHeaderHandler(HttpWebResponse response);

    /// <summary>
    /// 每次接收完数据回调
    /// </summary>
    /// <param name="bytes"></param>
    internal delegate void ReceiveEventHandler(byte[] bytes, int bLen);
    #endregion
}
