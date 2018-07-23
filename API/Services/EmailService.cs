using System;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using Microsoft.Extensions.Configuration;

namespace API.Services
{
    public interface IEmailService
    {
        void SendMail(string subject, string body, string to = null);
    }

    public class EmailService : IEmailService
    {
        private string _username;
        private string _password;
        private string _supportMail;

        public EmailService(IConfiguration configuration)
        {
            _username = configuration["Mail:Username"];
            _password = configuration["Mail:Password"];
            _supportMail = configuration["Mail:SupportMail"];
        }

        public void SendMail(string subject, string body, string to = null)
        {
            if (String.IsNullOrEmpty(to))
            {
                to = _supportMail;
            }
            
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            string fromMail =  string.Format("{0}@{1}.{2}", "noreply", "elearning.physik.uni-frankfurt", "de");
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Host = "itp.uni-frankfurt.de";
            client.Credentials = new NetworkCredential(_username, _password);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromMail);
            mailMessage.To.Add(to);
            mailMessage.Body = body;
            mailMessage.Subject = "[PDFCreator] " + subject;
            client.Send(mailMessage);
        }
    }
}