using System;
using System.Collections.Generic;
using System.Text;
using HtmlExtract.Entities;

namespace HtmlExtract
{
    public interface IDeal<T> where T : BasePart
    {
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="htmlFragment">Html块</param>
        void Process(HtmlFragment htmlFragment);
        /// <summary>
        /// 完成后处理
        /// </summary>
        /// <param name="?"></param>
        void ProcessEnd(params object[] args);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        T GetModel();
    }
}
