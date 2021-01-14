using System;

namespace EduJournal.BLL.Services.Report.Formats
{
    public class DuplicatedFormatterException : Exception
    {
        public DuplicatedFormatterException()
        {
        }

        public DuplicatedFormatterException(string? message) : base(message)
        {
        }

        public DuplicatedFormatterException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}