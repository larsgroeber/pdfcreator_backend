using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace API.Services
{
    public class EmailService
    {
        public static void SendMail(string to, string subject, string body)
        {
            var ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            string fromMail =  string.Format("{0}@{1}.{2}", "noreply", ipProperties.HostName, ipProperties.DomainName);
            SmtpClient client = new SmtpClient();
            client.Host = "localhost";
            client.UseDefaultCredentials = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(fromMail);
            mailMessage.To.Add(to);
            mailMessage.Body = body;
            mailMessage.Subject = "[PDFCreator] " + subject;
            client.Send(mailMessage);
        }
    }
}