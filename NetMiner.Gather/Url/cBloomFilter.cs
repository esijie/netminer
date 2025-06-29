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
        private static int[] MemoryPool;  //����int����ԭ��Ϊ262144����������ռ��1M

        private static int MaxNum;        //ƽ������8��ÿ���Ԫ����32768

        private MD5CryptoServiceProvider Md5 = new MD5CryptoServiceProvider();

        private SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();
        private int m_Size = 262144;
        private string m_workPath = string.Empty;

        public cBloomFilter(string workPath)  //�����ڴ��С ����������8
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
                        //�ַ���Ϊ��
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
                    //����򿪳�����ɾ�����½���
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

        private static bool PushInMemoryMd5(string url, int wz)   //��Ӧ�ڴ�λ����1,��4����Ӧλ�þ��Ѿ�����1�򷵻�false
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

        private static bool PushInMemorySha1(string url, int wz) //��Ӧ�ڴ�λ����1,��4����Ӧλ�þ��Ѿ�����1�򷵻�false
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
                return true;                              //�����ظ�
            return false;                                //���ظ�
        }

        ~cBloomFilter() 
        {
            //�洢�������ݣ�����url����ʹ��

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
