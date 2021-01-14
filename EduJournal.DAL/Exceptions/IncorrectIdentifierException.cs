using System;

namespace EduJournal.DAL.Exceptions
{
    public class IncorrectIdentifierException : ArgumentOutOfRangeException
    {
        public IncorrectIdentifierException()
        {
        }

        public IncorrectIdentifierException(string? message) : base(message)
        {
        }

        public IncorrectIdentifierException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
