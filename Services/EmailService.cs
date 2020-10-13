using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using stock_quote_alert.Abstractions;
using stock_quote_alert.Configurations;

namespace stock_quote_alert.Services
{
    public class EmailService : INotification
    {
        private readonly ServiceConfigurations _serviceConfigurations;

        public EmailService(IOptions<ServiceConfigurations> serviceConfigurations)
        {
            _serviceConfigurations = serviceConfigurations.Value;
        }

        public void send(string from, string to, string subject, string message)
        {
            var mailMessage = createMimeMessage(from, to, subject, message);

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(_serviceConfigurations.Smtp.Host, _serviceConfigurations.Smtp.Port);
                smtpClient.Authenticate(_serviceConfigurations.Smtp.Username, _serviceConfigurations.Smtp.Password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }

        private MimeMessage createMimeMessage(string emailFrom, string emailTo, string subject, string message)
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Inoa Sistemas", emailFrom));
            mailMessage.To.Add(new MailboxAddress(emailTo, emailTo));
            mailMessage.Subject = subject;
            mailMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            return mailMessage;
        }
    }
}
