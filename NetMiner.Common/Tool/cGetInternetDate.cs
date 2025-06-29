using System;
using System.Collections.Generic;
using System.Text;
using System.Net;  
using System.Net.Sockets;  
using System.Runtime.InteropServices; 

namespace NetMiner.Common.Tool
{
    public struct SystemTime  
            {  
                public ushort wYear;  
                public ushort wMonth;  
                public ushort wDayOfWeek;  
                public ushort wDay;  
                public ushort wHour;  
                public ushort wMinute;  
                public ushort wSecond;  
               public ushort wMilliseconds;  
     
               /// <summary>  
               /// �� System.DateTimeת����  
               /// </summary>  
               /// <param name="time"& gt;System.DateTime���͵�ʱ�䡣</param>  
               public void FromDateTime(DateTime time)  
               {  
                   wYear = (ushort)time.Year;  
                   wMonth = (ushort)time.Month;  
                   wDayOfWeek = (ushort)time.DayOfWeek;  
                   wDay = (ushort)time.Day;  
                   wHour = (ushort)time.Hour;  
                   wMinute = (ushort)time.Minute;  
                   wSecond = (ushort)time.Second;  
                   wMilliseconds = (ushort)time.Millisecond;  
               }  
               /// <summary>  
               /// ת��ΪSystem.DateTime ���͡�  
               /// </summary>  
               /// <returns></returns>  
               public DateTime ToDateTime()  
               {  
                   return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds);  
               }  
               /// <summary>  
               /// ��̬������ת��Ϊ System.DateTime���͡�  
               /// </summary>  
               /// <param name="time"& gt;SYSTEMTIME���͵�ʱ�䡣</param>  
               /// <returns></returns>  
               public static DateTime ToDateTime(SystemTime time)  
               {  
                   return time.ToDateTime();  
               }  
           }  

    public class cGetInternetDate
    {
        public cGetInternetDate()
        {
        }

        ~cGetInternetDate()
        {
        }

        public DateTime GetInternetTime()
        {
            // ��¼��ʼ��ʱ��
            DateTime startDT = DateTime.Now;

            //����IPAddress������˿ڣ�����IPEndPoint�ڵ�:  
            int port = 13;
            string[] whost = { "5time.nist.gov", "time-nw.nist.gov", "time-a.nist.gov", "time-b.nist.gov", "tick.mit.edu", "time.windows.com", "clock.sgi.com" };
            
            DateTime SetDT;

            try
            {
                IPHostEntry iphostinfo;
                IPAddress ip;
                IPEndPoint ipe;
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//����Socket  

                c.ReceiveTimeout = 10 * 1000;//���ó�ʱʱ��  

                string sEX = "";// ���ܴ�����Ϣ  

                // ����ʱ��������б�  
                foreach (string strHost in whost)
                {
                    try
                    {
                        iphostinfo = Dns.GetHostEntry(strHost);
                        ip = iphostinfo.AddressList[0];
                        ipe = new IPEndPoint(ip, port);

                        c.Connect(ipe);//���ӵ�������  
                        if (c.Connected) break;// ������ӵ�������������  
                    }
                    catch (Exception ex)
                    {
                        sEX = ex.Message;
                    }
                }

                if (!c.Connected)
                {
                    return DateTime.Now;
                }

                //SOCKETͬ����������  
                byte[] RecvBuffer = new byte[1024];
                int nBytes, nTotalBytes = 0;
                StringBuilder sb = new StringBuilder();
                System.Text.Encoding myE = Encoding.UTF8;

                while ((nBytes = c.Receive(RecvBuffer, 0, 1024, SocketFlags.None)) > 0)
                {
                    nTotalBytes += nBytes;
                    sb.Append(myE.GetString(RecvBuffer, 0, nBytes));
                }

                //�ر�����  
                c.Close();

                string[] o = sb.ToString().Split(' '); // ����ַ���  

                //string CurrentDateTime = sb.ToString();  

                TimeSpan k = new TimeSpan();
                k = (TimeSpan)(DateTime.Now - startDT);// �õ���ʼ�����������ĵ�ʱ��  

                SetDT = Convert.ToDateTime(o[1] + " " + o[2]).Subtract(-k);// ��ȥ��;���ĵ�ʱ��  

                //���ñ���ʱ�� +8ʱ  
                SetDT = SetDT.AddHours(8);

                //ת��System.DateTime��SystemTime  
                SystemTime st = new SystemTime();
                st.FromDateTime(SetDT);
            }
            catch (System.Exception)
            {
                SetDT = DateTime.Now;
            }

            return SetDT;

        }

    }
}
