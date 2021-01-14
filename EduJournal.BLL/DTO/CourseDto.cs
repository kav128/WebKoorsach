#nullable enable
using System;
using System.Linq;

namespace EduJournal.BLL.DTO
{
    public record CourseDto
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public int LecturerId { get; init; }
        public int[] LectureIds { get; init; }

        public virtual bool Equals(CourseDto? other)
        {
            return other is not null &&
                   Id == other.Id &&
                   Name == other.Name &&
                   LecturerId == other.LecturerId &&
                   LectureIds.SequenceEqual(other.LectureIds);
        }

        public override int GetHashCode() => HashCode.Combine(Id, Name, LecturerId, LectureIds);
    }
}
