using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents a lecture course.
    /// </summary>
    public sealed record Course : DbEntity
    {
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; init; }

        /// <summary>
        /// Gets or sets lecturer id.
        /// </summary>
        [Required]
        public int LecturerId { get; init; }
        
        /// <summary>
        /// Gets or sets lecturer.
        /// </summary>
        [Required]
        public Lecturer Lecturer { get; init; }
        
        
        /// <summary>
        /// Gets or inits a collection of lectures.
        /// </summary>
        public IEnumerable<Lecture>? Lectures { get; init; }

        public bool Equals(Course? other)
        {
            return other is not null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   LecturerId == other.LecturerId &&
                   (Lectures ?? Array.Empty<Lecture>()).SequenceEqual(other.Lectures ?? Array.Empty<Lecture>());
        }

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Name, LecturerId, Lecturer, Lectures);
    }
}
