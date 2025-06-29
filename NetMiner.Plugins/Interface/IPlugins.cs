using System;
using System.Collections.Generic;
using System.Text;
using System.Data ;

///网络矿工插件接口类，用于用户自定义插件使用，继承此接口。
namespace NetMiner.Plugins
{
    public interface IPlugins
    {
        /// <summary>
        /// 获取验证码文本内容接口
        /// </summary>
        /// <param name="imgName">传入验证码图片文件地址</param>
        /// <param name="imgUrl">传入验证码图片Url，并不是获取验证码图片，而是根据Url来识别验证码不同网站的类别</param>
        /// <param name="inCookie">访问图片地址时所需要的cookie</param>
        /// <returns>返回验证码文本</returns>
        string GetVerifyCode(string imgName,string imgUrl);

       /// <summary>
       /// 获取cookie数据
       /// </summary>
       /// <param name="Domain">域名地址，也可以直接是有效的网址</param>
       /// <returns>获取的cookie</returns>
        string GetCookie(string Domain,string TaskName);

        /// <summary>
        /// 将采集获取的数据通过接口进行发布操作
        /// </summary>
        /// <param name="d">需要发布的数据，DataTable类型</param>
        void PublishData(DataTable d);

        /// <summary>
        /// 对采集的数据进行加工处理
        /// </summary>
        /// <param name="d">需要加工的数据，DataTable类型</param>
        /// <returns>加工完成的数据，DataTable类型</returns>
        DataTable TransData(DataTable d);

        /// <summary>
        /// 设置插件属性，在此可以调用一个窗体进行插件信息的配置工作
        /// </summary>
        void Config();
    }
}
