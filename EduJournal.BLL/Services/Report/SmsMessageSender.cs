using Microsoft.Extensions.Logging;

namespace EduJournal.BLL.Services.Report
{
    internal class SmsMessageSender : IMessageSender
    {
        private readonly ILogger<SmsMessageSender> _logger;

        public SmsMessageSender(ILogger<SmsMessageSender> logger)
        {
            _logger = logger;
        }

        public void Send(string message, string address)
        {
            _logger.LogInformation($"Message '{message}' has sent to '{address}' by SMS");
        }
    }
}
