using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents a lecturer.
    /// </summary>
    public record Lecturer : DbEntity
    {
        /// <summary>
        /// Gets or sets full name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets email address.
        /// </summary>
        [EmailAddress]
        [MaxLength(50)]
        public string? Email { get; set; }

        
        /// <summary>
        /// Gets or inits a collection of lecture courses.
        /// </summary>
        public IEnumerable<Course> Courses { get; init; }

        public virtual bool Equals(Lecturer? other)
        {
            return other is not null &&
                   Id == other.Id &&
                   FullName == other.FullName &&
                   Courses.SequenceEqual(other.Courses);
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), FullName, Courses);
    }
}
