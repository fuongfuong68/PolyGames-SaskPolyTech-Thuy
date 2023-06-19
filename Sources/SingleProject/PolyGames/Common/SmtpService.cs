using System.Net.Mail;
using System.Net;

namespace PolyGames.Common
{
    public class SmtpService
    {
        private readonly SmtpConfiguration _smtpConfiguration;

        public SmtpService(SmtpConfiguration smtpConfiguration) {
            _smtpConfiguration = smtpConfiguration;
        }

        public void Send(string toAddress, string subject, string body)
        {
            if (!_smtpConfiguration.Enable)
                return;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_smtpConfiguration.Username);
            mail.To.Add(toAddress);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            using (SmtpClient smtpClient = new SmtpClient(_smtpConfiguration.Server, _smtpConfiguration.Port))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password);

                smtpClient.Send(mail);
            }
        }
    }
}