namespace EduJournal.BLL.Services.Report
{
    public interface IMessageSender
    {
        public void Send(string message, string address);
    }
}
