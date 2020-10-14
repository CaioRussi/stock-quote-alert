using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using stock_quote_alert.Abstractions;
using stock_quote_alert.Configurations;
using MimeKit;
using MailKit.Net.Smtp;

namespace stock_quote_alert.Services
{
    public class EmailService : INotification
    {
        private readonly ServiceConfigurations _serviceConfigurations;

        public EmailService(IOptions<ServiceConfigurations> serviceConfigurations)
        {
            _serviceConfigurations = serviceConfigurations.Value;

            ValidateEmails();
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

        private void ValidateEmails()
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                RegexOptions.CultureInvariant | RegexOptions.Singleline);
            
            if (!regex.IsMatch(_serviceConfigurations.Smtp.EmailFrom))
            {
                throw new ArgumentException("The EmailFrom is invalid in appsettings.json");
            }
            
            if (!regex.IsMatch(_serviceConfigurations.Smtp.EmailTo))
            {
                throw new ArgumentException("The EmailTo is invalid in appsettings.json");
            }
        }
    }
}
