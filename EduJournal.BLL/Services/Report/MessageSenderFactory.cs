using EduJournal.BLL.Services.Report.Formats;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EduJournal.BLL.Services.Report
{
    class MessageSenderFactory : IMessageSenderFactory
    {
        private readonly ILogger<EmailMessageSender> _emailLogger;
        private readonly ILogger<SmsMessageSender> _smsLogger;
        private readonly IConfiguration _configuration;

        public MessageSenderFactory(IFormatterManager formatterManager,
            ILogger<EmailMessageSender> emailLogger,
            ILogger<SmsMessageSender> smsLogger,
            IConfiguration configuration)
        {;
            _emailLogger = emailLogger;
            _smsLogger = smsLogger;
            _configuration = configuration;
        }

        public IMessageSender GetEmailSender() => new EmailMessageSender(_emailLogger, _configuration);

        public IMessageSender GetSmsSender() => new SmsMessageSender(_smsLogger);
    }
}
