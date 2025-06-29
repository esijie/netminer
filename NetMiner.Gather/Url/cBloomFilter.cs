using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using NetMiner.Common;

namespace NetMiner.Gather.Url
{
    public class cBloomFilter
    {
        private static int[] MemoryPool;  //分配int数组原素为262144则整个数组占用1M

        private static int MaxNum;        //平均分配8块每块的元素数32768

        private MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();

        private SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();
        private int m_Size = 262144;
        private string m_workPath = string.Empty;

        public cBloomFilter(string workPath)  //分配内存大小 必须能整除8
        {
            m_workPath = workPath;

            if (File.Exists(m_workPath + "monitor\\urlData.db"))
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream(m_workPath + "monitor\\urlData.db", FileMode.Open, FileAccess.Read, FileShare.Read);

                    if (stream.Length == 0)
                    {
                        //字符串为空
                        MemoryPool = new int[m_Size];
                    }
                    else
                    {
                        MemoryPool = (int[])formatter.Deserialize(stream);
                    }
                    stream.Close();
                }
                catch
                {
                    //如果打开出错，则删除重新建立
                    File.Delete(m_workPath + "monitor\\urlData.db");
                    if ((m_Size % 8) == 0)
                    {
                        MemoryPool = new int[m_Size];

                    }
                }
            }
            else
            {
                if ((m_Size % 8) == 0)
                {
                    MemoryPool = new int[m_Size];
                    
                }
            }

            MaxNum = m_Size / 8;
        }

        private string Md5Decode(string str)
        {
            return BitConverter.ToString(Md5.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
        }

        private string Sha1Decode(string str)
        {
            return BitConverter.ToString(Sha1.ComputeHash(Encoding.Default.GetBytes(str))).Replace("-", "");
        }

        private static bool PushInMemoryMd5(string url, int wz)   //对应内存位置置1,如4个对应位置均已经被置1则返回false
        {
            int tmp = 0;
            int k = 0;
            for (int i = 0; i < 16; i = i + 4)
            {
                string str = url.ToString().Substring(i, 4);
                int Hex10=Convert.ToInt32(str,16);
                int yy = Hex10 % MaxNum;
                if (MemoryPool[wz + yy] == 0)
                    MemoryPool[wz + yy] = 1;
                else
                    tmp = tmp + 1;
                k++;
                wz = wz + MaxNum;

            }
            if (tmp == 4)
                return false;
            return true;
        }

        private static bool PushInMemorySha1(string url, int wz) //对应内存位置置1,如4个对应位置均已经被置1则返回false
        {
            int tmp = 0;
            int k = 0;
            for (int i = 0; i < 16; i = i + 4)
            {
                string str = url.ToString().Substring(i, 7);
                int Hex10 = Convert.ToInt32(str, 16);
                int yy = Hex10 % MaxNum;
                if (MemoryPool[wz + yy] == 0)
                    MemoryPool[wz + yy] = 1;
                else
                    tmp = tmp + 1;
                k++;
                wz = wz + MaxNum;

            }
            if (tmp == 4)
                return false;
            return true;
        }

        public bool IsUrl(string url)
        {
            int wz = 0;
            string Md5Url = Md5Decode(url);
            bool Result1 = PushInMemoryMd5(Md5Url,wz);
            Md5Url = Md5Decode(Md5Url);
            bool Result2 = PushInMemorySha1(Md5Url,wz);
            if ((Result1 == false) && (Result1 == false))
                return true;                              //存在重复
            return false;                                //不重复
        }

        ~cBloomFilter() 
        {
            //存储整形数据，进行url排重使用

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(m_workPath + "monitor\\urlData.db", FileMode.Create, FileAccess.Write, FileShare.Write);
            formatter.Serialize(stream, MemoryPool);
            stream.Close();


            MemoryPool = null;
            Md5 = null;
            Sha1 = null;
        }
    }
}
