using System;

namespace EduJournal.BLL.Services.Report.Formats
{
    public class RegistrationNotSupportedException : Exception
    {
        public RegistrationNotSupportedException()
        {
        }

        public RegistrationNotSupportedException(string? message) : base(message)
        {
        }

        public RegistrationNotSupportedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}