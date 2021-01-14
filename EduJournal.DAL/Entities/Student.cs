using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents a student.
    /// </summary>
    public record Student : DbEntity
    {
        /// <summary>
        /// Gets or sets full name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; init; }

        /// <summary>
        /// Gets or sets email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(50)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or inits a collection of journal records associated with this student.
        /// </summary>
        public IEnumerable<JournalRecord> JournalRecords { get; init; }

        public virtual bool Equals(Student? other)
        {
            return other is not null &&
                   Id == other.Id &&
                   FullName == other.FullName &&
                   (ReferenceEquals(JournalRecords, other.JournalRecords) ||
                   JournalRecords.SequenceEqual(other.JournalRecords));
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), FullName, JournalRecords);
    }
}
