using System;

namespace EduJournal.BLL
{
    public class IncorrectIdException : ArgumentOutOfRangeException
    {
        public IncorrectIdException()
        {
        }

        public IncorrectIdException(string? message) : base(null, message)
        {
        }

        public IncorrectIdException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}