using System;
using System.Linq;

namespace EduJournal.BLL.DTO
{
    public record LecturerDto
    {
        public int Id { get; init; }
        public string FullName { get; init; }
        public int[] CourseIds { get; init; }
        public string Email { get; init; }

        public virtual bool Equals(LecturerDto other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && FullName == other.FullName && CourseIds.SequenceEqual(other.CourseIds);
        }

        public override int GetHashCode() => HashCode.Combine(Id, FullName, CourseIds);
    }
}
