using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents a lecture.
    /// </summary>
    public record Lecture : DbEntity
    {
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; init; }

        /// <summary>
        /// Gets or sets id of course to which this lecture belongs.
        /// </summary>
        [Required]
        public int CourseId { get; init; }

        /// <summary>
        /// Gets or sets course to which this lecture belongs.
        /// </summary>
        [Required]
        public Course Course { get; init; }
        
        /// <summary>
        /// Gets or inits journal records associated with this lecture.
        /// </summary>
        public IEnumerable<JournalRecord> JournalRecords { get; init; }

        public virtual bool Equals(Lecture? other)
        {
            if (other is null) return false;
            return Id == other.Id &&
                   Name == other.Name &&
                   CourseId == other.CourseId &&
                   (ReferenceEquals(JournalRecords, other.JournalRecords) ||
                    JournalRecords.SequenceEqual(other.JournalRecords));
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Name, CourseId, Course, JournalRecords);
    }
}
