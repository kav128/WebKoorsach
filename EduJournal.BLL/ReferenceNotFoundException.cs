using System;

namespace EduJournal.BLL
{
    public class ReferenceNotFoundException : Exception
    {
        public ReferenceNotFoundException()
        {
        }

        public ReferenceNotFoundException(string? referenceName, string? message) : base(message)
        {
            ReferenceName = referenceName;
        }

        public ReferenceNotFoundException(string?  referenceName, string? message, Exception? innerException) : base(message, innerException)
        {
            ReferenceName = referenceName;
        }
        
        public string? ReferenceName { get; init; }
    }
}
