namespace EduJournal.BLL.Services.Report
{
    public interface IMessageSenderFactory
    {
        public IMessageSender GetEmailSender();
        public IMessageSender GetSmsSender();
    }
}
