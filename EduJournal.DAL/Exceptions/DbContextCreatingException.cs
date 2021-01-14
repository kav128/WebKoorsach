using System;

namespace EduJournal.DAL.Exceptions
{
    public class DbContextCreatingException : Exception
    {
        public DbContextCreatingException()
        {
        }
        
        public DbContextCreatingException(string? message) : base(message)
        {
        }

        public DbContextCreatingException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
