using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EduJournal.BLL.Services.Report
{
    internal class EmailMessageSender : IMessageSender
    {
        private readonly ILogger<EmailMessageSender> _logger;
        private readonly SmtpClient _smtpClient;
        private readonly string _senderMail;

        public EmailMessageSender(ILogger<EmailMessageSender> logger, IConfiguration configuration)
        {
            _logger = logger;

            string smtpHost = configuration["SmtpHost"];
            int smtpPort = int.Parse(configuration["SmtpPort"]);
            string smtpLogin = configuration["SmtpLogin"];
            string smtpPassword = configuration["SmtpPassword"];
            _senderMail = configuration["SmtpSender"];

            _smtpClient = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpLogin, smtpPassword),
                EnableSsl = true
            };
        }

        public void Send(string message, string address)
        {
            
            var from = new MailAddress(_senderMail, "M10");
            var to = new MailAddress(address);

            var mailMessage = new MailMessage(from, to)
            {
                Subject = "M10 email notification",
                Body = message
            };

            try
            {
                _smtpClient.Send(mailMessage);
                _logger.LogInformation($"Message '{message}' has sent to '{address}' by email");
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, $"Failed to send '{message}' to '{address}' by email");
            }
        }
    }
}
