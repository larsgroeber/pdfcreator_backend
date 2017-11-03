using System.Net;
using System.Net.Mail;

namespace API.Services
{
    public class EmailService
    {
        public static void SendMail(string to, string subject, string body)
        {
            SmtpClient client = new SmtpClient("http://itp.uni-frankfurt.de");
            client.UseDefaultCredentials = true;

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("whoever@me.com");
            mailMessage.To.Add(to);
            mailMessage.Body = body;
            mailMessage.Subject = "[PDFCreator] " + subject;
            client.Send(mailMessage);
        }
    }
}