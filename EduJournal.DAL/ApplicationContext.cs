using System.Linq;
using EduJournal.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace EduJournal.DAL
{
    public class ApplicationContext : DbContext
    {
        protected ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<JournalRecord> JournalRecords { get; set; }
        public virtual DbSet<Lecture> Lectures { get; set; }
        public virtual DbSet<Lecturer> Lecturers { get; set; }
        public virtual DbSet<Student> Students { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Student[] students = new[]
            {
                "Eric Jenkins",
                "Brian White",
                "Virginia Martinez",
                "Amy Johnson",
                "Terry Parker"
            }.Select((name, i) => new Student { FullName = name, Email = "testm10stud@mail.ru", Id = i + 1 }).ToArray();
            modelBuilder.Entity<Student>().HasData(students);

            Lecturer[] lecturers = new[]
            {
                "Patricia James",
                "Phillip Ross",
                "Paul Baker"
            }.Select((name, i) => new Lecturer { FullName = name, Email = "testm10lec@mail.ru", Id = i + 1 }).ToArray();
            modelBuilder.Entity<Lecturer>().HasData(lecturers);

            var courses = new[]
            {
                new Course { Id = 1, Name = "Chemistry", LecturerId = 2 },
                new Course { Id = 2, Name = "Math", LecturerId = 3 },
                new Course { Id = 3, Name = "English", LecturerId = 3 },
                new Course { Id = 4, Name = "Physics", LecturerId = 3 },
                new Course { Id = 5, Name = "Computer Science", LecturerId = 1 }
            };
            modelBuilder.Entity<Course>().HasData(courses);

            var lectureId = 0;
            Lecture[] lectures = courses.Zip(new[] { 4, 3, 3, 3, 4 })
                .SelectMany(courseTuple => Enumerable.Range(1, courseTuple.Second)
                    .Select(indexInCourse => new Lecture
                    {
                        Id = ++lectureId,
                        CourseId = courseTuple.First.Id,
                        Name = $"{courseTuple.First.Name}, lecture {indexInCourse}"
                    }))
                .ToArray();
            modelBuilder.Entity<Lecture>().HasData(lectures);

            modelBuilder.Entity<JournalRecord>().HasAlternateKey(record => new { record.LectureId, record.StudentId });
            modelBuilder.Entity<JournalRecord>()
                .HasCheckConstraint("CK_JournalRecords_Score", "[Attendance] = 0 AND [Score] = 0 OR [Attendance] = 1 AND [Score] BETWEEN 0 AND 5");
        }
    }
}
