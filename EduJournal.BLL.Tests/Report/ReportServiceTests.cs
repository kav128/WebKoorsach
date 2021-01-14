using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduJournal.BLL.DTO;
using EduJournal.BLL.Services.Entity;
using EduJournal.BLL.Services.Report;
using Moq;
using NUnit.Framework;

namespace EduJournal.BLL.Tests.Report
{
    public class ReportServiceTests
    {
        #region GetReportByLecture

        public static IEnumerable<IEnumerable<object>> GetReportByLectureTestCaseGenerator()
        {
            yield return new object[]
            {
                new LectureDto { Id = 1, Name = "Physics, Lecture 1", CourseId = 1 },
                new JournalRecordDto[]
                {
                    new() { StudentId = 1, LectureId = 1, Attendance = true, Score = 5 },
                    new() { StudentId = 2, LectureId = 1, Attendance = true, Score = 4 },
                    new() { StudentId = 3, LectureId = 1, Attendance = false, Score = 0 }
                },
                new CourseDto { Id = 1, Name = "Physics", LecturerId = 1, LectureIds = new[] { 1, 2, 3 } },
                new StudentDto[]
                {
                    new() { Id = 1, FullName = "Isaac Newton" },
                    new() { Id = 2, FullName = "Albert Einstein" },
                    new() { Id = 3, FullName = "Nikola Tesla" }
                },
                new ReportData
                {
                    Header = new ReportHeader
                    {
                        Course = "Physics",
                        Lecture = "Physics, Lecture 1"
                    },
                    Records = new ReportRecord[]
                    {
                        new() { Student = "Isaac Newton", Attendance = true, Score = 5, Lecture = null },
                        new() { Student = "Albert Einstein", Attendance = true, Score = 4, Lecture = null },
                        new() { Student = "Nikola Tesla", Attendance = false, Score = 0, Lecture = null }
                    },
                    AttendancePercentage = 200.0 / 3,
                    AverageScore = null
                }
            };
        }

        [TestCaseSource(nameof(GetReportByLectureTestCaseGenerator))]
        public async Task GetReportByLectureTest(LectureDto lecture, IList<JournalRecordDto> journalRecords, CourseDto course,
            IList<StudentDto> students, ReportData expected)
        {
            // Arrange
            var journalServiceMock = new Mock<IJournalService>();
            journalServiceMock.Setup(service => service.GetRecords(lecture.Id, 0, 0))
                .ReturnsAsync(journalRecords)
                .Verifiable();
            var studentServiceMock = new Mock<IStudentService>();
            studentServiceMock.Setup(service => service.GetStudentsByIds(It.Is<IList<int>>(ints => ints.OrderBy(i => i)
                    .SequenceEqual(students.Select(dto => dto.Id)
                        .OrderBy(i => i)))))
                .ReturnsAsync(students)
                .Verifiable();
            var courseServiceMock = new Mock<ICourseService>();
            courseServiceMock.Setup(service => service.GetCourse(lecture.CourseId)).ReturnsAsync(course);
            var lectureServiceMock = new Mock<ILectureService>();
            var reportService = new ReportService(journalServiceMock.Object, studentServiceMock.Object, lectureServiceMock.Object, courseServiceMock.Object);

            // Act
            ReportData report = await reportService.GetReportByLecture(lecture);

            // Assert
            Assert.That(report, Is.EqualTo(expected));
            journalServiceMock.Verify();
            journalServiceMock.VerifyNoOtherCalls();
            studentServiceMock.Verify();
            studentServiceMock.VerifyNoOtherCalls();
        }

        #endregion
        
        #region GetReportByStudent

        public static IEnumerable<IEnumerable<object>> GetReportByStudentTestCaseGenerator()
        {
            yield return new object[]
            {
                new JournalRecordDto[]
                {
                    new() { StudentId = 1, LectureId = 1, Attendance = true, Score = 5 },
                    new() { StudentId = 1, LectureId = 2, Attendance = true, Score = 4 },
                    new() { StudentId = 1, LectureId = 3, Attendance = false, Score = 0 }
                },
                new StudentDto { Id = 1, FullName = "Isaac Newton" },
                new CourseDto { Id = 1, Name = "Physics", LecturerId = 1, LectureIds = new[] { 1, 2, 3 } },
                new LectureDto[]
                {
                    new() { Id = 1, Name = "Physics, Lecture 1", CourseId = 1 },
                    new() { Id = 2, Name = "Physics, Lecture 2", CourseId = 1 },
                    new() { Id = 3, Name = "Physics, Lecture 3", CourseId = 1 }
                },
                new ReportData
                {
                    Header = new ReportHeader
                    {
                        Student = "Isaac Newton",
                        Course = "Physics"
                    },
                    Records = new ReportRecord[]
                    {
                        new() { Lecture = "Physics, Lecture 1", Attendance = true, Score = 5 },
                        new() { Lecture = "Physics, Lecture 2", Attendance = true, Score = 4 },
                        new() { Lecture = "Physics, Lecture 3", Attendance = false, Score = 0 }
                    },
                    AverageScore = 3.0,
                    AttendancePercentage = 200.0 / 3
                }
            };
        }

        [TestCaseSource(nameof(GetReportByStudentTestCaseGenerator))]
        public async Task GetReportByStudentTest(IList<JournalRecordDto> journalRecords, StudentDto student,
            CourseDto course, IList<LectureDto> lectures, ReportData expected)
        {
            // Arrange
            var lectureServiceMock = new Mock<ILectureService>();
            lectureServiceMock.Setup(service => service.GetAllByCourse(course.Id))
                .ReturnsAsync(lectures)
                .Verifiable();
            var journalServiceMock = new Mock<IJournalService>();
            journalServiceMock.Setup(service => service.GetRecords(0, student.Id, course.Id))
                .ReturnsAsync(journalRecords)
                .Verifiable();
            var courseServiceMock = new Mock<ICourseService>();
            var studentServiceMock = new Mock<IStudentService>();
            var reportService = new ReportService(journalServiceMock.Object, studentServiceMock.Object, lectureServiceMock.Object, courseServiceMock.Object);

            // Act
            ReportData report = await reportService.GetReportByStudent(student, course);

            // Assert
            Assert.That(report, Is.EqualTo(expected));
        }

        #endregion
    }
}
