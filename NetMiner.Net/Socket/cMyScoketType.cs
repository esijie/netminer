using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Net.Socket
{
    #region 枚举类型
    internal enum SocketOptionLevel
    {
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 IP 套接字。
        IP = 0,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 TCP 套接字。
        Tcp = 6,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 UDP 套接字。
        Udp = 17,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项仅适用于 IPv6 套接字。
        IPv6 = 41,
        //
        // 摘要:
        //     System.Net.Sockets.Socket 选项适用于所有套接字。
        Socket = 65535,
    }

    // 摘要:
    //     指定 System.Net.Sockets.Socket 类的实例可以使用的寻址方案。
    internal enum AddressFamily
    {
        // 摘要:
        //     未知的地址族。
        Unknown = -1,
        //
        // 摘要:
        //     未指定的地址族。
        Unspecified = 0,
        //
        // 摘要:
        //     Unix 本地到主机地址。
        Unix = 1,
        //
        // 摘要:
        //     IP 版本 4 的地址。
        InterNetwork = 2,
        //
        // 摘要:
        //     ARPANET IMP 地址。
        ImpLink = 3,
        //
        // 摘要:
        //     PUP 协议的地址。
        Pup = 4,
        //
        // 摘要:
        //     MIT CHAOS 协议的地址。
        Chaos = 5,
        //
        // 摘要:
        //     IPX 或 SPX 地址。
        Ipx = 6,
        //
        // 摘要:
        //     Xerox NS 协议的地址。
        NS = 6,
        //
        // 摘要:
        //     OSI 协议的地址。
        Osi = 7,
        //
        // 摘要:
        //     ISO 协议的地址。
        Iso = 7,
        //
        // 摘要:
        //     欧洲计算机制造商协会 (ECMA) 地址。
        Ecma = 8,
        //
        // 摘要:
        //     Datakit 协议的地址。
        DataKit = 9,
        //
        // 摘要:
        //     CCITT 协议（如 X.25）的地址。
        Ccitt = 10,
        //
        // 摘要:
        //     IBM SNA 地址。
        Sna = 11,
        //
        // 摘要:
        //     DECnet 地址。
        DecNet = 12,
        //
        // 摘要:
        //     直接数据链接接口地址。
        DataLink = 13,
        //
        // 摘要:
        //     LAT 地址。
        Lat = 14,
        //
        // 摘要:
        //     NSC Hyperchannel 地址。
        HyperChannel = 15,
        //
        // 摘要:
        //     AppleTalk 地址。
        AppleTalk = 16,
        //
        // 摘要:
        //     NetBios 地址。
        NetBios = 17,
        //
        // 摘要:
        //     VoiceView 地址。
        VoiceView = 18,
        //
        // 摘要:
        //     FireFox 地址。
        FireFox = 19,
        //
        // 摘要:
        //     Banyan 地址。
        Banyan = 21,
        //
        // 摘要:
        //     本机 ATM 服务地址。
        Atm = 22,
        //
        // 摘要:
        //     IP 版本 6 的地址。
        InterNetworkV6 = 23,
        //
        // 摘要:
        //     Microsoft 群集产品的地址。
        Cluster = 24,
        //
        // 摘要:
        //     IEEE 1284.4 工作组地址。
        Ieee12844 = 25,
        //
        // 摘要:
        //     IrDA 地址。
        Irda = 26,
        //
        // 摘要:
        //     支持网络设计器 OSI 网关的协议的地址。
        NetworkDesigners = 28,
        //
        // 摘要:
        //     MAX 地址。
        Max = 29,
    }

    //public enum ProxyType
    //{
    //    None = 1000,
    //    /// <summary>
    //    /// http代理
    //    /// </summary>
    //    HttpProxy = 1001,
    //    /// <summary>
    //    /// Socket5代理
    //    /// </summary>
    //    Scoket5 = 1002,
    //    /// <summary>
    //    /// 系统代理
    //    /// </summary>
    //    SystemProxy=1003,
    //}

    #endregion

  

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
    internal delegate void ReceiveEventHandler(byte[] bytes,int bLen);
    #endregion
}
