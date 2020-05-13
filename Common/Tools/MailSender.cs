using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Common.Tools
{
    public class MailSender
    {
        private string smtpServer;
        private int smtpPort;
        private string smtpEmail;
        private string smtpPassword;
        private bool smtpEnableSsl;
        public MailSender(string smtpServer, int smtpPort, string smtpEmail, string smtpPassword, bool smtpEnableSsl)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            this.smtpEmail = smtpEmail;
            this.smtpPassword = smtpPassword;
            this.smtpEnableSsl = smtpEnableSsl;
        }

        public  void SendEmail(string to, string cc, string bcc, string subject, string body, bool isHtml)
        {

            MailMessage mail = new MailMessage(smtpEmail, to);
            mail.IsBodyHtml = isHtml;
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(smtpEmail, smtpPassword);
            smtpClient.EnableSsl = smtpEnableSsl;
            smtpClient.Send(mail);

            //https://myaccount.google.com/lesssecureapps

        }
    }
}
