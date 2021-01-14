using System;

namespace EduJournal.BLL.Services.Report.Formats
{
    public class FormatterNotFoundException : Exception
    {
        public FormatterNotFoundException()
        {
        }

        public FormatterNotFoundException(string? message) : base(message)
        {
        }

        public FormatterNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}