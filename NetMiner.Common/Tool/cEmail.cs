using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;

namespace NetMiner.Common.Tools
{
    public class cEmail
    {
        public cEmail()
        {

        }

        ~cEmail()
        {

        }

        private string m_Email;
        public string Email
        {
            get { return m_Email; }
            set { m_Email = value; }
        }

        private string m_User;
        public string User
        {
            get { return m_User; }
            set { m_User = value; }
        }

        private string m_Pwd;
        public string Pwd
        {
            get { return m_Pwd; }
            set { m_Pwd = value; }
        }

        private string m_PopServer;
        public string PopServer
        {
            get { return m_PopServer; }
            set { m_PopServer = value; }
        }

        private string m_Port;
        public string Port
        {
            get { return m_Port; }
            set { m_Port = value; }
        }

        private bool m_IsPopVerfy;
        public bool IsPopVerfy
        {
            get { return m_IsPopVerfy; }
            set { m_IsPopVerfy = value; }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        private string m_Content;
        public string Content
        {
            get { return m_Content; }
            set { m_Content = value; }
        }

        private string m_ReceiveEmail;
        public string ReceiveEmail
        {
            get { return m_ReceiveEmail; }
            set { m_ReceiveEmail = value; }
        }

        //public void SendMail()
        //{
        //    try
        //    {
        //        //�����������ַ���ռ��������ַ���ʼ����⣬�ʼ�����
        //        MailMessage myMail = new MailMessage(this.Email, this.Email, this.Title, this.Content);

        //        myMail.SubjectEncoding = System.Text.Encoding.UTF8;//�ʼ�����ı���
        //        myMail.BodyEncoding = System.Text.Encoding.UTF8;//�ʼ����ݵı���
        //        myMail.IsBodyHtml = true;//�ʼ�������ΪHtml��ʽ
        //        SmtpClient client = new SmtpClient("smtp.163.com",25);//ָ��SMTP������
        //        client.Credentials = new NetworkCredential(this.Email, this.Pwd);//ָ���û���������
        //        client.EnableSsl = false;//�Ƿ��������
        //        client.Send(myMail);//����ָ���ʼ�

        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw ex;
        //    }
        //} 


        public void SendMail()
        {
            try
            {
                //ȷ��smtp��������ַ��ʵ����һ��Smtp�ͻ���

                //����һ�����͵�ַ
                string strFrom = string.Empty;

                strFrom = this.Email;

                //����һ�������˵�ַ����
                MailAddress from = new MailAddress(strFrom, this.Email, Encoding.UTF8);
                //����һ���ռ��˵�ַ����
                MailAddress to = new MailAddress(this.ReceiveEmail, this.ReceiveEmail, Encoding.UTF8);

                //����һ��Email��Message����
                MailMessage message = new MailMessage(from, to);

                //����ʼ����������
                message.Subject = this.Title;
                message.SubjectEncoding = Encoding.UTF8;
                message.Body =this.Content;
                message.BodyEncoding = Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;


                SmtpClient client = new SmtpClient(this.PopServer,int.Parse ( this.Port));
                client.EnableSsl = false;
                
                client.UseDefaultCredentials = false;


                //�����û��������롣
                string userState = message.Subject;
                client.UseDefaultCredentials = false;
                string username = this.User ;
                string passwd = this.Pwd;

                if (this.IsPopVerfy == true)
                {
                    //�û���½��Ϣ
                    NetworkCredential myCredentials = new NetworkCredential(username, passwd);
                    client.Credentials = myCredentials;
                }
                
                //�����ʼ�����Ϣ
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                //�����ʼ�
                client.Send(message);
                //��ʾ���ͳɹ�

                client = null;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
