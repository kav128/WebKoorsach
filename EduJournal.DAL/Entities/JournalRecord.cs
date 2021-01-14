using System;
using System.ComponentModel.DataAnnotations;

namespace EduJournal.DAL.Entities
{
    /// <summary>
    /// Represents a record of attendance and academic progress journal.
    /// </summary>
    public record JournalRecord : DbEntity
    {
        /// <summary>
        /// Gets or sets whether student attended a lecture or not.
        /// </summary>
        [Required]
        public bool Attendance { get; init; }
        
        /// <summary>
        /// Gets or sets homework score.
        /// </summary>
        [Required]
        public int Score { get; init; }


        /// <summary>
        /// Gets or sets student id.
        /// </summary>
        [Required]
        public int StudentId { get; init; }

        /// <summary>
        /// Gets or sets student.
        /// </summary>
        [Required]
        public Student Student { get; init; }

        /// <summary>
        /// Gets or sets lecture id.
        /// </summary>
        [Required]
        public int LectureId { get; init; }

        /// <summary>
        /// Gets or sets lecture.
        /// </summary>
        [Required]
        public Lecture Lecture { get; init; }

        public virtual bool Equals(JournalRecord? other) =>
            other is not null &&
            Attendance == other.Attendance &&
            Score == other.Score &&
            LectureId == other.LectureId &&
            StudentId == other.StudentId;

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Attendance, Score, StudentId, LectureId);
    }
}
