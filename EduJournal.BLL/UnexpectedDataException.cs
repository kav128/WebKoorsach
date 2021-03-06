using System;

namespace EduJournal.BLL
{
    public class UnexpectedDataException : Exception
    {
        public UnexpectedDataException()
        {
        }

        public UnexpectedDataException(string? message) : base(message)
        {
        }

        public UnexpectedDataException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}