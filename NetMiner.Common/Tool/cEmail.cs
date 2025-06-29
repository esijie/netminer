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
        //        //发件人邮箱地址，收件人邮箱地址，邮件标题，邮件内容
        //        MailMessage myMail = new MailMessage(this.Email, this.Email, this.Title, this.Content);

        //        myMail.SubjectEncoding = System.Text.Encoding.UTF8;//邮件标题的编码
        //        myMail.BodyEncoding = System.Text.Encoding.UTF8;//邮件内容的编码
        //        myMail.IsBodyHtml = true;//邮件的内容为Html格式
        //        SmtpClient client = new SmtpClient("smtp.163.com",25);//指定SMTP服务器
        //        client.Credentials = new NetworkCredential(this.Email, this.Pwd);//指定用户名和密码
        //        client.EnableSsl = false;//是否加密连接
        //        client.Send(myMail);//发送指定邮件

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
                //确定smtp服务器地址。实例化一个Smtp客户端

                //生成一个发送地址
                string strFrom = string.Empty;

                strFrom = this.Email;

                //构造一个发件人地址对象
                MailAddress from = new MailAddress(strFrom, this.Email, Encoding.UTF8);
                //构造一个收件人地址对象
                MailAddress to = new MailAddress(this.ReceiveEmail, this.ReceiveEmail, Encoding.UTF8);

                //构造一个Email的Message对象
                MailMessage message = new MailMessage(from, to);

                //添加邮件主题和内容
                message.Subject = this.Title;
                message.SubjectEncoding = Encoding.UTF8;
                message.Body =this.Content;
                message.BodyEncoding = Encoding.UTF8;
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;


                SmtpClient client = new SmtpClient(this.PopServer,int.Parse ( this.Port));
                client.EnableSsl = false;
                
                client.UseDefaultCredentials = false;


                //设置用户名和密码。
                string userState = message.Subject;
                client.UseDefaultCredentials = false;
                string username = this.User ;
                string passwd = this.Pwd;

                if (this.IsPopVerfy == true)
                {
                    //用户登陆信息
                    NetworkCredential myCredentials = new NetworkCredential(username, passwd);
                    client.Credentials = myCredentials;
                }
                
                //设置邮件的信息
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                //发送邮件
                client.Send(message);
                //提示发送成功

                client = null;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
