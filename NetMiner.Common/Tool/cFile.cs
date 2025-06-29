using System;
using System.Collections.Generic;
using System.Text;
using System.IO ;
using System.Runtime.Serialization.Formatters.Binary;

namespace NetMiner.Common.Tool
{
    public class cFile
    {
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="strContent">文件内容，字符串</param>
        /// <param name="isCover">是否覆盖，true-覆盖，false-不覆盖，如果选择不覆盖文件，且已经存在此文件，则返回false</param>
        /// <returns>true-保存成功；false-保存失败</returns>
        public static bool SaveFileBinary(string fileName,string strContent,bool isCover)
        {
            if (isCover == false && File.Exists(fileName))
                return false;

            FileStream myStream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.Write);

            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(myStream, strContent);

            myStream.Close();

            myStream.Close();
            myStream.Dispose();

            return true;
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>返回文件字符串</returns>
        public static string ReadFileBinary(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new NetMinerException("任务文件不存在！");
            }
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            
            BinaryFormatter bf = new BinaryFormatter();
            string strContent = bf.Deserialize(fs) as string;
            
            fs.Close();
            fs.Dispose();
            return strContent;
        }
    }
}
